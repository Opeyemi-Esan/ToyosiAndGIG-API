using System.Net;

namespace ToyosiAndGIG.Domain.Response
{
    public class Response<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public bool IsSuccess => (int)StatusCode is >= 200 and < 300;

        public static Response<T> Success(T data, string message = "Success", HttpStatusCode statusCode = HttpStatusCode.OK)
        => new()
        {
            Data = data,
            Message = message,
            StatusCode = statusCode
        };

        public static Response<T> Fail(string message,HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new()
        {
            Data = default,
            Message = message,
            StatusCode = statusCode
        };
    }
}
