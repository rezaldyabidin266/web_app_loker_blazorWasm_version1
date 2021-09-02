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

namespace BlazorWasmLoker.Pages.UserPages
{
    public class UpdatePertanyaanPelamarCompBase : ComponentBase
    {
        [Parameter]
        public int IdPertanyaan { get; set; }
        [Inject]
        protected LokerService LokerService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        protected Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        protected List<FromPertanyaanResoruce> FormPertanyaan = new List<FromPertanyaanResoruce>();
        protected List<JawabanResoruce> jawabans = new List<JawabanResoruce>();
        public EditContext editContext { get; set; }
        protected string messageGetPertanyaan;
        protected string token;
        protected JawabanResoruce jawab;
        protected DateTime Date = DateTime.Today;
        protected MyHelper.BentukIsian bentukIsian { get; set; }
        protected List<string> valueCheckbox { get; set; }
        protected int valueNominal { get; set; }
        protected string valuePilihanGanda { get; set; }
        protected DateTime valueDate { get; set; }
        protected override void OnInitialized()
        {
            editContext = new EditContext(FormPertanyaan);
        }

        protected override async Task OnInitializedAsync()
        {
            token = await LocalStorage.GetItemAsync<string>("token");
            try
            {
                var root = await LokerService.FormPertanyaan(token, IdPertanyaan);
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

                    if (item.BentukIsian == MyHelper.InfoBentukIsian(MyHelper.BentukIsian.Checkbox))
                    {
                        valueCheckbox = item.Jawaban.Split(',').ToList();
                    }
                    else if (item.BentukIsian == MyHelper.InfoBentukIsian(MyHelper.BentukIsian.Nominal))
                    {
                        //(int.Parse(item.Jawaban.Replace(",", "")))
                        valueNominal = int.Parse(item.Jawaban.Replace(",", ""));
                    }
                    else if (item.BentukIsian == MyHelper.InfoBentukIsian(MyHelper.BentukIsian.PilihanGanda))
                    {
                        valuePilihanGanda = item.Jawaban;
                    }
                    else if (item.BentukIsian == MyHelper.InfoBentukIsian(MyHelper.BentukIsian.Tanggal))
                    {
                        // (Convert.ToDateTime(item.Jawaban))
                        valueDate = Convert.ToDateTime(item.Jawaban);
                    }
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
        protected void pushJawaban(object jawabanHtml, int id, string pertanyaan, string bentukIsian)
        {
            if (bentukIsian == MyHelper.InfoBentukIsian(MyHelper.BentukIsian.PilihanGanda))
            {
                string jawabanString = jawabanHtml.ToString();
                valuePilihanGanda = jawabanString;
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
                valueCheckbox = ((IEnumerable)jawabanHtml).Cast<string>().ToList();

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
                valueNominal = jawabanInt;

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

                valueDate = Date;

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
        protected async void SaveUpdate()
        {
            LokerService.JsConsoleLog(jawabans);
            try
            {
                var post = await LokerService.FormSaveListJawaban(jawabans);
                messageGetPertanyaan = "Sukses Update Form";
                LokerService.JsConsoleLog("Sukses Update Form");
            }
            catch (Exception ex)
            {
                messageGetPertanyaan = ex.Message;
            }
        }
    }
}
