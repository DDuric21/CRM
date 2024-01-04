namespace Backend_API.HelperMethods
{
    public static class Helper
    {
        public static bool IsNullOrEmpty<T>(this T obj)
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
    }
}
