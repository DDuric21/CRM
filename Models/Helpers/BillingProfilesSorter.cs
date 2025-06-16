using Models.Classes;

namespace Models.Helpers
{
    public class BillingProfileKeySorter : IComparer<EditableID>
    {
        public int Compare(EditableID x, EditableID y)
        {
            if (x == null || y == null)
            {
                throw new ArgumentException("Arguments cannot be null");
            }

            if (ReferenceEquals(x.CustomId, y.CustomId)) return 0;
            if (x.CustomId is null) return 1;
            if (y.CustomId is null) return -1;

            // Try to split into two ints
            var partsX = x.CustomId.Split('-', 2);
            var partsY = y.CustomId.Split('-', 2);

            if (partsX.Length != 2 || partsY.Length != 2)
            {
                // Fall back to simple string compare if format is unexpected
                return string.Compare(y.CustomId, x.CustomId,  StringComparison.Ordinal);
            }

            // Parse both numbers (with safe TryParse)
            if (!int.TryParse(partsX[0], out var x1)
                || !int.TryParse(partsX[1], out var x2)
                || !int.TryParse(partsY[0], out var y1)
                || !int.TryParse(partsY[1], out var y2))
            {
                return string.Compare(y.CustomId, x.CustomId, StringComparison.Ordinal);
            }

            // Compare first segment
            var cmp = y1.CompareTo(x1);
            if (cmp != 0) return cmp;

            // If equal, compare second segment
            return y2.CompareTo(x2);
        }
    }
}
