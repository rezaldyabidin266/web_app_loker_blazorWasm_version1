using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorWasmLoker.Resoruces.Users
{
    public class PengalamanResoruce
    {
        public string TempatKerja { get; set; }
        public string Posisi { get; set; }
        public string Keterangan { get; set; }
        public double Nominal { get; set; }
        [Required(ErrorMessage = "Harap isi Tanggal Awal")]
        public DateTime TglAwal { get; set; }
        public DateTime TglAkhir { get; set; }
    }

}
