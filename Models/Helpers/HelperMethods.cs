using System.Reflection;

namespace Models.Helpers
{
    public static class HelperMethods
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

        public static T Clone<T>(this T source) where T : new()
        {
            if (source is null)
            {
                return default;
            }

            T clone = new T();
            var sourceType = source.GetType();
            var sourceProperties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in sourceProperties)
            {
                if (property.CanRead 
                    && property.CanWrite
                    && (property.PropertyType.IsValueType || property.PropertyType == typeof(string)))
                {
                    property.SetValue(clone, property.GetValue(source));
                }
            }

            var sourceFields = sourceType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in sourceFields)
            {
                if (field.FieldType.IsValueType || field.FieldType == typeof(string))
                {
                    field.SetValue(clone, field.GetValue(source));
                }
            }

            return clone;
        }
    }
}
