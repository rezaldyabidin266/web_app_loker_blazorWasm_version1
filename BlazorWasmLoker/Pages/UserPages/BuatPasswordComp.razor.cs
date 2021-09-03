using BlazorWasmLoker.Resoruces.Users;
using BlazorWasmLoker.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWasmLoker.Pages.UserPages
{
    public class BuatPasswordCompBase : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        protected UserService userService { get; set; }
        [Inject]
        protected Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        public PasswordBaruResoruce PasswordBaruResoruce = new PasswordBaruResoruce();
        public EditContext editContext { get; set; }

        protected bool spin = false;
        protected string message;

        protected override void OnInitialized()
        {
            editContext = new EditContext(PasswordBaruResoruce);
        }

        protected async Task save()
        {
            spin = true;
            if (editContext.Validate())
            {
                try
                {
                    spin = false;
                    var result = await userService.BuatPasswrodBaru(PasswordBaruResoruce);
                    await JSRuntime.InvokeVoidAsync("notifDev", result, "success", 3000);
                    NavigationManager.NavigateTo("/login");
                }
                catch (Exception ex)
                {
                    await JSRuntime.InvokeVoidAsync("notifDev", ex.Message, "error", 3000);
                    spin = false;
                }
            }
            else
            {
                spin = false;
                await JSRuntime.InvokeVoidAsync("notifDev", "Form Invalid", "error", 3000);
            }
        }
    }
}
