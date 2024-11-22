using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using prjProgFinalMVC2.ViewModels;

namespace prjProgFinalMVC2.Services
{
    public interface IDocumentService
    {
        Task<bool> UploadDocumentAsync(int claimId, IFormFile file);
        Task<byte[]> DownloadDocumentAsync(int documentId);
        Task<DocumentViewModel> GetDocumentAsync(int id);
    }

    public class DocumentService : IDocumentService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DocumentService> _logger;

        public DocumentService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ILogger<DocumentService> logger)
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

        public async Task<bool> UploadDocumentAsync(int claimId, IFormFile file)
        {
            AddAuthorizationHeader();
            using var content = new MultipartFormDataContent();
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            content.Add(new ByteArrayContent(stream.ToArray()), "file", file.FileName);

            var response = await _httpClient.PostAsync($"api/Document/{claimId}", content);
            _logger.LogInformation($"Document upload response: {response.StatusCode}");

            return response.IsSuccessStatusCode;
        }

        public async Task<byte[]> DownloadDocumentAsync(int documentId)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"api/Document/{documentId}");
            return response.IsSuccessStatusCode ?
                await response.Content.ReadAsByteArrayAsync() : null;
        }

        public async Task<DocumentViewModel> GetDocumentAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"api/Document/{id}");

            if (response.IsSuccessStatusCode)
            {
                var document = await response.Content.ReadFromJsonAsync<DocumentViewModel>();
                var fileContent = await DownloadDocumentAsync(id);
                document.FileContent = fileContent;
                return document;
            }

            return null;
        }
    }
}
