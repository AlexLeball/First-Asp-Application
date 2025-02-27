namespace project_5.Models.Entities.cs
{
    public class Repair
    {
        public int RepairId { get; set; }
        public int Price { get; set; }
        public virtual Car? Car { get; set; }
        public string? Description { get; set; }

    }
}
