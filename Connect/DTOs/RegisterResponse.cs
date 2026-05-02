namespace Connect.DTOs
{
    public class RegisterResponse
    {
        public bool IsAuthenticated { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
}
