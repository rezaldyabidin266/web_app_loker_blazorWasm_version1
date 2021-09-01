using BlazorWasmLoker.Resoruces.Users;
using BlazorWasmLoker.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWasmLoker.Pages.LokerPages
{
    public class PengalamanCompBase : ComponentBase
    {
        [Inject]
        protected UserService UserService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        protected Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        protected string token;
        public PengalamanResoruce PengalamanResoruceMapping = new PengalamanResoruce() { TglAwal = DateTime.Now, TglAkhir = DateTime.Now };
        public List<PengalamanResourceId> PengalamanResoruceId = new List<PengalamanResourceId>() { };
        public EditContext editContext { get; set; }
        protected string messagePengalaman;
        protected string errorMessage;
        protected string suksesMessage;
        protected string TanggalAwalKerja;
        protected string TanggalAkhirKerja;
        protected bool spin = false;
        protected bool spinDelete = false;

        protected override void OnInitialized()
        {
            editContext = new EditContext(PengalamanResoruceMapping);
        }
        protected override async void OnAfterRender(bool firstRender)
        {
            await JSRuntime.InvokeVoidAsync("toastShow");
        }
        protected override async Task OnInitializedAsync()
        {
            token = await LocalStorage.GetItemAsync<string>("token");
            await ListPengalamanOnInit();
        }

        protected async Task ListPengalamanOnInit()
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
            catch (Exception ex)
            {
                UserService.GotoLogin();
                errorMessage = ex.Message;
            }
        }

        protected async Task DeletePengalaman(MouseEventArgs e, int id)
        {
          
            spinDelete = true;
            try
            {
                var tokenVar = await LocalStorage.GetItemAsync<string>("token");
                var result = await UserService.DeletePengalaman(token, id);
                suksesMessage = result;
                spinDelete = false;
                try
                {
                    PengalamanResoruceId = await UserService.ListPengalaman(token);
                    foreach (var item in PengalamanResoruceId)
                    {
                        TanggalAwalKerja = item.TglAwal.ToString("yyyy-MM-dd");
                        TanggalAkhirKerja = item.TglAkhir.ToString("yyyy-MM-dd");
                    }
                }
                catch (Exception ex)
                {
                    UserService.JsConsoleLog(ex.Message);
                }
            }
            catch (Exception ex)
            {
                spinDelete = false;
                suksesMessage = ex.Message;
            }
        }

        protected async Task submitPengalaman()
        {
            spin = true;
            if (editContext.Validate())
            {
                try
                {
                    var result = await UserService.AddPengalaman(token, PengalamanResoruceMapping);
                    suksesMessage = result;
                    spin = false;
                    try
                    {
                        PengalamanResoruceId = await UserService.ListPengalaman(token);
                        foreach (var item in PengalamanResoruceId)
                        {
                            TanggalAwalKerja = item.TglAwal.ToString("yyyy-MM-dd");
                            TanggalAkhirKerja = item.TglAkhir.ToString("yyyy-MM-dd");

                    
                        }
                    }
                    catch (Exception ex)
                    {

                         UserService.JsConsoleLog(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    spin = false;
                    suksesMessage = ex.Message;
                }
               
            }
            else
            {
                spin = false;
                messagePengalaman = "Form Invalid";
            }
        }
        protected void kirim()
        {
            NavigationManager.NavigateTo("/endPage");
        }
    }
}
