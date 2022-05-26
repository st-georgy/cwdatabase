using Npgsql;
using System;
using System.Data;
using System.Threading.Tasks;

namespace CourseWork.Database
{
    public sealed class DbWorker
    {
        private static readonly string connectionString = "Server=localhost;Port=5432;User ID={0};Database=coursework;Password={1};";
        private static NpgsqlConnection _connection = default!;

        public DbWorker(string login, string password)
        {
            _connection = new NpgsqlConnection(string.Format(connectionString, login, password));
            _connection.OpenAsync();
        }

        ~DbWorker() => _connection.Close();

        public static async Task<bool> checkConnection(string login, string password)
        {
            var connString = string.Format(connectionString, login, password);
            bool result = false;
            
            await using (var conn = new NpgsqlConnection(connString))
            {
                try
                {
                    await conn.OpenAsync();
                    result = conn.State == ConnectionState.Open ? true : false;
                }
                catch (Exception e) { Console.Error.WriteLine(e); }
                finally { conn.Close(); }
            }

            return result;
        }

        public DataSet makeQuery(string queryStr)
        {
            using var cmd = new NpgsqlCommand(queryStr, _connection);
            cmd.Connection = _connection;

            var adapter = new NpgsqlDataAdapter(cmd);

            var ds = new DataSet();
            adapter.Fill(ds);
            return ds;
        }
    }
}
