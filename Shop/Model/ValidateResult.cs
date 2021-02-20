namespace Shop.Model
{
    class ValidateResult : IValidateResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public ValidateResult(bool result)
        {
            Success = result;
        }
    }
}