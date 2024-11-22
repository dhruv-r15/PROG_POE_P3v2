namespace prjProgFinalMVC2.ViewModels
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }
    }

}
