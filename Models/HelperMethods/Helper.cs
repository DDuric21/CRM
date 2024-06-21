namespace Models.HelperMethods
{
    public static class Helper
    {
        public static bool IsNullOrEmpty(this object obj)
        {
            if (obj == null)
            {
                return true;
            }

            return !obj.GetType()
                     .GetProperties() //get all properties on object
                     .Select(x => x.GetValue(obj)) //get value for the property
                     .Any(y => y != null);
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return list == null || !list.Any();
        }

        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source, CancellationToken cancellationToken = default)
        {
            var list = new List<T>();
            await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                list.Add(item);
            }

            return list;
        }

        public static DateTime ParseClaimExpiryToDatetime(string claimExpiry)
        {
            DateTime expiryDate = DateTime.UnixEpoch;
            try
            {
                var value = long.Parse(claimExpiry);
                expiryDate = expiryDate.AddSeconds(value).ToUniversalTime();
            }
            catch (FormatException)
            {
                //dodati logiranje
            }

            return expiryDate;
        }
    }
}
