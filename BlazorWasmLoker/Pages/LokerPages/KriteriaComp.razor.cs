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
using System.Timers;

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
        protected bool logIn;

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
        protected MaskAutoCompleteMode AutoCompleteMode { get; set; } = MaskAutoCompleteMode.Strong;
        protected string EmailMask { get; set; } = @"(\w|[.-])+@(\w|-)+\.(\w|-){2,4}";
        protected bool PlaceholderVisible { get; set; } = true;
        //Mask default
        protected string DateTimeMaskValue { get; set; } = DateTimeMask.LongDate;

        protected string fade = "fade show";

        //Spinner
        protected bool spin = false;

        //Timer
        protected Timer timer = new Timer();
        protected string userNetwork;

        protected override void OnInitialized()
        {
            editContext = new EditContext(DaftarResource);
        }
        protected override async Task OnInitializedAsync()
        {
            token = await LocalStorage.GetItemAsync<string>("token");
            await GetLoker(IdLoker);
            await KriteriaPelamar(IdLoker);
            await GambarBg(IdLoker);
            await GambarIl(IdLoker);

            timer.Interval = 1000;
            timer.Elapsed += async (s, e) =>
            {
                userNetwork = await LocalStorage.GetItemAsync<string>("statusNetwork");
                await InvokeAsync(StateHasChanged);
            };
            timer.Start();

            var logInLocal = await LocalStorage.GetItemAsync<string>("logIn");
            if (logInLocal == "true")
            {
                logIn = true;
            }
            else
            {
                logIn = false;
            }
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
                await JSRuntime.InvokeVoidAsync("notifDev", ex.Message, "error", 3000);
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
                await JSRuntime.InvokeVoidAsync("notifDev", ex.Message, "error", 3000);
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
                await JSRuntime.InvokeVoidAsync("notifDev", ex.Message, "error", 3000);
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
                await JSRuntime.InvokeVoidAsync("notifDev", ex.Message, "error", 3000);
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
                    await JSRuntime.InvokeVoidAsync("notifDev", message, "success", 3000);
                }
                catch (Exception ex)
                {
                    
                    spin = false;
                    if (userNetwork != "Online")
                    {
                        await JSRuntime.InvokeVoidAsync("notifDev", "Berhasil request data offline, diharapkan segera online", "warning", 5000);
                    }
                    else
                    {
                        message = ex.Message;
                        await JSRuntime.InvokeVoidAsync("notifDev", message, "error", 3000);
                    }
                }
            }
            else
            {
                spin = false;
                LokerService.JsConsoleLog("Form Invalid");
                await JSRuntime.InvokeVoidAsync("notifDev", "Form Invalid", "error", 3000);

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
                await JSRuntime.InvokeVoidAsync("notifDev", message, "error", 3000);
            }

        }

        protected void goPertanyaan()
        {
            NavigationManager.NavigateTo("/pertanyaan");
        }

    }
}
