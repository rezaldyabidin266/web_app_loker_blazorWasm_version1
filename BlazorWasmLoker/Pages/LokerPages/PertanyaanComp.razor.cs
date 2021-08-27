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
using System.Reflection;
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
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        protected List<FromPertanyaanResoruce> FormPertanyaan = new List<FromPertanyaanResoruce>();
        protected List<JawabanResoruce> jawabans = new List<JawabanResoruce>();
        public EditContext editContext { get; set; }
        protected string messageGetPertanyaan;
        protected string token;
        protected string messagePostPertanyaan;
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
                // messageGetPertanyaan = root.Message;

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
                messageGetPertanyaan = ex.Message;
                LokerService.GotoLogin();
                LokerService.JsConsoleLog(messageGetPertanyaan);
            }
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            await JSRuntime.InvokeVoidAsync("toastShow");
        }

        protected void pushJawaban(object jawabanHtml, int id, string pertanyaan, string bentukIsian)
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

            }
            else if (bentukIsian == MyHelper.InfoBentukIsian(MyHelper.BentukIsian.Checkbox))
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

            }
            else if (bentukIsian == MyHelper.InfoBentukIsian(MyHelper.BentukIsian.Paragraf))
            {
                //Get value di ChangeEvent
                var prop = jawabanHtml.GetType().GetProperties().First(o => o.Name == "Value").GetValue(jawabanHtml, null);
                var jawaban = prop.ToString();
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

            }
            else if (bentukIsian == MyHelper.InfoBentukIsian(MyHelper.BentukIsian.SimpleText))
            {
                //Get value di ChangeEvent
                var prop = jawabanHtml.GetType().GetProperties().First(o => o.Name == "Value").GetValue(jawabanHtml, null);
                var jawaban = prop.ToString();
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

            }
            else if (bentukIsian == MyHelper.InfoBentukIsian(MyHelper.BentukIsian.YesNo))
            {
                //Get value di ChangeEvent
                var prop = jawabanHtml.GetType().GetProperties().First(o => o.Name == "Value").GetValue(jawabanHtml, null);
                var jawaban = prop.ToString();
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

            }
            else if (bentukIsian == MyHelper.InfoBentukIsian(MyHelper.BentukIsian.Nominal))
            {
                var jawaban = jawabanHtml.ToString();
                var jawabanInt = (int)jawabanHtml;
                jawab = new JawabanResoruce
                {
                    Id = id,
                    Pertanyaan = pertanyaan,
                    Jawaban = jawaban,
                    Nominal = jawabanInt,
                    Tanggal = DateTime.Now,
                    FilePendukung = null,
                    JawabanTambahan = null
                };
                jawabans.Add(jawab);
            }
            else if (bentukIsian == MyHelper.InfoBentukIsian(MyHelper.BentukIsian.Tanggal))
            {

                Date = Convert.ToDateTime(jawabanHtml);
                var jawaban = Date.ToString("yyyy-MM-dd");

                if (MyFungsi.Helper.IsNotEmpty(jawaban))
                {
                    jawab = new JawabanResoruce
                    {
                        Id = id,
                        Pertanyaan = pertanyaan,
                        Jawaban = jawaban,
                        Nominal = 1,
                        Tanggal = Date,
                        FilePendukung = null,
                        JawabanTambahan = null
                    };
                }
                else
                {
                    jawab = new JawabanResoruce
                    {
                        Id = id,
                        Pertanyaan = pertanyaan,
                        Jawaban = DateTime.Now.ToString(),
                        Nominal = 1,
                        Tanggal = DateTime.Now,
                        FilePendukung = null,
                        JawabanTambahan = null
                    };
                }

                jawabans.Add(jawab);
            }
        }
        protected async void Save()
        {
            LokerService.JsConsoleLog(jawabans);
            try
            {
                var post = await LokerService.FormSaveListJawaban(jawabans);
                messagePostPertanyaan = "Sukses isi form";
                NavigationManager.NavigateTo("/pengalaman");
            }
            catch (Exception ex)
            {
                messagePostPertanyaan = ex.Message;
            }
        }
    }
}