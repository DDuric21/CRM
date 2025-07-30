using Models.Enums;

namespace Models.DTO
{
    public class OptionDTO
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public ItemAction ItemAction { get; set; }
    }
}
