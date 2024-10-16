namespace DIBN.Models
{
    public class ServiceRequestStatusModel
    {
        public int Id { get; set; }
        public int DisplayId { get; set; }
        public string DisplayName { get; set; }
        public int UserId { get; set; }
        public string CreatedOn { get; set; }
        public string ModifyOn { get; set; }
        public int CreatedBy { get; set; }
        public int ModifyBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string Module { get; set; }
    }
}
