using Npgsql;
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
                await conn.OpenAsync();
                result = conn.State == ConnectionState.Open ? true : false;
            }

            return result;
        }

        public DataTable makeQuery(string queryStr)
        {
            using var cmd = new NpgsqlCommand(queryStr, _connection);
            cmd.Connection = _connection;

            var adapter = new NpgsqlDataAdapter(cmd);

            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
    }
}
