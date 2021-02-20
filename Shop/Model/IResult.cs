namespace Shop.Model
{
    interface IResult
    {
        bool Success { get; set; }
        string Message { get; set; }
    }
}