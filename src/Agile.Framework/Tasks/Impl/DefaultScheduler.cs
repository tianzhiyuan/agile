using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Agile.Common.Exceptions;
using Agile.Common.Logging;
using Agile.Framework.Properties;


namespace Agile.Framework.Tasks.Impl
{
    public class DefaultScheduler : IScheduler, IDisposable
    {
        private readonly IDictionary<Guid, ITask> nonReentrantTasks;
        private readonly IDictionary<Guid, ITask> tasks;
        private readonly IList<ITrigger> triggers;
        private readonly IDictionary<ITrigger, EventHandler> triggerHandlers; 
        private readonly ILogger _logger;
        private readonly IList<ITrigger> _unenabledTriggers; 
        public DefaultScheduler(ILoggerFactory loggerFactory)
        {
            nonReentrantTasks = new ConcurrentDictionary<Guid, ITask>();
            tasks = new ConcurrentDictionary<Guid, ITask>();
            triggers = new List<ITrigger>();
            triggerHandlers = new Dictionary<ITrigger, EventHandler>();
            _logger = loggerFactory.Create(this.GetType());
            _unenabledTriggers = new List<ITrigger>();
        }

        public event EventHandler<TaskExecutionArgs> BeforeTaskExecute;
        public event EventHandler<TaskExecutionArgs> AfterTaskExecute;
        
        public void ScheduleTask(ITask task, ITrigger trigger)
        {
            
            tasks.Add(task.Key, task);
            triggers.Add(trigger);
            trigger.Triggered += this.GetOrCreateHandler(task, trigger);
            if (this.IsRunning)
            {
                trigger.Enable();
            }
            else
            {
                _unenabledTriggers.Add(trigger);
            }
        }

        public void Shutdown(bool waitForTaskComplete)
        {
            this.IsRunning = false;
            if (waitForTaskComplete)
            {
                foreach (var trigger in triggers)
                {
                    trigger.Disable();
                }
            }
            else
            {
                throw new NotImplementedException("");
            }
            
        }

        public bool IsRunning { get; private set; }
        public void Start()
        {
            this.IsRunning = true;
            foreach (var trigger in _unenabledTriggers)
            {
                trigger.Enable();
            }
            _unenabledTriggers.Clear();
        }

        private bool disposed;
        protected void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                if (this.IsRunning)
                {
                    this.Shutdown(true);
                }
            }
            foreach (var trigger in triggers)
            {
                trigger.Triggered -= GetOrCreateHandler(null, trigger);
            }
            
            disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }
        protected virtual EventHandler GetOrCreateHandler(ITask task, ITrigger trigger)
        {
            EventHandler handler;
            if (!triggerHandlers.TryGetValue(trigger, out handler))
            {
                handler = (sender, args) =>
                    {
                        if (!this.IsRunning) return;
                        if (!task.Reentrant)
                        {
                            ITask existTask;
                            if (nonReentrantTasks.TryGetValue(task.Key, out existTask))
                            {
                                _logger.DebugFormat(Resources.Schedule_NonreentrantTaskRunning, task.Key, task.FriendlyName);
                                return;
                            }
                            nonReentrantTasks.Add(task.Key, task);
                        }
                        

                        var context = new TaskExecutionContext(this, trigger);
                        Stopwatch stopwatch = new Stopwatch();

                        DateTime startTime = DateTime.UtcNow;
                        if (BeforeTaskExecute != null)
                        {
                            var beforeArgs = new TaskExecutionArgs(task.Key, trigger.Key, task.FriendlyName, trigger.Name,
                                                                   startTime);
                            try
                            {
                                BeforeTaskExecute.Invoke(this, beforeArgs);
                            }
                            catch (Exception error)
                            {
                                _logger.Debug(string.Format(Resources.Schedule_BeforeTaskRunningEventError, task.Key, task.FriendlyName), error);
                            }
                        }
                        Exception lastError = null;
                        _logger.DebugFormat(Resources.Schedule_TaskStart, task.Key, task.FriendlyName);
                        stopwatch.Start();
                        try
                        {
                            task.Execute(context);
                            stopwatch.Stop();
                            _logger.DebugFormat(Resources.Schedule_TaskEnd, task.Key, task.FriendlyName, stopwatch.ElapsedMilliseconds);
                        }
                        catch (Exception error)
                        {
                            stopwatch.Stop();
                            lastError = error;
                            _logger.Debug(
                                    string.Format(Resources.Schedule_TaskEnd, task.Key, task.FriendlyName,
                                                  stopwatch.ElapsedMilliseconds), error);
                        }
                        if (!task.Reentrant)
                        {
                            ITask existTask;
                            if (nonReentrantTasks.TryGetValue(task.Key, out existTask))
                            {
                                Require.EatException(() => nonReentrantTasks.Remove(task.Key));
                            }
                        }
                        if (AfterTaskExecute != null)
                        {
                            var afterArgs = new TaskExecutionArgs(task.Key, trigger.Key, task.FriendlyName, trigger.Name,
                                                                  startTime,
                                                                  DateTime.UtcNow, lastError);
                            try
                            {
                                AfterTaskExecute(this, afterArgs);
                            }
                            catch (Exception error)
                            {
                                _logger.Debug(string.Format(Resources.Schedule_AfterTaskRunningEventError, task.Key, task.FriendlyName), error);
                            }
                        }
                        
                    };
                triggerHandlers[trigger] = handler;
            }
            return handler;
        }
    }
}
