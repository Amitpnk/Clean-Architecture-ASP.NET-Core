using CleanArch.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace CleanArch.Application.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        public ExceptionHandlerMiddleware(RequestDelegate next,
            ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await ConvertException(context, ex);
            }
        }

        private Task ConvertException(HttpContext context, Exception exception)
        {
            int httpStatusCode;
            var result = exception.Message;

            switch (exception)
            {
                case ValidationException validationException:
                    httpStatusCode = (int)HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(validationException.ValdationErrors);
                    break;
                case BadRequestException badRequestException:
                    httpStatusCode = (int)HttpStatusCode.BadRequest;
                    result = badRequestException.Message;
                    break;
                case NotFoundException:
                    httpStatusCode = (int)HttpStatusCode.NotFound;
                    break;
                case ApiException apiException:
                    httpStatusCode = (int)HttpStatusCode.InternalServerError;
                    result = apiException.Message;
                    break;
                default:
                    httpStatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }


            _logger.LogError(result);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = httpStatusCode;

            if (result == string.Empty)
            {
                result = JsonConvert.SerializeObject(new { StatusCode = httpStatusCode, error = exception.Message });
            }

            return context.Response.WriteAsync(result);
        }
    }
}
