
using Backend_API.Services;
using Backend_API.Services.Implementations;

namespace Backend_API.MessageCommands
{
    public class OrderStatusUpdateCommand : BaseCommand
    {
        public Guid OrderId { get; set; }

        public string Status { get; set; }

        public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            using var scope = DiContainer.Provider.CreateScope();
            var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

            var statusId = Enum.TryParse<Models.Enums.OrderStatus>(Status, out var parsedStatus)
                ? (int)parsedStatus
                : throw new ArgumentException($"Invalid order status: {Status}");

            await orderService.UpdateOrderStatusAsync(OrderId, statusId);
        }
    }
}
