namespace Shop.Model
{
    interface IEntity
    {
        int Id { get; set; }
        IResult Validate();
    }
}
