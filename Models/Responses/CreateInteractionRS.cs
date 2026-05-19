namespace Models.Responses
{
    public class CreateInteractionRS : ResponseBase
    {
        public long InteractionId { get; set; }

        public CreateInteractionRS()
        {
        }

        public CreateInteractionRS(long interactionId)
        {
            InteractionId = interactionId;
        }
    }
}
