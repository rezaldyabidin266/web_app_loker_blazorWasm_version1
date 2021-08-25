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

        protected List<LokerResource> lokers = new List<LokerResource>();
        protected string ErrorMessage;

        protected override async Task OnInitializedAsync()
        {
            await GetLoker();         
        }

        protected async Task GetLoker()
        {
            try
            {
                lokers = (List<LokerResource>)await lokerService.ListLoker();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        protected void RouteKriteria(int idLoker)
        {
            NavigationManager.NavigateTo("/kriteria/" + idLoker);
            LocalStorage.SetItemAsync("IdLoker", idLoker);

        }
    }
}