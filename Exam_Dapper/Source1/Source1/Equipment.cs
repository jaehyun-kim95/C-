
namespace Source1
{
    public enum EquipmentStatus { Available = 0, OnLoan = 1, Broken = 2 };
    internal class Equipment
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public EquipmentStatus Status { get; set; }
        public DateTime? PurchasedOn { get; set; }
        public string? Note { get; set; }
        public string Category { get; set; } = "";
    }
}
