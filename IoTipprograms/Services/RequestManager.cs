using IoTipprograms.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace IoTipprograms.Services
{
    public class RequestManager
    {
        public TokenModel token = new TokenModel();
        public List<MessageModel> messagesList = new List<MessageModel>();
        public List<UserModel> usersList = new List<UserModel>();
        HttpClient httpClient = new HttpClient();

        public async Task HttpUserComunication(UserModel user, string url, string requestType, string bearerToken)
        {
            if (!string.IsNullOrEmpty(bearerToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
            }

            //Simple SHA256 hashing is used in here.            
            //This type of hashing can be cracked using reversed SHA256. 
            //To make it more safe there should be added salt from random numbers which will be implemented in the future.
            if (user.UserPassword != null)
            {
                var sha256 = System.Security.Cryptography.SHA256.Create();
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.UserPassword));
                user.UserPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
            if (user.UserNewPassword != null)
            {
                var sha256 = System.Security.Cryptography.SHA256.Create();
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.UserNewPassword));
                user.UserNewPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }

            var json = JsonConvert.SerializeObject(user);
            httpClient.DefaultRequestHeaders.Add("Host", "postiotapi.ipprograms.pl");

            if (requestType == "POST")
            {
                var response = await httpClient.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                string responseContent = await response.Content.ReadAsStringAsync();
                if (responseContent.Contains("token") && !responseContent.Contains("Nieprawidłowe dane logowania"))
                {
                    token = JsonConvert.DeserializeObject<TokenModel>(responseContent);
                }
            }
            if (requestType == "GET")
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseContent = await response.Content.ReadAsStringAsync();
                usersList = JsonConvert.DeserializeObject<List<UserModel>>(responseContent);
            }
            if (requestType == "PUT")
            {
                var response = await httpClient.PutAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                string responseContent = await response.Content.ReadAsStringAsync();
            }
        }

        public async Task HttpMessageComunication(MessageModel message, string url, string requestType, string bearerToken)
        {
            if (message.MessageDeath != null)
            {
                message.MessageDeathForAspNet = DateTime.ParseExact(message.MessageDeath, "dd.MM.yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            }        
            if (!string.IsNullOrEmpty(bearerToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
            }
            var json = JsonConvert.SerializeObject(message);
            httpClient.DefaultRequestHeaders.Add("Host", "postiotapi.ipprograms.pl");

            if (requestType == "POST")
            {
                var response = await httpClient.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                string responseContent = await response.Content.ReadAsStringAsync();
            }
            if (requestType == "GET")
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseContent = await response.Content.ReadAsStringAsync();
                messagesList = JsonConvert.DeserializeObject<List<MessageModel>>(responseContent);
            }
            if (requestType == "PUT")
            {
                var response = await httpClient.PutAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                string responseContent = await response.Content.ReadAsStringAsync();
            }
        }
    }
}
