using BlazorWasmLoker.Resoruces.Settings;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorWasmLoker.Services
{
    public class SettingService
    {
        private readonly HttpClient _httpClient;
        private const string Controller = "Settings/";
        private readonly IJSRuntime _jsRuntime;

        public SettingService(HttpClient httpClient, IJSRuntime jsruntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsruntime;
        }
        public void JsConsoleLog(object message)
        {
            _jsRuntime.InvokeVoidAsync("console.log", message);
        }
        public async Task<string> SaveCounter(CounterResoruce counterResoruce)
        {
            var respond = await _httpClient.PostAsJsonAsync(Controller + "counter", counterResoruce);
            return respond.IsSuccessStatusCode
                ? await respond.Content.ReadAsStringAsync()
                : throw new Exception(await respond.Content.ReadAsStringAsync());
        }

        public async Task<IpResource> getIp()
        {
            var respond = await _httpClient.GetAsync("https://jsonip.com");

            return respond.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<IpResource>(await respond.Content.ReadAsStringAsync())
                : throw new Exception(await respond.Content.ReadAsStringAsync());
        }
        public async Task<IpDocResource> getDocIp(string ip)
        {
            var respond = await _httpClient.GetAsync($"https://ipapi.co/{ip}/json/");

            return respond.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<IpDocResource>(await respond.Content.ReadAsStringAsync())
                : throw new Exception(await respond.Content.ReadAsStringAsync());
        }
    }
}
