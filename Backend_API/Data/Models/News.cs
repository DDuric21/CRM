namespace Backend_API.Data.Models
{
    public class News : BaseModel
    {
        public string Title { get; set; }

        public string Content { get; set; }
        
        public int NewsTypeID { get; set; }
    }
}
