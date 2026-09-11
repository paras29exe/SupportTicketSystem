using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SupportTicketSystem.Core.Exceptions;
using SupportTicketSystem.Core.Models;
using System.Data.Common;

namespace SupportTicketSystem.Api.ExceptionHandler
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> _logger) : IExceptionHandler
    {
        private readonly ILogger logger = _logger;

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken
            )
        {
            ApiError apiError;

            if(exception is AppException appException)
            {
                logger.LogWarning($"Custom Exception thrown at {httpContext.Request.Method} - {httpContext.Request.Path} \n\n\tDetails: {appException.loggingDetails}");

                httpContext.Response.StatusCode = appException.status;

                apiError = new()
                {
                    status = appException.status,
                    message = appException.message,
                    details = appException.errorInfo
                };
            }else if(exception is DbUpdateException e){
                logger.LogError($"Database updation failed at {httpContext.Request.Method}  - {httpContext.Request.Path}\n\n\t Message: {e.InnerException?.Message ?? e.Message} \n\n\tError Info : {e.InnerException}");

                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

                apiError = new()
                {
                    status = 500,
                    message = "Database update failed due to some unexpected reason",
                    details = e.InnerException?.Message ?? e.Message ?? string.Empty
                };
            }
            else
            {
                logger.LogError($"Unhandled exception occurred at {httpContext.Request.Method}  - {httpContext.Request.Path}\n \n\tDetails: {exception}");

                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

                apiError = new()
                {
                    status = 500,
                    message = "An unexpected error occured while processing the request",
                };
            }

            await httpContext.Response.WriteAsJsonAsync(apiError, cancellationToken);
            return true;
        }
    }
}