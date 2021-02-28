namespace Shop.Model
{
    public interface IResult
    {
        bool Success { get; set; }
        string Message { get; set; }
    }
}