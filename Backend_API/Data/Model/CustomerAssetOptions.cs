using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Data.Model
{
    public class CustomerAssetOptions : ITrackChanges
    {
        [ForeignKey("CustomerAssetsID")]
        public long CustomerAssetsID { get; set; }
        public virtual CustomerAssets CustomerAssets { get; set; }

        [ForeignKey("OptionID")]
        public long OptionID { get; set; }
        public virtual Option Option { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
