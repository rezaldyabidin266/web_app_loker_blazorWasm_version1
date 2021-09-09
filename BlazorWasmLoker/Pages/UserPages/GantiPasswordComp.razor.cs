using BlazorWasmLoker.Resoruces.Users;
using BlazorWasmLoker.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWasmLoker.Pages.UserPages
{
    public class GantiPasswordCompBase : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        protected UserService userService { get; set; }
        [Inject]
        protected Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        protected string passwordLama;
        protected string passwordBaru;
        protected string passwordConfirm;
        protected string token;
        protected bool spin = false;
        protected string message;

        protected override async Task OnInitializedAsync()
        {
            token = await LocalStorage.GetItemAsync<string>("token");
        }
        protected async Task submit()
        {
            spin = true;
            if (passwordBaru == passwordConfirm)
            {
                try
                {
                    spin = false;
                    var gantiPassword = new GantiPasswordResoruce
                    {
                        passwrodLama = passwordLama,
                        passwrodBaru = passwordBaru,
                        token = token
                    };
                    message = await userService.GantiPassword(gantiPassword);
                    await JSRuntime.InvokeVoidAsync("notifDev", message, "info", 3000);
                }
                catch (Exception ex)
                {
                    spin = false;
                    message = ex.Message;
                    await JSRuntime.InvokeVoidAsync("notifDev", message, "error", 3000);
                }
            }
            else
            {
                spin = false;
                message = "Password Confirm dan Password Baru tidak sama";
                await JSRuntime.InvokeVoidAsync("notifDev", message, "error", 3000);
            }
        }
    }
}
