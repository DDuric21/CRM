using Models.DTO;
using System.Text.Json;

namespace CRM_UI.Services
{
    public class CustomerService : ICustomerService
    {
        public CustomerService()
        {
        }

        public async Task<IAsyncEnumerable<CustomerDTO>> GetCustomers()
        {
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7076/Customers");
            var response = await httpClient.SendAsync(request);
            var result = await response.Content.ReadAsStreamAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var deserialisedResult = JsonSerializer.DeserializeAsyncEnumerable<CustomerDTO>(result, options: options);

            return deserialisedResult;
        }

    }
}
