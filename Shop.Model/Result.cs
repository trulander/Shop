namespace Shop.Model
{
    public class Result : IResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public Result(bool success = false) => Success = success;
        public Result(string message) => Message = message;
    }
}
