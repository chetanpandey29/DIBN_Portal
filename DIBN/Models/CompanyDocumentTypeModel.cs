namespace DIBN.Models
{
    public class CompanyDocumentTypeModel
    {
		public int ID { get; set; }
		public string DocumentName { get; set; }
		public bool IsActive { get; set; }
		public bool IsDelete { get; set; }
		public string CreatedOnUtc { get; set; }
		public string ModifyOnUtc { get; set; }
		public string Module { get; set; }
	}
}
