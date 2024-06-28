using Models.Enums;

namespace Models.DTO
{
    public class InteractionDTO
    {
        public long Id { get; set; }

        public DateTime DateTime { get; set; }

        public InteractionType Type { get; set; }

        public string? Description { get; set; }
    }
}
