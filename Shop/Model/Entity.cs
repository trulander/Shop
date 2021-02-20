namespace Shop.Model
{
    abstract class Entity : IEntity
    {
        public int Id { get; set; }
        public abstract IValidateResult Validate();
    }
}
