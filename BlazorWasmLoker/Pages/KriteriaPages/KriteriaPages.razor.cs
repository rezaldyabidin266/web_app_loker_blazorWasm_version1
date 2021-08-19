using BlazorWasmLoker.Resoruces.Lokers;
using BlazorWasmLoker.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFungsi;

namespace BlazorWasmLoker.Pages.KriteriaPages
{
    public partial class KriteriaPages : ComponentBase
    {
        [Inject]
        protected LokerService LokerService { get; set; }

        [Parameter]
        public int IdLoker { get; set; }

        protected LokerResource Lokers { get; set; } = new LokerResource();

        protected List<string> kriteriaResc = new();

        protected override async Task OnInitializedAsync()
        {
            Lokers = await LokerService.GetLoker(IdLoker);
            kriteriaResc = await LokerService.GetKriteria(IdLoker);
        }
    }
}
