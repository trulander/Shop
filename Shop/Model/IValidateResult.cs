namespace Shop.Model
{
    public interface IValidateResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}