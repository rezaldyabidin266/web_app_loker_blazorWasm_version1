using BlazorWasmLoker.Resoruces.Lokers;
using BlazorWasmLoker.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace BlazorWasmLoker.Pages.LokerPages
{
    public class LokerCompBase : ComponentBase
    {
        [Inject]
        protected LokerService lokerService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        protected Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }

        protected List<LokerResource> lokers;

        protected override async Task OnInitializedAsync()
        {
            
            lokers = (List<LokerResource>)await lokerService.ListLoker();
            
        }

        protected string JudulLowongan;

        protected void GetLoker(int idLoker)
        {
            //JudulLowongan = lokers.Single(x => x.Id == idLoker).Keterangan;
            NavigationManager.NavigateTo("/kriteria/" + idLoker);

            LocalStorage.SetItemAsync("IdLoker", idLoker);
        }
    }
}