using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWasmLoker
{
    public class Helper
    {
        public string ConvertImageToDisplay(byte[] image)
        {
            if (image != null)
            {
                var base64 = Convert.ToBase64String(image);
                var finalStr = string.Format($"data:image/jpg;base64,{base64}");
                return finalStr;
            }
            return string.Empty;
        }

    }
}
