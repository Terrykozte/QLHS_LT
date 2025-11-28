using System;

namespace QLTN_LT.DTO
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static Result<T> Success(T data, string message = "Success")
        {
            return new Result<T> { IsSuccess = true, Data = data, Message = message };
        }

        public static Result<T> Failure(string message)
        {
            return new Result<T> { IsSuccess = false, Message = message };
        }
    }

    public class Result
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public static Result Success(string message = "Success")
        {
            return new Result { IsSuccess = true, Message = message };
        }

        public static Result Failure(string message)
        {
            return new Result { IsSuccess = false, Message = message };
        }
    }
}
