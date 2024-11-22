namespace prjProgFinalMVC2.ViewModels
{
    public class DocumentViewModel
    {
        public int DocumentId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] FileContent { get; set; }
        public int ClaimId { get; set; }
    }

}
