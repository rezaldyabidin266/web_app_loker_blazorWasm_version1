using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorWasmLoker.Resoruces.Users
{
    public class DaftarResource
    {
        [Required(ErrorMessage = "Harap isi Nama")]
        public string Nama { get; set; }

        [Required(ErrorMessage = "Harap isi Alamat")]
        public string Alamat { get; set; }

        [Required(ErrorMessage = "Harap isi Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Harap isi NoTlp")]
        public string NoTlp { get; set; }

        [Required(ErrorMessage = "Harap isi TempatLahir")]
        public string TempatLahir { get; set; }

        [Required(ErrorMessage = "Harap isi TglLahir")]
        public DateTime TglLahir { get; set; } 

        [Required(ErrorMessage = "Harap isi Password")]
        public string Password { get; set; }
        public string Note { get; set; }
    }

}
