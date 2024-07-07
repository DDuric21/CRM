using Models.Classes;

namespace Models.Helpers
{
    public class InteractionKeySorter : IComparer<EditableID>
    {
        public int Compare(EditableID x, EditableID y)
        {
            if (x == null || y == null)
            {
                throw new ArgumentException("Arguments cannot be null");
            }

            return y.DateCreated.CompareTo(x.DateCreated);
        }
    }
}
