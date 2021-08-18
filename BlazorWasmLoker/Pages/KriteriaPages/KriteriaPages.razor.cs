using BlazorWasmLoker.Resoruces.Lokers;
using BlazorWasmLoker.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWasmLoker.Pages.KriteriaPages
{
    public partial class KriteriaPages : ComponentBase
    {
        [Inject]
        protected LokerService lokerService { get; set; }

        [Parameter]
        public int idLoker { get; set; }


        protected LokerResource lokers { get; set; } = new LokerResource();
        protected string[] kriteriaResc = new string[] { }; 

        protected override async Task OnInitializedAsync()
        {
            lokers = await lokerService.GetLoker(Convert.ToInt32(idLoker));

            kriteriaResc = await lokerService.GetKriteria(Convert.ToInt32(idLoker));
        }
    }
}
