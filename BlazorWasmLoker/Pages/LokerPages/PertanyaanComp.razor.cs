using BlazorWasmLoker.Resoruces.Lokers;
using BlazorWasmLoker.Services;
using BlazorWasmLoker.Utility;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWasmLoker.Pages.LokerPages
{
    public class PertanyaanCompBase : ComponentBase
    {
        [Inject]
        protected LokerService LokerService { get; set; }

        protected List<FromPertanyaanResoruce> FormPertanyaan = new List<FromPertanyaanResoruce>();
        protected string message;

        protected string ErrorMessage;

        protected override async Task OnInitializedAsync()
        {
            await Task.Delay(1000);
            try
            {
                var root = await LokerService.FormPertanyaan("S+94GYw0nhZkzsUCIy1hUQ3BuJqsUkh0Dxx0rhVuCFt+ID2qzPb/SA==", 1);
                message = root.Message;

                foreach (var item in root.Pertanyaan)
                {
                    var data = new FromPertanyaanResoruce
                    {
                        Id = item.Id,
                        BentukIsian = item.BentukIsian,
                        IsRequired = item.IsRequired,
                        Jawaban = item.Jawaban,
                        No = item.No,
                        Pertanyaan = item.Pertanyaan,
                        Pilihan = item.Pilihan
                    };
                    FormPertanyaan.Add(data);
                }

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;

            }
        }


        protected async void Save()
        {
            var jawabans = new List<JawabanResoruce>();

            foreach (var item in FormPertanyaan)
            {
                var jawab = new JawabanResoruce
                {
                    Id = item.Id,
                    Jawaban = item.Jawaban,
                    FilePendukung = null,
                    JawabanTambahan = null
                };

                if (item.BentukIsian == MyHelper.InfoBentukIsian(MyHelper.BentukIsian.Tanggal))
                {
                    jawab.Tanggal = DateTime.Now.AddMonths(-3);
                }
                else if (item.BentukIsian == MyHelper.InfoBentukIsian(MyHelper.BentukIsian.Nominal))
                {
                    jawab.Nominal = 50000;
                }

                jawabans.Add(jawab);
            }

            try
            {
                message = await LokerService.FormSaveListJawaban(jawabans);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }
    }
}