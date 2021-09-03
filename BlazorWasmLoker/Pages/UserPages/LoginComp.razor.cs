using BlazorWasmLoker.Resoruces.Users;
using BlazorWasmLoker.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace BlazorWasmLoker.Pages.UserPages
{
    public class LoginCompBase : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        protected UserService userService { get; set; }
        [Inject]
        protected Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        public UserLoginResource UserLoginResource = new UserLoginResource() { };
        public EditContext editContext { get; set; }
        protected string Email;
        protected string Password;
        protected TokenResource loginRespond;
        protected string MessageLoginTrue;
        protected string MessageLogin;
        protected string MessageErrorInvalid;

        protected override void OnInitialized()
        {
            editContext = new EditContext(UserLoginResource);
        }
        protected async Task loginAsync()
        {
            /* var login = new UserLoginResource { Email = Email, Password = Password, Browser = string.Empty, IpAddress = string.Empty }*/
            if (editContext.Validate())
            {
                try
                {
                    var result = await userService.Login(UserLoginResource);
                    string logIn = "true";
                    await LocalStorage.SetItemAsync("token", result.Token);
                    await LocalStorage.SetItemAsync<string>("logIn",logIn);
                    loginRespond = new TokenResource
                    {
                        Token = result.Token,
                        Message = result.Message
                    };
                    MessageLoginTrue = loginRespond.Message;
                    NavigationManager.NavigateTo("/loker");

                }
                catch (Exception ex)
                {
                    var result = JsonConvert.DeserializeObject<TokenResource>(ex.Message);
                    MessageLoginTrue = result.Message ;
                    await JSRuntime.InvokeVoidAsync("notifDev", result.Message,"error",3000);
                }
            }
            else
            {
                userService.JsConsoleLog("FormInvalid");
                await JSRuntime.InvokeVoidAsync("notifDev", "Form Invalid", "error", 3000);

            }
        }

        protected void goRequestOtp()
        {
            NavigationManager.NavigateTo("/requestOtp");
        }
        protected void goRegister()
        {
            NavigationManager.NavigateTo("/register");
        }
    }
}