using Models.DTO;

namespace Models.Helpers
{
    public class InteractionsSorter : IComparer<InteractionDTO>
    {
        public int Compare(InteractionDTO x, InteractionDTO y)
        {
            if (x == null || y == null)
            {
                throw new ArgumentException("Arguments cannot be null");
            }
            return y.DateTime.CompareTo(x.DateTime);
        }
    }
}
