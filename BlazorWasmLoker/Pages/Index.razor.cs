using BlazorWasmLoker.Resoruces.Motivations;
using BlazorWasmLoker.Resoruces.Settings;
using BlazorWasmLoker.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFungsi;

namespace BlazorWasmLoker.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject]
        protected MotivationService motivationService { get; set; }
        [Inject]
        protected SettingService settingService { get; set; }
        protected List<KalimatMotivasiResoruce> listMotivasi = new List<KalimatMotivasiResoruce>();

        protected byte[] gambarByte;
        protected string gambar;

        protected override async Task OnInitializedAsync()
        {
            listMotivasi = (List<KalimatMotivasiResoruce>)await motivationService.GetListKalimat();
            gambarByte = await motivationService.GetGambarMotivasi();
            gambar = Convert.ToBase64String(gambarByte);
        }

        protected async void SaveCounter(CounterResoruce counterResoruce)
        {
            await settingService.SaveCounter(counterResoruce);
        }



    }
}