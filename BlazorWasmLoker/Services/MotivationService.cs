using BlazorWasmLoker.Resoruces.Motivations;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
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

        public async Task<IEnumerable<KalimatMotivasiResoruce>> GetListKalimat()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<KalimatMotivasiResoruce>>(Controller + "list-kalimat");
        }

        public async Task<byte[]> GetGambarMotivasi()
        {
            var responsePhoto = await _httpClient.GetAsync(Controller + "show-gambar-motivasiftp");
            byte[] byteArrayPhoto = await responsePhoto.Content.ReadAsByteArrayAsync();
            return byteArrayPhoto;
        }

       

    }
}
