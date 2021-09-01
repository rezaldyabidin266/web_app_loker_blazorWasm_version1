using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWasmLoker.Resoruces.Users
{
    public class PengalamanResourceUpdate
    {
        public int Id { get; set; }
        public string TempatKerja { get; set; }
        public string Posisi { get; set; }
        public string Keterangan { get; set; }
        public double Nominal { get; set; }
        public DateTime TglAwal { get; set; }
        public DateTime TglAkhir { get; set; }
        public string masaKerja { get; set; }

    }
}
