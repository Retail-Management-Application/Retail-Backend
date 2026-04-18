namespace RetailOrdering.API.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public object? Errors { get; set; }

        public static ApiResponse<T> Ok(T? data, string message = "")
            => new() { Success = true, Data = data, Message = message };

        public static ApiResponse<T> Error(string message, object? errors = null)
            => new() { Success = false, Message = message, Errors = errors };
    }
}