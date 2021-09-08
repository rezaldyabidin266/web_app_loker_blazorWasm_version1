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
        protected string messageGetPertanyaan;
        protected string token;
        protected JawabanResoruce jawab;

        protected override void OnInitialized()
        {
         
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
    
        protected async void SaveUpdate()
        {
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

