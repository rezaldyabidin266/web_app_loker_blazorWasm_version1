using Blazored.LocalStorage;
using BlazorWasmLoker.Resoruces.Lokers;
using BlazorWasmLoker.Resoruces.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWasmLoker.Services
{

    public class LokerService
    {
        private readonly HttpClient _httpClient; 
        private const string Controller = "Lokers/";          

        public IJSRuntime _Jsruntime { get; }
        public ILocalStorageService _LocalStorage { get; }

        public LokerService(HttpClient httpClient , IJSRuntime jsruntime, ILocalStorageService localStorage )
        {
            _httpClient = httpClient;
            _LocalStorage = localStorage;
            _Jsruntime = jsruntime;
        }

        public void consoleLog(object message)
        {
             _Jsruntime.InvokeVoidAsync("console.log", message);
        }

        public async Task<List<LokerResource>> ListLoker()
        {
            var respond = await _httpClient.GetAsync(Controller + "list-loker");

            return respond.IsSuccessStatusCode
              ? JsonConvert.DeserializeObject<List<LokerResource>>(respond.Content.ReadAsStringAsync().Result)
              : throw new Exception(await respond.Content.ReadAsStringAsync());
        }

        public async Task<LokerResource> GetLoker(int idLoker)
        {
            var respond = await _httpClient.GetAsync(Controller + $"get-loker?LokerId={idLoker}");
            var data = _LocalStorage.GetItemAsync<string>("token");
            return respond.IsSuccessStatusCode
               ? JsonConvert.DeserializeObject<LokerResource>(respond.Content.ReadAsStringAsync().Result)
               : throw new Exception(await respond.Content.ReadAsStringAsync());
        }

        public async Task<List<string>> GetKriteria(int idLoker)
        {
            var respond = await _httpClient.GetAsync(Controller + $"get-kriteria?LokerId={idLoker}");

            return respond.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<string>>(respond.Content.ReadAsStringAsync().Result)
                : throw new Exception(await respond.Content.ReadAsStringAsync());
        }

        public async Task<byte[]> GetImageBackground(int idLoker)
        {
            var respond = await _httpClient.GetAsync(Controller + $"get-image-background?lokerId={idLoker}");

            return respond.IsSuccessStatusCode
                ? await respond.Content.ReadAsByteArrayAsync()
                : throw new Exception(await respond.Content.ReadAsStringAsync());
        }

        public async Task<byte[]> GetImageIlustrasi(int idLoker)
        {
            var respond = await _httpClient.GetAsync(Controller + $"get-image-ilustrasi?lokerId={idLoker}");

            return respond.IsSuccessStatusCode
                ? await respond.Content.ReadAsByteArrayAsync()
                : throw new Exception(await respond.Content.ReadAsStringAsync());
        }

        public async Task<DaftarResponse> DaftarNoRegister(DaftarResource userDaftar)
        {
            var content = JsonConvert.SerializeObject(userDaftar);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var respond = await _httpClient.PostAsync(Controller + "daftar-no-register", bodyContent);

            return respond.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<DaftarResponse>(await respond.Content.ReadAsStringAsync())
                : throw new Exception(await respond.Content.ReadAsStringAsync());
        }

        public async Task<List<LokerSayaResource>> ListDaftarLokerSaya(string token)
        {
            _httpClient.DefaultRequestHeaders.Add("token", token);
            var respond = await _httpClient.GetAsync(Controller + "list-daftar-loker-saya");

            return respond.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<LokerSayaResource>>(await respond.Content.ReadAsStringAsync())
                : throw new Exception(await respond.Content.ReadAsStringAsync());
        }

        public async Task<RootPertanyaanResource> FormPertanyaan(string token, int lokerId)
        {
            _httpClient.DefaultRequestHeaders.Add("token", token);
            _httpClient.DefaultRequestHeaders.Add("lokerId", lokerId.ToString());
            var respond = await _httpClient.GetAsync(Controller + "form-pertanyaan");

            return respond.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<RootPertanyaanResource>(respond.Content.ReadAsStringAsync().Result)
                : throw new Exception(await respond.Content.ReadAsStringAsync(), null);
        }

        public async Task<string> FormSaveListJawaban(List<JawabanResoruce> jawabanResoruce)
        {
            var content = JsonConvert.SerializeObject(jawabanResoruce);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var respond = await _httpClient.PostAsync(Controller + "form-save-list-jawaban", bodyContent);

            return respond.IsSuccessStatusCode
                ? await respond.Content.ReadAsStringAsync()
                : throw new Exception(await respond.Content.ReadAsStringAsync());
        }
    }

}




