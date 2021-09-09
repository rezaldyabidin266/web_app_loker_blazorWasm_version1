using BlazorWasmLoker.Resoruces.Lokers;
using BlazorWasmLoker.Resoruces.Users;
using BlazorWasmLoker.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWasmLoker.Pages.LokerPages
{
    public class EndPageCompBase : ComponentBase
    {

        [Inject]
        protected MotivationService MotivationService { get; set; }
        [Inject]
        protected UserService UserService { get; set; }
        [Inject]
        protected LokerService LokerService{ get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        protected Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        protected string token;
        protected string gambarMotivasi;
        protected string fotoPelamar;
        protected string ErrorBg;
       
        protected InformasiPelamarRespond responseInfoPelamar = new InformasiPelamarRespond();
        protected List<FromPertanyaanResoruce> FormPertanyaan = new List<FromPertanyaanResoruce>();
        protected List<PengalamanResourceId> PengalamanResoruceId = new List<PengalamanResourceId>();
        protected string TanggalLahirPelamar;
        protected string TanggalAwalKerja;
        protected string TanggalAkhirKerja;

        protected override async Task OnInitializedAsync()
        {
            token = await LocalStorage.GetItemAsync<string>("token");
            await GambarMotivasi();
            await FotoPelamar();
            await InformasiPelamar();
            await GetPertanyaan();
            await GetPengalaman();
        }

        protected async Task GambarMotivasi()
        {
            try
            {
                byte[] gambarBgByte = await MotivationService.GetGambarMotivasi();
                gambarMotivasi = Convert.ToBase64String(gambarBgByte);
            }
            catch (Exception ex)
            {
                ErrorBg = ex.Message;
                await JSRuntime.InvokeVoidAsync("notifDev", ErrorBg, "error", 3000);
            }

        }
        protected async Task FotoPelamar()
        {
            try
            {
                byte[] fotoByte = await UserService.GetFoto(token);
                var foto = Convert.ToBase64String(fotoByte);
                fotoPelamar = "data:image/png;base64," + foto;
            }
            catch (Exception)
            {
                fotoPelamar = "asset/image/avatar.jpg";
                await JSRuntime.InvokeVoidAsync("notifDev", ErrorBg, "error", 3000);
            }
        }

        protected async Task InformasiPelamar()
        {
            try
            {
                responseInfoPelamar = await UserService.InformasiPelamar(token);
                TanggalLahirPelamar = responseInfoPelamar.TglLahir.ToString("yyyy-MM-dd");
            }
            catch (Exception ex)
            {
                UserService.GotoLogin();
                await JSRuntime.InvokeVoidAsync("notifDev", ex.Message, "error", 3000);
            }

        }

        protected async Task GetPertanyaan()
        {
            try
            {
                var idLoker = await LocalStorage.GetItemAsync<int>("IdLoker");
                var root = await LokerService.FormPertanyaan(token, idLoker);
                foreach (var item in root.Pertanyaan)
                {
                    var data = new FromPertanyaanResoruce
                    {
                        Id = item.Id,
                        BentukIsian = item.BentukIsian,
                        IsRequired = item.IsRequired,
                        Jawaban = item.Jawaban,
                        No = item.No,
                        Pertanyaan = item.Pertanyaan,
                        Pilihan = item.Pilihan
                    };
                    FormPertanyaan.Add(data);
                }
            }
            catch (Exception)
            {
                await JSRuntime.InvokeVoidAsync("notifDev", ErrorBg, "error", 3000);
            }
        }

        protected async Task GetPengalaman()
        {
            try
            {
                PengalamanResoruceId = await UserService.ListPengalaman(token);
                foreach (var item in PengalamanResoruceId)
                {
                    TanggalAwalKerja = item.TglAwal.ToString("yyyy-MM-dd");
                    TanggalAkhirKerja = item.TglAkhir.ToString("yyyy-MM-dd");
                }
                  
            }
            catch (Exception)
            {
                await JSRuntime.InvokeVoidAsync("notifDev", ErrorBg, "error", 3000);
            }
        }

        protected void goHome()
        {
            NavigationManager.NavigateTo("/");
        }
    }
}
