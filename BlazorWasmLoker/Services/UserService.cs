using BlazorWasmLoker.Resoruces.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorWasmLoker.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private const string Controller = "Users/";
        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TokenResource> Login(UserLoginResource userLogin)
        {            
            var respond = await _httpClient.PostAsJsonAsync(Controller + "login", userLogin);
            return await respond.Content.ReadFromJsonAsync<TokenResource>();
        }


    }
}
