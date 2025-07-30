namespace Backend_API.MessageCommands
{
    public abstract class BaseCommand
    {
        public abstract Task ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
