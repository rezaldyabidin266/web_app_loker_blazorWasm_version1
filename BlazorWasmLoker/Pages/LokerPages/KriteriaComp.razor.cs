using BlazorWasmLoker.Resoruces.Lokers;
using BlazorWasmLoker.Resoruces.Users;
using BlazorWasmLoker.Services;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace BlazorWasmLoker.Pages.LokerPages
{
    public class KriteriaCompBase : ComponentBase
    {

        [Inject]
        protected LokerService LokerService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        protected Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Parameter]
        public int IdLoker { get; set; }

        protected LokerResource Lokers { get; set; } = new LokerResource();
        public DaftarResource DaftarResource = new DaftarResource() { TglLahir = DateTime.Today };
        public EditContext editContext { get; set; }

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

        //Mask Char
        protected char maskChar = ' ';
        protected char maskGmail = '_';
        protected MaskAutoCompleteMode AutoCompleteMode { get; set; } = MaskAutoCompleteMode.Strong;
        protected string EmailMask { get; set; } = @"(\w|[.-])+@(\w|-)+\.(\w|-){2,4}";
        //Mask default
        protected string DateTimeMaskValue { get; set; } = DateTimeMask.LongDate;

        protected string fade = "fade show";

        //Spinner
        protected bool spin = false;
        protected bool isVisible { get; set; } = false;

        protected override void OnInitialized()
        {
            editContext = new EditContext(DaftarResource);

          
        }
        protected override async void OnAfterRender(bool firstRender)
        {
            //await JSRuntime.InvokeVoidAsync("renderjQueryComponents");
            await JSRuntime.InvokeVoidAsync("toastShow");
        }

        protected override async Task OnInitializedAsync()
        {
            token = await LocalStorage.GetItemAsync<string>("token");

            //editContext = new(DaftarResource);

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

        protected async Task daftarSubmit()
        {
          
            spin = true;
            if (editContext.Validate())
            {

                try
                {
                    var result = await LokerService.DaftarNoRegister(DaftarResource);
                    await LocalStorage.SetItemAsync("token", result.Token);
                    NavigationManager.NavigateTo("/pertanyaan");
                    response = new DaftarResponse
                    {
                        IdPelamar = result.IdPelamar,
                        Message = result.Message,
                        Token = result.Token
                    };

                    spin = false;
                    message = result.Message;
                    isVisible = true;
                 
                }
                catch (Exception ex)
                {
                    spin = false;
                    message = ex.Message;
                    isVisible = true;
                   
                }
            }
            else
            {
                spin = false;
                message = "Form Invalid";
                isVisible = true;
                
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
