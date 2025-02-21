using System.Data;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using STLServerlessNET.Helpers;

namespace STLServerlessNET.Controllers.Web
{

    [ApiController]
    [Route("web/[controller]")]
    public class CartController(MySqlConnectionFactory connectionFactory, ILogger<CartController> logger) : ControllerBase
    {
        private readonly MySqlConnectionFactory _connectionFactory = connectionFactory;
        private readonly ILogger<CartController> _logger = logger;

        [HttpGet("{id}")]
        public IActionResult GetCartDetails(int id)
        {
            _logger.LogInformation("Calling GetCartDetails()...");
            _logger.LogInformation("Order ID:{@orderId}", id);

            try
            {
                var webConnection = _connectionFactory.CreateConnection("WebConnection");
                var serviceConnection = _connectionFactory.CreateConnection("ServiceConnection");

                WebDatabaseHelper wdh = new(webConnection);
                ServiceDatabaseHelper sdh = new(serviceConnection);
                FishbowlHelper fbh = new();

                DataTable order = wdh.GetEligibleOrders(id);
                if (order.Rows.Count <= 0)
                {
                    return StatusCode(404, "No order record found.");
                }

                order.Columns["email"].ColumnName = "user_email";
                order.Columns.Add("full_name", typeof(String));
                order.Columns.Add("bill_state_name", typeof(String));
                order.Columns.Add("bill_country_name", typeof(String));
                order.Columns.Add("ship_state_name", typeof(String));
                order.Columns.Add("ship_country_name", typeof(String));
                order.Columns.Add("referred_by_name", typeof(String));
                order.Columns.Add("full_company_name", typeof(String));
                order.Columns.Add("full_customer_name", typeof(String));
                order.Columns.Add("bill_address_count", typeof(int));
                order.Columns.Add("account_name", typeof(string));

                string results = "";
                foreach (DataRow dr in order.Rows)
                {
                    dr["company"] = StringHelper.StripIncompatableQuotes(dr["company"].ToString().Trim());
                    dr["first_name"] = StringHelper.StripIncompatableQuotes(dr["first_name"].ToString().Trim());
                    dr["last_name"] = StringHelper.StripIncompatableQuotes(dr["last_name"].ToString().Trim());
                    dr["suffix"] = StringHelper.StripIncompatableQuotes(dr["suffix"].ToString().Trim());
                    dr["full_company_name"] = dr["company"].ToString().Trim();

                    DataTable orderDetails = wdh.GetOrderDetails(id);

                    //Update the SKU to match the proper product.
                    foreach (DataRow row in orderDetails.Rows)
                    {
                        DataTable fishbowlProducts = sdh.GetProperSku(row["prod_sku"].ToString());

                        if (fishbowlProducts.Rows.Count > 1)
                        {
                            //Look at the web database for the proper sku
                            row["prod_sku"] = wdh.GetAttributeDetails(row["prod_sku"].ToString(), row["attributes"].ToString());
                        }
                        else if (fishbowlProducts.Rows.Count == 1)
                        {
                            row["prod_sku"] = fishbowlProducts.Rows[0]["NUM"];
                        }
                    }

                    //Update the attribute string
                    wdh.UpdateAttributeString(orderDetails);

                    //If clearance update the datatable.
                    wdh.UpdateProductType(orderDetails);

                    //Update the coupon codes to JSON
                    wdh.UpdateCouponString(dr);

                    DataTable shippingBoxes = wdh.GetShippingBoxes((int)dr["order_id"]);

                    //Get carrier/service codes.
                    DataTable carrierServices = null;
                    string shippingMethod = dr["shipping_method"].ToString().ToLower().Trim();
                    bool isInternational = dr["ship_country"].ToString().ToLower().Trim() != "us";

                    if (shippingMethod.Contains("international"))
                    {
                        carrierServices = sdh.GetCarrierServices("UPS International");
                    }
                    else
                    {
                        if (isInternational && shippingMethod == "ups access point delivery")
                        {
                            carrierServices = sdh.GetCarrierServices("UPS International");
                        }
                        else
                        {
                            carrierServices = sdh.GetCarrierServices("UPS");
                        }
                    }

                    //Get the response by adding the sales order
                    string orderType = dr["order_type"].ToString();
                    string response = fbh.ProcessSO(dr, orderDetails, orderType, carrierServices, shippingBoxes);

                    //Generate the line numbers
                    XmlDocument xmlDoc = fbh.AddLineNumbers(response, true);

                    results = JsonConvert.SerializeXmlNode(xmlDoc, Newtonsoft.Json.Formatting.Indented);
                }

                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, "An internal server error occurred.");
            }
        }
    }
}