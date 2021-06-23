using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Vertmarkets
{
   public class Repository
    {
        
        RestClient client = new RestClient(new Uri(AppConstants.Endpoint, UriKind.Absolute));
        private async Task<RequestToken> GetToken( string endpoint)
        {
            try
            {
                var request = new RestRequest(AppConstants.Token);
                var response = await client.GetAsync<RequestToken>(request);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(AppConstants.TokenError + ex.Message);
                return new RequestToken() { Success = false.ToString(), Token = string.Empty };
            }
        }
        private async Task<Categories> GetMagazineCategories(string token)
        {
                  var request = new RestRequest("categories/" + token);
         //   var request = new RestRequest(AppConstants.Categories+"/"+token);
            var response = await client.GetAsync<Categories>(request);
                return response;
                   }

        private async Task<MagazineList> GetMagazine(string token, string category)
        {

            var request = new RestRequest("magazines/" + token+"/"+ category);
            var response = await client.GetAsync<MagazineList>(request);
            return response;


        }
        private async Task<SubscriberList> GetSubscribers(string token)
        {
            var request = new RestRequest("subscribers/" + token);
            var response = await client.GetAsync<SubscriberList>(request);
            return response;


        }
        private  async Task<Categories> GetAnswer(string token)
        {

            var request = new RestRequest("categories/" + token);
            var response = await client.GetAsync<Categories>(request);
            return response;


        }

        public async Task<string> SendAnswerData()
        {
            var result = string.Empty;
            var token = await GetToken(AppConstants.Endpoint);
            if (token != null && !string.IsNullOrWhiteSpace(token.Token))
            {
                var categories = await GetMagazineCategories(token?.Token);
                var subscribers = await GetSubscribers(token?.Token);
                var magazineCategory = new ConcurrentDictionary<string, List<int>>();
                foreach (var category in categories?.Data)
                {
                    var magazines = await GetMagazine(token?.Token, category);
                    if (magazineCategory.ContainsKey(category))
                    {
                        magazineCategory[category].AddRange(magazines?.Data.Select(d => d.Id));
                    }
                    else
                    {
                        var magazineIds = new List<int>();
                        magazineIds.AddRange(magazines?.Data.Select(d => d.Id));
                        magazineCategory.TryAdd(category, magazineIds);
                    }
                }

                var magSubs = new ConcurrentDictionary<string, int>();
                foreach (var subscriber in subscribers?.Data)
                {
                    foreach (var magCat in magazineCategory)
                    {
                        if (subscriber.MagazineIds.Intersect(magCat.Value).Any())
                        {
                            if (magSubs.ContainsKey(subscriber.Id))
                            {
                                magSubs[subscriber.Id] += 1;
                            }
                            else
                            {
                                magSubs.TryAdd(subscriber.Id, 1);
                            }
                        }
                    }
                }

                var output = magSubs.Where(ms => ms.Value == categories?.Data.Count).Select(sub => sub.Key).ToList();
                if (output.Any())
                {
                    result = output.Aggregate(new StringBuilder(), (current, next) => current.Append(current.Length == 0 ? "" : ", ").Append(next)).ToString();
                    await PostData(token.Token, output);
                }
            }

            return result;
        }

        private async Task<List<string>> PostData(string token, List<string> subscriberIds)
        {
          
            var response = new List<string>();
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    Console.WriteLine("Request token not set...");
                    return response;
                }

                var request = new RestRequest("Answer/"+token);
                request.AddJsonBody(subscriberIds);

                response = await client.PostAsync<List<string>>(request);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while getting token...\n" + ex.Message);
                return new List<string>();
            }
        }




    }
}
