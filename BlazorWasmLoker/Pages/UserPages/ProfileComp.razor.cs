﻿using BlazorWasmLoker.Resoruces.Users;
using BlazorWasmLoker.Services;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWasmLoker.Pages.UserPages
{
    public class ProfileCompBase : ComponentBase
    {

        [Inject]
        protected UserService UserService { get; set; }
        [Inject]
        protected LokerService LokerService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        protected Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        protected string token;
        protected string fotoPelamar;
        protected string ErrorBg;
        protected InformasiPelamarRespond responseInfoPelamar = new InformasiPelamarRespond();
        protected List<PengalamanResourceId> PengalamanResoruceId = new List<PengalamanResourceId>();
        protected string TanggalLahirPelamar;
        protected string TanggalAwalKerja;
        protected string TanggalAkhirKerja;
        protected string PengalamanGaji;
        protected int PengalamanId;
        protected string urlFotoUpload;
        protected string urlCvUpload;
        protected string messageUpload;
        protected string MessageRespon;
        protected bool editProfile = true;
        protected bool UploadVisible { get; set; } = false;
        public UpdatePelamarResoruce UpdatePelamarResoruce = new UpdatePelamarResoruce();
        public PengalamanResourceId PengalamanResourceUpdate = new PengalamanResourceId();
        public PengalamanResoruce PengalamanResourdeAdd = new PengalamanResoruce() { TglAwal = DateTime.Now, TglAkhir = DateTime.Now };
        public EditContext editContext { get; set; }
        public EditContext pengalamanUpdateContext { get; set; }
        public EditContext pengalamanAddContext { get; set; }
        protected char maskChar = ' ';

        //spin
        protected bool spinSave = false;
        protected bool spinDeletePengalaman = false;
        protected bool spinCv = false;
        protected bool spinUpdatePengalaman = false;
        protected bool spinAddPengalaman = false;
        protected bool dowloadGambar = false;
        protected bool toastTimer = false;

        protected override void OnInitialized()
        {
            urlFotoUpload = UrlApi();
            urlCvUpload = UrlApiCv();
            editContext = new EditContext(UpdatePelamarResoruce);
            pengalamanUpdateContext = new EditContext(PengalamanResourceUpdate);
            pengalamanAddContext = new EditContext(PengalamanResourdeAdd);
        }

        protected override async Task OnInitializedAsync()
        {
            token = await LocalStorage.GetItemAsync<string>("token");
            await FotoPelamar();
            await InformasiPelamar();
            await ListPengalaman();
        }
        protected override async void OnAfterRender(bool firstRender)
        {
            await JSRuntime.InvokeVoidAsync("toastShow");
        }
        protected async Task FotoPelamar()
        {
            try
            {
                byte[] fotoByte = await UserService.GetFoto(token);
                var foto = Convert.ToBase64String(fotoByte);
                fotoPelamar = "data:image/png;base64," + foto;
            }
            catch (Exception ex)
            {
                fotoPelamar = "asset/image/avatar.jpg";
                MessageRespon = ex.Message;
            }
         
        }

        protected string GetUploadUrl(string url)
        {
            return NavigationManager.ToAbsoluteUri(url).AbsoluteUri;
        }

        protected string UrlApi()
        {
            return UserService.UploadFoto();
        }
        protected void SelectedFilesChanged(IEnumerable<UploadFileInfo> files)
        {
            UploadVisible = files.ToList().Count > 0;
            InvokeAsync(StateHasChanged);
        }
        protected void OnFileUploadStart(FileUploadStartEventArgs args)
        {
            args.RequestHeaders.Add("token", token);
        }
        protected async void UploadSukses(FileUploadEventArgs e)
        {
            dowloadGambar = true;
            UserService.JsConsoleLog("Sukses Change Profile");
            MessageRespon = "Sukses Change Profile";
            try
            {
                dowloadGambar = false;
                byte[] fotoByte = await UserService.GetFoto(token);
                var foto = Convert.ToBase64String(fotoByte);
                fotoPelamar = "data:image/png;base64," + foto;
            }
            catch (Exception ex)
            {
                dowloadGambar = false;
                fotoPelamar = "asset/image/avatar.jpg";
                MessageRespon = ex.Message;
            }
            //finally
            //{
            //    dowloadGambar = false;
            //    await InvokeAsync(StateHasChanged);
            //}
            //await toastSetting();
            await InvokeAsync(StateHasChanged);

            //await Task.Delay(3000);
            //toastTimer = false;
            //await InvokeAsync(StateHasChanged);
            

        }
        protected async Task InformasiPelamar()
        {
            try
            {
                responseInfoPelamar = await UserService.InformasiPelamar(token);
                TanggalLahirPelamar = responseInfoPelamar.TglLahir.ToString("yyyy-MM-dd");
                UpdatePelamarResoruce = new UpdatePelamarResoruce()
                {
                    Nama = responseInfoPelamar.Nama,
                    NoTlp = responseInfoPelamar.NoTlp,
                    Alamat = responseInfoPelamar.Alamat,
                    TempatLahir = responseInfoPelamar.TmptLahir,
                    TglLahir = responseInfoPelamar.TglLahir
                  
                };
            }
            catch (Exception ex)
            {
                MessageRespon = ex.Message;
                UserService.JsConsoleLog(ex.Message);
            }

        }
        protected void editPelamar()
        {
            editProfile = false;
        }

        protected void editCancel()
        {

            editProfile = true;
        }

        protected async Task editProfileSubmit()
        {
            spinSave = true;
            if (editContext.Validate())
            {
                try
                {
                    var result = await UserService.UpdateDataPelamar(token,UpdatePelamarResoruce);
                    LokerService.JsConsoleLog(result);
                    MessageRespon = result;
                    editProfile = true;
                    spinSave = false;
                    try
                    {
                        responseInfoPelamar = await UserService.InformasiPelamar(token);
                        TanggalLahirPelamar = responseInfoPelamar.TglLahir.ToString("yyyy-MM-dd");
                        UpdatePelamarResoruce = new UpdatePelamarResoruce()
                        {
                            Nama = responseInfoPelamar.Nama,
                            NoTlp = responseInfoPelamar.NoTlp,
                            Alamat = responseInfoPelamar.Alamat,
                            TempatLahir = responseInfoPelamar.TmptLahir,
                            TglLahir = responseInfoPelamar.TglLahir
                        };
                    }
                    catch (Exception ex)
                    {
                        UserService.JsConsoleLog(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    spinSave = false;
                    LokerService.JsConsoleLog(ex.Message);
                    MessageRespon = ex.Message;
                }
            }
            else
            {
                spinSave = false;
                LokerService.JsConsoleLog("Form Invalid");
                MessageRespon = "Form Invalid";
            }
        }

        protected void SelectedFilesChangedCv(IEnumerable<UploadFileInfo> files)
        {
            UploadVisible = files.ToList().Count > 0;
            InvokeAsync(StateHasChanged);
        }
        protected string UrlApiCv()
        {
            return UserService.UploadCv();
        }
        protected void OnFileUploadStartCv(FileUploadStartEventArgs args)
        {
            args.RequestHeaders.Add("token", token);
            spinCv = true;
        }
        protected void UploadSuksesCv(FileUploadEventArgs e)
        {
            spinCv = false;
            MessageRespon = "Berhasil Upload";
        
        }

        protected async Task ShowPdf()
        {
            try
            {
                byte[] CvByte = await UserService.GetCv(token);
                await JSRuntime.InvokeVoidAsync("openCv",CvByte);
            }
            catch (Exception ex)
            {
                MessageRespon = ex.Message;
                UserService.JsConsoleLog(ex.Message);
            }
        }
        protected async Task ListPengalaman()
        {
            try
            {
                PengalamanResoruceId = await UserService.ListPengalaman(token);
            }
            catch (Exception ex)
            {
                MessageRespon = ex.Message;
                UserService.JsConsoleLog(ex.Message);
            }
        }

        protected async Task DeletePengalaman(int id)
        {
            spinDeletePengalaman = true;
            try
            {
                var result = await UserService.DeletePengalaman(token, id);
                UserService.JsConsoleLog(result);
                MessageRespon = result;
                spinDeletePengalaman = false;
                try
                {
                    PengalamanResoruceId = await UserService.ListPengalaman(token);
                }
                catch (Exception ex)
                {
                    UserService.JsConsoleLog(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageRespon = ex.Message;
                UserService.JsConsoleLog(ex.Message);
                spinDeletePengalaman = false;
            }
        }


        protected async Task ModalPengalamanUpdate(int id)
        {
            try
            {
                PengalamanResoruceId = await UserService.ListPengalaman(token);
                foreach (var item in PengalamanResoruceId)
                {
                    if (item.Id == id)
                    {
                        PengalamanId = item.Id;
                        PengalamanResourceUpdate = new PengalamanResourceId()
                        {
                            TempatKerja = item.TempatKerja,
                            Posisi = item.Posisi,
                            Keterangan = item.Keterangan,
                            Nominal = item.Nominal,
                            TglAwal = item.TglAwal,
                            TglAkhir = item.TglAkhir,
                            masaKerja = item.masaKerja
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                UserService.JsConsoleLog(ex.Message);
            }
        }

        protected async Task updatePengalamanSubmit(int id)
        {
            spinUpdatePengalaman = true;
            if (pengalamanUpdateContext.Validate())
            {
                try
                {
                    var idString = id.ToString();
                    var result = await UserService.UpdatePengalaman(token, idString, PengalamanResourceUpdate);
                    UserService.JsConsoleLog(result);
                    MessageRespon = result;
                    spinUpdatePengalaman = false;
                    try
                    {
                        PengalamanResoruceId = await UserService.ListPengalaman(token);
                     
                    }
                    catch (Exception ex)
                    {
                        UserService.JsConsoleLog(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    MessageRespon = ex.Message;
                    spinUpdatePengalaman = false;
                    UserService.JsConsoleLog(ex.Message);
                }
            }
            else
            {
                spinUpdatePengalaman = false;
                UserService.JsConsoleLog("FORM INVALID");
                MessageRespon = "Form Invalid";
            }
        }

        protected async Task AddPengalaman()
        {
            spinAddPengalaman = true;
            if (pengalamanAddContext.Validate())
            {
                try
                {
                    var result = await UserService.AddPengalaman(token, PengalamanResourdeAdd);
                    UserService.JsConsoleLog(result);
                    MessageRespon = result;
                    spinAddPengalaman = false;
                    try
                    {
                        PengalamanResoruceId = await UserService.ListPengalaman(token);
                       
                    }
                    catch (Exception ex)
                    {
                        UserService.JsConsoleLog(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    MessageRespon = ex.Message;
                    UserService.JsConsoleLog(ex.Message);
                    spinAddPengalaman = false;
                }
            }
            else
            {
                spinAddPengalaman = false;
                UserService.JsConsoleLog("FORM INVALID");
                MessageRespon = "Form Invalid";
            }
        }
        protected async Task toastSetting()
        {
            toastTimer = true;
            await Task.Delay(3000);
            toastTimer = false;
            await InvokeAsync(StateHasChanged);
        }
    }
}
