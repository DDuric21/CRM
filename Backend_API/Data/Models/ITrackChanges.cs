namespace Backend_API.Data.Models
{
    public interface ITrackChanges
    {
        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }
    }
}
