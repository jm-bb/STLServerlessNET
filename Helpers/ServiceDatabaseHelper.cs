using MySql.Data.MySqlClient;
using System.Data;

public class ServiceDatabaseHelper
{
    private MySqlConnection m_conn;
    
    public ServiceDatabaseHelper(MySqlConnection connection)
    {
        m_conn = connection;
    }

    public DataTable GetProperSku(string sku)
    {
        DataTable dt = new DataTable("Products");

        try
        {
            string sql = "SELECT * FROM PRODUCT WHERE PARTID IN (SELECT DISTINCT ID FROM PART WHERE NUM LIKE '" + sku + "%')";
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
            m_conn.Close();
        }
    }
}