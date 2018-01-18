using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Web;
using System.Net;
using System.Web.Script;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;


namespace CryptoPricelineUpdateService
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string url = @"https://rest.coinapi.io/v1/trades/latest";
                var client = new WebClient();
                client.Headers.Add("X-CoinAPI-Key", "BADF8493-2F65-40BC-AB81-5B49516243AC");
                var response = client.DownloadString(url);
                //string s1 = HttpUtility.HtmlDecode(response.ToString());
                List<RootObject> rbs = JsonConvert.DeserializeObject<List<RootObject>>(response);

                foreach (RootObject p in rbs)
                {                    
                    try
                    {
                        // instance my class to convert data from JSON return class
                        PricelineRecord r;

                        // break down the string symbol in the four buckets
                        string[] symbolVals = p.symbol_id.Split('_');

                        // assign MARKET, SYMBOL, CURRENCY vals
                        r = new PricelineRecord(symbolVals[0].ToString(), symbolVals[2].ToString(), symbolVals[3].ToString());
                        

                        // Price & Timestamp - Handle conversion to KRW only now
                        if (r.currency == "KRW")
                        {
                            float d = Convert.ToInt32(p.price);
                            float xratePrice = (d * Convert.ToInt32(.00093));
                            r.price = xratePrice;
                            r.timeStamp = p.time_exchange;
                        }
                        else
                        {
                            r.price = p.price;
                            r.timeStamp = p.time_exchange;
                        }


                        // call db insert stored procedure
                        SqlConnection conn = new SqlConnection(@"Data Source=119.81.9.244,780;Initial Catalog=JBlack1776_CCN;Persist Security Info=True;User ID=JBlack1776_sysdba;Password=masterkey");
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("ADD_PRICELINE", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("MARKET", SqlDbType.NVarChar).Value = r.market;
                        cmd.Parameters.AddWithValue("SYMBOL", SqlDbType.NVarChar).Value = r.symbol;
                        cmd.Parameters.AddWithValue("CURRENCY", SqlDbType.NVarChar).Value = r.currency;
                        cmd.Parameters.AddWithValue("PRICE", SqlDbType.Float).Value = r.price;
                        cmd.Parameters.AddWithValue("TIMESTAMP", SqlDbType.DateTime).Value = r.timeStamp;
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        conn.Dispose();

                    }
                    catch (Exception ex)
                    {
                        //throw ex;
                    }
                }


            }
            catch (Exception ex)
            {
                //throw ex;
            }
        }

        // my class
        public class PricelineRecord
        {
            public string symbol { get; set; }
            public string currency { get; set; }
            public string market { get; set; }
            public double price { get; set; }
            public double arbitrage { get; set; }
            public DateTime timeStamp { get; set; }

            public PricelineRecord(string a, string b, string c)
            {
                market = a;
                symbol = b;
                currency = c;
            }
        }

        // JSON Return Classes

        public class RootObject
        {
            public string symbol_id { get; set; }
            public DateTime time_exchange { get; set; }
            public DateTime time_coinapi { get; set; }
            public string uuid { get; set; }
            public double price { get; set; }
            public double size { get; set; }
            public string taker_side { get; set; }
        }

    }
}
