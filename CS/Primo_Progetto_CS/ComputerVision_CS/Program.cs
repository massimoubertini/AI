using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using static System.Console;

namespace ComputerVision_CS
{
    internal class Program
    {
        private const string subscriptionKey = "bc116f9fdbdd49dc9627695b657be473";

        private static void Main(string[] args)
        {
            Clear();
            MakeRequest();
            ReadKey();
        }

        private static async void MakeRequest()
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            // richiesta intestazioni
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            // richiesta parametri
            queryString["visualFeatures"] = "Categories";
            queryString["details"] = "";
            var uri = "https://api.projectoxford.ai/vision/v1.0/analyze?" + queryString;
            HttpResponseMessage response;
            // richiesta body
            byte[] byteData = Encoding.UTF8.GetBytes("{\"url\": \"https://projectoxfordportal.azureedge.net/vision/Analysis/1-1.jpg\"}");
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
                var json = await response.Content.ReadAsStringAsync();
                WriteLine(json);
            }
        }
    }
}