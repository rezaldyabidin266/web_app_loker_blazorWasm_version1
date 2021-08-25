using BlazorWasmLoker.Resoruces.Lokers;
using BlazorWasmLoker.Services;
using BlazorWasmLoker.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
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
        [Inject]
        protected Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        protected List<FromPertanyaanResoruce> FormPertanyaan = new List<FromPertanyaanResoruce>();
        protected List<JawabanResoruce> jawabans = new List<JawabanResoruce>();
        public EditContext editContext { get; set; }
        protected string message;
        protected string token;
        protected string ErrorMessage;
        protected JawabanResoruce jawab;

        protected override void OnInitialized()
        {
            editContext = new EditContext(FormPertanyaan);
        }

        protected override async Task OnInitializedAsync()
        {



            token = await LocalStorage.GetItemAsync<string>("token");
            var idLoker = await LocalStorage.GetItemAsync<int>("IdLoker");
            try
            {
                var root = await LokerService.FormPertanyaan(token, idLoker);
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

        protected void pushJawaban(ChangeEventArgs e, int id, string pertanyaan, string bentukIsian)
        {

            var jawaban = (string)e.Value;
            jawab = new JawabanResoruce
            {
                Id = id,
                Pertanyaan = pertanyaan,
                Jawaban = jawaban,
                Nominal = 1,
                Tanggal = DateTime.Now,
                FilePendukung = null,
                JawabanTambahan = null
            };

            //var nominalterbilang = MyFungsi.Helper.Terbilang(10000000);
            //var umur = MyFungsi.Helper.HitungWaktu(DateTime.Now, DateTime.Now.AddYears(-20));


            //if (item.BentukIsian == MyHelper.InfoBentukIsian(MyHelper.BentukIsian.Tanggal))
            //{
            //    jawab.Tanggal = DateTime.Now.AddMonths(-3);
            //}
            //else if (item.BentukIsian == MyHelper.InfoBentukIsian(MyHelper.BentukIsian.Nominal))
            //{
            //    jawab.Nominal = 50000;
            //}
            jawabans.Add(jawab);
            LokerService.consoleLog(jawab);
            Console.WriteLine(jawab);

        }

        protected void pushJawabanCheckBox(object e, int id, string pertanyaan, string bentukIsian)
        {
        
            //var jawaban = (string)e;
            //var jawaban = string.Join(" ", e);
            string arrayConver = string.Join(",", e);
            string value = e.ToString();

            object varObject = e;

            //string arrayy = Array.ConvertAll((object)e, Convert.ToString);

            jawab = new JawabanResoruce
            {
                Id = id,
                Pertanyaan = pertanyaan,
                Jawaban = value,
                Nominal = 1,
                Tanggal = DateTime.Now,
                FilePendukung = null,
                JawabanTambahan = null
            };
            //jawabans.Add(jawab);

            JSRuntime.InvokeVoidAsync("console.log", e);
            LokerService.consoleLog(jawab);
            LokerService.consoleLog(arrayConver);
            LokerService.consoleLog(varObject);

            //LokerService.consoleLog(value);
        }
        protected async void Save()
        {

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