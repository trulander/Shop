namespace Shop.Model
{
    class ActionResult: IActionResult
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "";

        public ActionResult()
        {
            
        }

        public ActionResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
