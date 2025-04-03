using Models.Authentication;
using System.Text.Json;

namespace UI.Authentication
{
    public static class JasonWebToken
    {
        private static DateTime ExpiryTime { get; set; }

        private static Dictionary<string, object> jwtValue { get; set; }

        private static string jwt { get; set; }

        public static string Value
        {
            get
            {
                return jwt;
            }
            set
            {
                jwt = value;
                DecodeJwt();
                ExpiryTime = ReadExpiryTime();
            }
        }

        public static bool IsExpired()
        {
            return ExpiryTime <= DateTime.UtcNow;
        }

        private static DateTime ReadExpiryTime()
        {
            var expiryTime = ReadValue(CrmJwtClaimNames.Expiration).FirstOrDefault();

            var expiryTimeValue = DateTime.TryParse(expiryTime, out var parsedValue)
                ? parsedValue
                : DateTime.MinValue;

            return expiryTimeValue;
        }

        private static void DecodeJwt()
        {
            if (string.IsNullOrEmpty(jwt))
            {
                jwtValue = null;
                return;
            }

            var parts = jwt.Split('.');
            if (parts.Length != 3)
            {
                throw new ArgumentException("Invalid JWT format");
            }

            var payload = parts[1];
            var alignedPayload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            var jsonBytes = Convert.FromBase64String(alignedPayload);
            jwtValue = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        }

        public static IEnumerable<string> ReadValue(string nodeKey)
        {
            if (!jwtValue.TryGetValue(nodeKey, out var nodeValue) || nodeValue is null)
            {
                // add logging
                return Enumerable.Empty<string>();
            }

            var values = new List<string>();

            if (nodeValue is JsonElement jsonElement
                && jsonElement.ValueKind == JsonValueKind.Array)
            {
                values = jsonElement.EnumerateArray()
                    .Select(r => r.ToString())
                    .ToList();
            }
            else
            {
                values.Add(nodeValue.ToString());
            }

            return values;
        }
    }
}
