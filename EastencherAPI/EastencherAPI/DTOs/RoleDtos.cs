using System.ComponentModel.DataAnnotations;

namespace AuthApplication.DTOs
{
    public class RoleWithApp
    {
        public Guid? RoleID { get; set; }
        public string RoleName { get; set; }
        public int[] AppIDList { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string? ClientId { get; set; }
    }

    public class RoleWithAppView
    {
        public Guid RoleID { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? ClientId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public int[] AppIDList { get; set; }
        public string AppNames { get; set; }
    }
}