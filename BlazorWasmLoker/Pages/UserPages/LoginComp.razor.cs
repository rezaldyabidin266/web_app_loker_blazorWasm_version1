using BlazorWasmLoker.Resoruces.Users;
using BlazorWasmLoker.Services;
using Microsoft.AspNetCore.Components;
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

        protected string Email;
        protected string Password;
        protected TokenResource loginRespond;
        protected async Task loginAsync()
        {
            var login = new UserLoginResource { Email = Email, Password = Password, Browser = string.Empty, IpAddress = string.Empty };
            var result = await userService.Login(login);

            await LocalStorage.SetItemAsync("token", result.Token);

            loginRespond = new TokenResource
            {
                Token = result.Token,
                Message = result.Message
            };
        }
    }
}