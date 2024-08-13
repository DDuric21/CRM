namespace Backend_API.Data.Model
{
    public interface ITrackChanges
    {
        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }
    }
}
