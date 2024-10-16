using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DIBN.Areas.Admin.Models
{
    public class MainCompanySubTypeModel
    {
        public MainCompanySubTypeModel()
        {
            allowedModule = new List<string>();
            companySubTypes = new List<GetAllCompanySubTypeModel>();
        }
        public List<string> allowedModule {  get; set; }
        public List<GetAllCompanySubTypeModel> companySubTypes { get; set; }
        public string message {  get; set; }
    }
    public class GetAllCompanySubTypeModel
    {
        public int Index { get; set; }
        public int Id { get; set; }
        public string CompanyType {  get; set; }
        public string SubType {  get; set; }
    }
    public class SaveCompanySubTypeModel
    {
        public SaveCompanySubTypeModel()
        {
            MainTypes = new List<SelectListItem>();
        }
        public List<SelectListItem> MainTypes { get; set; }
        [DisplayName("Sub Type")]
        [Required(ErrorMessage ="Please provide sub type.")]
        [MaxLength(150,ErrorMessage ="Please provide valid sub type. Maximum length of Sub type should be less than 150.")]
        public string SubType { get; set; }
        [DisplayName("Main Type")]
        [Required(ErrorMessage = "Please provide main type.")]
        [MaxLength(150, ErrorMessage = "Please provide valid main type. Maximum length of Main type should be less than 150.")]
        public string MainType { get; set; }
        public int UserId {  get; set; }
    }
    public class UpdateCompanySubTypeModel
    {
        public UpdateCompanySubTypeModel()
        {
            MainTypes = new List<SelectListItem>();
        }
        public List<SelectListItem> MainTypes { get; set; }
        [DisplayName("Main Type")]
        [Required(ErrorMessage = "Please provide main type.")]
        [MaxLength(150, ErrorMessage = "Please provide valid main type. Maximum length of Main type should be less than 150.")]
        public string SubType { get; set; }

        [DisplayName("Sub Type")]
        [Required(ErrorMessage = "Please provide sub type.")]
        [MaxLength(150, ErrorMessage = "Please provide valid sub type. Maximum length of Sub type should be less than 150.")]
        public string MainType { get; set; }
        public int UserId { get; set; }
        public int Id { get; set; }
    }
}
