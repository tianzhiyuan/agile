using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Agile.Framework.Data
{
    /// <summary>
    /// 连接管理器
    /// 提供线程级别的唯一Connection
    /// </summary>
    public class ConnectionManager : IDisposable
    {

        [ThreadStatic]
        private static object _lockObject;
        private static object LockObject
        {
            get { return _lockObject ?? (_lockObject = new object()); }
        }

        [ThreadStatic]
        private static Dictionary<string, ConnectionManager> managers;
        private static Dictionary<string, ConnectionManager> Managers
        {
            get
            {
                if (managers == null)
                    managers = new Dictionary<string, ConnectionManager>();
                return managers;
            }
        }

        private readonly IDbConnection connection = null;
        private int _referenceCount;
        private readonly string _name;


        public static ConnectionManager Get(string connectionName)
        {
            lock (LockObject)
            {
                ConnectionManager mgr;
                if (Managers.ContainsKey(connectionName))
                {
                    mgr = Managers[connectionName];
                }
                else
                {
                    mgr = new ConnectionManager(connectionName);
                    Managers.Add(connectionName, mgr);
                }

                mgr.AddRef();
                return mgr;
            }
        }

        private ConnectionManager(string connectionName)
        {
            _name = connectionName;
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings[_name].ConnectionString);
//            connection = new StackExchange.Profiling.Data.ProfiledDbConnection((DbConnection)connection, MiniProfiler.Current);
            if (Connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
        }


        public IDbConnection Connection
        {
            get { return connection; }
        }

        private void AddRef()
        {
            _referenceCount += 1;
        }

        private void DeRef()
        {
            lock (LockObject)
            {
                _referenceCount -= 1;
                if (_referenceCount == 0)
                {
                    connection.Dispose();
                    Managers.Remove(_name);
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DeRef();
            }
        }

        ~ConnectionManager()
        {
            Dispose(false);
        }

        #endregion

    }
}
