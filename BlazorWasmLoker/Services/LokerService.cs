using BlazorWasmLoker.Resoruces.Lokers;
using BlazorWasmLoker.Resoruces.Users;
using Grpc.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
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
        // private readonly HttpResponseMessage _httpRespon;
        private const string Controller = "Lokers/";
        [Inject]
        protected Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }
        public string token;

        public LokerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            // _httpRespon = httpRespon;

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

        public async Task<DaftarResponse> DaftarNonRegis(DaftarResource userDaftar)
        {
            var respond = await _httpClient.PostAsJsonAsync(Controller + "daftar-no-register", userDaftar);
            return await respond.Content.ReadFromJsonAsync<DaftarResponse>();
        }

        public async Task<(string info, List<LokerSayaResource> lokerSayaResources)> ListDaftarLokerSaya(string token)
        {
            _httpClient.DefaultRequestHeaders.Add("token", token);
            var result = await _httpClient.GetAsync(Controller + "list-daftar-loker-saya");

            return result.IsSuccessStatusCode
                ? ("Sukses", await _httpClient.GetFromJsonAsync<List<LokerSayaResource>>(Controller + "list-daftar-loker-saya"))
                : (await result.Content.ReadAsStringAsync(), null);
        }

        public async Task<(string info, List<FromPertanyaanResoruce> fromPertanyaans)> FormPertanyaan(string token, int lokerId)
        {
            _httpClient.DefaultRequestHeaders.Add("token", token);
            _httpClient.DefaultRequestHeaders.Add("lokerId", lokerId.ToString());

            object response = await httpSend.Content.ReadFromJsonAsync<object>();
            return response;

            if (result.IsSuccessStatusCode)
            {
                var respond = JsonConvert.DeserializeObject<RootPertanyaanResource>(result.Content.ReadAsStringAsync().Result);
                return (respond.Message, respond.Pertanyaan);
            }
            else
            {
                return (await result.Content.ReadAsStringAsync(), null);
            }
        }
    }

}





//var request = new HttpRequestMessage(HttpMethod.Get, Controller + "list-daftar-loker-saya");
//request.Headers.Add("token", token);

//var httpSend = await _httpClient.SendAsync(request);

//if (!httpSend.IsSuccessStatusCode)
//{
//    //error kondisi
//    object errormessage;
//    errormessage = httpSend.ReasonPhrase;  
//    return errormessage;
//}

//object response = await httpSend.Content.ReadFromJsonAsync<object>();
//return response;
