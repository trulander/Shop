namespace Shop.Model
{
    internal interface IActionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}