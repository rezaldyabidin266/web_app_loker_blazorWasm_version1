using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorWasmLoker.Utility
{
    public static class MyHelper
    {
        public enum BentukIsian
        {
            SimpleText,
            YesNo,
            Paragraf,
            PilihanGanda,
            Checkbox,
            DropDown,
            UploadFile,
            Tanggal,
            Nominal,
            PrivateOnSite
        }

        public static string InfoBentukIsian(BentukIsian value)
        {
            switch (value)
            {

                case BentukIsian.SimpleText:
                    return "Simple Text";
                case BentukIsian.YesNo:
                    return "Yes No Question";
                case BentukIsian.Paragraf:
                    return "Paragraf";
                case BentukIsian.PilihanGanda:
                    return "Pilihan Ganda";
                case BentukIsian.Checkbox:
                    return "Checkbox";
                case BentukIsian.DropDown:
                    return "DropDown";
                case BentukIsian.UploadFile:
                    return "Upload File";
                case BentukIsian.Tanggal:
                    return "Tanggal";
                case BentukIsian.Nominal:
                    return "Nominal";
                case BentukIsian.PrivateOnSite:
                    return "Private On Site";
                default:
                    throw new ArgumentException(value.ToString());
            }
        } 
    }

}
