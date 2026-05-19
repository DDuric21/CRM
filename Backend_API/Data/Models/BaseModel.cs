using System.ComponentModel.DataAnnotations;

namespace Backend_API.Data.Models
{
    public abstract class BaseModel : ITrackChanges
    {
        [Key]
        public long Id { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }
    }
}
