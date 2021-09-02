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
    public class ListDaftarPelamarCompBase : ComponentBase
    {
        [Inject]
        protected LokerService LokerService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        protected Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        protected string token;
        protected List<LokerSayaResource> listDatar = new List<LokerSayaResource>();
        protected string tanggalDaftar;

        protected override async Task OnInitializedAsync()
        {
            token = await LocalStorage.GetItemAsync<string>("token");
            await listDaftarPelamar();
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
        protected void pertanyaanId(int idPertanyaan)
        {
            NavigationManager.NavigateTo("/updatePertanyaan/" + idPertanyaan);
        }
    }
}
