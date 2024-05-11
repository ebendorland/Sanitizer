using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanitizer.Core.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; }

        public ApiException(int statusCode, string message, Exception? innerException) : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
