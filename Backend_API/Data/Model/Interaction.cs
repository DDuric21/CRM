using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Data.Model
{
    public class Interaction : BaseModel
    {
        [ForeignKey("CustomerId")]
        public long CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        public DateTime DateTime { get; set; }

        public int TypeID { get; set; }

        public string? Description { get; set; }
    }
}
