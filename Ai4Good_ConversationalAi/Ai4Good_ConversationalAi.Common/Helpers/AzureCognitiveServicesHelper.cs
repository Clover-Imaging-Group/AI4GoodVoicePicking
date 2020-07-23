using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ai4Good_ConversationalAi.Common.Helpers
{
    public class CognitiveServicesHelper
    {
        public static async Task<string> FetchTokenAsync(string APITokenEndPoint, string APIKey)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", APIKey);
                UriBuilder uriBuilder = new UriBuilder(APITokenEndPoint);

                var result = await client.PostAsync(uriBuilder.Uri.AbsoluteUri, null);
                Console.WriteLine("Token Uri: {0}", uriBuilder.Uri.AbsoluteUri);
                return await result.Content.ReadAsStringAsync();
            }
        }
    }
}
