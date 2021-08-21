using System.Collections.Generic;

namespace BlazorWasmLoker.Resoruces.Lokers
{
    public class FromPertanyaanResoruce
    {
        public int Id { get; set; }
        public int No { get; set; }
        public string Pertanyaan { get; set; }
        public string BentukIsian { get; set; }
        public string Jawaban { get; set; }
        public bool IsRequired { get; set; }
        public List<string> Pilihan { get; set; }
    }

}
