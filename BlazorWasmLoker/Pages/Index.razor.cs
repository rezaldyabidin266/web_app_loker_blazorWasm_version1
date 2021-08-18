using BlazorWasmLoker.Resoruces.Motivations;
using BlazorWasmLoker.Resoruces.Settings;
using BlazorWasmLoker.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWasmLoker.Pages
{
    public  class IndexBase : ComponentBase
    {
        [Inject]
        protected MotivationService motivationService { get; set; }

        [Inject]
        protected SettingService settingService { get; set; }

        protected List<KalimatMotivasiResoruce> listMotivasi = new List<KalimatMotivasiResoruce>();
        
        protected override async Task OnInitializedAsync()
        {
            listMotivasi = (List<KalimatMotivasiResoruce>)await motivationService.GetListKalimat();
        }
        protected async void SaveCounter(CounterResoruce counterResoruce)
        {            
            await settingService.SaveCounter(counterResoruce);
        }

        protected async Task<byte[]> GetGambarMotivasi()
        {
           return await motivationService.GetGamabarMotivasiFtp();
        }


    }
}