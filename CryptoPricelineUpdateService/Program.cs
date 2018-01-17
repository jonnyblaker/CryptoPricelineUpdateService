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
                string url = @"https://rest.coinapi.io/v1/quotes/current";
                var client = new WebClient();
                client.Headers.Add("X-CoinAPI-Key", "BADF8493-2F65-40BC-AB81-5B49516243AC");
                var response = client.DownloadString(url);
                string s = HttpUtility.HtmlDecode(response.ToString());
                string x = s;

                Rootobject rb = JsonConvert.DeserializeObject<Rootobject>(response);
                foreach (Priceline p in rb.Pricelines)
                {
                    try
                    {

                    }
                    catch (Exception ex)
                    {

                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        // JSON Return Classes
        public class Rootobject
        {
            public Priceline[] Pricelines { get; set; }
        }

        public class Priceline
        {
            public string symbol_id { get; set; }
            public DateTime time_exchange { get; set; }
            public DateTime time_coinapi { get; set; }
            public float ask_price { get; set; }
            public float ask_size { get; set; }
            public float bid_price { get; set; }
            public float bid_size { get; set; }
            public Last_Trade last_trade { get; set; }
        }

        public class Last_Trade
        {
            public DateTime time_exchange { get; set; }
            public DateTime time_coinapi { get; set; }
            public string uuid { get; set; }
            public float price { get; set; }
            public float size { get; set; }
            public string taker_side { get; set; }
        }



    }
}
