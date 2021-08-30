using BlazorWasmLoker.Resoruces.Users;
using BlazorWasmLoker.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWasmLoker.Pages.UserPages
{
    public class RegisterCompBase : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        protected UserService userService { get; set; }
        [Inject]
        protected Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        public DaftarResource DaftarResource = new DaftarResource() { TglLahir = DateTime.Today };
        public EditContext editContext { get; set; }

        protected char maskChar = ' ';
        protected bool spin = false;
        protected string message;
        protected override void OnInitialized()
        {
            editContext = new EditContext(DaftarResource);
        }
        protected override async void OnAfterRender(bool firstRender)
        {
            await JSRuntime.InvokeVoidAsync("toastShow");
        }
        protected async Task RegisterSubmit()
       {
            if (editContext.Validate())
            {
                try
                {
                    message = await userService.Register(DaftarResource);
                    spin = false;
                }
                catch (Exception ex)
                {
                    spin = false;
                    message = ex.Message;
                }
            }
            else
            {
                userService.JsConsoleLog("Form Invalid");
            }
        }
    }
}
