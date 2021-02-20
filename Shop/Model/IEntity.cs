namespace Shop.Model
{
    public interface IEntity
    {
        public int Id { get; set; }
        public IValidateResult Validate();
    }
}
