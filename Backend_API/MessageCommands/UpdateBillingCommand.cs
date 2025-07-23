namespace Backend_API.MessageCommands
{
    public class UpdateBillingCommand : BaseCommand
    {
        public Guid OrderId { get; set; }
    }
}
