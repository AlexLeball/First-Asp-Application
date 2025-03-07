namespace project_5.Models.Entities
{
    public class Repair
    {
        public int Id { get; set; }
        public int Price { get; set; }
        public virtual Car? Car { get; set; }
        public string? Description { get; set; }

    }
}
