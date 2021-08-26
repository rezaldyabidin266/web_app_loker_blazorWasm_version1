using BlazorWasmLoker.Resoruces.Lokers;
using BlazorWasmLoker.Services;
using BlazorWasmLoker.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections;
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
        protected DateTime Date = DateTime.Today;
        protected MyHelper.BentukIsian bentukIsian { get; set; }



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

        protected override async void OnAfterRender(bool firstRender)
        {
            await JSRuntime.InvokeVoidAsync("toastShow");
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

            jawabans.Add(jawab);
            LokerService.JsConsoleLog(jawab);
            LokerService.JsConsoleLog(jawaban);

        }

        protected void pushJawabanCheckBox(object jawabanHtml, int id, string pertanyaan, string bentukIsian)
        {


            if (bentukIsian == MyHelper.InfoBentukIsian(MyHelper.BentukIsian.PilihanGanda))
            {

                string jawabanString = jawabanHtml.ToString();
                jawab = new JawabanResoruce
                {
                    Id = id,
                    Pertanyaan = pertanyaan,
                    Jawaban = jawabanString,
                    Nominal = 1,
                    Tanggal = DateTime.Now,
                    FilePendukung = null,
                    JawabanTambahan = null
                };
                jawabans.Add(jawab);
                LokerService.JsConsoleLog(jawabanString);
            }
            else
            {
                //object -> List<object> kalau gak di ubah hasilnya system-sytem
                var result = ((IEnumerable)jawabanHtml).Cast<object>().ToList();
                //List<Object> -> string
                string arrayConver = string.Join(",", result);

                jawab = new JawabanResoruce
                {
                    Id = id,
                    Pertanyaan = pertanyaan,
                    Jawaban = arrayConver,
                    Nominal = 1,
                    Tanggal = DateTime.Now,
                    FilePendukung = null,
                    JawabanTambahan = null
                };
                jawabans.Add(jawab);
                LokerService.JsConsoleLog(arrayConver);
            }



            LokerService.JsConsoleLog(jawabans);
        
        }

        protected void pushJawabanNominal(int e, int id, string pertanyaan, string bentukIsian)
        {
            var jawaban = e.ToString();
            jawab = new JawabanResoruce
            {
                Id = id,
                Pertanyaan = pertanyaan,
                Jawaban = jawaban,
                Nominal = e,
                Tanggal = DateTime.Now,
                FilePendukung = null,
                JawabanTambahan = null
            };
            jawabans.Add(jawab);

            LokerService.JsConsoleLog(jawab);
            LokerService.JsConsoleLog(jawabans);
        }

        protected void pushJawabanDate(DateTime e, int id, string pertanyaan, string bentukIsian)
        {
            Date = e;
            var jawaban = e.ToString("yyyy-MM-dd");
            jawab = new JawabanResoruce
            {
                Id = id,
                Pertanyaan = pertanyaan,
                Jawaban = jawaban,
                Nominal = 1,
                Tanggal = e,
                FilePendukung = null,
                JawabanTambahan = null
            };
            jawabans.Add(jawab);
        }

        protected async void Save()
        {
            LokerService.JsConsoleLog(jawabans);
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