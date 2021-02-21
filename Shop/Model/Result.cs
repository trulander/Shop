namespace Shop.Model
{
    class Result: IResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";

        public Result(bool success = false)
        {
            Success = success;
        }
    }
}
