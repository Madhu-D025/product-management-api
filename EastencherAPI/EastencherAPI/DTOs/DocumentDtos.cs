using System.ComponentModel.DataAnnotations;

namespace AuthApplication.DTOs
{
    public class Documentdto
    {
        public string? Id { get; set; }
        public List<IFormFile>? Documents { get; set; }
        public string? DocumentId { get; set; }
        public List<int>? DocumentNameId { get; set; }
        public string? documentType { get; set; }
        public string? FolderName { get; set; }
        public string? UserID { get; set; }
        public int? InitiationId { get; set; } = 0;
    }
}