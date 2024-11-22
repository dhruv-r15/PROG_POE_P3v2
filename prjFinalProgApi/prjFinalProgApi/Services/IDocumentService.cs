namespace prjFinalProgApi.Services
{
    public interface IDocumentService
    {
        Task<bool> UploadDocumentAsync(int claimId, IFormFile file);
        Task<byte[]> DownloadDocumentAsync(int documentId);
    }

}
