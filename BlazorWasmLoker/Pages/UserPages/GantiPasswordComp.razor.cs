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

        protected string message;

        protected override async Task OnInitializedAsync()
        {
            token = await LocalStorage.GetItemAsync<string>("token");
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            await JSRuntime.InvokeVoidAsync("toastShow");
        }

        protected async Task submit()
        {
            if (passwordBaru == passwordConfirm)
            {
                try
                {
                    var gantiPassword = new GantiPasswordResoruce
                    {
                        passwrodLama = passwordLama,
                        passwrodBaru = passwordBaru,
                        token = token
                    };
                    message = await userService.GantiPassword(gantiPassword);
                    userService.GotoLogin();
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                   
                }
            }
            else
            {
                message = "Password tidak sama";
               
            }
        }
    }
}
