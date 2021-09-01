using BlazorWasmLoker.Resoruces.Users;
using BlazorWasmLoker.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWasmLoker.Shared
{
    public class NavHeaderCompBase : ComponentBase
    {
        [Inject]
        protected LokerService LokerService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        protected Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        protected bool logIn;
        protected string token;
        protected List<LokerSayaResource> listDatar = new List<LokerSayaResource>();
        protected string tanggalDaftar;
        protected override void OnInitialized()
        {
            JSRuntime.InvokeAsync<object>("navbarScroll");
        }
        protected override async Task OnInitializedAsync()
        {
            var logInLocal = await LocalStorage.GetItemAsync<string>("logIn");
            token = await LocalStorage.GetItemAsync<string>("token");

            await listDaftarPelamar();

            if (logInLocal == "true")
            {
                logIn = true;
            }
            else
            {
                logIn = false;
            }
        }

        protected async Task listDaftarPelamar()
        {
            try
            {
                listDatar = await LokerService.ListDaftarLokerSaya(token);
                foreach (var item in listDatar)
                {
                    tanggalDaftar = item.Tanggal.ToString("M/d/yy");
                }
            }
            catch (Exception ex)
            {
                LokerService.JsConsoleLog(ex.Message);
            }
        }
        protected void logout()
        {
            LocalStorage.ClearAsync();
            LokerService.GotoLogin();
        }
    }
}
