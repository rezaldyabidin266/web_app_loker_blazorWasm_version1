using System;

namespace BlazorWasmLoker.Resoruces.Lokers
{
    public class JawabanResoruce
    {
        public int Id { get; set; }
        public string Pertanyaan { get; set; }
        public string Jawaban { get; set; }
        public string JawabanTambahan { get; set; }
        public decimal Nominal { get; set; }
        public DateTime? Tanggal { get; set; }
        public string FilePendukung { get; set; }
    }

}
