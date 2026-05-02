namespace Connect.Options
{
    public class Jwt
    {
        public string Key { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public ushort ExpireDays { get; set; }
    }
}
