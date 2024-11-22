using prjProgFinalMVC2.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;

namespace prjProgFinalMVC2.Services
{
    public interface IModuleService
    {
        Task<IEnumerable<ModuleViewModel>> GetModulesAsync();
        Task<ModuleViewModel> GetModuleAsync(int id);
        Task<bool> CreateModuleAsync(CreateModuleViewModel module);
        Task<bool> DeleteModuleAsync(int id);
    }

    public class ModuleService : IModuleService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ModuleService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddAuthorizationHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<IEnumerable<ModuleViewModel>> GetModulesAsync()
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync("api/Module");
            return response.IsSuccessStatusCode ?
                await response.Content.ReadFromJsonAsync<IEnumerable<ModuleViewModel>>() :
                new List<ModuleViewModel>();
        }

        public async Task<ModuleViewModel> GetModuleAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"api/Module/{id}");
            return response.IsSuccessStatusCode ?
                await response.Content.ReadFromJsonAsync<ModuleViewModel>() : null;
        }

        public async Task<bool> CreateModuleAsync(CreateModuleViewModel module)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("api/Module", module);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteModuleAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"api/Module/{id}");
            return response.IsSuccessStatusCode;
        }
    }


}
