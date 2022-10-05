namespace API.DTOs
{
    // This Dto will be used when user logs in or registers in this case.
    public class UserDto
    {
        public string Username { get; set; }
        public string Token { get; set; }
    }
}