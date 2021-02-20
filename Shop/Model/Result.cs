namespace Shop.Model
{
    class Result: IResult
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "";

        public Result()
        {
            
        }

        public Result(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
