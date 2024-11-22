using prjFinalProgApi.Data;
using prjFinalProgApi.Models;

namespace prjFinalProgApi.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly ClaimsDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DocumentService(ClaimsDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<bool> UploadDocumentAsync(int claimId, IFormFile file)
        {
            var uploadsFolder = Path.Combine(_environment.ContentRootPath, "Uploads");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var document = new Document
            {
                ClaimId = claimId,
                FileName = file.FileName,
                FilePath = uniqueFileName,
                UploadDate = DateTime.Now
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<byte[]> DownloadDocumentAsync(int documentId)
        {
            var document = await _context.Documents.FindAsync(documentId);
            if (document == null) return null;

            var filePath = Path.Combine(_environment.ContentRootPath, "Uploads", document.FilePath);
            return await System.IO.File.ReadAllBytesAsync(filePath);
        }
    }

}
