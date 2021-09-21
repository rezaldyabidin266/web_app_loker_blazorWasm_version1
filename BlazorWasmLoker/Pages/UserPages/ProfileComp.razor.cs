using BlazorWasmLoker.Resoruces.Users;
using BlazorWasmLoker.Services;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections;
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
        protected ArrayList pengalamanId = new ArrayList();

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
                await JSRuntime.InvokeVoidAsync("notifDev", MessageRespon, "error", 3000);
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
            await JSRuntime.InvokeVoidAsync("notifDev", "Sukses Change Profile", "success", 3000);
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

            await InvokeAsync(StateHasChanged);
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
                await JSRuntime.InvokeVoidAsync("notifDev", MessageRespon, "error", 3000);
        
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
                    MessageRespon = result;
                    await JSRuntime.InvokeVoidAsync("notifDev", MessageRespon, "success", 3000);
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
                        await JSRuntime.InvokeVoidAsync("notifDev", ex.Message, "error", 3000);
                    }
                }
                catch (Exception ex)
                {
                    spinSave = false;
                    await JSRuntime.InvokeVoidAsync("notifDev", ex.Message, "error", 3000);
                 
                }
            }
            else
            {
                spinSave = false;
                MessageRespon = "Form Invalid";
                await JSRuntime.InvokeVoidAsync("notifDev", MessageRespon, "error", 3000);
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
        protected async void UploadSuksesCv(FileUploadEventArgs e)
        {
            spinCv = false;
            await JSRuntime.InvokeVoidAsync("notifDev", "Berhasil Upload", "success", 3000);
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
                await JSRuntime.InvokeVoidAsync("notifDev", MessageRespon, "error", 3000);
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
                await JSRuntime.InvokeVoidAsync("notifDev", MessageRespon, "error", 3000);
                UserService.JsConsoleLog(ex.Message);
            }
        }

        protected async Task DeletePengalaman(int id)
        {
            spinDeletePengalaman = true;

            pengalamanId.Add(id);
            await LocalStorage.SetItemAsync<ArrayList>("pengalamanIdArray", pengalamanId);
 
            try
            {
                var result = await UserService.DeletePengalaman(token, id);
                UserService.JsConsoleLog(result);
                MessageRespon = result;
                await JSRuntime.InvokeVoidAsync("notifDev", MessageRespon, "success", 3000);
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
                await JSRuntime.InvokeVoidAsync("notifDev", MessageRespon, "error", 3000);
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
                await JSRuntime.InvokeVoidAsync("notifDev", ex.Message, "error", 3000);
                UserService.JsConsoleLog(ex.Message);
            }
        }

        protected async Task updatePengalamanSubmit(int id)
        {
            pengalamanId.Add(id);

            await LocalStorage.SetItemAsync<ArrayList>("pengalamanIdArray", pengalamanId);

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
                    await JSRuntime.InvokeVoidAsync("notifDev", MessageRespon, "success", 3000);
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
                    await JSRuntime.InvokeVoidAsync("notifDev", MessageRespon, "error", 3000);
                }
            }
            else
            {
                spinUpdatePengalaman = false;
                UserService.JsConsoleLog("FORM INVALID");
                MessageRespon = "Form Invalid";
                await JSRuntime.InvokeVoidAsync("notifDev", MessageRespon, "error", 3000);
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
                    await JSRuntime.InvokeVoidAsync("notifDev", MessageRespon, "success", 3000);
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
                    await JSRuntime.InvokeVoidAsync("notifDev", MessageRespon, "error", 3000);
                }
            }
            else
            {
                spinAddPengalaman = false;
                UserService.JsConsoleLog("FORM INVALID");
                MessageRespon = "Form Invalid";
                await JSRuntime.InvokeVoidAsync("notifDev", MessageRespon, "error", 3000);
            }
        }
    }
}
