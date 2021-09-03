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
        IJSRuntime JSRuntime { get; set; }

        protected IHttpContextAccessor httpContextAccessor;

        protected List<KalimatMotivasiResoruce> listMotivasi = new List<KalimatMotivasiResoruce>();

        protected byte[] gambarByte;
        protected string gambar;
        protected HttpRequest requestt;
        protected HttpContext Contextt;
        public string UserAgent { get; set; }
        public string IPAddress { get; set; }
        public string browserUser { get; set; }

        protected override void OnInitialized()
        {
    
        }

        protected override async Task OnInitializedAsync()
        {
            //listMotivasi = (List<KalimatMotivasiResoruce>)await motivationService.GetListKalimat();
            gambarByte = await motivationService.GetGambarMotivasi();
            gambar = Convert.ToBase64String(gambarByte);

            browserUser = await JSRuntime.InvokeAsync<string>("myBrowser");

            //UserAgent = httpContextAccessor.HttpContext.Request.Headers["User-Agent"];
            //IPAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        }



        protected async void SaveCounter(CounterResoruce counterResoruce)
        {
            await settingService.SaveCounter(counterResoruce);
        }



    }
}