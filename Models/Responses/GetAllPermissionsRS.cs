namespace Models.Responses
{
    public class GetAllPermissionsRS : ResponseBase
    {
        public IEnumerable<string> Permissions { get; set; }
    }
}
