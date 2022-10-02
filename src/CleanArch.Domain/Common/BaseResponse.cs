using System.Collections.Generic;

namespace CleanArch.Domain.Common
{
    public class BaseResponse<T>
    {
        public BaseResponse()
        {
            Success = true;
        }
        public BaseResponse(string message)
        {
            Success = true;
            Message = message;
        }

        public BaseResponse(string message, bool success)
        {
            Success = success;
            Message = message;
        }
        public BaseResponse(T data, string message = null)
        {
            Success = true;
            Message = message;
            Data = data;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string> ValidationErrors { get; set; }
        public T Data { get; set; }
    }
}
