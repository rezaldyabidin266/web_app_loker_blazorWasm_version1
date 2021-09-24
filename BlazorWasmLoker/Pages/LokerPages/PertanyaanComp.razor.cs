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
using System.Timers;

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
        protected string messageGetPertanyaan;
        protected string token;
        protected string messagePostPertanyaan;
        protected JawabanResoruce jawab;
        protected DateTime Date = DateTime.Today;
        protected bool spin = false;

        //Timer
        protected Timer timer = new Timer();
        protected string userNetwork;

        protected override async Task OnInitializedAsync()
        {
            token = await LocalStorage.GetItemAsync<string>("token");

            timer.Interval = 1000;
            timer.Elapsed += async (s, e) =>
            {
                userNetwork = await LocalStorage.GetItemAsync<string>("statusNetwork");
                await InvokeAsync(StateHasChanged);
            };
            timer.Start();

            var idLoker = await LocalStorage.GetItemAsync<int>("IdLoker");
            try
            {
                var root = await LokerService.FormPertanyaan(token, idLoker);
                LokerService.JsConsoleLog(root.Pertanyaan);
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
                await JSRuntime.InvokeVoidAsync("notifDev", messageGetPertanyaan, "error", 3000);
            }
        }

        protected async void Save()
        {
            spin = true;
            try
            {
                foreach (var item in FormPertanyaan)
                {
                    if (item.BentukIsian == MyHelper.InfoBentukIsian(MyHelper.BentukIsian.Nominal))
                    {
                        var jawabanInt = int.Parse(item.Jawaban.Replace(",", ""));
                        if (jawabanInt == 0)
                        {
                            jawab = new JawabanResoruce
                            {
                                Id = item.Id,
                                Pertanyaan = item.Pertanyaan,
                                Jawaban = item.Jawaban,
                                Nominal = 1,
                                Tanggal = DateTime.Now,
                                FilePendukung = "kosong",
                            };
                        }
                        else
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

                    }
                    else if (item.BentukIsian == MyHelper.InfoBentukIsian(MyHelper.BentukIsian.Tanggal))
                    {
                        if (MyFungsi.Helper.IsNotEmpty(item.Jawaban))
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
                                Jawaban = DateTime.Now.ToString(),
                                Nominal = 1,
                                Tanggal = DateTime.Now,
                                FilePendukung = "kosong"
                            };
                        }
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

                spin = false;
                var post = await LokerService.FormSaveListJawaban(jawabans);
                messagePostPertanyaan = "Sukses isi form";
                NavigationManager.NavigateTo("/berkasPelamar");
                await JSRuntime.InvokeVoidAsync("notifDev", messagePostPertanyaan, "success", 3000);
            }
            catch (Exception ex)
            {
                spin = false;
                if (userNetwork != "Online")
                {
                    await JSRuntime.InvokeVoidAsync("notifDev", "Berhasil request data offline, diharapkan segera online", "warning", 5000);
                }
                else
                {
                    messagePostPertanyaan = ex.Message;
                    await JSRuntime.InvokeVoidAsync("notifDev", messagePostPertanyaan, "error", 3000);
                }
            }
        }
    }
}