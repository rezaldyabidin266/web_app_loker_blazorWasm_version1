﻿using BlazorWasmLoker.Resoruces.Lokers;
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
        protected FromPertanyaanResoruce FormPertanyaanSingle = new();
        protected List<JawabanResoruce> jawabans = new List<JawabanResoruce>();
        public EditContext editContext { get; set; }
        protected string messageGetPertanyaan;
        protected string token;
        protected JawabanResoruce jawab;
        protected DateTime Date = DateTime.Today;
        protected MyHelper.BentukIsian bentukIsian { get; set; }
        protected List<string> valueCheckbox { get; set; }
        protected int valueNominal { get; set; }
        protected int nominalHtml { get; set; }
        protected bool nominalHtmlEdit { get; set; } = false;
        protected string valuePilihanGanda { get; set; }
        protected DateTime valueDate { get; set; }
        protected DateTime valuezz { get; set; }
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
                await JSRuntime.InvokeVoidAsync("notifDev", messageGetPertanyaan, "error", 3000);
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
                LokerService.JsConsoleLog(jawabans);

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

                nominalHtml = jawabanInt;
                valueNominal = jawabanInt;
                nominalHtmlEdit = true;
                LokerService.JsConsoleLog(nominalHtml);
                if (jawabanInt == 0)
                {
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
                }
                else
                {
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
                }

                jawabans.Add(jawab);
                LokerService.JsConsoleLog(jawabans);
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
           
            try
            {
               
                foreach (var item in FormPertanyaan)
                {
                    if (item.BentukIsian == MyHelper.InfoBentukIsian(MyHelper.BentukIsian.Nominal))
                    {

                        jawab = new JawabanResoruce
                        {
                            Id = item.Id,
                            Pertanyaan = item.Pertanyaan,
                            Jawaban = item.Jawaban,
                            Nominal = int.Parse(item.Jawaban.Replace(",", "")),
                            Tanggal = DateTime.Now,
                            FilePendukung = "kosong"
                        };
                    }
                    else if (item.BentukIsian == MyHelper.InfoBentukIsian(MyHelper.BentukIsian.Tanggal))
                    {
                        jawab = new JawabanResoruce
                        {
                            Id = item.Id,
                            Pertanyaan = item.Pertanyaan,
                            Jawaban = item.Jawaban,
                            Nominal = 1,
                            Tanggal = Convert.ToDateTime(item.Jawaban),
                            FilePendukung = "kosong"
                        };
                    }
                    else
                    {
                        jawab = new JawabanResoruce
                        {
                            Id = item.Id,
                            Pertanyaan = item.Pertanyaan,
                            Jawaban = item.Jawaban,
                            Nominal = 1,
                            Tanggal = DateTime.Now,
                            FilePendukung = "kosong"
                        };
                    }
                    jawabans.Add(jawab);
                }

                var post = await LokerService.FormSaveListJawaban(jawabans);
                LokerService.JsConsoleLog(FormPertanyaan);
                LokerService.JsConsoleLog(jawabans);
                messageGetPertanyaan = "Sukses Update Form";
                await JSRuntime.InvokeVoidAsync("notifDev", messageGetPertanyaan, "success", 3000);

            }
            catch (Exception ex)
            {
                messageGetPertanyaan = ex.Message;
                await JSRuntime.InvokeVoidAsync("notifDev", messageGetPertanyaan, "error", 3000);
            }
        }
    }
}

