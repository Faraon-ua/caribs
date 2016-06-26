using Caribs.Common.Helpers;
using MySql.Data.MySqlClient;

namespace Caribs.Services.Data
{
    public class MySqlService
    {
        private string _connectionString;
        public MySqlService(string host, string dbName, string userName, string userPassword)
        {
            _connectionString = string.Format("server={0};userid={1};password={2};database={3};", host, userName, userPassword, dbName);
        }

        public void ExecuteSql(string sql)
        {
            MySqlConnection conn = null;

            try
            {
                conn = new MySqlConnection(_connectionString);
                conn.Open();
                var cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                EmailHelper.Instance.SendSqlConnectionException(ex.ToString());
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
    }
}
