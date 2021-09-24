using BlazorWasmLoker.Resoruces.Users;
using BlazorWasmLoker.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace BlazorWasmLoker.Pages.LokerPages
{
    public class PengalamanCompBase : ComponentBase
    {
        [Inject]
        protected UserService UserService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        protected Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        protected string token;
        public PengalamanResoruce PengalamanResoruceMapping = new PengalamanResoruce() { TglAwal = DateTime.Now, TglAkhir = DateTime.Now };
        public List<PengalamanResourceId> PengalamanResoruceId = new List<PengalamanResourceId>() { };
        public EditContext editContext { get; set; }
        protected string messagePengalaman;
        protected string errorMessage;
        protected string suksesMessage;
        protected string TanggalAwalKerja;
        protected string TanggalAkhirKerja;
        protected bool spin = false;
        protected bool spinDelete = false;
        protected ArrayList pengalamanId = new ArrayList();

        //Timer
        protected Timer timer = new Timer();
        protected string userNetwork;


        protected override void OnInitialized()
        {
            editContext = new EditContext(PengalamanResoruceMapping);
        }
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

            await ListPengalamanOnInit();
        }

        protected async Task ListPengalamanOnInit()
        {
            try
            {
                PengalamanResoruceId = await UserService.ListPengalaman(token);
                foreach (var item in PengalamanResoruceId)
                {
                    TanggalAwalKerja = item.TglAwal.ToString("yyyy-MM-dd");
                    TanggalAkhirKerja = item.TglAkhir.ToString("yyyy-MM-dd");
                }
            }
            catch (Exception ex)
            {
                UserService.GotoLogin();
                errorMessage = ex.Message;
                await JSRuntime.InvokeVoidAsync("notifDev", errorMessage, "error", 3000);
            }
        }

        protected async Task DeletePengalaman(MouseEventArgs e, int id)
        {

            pengalamanId.Add(id);
            await LocalStorage.SetItemAsync<ArrayList>("pengalamanIdArray", pengalamanId);

            spinDelete = true;
            try
            {
                var tokenVar = await LocalStorage.GetItemAsync<string>("token");
                var result = await UserService.DeletePengalaman(token, id);
                suksesMessage = result;
                await JSRuntime.InvokeVoidAsync("notifDev", suksesMessage, "success", 3000);
                spinDelete = false;
                try
                {
                    PengalamanResoruceId = await UserService.ListPengalaman(token);
                    foreach (var item in PengalamanResoruceId)
                    {
                        TanggalAwalKerja = item.TglAwal.ToString("yyyy-MM-dd");
                        TanggalAkhirKerja = item.TglAkhir.ToString("yyyy-MM-dd");
                    }
                }
                catch (Exception ex)
                {
                    await JSRuntime.InvokeVoidAsync("notifDev", ex.Message, "error", 3000);
                }
            }
            catch (Exception ex)
            {
                spinDelete = false;
                if (userNetwork != "Online")
                {
                    await JSRuntime.InvokeVoidAsync("notifDev", "Berhasil request data offline, diharapkan segera online", "warning", 5000);
                }
                else
                {
                    var MessageRespon = ex.Message;
                    await JSRuntime.InvokeVoidAsync("notifDev", MessageRespon, "error", 3000);
                }
            }
        }

        protected async Task submitPengalaman()
        {
            spin = true;
            if (editContext.Validate())
            {
                try
                {
                    var result = await UserService.AddPengalaman(token, PengalamanResoruceMapping);
                    suksesMessage = result;
                    await JSRuntime.InvokeVoidAsync("notifDev", suksesMessage, "success", 3000);
                    spin = false;
                    try
                    {
                        PengalamanResoruceId = await UserService.ListPengalaman(token);
                        foreach (var item in PengalamanResoruceId)
                        {
                            TanggalAwalKerja = item.TglAwal.ToString("yyyy-MM-dd");
                            TanggalAkhirKerja = item.TglAkhir.ToString("yyyy-MM-dd");
                        }
                    }
                    catch (Exception ex)
                    {
                        await JSRuntime.InvokeVoidAsync("notifDev", ex.Message, "error", 3000);
                    }
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
                        var MessageRespon = ex.Message;
                        await JSRuntime.InvokeVoidAsync("notifDev", MessageRespon, "error", 3000);
                    }
                }
               
            }
            else
            {
                spin = false;
                messagePengalaman = "Form Invalid";
                await JSRuntime.InvokeVoidAsync("notifDev", messagePengalaman, "error", 3000);
            }
        }
        protected void kirim()
        {
            NavigationManager.NavigateTo("/endPage");
        }
    }
}
