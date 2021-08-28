using BlazorWasmLoker.Resoruces.Users;
using BlazorWasmLoker.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWasmLoker.Pages.UserPages
{
    public class LoginCompBase : ComponentBase
    {

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
        //protected override async void OnAfterRender(bool firstRender)
        //{

        //}
        protected async Task loginAsync()
        {
            await JSRuntime.InvokeVoidAsync("toastShow");
            /* var login = new UserLoginResource { Email = Email, Password = Password, Browser = string.Empty, IpAddress = string.Empty }*/
            if (editContext.Validate())
            {
                try
                {
                    var result = await userService.Login(UserLoginResource);
                    await LocalStorage.SetItemAsync("token", result.Token);
                    loginRespond = new TokenResource
                    {
                        Token = result.Token,
                        Message = result.Message
                    };
                    MessageLoginTrue = result.Message;

                }
                catch (Exception ex)
                {
                    MessageLogin = ex.Message;
                }
            }
            else
            {
                MessageErrorInvalid = "Form Invalid";
            }
        }
    }
}