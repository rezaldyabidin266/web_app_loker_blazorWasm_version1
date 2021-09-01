using BlazorWasmLoker.Resoruces.Users;
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
        protected bool editProfile = true;
        protected bool UploadVisible { get; set; } = false;
        public UpdatePelamarResoruce UpdatePelamarResoruce = new UpdatePelamarResoruce();
        public PengalamanResourceId PengalamanResourceUpdate = new PengalamanResourceId();
        public EditContext editContext { get; set; }
        public EditContext pengalamanUpdateContext { get; set; }
        protected char maskChar = ' ';

        //spin
        protected bool spinSave = false;
     
        protected override void OnInitialized()
        {
            urlFotoUpload = UrlApi();
            urlCvUpload = UrlApiCv();
            editContext = new EditContext(UpdatePelamarResoruce);
            pengalamanUpdateContext = new EditContext(PengalamanResourceUpdate);
        }

        protected override async Task OnInitializedAsync()
        {
            token = await LocalStorage.GetItemAsync<string>("token");
            await FotoPelamar();
            await InformasiPelamar();
            await ListPengalaman();
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
                ErrorBg = ex.Message;
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
            UserService.JsConsoleLog("Sukses Change Profile");
            await FotoPelamar().ConfigureAwait(false);
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
                //UserService.GotoLogin();
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

                }
            }
            else
            {
                spinSave = false;
                LokerService.JsConsoleLog("Form Invalid");
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
        }
        protected void UploadSuksesCv(FileUploadEventArgs e)
        {
            messageUpload = "Berhasil di upload";
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
                UserService.JsConsoleLog(ex.Message);
            }
        }

        protected async Task ListPengalaman()
        {
            try
            {
                PengalamanResoruceId = await UserService.ListPengalaman(token);
                foreach (var item in PengalamanResoruceId)
                {
                    TanggalAwalKerja = item.TglAwal.ToString("yyyy-MM-dd");
                    TanggalAkhirKerja = item.TglAkhir.ToString("yyyy-MM-dd");
                    double nominal = item.Nominal;
                    //PengalamanGaji = String.Format(CultureInfo.CreateSpecificCulture("id-id"), "Rp. {0:0}", nominal);
                    PengalamanGaji = $"Rp {nominal:n0}";
                }
            }
            catch (Exception ex)
            {
                UserService.JsConsoleLog(ex.Message);
            }
        }

        protected async Task DeletePengalaman(int id)
        {
            try
            {
                var result = await UserService.DeletePengalaman(token, id);
                UserService.JsConsoleLog(result);
            }
            catch (Exception ex)
            {
                UserService.JsConsoleLog(ex.Message);
               
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
            if (pengalamanUpdateContext.Validate())
            {
                try
                {
                    var idString = id.ToString();
                    var result = await UserService.UpdatePengalaman(token, idString, PengalamanResourceUpdate);
                    UserService.JsConsoleLog(result);
                    try
                    {
                        PengalamanResoruceId = await UserService.ListPengalaman(token);
                        foreach (var item in PengalamanResoruceId)
                        {
                            TanggalAwalKerja = item.TglAwal.ToString("yyyy-MM-dd");
                            TanggalAkhirKerja = item.TglAkhir.ToString("yyyy-MM-dd");
                            double nominal = item.Nominal;
                            PengalamanGaji = $"Rp {nominal:n0}";

                        }
                    }
                    catch (Exception ex)
                    {
                        UserService.JsConsoleLog(ex.Message);
                    }
                }
                catch (Exception ex)
                {

                    UserService.JsConsoleLog(ex.Message);
                }
            }
            else
            {

            }
        }

    }
}
