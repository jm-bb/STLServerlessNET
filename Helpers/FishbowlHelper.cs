using System.Net;
using System.Xml.Linq;
using System.Data;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using STLServerlessNET.Models;
using System.Xml;

namespace STLServerlessNET.Helpers
{
    public class FishbowlHelper
    {
        public string ProcessSO(DataRow order, DataTable orderDetails, string orderType, DataTable carrierServices, DataTable boxDt)
        {
            List<Coupon> orderCoupons = null;
            try
            {
                string xml = "";
                string orderId = order["order_id"].ToString().Trim();

                //Check if a package deal.
                bool hasPackage = false;
                bool hasGiftCard = false;
                bool hasFullLightbar = HasFullLightbar(orderDetails);
                Dictionary<string, double> giftCardList = new Dictionary<string, double>();

                string customBar = "";
                foreach (DataRow detailRow in orderDetails.Rows)
                {
                    if (detailRow["product_type"].ToString() == "package")
                    {
                        hasPackage = true;
                        break;
                    }

                    if (Convert.ToInt32(detailRow["CustomBar"]) > 0)
                    {
                        customBar += "\r\n";
                        customBar += "https://www.speedtechlights.com/lightbars/preview?id=" + detailRow["CustomBar"].ToString();
                    }
                }

                //hasGiftCard = true;
                for (int x = 1; x < 4; x++)
                {
                    string gfSuffix = x > 1 ? $"{x}" : "";
                    string gfNumber = order[$"giftcard_number{gfSuffix}"].ToString().Trim();
                    double gfPayment = Convert.ToDouble(order[$"giftcard_payment{gfSuffix}"].ToString().Trim().Length <= 0 ? 0 : order[$"giftcard_payment{gfSuffix}"]);
                    if (gfNumber.Length > 0 && gfPayment > 0)
                    {
                        giftCardList.Add(gfNumber, gfPayment);
                    }
                }

                if (giftCardList.Count > 0) { hasGiftCard = true; }

                string orderNote = "";
                string carrier = "";
                string service = "";
                int points_claimed = Convert.ToInt32(order["points_claimed"]);
                string referred_id = order["referred_by"].ToString();
                string referred_name = order["referred_by_name"].ToString();
                string shipping_method = order["shipping_method"].ToString().Trim();
                bool isInternational = order["ship_country"].ToString().Trim().ToLower() != "us";
                int priorityProcessing = Convert.ToInt16(order["priority_processing"]);
                int containsCustomLightbar = Convert.ToInt16(order["contains_custom_lightbar"]);
                shipping_method = shipping_method.Replace("*", "");
                string company = order["company"].ToString().Trim();
                decimal orderTotal = Convert.ToDecimal(order["sub_total"]);

                string tax_exempt = order["tax_exempt"].ToString();
                bool hasTaxFreeCoupon = HasTaxFree(order);

                DateTime time = DateTime.Now;

                //Get the correct country if its certains states like Puerto Rico, Guam, etc.
                order["bill_country"] = GetCorrectCountry(order["bill_state"].ToString(), order["bill_country"].ToString());
                order["ship_country"] = GetCorrectCountry(order["ship_state"].ToString(), order["ship_country"].ToString());

                if (shipping_method.Equals("Fedex 2 Day"))
                {
                    shipping_method = "FedEx 2-Day";
                }

                // t1.Enrolled, t1.PAResStatus, t1.SignatureVerification
                orderNote += "Web Order ID: " + order["order_id"].ToString() + "\r\nCardinal: ";

                orderNote += SetCardinal(order["Enrolled"].ToString());
                orderNote += "-" + SetCardinal(order["PAResStatus"].ToString());
                orderNote += "-" + SetCardinal(order["SignatureVerification"].ToString());

                orderNote += "\r\n\r\nPhone: " + order[8].ToString() + "\r\nEmail: " + order[6].ToString();
                if (referred_name.Length > 0)
                {
                    orderNote += "\r\n\r\nReferral Name: " + referred_name + "\r\nReferral ID: " + referred_id + "\r\n";
                }

                if (hasPackage)
                {
                    orderNote += "\r\nVehicle Package";
                }

                if (hasGiftCard)
                {
                    int cardIndex = 1;
                    foreach (KeyValuePair<string, double> giftCard in giftCardList)
                    {
                        orderNote += $"\r\nGift Card Number {cardIndex}: {giftCard.Key}";
                        cardIndex++;
                    }
                }

                if (customBar != "")
                {
                    orderNote += "\r\n" + customBar;
                }

                if (shipping_method.Trim().ToLower().Equals("ups ground free shipping") || shipping_method.Trim().ToLower().Equals("free shipping"))
                {
                    carrier = "UPS";
                    string expression = "Name = 'Ground'";
                    DataRow[] foundRows;

                    // Use the Select method to find all rows matching the filter.
                    foundRows = carrierServices.Select(expression);
                    foreach (DataRow fr in foundRows)
                    {
                        service = "<CarrierServiceID>" + fr[0].ToString() + "</CarrierServiceID>";
                    }
                }
                else if (shipping_method.Trim().ToLower().Equals("ups surepost free shipping"))
                {
                    carrier = "UPS";
                    string expression = "Name = 'SurePost'";
                    DataRow[] foundRows = carrierServices.Select(expression);
                    foreach (DataRow fr in foundRows)
                    {
                        service = "<CarrierServiceID>" + fr[0].ToString() + "</CarrierServiceID>";
                    }
                }
                else
                {
                    if (shipping_method.Trim().ToLower().Contains("international"))
                    {
                        carrier = "UPS International";
                        shipping_method = shipping_method.Trim().ToLower().Replace("ups international ", "").Replace("worldwide ", "");

                        string expression = "Name = '" + shipping_method + "'";
                        DataRow[] foundRows;

                        // Use the Select method to find all rows matching the filter.
                        foundRows = carrierServices.Select(expression);
                        foreach (DataRow fr in foundRows)
                        {
                            service = "<CarrierServiceID>" + fr[0].ToString() + "</CarrierServiceID>";
                        }
                    }
                    else
                    {
                        shipping_method = shipping_method.Trim().ToLower().Replace("ups ", "").Replace("second", "2nd").Replace("three", "3").Replace("-", " ");
                        if (isInternational && shipping_method.Contains("access point delivery"))
                        {
                            carrier = "UPS International";
                            shipping_method = shipping_method.Trim().ToLower().Replace("ups international ", "").Replace("worldwide ", "");

                            string expression = "Name = 'international " + shipping_method + "'";
                            DataRow[] foundRows;

                            // Use the Select method to find all rows matching the filter.
                            foundRows = carrierServices.Select(expression);
                            foreach (DataRow fr in foundRows)
                            {
                                service = "<CarrierServiceID>" + fr[0].ToString() + "</CarrierServiceID>";
                            }
                        }
                        else
                        {
                            carrier = "UPS";
                            string expression = "Name = '" + shipping_method + "'";
                            DataRow[] foundRows;

                            // Use the Select method to find all rows matching the filter.
                            foundRows = carrierServices.Select(expression);
                            foreach (DataRow fr in foundRows)
                            {
                                service = "<CarrierServiceID>" + fr[0].ToString() + "</CarrierServiceID>";
                            }
                        }
                    }
                }

                xml = @"<SalesOrder><Note>[*REPLACE_NOTES*]</Note><Salesman>admin</Salesman><Carrier>" + carrier + "</Carrier>" + service;

                string tax = "true";
                string coName = order[7].ToString().Trim();
                coName = coName.Length > 38 ? WebUtility.HtmlEncode(coName.Substring(0, 37)) : WebUtility.HtmlEncode(coName);
                if (orderType.ToLower().Trim() == "dealer")
                {
                    xml += "<TaxRateName>None</TaxRateName>";
                    tax = "false";
                }
                else if (order["ship_state"].ToString().ToLower() == "tx" || order["ship_state"].ToString().ToLower() == "texas" || order["ship_state"].ToString().ToLower() == "tx.")
                {
                    xml += "<TaxRateName>Texas</TaxRateName>";
                    tax = "true";

                    if (tax_exempt.ToLower() == "y")
                    {
                        xml = xml.Replace("<TaxRateName>Texas</TaxRateName>", "<TaxRateName>None</TaxRateName>");
                        tax = "false";
                    }
                }
                else
                {
                    xml += "<TaxRateName>None</TaxRateName>";
                    tax = "false";
                }

                xml += "<PaymentTerms>CCD</PaymentTerms><CustomerPO>" + order["order_id"].ToString() + "</CustomerPO>";
                xml += "<CustomerName>" + WebUtility.HtmlEncode(order["fishbowl_id"].ToString()) + "</CustomerName>";

                string fullName = order["first_name"].ToString().Trim() + " " + order["last_name"].ToString().Trim();
                string nameSuffix = order["suffix"].ToString().Trim().Length > 0 ? order["suffix"].ToString().Trim() : "";
                fullName = nameSuffix.Length > 0 ? fullName + " " + nameSuffix : fullName;
                fullName = fullName.Length > 38 ? fullName.Substring(0, 37) : fullName;

                //Bill Address
                string billApt = WebUtility.HtmlEncode(order["bill_apt"].ToString().Trim());
                string billAptSuffix = WebUtility.HtmlEncode(order["bill_apt_suffix"].ToString().Trim());
                string billingAddress = WebUtility.HtmlEncode(order["bill_street"].ToString().Trim());
                if (billApt.Length > 0 && billAptSuffix.Length > 0)
                {
                    billingAddress += "&#xA;" + billAptSuffix + " " + billApt;
                }
                else
                {
                    if (billApt.Length > 0 && billAptSuffix.Length == 0)
                    {
                        billingAddress += "&#xA;" + billApt;
                    }

                    if (billApt.Length == 0 && billAptSuffix.Length > 0)
                    {
                        billingAddress += "&#xA;" + billAptSuffix;
                    }
                }


                if (coName.Length <= 0)
                {
                    xml += "<BillTo><Name>" + fullName + "</Name><AddressField>" + billingAddress + "</AddressField>";
                    xml += "<City>" + order["bill_city"].ToString() + "</City><Zip>" + order["bill_zip"].ToString() + "</Zip><Country>" + order["bill_country"].ToString() + "</Country>";
                    xml += "<State>" + order["bill_state"].ToString() + "</State></BillTo>";
                }
                else
                {
                    if (IsBadName(coName))
                    {
                        xml += "<BillTo><Name>" + fullName + "</Name><AddressField>" + billingAddress + "</AddressField>";
                    }
                    else
                    {
                        xml += "<BillTo><Name>" + coName + "</Name><AddressField>" + fullName + "&#xA;" + billingAddress + "</AddressField>";
                    }
                    xml += "<City>" + order["bill_city"].ToString() + "</City><Zip>" + order["bill_zip"].ToString() + "</Zip><Country>" + order["bill_country"].ToString() + "</Country>";
                    xml += "<State>" + order["bill_state"].ToString() + "</State></BillTo>";
                }

                xml += "<Ship>";
                if (coName.Length > 0)
                {
                    if (IsBadName(coName))
                    {
                        xml += "<Name>" + fullName + "</Name>";
                    }
                    else
                    {
                        xml += "<Name>" + coName + "</Name>";
                    }
                }
                else
                {
                    xml += "<Name>" + fullName + "</Name>";
                }

                string shipApt = WebUtility.HtmlEncode(order["ship_apt"].ToString().Trim());
                string shipAptSuffix = WebUtility.HtmlEncode(order["ship_apt_suffix"].ToString().Trim());
                string shipAddress = WebUtility.HtmlEncode(order["ship_street"].ToString().Trim());
                string attention = WebUtility.HtmlEncode(order["attention"].ToString().Trim());

                if (shipping_method == "access point delivery")
                {
                    xml += "<AddressField>" + shipApt + "&#xA;" + shipAddress.Replace("&lt;br/&gt;", " ") + "</AddressField>";
                }
                else
                {
                    if (shipApt.Length > 0 && shipAptSuffix.Length > 0)
                    {
                        xml += "<AddressField>" + shipAddress + "&#xA;" + shipAptSuffix + " " + shipApt + "</AddressField>";
                    }
                    else
                    {
                        if (shipAptSuffix.Length > 0 && shipApt.Length == 0)
                        {
                            xml += "<AddressField>" + shipAddress + "&#xA;" + shipAptSuffix + "</AddressField>";
                        }
                        else if (shipAptSuffix.Length == 0 && shipApt.Length > 0)
                        {
                            xml += "<AddressField>" + shipAddress + "&#xA;" + shipApt + "</AddressField>";
                        }
                        else
                        {
                            xml += "<AddressField>" + shipAddress + "</AddressField>";
                        }
                    }
                }

                xml += "<City>" + order["ship_city"].ToString().Trim() + "</City><Zip>" + order["ship_zip"].ToString().Trim() + "</Zip><Country>" + order["ship_country"].ToString().Trim() + "</Country>";
                xml += "<State>" + order["ship_state"].ToString().Trim() + "</State></Ship>";
                if (attention.Length > 0)
                {
                    xml += "<CustomerContact>" + attention + "</CustomerContact>";
                }
                else
                {
                    xml += "<CustomerContact>" + fullName + "</CustomerContact>";
                }

                xml += "<CustomFields>";
                xml += "<CustomField><ID>13</ID><Name>CF-Order Type</Name><SortOrder>1</SortOrder><Info>Order</Info><RequiredFlag>true</RequiredFlag><ActiveFlag>true</ActiveFlag></CustomField>";
                xml += "<CustomField><ID>15</ID><Name>CF-Sales Rep</Name><SortOrder>1</SortOrder><Info>" + referred_name + "</Info><RequiredFlag>true</RequiredFlag><ActiveFlag>true</ActiveFlag></CustomField>";
                xml += "</CustomFields>";
                xml += "<*MEMO*>";

                //Get the attributes and put the info into arrays.
                //List products
                xml += "<Items>";
                int rowCount = 0;

                //List<string> boxNotes = new List<string>();
                DataTable boxNotes = new DataTable();
                boxNotes.Columns.Add("prodId", typeof(int));
                boxNotes.Columns.Add("prodSku", typeof(string));
                boxNotes.Columns.Add("qty", typeof(int));
                boxNotes.Columns.Add("box", typeof(string));

                List<string> appliedCategoryCoupon = new List<string>();
                List<string> couponCategoryIds = getListOfCouponCategoryIds(order["coupon_code"].ToString().Trim());
                if (couponCategoryIds.Count > 0)
                {
                    //Reorder the orderDetails table.
                    EnumerableRowCollection<DataRow> orderDetailResults = orderDetails.AsEnumerable().Where((s) =>
                    {
                        string[] c = s["CategoryIds"].ToString().Split(',');
                        return couponCategoryIds.Any(x => c.Any(y => y == x));
                    });
                    if (orderDetailResults.Count() > 0)
                    {
                        DataTable dt1 = orderDetailResults.CopyToDataTable();
                        List<string> cartIds = dt1.AsEnumerable().Select(x => x["cart_id"].ToString()).Distinct().ToList();

                        EnumerableRowCollection<DataRow> orderDetailResultsNot = orderDetails.AsEnumerable().Where(s => !cartIds.Contains(s["cart_id"].ToString()));
                        if (orderDetailResultsNot.Count() > 0)
                        {
                            dt1.Merge(orderDetailResultsNot.CopyToDataTable());
                            dt1.AcceptChanges();
                        }
                        orderDetails = dt1;
                    }
                }
                foreach (DataRow row in orderDetails.Rows)
                {
                    bool showItem = true;
                    string mountXml = "";
                    string attributeNotes = "";
                    double price = 0.00;
                    double attribPrice = 0.00;
                    price = Convert.ToDouble(row["purchase_price"]);

                    string prodId = row["prod_id"].ToString();
                    string prodName = row["prod_sku"].ToString();
                    string prodDesc = WebUtility.HtmlEncode(row["prod_name"].ToString());
                    var primary_attribs = JsonConvert.DeserializeObject<List<string>>(row["primary_attrib"].ToString());
                    string pack = row["n_packs"].ToString();
                    int discountPack = Convert.ToInt32(row["DiscountPack"]);
                    //string catId = row["category_for_url"].ToString();
                    string[] prodCatIds = row["CategoryIds"].ToString().Split(',');
                    string lightbarType = row["lightbar_type"].ToString().Trim();

                    if (row["taxable"].ToString() == "N" || hasTaxFreeCoupon)
                    {
                        tax = "false";
                    }

                    double warrantyPrice = 0;
                    if (row["aplus_warranty"].ToString() == "Y")
                    {
                        attributeNotes += "A+ Warranty + $" + string.Format("{0:C}", row["warranty"].ToString()) + "\n  ";
                        warrantyPrice = Convert.ToDouble(row["warranty"]);
                    }

                    double itemPrice = price + warrantyPrice;
                    int row_qty = Convert.ToInt32(row["quantity"]);

                    string[] attributes = row["attributes"].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    int ruleid = (int)row["fishbowl_rule_id"];
                    if (ruleid != 7)
                    {
                        ruleid = 7;
                    }

                    switch (ruleid)
                    {
                        case 7: //New one to one
                            switch (pack.ToLower())
                            {
                                case "bundles":
                                    showItem = false;
                                    foreach (string attribute in attributes)
                                    {
                                        string[] attribValues = attribute.Split(new string[] { "|" }, StringSplitOptions.None);

                                        //Add misc sale item if price is greater than zero?
                                        if (Convert.ToDouble(attribValues[2]) > 0 && attribValues.Length <= 4)
                                        {
                                            int attribQty = Convert.ToInt32(row["quantity"]) * Convert.ToInt32(attribValues[3]);

                                            double pricePer = 0.0;
                                            pricePer = Convert.ToDouble(attribValues[2]) / Convert.ToDouble(attribValues[3]);

                                            attribPrice += Convert.ToDouble(attribValues[2]);
                                            mountXml += "<SalesOrderItem><ProdID>" + prodId + "</ProdID><ProductNumber>" + WebUtility.HtmlEncode(prodName) + "</ProductNumber><Description>" + WebUtility.HtmlEncode(attribValues[1].ToString()) + "</Description><Taxable>" + tax + "</Taxable><Quantity>" + attribQty + "</Quantity><ProductPrice>" + pricePer + "</ProductPrice><TotalPrice>" + attribValues[2].ToString() + "</TotalPrice><UOMCode>ea</UOMCode><ItemType>11</ItemType>";
                                            mountXml += "<Note>" + attributeNotes + "</Note><NewItemFlag>false</NewItemFlag></SalesOrderItem>";
                                            boxNotes.Rows.Add(prodId, prodName, attribQty);
                                        }

                                        if (attribValues.Length > 4)
                                        {
                                            prodName = attribValues[4].ToString();
                                            int attribQty = Convert.ToInt32(row["quantity"]) * Convert.ToInt32(attribValues[3]);

                                            double pricePer = 0.0;
                                            if (primary_attribs != null)
                                            {
                                                if (!primary_attribs.Contains(attribValues[0]))
                                                {
                                                    pricePer = Convert.ToDouble(attribValues[2]) / Convert.ToDouble(attribValues[3]);
                                                }
                                                else
                                                {
                                                    pricePer = (itemPrice + Convert.ToDouble(attribValues[2])) / Convert.ToDouble(attribValues[3]);
                                                }
                                            }
                                            else
                                            {
                                                pricePer = Convert.ToDouble(attribValues[2]) / Convert.ToDouble(attribValues[3]);
                                            }

                                            attribPrice += Convert.ToDouble(attribValues[2]);

                                            mountXml += "<SalesOrderItem><ProdID>" + prodId + "</ProdID><ProductNumber>" + WebUtility.HtmlEncode(attribValues[4].ToString()) + "</ProductNumber><Description>" + WebUtility.HtmlEncode(attribValues[1].ToString()) + "</Description><Taxable>" + tax + "</Taxable><Quantity>" + attribQty + "</Quantity><ProductPrice>" + pricePer + "</ProductPrice><TotalPrice>" + attribValues[2].ToString() + "</TotalPrice><UOMCode /><ItemType>10</ItemType>";
                                            mountXml += "<Note>" + attributeNotes + "</Note><NewItemFlag>false</NewItemFlag></SalesOrderItem>";
                                            boxNotes.Rows.Add(prodId, attribValues[4].ToString(), attribQty);
                                        }
                                    }
                                    break;
                                case "none":
                                    showItem = true;
                                    //If pack is none and it has a primary attribute. Do not show the product sku and use the attribute SKUs.
                                    foreach (string attribute in attributes)
                                    {
                                        string[] attribValues = attribute.Split(new string[] { "|" }, StringSplitOptions.None);
                                        if (primary_attribs != null)
                                        {
                                            foreach (string primary_attrib in primary_attribs)
                                            {
                                                if (primary_attrib.Trim() != "" && attribValues.Contains(primary_attrib))
                                                    showItem = false;
                                            }
                                        }

                                        //Add misc sale item if price is greater than zero?
                                        if (Convert.ToDouble(attribValues[2]) > 0 && attribValues.Length <= 4)
                                        {
                                            int attribQty = Convert.ToInt32(row["quantity"]) * Convert.ToInt32(attribValues[3]);

                                            double pricePer = 0.0;
                                            pricePer = Convert.ToDouble(attribValues[2]) / Convert.ToDouble(attribValues[3]);

                                            attribPrice += Convert.ToDouble(attribValues[2]);
                                            mountXml += "<SalesOrderItem><ProdID>" + prodId + "</ProdID><ProductNumber>" + WebUtility.HtmlEncode(prodName) + "</ProductNumber><Description>" + WebUtility.HtmlEncode(attribValues[1].ToString()) + "</Description><Taxable>" + tax + "</Taxable><Quantity>" + attribQty + "</Quantity><ProductPrice>" + pricePer + "</ProductPrice><TotalPrice>" + attribValues[2].ToString() + "</TotalPrice><UOMCode>ea</UOMCode><ItemType>11</ItemType>";
                                            mountXml += "<Note>" + attributeNotes + "</Note><NewItemFlag>false</NewItemFlag></SalesOrderItem>";
                                            boxNotes.Rows.Add(prodId, prodName, attribQty);
                                        }

                                        if (attribValues.Length > 4)
                                        {
                                            if (!showItem)
                                            {
                                                prodName = attribValues[4].ToString();
                                            }
                                            int attribQty = Convert.ToInt32(row["quantity"]) * Convert.ToInt32(attribValues[3]);

                                            double pricePer = 0.0;
                                            if (primary_attribs != null)
                                            {
                                                if (!primary_attribs.Contains(attribValues[0]))
                                                {
                                                    pricePer = Convert.ToDouble(attribValues[2]) / Convert.ToDouble(attribValues[3]);
                                                }
                                                else
                                                {
                                                    pricePer = (itemPrice + Convert.ToDouble(attribValues[2])) / Convert.ToDouble(attribValues[3]);
                                                }
                                            }
                                            else
                                            {
                                                pricePer = Convert.ToDouble(attribValues[2]) / Convert.ToDouble(attribValues[3]);
                                            }

                                            attribPrice += Convert.ToDouble(attribValues[2]);

                                            mountXml += "<SalesOrderItem><ProdID>" + prodId + "</ProdID><ProductNumber>" + WebUtility.HtmlEncode(attribValues[4].ToString()) + "</ProductNumber><Description>" + WebUtility.HtmlEncode(attribValues[1].ToString()) + "</Description><Taxable>" + tax + "</Taxable><Quantity>" + attribQty + "</Quantity><ProductPrice>" + pricePer + "</ProductPrice><TotalPrice>" + attribValues[2].ToString() + "</TotalPrice><UOMCode /><ItemType>10</ItemType>";
                                            mountXml += "<Note>" + attributeNotes + "</Note><NewItemFlag>false</NewItemFlag></SalesOrderItem>";
                                            boxNotes.Rows.Add(prodId, attribValues[4].ToString(), attribQty);
                                        }
                                    }
                                    break;
                                default:
                                    showItem = false;
                                    pack = pack.ToLower();
                                    int packIndex = pack.LastIndexOf(" pack");
                                    int prodCount = Convert.ToInt32(pack.Substring(0, packIndex));
                                    var altRegex = new Regex(@"(^Color)|(^Model)");

                                    // This product is a discount series pack so combine the products.
                                    if (discountPack >= 1)
                                    {
                                        List<string> attribList = new List<string>();
                                        attributes = FixDiscountAttribute(attributes);

                                        var newAttributes = attributes.GroupBy(x => x)
                                            .Select(x => new { x.Key, Count = x.Count() })
                                            .ToList();

                                        foreach (var a in newAttributes)
                                        {
                                            string[] aValues = a.Key.Split(new string[] { "|" }, StringSplitOptions.None);

                                            //replace the quantity
                                            //if it's a pack and the attribute count > 1 check to see if there's an attribute count.
                                            if (a.Count > 1)
                                            {
                                                aValues[3] = a.Count.ToString();
                                            }

                                            attribList.Add(string.Join("|", aValues));
                                        }

                                        altRegex = new Regex(@"(^DiscountPack)");
                                        foreach (var attribute in attribList.ToArray().Where(x => altRegex.IsMatch(x)))
                                        {
                                            string[] attribValues = attribute.Split(new string[] { "|" }, StringSplitOptions.None);
                                            if (attribValues.Length > 4)
                                            {
                                                prodName = attribValues[4].ToString();
                                                int qty = Convert.ToInt32(row["quantity"]);
                                                int attribQty = qty * Convert.ToInt32(attribValues[3]);

                                                double pricePer = itemPrice / prodCount + Convert.ToDouble(attribValues[2]);

                                                mountXml += "<SalesOrderItem><ProdID>" + prodId + "</ProdID><ProductNumber>" + prodName + "</ProductNumber><Description>" + WebUtility.HtmlEncode(attribValues[1].ToString()) + "</Description><Taxable>" + tax + "</Taxable><Quantity>" + attribQty + "</Quantity><ProductPrice>" + pricePer + "</ProductPrice><TotalPrice>" + attribValues[2].ToString() + "</TotalPrice><UOMCode /><ItemType>10</ItemType>";
                                                mountXml += "<Note>" + attributeNotes + "</Note><NewItemFlag>false</NewItemFlag></SalesOrderItem>";
                                                boxNotes.Rows.Add(prodId, prodName, attribQty);
                                            }
                                        }

                                        foreach (var attribute in attributes.Where(x => !altRegex.IsMatch(x)))
                                        {
                                            string[] mountAttr = attribute.Split(new string[] { "|" }, StringSplitOptions.None);
                                            int mountQty = Convert.ToInt16(mountAttr[3]);
                                            int prodQty = Convert.ToInt16(row["quantity"]);
                                            int attribQty = prodQty * mountQty;
                                            double pricePer = Convert.ToDouble(mountAttr[2]);
                                            if (prodQty > 1 && mountQty > 1)
                                            {
                                                pricePer = pricePer * prodQty / attribQty;
                                            }
                                            else if (prodQty == 1 && mountQty > 1)
                                            {
                                                pricePer = pricePer / attribQty;
                                            }

                                            if (mountAttr[0].ToString().ToLower().Trim() == "mounting bracket")
                                            {
                                                if (mountAttr[1].ToString().ToLower().Trim() != "none")
                                                {
                                                    mountXml += "<SalesOrderItem><ProdID>" + prodId + "</ProdID><ProductNumber>" + WebUtility.HtmlEncode(mountAttr[4].ToString()) + "</ProductNumber><Description>" + WebUtility.HtmlEncode(mountAttr[1].ToString()) + "</Description><Taxable>" + tax + "</Taxable><Quantity>" + attribQty + "</Quantity><ProductPrice>" + pricePer + "</ProductPrice><TotalPrice>" + mountAttr[2].ToString() + "</TotalPrice><UOMCode /><ItemType>10</ItemType>";
                                                    mountXml += "<Note>" + attributeNotes + "</Note><NewItemFlag>false</NewItemFlag></SalesOrderItem>";
                                                    boxNotes.Rows.Add(prodId, mountAttr[4].ToString(), attribQty);
                                                }
                                            }
                                            else if (mountAttr.Length > 4)
                                            {
                                                mountXml += "<SalesOrderItem><ProdID>" + prodId + "</ProdID><ProductNumber>" + WebUtility.HtmlEncode(mountAttr[4].ToString()) + "</ProductNumber><Description>" + WebUtility.HtmlEncode(mountAttr[1].ToString()) + "</Description><Taxable>" + tax + "</Taxable><Quantity>" + attribQty + "</Quantity><ProductPrice>" + pricePer + "</ProductPrice><TotalPrice>" + mountAttr[2].ToString() + "</TotalPrice><UOMCode /><ItemType>10</ItemType>";
                                                mountXml += "<Note>" + attributeNotes + "</Note><NewItemFlag>false</NewItemFlag></SalesOrderItem>";
                                                boxNotes.Rows.Add(prodId, mountAttr[4].ToString(), attribQty);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (var attribute in attributes.Where(x => altRegex.IsMatch(x)))
                                        {
                                            string[] attribValues = attribute.Split(new string[] { "|" }, StringSplitOptions.None);
                                            if (attribValues.Length > 4)
                                            {
                                                prodName = attribValues[4].ToString();
                                                int attribQty = Convert.ToInt32(row["quantity"]) * Convert.ToInt32(attribValues[3]);

                                                var qty = Convert.ToInt32(row["quantity"]);
                                                double pricePer = itemPrice / prodCount + Convert.ToDouble(attribValues[2]);

                                                mountXml += "<SalesOrderItem><ProdID>" + prodId + "</ProdID><ProductNumber>" + prodName + "</ProductNumber><Description>" + WebUtility.HtmlEncode(attribValues[1].ToString()) + "</Description><Taxable>" + tax + "</Taxable><Quantity>" + qty + "</Quantity><ProductPrice>" + pricePer + "</ProductPrice><TotalPrice>" + attribValues[2].ToString() + "</TotalPrice><UOMCode /><ItemType>10</ItemType>";
                                                mountXml += "<Note>" + attributeNotes + "</Note><NewItemFlag>false</NewItemFlag></SalesOrderItem>";
                                                boxNotes.Rows.Add(prodId, prodName, qty);
                                            }
                                        }

                                        foreach (var attribute in attributes.Where(x => !altRegex.IsMatch(x)))
                                        {
                                            string[] mountAttr = attribute.Split(new string[] { "|" }, StringSplitOptions.None);
                                            int mountQty = Convert.ToInt16(mountAttr[3]);
                                            int prodQty = Convert.ToInt16(row["quantity"]);
                                            int attribQty = prodQty * mountQty;
                                            double pricePer = Convert.ToDouble(mountAttr[2]);
                                            if (prodQty > 1 && mountQty > 1)
                                            {
                                                pricePer = pricePer * prodQty / attribQty;
                                            }
                                            else if (prodQty == 1 && mountQty > 1)
                                            {
                                                pricePer = pricePer / attribQty;
                                            }

                                            if (mountAttr[0].ToString().ToLower().Trim() == "mounting bracket")
                                            {
                                                if (mountAttr[1].ToString().ToLower().Trim() != "none")
                                                {
                                                    mountXml += "<SalesOrderItem><ProdID>" + prodId + "</ProdID><ProductNumber>" + WebUtility.HtmlEncode(mountAttr[4].ToString()) + "</ProductNumber><Description>" + WebUtility.HtmlEncode(mountAttr[1].ToString()) + "</Description><Taxable>" + tax + "</Taxable><Quantity>" + attribQty + "</Quantity><ProductPrice>" + pricePer + "</ProductPrice><TotalPrice>" + mountAttr[2].ToString() + "</TotalPrice><UOMCode /><ItemType>10</ItemType>";
                                                    mountXml += "<Note>" + attributeNotes + "</Note><NewItemFlag>false</NewItemFlag></SalesOrderItem>";
                                                    boxNotes.Rows.Add(prodId, mountAttr[4].ToString(), attribQty);
                                                }
                                            }
                                            else if (mountAttr.Length > 4)
                                            {
                                                mountXml += "<SalesOrderItem><ProdID>" + prodId + "</ProdID><ProductNumber>" + WebUtility.HtmlEncode(mountAttr[4].ToString()) + "</ProductNumber><Description>" + WebUtility.HtmlEncode(mountAttr[1].ToString()) + "</Description><Taxable>" + tax + "</Taxable><Quantity>" + attribQty + "</Quantity><ProductPrice>" + pricePer + "</ProductPrice><TotalPrice>" + mountAttr[2].ToString() + "</TotalPrice><UOMCode /><ItemType>10</ItemType>";
                                                mountXml += "<Note>" + attributeNotes + "</Note><NewItemFlag>false</NewItemFlag></SalesOrderItem>";
                                                boxNotes.Rows.Add(prodId, mountAttr[4].ToString(), attribQty);
                                            }
                                        }
                                    }

                                    break;
                            }
                            break;
                        default:
                            foreach (string attribute in attributes)
                            {
                                string[] attribValues = attribute.Split(new string[] { "|" }, StringSplitOptions.None);
                                int attribQty = Convert.ToInt32(row["quantity"]) * Convert.ToInt32(attribValues[3]);
                                double pricePer = Convert.ToDouble(attribValues[2]) / Convert.ToDouble(attribValues[3]);

                                if (attribValues.Length > 4)
                                {
                                    if (Convert.ToDouble(attribValues[3]) > 0)
                                    {
                                        mountXml += "<SalesOrderItem><ProdID>" + prodId + "</ProdID><ProductNumber>" + WebUtility.HtmlEncode(attribValues[4].ToString()) + "</ProductNumber><Description>" + WebUtility.HtmlEncode(attribValues[1].ToString()) + "</Description><Taxable>" + tax + "</Taxable><Quantity>" + attribQty + "</Quantity><ProductPrice>" + pricePer + "</ProductPrice><TotalPrice>" + attribValues[2].ToString() + "</TotalPrice><UOMCode /><ItemType>10</ItemType>";
                                        mountXml += "<Note/><NewItemFlag>false</NewItemFlag></SalesOrderItem>";
                                        boxNotes.Rows.Add(prodId, attribValues[4].ToString(), attribQty);
                                    }
                                }
                                else
                                {
                                    attribPrice += Convert.ToDouble(attribValues[2]);
                                    attributeNotes += WebUtility.HtmlEncode(attribValues[0].ToString()) + ": " + WebUtility.HtmlEncode(attribValues[1].ToString()) + " + $" + string.Format("{0:C}", attribValues[2]) + "\n  ";
                                }
                            }
                            break;
                    }

                    double eaPrice = itemPrice;
                    double eaPriceTotal = eaPrice * row_qty;

                    string totalEachPrice = eaPrice.ToString();
                    string totalPrice = eaPriceTotal.ToString();

                    orderCoupons = cleanDiscountList(row["prod_id"].ToString().Trim(), order);
                    List<Coupon> categoryCoupons = orderCoupons.Where((p) =>
                    {
                        string[] cats = p.cat_id.Split(',');
                        return cats.Intersect(prodCatIds).Any();
                    }).ToList();

                    if (showItem)
                    {
                        xml += "<SalesOrderItem><ProdID>" + prodId + "</ProdID><ProductNumber>" + prodName + "</ProductNumber><Description>" + prodDesc +
                                "</Description><Taxable>" + tax + "</Taxable><Quantity>" + row_qty +
                                "</Quantity><ProductPrice>" + totalEachPrice + "</ProductPrice><TotalPrice></TotalPrice><ItemType>10</ItemType>";
                        xml += "<Note>" + attributeNotes + "</Note><NewItemFlag>false</NewItemFlag></SalesOrderItem>";
                        boxNotes.Rows.Add(prodId, prodName, row_qty);

                        if (row["product_type"].ToString() == "clearance")
                        {
                            xml += "<SalesOrderItem><ProdID>" + prodId + "</ProdID><ProductNumber>CLEARANCE</ProductNumber><Description /><Taxable>" + tax + "</Taxable><Quantity>" + row_qty + "</Quantity><ProductPrice>0</ProductPrice><TotalPrice>0</TotalPrice><UOMCode /><ItemType>10</ItemType>";
                            xml += "<Note/><NewItemFlag>false</NewItemFlag></SalesOrderItem>";
                        }


                        if (categoryCoupons.Count() > 0)
                        {
                            xml += BuildFinalDiscountXml(categoryCoupons.ToList());
                            foreach (Coupon c in categoryCoupons)
                            {
                                appliedCategoryCoupon.Add(c.coupon_code);
                            }
                        }
                    }

                    if (mountXml != "")
                    {
                        xml += mountXml;
                        if (categoryCoupons.Count() > 0)
                        {
                            xml += BuildFinalDiscountXml(categoryCoupons.ToList());
                            foreach (Coupon c in categoryCoupons)
                            {
                                appliedCategoryCoupon.Add(c.coupon_code);
                            }
                        }
                    }

                    if (rowCount != orderDetails.Rows.Count - 1)
                    {
                        //Check next row to see if change of package.
                        if (row["package_id"].ToString() != orderDetails.Rows[rowCount + 1]["package_id"].ToString())
                        {
                            //End of package. Add Subtotal.
                            xml += "<SalesOrderItem><ProductNumber/><Description>Package Subtotal</Description><Taxable>" + tax + "</Taxable><Quantity>1</Quantity><ProductPrice/><TotalPrice/><UOMCode /><ItemType>40</ItemType><Status>10</Status><Note/><NewItemFlag>false</NewItemFlag><AdjustmentAmount/></SalesOrderItem>";
                        }
                    }
                    else if (rowCount == orderDetails.Rows.Count - 1 && row["package_id"].ToString().Length > 0)
                    {
                        xml += "<SalesOrderItem><ProductNumber/><Description>Package Subtotal</Description><Taxable>" + tax + "</Taxable><Quantity>1</Quantity><ProductPrice/><TotalPrice/><UOMCode /><ItemType>40</ItemType><Status>10</Status><Note/><NewItemFlag>false</NewItemFlag><AdjustmentAmount/></SalesOrderItem>";
                    }

                    xml += BuildDiscountXml(row, order["coupon_code"].ToString());

                    rowCount++;
                }

                //Add Final discounts
                orderCoupons.RemoveAll((c) =>
                {
                    return appliedCategoryCoupon.Distinct().Contains(c.coupon_code);
                });
                xml += BuildFinalDiscountXml(orderCoupons);

                //Add giftcard if any
                if (hasGiftCard)
                {
                    foreach (KeyValuePair<string, double> giftCard in giftCardList)
                    {
                        xml += "<SalesOrderItem><ProductNumber>GC-000</ProductNumber><Description /><Taxable>false</Taxable><Quantity>1</Quantity><ProductPrice>(" + giftCard.Value + ")</ProductPrice><TotalPrice/><UOMCode>ea</UOMCode><ItemType>10</ItemType>";
                        xml += "<Note/><NewItemFlag>false</NewItemFlag></SalesOrderItem>";
                    }
                }

                //Add priority pricing.
                if (priorityProcessing == 1)
                {
                    var priorityPrice = "12.99";
                    if (containsCustomLightbar == 1)
                    {
                        priorityPrice = "18.99";
                    }

                    xml += "<SalesOrderItem><ProductNumber>Priority Processing</ProductNumber><Description /><Taxable>" + tax + "</Taxable><Quantity>1</Quantity><ProductPrice>" + priorityPrice + "</ProductPrice><TotalPrice/><UOMCode /><ItemType>10</ItemType>";
                    xml += "<Note/><NewItemFlag>false</NewItemFlag></SalesOrderItem>";
                }

                //Add reward claims
                if (points_claimed > 0)
                {
                    DateTime orderTime = DateTime.Parse(order["create_date"].ToString());
                    DateTime testTime = DateTime.Parse("11/11/2016 05:56:59");
                    string rewardPrice = "0.00";
                    if (orderTime < testTime)
                    {
                        rewardPrice = (points_claimed / 50.00).ToString();
                    }
                    else
                    {
                        rewardPrice = (points_claimed / 100.00).ToString();
                    }

                    xml += "<SalesOrderItem><ProductNumber>RP-000</ProductNumber><Description /><Taxable>false</Taxable><Quantity>1</Quantity><ProductPrice>(" + rewardPrice + ")</ProductPrice><TotalPrice/><UOMCode /><ItemType>10</ItemType>";
                    xml += "<Note/><NewItemFlag>false</NewItemFlag></SalesOrderItem>";
                }

                //Add the Shipping Info
                string shipMethod = order["shipping_method"].ToString();
                shipMethod = shipMethod.Replace("*", "").Replace("<sup>", "").Replace("</sup>", "").Replace("®", "");

                foreach (Coupon c in orderCoupons)
                {
                    if (c.coupon_code == "STLFREESHIP" || c.coupon_code == "FREESHIP")
                    {
                        if (shipping_method.Trim().ToLower().Equals("ups ground free shipping") || shipping_method.Trim().ToLower().Equals("free shipping"))
                        {
                            shipMethod = "Free Ground Shipping";
                        }
                    }

                    if (c.coupon_code == "STLTAXFREE")
                    {
                        xml = xml.Replace("<TaxRateName>Texas</TaxRateName>", "<TaxRateName>None</TaxRateName>");
                        xml = xml.Replace("<Taxable>true</Taxable>", "<Taxable>false</Taxable>");
                    }
                }

                if (xml.IndexOf("<TaxRateName>None</TaxRateName>") > 0)
                {
                    xml = xml.Replace("<Taxable>true</Taxable>", "<Taxable>false</Taxable>");
                }

                if (shipMethod != "None")
                {
                    if (shipMethod.Equals("Fedex 2 Day"))
                    {
                        shipMethod = "FedEx 2-Day";
                    }

                    if (shipMethod.Equals("UPS Three-Day Select"))
                    {
                        shipMethod = "UPS Three Day Select";
                    }

                    if (shipMethod.Trim().ToLower().Equals("free shipping"))
                    {
                        shipMethod = "Free Ground Shipping";
                    }

                    if (shipMethod == "")
                    {
                        throw new Exception($"Web order {orderId} does not have a shipping method.");
                    }

                    if (isInternational && shipMethod.Equals("UPS Access Point Delivery"))
                    {
                        xml += "<SalesOrderItem><ProductNumber>UPS International " + shipMethod.Replace("UPS ", "") + "</ProductNumber><Description/><Taxable>" + tax + "</Taxable><Quantity>1</Quantity><ProductPrice>" + order["shipping_price"].ToString() + "</ProductPrice><TotalPrice>" + order["shipping_price"].ToString() + "</TotalPrice><UOMCode /><ItemType>60</ItemType><Note/><NewItemFlag>false</NewItemFlag></SalesOrderItem>";
                    }
                    else
                    {
                        xml += "<SalesOrderItem><ProductNumber>" + shipMethod + "</ProductNumber><Description/><Taxable>" + tax + "</Taxable><Quantity>1</Quantity><ProductPrice>" + order["shipping_price"].ToString() + "</ProductPrice><TotalPrice>" + order["shipping_price"].ToString() + "</TotalPrice><UOMCode /><ItemType>60</ItemType><Note/><NewItemFlag>false</NewItemFlag></SalesOrderItem>";
                    }
                }

                //Add Signature Delivery if needed.
                if (Convert.ToDecimal(order["signature_delivery"]) > 0)
                {
                    xml += "<SalesOrderItem><ProductNumber>Signature Delivery</ProductNumber><Description/><Taxable>" + tax + "</Taxable><Quantity>1</Quantity><ProductPrice>5.90</ProductPrice><TotalPrice>5.90</TotalPrice><UOMCode /><ItemType>60</ItemType><Note/><NewItemFlag>false</NewItemFlag></SalesOrderItem>";
                }

                orderNote += "\r\n";

                foreach (DataRow row in boxDt.Rows)
                {
                    string sql = "prodId=" + row["product_id"].ToString() + " AND prodSku LIKE '" + row["prod_sku"].ToString() + "%'";
                    DataRow[] selectRows = boxNotes.Select(sql);
                    if (selectRows.Length > 0)
                    {
                        row["prod_sku"] = selectRows[0]["prodSku"];
                    }

                    orderNote += "\r\n" + row["NAME"].ToString() + ", " + row["TOTAL_WEIGHT"].ToString() + ", " + row["qty"].ToString() + " " + row["prod_sku"].ToString();
                }

                xml = xml.Replace("[*REPLACE_NOTES*]", orderNote);

                if (boxDt.Rows.Count > 0)
                {

                    string JSONresult = JsonConvert.SerializeObject(boxDt);
                    string customShippingBoxes = "<Memos><Memo><Memo>{\"osco\":" + JSONresult.Replace('"', '\"') + "}</Memo><UserName>admin</UserName><DateCreated>" + time.ToString() + "</DateCreated></Memo></Memos>";
                    xml = xml.Replace("<*MEMO*>", customShippingBoxes);
                }
                else
                {
                    xml = xml.Replace("<*MEMO*>", "");
                }

                xml += "</Items></SalesOrder>";

                foreach (Coupon c in orderCoupons)
                {
                    var doc = XDocument.Parse(xml);
                    bool removedFreeShipCoupon = false;

                    if ((c.coupon_code == "FREESHIP" || c.coupon_code == "STLFREESHIP") && shipping_method.Trim().ToLower() != "free shipping")
                    {
                        if (carrier == "UPS International" && !removedFreeShipCoupon)
                        {
                            doc.Descendants("Items").Descendants("SalesOrderItem").Where(e => e.Element("ProductNumber").Value == "FREESHIP").Remove();
                            removedFreeShipCoupon = true;
                        }

                        if (carrier == "UPS" && service != "<CarrierServiceID>3</CarrierServiceID>" && !removedFreeShipCoupon)
                        {
                            doc.Descendants("Items").Descendants("SalesOrderItem").Where(e => e.Element("ProductNumber").Value == "FREESHIP").Remove();
                            removedFreeShipCoupon = true;
                        }

                        if (orderTotal < 59 && !removedFreeShipCoupon)
                        {
                            doc.Descendants("Items").Descendants("SalesOrderItem").Where(e => e.Element("ProductNumber").Value == "FREESHIP").Remove();
                            removedFreeShipCoupon = true;
                        }

                        xml = doc.ToString();
                    }
                }

                return xml;
            }
            catch
            {
                throw;
            }
        }

        public string GetCorrectCountry(string code, string countryCode)
        {
            List<KeyValuePair<string, string>> countries =
            [
                new KeyValuePair<string, string>("American Samoa", "AS"),
            new KeyValuePair<string, string>("Federated State of Micronesia", "FM"),
            new KeyValuePair<string, string>("Guam", "GU"),
            new KeyValuePair<string, string>("Marshall Islands", "MH"),
            new KeyValuePair<string, string>("Northern Mariana Islands", "MP"),
            new KeyValuePair<string, string>("Palau", "PW"),
            new KeyValuePair<string, string>("Puerto Rico", "PR"),
            new KeyValuePair<string, string>("Virgin Islands", "VI"),
        ];

            var result = countries.Where(kvp => kvp.Value == $"{code.Trim()}");
            string name = "";
            string abbreviation = countryCode;
            if (result.Count() > 0)
            {
                name = result.First().Key;
                abbreviation = result.First().Value;
            }

            return abbreviation;
        }

        public string BuildFinalDiscountXml(List<Coupon> coupons)
        {
            string xml = "";
            foreach (Coupon c in coupons)
            {
                double discountAmount = 0.00;
                string itemType = "";
                switch (c.coupon_type)
                {
                    case "dollar":
                    case "freeshipping":
                    case "salestaxwavedoff":
                        discountAmount = Convert.ToDouble(c.coupon_discount);
                        itemType = "31";
                        break;
                    case "percentage":
                        itemType = "30";
                        xml += "<SalesOrderItem><ProductNumber/><Description>Subtotal</Description><Taxable>true</Taxable><Quantity>1</Quantity><ProductPrice/><TotalPrice/><UOMCode>ea</UOMCode><ItemType>40</ItemType><Status>10</Status><Note/><NewItemFlag>false</NewItemFlag><AdjustmentAmount/></SalesOrderItem>";
                        break;
                    default:
                        itemType = "30";
                        xml += "<SalesOrderItem><ProductNumber/><Description>Subtotal</Description><Taxable>true</Taxable><Quantity>1</Quantity><ProductPrice/><TotalPrice/><UOMCode>ea</UOMCode><ItemType>40</ItemType><Status>10</Status><Note/><NewItemFlag>false</NewItemFlag><AdjustmentAmount/></SalesOrderItem>";
                        break;
                }

                xml += "<SalesOrderItem><ProductNumber>" + c.coupon_code + "</ProductNumber><Description>" + WebUtility.HtmlEncode(c.coupon_message) + "</Description><Taxable>true</Taxable><Quantity>1</Quantity><ProductPrice>(" + discountAmount + ")</ProductPrice><TotalPrice>(" + discountAmount + ")</TotalPrice><UOMCode>ea</UOMCode><ItemType>" + itemType + "</ItemType><Status>10</Status><Note/><NewItemFlag>false</NewItemFlag><AdjustmentAmount>" + discountAmount + "</AdjustmentAmount></SalesOrderItem>";
            }

            return xml;
        }

        public string BuildDiscountXml(DataRow dr, string couponCode)
        {
            string xml = "";
            if (couponCode.Length > 0)
            {
                List<Coupon> jsonCoupons = JsonConvert.DeserializeObject<List<Coupon>>(couponCode);

                var prodId = dr["prod_id"].ToString();
                int couponCount = (int)dr["quantity"];
                foreach (Coupon c in jsonCoupons)
                {
                    double discountAmount = 0.00;
                    string itemType = "";
                    var couponProdId = c.product_id;
                    if (prodId == couponProdId)
                    {
                        //Build the xml string
                        switch (c.coupon_type)
                        {
                            case "dollar":
                            case "freeshipping":
                            case "salestaxwavedoff":
                                discountAmount = Convert.ToDouble(c.coupon_discount);
                                itemType = "31";
                                break;
                            case "percentage":
                                itemType = "30";
                                break;
                        }

                        for (int x = 1; x <= couponCount; x++)
                        {
                            xml += "<SalesOrderItem><ProdID>" + prodId + "</ProdID><ProductNumber>" + c.coupon_code +
                                    "</ProductNumber><Description>COUPON: " + c.coupon_message +
                                    "</Description><Taxable>true</Taxable><Quantity>1</Quantity><ProductPrice>(" + discountAmount + ")</ProductPrice><TotalPrice>(" +
                                    discountAmount + ")</TotalPrice><UOMCode>ea</UOMCode><ItemType>" + itemType +
                                    "</ItemType><Status>10</Status><Note/><NewItemFlag>false</NewItemFlag><AdjustmentAmount>" +
                                    discountAmount + "</AdjustmentAmount></SalesOrderItem>";
                        }
                    }
                }
            }

            return xml;
        }

        public bool HasFullLightbar(DataTable dtCart)
        {
            foreach (DataRow dr in dtCart.Rows)
            {
                if (dr["lightbar_type"].ToString().Trim().IndexOf("FULL_LIGHTBAR_") > -1)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasTaxFree(DataRow dr)
        {
            int count = 0;
            List<Coupon> jsonCoupons = JsonConvert.DeserializeObject<List<Coupon>>(dr["coupon_code"].ToString());
            if (jsonCoupons != null)
            {
                foreach (Coupon c in jsonCoupons)
                {
                    if (c.coupon_code == "STLTAXFREE")
                    {
                        count++;
                    }
                }
            }

            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<string> getListOfCouponCategoryIds(string couponCode)
        {
            List<string> returnCoupons = new List<string>();
            if (couponCode.Length > 0)
            {
                List<Coupon> jsonCoupons = JsonConvert.DeserializeObject<List<Coupon>>(couponCode);

                foreach (Coupon coupon in jsonCoupons)
                {
                    if (coupon.cat_id.Length > 0 && coupon.cat_id != "0" && coupon.apply_to == "category")
                    {
                        returnCoupons.AddRange(coupon.cat_id.Split(','));
                    }
                }
            }

            return returnCoupons;
        }

        public List<Coupon> cleanDiscountList(string prodId, DataRow order)
        {
            string couponCode = order["coupon_code"].ToString().Trim();
            List<Coupon> returnCoupons = new List<Coupon>();
            if (couponCode.Length > 0)
            {
                List<Coupon> jsonCoupons = JsonConvert.DeserializeObject<List<Coupon>>(couponCode);

                //var prodId = dr["prod_id"].ToString();
                foreach (Coupon c in jsonCoupons)
                {
                    var couponProdId = c.product_id;
                    if (prodId != couponProdId)
                    {
                        returnCoupons.Add(c);
                    }
                }
            }

            int index = 0;
            bool sort = false;
            foreach (Coupon c in returnCoupons)
            {
                if (c.coupon_type == "freeshipping")
                {
                    sort = true;
                    break;
                }
                index++;
            }

            if (sort)
            {
                Coupon freeShip = returnCoupons[index];
                returnCoupons.Add(freeShip);
                returnCoupons.RemoveAt(index);
            }

            return returnCoupons;
        }

        public bool IsBadName(string coname)
        {
            // String array of bad names
            string[] badNames =
            [
                "nope",
                "none",
                "na",
                "self",
                "fire",
                "home",
                "police",
                "private",
                "vfd",
                "-",
                "mr",
                "yes",
                "no",
                "self employed",
                "individual",
                "freelance",
                "law enforcement",
                "wrecker/towing",
                "other",
                "mr. and mrs.",
                "me",
                ".",
                "volunteer",
                "white rose pilot car",
                "--------",
                "select one",
                "deputy",
                "volunteer fire",
                "city hall",
                "retired from dte energy",
                "test",
                "retired",
                "public safety",
                "off duty services",
                "fire rescue",
                "snow removal services",
                "pd",
                "mr.",
                "firefighter",
                "mastec",
                "central",
                "volunteer fireman",
                "pavetex",
                "security",
                "fire department",
                "sfd",
                "?",
                "persona;",
                "personal",
                "personal (utility locator)",
                "personal (volunteer firefighter)",
                "personal - snow plow",
                "personal account",
                "personal card",
                "personal dot",
                "personal order",
                "personal pov",
                "personal purchase",
                "personal rr contractor",
                "personal sec",
                "personal security vehicle",
                "personal truck",
                "personal use",
                "personal use volunteer firefighter",
                "personal vehicle",
                "personel",
                "pernaonal",
                "perosnal",
                "myself",
                "n//a",
                "n/a (personal use)",
                "n/a at the moment",
                "n/a us army mp",
                "n/a-gift for son-firefighter",
                "n/a.",
                "n/k",
                "n\\a",
                "n/a",
                "n-a",
                "n.a",
                "no answer",
                "no company",
                "nobody",
                "no affiliates",
                "non",
                "non-affliated",
                "none",
                "none but mine",
                "none company",
                "none mike",
                "none/personal",
                "noneya",
                "nonw",
                "nonya",
                "nope",
                "*"
            ];

            // Check if the input name exists in the bad names list
            return Array.IndexOf(badNames, coname.Trim().ToLower()) >= 0;
        }


        private string[] FixDiscountAttribute(string[] attributes)
        {
            for (int i = 0; i < attributes.Length; i++)
            {
                attributes[i] = Regex.Replace(attributes[i], @"(^Color \d*\|)|(^Model \d*\|)", "DiscountPack|");
            }

            return attributes;
        }

        private string SetCardinal(string cardinal)
        {
            if (!string.IsNullOrEmpty(cardinal))
            {
                return cardinal;
            }
            else
            {
                return "?";
            }
        }

        public XmlDocument AddLineNumbers(string doc)
        {
            XmlDocument xmlDoc = new();
            xmlDoc.LoadXml(doc);

            // Get all SalesOrderItem elements inside Items
            XmlNodeList salesOrderItems = xmlDoc.SelectNodes("//Items/SalesOrderItem");

            if (salesOrderItems != null)
            {
                int lineNumber = 1;
                foreach (XmlNode item in salesOrderItems)
                {
                    // Create new LineNumber element
                    XmlElement lineNumberElement = xmlDoc.CreateElement("LineNumber");
                    lineNumberElement.InnerText = lineNumber.ToString();

                    // Append LineNumber element to the current SalesOrderItem
                    item.AppendChild(lineNumberElement);

                    lineNumber++;
                }
            }

            return xmlDoc;
        } 
    }
}