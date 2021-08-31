using BlazorWasmLoker.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWasmLoker.Pages.UserPages
{
    public class RequestOtpResetPasswordCompBase : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        protected UserService userService { get; set; }
        [Inject]
        protected Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        protected char maskChar = ' ';
        protected bool spin = false;
        protected string message;

        public string Email { get; set; }
        public string NoTelepon { get; set; }


        protected async Task submitReq()
        {
            spin = true;
            try
            {
                //var noTelepon = NoTelepon.ToString();
                message = await userService.RequestOtpResetPassword(Email, NoTelepon);
                userService.JsConsoleLog(message);
                spin = false;
            }
            catch (Exception ex)
            {
                spin = false;
                message = ex.Message;
                userService.JsConsoleLog(message);

            }
        }

    }
}
