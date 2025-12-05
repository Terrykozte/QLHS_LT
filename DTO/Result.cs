using System;

namespace QLTN_LT.DTO
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public bool Success { get => IsSuccess; set => IsSuccess = value; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static Result<T> CreateSuccess(T data, string message = "Success")
        {
            return new Result<T> { IsSuccess = true, Data = data, Message = message };
        }

        public static Result<T> CreateFailure(string message)
        {
            return new Result<T> { IsSuccess = false, Message = message };
        }
    }

    public class Result
    {
        public bool IsSuccess { get; set; }
        public bool Success { get => IsSuccess; set => IsSuccess = value; }
        public string Message { get; set; }

        public static Result CreateSuccess(string message = "Success")
        {
            return new Result { IsSuccess = true, Message = message };
        }

        public static Result CreateFailure(string message)
        {
            return new Result { IsSuccess = false, Message = message };
        }
    }
}
