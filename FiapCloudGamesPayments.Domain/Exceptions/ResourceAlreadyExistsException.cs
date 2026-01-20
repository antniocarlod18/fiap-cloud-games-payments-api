using Microsoft.Extensions.Logging;
using System.Net;

namespace FiapCloudGamesPayments.Domain.Exceptions;

[Serializable]
public class ResourceAlreadyExistsException : BaseException
{
    private static string _customMessage = "Oops! {0} already exists.";
    public override HttpStatusCode StatusCode { get; set; } = HttpStatusCode.Conflict;
    public override LogLevel LogLevel { get; set; } = LogLevel.Warning;

    public ResourceAlreadyExistsException(string resourceName) : base(string.Format(_customMessage, resourceName))
    {
    }
}