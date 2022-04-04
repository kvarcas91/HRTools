using Dapper;
using Domain.IO;
using Domain.Storage;
using Domain.Types;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public abstract class BaseRepository
    {

        internal bool Execute(string query)
        {
            var connection = OpenConnection();
            var transaction = connection.BeginTransaction();

            try
            {
                IEnumerable<dynamic> results = connection.Query(query, transaction: transaction);
                transaction.Commit();
                return results != null;
            }
            catch
            {
                return false;
            }
            finally
            {
                transaction.Dispose();
                CloseConnection(connection);
            }

        }

        internal T GetScalar<T>(string query)
        {
            var connection = OpenConnection();
            try
            {
                var output = connection.QueryFirstOrDefault<T>(query);
                return output;
            }
            catch 
            {
                return default; 
            }
            finally
            {
                CloseConnection(connection);
            }
        }

        internal T GetCachedScalar<T>(string query)
        {
            var connection = OpenCachedConnection();
            try
            {
                var output = connection.QueryFirstOrDefault<T>(query);
                return output;
            }
            catch 
            {
                return default; 
            }
            finally
            {
                CloseConnection(connection);
            }
        }

        internal IEnumerable<T> GetCached<T>(string query)
        {
            var connection = OpenConnection();
            try
            {
                var output = connection.Query<T>(query);
                return output;
            }
            catch 
            {
                return default; 
            }
            finally
            {
                CloseConnection(connection);
            }
        }

        internal Task<bool> ExecuteAsync(string query)
        {
            return Task.Run(() =>
            {
                var connection = OpenConnection();
                var transaction = connection.BeginTransaction();

                try
                {
                    IEnumerable<dynamic> results = connection.Query(query, transaction: transaction);
                    transaction.Commit();
                    CacheManager.ResetTimer();
                    return results != null;
                }
                catch (Exception e)
                {
                    return false;
                }
                finally
                {
                    transaction.Dispose();
                    CloseConnection(connection);
                }
            });

        }

        internal Task<T> GetScalarAsync<T>(string query)
        {
            return Task.Run(() =>
            {
                var connection = OpenConnection();
                try
                {
                    var output = connection.QueryFirstOrDefault<T>(query);
                    return output;
                }
                catch
                {
                    return default;
                }
                finally
                {
                    CloseConnection(connection);
                }
            });
            

        }

        internal Task<T> GetCachedScalarAsync<T>(string query)
        {
            return Task.Run(async () =>
            {
                if (!CacheManager.CacheState.Equals(CacheState.Stable))
                {
                    await Task.Delay(1000);
                }
                var connection = OpenCachedConnection();
                try
                {
                    var output = connection.QueryFirstOrDefault<T>(query);
                    return output;
                }
                catch
                {
                    return default;
                }
                finally
                {
                    CloseConnection(connection);
                }
            });
        }

        internal Task<IEnumerable<T>> GetCachedAsync<T>(string query)
        {
            return Task.Run(async () =>
            {
                if (CacheManager.CacheState.Equals(CacheState.InProgress))
                {
                    await Task.Delay(1000);
                }
                var connection = OpenCachedConnection();
                try
                {
                    var output = connection.Query<T>(query);
                    return output;
                }
                catch
                {
                    return default;
                }
                finally
                {
                    CloseConnection(connection);
                }
            });
        }

        #region Connection

        private SQLiteConnection OpenConnection()
        {
            var connection = GetConnection();
            try
            {
                if (connection.State == System.Data.ConnectionState.Closed) connection.Open();
            }
            catch { }
            return connection;
        }

        private SQLiteConnection OpenCachedConnection()
        {
            var connection = GetCachedConnection();
            try
            {
                if (connection.State == System.Data.ConnectionState.Closed) connection.Open();
            }
            catch { }
            return connection;
        }

        private void CloseConnection(SQLiteConnection connection)
        {
            connection.Close();
            connection.Dispose();
        }

        private static string GetConnectionString()
        {
            string dbPath = Environment.UserName.Equals("eslut") ? DataStorage.AppSettings.DbTestPath : DataStorage.AppSettings.DbProductionPath;
            return $"Data Source={dbPath};Version=3;PRAGMA journal_mode=WAL;pragma synchronous = normal;pragma mmap_size = 30000000000;pragma page_size = 32768;PRAGMA busy_timeout = 3000;";
        }

        private static string GetCachedConnectionString()
        {
            return $"Data Source={DataStorage.AppSettings.LocalDbPath};Version=3;PRAGMA journal_mode=WAL;pragma synchronous = normal;pragma mmap_size = 30000000000;pragma page_size = 32768;PRAGMA busy_timeout = 3000;";
        }

        private SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(GetConnectionString(), true);
        }

        private SQLiteConnection GetCachedConnection()
        {
            return new SQLiteConnection(GetCachedConnectionString(), true);
        }

        protected bool CreateDbFile(string path)
        {
            try
            {
                SQLiteConnection.CreateFile(path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

    }
}
