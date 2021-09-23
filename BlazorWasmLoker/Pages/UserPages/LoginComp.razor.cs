using BlazorWasmLoker.Resoruces.Settings;
using BlazorWasmLoker.Resoruces.Users;
using BlazorWasmLoker.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
//using System.Web.Helpers;

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
        [Inject]
        protected SettingService settingService { get; set; }
        public UserLoginResource UserLoginResource = new UserLoginResource() { };
        public IpResource getIpvalue { get; set; } = new IpResource() { };
        public EditContext editContext { get; set; }
        protected string Email;
        protected string Password;
        public string browserUser { get; set; }
        protected TokenResource loginRespond;
        protected bool spin = false;
        protected bool sp = false;
        protected string userNetwork;
        protected bool showPassword
        {
            get
            {
                return sp;
            }
            set
            {
                sp = value;
                JSRuntime.InvokeVoidAsync("showHidePassword", "PasswordTextBox", sp);
            }
        }
        //Timer
        protected Timer timer = new Timer();



        protected override void OnInitialized()
        {
            editContext = new EditContext(UserLoginResource);
        }

        protected override async Task OnInitializedAsync()
        {

            timer.Interval = 1000;
            timer.Elapsed += async (s, e) =>
            {
                userNetwork = await LocalStorage.GetItemAsync<string>("statusNetwork");
                userService.JsConsoleLog(userNetwork);
                await InvokeAsync(StateHasChanged);
            };
            timer.Start();

            browserUser = await JSRuntime.InvokeAsync<string>("myBrowser");
            await getIp();
          

        }
        protected async Task getIp()
        {
            try
            {
                getIpvalue = await settingService.getIp();
            }
            catch (Exception ex)
            {
                userService.JsConsoleLog(ex.Message + " error");
            }
        }
        protected async Task loginAsync()
        {
            spin = true;

            /* var login = new UserLoginResource { Email = Email, Password = Password, Browser = string.Empty, IpAddress = string.Empty }*/
            if (editContext.Validate())
            {
                try
                {
             
                    UserLoginResource.IpAddress = getIpvalue.ip;
                    UserLoginResource.Browser = browserUser;
                    var result = await userService.Login(UserLoginResource);
                    string logIn = "true";
                    await LocalStorage.SetItemAsync("token", result.Token);
                    await LocalStorage.SetItemAsync<string>("logIn",logIn);
                    loginRespond = new TokenResource
                    {
                        Token = result.Token,
                        Message = result.Message
                    };
                    userService.JsConsoleLog(result.Token);
                    NavigationManager.NavigateTo("/loker");
                    spin = false;
                    //userService.JsConsoleLog(UserLoginResource);
                }
                catch (Exception ex)
                {
                    var result = JsonConvert.DeserializeObject<TokenResource>(ex.Message);
                    await JSRuntime.InvokeVoidAsync("notifDev", result.Message,"error",3000);
                    spin = false;
                }
            }
            else
            {
                userService.JsConsoleLog("FormInvalid");
                await JSRuntime.InvokeVoidAsync("notifDev", "Form Invalid", "error", 3000);
                spin = false;

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