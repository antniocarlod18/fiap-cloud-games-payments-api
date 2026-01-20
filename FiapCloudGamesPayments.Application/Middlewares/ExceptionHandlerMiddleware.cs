using FiapCloudGamesPayments.Application.Dtos;
using FiapCloudGamesPayments.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace FiapCloudGamesPayments.Application.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                _logger.LogInformation("Handling request: " + context.Request.Path);

                await _next(context);

                _logger.LogInformation("Finished handling request. " + context.Request.Path);
            }
            catch(BaseException ex)
            {
                _logger.Log(ex.LogLevel, ex.Message);

                await HandleCustomExceptionResponseAsync(context, (int)ex.StatusCode,
                    ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                await HandleCustomExceptionResponseAsync(context, (int)HttpStatusCode.InternalServerError,
                    "Sorry! We ran into a problem. Please try again soon.");
            }
        }

        private async Task HandleCustomExceptionResponseAsync(HttpContext context, int statusCode, string? message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new ErrorDto()
            {
                StatusCode = context.Response.StatusCode,
                Message = message
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
