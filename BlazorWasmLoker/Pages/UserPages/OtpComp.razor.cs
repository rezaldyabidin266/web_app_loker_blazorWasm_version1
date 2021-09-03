using BlazorWasmLoker.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWasmLoker.Pages.UserPages
{
    public class OtpCompBase : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        protected UserService userService { get; set; }
        [Inject]
        protected Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        protected string otp;
        protected char maskChar = ' ';
        protected bool spin = false;

        protected async Task onSubmit()
        {
            spin = true;
            try
            {
                var result = await userService.ResetPassword(otp);
                await JSRuntime.InvokeVoidAsync("notifDev", result, "success", 3000);               
                spin = false;
                NavigationManager.NavigateTo("/buatPassword");
            }
            catch (Exception ex)
            {
                userService.JsConsoleLog(otp);
                spin = false;
                await JSRuntime.InvokeVoidAsync("notifDev", ex.Message, "error", 3000);
            }
        }
    }
}
