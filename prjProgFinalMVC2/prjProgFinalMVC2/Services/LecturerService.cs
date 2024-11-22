using prjProgFinalMVC2.ViewModels;
using System.Net.Http.Headers;

namespace prjProgFinalMVC2.Services
{
    public interface ILecturerService
    {
        Task<IEnumerable<LecturerViewModel>> GetLecturersAsync();
        Task<bool> CreateLecturerAsync(CreateLecturerViewModel lecturer);
        Task<bool> DeleteLecturerAsync(int id);
    }

    public class LecturerService : ILecturerService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LecturerService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddAuthorizationHeader()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWTToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<IEnumerable<LecturerViewModel>> GetLecturersAsync()
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync("api/Lecturer");

            if (response.IsSuccessStatusCode)
            {
                var lecturers = await response.Content.ReadFromJsonAsync<IEnumerable<LecturerViewModel>>();
                return lecturers;
            }
            return new List<LecturerViewModel>();
        }

        public async Task<bool> CreateLecturerAsync(CreateLecturerViewModel lecturer)
        {
            AddAuthorizationHeader();
            var lecturerDto = new
            {
                username = lecturer.Username,
                firstName = lecturer.FirstName,
                lastName = lecturer.LastName,
                email = lecturer.Email,
                password = lecturer.Password
            };

            var response = await _httpClient.PostAsJsonAsync("api/Lecturer", lecturerDto);
            Console.WriteLine($"Create Response: {response.StatusCode}");
            Console.WriteLine($"Response Content: {await response.Content.ReadAsStringAsync()}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteLecturerAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"api/Lecturer/{id}");
            return response.IsSuccessStatusCode;
        }

    }


}
