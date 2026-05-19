using Models.Enums;

namespace Models.DTO
{
    public class InteractionDTO
    {
        public long Id { get; set; }

        public long CustomerID { get; set; }

        public DateTime DateTime { get; set; }

        public InteractionType Type { get; set; }

        public string? Description { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            return Equals(obj as InteractionDTO);
        }

        public override int GetHashCode() =>
            HashCode.Combine(Id, CustomerID, DateTime, Type, Description);

        private bool Equals(InteractionDTO other)
        {
            if (other is null) return false;
            return Id == other.Id
                && CustomerID == other.CustomerID
                && DateTime == other.DateTime
                && Type == other.Type
                && Description == other.Description;
        }
    }
}
