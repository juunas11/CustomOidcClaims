namespace CustomOidcClaims.Data
{
    public class User
    {
        public int Id { get; set; }
        public string ObjectId { get; set; }
        public string TenantId { get; set; }
        public bool IsAdmin { get; set; }
        public string UserPrincipalName { get; set; }
    }
}