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
        protected object lokerSaya { get; set; } = new object();
        protected List<LokerSayaResource> lokerListDaftar = new List<LokerSayaResource>();
        protected List<LokerSayaResource> lokerCasting = new List<LokerSayaResource>();
        protected JavaScriptSerializer JsonConvert2 = new JavaScriptSerializer();
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

            lokerSaya = await LokerService.ListDaftarLokerSaya(token);



            // LokerSayaResource c = JsonConvert.DeserializeObject<LokerSayaResource>(lokerSaya)

            //foreach ( var item in (List<LokerSayaResource>)lokerSaya)
            //{
            //    Console.WriteLine(item.StatusLamaran);
            //}

            //string serializeString = JsonConvert.Serialize(lokerSaya);
            //var json = JsonConvert.Deserialize<dynamic>(serializeString);
            //lokerListDaftar = new List<LokerSayaResource>(json);

            //lokerListDaftar = await LokerService.ListDaftarLokerSaya(token);

            //lokerSaya = await LokerService.ListDaftarLokerSaya(token); //JSONElEMENT


            string serializeString = JsonConvert2.Serialize(lokerSaya);
            Console.WriteLine(serializeString);
            lokerCasting = new List<LokerSayaResource>(JsonConvert2.Deserialize<List<LokerSayaResource>>(serializeString)) { };

            //pake jsonConvert.serializeObject
            //object seriall = JsonConvert.SerializeObject(lokerSaya);
            //var ress = JsonConvert.DeserializeObject(LokerSayaResource)(seriall);

            //di ubah jadi object coleksi
            //IList coleksi = (IList)lokerSaya;
            //var coleksi2 = new List<object>((IEnumerable<object>)lokerSaya);

            //foreach (PropertyInfo prop in lokerSaya.GetType().GetProperties())
            //{
            //    Console.WriteLine(prop.GetValue("");
            //}

            //var apalah = JsonConvert.SerializeObject(lokerSaya);
            //var model = JsonConvert.DeserializeObject<List<LokerSayaResource>>(apalah);

            //foreach(var item in model)
            //{
            //    Console.WriteLine(item.StatusLamaran);
            //}

            foreach(var item in lokerCasting)
            {
                Console.WriteLine(item.JudulLowongan);
                Console.WriteLine(item.StatusLamaran);
            }

            Console.WriteLine(lokerSaya.GetType());
            Console.WriteLine(lokerSaya.GetType().GetProperties());
            Console.WriteLine(lokerSaya);
            Console.WriteLine(lokerCasting);
       

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

 

    }
}

