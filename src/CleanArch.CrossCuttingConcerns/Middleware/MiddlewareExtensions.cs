//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CleanArch.CrossCuttingConcerns.Middleware
//{
//    public class MiddlewareExtensions
//    {
//        private readonly RequestDelegate _next;

//        public ExceptionHandlerMiddleware(RequestDelegate next)
//        {
//            _next = next;
//        }

//        public async Task Invoke(HttpContext context)
//        {
//            try
//            {
//                await _next(context);
//            }
//            catch (Exception ex)
//            {
//                await ConvertException(context, ex);
//            }
//        }

//        private Task ConvertException(HttpContext context, Exception exception)
//        {
//            HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;

//            context.Response.ContentType = "application/json";

//            var result = string.Empty;

//            switch (exception)
//            {
//                case ValidationException validationException:
//                    httpStatusCode = HttpStatusCode.BadRequest;
//                    result = JsonConvert.SerializeObject(validationException.ValdationErrors);
//                    break;
//                case BadRequestException badRequestException:
//                    httpStatusCode = HttpStatusCode.BadRequest;
//                    result = badRequestException.Message;
//                    break;
//                case NotFoundException notFoundException:
//                    httpStatusCode = HttpStatusCode.NotFound;
//                    break;
//                case Exception ex:
//                    httpStatusCode = HttpStatusCode.BadRequest;
//                    break;
//            }

//            context.Response.StatusCode = (int)httpStatusCode;

//            if (result == string.Empty)
//            {
//                result = JsonConvert.SerializeObject(new { error = exception.Message });
//            }

//            return context.Response.WriteAsync(result);
//        }
//    }
//}
