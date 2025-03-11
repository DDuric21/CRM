using Models.Enums;

namespace Models.DTO
{
    public class NewsDTO
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public NewsType NewsType { get; set; }
    }
}
