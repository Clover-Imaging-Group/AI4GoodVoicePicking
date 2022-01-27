using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using AI4Good.Models;
using Newtonsoft.Json;

namespace AI4Good.Services
{
    public class WebAPIService
    {
        private HttpClient _client;

        public string _webAPIUrl
        {
            get; private set;
        }

        public ObservableCollection<UserRole> Items
        {
            get; private set;
        }
        public User UserItem
        {
            get; private set;
        }


        public WebAPIService()
        {
            _client = new HttpClient();
            _webAPIUrl = "https://ai4gooddemo.azurewebsites.net/api/Values/";
        }

        public async Task<ObservableCollection<UserRole>> GetUserRolesAsync()
        {
            try
            {
                var response = await _client.GetAsync(GetUpdatedUri(""));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Items = JsonConvert.DeserializeObject<ObservableCollection<UserRole>>(content);
                    return Items;
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            try
            {
                var response = await _client.GetAsync(GetUpdatedUri(id.ToString()));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    UserItem = JsonConvert.DeserializeObject<User>(content);
                    return UserItem;
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        private Uri GetUpdatedUri(string param)
        {
            _webAPIUrl += param.ToString();
            return new Uri(_webAPIUrl);
        }
    }
}
