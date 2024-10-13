using CleanArch.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace CleanArch.Application.Middleware;

public class ExceptionHandlerMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlerMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
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


        logger.LogError(result);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = httpStatusCode;

        if (result == string.Empty)
        {
            result = JsonConvert.SerializeObject(new { StatusCode = httpStatusCode, error = exception.Message });
        }

        return context.Response.WriteAsync(result);
    }
}