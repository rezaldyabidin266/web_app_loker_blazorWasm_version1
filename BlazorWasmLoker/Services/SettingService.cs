using BlazorWasmLoker.Resoruces.Settings;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorWasmLoker.Services
{
    public class SettingService
    {
        private readonly HttpClient _httpClient;
        private const string Controller = "Settings/";

        public SettingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task SaveCounter(CounterResoruce counterResoruce)
        {
            await _httpClient.PostAsJsonAsync(Controller + "counter", counterResoruce);
        }

    }
}
