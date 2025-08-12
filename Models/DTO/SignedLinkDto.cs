namespace Models.DTO
{
    public class SignedLinkDto
    {
        public string Url { get; set; } = default!;

        public DateTime ExpiresUtc { get; set; }
    }
}
