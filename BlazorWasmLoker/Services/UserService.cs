using Blazored.LocalStorage;
using BlazorWasmLoker.Resoruces.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWasmLoker.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private const string Controller = "Users/";
        private readonly IJSRuntime _jsRuntime;
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;

        public UserService(HttpClient httpClient, NavigationManager navigationManager,
                            IJSRuntime jsruntime, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            _jsRuntime = jsruntime;
            _localStorage = localStorage;
            _httpClient.DefaultRequestHeaders.Clear();
        }

        //DRY Code
        public void JsConsoleLog(object message)
        {
            _jsRuntime.InvokeVoidAsync("console.log", message);
        }
        public void GotoLogin()
        {
            _navigationManager.NavigateTo("/login");
        }
        public async Task<TokenResource> Login(UserLoginResource userLogin)
        {
            var respond = await _httpClient.PostAsJsonAsync(Controller + "login", userLogin);

            if (respond.IsSuccessStatusCode)
                return await respond.Content.ReadFromJsonAsync<TokenResource>();
            else
            {
                if (respond.StatusCode != System.Net.HttpStatusCode.Forbidden)
                    throw new Exception(await respond.Content.ReadAsStringAsync());
                else
                    throw new Exception($"403 :{respond.StatusCode} {await respond.Content.ReadAsStringAsync()}");
            }
        }
        public async Task<InformasiPelamarRespond> InformasiPelamar(string token)
        {
              _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("token", token);
            var respond = await _httpClient.GetAsync(Controller + "informasi-pelamar");

            return respond.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<InformasiPelamarRespond>(await respond.Content.ReadAsStringAsync())
                : throw new Exception(await respond.Content.ReadAsStringAsync());
        }
        public async Task<string> GantiPassword(GantiPasswordResoruce gantiPasswordResoruce)
        {
            var content = JsonConvert.SerializeObject(gantiPasswordResoruce);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var respond = await _httpClient.PostAsync(Controller + "ganti-password", bodyContent);

            return respond.IsSuccessStatusCode
                ? await respond.Content.ReadAsStringAsync()
                : throw new Exception(await respond.Content.ReadAsStringAsync());
        }
        public async Task<string> BuatPasswrodBaru(PasswordBaruResoruce passwordBaruResoruce)
        {
            var content = JsonConvert.SerializeObject(passwordBaruResoruce);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var respond = await _httpClient.PostAsync(Controller + "buat-password-baru", bodyContent);

            return respond.IsSuccessStatusCode
                ? await respond.Content.ReadAsStringAsync()
                : throw new Exception(await respond.Content.ReadAsStringAsync());
        }
        public async Task<string> Register(DaftarResource daftarResource)
        {
            var content = JsonConvert.SerializeObject(daftarResource);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var respond = await _httpClient.PostAsync(Controller + "register", bodyContent);

            return respond.IsSuccessStatusCode
                ? await respond.Content.ReadAsStringAsync()
                : throw new Exception(await respond.Content.ReadAsStringAsync());
        }
        public async Task<string> UpdateDataPelamar(string token, UpdatePelamarResoruce updatePelamarResoruce)
        {
            _httpClient.DefaultRequestHeaders.Add("token", token);
            var content = JsonConvert.SerializeObject(updatePelamarResoruce);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var respond = await _httpClient.PutAsync(Controller + "update-data-pelamar", bodyContent);

            return respond.IsSuccessStatusCode
                ? await respond.Content.ReadAsStringAsync()
                : throw new Exception(await respond.Content.ReadAsStringAsync());
        }
        public async Task<string> RequestOtpResetPassword(string email, string noHandphone)
        {
            _httpClient.DefaultRequestHeaders.Add("email", email);
            _httpClient.DefaultRequestHeaders.Add("noHandphone", noHandphone);
            var respond = await _httpClient.PutAsync(Controller + "request-otp-reset-password", null);

            return respond.IsSuccessStatusCode
                ? await respond.Content.ReadAsStringAsync()
                : throw new Exception(await respond.Content.ReadAsStringAsync());
        }
        public async Task<string> ResetPassword(string otp)
        {
           _httpClient.DefaultRequestHeaders.Add("Otp", otp);
            var respond = await _httpClient.PutAsync(Controller + "reset-password", null);
            return respond.IsSuccessStatusCode
                ? await respond.Content.ReadAsStringAsync()
                : throw new Exception(await respond.Content.ReadAsStringAsync());
        }
        public async Task<List<PengalamanResourceId>> ListPengalaman(string token)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("token",token);
            var respond = await _httpClient.GetAsync(Controller + "list-pengalaman");

            return respond.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<PengalamanResourceId>>(await respond.Content.ReadAsStringAsync())
                : throw new Exception(await respond.Content.ReadAsStringAsync());
        }
        public async Task<string> AddPengalaman(string token, PengalamanResoruce pengalamanResoruce)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("token", token);
            var content = JsonConvert.SerializeObject(pengalamanResoruce);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var respond = await _httpClient.PostAsync(Controller + "add-pengalaman", bodyContent);

            return respond.IsSuccessStatusCode
                ? await respond.Content.ReadAsStringAsync()
                : throw new Exception(await respond.Content.ReadAsStringAsync());
        }
        public async Task<string> UpdatePengalaman(string token, string pengalamanId, PengalamanResoruce pengalamanResoruce)
        {
            _httpClient.DefaultRequestHeaders.Add("token", token);
            _httpClient.DefaultRequestHeaders.Add("pengalamanId", pengalamanId);
            var content = JsonConvert.SerializeObject(pengalamanResoruce);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var respond = await _httpClient.PostAsync(Controller + "update-pengalaman", bodyContent);

            return respond.IsSuccessStatusCode
                ? await respond.Content.ReadAsStringAsync()
                : throw new Exception(await respond.Content.ReadAsStringAsync());
        }
        public async Task<string> DeletePengalaman(string token, int pengalamanId)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("token", token);
            _httpClient.DefaultRequestHeaders.Add("pengalamanId", pengalamanId.ToString());

            var respond = await _httpClient.DeleteAsync(Controller + "delete-pengalaman");
            return respond.IsSuccessStatusCode
                ? await respond.Content.ReadAsStringAsync()
                : throw new Exception(await respond.Content.ReadAsStringAsync());
        }
        public async Task<List<string>> ListEmailTerdaftar()
        {
            var respond = await _httpClient.GetAsync(Controller + "list-email-terdaftar");
            return respond.IsSuccessStatusCode
              ? JsonConvert.DeserializeObject<List<string>>(respond.Content.ReadAsStringAsync().Result)
              : throw new Exception(await respond.Content.ReadAsStringAsync());
        }
        public string UploadFoto()
        {
            return _httpClient.BaseAddress.AbsoluteUri + Controller + "upload-foto";
        }
        public string UploadCv()
        {
            return _httpClient.BaseAddress.AbsoluteUri + Controller + "upload-cv";
        }
        public async Task<byte[]> GetFoto(string token)
        {
            _httpClient.DefaultRequestHeaders.Add("token", token);
            var respond = await _httpClient.PostAsync(Controller + "get-foto", null);

            return respond.IsSuccessStatusCode
                ? await respond.Content.ReadAsByteArrayAsync()
                : throw new Exception(await respond.Content.ReadAsStringAsync());
        }
        public async Task<byte[]> GetCv(string token)
        {
            _httpClient.DefaultRequestHeaders.Add("token", token);
            var respond = await _httpClient.PostAsync(Controller + "get-cv", null);

            return respond.IsSuccessStatusCode
                ? await respond.Content.ReadAsByteArrayAsync()
                : throw new Exception(await respond.Content.ReadAsStringAsync());
        }
    }
}
