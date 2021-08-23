using BlazorWasmLoker.Resoruces.Lokers;
using BlazorWasmLoker.Resoruces.Users;
using BlazorWasmLoker.Services;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWasmLoker.Pages.LokerPages
{
    public partial class KriteriaComp : ComponentBase
    {
        [Inject]
        protected LokerService LokerService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        protected Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }

        [Parameter]
        public int IdLoker { get; set; }

        protected LokerResource Lokers { get; set; } = new LokerResource();

        protected List<string> kriteriaResc = new();
        protected string gambarBackground;
        protected string gambarIlustrasi;

        protected string Nama;
        protected string Alamat;
        protected string Email;
        protected string password;
        protected string tempatLahir;
        protected DateTime tglLahir { get; set; } = DateTime.Today;
        protected string noTlp;
        protected string note;

        //token
        protected string token;

        //response
        protected DaftarResponse response;
        protected dynamic lokerJsonElement = new System.Dynamic.ExpandoObject();
        protected string serializeString;
        protected List<LokerSayaResource> loker;
        protected string message;
        protected string messagePertanyaan;
        protected string ErrorMessage;
        protected string ErrorBg;
        protected string ErrorKriteria;

        protected char maskChar = ' ';

        //Mask default
        string DateTimeMaskValue { get; set; } = DateTimeMask.LongDate;


        protected override async Task OnInitializedAsync()
        {
            token = await LocalStorage.GetItemAsync<string>("token");

            await GetLoker(IdLoker);
            await KriteriaPelamar(IdLoker);
            await GambarBg(IdLoker);
            await GambarIl(IdLoker);

        }

        protected async Task KriteriaPelamar(int IdLoker)
        {
            try
            {
                kriteriaResc = await LokerService.GetKriteria(IdLoker);
            }
            catch (Exception ex)
            {
                ErrorKriteria = ex.Message;
            }
        }

        protected async Task GambarBg(int IdLoker)
        {
            try
            {
                byte[] gambarBgByte = await LokerService.GetImageBackground(IdLoker);
                gambarBackground = Convert.ToBase64String(gambarBgByte);
            }
            catch (Exception ex)
            {
                ErrorBg = ex.Message;
            }

        }

        protected async Task GambarIl(int IdLoker)
        {
            try
            {
                byte[] gambarIlbyte = await LokerService.GetImageIlustrasi(IdLoker);
                gambarIlustrasi = Convert.ToBase64String(gambarIlbyte);
            }
            catch (Exception ex)
            {
                ErrorBg = ex.Message;
            }

        }

        protected async Task GetLoker(int IdLoker)
        {
            try
            {
                Lokers = await LokerService.GetLoker(IdLoker);
            }
            catch (Exception ex)
            {

                ErrorMessage = ex.Message;
            }
        }

        protected async Task daftarClick()
        {
            var daftarNoRegis = new DaftarResource
            {
                Nama = Nama,
                Alamat = Alamat,
                Email = Email,
                Password = password,
                TempatLahir = tempatLahir,
                TglLahir = tglLahir,
                NoTlp = noTlp,
                Note = note,
            };

            try
            {
                var result = await LokerService.DaftarNoRegister(daftarNoRegis);
                await LocalStorage.SetItemAsync("token", result.Token);
                NavigationManager.NavigateTo("/pertanyaan");
                response = new DaftarResponse
                {
                    IdPelamar = result.IdPelamar,
                    Message = result.Message,
                    Token = result.Token
                };
            }
            catch (Exception ex)
            {
                message = "Gagal Registerasi";
            }
        }

        protected async Task GetListLokerSaya()
        {
            try
            {
                loker = await LokerService.ListDaftarLokerSaya(token);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

        }

        //protected async Task GetPertanyaan()
        //{
        //    var idLoker = await LocalStorage.GetItemAsync<int>("IdLoker");
        //    //var token = await LocalStorage.GetItemAsync<string>("Token");

        //    try
        //    {
        //        var pertanyaan = await LokerService.FormPertanyaan(token, idLoker);
        //        messagePertanyaan = pertanyaan.Message;
        //        Console.WriteLine(messagePertanyaan);

        //        if (pertanyaan.Pertanyaan != null)
        //        {
        //            foreach (var item in pertanyaan.Pertanyaan)
        //            {
        //                Console.WriteLine(item.Pertanyaan);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        message = ex.Message;
        //    }
        //}

    }
}
