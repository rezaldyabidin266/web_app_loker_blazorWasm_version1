using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWasmLoker.Resoruces.Settings
{
    public class CounterResoruce
    {
        public string Page { get; set; }
        public string Browser { get; set; }
        public string platform { get; set; }
        public string Location { get; set; }
        public string Isp { get; set; }
        public string ComputerScreen { get; set; }
        public string IpAddress { get; set; }
        public bool IsDoNotTrack { get; set; }
        public string Referred { get; set; }
        public bool IsTor { get; set; }

    }
}
