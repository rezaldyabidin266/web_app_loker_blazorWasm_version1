using BlazorWasmLoker.Resoruces;
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
        protected List<LokerResource> lokers;

        [Inject]
        protected LokerService lokerService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            lokers = (List<LokerResource>)await lokerService.ListLoker();
        }

        protected async Task<LokerResource> GetLokerAsync(int idLoker)
        {
            return await lokerService.GetLoker(idLoker);
        }
    }
}