using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QR.Application.Helpers
{
    public class ColorHelper
    {
        public static string GetColorCodesAsString(List<string> colorCodes)
        {
            return string.Join("-", colorCodes);
        }

        public static List<string> GetColorCodesAsList(string colorCodes)
        {
            return colorCodes.Split("-").ToList();
        }
    }
}
