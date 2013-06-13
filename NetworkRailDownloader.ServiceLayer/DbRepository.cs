using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Transactions;

namespace TrainNotifier.Service
{
    public abstract class DbRepository
    {
        private readonly DbProviderFactory _dbFactory;
        private readonly string _connString;
        protected const int _defaultCommandTimeout = 30;
        protected readonly int _commandTimeout;

        protected DbRepository(string connectionStringName = "archive", int? defaultCommandTimeout = null)
        {
            _connString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            _dbFactory = DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName);

            DbConnectionStringBuilder connBuilder = _dbFactory.CreateConnectionStringBuilder();
            connBuilder.ConnectionString = _connString;
            _commandTimeout = defaultCommandTimeout ?? _defaultCommandTimeout;
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

        protected virtual void ExecuteNonQuery(string sql, dynamic parameters = null, DbConnection existingConnection = null, int? commandTimeout = null)
        {
            if (existingConnection != null)
            {
                existingConnection.Execute(sql, (object)parameters);
            }
            else
            {
                using (DbConnection dbConnection = CreateAndOpenConnection())
                {
                    dbConnection.Execute(sql, (object)parameters, commandTimeout: commandTimeout ?? _commandTimeout);
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
                return existingConnection.Query<T>(sql, (object)parameters, commandTimeout: _commandTimeout).FirstOrDefault();
            }
            else
            {
                using (DbConnection dbConnection = CreateAndOpenConnection())
                {
                    // should be SingleOrDefault - but need to work around db bugs for now
                    return dbConnection.Query<T>(sql, (object)parameters, commandTimeout: _commandTimeout).FirstOrDefault();
                }
            }
        }

        protected virtual IEnumerable<T> Query<T>(string sql, dynamic parameters = null, DbConnection existingConnection = null)
        {
            if (existingConnection != null)
            {
                return existingConnection.Query<T>(sql, (object)parameters, commandTimeout: _commandTimeout);
            }
            else
            {
                using (DbConnection dbConnection = CreateAndOpenConnection())
                {
                    return dbConnection.Query<T>(sql, (object)parameters, commandTimeout: _commandTimeout);
                }
            }
        }

        public static TransactionScope GetTransactionScope()
        {
            return new TransactionScope(TransactionScopeOption.Required,
                                new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead });
        }

        public static TransactionScope GetTransactionScope(TimeSpan timeout)
        {
            return new TransactionScope(TransactionScopeOption.Required,
                                new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead, Timeout = timeout });
        }
    }
}
