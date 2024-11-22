namespace prjProgFinalMVC2.ViewModels
{
    public class LecturerViewModel
    {
        public int LecturerId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }
}
