using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using InkWell.Auth.DTOs;

namespace InkWell.Auth.Middleware
{
    public class GlobalExceptionHandler : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            // Decide status code based on exception type
            int status = GetStatusCode(context.Exception);
            string error = GetErrorMessage(status);

            // Build the error response
            ErrorResponseDTO response = new ErrorResponseDTO
            {
                Timestamp = DateTime.UtcNow.ToString("O"),
                Status = status,
                Error = error,
                Message = context.Exception.Message,
                Path = context.HttpContext.Request.Path.ToString()
            };

            // Return the response with correct status code
            context.Result = new ObjectResult(response)
            {
                StatusCode = status
            };

            // Tell ASP.NET Core the exception has been handled
            context.ExceptionHandled = true;
        }

        // Returns 400 for known exceptions, 500 for everything else
        private int GetStatusCode(Exception exception)
        {
            if (exception is ArgumentException)
            {
                return 400;
            }

            if (exception is UnauthorizedAccessException)
            {
                return 401;
            }

            if (exception is InvalidOperationException)
            {
                return 409;
            }

            return 500;
        }

        // Returns error text based on status code
        private string GetErrorMessage(int status)
        {
            if (status == 400)
            {
                return "Bad Request";
            }
            if (status == 401)
            {
                return "Unauthorized";
            }

            if (status == 409)
            {
                return "Conflict";
            }

            return "Internal Server Error";
        }
    }
}
