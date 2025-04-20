namespace API.DTOs
{
    public class RefreshRequest
    {
        public string Username { get; set; }

        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }

}
