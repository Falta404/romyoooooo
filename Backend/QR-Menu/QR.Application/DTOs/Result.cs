using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QR.Application.DTOs
{
    public class Result<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public Result(T data, string messege, HttpStatusCode statusCode)
        {
            Data = data;
            Message = messege;
            StatusCode = statusCode;
        }
        public Result(string messege, HttpStatusCode statusCode)
        {
            Message = messege;
            StatusCode = statusCode;
        }
        public Result()
        {

        }
    }
}
