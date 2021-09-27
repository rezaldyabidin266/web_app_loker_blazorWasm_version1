using BlazorWasmLoker.Services;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace BlazorWasmLoker.Pages.LokerPages
{
    public class BerkasPelamarCompBase : ComponentBase
    {
        [Inject]
        protected UserService userService { get; set; }
        [Inject]
        protected Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        protected bool UploadVisible { get; set; } = false;
        protected string urlApi;
        protected string urlApiCv;
        protected string token;
        protected string messageUpload;
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
        }
        protected override void OnInitialized()
        {
            urlApi = UrlApi();
            urlApiCv = UrlApiCv();
        }

        protected void SelectedFilesChanged(IEnumerable<UploadFileInfo> files)
        {
            UploadVisible = files.ToList().Count > 0;
            userService.JsConsoleLog("file Select");
            InvokeAsync(StateHasChanged);
        }

        protected void SelectedFilesChangedCv(IEnumerable<UploadFileInfo> files)
        {
            UploadVisible = files.ToList().Count > 0;
            InvokeAsync(StateHasChanged);
        }

        protected string GetUploadUrl(string url)
        {
            return NavigationManager.ToAbsoluteUri(url).AbsoluteUri;
        }

        protected string UrlApi()
        {
            return userService.UploadFoto();
        }

        protected string UrlApiCv()
        {
            return userService.UploadCv();
        }

        protected async void OnFileUploadStart(FileUploadStartEventArgs args)
        {
            args.RequestHeaders.Add("token", token);
            userService.JsConsoleLog("file Upload Start");

            if (userNetwork != "Online")
            {
                await JSRuntime.InvokeVoidAsync("notifDev", "Berhasil request data offline, diharapkan segera online", "warning", 5000);
            }
        }

        protected void OnFileError(FileUploadErrorEventArgs e)
        {
            userService.JsConsoleLog("file Upload Error");
        }

        protected async void OnFileUploadStartCv(FileUploadStartEventArgs args)
        {
            args.RequestHeaders.Add("token", token);
            if (userNetwork != "Online")
            {
                await JSRuntime.InvokeVoidAsync("notifDev", "Berhasil request data offline, diharapkan segera online", "warning", 5000);
            }
        }
        protected async void UploadSukses(FileUploadEventArgs e)
        {
            messageUpload = "Berhasil di upload";
            await JSRuntime.InvokeVoidAsync("notifDev", messageUpload, "success", 3000);
        }

        protected void goPengalaman()
        {
            spin = true;
            NavigationManager.NavigateTo("/pengalaman");
        }
    }
}
