namespace SignalRNet6.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual Team Team { get; set; }
    }
}
