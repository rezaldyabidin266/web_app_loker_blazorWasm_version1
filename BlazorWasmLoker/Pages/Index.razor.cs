using BlazorWasmLoker.Resoruces.Motivations;
using BlazorWasmLoker.Resoruces.Settings;
using BlazorWasmLoker.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFungsi;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

namespace BlazorWasmLoker.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject]
        protected MotivationService motivationService { get; set; }
        protected UserService UserService { get; set; }
        [Inject]
        protected SettingService settingService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        [Inject]
        protected Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }
        protected IHttpContextAccessor httpContextAccessor;

        protected List<KalimatMotivasiResoruce> listMotivasi = new List<KalimatMotivasiResoruce>();
        protected int idKalimat;
        protected byte[] gambarByte;
        protected string gambar;
        protected HttpRequest requestt;
        protected HttpContext Contextt;
        public string UserAgent { get; set; }
        public string IPAddress { get; set; }
        public string browserUser { get; set; }
        public string computerScreen { get; set; }
        public bool doNotTrack { get; set; }
        public string userReferrer { get; set; }
        public string userPathName { get; set; }
        public string getUserAgent { get; set; }
        public IpResource getIpvalue { get; set; } = new IpResource() { };
        public IpDocResource getIpDoc { get; set; } = new IpDocResource() { };
        public CounterResoruce saveCounter { get; set; }

        protected string swResponse;

        protected override void OnInitialized()
        {
        
        }

        protected override async Task OnInitializedAsync()
        {
          
            gambarByte = await motivationService.GetGambarMotivasi();
            gambar = Convert.ToBase64String(gambarByte);
            browserUser = await JSRuntime.InvokeAsync<string>("myBrowser");
            computerScreen = await JSRuntime.InvokeAsync<string>("deviceScreen");
            doNotTrack = await JSRuntime.InvokeAsync<bool>("doNotTrack");
            userReferrer = await JSRuntime.InvokeAsync<string>("userReferrer");
            getUserAgent = await JSRuntime.InvokeAsync<string>("getUserAgent");
            userPathName = NavigationManager.Uri;

            await getIp();
            await IpDoc();
            await SaveCounter();
        }

        protected async Task getIp()
        {
            try
            {
                getIpvalue = await settingService.getIp();        
            }
            catch (Exception ex)
            {
                settingService.JsConsoleLog(ex.Message + " error");
            }
        }
        protected async Task IpDoc()
        {
            try
            {
                getIpDoc = await settingService.getDocIp(getIpvalue.ip);
            }
            catch (Exception ex)
            {
                settingService.JsConsoleLog(ex.Message + " error");
            }
        }

        protected async Task SaveCounter()
        {   
            try
            {
                saveCounter = new CounterResoruce
                {
                    Browser = browserUser,
                    IpAddress = getIpvalue.ip,
                    platform = $"{getUserAgent})",
                    ComputerScreen = computerScreen,
                    Page = userPathName,
                    Location = $"{getIpDoc.city},{getIpDoc.country_name}",
                    Isp = getIpDoc.org,
                    Referred = userReferrer,
                    IsDoNotTrack = doNotTrack,
                };
                await settingService.SaveCounter(saveCounter);
            }
            catch (Exception ex)
            {
                settingService.JsConsoleLog(ex.Message + " error");
            }
        }
   
    }
}