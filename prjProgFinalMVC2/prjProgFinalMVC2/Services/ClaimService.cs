using prjProgFinalMVC2.Models;
using prjProgFinalMVC2.ViewModels;
using System.Net.Http.Headers;
using System.Text.Json;

namespace prjProgFinalMVC2.Services
{
    public interface IClaimService
    {
        Task<IEnumerable<ClaimViewModel>> GetClaimsAsync();
        Task<ClaimViewModel> GetClaimAsync(int id);
        Task<ClaimSubmissionResult> SubmitClaimAsync(CreateClaimViewModel claim);
        Task<bool> UpdateClaimStatusAsync(int id, string status);
    }


    public class ClaimService : IClaimService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ClaimService> _logger;

        public ClaimService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ILogger<ClaimService> logger)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private void AddAuthorizationHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<IEnumerable<ClaimViewModel>> GetClaimsAsync()
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync("api/Claims");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<ClaimViewModel>>();
            }
            return new List<ClaimViewModel>();
        }

        public async Task<ClaimViewModel> GetClaimAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"api/Claims/{id}");
            return response.IsSuccessStatusCode ?
                await response.Content.ReadFromJsonAsync<ClaimViewModel>() : null;
        }

        public async Task<ClaimSubmissionResult> SubmitClaimAsync(CreateClaimViewModel model)
        {
            AddAuthorizationHeader();

            var claimData = new
            {
                moduleId = model.ModuleId,
                hoursWorked = model.HoursWorked,
                lecturerId = model.LecturerId,
                status = "Pending",
                submissionDate = DateTime.UtcNow,
                totalAmount = model.TotalAmount
            };

            var response = await _httpClient.PostAsJsonAsync("api/Claims", claimData);
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"Response content: {content}");

            if (response.IsSuccessStatusCode)
            {
                var createdClaim = await response.Content.ReadFromJsonAsync<ClaimViewModel>();
                return new ClaimSubmissionResult
                {
                    Success = true,
                    ClaimId = createdClaim.ClaimId
                };
            }

            return new ClaimSubmissionResult { Success = false };
        }

        public async Task<bool> UpdateClaimStatusAsync(int id, string status)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PutAsync($"api/Claims/{id}/status?status={status}", null);
            return response.IsSuccessStatusCode;
        }
    }








}
