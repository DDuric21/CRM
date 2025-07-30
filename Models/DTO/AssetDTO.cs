using Models.Enums;

namespace Models.DTO
{
    public class AssetDTO
    {
        public long Id { get; set; }

        public long CustomerAssetID { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public long CurrencyID { get; set; }

        public ItemState AssetStatus { get; set; }

        public ItemAction ItemAction { get; set; }

        public BillingProfileDTO BillingProfile { get; set; }

        public List<OptionDTO> Options { get; set; }

        public AddressDTO AssetAddress { get; set; }

        public AssetDTO()
        {
            Options = new List<OptionDTO>();
            AssetAddress = new AddressDTO(); //this is temporary
        }
    }
}
