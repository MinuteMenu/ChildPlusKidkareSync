namespace ChildPlusKidkareSync.Core.Models.Sync
{
    public enum ResponseStatus
    {
        Success = 200,
        BadRequest = 400,
        Unauthorized = 401,
        NotFound = 404,
        InternalServerError = 500
    }

    public class ResponseWithData<T> where T : class
    {
        public T Data { get; private set; }
        public string Message { get; private set; }
        public bool IsSuccess { get; private set; }
        public int ResponseCode { get; private set; }

        private ResponseWithData(bool isSuccess, int responseCode, string message = null, T data = null)
        {
            IsSuccess = isSuccess;
            ResponseCode = responseCode;
            Message = message;
            Data = data;
        }

        // Factory methods
        public static ResponseWithData<T> Success(T data = null, string message = null)
        {
            return new ResponseWithData<T>(true, (int)ResponseStatus.Success, message, data);
        }

        public static ResponseWithData<T> Fail(string message = null, int responseCode = (int)ResponseStatus.BadRequest)
        {
            return new ResponseWithData<T>(false, responseCode, message);
        }
    }

    public class ParseResult<T>
    {
        public int RowNumber { get; set; }      // Track row position
        public T Result { get; set; }            // The actual data
        public List<Error> Errors { get; set; }  // Validation errors
    }

    public class Error
    {
        public string ColumnName { get; set; }
        public string CurrentValue { get; set; }
        public List<string> Errors { get; set; }
        public string ValidValues { get; set; }
    }
}
