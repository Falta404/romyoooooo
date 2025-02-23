using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QR_Domain.Enums;

namespace QR.Application.Helpers
{
    public static class URLHelper
    {
        public static string StaticFileRequestPath { get; set; }
        public static string GetStaticFilePath(string url) =>
            $"/{StaticFileRequestPath}/{url}";
    }
}
