namespace TenVids.Utilities
{
    public class ApiResponse
    {
        public ApiResponse() { }

        public ApiResponse(int statusCode, string message = null, object result = null, string title = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultStatusText(statusCode);
            Result = result;
            Title = title ?? GetDefaultStatusText(statusCode);
            IsSuccess = statusCode is >= 200 and < 300;
        }

        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object Result { get; set; }
        public string Title { get; set; }
        public bool IsSuccess { get; set; }

        private static string GetDefaultStatusText(int statusCode) => statusCode switch
        {
            200 => "Success",
            201 => "Created",
            204 => "Deleted",
            400 => "Bad Request",
            401 => "Unauthorized",
            403 => "Forbidden",
            404 => "Not Found",
            500 => "Server Error",
            _ => "Unknown Status"
        };
    }
}
