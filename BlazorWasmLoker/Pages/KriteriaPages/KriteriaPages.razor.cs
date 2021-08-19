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
        protected object lokerSaya;
        protected List<LokerSayaResource> lokerCasting;

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

            //di type casting 
            //sudah saya casting di html foreach juga muncul key class ya tp di run masih error
           // lokerCasting = (List<LokerSayaResource>)lokerSaya;




            




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

