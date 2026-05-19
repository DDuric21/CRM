using Backend_API.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Backend_API.Controllers
{
    [Route("api/billing")]
    public class BillingProxyController : AuthorizationController
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _cfg;

        public BillingProxyController(IHttpClientFactory httpFactory, IConfiguration cfg)
        {
            _httpFactory = httpFactory;
            _cfg = cfg;
        }

        [HttpGet("customers/{customerId:long}/invoices")]
        public async Task<IActionResult> GetInvoices(
            long customerId, 
            DateTime? from = null, 
            DateTime? to = null,                                                     
            int page = 1, int pageSize = 50)
        {

            var qs = new List<string> { $"page={page}", $"pageSize={pageSize}" };
            if (from.HasValue) qs.Add($"from={Uri.EscapeDataString(from.Value.ToString("O"))}");
            if (to.HasValue) qs.Add($"to={Uri.EscapeDataString(to.Value.ToString("O"))}");

            var url = $"/api/billing/customers/{customerId}/invoices";
            if (qs.Count > 0) url += "?" + string.Join("&", qs);

            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            try
            {
                var client = _httpFactory.CreateClient("BillingApp");
                using var resp = await client.SendAsync(req);
                var body = await resp.Content.ReadAsStringAsync();

                return new ContentResult
                {
                    StatusCode = (int)resp.StatusCode,
                    Content = body,
                    ContentType = "application/json"
                };
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, "Error while sending request to BillingApp");
                return StatusCode(500, $"Error fetching invoices: {ex.Message}");
            }
        }

        [HttpGet("signed-link/{billId:long}")]
        public async Task<IActionResult> MintSignedLink(long billId, [FromQuery] long customerId, [FromQuery] int ttlSeconds = 600)
        {
            var url = $"/api/bills/{billId}/signed-link?ttlSeconds={ttlSeconds}&customerId={customerId}";
            using var req = new HttpRequestMessage(HttpMethod.Post, url);                

            try
            {
                var client = _httpFactory.CreateClient("BillingApp");
                using var resp = await client.SendAsync(req);
                var body = await resp.Content.ReadAsStringAsync();
                return new ContentResult
                {
                    StatusCode = (int)resp.StatusCode,
                    Content = body,
                    ContentType = "application/json"
                };
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, $"Error while sending request {nameof(MintSignedLink)} to BillingApp");
                return StatusCode(500, $"Error minting signed link: {ex.Message}");
            }
        }
    }
}
