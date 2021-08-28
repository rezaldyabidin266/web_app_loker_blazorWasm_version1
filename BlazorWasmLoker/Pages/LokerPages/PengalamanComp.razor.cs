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
        IJSRuntime JSRuntime { get; set; }

        protected string token;
        public PengalamanResoruce PengalamanResoruceMapping = new PengalamanResoruce() { TglAwal = DateTime.Now, TglAkhir = DateTime.Now };
        public List<PengalamanResourceId> PengalamanResoruceId = new List<PengalamanResourceId>() { };
        public EditContext editContext { get; set; }
        protected string messagePengalaman;
        protected string errorMessage;
        protected string suksesMessage;
        protected override void OnInitialized()
        {
            editContext = new EditContext(PengalamanResoruceMapping);
        }
        protected override async void OnAfterRender(bool firstRender)
        {
            //await JSRuntime.InvokeVoidAsync("toastShow");
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
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        protected async Task DeletePengalaman(MouseEventArgs e, int id)
        {
            //await JSRuntime.InvokeVoidAsync("toastShow");
            try
            {
                var tokenVar = await LocalStorage.GetItemAsync<string>("token");
                var result = await UserService.DeletePengalaman(token, id);
                suksesMessage = result;
                try
                {
                    PengalamanResoruceId = await UserService.ListPengalaman(token);
                }
                catch (Exception ex)
                {
                    UserService.JsConsoleLog(ex.Message);
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        protected async Task submitPengalaman()
        {
            if (editContext.Validate())
            {
                try
                {
                    var result = await UserService.AddPengalaman(token, PengalamanResoruceMapping);
                    suksesMessage = result;
                    try
                    {
                        PengalamanResoruceId = await UserService.ListPengalaman(token);
                    }
                    catch (Exception ex)
                    {

                         UserService.JsConsoleLog(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }
               
            }
            else
            {
                messagePengalaman = "Form Invalid";
            }
        }
    }
}
