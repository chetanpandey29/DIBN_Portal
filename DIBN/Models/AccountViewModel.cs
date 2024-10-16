using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DIBN.Models
{
    public class AccountViewModel
    {
        #region NestedClass
        public class LoginViewModel
        {
            [Required(ErrorMessage = "Please enter your Username.")]
            public string Email { get; set; }
            [Required(ErrorMessage = "Please enter your Password.")]
            public string Password { get; set; }
            public bool RememberMe { get; set; }
            public List<BannerImageViewModel> banners { get; set; }
        }

        public class LoginReturnResult
        {
            public string Role { get; set; }
            public string CompanyId { get; set; }
            public int countOfPermission { get; set; }
        }
        #endregion
    }


    public class ChangePasswordModel
    {
        public string User { get; set; }
        public int Id { get; set; }
        public string NewPassword { get; set; }
        public string AccountType { get; set; }
    }
}
