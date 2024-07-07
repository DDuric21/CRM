namespace Models.Classes
{
    public class EditableID
    {
        public long Id{ get; set; }

        public bool IsEditable { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
