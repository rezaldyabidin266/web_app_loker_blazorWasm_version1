using BlazorWasmLoker.Resoruces.Motivations;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorWasmLoker.Services
{
    public class MotivationService
    {
        private readonly HttpClient _httpClient;
        private const string Controller = "Motivations/";

        public MotivationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<KalimatMotivasiResoruce>> GetListKalimat()
        {
            return await _httpClient.GetFromJsonAsync<List<KalimatMotivasiResoruce>>(Controller + "list-kalimat");
        }
        public async Task<byte[]> GetGambarMotivasi()
        {
            var respond = await _httpClient.GetAsync(Controller + "show-gambar-motivasiftp");
            //return await respond.Content.ReadAsByteArrayAsync();
            return respond.IsSuccessStatusCode
               ? await respond.Content.ReadAsByteArrayAsync()
               : throw new Exception(await respond.Content.ReadAsStringAsync());

        }
    }
}
