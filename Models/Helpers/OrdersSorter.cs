using Models.DTO;

namespace Models.Helpers
{
    public class OrdersSorter : IComparer<OrderDTO>
    {
        public int Compare(OrderDTO x, OrderDTO y)
        {
            if (x == null || y == null)
            {
                throw new ArgumentException("Arguments cannot be null");
            }

            var created = y.DateCreated.CompareTo(x.DateCreated);

            var comparationResult = created == 0
                ? y.DateSubmitted.CompareTo(x.DateSubmitted)
                : created;

            return comparationResult;
        }
    }
}
