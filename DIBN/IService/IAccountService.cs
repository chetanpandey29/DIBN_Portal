using DIBN.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using static DIBN.Models.AccountViewModel;

namespace DIBN.IService
{
    public interface IAccountService
    {
        List<string> Login(LoginViewModel model);
        Task<string> sendMail(string CustomerName, string Email, string url);
        List<string> CheckExistanceOfEmail(string Email);
        string GetAccountType(string Email);
        int ChangePassword(ChangePasswordModel model);
        int CheckSalesPerson(string Email);
        List<string> SalesPersonLogin(LoginViewModel model);
    }
}
