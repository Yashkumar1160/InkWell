using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using InkWell.Notification.DTOs;

namespace InkWell.Notification.Middleware
{
    public class GlobalExceptionHandler : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            int status = GetStatusCode(context.Exception);
            string error = GetErrorMessage(status);

            ErrorResponseDTO response = new ErrorResponseDTO
            {
                Timestamp = DateTime.UtcNow.ToString("O"),
                Status = status,
                Error = error,
                Message = context.Exception.Message,
                Path = context.HttpContext.Request.Path.ToString()
            };

            context.Result = new ObjectResult(response)
            {
                StatusCode = status
            };

            context.ExceptionHandled = true;
        }

        private int GetStatusCode(Exception exception)
        {
            if (exception is ArgumentException) return 400;
            if (exception is UnauthorizedAccessException) return 401;
            if (exception is InvalidOperationException) return 409;
            return 500;
        }

        private string GetErrorMessage(int status)
        {
            if (status == 400) return "Bad Request";
            if (status == 401) return "Unauthorized";
            if (status == 409) return "Conflict";
            return "Internal Server Error";
        }
    }
}
