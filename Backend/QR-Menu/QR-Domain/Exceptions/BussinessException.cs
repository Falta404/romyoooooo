using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QR_Domain.Exceptions
{
  public class BusinessException : Exception
  {
    public BusinessException(int statusCode, string message) : base(message)
    {
      StatusCode = statusCode;
    }
    public Guid Id { get; set; } = Guid.NewGuid();
    public int StatusCode { get; set; }
  }
}
