using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;

namespace canary.Filter
{
    public class ExceptionHandlingFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            string msg = exception.Message;
            if (msg == "Exception has been thrown by the target of an invocation.")
            {
                if (exception.InnerException.Message.Length > 38 && exception.InnerException.Message.Substring(0,38)== "Could not parse given string, expected")
                {
                    msg = exception.InnerException.Message;  // Replace generic exception message with message intended for UI which was loaded into InnerException
                }
            }

            var errorResponse = new
            {
                ErrorDetails = msg // Was: exception.Message
            };

            context.Result = new JsonResult(errorResponse)
            {
                StatusCode = 500
            };

            context.ExceptionHandled = true;
        }
    }
}
