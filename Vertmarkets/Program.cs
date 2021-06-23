using System;
using System.Net.Http;

namespace Vertmarkets
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new HttpClient())
            {

                //client.BaseAddress = new Uri("http://magazinestore.azurewebsites.net/api/");
                ////HTTP GET
                //var responseTask = client.GetAsync("/api/token");
                //responseTask.Wait();

                ////  var result = responseTask.Result;
                ///
                Repository subscribers = new Repository();
                string result = subscribers.SendAnswerData().Result;
                Console.WriteLine(result);
                Console.ReadLine();
            }
           
        }
    }
}
