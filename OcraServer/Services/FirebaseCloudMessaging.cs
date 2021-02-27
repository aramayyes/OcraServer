using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace OcraServer.Services
{
    public class FirebaseCloudMessaging : INotificationSender
    {
        private const string SERVER_KEY = "Danmark";

        public async Task SendNotificationAsync(List<string> to, object data, long timeToLive)
        {
            using (HttpClient client = new HttpClient())
            {
                var body = JObject.FromObject(new
                {
                    registration_ids = to,
                    data
                }, new JsonSerializer { ContractResolver = new CamelCasePropertyNamesContractResolver() });

                try
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "key=" + SERVER_KEY);
                    var content = new StringContent(body.ToString(), Encoding.UTF8, "application/json");

                    var result = await client.PostAsync($"https://fcm.googleapis.com/fcm/send", content);
                }
                catch
                {
                    return;
                }
            }

        }
    }
}
