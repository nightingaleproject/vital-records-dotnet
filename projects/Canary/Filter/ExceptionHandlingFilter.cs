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

            string msg;
            if (exception.Message == "Exception has been thrown by the target of an invocation.")
            {
                msg = exception.InnerException.Message;
            }
            else
            {
                msg = exception.Message;
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
