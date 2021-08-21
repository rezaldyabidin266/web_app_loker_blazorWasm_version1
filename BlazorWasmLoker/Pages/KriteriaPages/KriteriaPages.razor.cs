using BlazorWasmLoker.Resoruces.Lokers;
using BlazorWasmLoker.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFungsi;
using BlazorWasmLoker.Resoruces.Users;
using System.Globalization;
using DevExpress.Blazor;
using Newtonsoft.Json;
using Nancy.Json;
using System.Reflection;
using System.Collections;
using Nancy.Json.Simple;

namespace BlazorWasmLoker.Pages.KriteriaPages
{
    public partial class KriteriaPages : ComponentBase
    {
        [Inject]
        protected LokerService LokerService { get; set; }

        [Inject]
        protected Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }

        [Parameter]
        public int IdLoker { get; set; }

        protected LokerResource Lokers { get; set; } = new LokerResource();

        protected List<string> kriteriaResc = new();
        protected byte[] gambarBackgroundByte;
        protected byte[] gambarIlustrasiByte;
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

        //Mask default
        string DateTimeMaskValue { get; set; } = DateTimeMask.LongDate;

     

        protected override async Task OnInitializedAsync()
        {
            Lokers = await LokerService.GetLoker(IdLoker);
            kriteriaResc = await LokerService.GetKriteria(IdLoker);
            gambarBackgroundByte = await LokerService.GetImageBackground(IdLoker);
            gambarBackground = Convert.ToBase64String(gambarBackgroundByte);
            gambarIlustrasiByte = await LokerService.GetImageIlustrasi(IdLoker);
            gambarIlustrasi = Convert.ToBase64String(gambarIlustrasiByte);


            token = await LocalStorage.GetItemAsync<string>("token");

            await GetListLokerSaya();
            await GetPertanyaan();
        
        }

        protected async Task daftarClick()
        {
            DaftarResource daftarNoRegis = new DaftarResource { };

            daftarNoRegis.Nama = Nama;
            daftarNoRegis.Alamat = Alamat;
            daftarNoRegis.Email = Email;
            daftarNoRegis.password = password;
            daftarNoRegis.TempatLahir = tempatLahir;
            daftarNoRegis.TglLahir = tglLahir;
            daftarNoRegis.NoTlp = noTlp;
            daftarNoRegis.Note = note;

            var result = await LokerService.DaftarNonRegis(daftarNoRegis);
            await LocalStorage.SetItemAsync("token", result.Token);
          
            response = new DaftarResponse
            {
                IdPelamar = result.IdPelamar,
                Message = result.Message,
                Token = result.Token
            };
        }

        protected async Task GetListLokerSaya()
        {
            var lokerSaya = await LokerService.ListDaftarLokerSaya(token);
            message = lokerSaya.info;

            if (lokerSaya.lokerSayaResources != null)
                loker = lokerSaya.lokerSayaResources;

        }

        protected async Task GetPertanyaan()
        {

            var idLoker = await LocalStorage.GetItemAsync<int>("IdLoker");
            var pertanyaan = await LokerService.FormPertanyaan(idLoker);
            messagePertanyaan = pertanyaan.info;
            Console.WriteLine(messagePertanyaan);

            if (pertanyaan.fromPertanyaans != null)
            {
                foreach (var item in pertanyaan.fromPertanyaans)
                {
                    Console.WriteLine(item.Pertanyaan);
                }
            }
            
        }

    }
}

