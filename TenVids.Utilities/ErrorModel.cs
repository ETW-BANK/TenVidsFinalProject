

namespace TenVids.Utilities
{
    public class ErrorModel<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public int StatusCode { get; set; } = 200;

        // Success factories
        public static ErrorModel<T> Success(T data, string message = "Operation succeeded")
            => new ErrorModel<T> { IsSuccess = true, Data = data, Message = message };

        public static ErrorModel<T> Success(string message)
            => new ErrorModel<T> { IsSuccess = true, Message = message };

        // Failure factories
        public static ErrorModel<T> Failure(string error, int statusCode = 400)
            => new ErrorModel<T> { IsSuccess = false, Message = error, StatusCode = statusCode };

        public static ErrorModel<T> Failure(IEnumerable<string> errors, int statusCode = 400)
            => new ErrorModel<T> { IsSuccess = false, Errors = errors.ToList(), StatusCode = statusCode };

        // Validation failure
        public static ErrorModel<T> ValidationFailure(IDictionary<string, string[]> validationErrors)
            => new ErrorModel<T>
            {
                IsSuccess = false,
                StatusCode = 400,
                Errors = validationErrors.SelectMany(e => e.Value).ToList(),
                Message = "Validation errors occurred"
            };
    }
}