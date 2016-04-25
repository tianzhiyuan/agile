using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using Agile.Common.Components;
using Agile.Common.Logging;
using Agile.Common.Message;
using Agile.Common.Serialization;
using Agile.Common.Utils;

namespace LLY.Core.LiCai.Message
{
    /// <summary>
    /// 基于 <see cref="BlockingCollection"/> 的异步消息监听器
    /// 注意在IIS或者其他宿主中，会有各种原因导致Appdomain被unload，从而引发ThreadAbort，
    /// 因此这里的异步线程是不安全的，可能丢失
    /// 另外listeners 不可以过于消耗时间，会导致worker线程不够，阻塞主处理线程
    /// </summary>
    [Component]
    public sealed class DefaultMessageBus : IMessageBus, IDisposable, IRegisteredObject
    {
        private readonly IDictionary<Type, IList<IListener>> _mapping = new ConcurrentDictionary<Type, IList<IListener>>();
        private readonly ILogger _logger;
        private readonly IJsonSerializer _json;
        private const int BUFFER_SIZE = 1000;
        private BlockingCollection<object> _messages;
        private Task[] _workers;
        private readonly int _workerSize = Environment.ProcessorCount;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _token;
        private readonly TimeSpan _shutdownFlushTimespan = TimeSpan.FromSeconds(5);

        public DefaultMessageBus(ILoggerFactory factory, IJsonSerializer json)
        {
            _logger = factory.Create(typeof (DefaultMessageBus));
            _json = json;
            InitWorkers();
        }
        public void Subscribe(IListener listener, Type messageType)
        {
            if (!_mapping.ContainsKey(messageType))
            {
                _mapping[messageType] = new List<IListener>();
            }
            _mapping[messageType].Add(listener);
        }

        public void Post<TMessage>(TMessage message) where TMessage : class
        {
            var guid = Guid.NewGuid().ToString().Replace("-", string.Empty) + " : ";
            if (message == null)
            {
                _logger.Debug(guid + "message is null");
                return;
            }
            var messageType = typeof (TMessage);
            _logger.InfoFormat("{0}received message type: {1}. message:{2}", guid, messageType, _json.Serialize(message));

            var wrapper = new MessageWrapper()
            {
                Id = guid,
                Message = message,
                MessageType = messageType
            };
            _messages.Add(wrapper, _token);
        }

        private void InitWorkers()
        {
            _messages = new BlockingCollection<object>(BUFFER_SIZE);
            _cancellationTokenSource = new CancellationTokenSource();
            _token = _cancellationTokenSource.Token;

            _workers = new Task[_workerSize];
            for (var i = 0; i < _workerSize; i++)
            {
                _workers[i] = new Task(WorkLoop, _token);
                _workers[i].Start();
            }
        }

        private void WorkLoop()
        {
            Thread.CurrentThread.Name = "Event handle worker";
            try
            {
                foreach (var entry in _messages.GetConsumingEnumerable(_token))
                {
                    var wrapper = entry as MessageWrapper;
                    if (wrapper == null)
                    {
                        continue;
                    }
                    HandleMessage(wrapper);
                }
            }
            catch (ThreadAbortException error)
            {
                CompleteHandle();
            }
            catch (System.Exception error)
            {
                CompleteHandle();
                _logger.Error("", error);
            }
        }

        private void HandleMessage(MessageWrapper wrapper)
        {
            IList<IListener> listeners;
            
            if (_mapping.TryGetValue(wrapper.MessageType, out listeners) && CollectionUtils.NotEmpty(listeners))
            {
                var exceptions = new List<System.Exception>();
                foreach (IListener listener in listeners)
                {
                    try
                    {
                        _logger.DebugFormat("{0}try handle message by listernType:{1}", wrapper.Id, wrapper.MessageType);
                        listener.Handle(wrapper.Message);
                    }
                    catch (System.Exception error)
                    {
                        exceptions.Add(error);
                        _logger.Error($"{wrapper.Id}error when handle.", error);
                    }
                }
            }
            else
            {
                _logger.DebugFormat("{0}no listeners registered", wrapper.Id);
            }
        }

        private void CompleteHandle()
        {
            if (_messages == null || _messages.IsAddingCompleted)
            {
                return;
            }
            _messages.CompleteAdding();
            Thread.Sleep(_shutdownFlushTimespan);
            if (!_messages.IsCompleted & !_token.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
                if (_workers != null)
                {
                    foreach (var worker in _workers)
                    {
                        worker.Wait();
                    }
                }
            }
            
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed = false;
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (CollectionUtils.NotEmpty(_workers))
                    {
                        CompleteHandle();
                        foreach (var worker in _workers)
                        {
                            try
                            {
                                worker.Dispose();
                            }
                            catch
                            {
                                
                            }
                        }
                        _workers = null;
                    }
                    if (_messages != null)
                    {
                        try
                        {
                            _messages.Dispose();
                        }
                        catch
                        {
                            
                        }
                        _messages = null;
                    }
                    if (_cancellationTokenSource != null)
                    {
                        try
                        {
                            _cancellationTokenSource.Dispose();
                        }
                        catch
                        {
                            
                        }
                        _cancellationTokenSource = null;
                    }
                }
                _disposed = true;
            }
        }

        ~DefaultMessageBus()
        {
            Dispose(false);
        }

        public void Stop(bool immediate)
        {
            CompleteHandle();
        }
    }

    

    class MessageWrapper
    {
        internal object Message { get; set; }
        internal string Id { get; set; }
        internal Type MessageType { get; set; }
    }
}
