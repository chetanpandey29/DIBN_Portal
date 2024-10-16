using DIBN.Models;
using System.Collections.Generic;
using System.Data;
using static DIBN.Models.UserEmployeeViewModel;

namespace DIBN.IService
{
    public interface IUserEmployeesService
    {
        int CreateUser(UserEmployeeViewModel user);
        string GetLastAccountNumber();
        UserEmployeeViewModel GetUserDetail(int Id);
        int UpdateUser(UserEmployeeViewModel user);
        int DeleteUser(int Id);
        int CheckExistanceOfUserAccountNumber(string AccountNumber);
        int CheckExistanceOfEmail(string Email);
        List<GetRoles> GetActiveRoles();
        List<GetActiveEmployees> GetAllActiveEmployees(int CompanyId);
        List<GetInActiveEmployees> GetAllInActiveEmployees(int CompanyId);
        int GetCountOfManagers(int CompanyId);
        int GetCountOfEmployees(int CompanyId,int UserId);
        int GetCountOfShareholders(int CompanyId);
        DataSet GetEmployeesForExport(int ID);
        LoggedinStatus GetLoggedinStatus(int CompanyId, string Role, string LoggedInUser);
    }
}
