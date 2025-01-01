using MySql.Data.MySqlClient;
using System.Data;

namespace STLServerlessNET.Helpers
{
    public class ServiceDatabaseHelper(MySqlConnection connection)
    {
        private readonly MySqlConnection m_conn = connection;

        public DataTable GetProperSku(string sku)
        {
            DataTable dt = new("Products");

            try
            {
                string sql = "SELECT * FROM product WHERE partid IN (SELECT DISTINCT id FROM part WHERE num LIKE '" + sku + "%')";
                MySqlDataAdapter da = new MySqlDataAdapter(sql, m_conn);
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CleanUp();
            }
        }

        public DataTable GetCarrierServices(string carrier)
        {
            DataTable dt = new("Services");

            try
            {
                string sql = "SELECT id, name FROM carrierservice WHERE carrierId=(SELECT id FROM carrier WHERE name='" + carrier + "');";
                MySqlDataAdapter da = new MySqlDataAdapter(sql, m_conn);
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CleanUp();
            }
        }

        private void OpenConn()
        {
            if (m_conn.State != ConnectionState.Open)
            {
                m_conn.Open();
            }
        }

        private void CleanUp()
        {
            m_conn.Close();
        }
    }
}