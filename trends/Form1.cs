using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace trends
{
    public partial class Form1 : Form
    {
        public class SearchParameters
        {
            public string date { get; set; }
        }

        public class Query
        {
            public string query { get; set; }
            public double extracted_value { get; set; }
        }

        public class RelatedQueries
        {
            public List<Query> rising { get; set; }
            public List<Query> top { get; set; }
        }

        public class RootObject
        {
            public SearchParameters search_parameters { get; set; }
            public RelatedQueries related_queries { get; set; }
        }
        public Form1()
        {
            InitializeComponent();
        }

        static void AnalyzeRisingQueries(List<Query> risingQueries)
        {
            Console.WriteLine("\nTop 5 Rising Queries:");
            foreach (var query in risingQueries.OrderByDescending(q => q.extracted_value).Take(5))
            {
                Console.WriteLine($"- {query.query}: {query.extracted_value}");
            }
        }

        static void AnalyzeTopQueries(List<Query> topQueries)
        {
            Console.WriteLine("\nTop 5 Most Popular Queries:");
            foreach (var query in topQueries.Take(5))
            {
                Console.WriteLine($"- {query.query}: {query.extracted_value}");
            }
        }

        static double CalculateAverageRise(List<Query> risingQueries)
        {
            var validRises = risingQueries.Where(q => q.extracted_value != 5950).Select(q => q.extracted_value);
            return validRises.Any() ? validRises.Average() : 0;
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            string baseUrl = "https://serpapi.com/search";
            string apiKey = "6095ab0493123759c31573548542cec4b35b219372c692d5c7fd91c65af50e1";

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["engine"] = "google_trends";
            query["q"] = "coffee";
            query["data_type"] = "RELATED_QUERIES";
            query["api_key"] = apiKey;

            string url = $"{baseUrl}?{query}";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            // JSON'u deserializasyon yaparak RootObject'e dönüştür
RootObject data = JsonConvert.DeserializeObject<RootObject>(responseBody);

            // Verileri işleme
            Console.WriteLine("Coffee Trend Analysis");
            Console.WriteLine("=====================");

            Console.WriteLine($"\nTime Period: {data.search_parameters.date}");

            AnalyzeRisingQueries(data.related_queries.rising);
            AnalyzeTopQueries(data.related_queries.top);

            double avgRise = CalculateAverageRise(data.related_queries.rising);
            Console.WriteLine($"\nAverage Rise in Popularity: {avgRise:F2}%");

            Console.WriteLine("\nInteresting Findings:");
            Console.WriteLine($"1. Most breakout query: {data.related_queries.rising.First().query}");
            Console.WriteLine($"2. Most popular query: {data.related_queries.top.First().query}");
            Console.WriteLine($"3. Number of queries with over 100% rise: {data.related_queries.rising.Count(q => q.extracted_value > 100)}");



           

          
            
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
