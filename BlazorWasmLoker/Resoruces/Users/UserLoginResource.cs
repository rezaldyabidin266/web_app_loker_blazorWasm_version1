using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWasmLoker.Resoruces.Users
{
    public class UserLoginResource
    {
        [Required(ErrorMessage = "Harap isi Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Harap isi Password")]
        public string Password { get; set; }
        public string IpAddress { get; set; }
        public string Browser { get; set; }
    }


}
