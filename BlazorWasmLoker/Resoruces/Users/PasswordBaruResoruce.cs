using System.ComponentModel.DataAnnotations;

namespace BlazorWasmLoker.Resoruces.Users
{
    public class PasswordBaruResoruce
    {
        [Required(ErrorMessage = "Harap isi Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Harap isi Password")]
        public string Password { get; set; }
    }


}
