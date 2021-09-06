using BlazorWasmLoker.Resoruces.Lokers;
using BlazorWasmLoker.Resoruces.Motivations;
using BlazorWasmLoker.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        [Inject]
        protected MotivationService motivationService { get; set; }

        protected List<LokerResource> lokers = new List<LokerResource>();
        protected string ErrorMessage;
        protected List<KalimatMotivasiResoruce> listMotivasi = new List<KalimatMotivasiResoruce>();
        protected int idKalimat;

        protected override async Task OnInitializedAsync()
        {
            await GetLoker();
            await getKalimat();
        }

        protected async Task GetLoker()
        {
            try
            {
                lokers = await lokerService.ListLoker();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        protected async Task getKalimat()
        {
            listMotivasi = await motivationService.GetListKalimat();
            foreach (var item in listMotivasi)
            {
                idKalimat = item.Id;
            }

            await JSRuntime.InvokeAsync<object>("ketikan", listMotivasi, idKalimat);

        }

        protected void RouteKriteria(int idLoker)
        {
            NavigationManager.NavigateTo("/kriteria/" + idLoker);
            LocalStorage.SetItemAsync("IdLoker", idLoker);

        }
    }
}