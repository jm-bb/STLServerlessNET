using MySql.Data.MySqlClient;
using System.Data;

public class ServiceDatabaseHelper(MySqlConnection connection)
{
    private readonly MySqlConnection m_conn = connection;

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
            CleanUp();
        }
    }

    public DataTable GetCarrierServices(string carrier)
    {
        DataTable dt = new DataTable("Services");

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