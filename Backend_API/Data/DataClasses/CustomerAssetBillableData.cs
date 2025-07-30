using Newtonsoft.Json;

namespace Backend_API.Data.DataClasses
{
    public class CustomerAssetBillableData
    {
        [JsonProperty("BillingAssetId")]
        public long CustomerAssetId { get; set; }

        [JsonProperty("BillingAssetActionTypeId")]
        public int CustomerAssetActionId { get; set; }

        public decimal Price { get; set; }

        public long CurrencyID { get; set; }

        [JsonProperty("BillingItemSupplements")]
        public IEnumerable<CustomerAssetAddition> CustomerAssetAdditions { get; set; }
    }
}
