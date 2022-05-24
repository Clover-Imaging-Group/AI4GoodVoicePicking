using System;
using System.Collections.Generic;
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


        public WebAPIService(string controllerName)
        {
            _client = new HttpClient();
            _webAPIUrl = $"https://ai4gooddemo.azurewebsites.net/api/{controllerName}/";
        }

        public async Task<ObservableCollection<UserRole>> GetUserRolesAsync()
        {
            try
            {
                var response = await _client.GetAsync(GetUpdatedUri("", ""));

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
                var response = await _client.GetAsync(GetUpdatedUri("", id.ToString()));

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

        public async Task<List<Item>> GetItemsToPickAsync()
        {
            try
            {
                var response = await _client.GetAsync(GetUpdatedUri("GetItemsToPick", ""));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Item>>(content);
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public async Task UpatePickedItemsByIdsAsync(string itemIds)
        {
            try
            {
                var response = await _client.GetAsync(GetUpdatedUri("UpdatePickedItemsByIds", itemIds));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    JsonConvert.DeserializeObject<List<Item>>(content);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private Uri GetUpdatedUri(string actionName, string param)
        {
            _webAPIUrl += $"{actionName}/{param.ToString()}";
            return new Uri(_webAPIUrl);
        }
    }
}
