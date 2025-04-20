namespace API.DTOs
{
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "User"; // default role
    }

}
