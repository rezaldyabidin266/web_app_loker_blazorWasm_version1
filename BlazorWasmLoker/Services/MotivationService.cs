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

<<<<<<< HEAD
        public async Task<byte[]> GetGambarMotivasi()
        {
            var responsePhoto = await _httpClient.GetAsync(Controller + "show-gambar-motivasiftp");
            byte[] byteArrayPhoto = await responsePhoto.Content.ReadAsByteArrayAsync();
            return byteArrayPhoto;
        }

       

=======
         public async Task<byte[]> GetGamabarMotivasiFtp()
        {            
            var result = await _httpClient.GetFromJsonAsync<byte[]>(Controller + "show-gambar-motivasiftp");
            return result;
        }
>>>>>>> cc0105c73f28b9d2b7efb2dbf615bb7ed995263c
    }
}
