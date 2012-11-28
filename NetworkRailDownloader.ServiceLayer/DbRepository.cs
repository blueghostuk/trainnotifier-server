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
        private const string SelectLastInsertId = "SELECT LAST_INSERT_ID();";

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

        protected virtual void ExecuteNonQuery(string sql, dynamic parameters)
        {
            using (DbConnection dbConnection = CreateAndOpenConnection())
            {
                dbConnection.Execute(sql, (object)parameters);
            }
        }

        protected virtual Guid ExecuteInsert(string sql, dynamic parameters)
        {
            return ExecuteScalar<Guid>(sql, parameters);
        }

        protected virtual T ExecuteScalar<T>(string sql, dynamic parameters)
        {
            using (DbConnection dbConnection = CreateAndOpenConnection())
            {
                // should be SingleOrDefault - but need to work around db bugs for now
                return dbConnection.Query<T>(sql, (object)parameters).FirstOrDefault();
            }
        }

        protected virtual IEnumerable<T> Query<T>(string sql, dynamic parameters)
        {
            using (DbConnection dbConnection = CreateAndOpenConnection())
            {
                return dbConnection.Query<T>(sql, (object)parameters);
            }
        }

        protected virtual long? GetId(DbConnection dbConnection, dynamic parameters)
        {
            return default(long?);
        }

        public static TransactionScope GetTransactionScope()
        {
            return new TransactionScope(TransactionScopeOption.Required,
                                new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead });
        }
    }
}
