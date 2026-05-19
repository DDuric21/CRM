using System.Text;

namespace Models.Helpers
{
    public static class ExceptionHelper
    {
        public static string FlattenExceptionMessages(Exception ex)
        {
            if (ex == null) return string.Empty;

            var sb = new StringBuilder();
            int level = 0;

            while (ex != null)
            {
                sb.AppendLine($"--- Exception Level {level} ---");
                sb.AppendLine($"Type      : {ex.GetType().FullName}");
                sb.AppendLine($"Message   : {ex.Message}");
                sb.AppendLine($"StackTrace: {ex.StackTrace}");
                sb.AppendLine();

                ex = ex.InnerException;
                level++;
            }

            return sb.ToString();
        }
    }
}
