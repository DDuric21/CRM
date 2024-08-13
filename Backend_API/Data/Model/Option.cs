using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Data.Model
{
    public class Option : BaseModel
    {
        public string Name { get; set; }

        [Precision(18, 2)]
        public decimal Price { get; set; }

        public long CurrencyID { get; set; }

        [ForeignKey("AssetID")]
        public long AssetID { get; set; }
        public virtual Asset Asset { get; set; }

        [NotMapped]
        public virtual ICollection<CustomerAssetOptions> CustomerAssetOptions { get; set; }
    }
}
