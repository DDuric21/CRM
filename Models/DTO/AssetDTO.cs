namespace Models.DTO
{
    public class AssetDTO
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public long CurrencyID { get; set; }

        public List<OptionDTO> Options { get; set; }

        public AssetDTO()
        {
            Options = new List<OptionDTO>();
        }
    }
}
