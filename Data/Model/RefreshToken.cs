namespace Backend_API.Data.Model
{
    public class RefreshToken
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public string TokenId { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
