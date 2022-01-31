namespace Aurelia.App.Models
{
    public abstract class Base 
    {
        public Base()
        {
            this.Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
    }
}
