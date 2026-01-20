using Microsoft.Extensions.Logging;
using System.Net;

namespace FiapCloudGamesPayments.Domain.Exceptions
{
    public class BaseException : Exception
    {
        public virtual HttpStatusCode StatusCode { get; set; } = HttpStatusCode.InternalServerError;
        public virtual LogLevel LogLevel { get; set; } = LogLevel.Error;


        public BaseException() : base("Sorry! We ran into a problem. Please try again soon.")
        {
        }

        public BaseException(string? message) : base(message)
        {
        }
    }
}
