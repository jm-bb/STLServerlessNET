using System.Data;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using STLServerlessNET.Models;

namespace STLServerlessNET.Helpers
{
    public class WebDatabaseHelper(MySqlConnection connection)
    {
        private readonly MySqlConnection m_conn = connection;

        public DataTable GetEligibleOrders(int orderId)
        {
            DataSet ds = new("EligibleOrders");

            try
            {
                string sql = SqlQueries.EligibleOrder.Replace("_ORDER_ID_", orderId.ToString());
                MySqlDataAdapter da = new(sql, m_conn);
                da.Fill(ds);

                return ds.Tables[0];
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

        public string GetAttributeDetails(string sku, string webAttributes)
        {
            try
            {
                if (webAttributes.Trim().Length > 0)
                {
                    string attributeValueIDs = "";

                    string[] attributes = webAttributes.Split([";"], StringSplitOptions.RemoveEmptyEntries);
                    foreach (string attribute in attributes)
                    {
                        string[] attribValues = attribute.Split(["|"], StringSplitOptions.RemoveEmptyEntries);

                        if (attribValues[0] == "2132")
                        {
                            continue;
                        }
                        attributeValueIDs += attribValues[1] + ",";
                    }

                    attributeValueIDs = attributeValueIDs.Remove(attributeValueIDs.Length - 1, 1);

                    DataTable dt = new DataTable("Attributes");

                    string sql = "SELECT fishbowl_sku FROM attribute_values WHERE attribute_value_id IN (" + attributeValueIDs.ToString() + ") AND fishbowl_sku like '" + sku + "%' AND (group_name IS NULL or TRIM(group_name) = '');";
                    MySqlCommand cmd = new(sql, m_conn);
                    OpenConn();
                    var dbResult = cmd.ExecuteScalar();
                    if (dbResult == null)
                    {
                        return sku;
                    }
                    else
                    {
                        return dbResult.ToString();
                    }
                }
                else
                {
                    return sku;
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

        public void UpdateAttributeString(DataTable dt)
        {
            //Add primary tag
            dt.Columns.Add("primary_attrib", typeof(String));
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["attributes"].ToString().Length > 0)
                {
                    string newAttributes = "";

                    string[] attributes = dr["attributes"].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    List<string> primaryAttribs = new List<string>();
                    foreach (string attribute in attributes)
                    {
                        List<string> myList = attribute.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                        string attributeValueID = myList[1];
                        if (myList.Count < 3)
                        {
                            myList.Add(GetAttributeValuePrice(Convert.ToInt32(attributeValueID)));
                        }

                        string fishbowlSku = "";
                        string fishbowlSkuQty = "1";

                        int aid = Convert.ToInt32(myList[0]);
                        int customLightbarId = 0;
                        string aname = "";

                        if (aid == 2132)
                        {
                            int order_id = Convert.ToInt32(dr["order_id"]);
                            int prod_id = Convert.ToInt32(dr["prod_id"]);
                            int cart_id = Convert.ToInt32(dr["cart_id"]);
                            customLightbarId = GetCustomLightbarSelectionId(order_id, prod_id, cart_id);
                        }
                        else
                        {
                            aname = GetAttributeName(Convert.ToInt32(myList[0]));
                            myList[0] = aname;
                            myList[1] = GetAttributeValueDesc(Convert.ToInt32(myList[1]));
                            fishbowlSku = GetMountingFishbowlSku(attributeValueID);
                            fishbowlSkuQty = GetMountingFishbowlSkuQty(attributeValueID);
                        }

                        if (fishbowlSkuQty == null)
                        {
                            myList.Add("1");
                        }
                        else
                        {
                            myList.Add(fishbowlSkuQty);
                        }

                        if (fishbowlSku != "")
                        {
                            myList.Add(fishbowlSku);
                        }

                        //Check to see if this is a primary attribute
                        if (GetPrimaryAttrib(aid) == "Y")
                        {
                            primaryAttribs.Add(aname);
                        }



                        if (aid != 2132)
                        {
                            newAttributes += string.Join("|", myList) + ";";
                        }
                        else
                        {
                            //Add the attribute price to the dr price.
                            var newPrice = Convert.ToDecimal(dr[8]) + Convert.ToDecimal(myList[2]);
                            dr["purchase_price"] = newPrice;
                            dr["CustomBar"] = customLightbarId;
                        }
                    }

                    if (primaryAttribs.Count > 0)
                    {
                        dr["primary_attrib"] = JsonConvert.SerializeObject(primaryAttribs);
                    }

                    dr["attributes"] = newAttributes.Remove(newAttributes.Length - 1, 1);
                }
            }
        }

        public string GetMountingFishbowlSku(string id)
        {
            try
            {
                string sql = "SELECT fishbowl_sku FROM attribute_values WHERE attribute_value_id=" + id.ToString();
                MySqlCommand cmd = new MySqlCommand(sql, m_conn);
                OpenConn();
                var result = cmd.ExecuteScalar();
                if (result == null)
                {
                    return string.Empty;
                }
                else
                {
                    return result.ToString();
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

        public void UpdateProductType(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                int prodid = Convert.ToInt32(dr["prod_id"]);

                if (IsClearance(prodid))
                {
                    dr["product_type"] = "clearance";
                }
            }
        }

        public bool IsClearance(int prodid)
        {
            DataSet ds = new DataSet("Category");

            try
            {
                string sql = @"SELECT * FROM category_product WHERE cat_id=30 AND prod_id=" + prodid.ToString();
                MySqlDataAdapter da = new MySqlDataAdapter(sql, m_conn);
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
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

        public void UpdateCouponString(DataRow dr)
        {
            //Free Coupons
            List<string> freeCoupons = new List<string>(new string[] { "STLFREESHIP", "FREESHIP", "STLCSFS", "STLTAXFREE" });
            List<string> appliedCoupons = new List<string>();

            //Coupon list for order
            List<string> couponCode = dr["coupon_code"].ToString().Trim().Split(',').ToList();
            string[] couponDiscount = dr["coupon_discount"].ToString().TrimEnd(';').Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string c in couponDiscount)
            {
                string[] code = c.Split('|');
                appliedCoupons.Add(code[0]);
            }

            couponCode.RemoveAll(x => !freeCoupons.Contains(x));
            if (appliedCoupons.Count > 0)
            {
                couponCode.AddRange(appliedCoupons);
            }

            List<string> finalCoupons = OnlyValidCoupons(couponCode);
            dr["coupon_code"] = GetCouponJson(finalCoupons.ToArray());
        }

        private List<string> OnlyValidCoupons(List<string> validCoupons)
        {
            try
            {
                string codesArray = String.Join("','", validCoupons);

                DataSet dsCodes = new DataSet("Codes");
                //string sql = $"SELECT coupon_code FROM coupons WHERE coupon_valid='Y' AND coupon_code IN ('{codesArray}');";
                string sql = $"SELECT coupon_code FROM coupons WHERE coupon_code IN ('{codesArray}');";
                MySqlDataAdapter da = new MySqlDataAdapter(sql, m_conn);
                da.Fill(dsCodes);

                List<string> finalCoupons = new List<string>();
                foreach (DataRow drCode in dsCodes.Tables[0].Rows)
                {
                    if (drCode["coupon_code"].ToString().Trim() != "")
                    {
                        finalCoupons.Add(drCode["coupon_code"].ToString().Trim());
                    }
                }

                return finalCoupons;
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

        public string GetCouponJson(string[] coupons)
        {
            if (coupons.Length > 0)
            {
                string sql = "SELECT coupon_id, coupon_code, coupon_message, coupon_type, coupon_discount, apply_to, product_id, cat_id FROM coupons WHERE coupon_code IN (";
                foreach (string coupon in coupons)
                {
                    sql += "'" + coupon + "',";
                }
                sql = sql.TrimEnd(',');
                sql += ") ORDER BY IF(coupon_type='percentage', 1, 0), coupon_type;";

                DataTable dt = new DataTable("Coupons");
                try
                {
                    MySqlDataAdapter da = new MySqlDataAdapter(sql, m_conn);
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        return JsonConvert.SerializeObject(dt, Formatting.None);
                    }
                    else
                    {
                        return "[{ \"coupon_code\": \"" + String.Join(",", coupons) + "\"} ]";
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
            else
            {
                return String.Join(",", coupons);
            }
        }

        public DataTable GetShippingBoxes(int orderId)
        {
            DataTable dt = new DataTable("ShippingBoxes");

            try
            {

                string sql = @"select s.box_id, s.order_id, s.BOX_NUMBER, s.NAME, s.SKU_COUNT, s.TOTAL_WEIGHT, b.product_id, p.prod_sku, b.qty from shipping_boxes s inner join boxes_sku b "
                             + "on b.box_id = s.box_id inner join product p on p.prod_id = b.product_id where s.order_id=@orderId";
                MySqlCommand cmd = new MySqlCommand(sql, m_conn);
                cmd.Parameters.AddWithValue("@orderId", orderId);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
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

        private string GetAttributeValuePrice(int id)
        {
            try
            {
                string sql = "SELECT attribute_price FROM attribute_values WHERE attribute_value_id=" + id.ToString();
                MySqlCommand cmd = new MySqlCommand(sql, m_conn);
                OpenConn();
                return cmd.ExecuteScalar().ToString();
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

        private string GetAttributeName(int id)
        {
            try
            {
                string sql = "SELECT attribute_name FROM attributes WHERE attribute_id=" + id.ToString();
                MySqlCommand cmd = new MySqlCommand(sql, m_conn);
                OpenConn();
                var result = cmd.ExecuteScalar();
                if (result == null)
                {
                    return string.Empty;
                }
                else
                {
                    return result.ToString().Trim();
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

        private string GetAttributeValueDesc(int id)
        {
            try
            {
                string sql = "SELECT attribute_desc FROM attribute_values WHERE attribute_value_id=" + id.ToString();
                MySqlCommand cmd = new MySqlCommand(sql, m_conn);
                OpenConn();
                var result = cmd.ExecuteScalar();
                if (result == null)
                {
                    return string.Empty;
                }
                else
                {
                    return result.ToString().Trim();
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

        private int GetCustomLightbarSelectionId(int orderId, int prodId, int cartId)
        {
            try
            {
                string sql = "select id from lightbar_selections where order_id=@orderId and prod_id=@prodId and cart_id=@cartId;";
                MySqlCommand cmd = new MySqlCommand(sql, m_conn);
                cmd.Parameters.AddWithValue("@orderId", orderId);
                cmd.Parameters.AddWithValue("@prodId", prodId);
                cmd.Parameters.AddWithValue("@cartId", cartId);

                OpenConn();

                return Convert.ToInt32(cmd.ExecuteScalar());
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

        public string GetMountingFishbowlSkuQty(string id)
        {
            try
            {
                string sql = "SELECT fishbowl_sku_qty FROM attribute_values WHERE attribute_value_id=" + id;
                MySqlCommand cmd = new MySqlCommand(sql, m_conn);
                OpenConn();
                var result = cmd.ExecuteScalar();
                if (result == null)
                {
                    return "0";
                }
                else
                {
                    return result.ToString();
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

        private string GetPrimaryAttrib(int id)
        {
            try
            {
                string sql = "SELECT primary_attrib FROM attributes WHERE attribute_id=" + id.ToString();
                MySqlCommand cmd = new MySqlCommand(sql, m_conn);
                OpenConn();
                var result = cmd.ExecuteScalar();
                if (result == null)
                {
                    return "N";
                }
                else
                {
                    return result.ToString();
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
}