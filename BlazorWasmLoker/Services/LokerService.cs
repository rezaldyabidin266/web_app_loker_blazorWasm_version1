using BlazorWasmLoker.Resoruces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorWasmLoker.Services
{

    public class LokerService
    {
        private readonly HttpClient _httpClient;
        private const string Controller = "Lokers/";
        public LokerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<LokerResource>> ListLoker()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<LokerResource>>(Controller + "list-loker");
        }

        public async Task<LokerResource> GetLoker(int idLoker)
        {
            return await _httpClient.GetFromJsonAsync<LokerResource>(Controller + $"get-loker?LokerId={idLoker}");
        }

        public async Task<List<string>> GetKriteria(int idLoker)
        {
            return await _httpClient.GetFromJsonAsync<List<string>>(Controller + $"get-kriteria?LokerId={idLoker}");            
        }



    }
}
