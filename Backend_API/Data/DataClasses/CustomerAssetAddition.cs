namespace Backend_API.Data.DataClasses
{
    public class CustomerAssetAddition
    {
        public long AdditionId { get; set; }

        public decimal Price { get; set; }

        public long CurrencyID { get; set; }

        public int AdditionActionId { get; set; }
    }
}
