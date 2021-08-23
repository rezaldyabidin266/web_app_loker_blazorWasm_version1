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
            await Task.Delay(2000);
            Lokers = await LokerService.GetLoker(IdLoker);
            kriteriaResc = await LokerService.GetKriteria(IdLoker);
            gambarBackgroundByte = await LokerService.GetImageBackground(IdLoker);
            gambarBackground = Convert.ToBase64String(gambarBackgroundByte);
            gambarIlustrasiByte = await LokerService.GetImageIlustrasi(IdLoker);
            gambarIlustrasi = Convert.ToBase64String(gambarIlustrasiByte);

            //token = await LocalStorage.GetItemAsync<string>("token");

            //await GetListLokerSaya();
            //await GetPertanyaan();

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
                response = new DaftarResponse
                {
                    IdPelamar = result.IdPelamar,
                    Message = result.Message,
                    Token = result.Token
                };
            }
            catch (Exception ex)
            {
                message = ex.Message;
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

            //var lokerSaya = await LokerService.ListDaftarLokerSaya(token);
            //message = lokerSaya.info;

            //if (lokerSaya.lokerSayaResources != null)
            //    loker = lokerSaya.lokerSayaResources;


        }

        protected async Task GetPertanyaan()
        {
            var idLoker = await LocalStorage.GetItemAsync<int>("IdLoker");
            var token = await LocalStorage.GetItemAsync<string>("Token");

            try
            {
                var pertanyaan = await LokerService.FormPertanyaan(token, idLoker);
                messagePertanyaan = pertanyaan.Message;
                Console.WriteLine(messagePertanyaan);

                if (pertanyaan.Pertanyaan != null)
                {
                    foreach (var item in pertanyaan.Pertanyaan)
                    {
                        Console.WriteLine(item.Pertanyaan);
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }         
        }

    }
}

