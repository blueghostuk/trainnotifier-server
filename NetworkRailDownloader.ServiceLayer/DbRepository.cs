using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Transactions;

namespace TrainNotifier.ServiceLayer
{
    public abstract class DbRepository
    {
        private readonly DbProviderFactory _dbFactory;
        private readonly string _connString;
        protected const int _defaultCommandTimeout = 30;
        protected readonly int _commandTimeout;

        protected DbRepository(string connectionStringName = "database")
        {
            _connString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            _dbFactory = DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName);

            DbConnectionStringBuilder connBuilder = _dbFactory.CreateConnectionStringBuilder();
            connBuilder.ConnectionString = _connString;
            if (connBuilder.ContainsKey("DefaultCommandTimeout"))
            {
                _commandTimeout = Convert.ToInt32(connBuilder["DefaultCommandTimeout"]);
            }
            else
            {
                _commandTimeout = _defaultCommandTimeout;
            }
        }

        protected DbConnection CreateConnection()
        {
            DbConnection connection = _dbFactory.CreateConnection();
            connection.ConnectionString = _connString;
            return connection;
        }

        protected DbConnection CreateAndOpenConnection()
        {
            DbConnection connection = CreateConnection();
            connection.Open();
            return connection;
        }

        protected virtual void ExecuteNonQuery(string sql, dynamic parameters = null, DbConnection existingConnection = null)
        {
            if (existingConnection != null)
            {
                existingConnection.Execute(sql, (object)parameters);
            }
            else
            {
                using (DbConnection dbConnection = CreateAndOpenConnection())
                {
                    dbConnection.Execute(sql, (object)parameters);
                }
            }
        }
        protected virtual Guid ExecuteInsert(string sql, dynamic parameters = null, DbConnection existingConnection = null)
        {
            return ExecuteScalar<Guid>(sql, parameters, existingConnection);
        }

        protected virtual T ExecuteScalar<T>(string sql, dynamic parameters = null, DbConnection existingConnection = null)
        {
            if (existingConnection != null)
            {
                // should be SingleOrDefault - but need to work around db bugs for now
                return existingConnection.Query<T>(sql, (object)parameters).FirstOrDefault();
            }
            else
            {
                using (DbConnection dbConnection = CreateAndOpenConnection())
                {
                    // should be SingleOrDefault - but need to work around db bugs for now
                    return dbConnection.Query<T>(sql, (object)parameters).FirstOrDefault();
                }
            }
        }

        protected virtual IEnumerable<T> Query<T>(string sql, dynamic parameters = null, DbConnection existingConnection = null)
        {
            if (existingConnection != null)
            {
                return existingConnection.Query<T>(sql, (object)parameters);
            }
            else
            {
                using (DbConnection dbConnection = CreateAndOpenConnection())
                {
                    return dbConnection.Query<T>(sql, (object)parameters);
                }
            }
        }

        public static TransactionScope GetTransactionScope()
        {
            return new TransactionScope(TransactionScopeOption.Required,
                                new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead });
        }
    }
}
