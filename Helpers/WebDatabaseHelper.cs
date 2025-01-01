using MySql.Data.MySqlClient;
using System.Data;

public class WebDatabaseHelper(MySqlConnection connection)
{
    private readonly MySqlConnection m_conn = connection;

    public DataTable GetOrderDetails(int orderId)
    {
        DataTable dt = new("OriginalOrderDetails");

        try
        {
            //Get the order's product info
            string sql = @"SELECT oc.order_id, oc.quantity, oc.attributes, oc.aplus_warranty, p.prod_id, p.prod_name, oc.prod_sku, p.list_price, oc.purchase_price, p.taxable, p.product_type, null AS package_id, 0 as warranty,
                           oc.fishbowl_rule_id, p.n_packs, oc.cart_id, p.lightbar_type, p.category_for_url FROM cart oc INNER JOIN product p ON oc.prod_id=p.prod_id WHERE oc.order_id=" + orderId + " ORDER BY product_type DESC";
            MySqlDataAdapter da = new(sql, m_conn);
            da.Fill(dt);
            DataColumn dc = new("DiscountPack", typeof(Int32))
            {
                DefaultValue = 0
            };
            dt.Columns.Add(dc);

            DataColumn dc2 = new("CategoryIds", typeof(string))
            {
                DefaultValue = null
            };
            dt.Columns.Add(dc2);

            DataTable retDt = dt.Clone();

            foreach (DataRow dr in dt.Rows)
            {
                int prod_id = Convert.ToInt32(dr["prod_id"]);
                List<int> catIds = GetCategoryIds(prod_id);
                if (catIds.Count > 0)
                {
                    dr["CategoryIds"] = string.Join(",", catIds.ToArray());
                }

                if (IsDiscountPack(prod_id, 20))
                {
                    dr["DiscountPack"] = 1;
                }

                if (dr["product_type"].ToString() == "package")
                {
                    //Append individual products to the data table
                    int packageId = Convert.ToInt32(dr["prod_id"]);
                    DataRowCollection packageRows = GetPackageDetails(orderId, packageId).Rows;
                    foreach (DataRow packageRow in packageRows)
                    {
                        decimal price = Convert.ToDecimal(packageRow["purchase_price"].ToString() == "0.00"
                                                              ? packageRow["list_price"]
                                                              : packageRow["purchase_price"]);
                        //Get the package discounted price of 15% off.
                        price = Decimal.Multiply(price, 0.85m);
                        packageRow["aplus_warranty"] = dr["aplus_warranty"];
                        packageRow["warranty"] = Convert.ToDouble(dr["warranty"]) / packageRows.Count;
                        packageRow["purchase_price"] = price;
                        packageRow["product_type"] = "package";
                        retDt.ImportRow(packageRow);
                    }

                }
                else
                {
                    retDt.ImportRow(dr);
                }
            }

            DataColumn dc3 = new("CustomBar", typeof(Int32))
            {
                DefaultValue = 0
            };
            retDt.Columns.Add(dc3);

            return retDt;
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

    public DataTable GetPackageDetails(int orderId, int packageId)
    {
        DataTable dt = new("package_details");
        try
        {
            string sql = "SELECT pd.order_id, pd.quantity, pd.attribute as attributes, pd.aplus_warranty, p.prod_id, p.prod_name, p.prod_sku, p.list_price, 0 AS warranty, pd.purchase_price, p.taxable, p.product_type, pd.package_id AS package_id, p.fishbowl_rule_id FROM package_details pd INNER JOIN product p ON pd.prod_id=p.prod_id WHERE pd.shopping_cart_id > 0 AND pd.order_id=" + orderId + " AND pd.package_id=" + packageId;
            MySqlDataAdapter da = new(sql, m_conn);
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

    public List<int> GetCategoryIds(int prodId)
    {
        DataTable dt = new("CategoryProduct");
        List<int> catIds = [];
        try
        {
            string sql = $"select * from category_product where prod_id={prodId};";
            MySqlDataAdapter da = new(sql, m_conn);
            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                catIds.Add(Convert.ToInt32(dr["cat_id"]));
            }

            return catIds;
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

    public bool IsDiscountPack(int prodId, int serisId)
    {
        try
        {
            string sql = "SELECT COUNT(series_product_id) FROM series_products WHERE product_id=" + prodId.ToString() + " and series_id=" + serisId;
            MySqlCommand cmd = new(sql, m_conn);
            OpenConn();
            object result = cmd.ExecuteScalar();
            if (result == null)
            {
                return false;
            }
            else
            {
                int x = Convert.ToInt32(result);
                if (x > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
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