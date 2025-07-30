using Backend_API.Data.DataClasses;
using Newtonsoft.Json;

namespace Backend_API.MessageCommands
{
    public class UpdateBillingCommand : BaseCommand
    {
        public Guid OrderId { get; set; }

        public string BillingProfileId { get; set; }

        [JsonProperty("BillingItemData")]
        public CustomerAssetBillableData CustomerAssetBillableData { get; set; }
    }
}
