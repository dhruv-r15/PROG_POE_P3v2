using System.Security.Claims;

namespace prjFinalProgApi.Models
{
    public class Lecturer
    {
        public int LecturerId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public virtual ICollection<Claim> Claims { get; set; } = new List<Claim>();
    }



}
