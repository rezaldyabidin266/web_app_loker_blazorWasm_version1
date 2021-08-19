using BlazorWasmLoker.Resoruces.Lokers;
using BlazorWasmLoker.Resoruces.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
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

        public async Task<byte[]> GetImageBackground(int idLoker)
        {
            var respond = await _httpClient.GetAsync(Controller + $"get-image-background?lokerId={idLoker}");
            return await respond.Content.ReadAsByteArrayAsync();
        }

        public async Task<byte[]> GetImageIlustrasi(int idLoker)
        {
            var respond = await _httpClient.GetAsync(Controller + $"get-image-ilustrasi?lokerId={idLoker}");
            return await respond.Content.ReadAsByteArrayAsync();
        }

        public async Task<string> Login(DaftarResource userDaftar)
        {
            var respond = await _httpClient.PostAsJsonAsync(Controller + "daftar-no-register", userDaftar);
            return await respond.Content.ReadFromJsonAsync<string>();
        }

        public async Task<List<LokerSayaResource>> ListDaftarLokerSaya(string token)
        {
            //var request = new HttpRequestMessage(HttpMethod.Get, Controller + "list-daftar-loker-saya");
            //var result =   request.Headers.Authorization = new AuthenticationHeaderValue("token", token);

            //mungkin pake deserializedErrorObject
            try
            {
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("token", token);
                var result = await _httpClient.GetFromJsonAsync<List<LokerSayaResource>>(Controller + "list-daftar-loker-saya");

                return result;
            }
            catch 
            {
             

                return null ;
            }

           



            //var status = result.StatusCode;
            //var responBody = result.Content;


          



        }
    }
}
