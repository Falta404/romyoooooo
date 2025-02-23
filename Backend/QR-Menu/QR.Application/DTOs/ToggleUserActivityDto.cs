using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QR.Application.DTOs
{
    public class ToggleUserActivityDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool IsActivity { get; set; }
    }
}
