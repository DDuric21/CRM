namespace Models.Responses
{
    public class CreateInteractionRS
    {
        public long InteractionId { get; set; }

        public CreateInteractionRS(long interactionId)
        {
            InteractionId = interactionId;
        }
    }
}
