using BlazorWasmLoker.Resoruces;
using BlazorWasmLoker.Responds;
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
        protected string Email;
        protected string Password;
        protected LoginRespond loginRespond;
        protected async Task loginAsync()
        {
            var login = new UserLoginResource { Email = Email, Password = Password, Browser = string.Empty, IpAddress = string.Empty };
            var result = await userService.Login(login);
            loginRespond = new LoginRespond
            {
                Token = result.Token,
                Message = result.Message
            };
        }
    }
}