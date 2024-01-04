namespace Backend_API.Data.DTO
{
    public class AssetDTO
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public long CurrencyID { get; set; }

        public ICollection<OptionDTO>? Options { get; set; }
    }
}
