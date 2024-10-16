using DIBN.Areas.Admin.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using static DIBN.Areas.Admin.Models.UserViewModel;

namespace DIBN.Areas.Admin.IRepository
{
    public interface IUserRepository
    {
        List<UserViewModel> GetUsers();
        int CreateUser(UserViewModel user);
        string GetLastAccountNumber();
        UserViewModel GetUserDetail(int Id);
        int UpdateUser(UserViewModel user);
        int DeleteUser(int Id, int? mainCompany, int UserId);
        int CheckExistanceOfUserAccountNumber(string AccountNumber, int? userId);
        int CheckExistanceOfEmail(string Email);
        int CreateUserForCompany(SaveUser user);
        int AddNewCompanyForUser(SaveCompanyForUser company);
        int GetCompanyId(int UserId);
        List<GetInActiveEmployees> GetAllInActiveEmployees(int CompanyId);
        List<GetActiveEmployees> GetAllActiveEmployees(int CompanyId);
        List<int> GetEmployeesCount();
        DataSet GetEmployeesForExport(int? ID);
        List<GetMainCompanyEmployees> GetMainCompanyEmployees(int CompanyId);
        List<KeyValuePair<string, string>> GetUserNamesForAssign(int CompanyId);
        List<UserListForCompany> GetUserListForCompany(int CompanyId);
        List<GetInActiveEmployees> GetInActiveEmployeesCompanyWise(int CompanyId);
        List<GetActiveEmployees> GetActiveEmployeesCompanyWise(int CompanyId);
        List<decimal> GetTotalPortalBalance();
        int CreateEmployee(SaveNewUser user);
        int RemoveDocument(int DocumentId, int UserId);
        string GetCompanyType(int CompanyId);
        int GetCurrentNotificationCount(int UserId);
        List<UserListForCompany> GetAssignedUserListForCompany(int CompanyId);
        DataSet GetMainCompanyEmployeesForExport();
        LoggedinStatus GetLoggedinStatus(int CompanyId, string Role, string LoggedInUser);
        Task<bool> GetCompanyEmployeeActiveStatus(int Id);
        Task<GetAllActiveEmployeeListWithPaginationModel> GetAllActiveEmployeesWithPagination(int? companyId,int page, int pageSize, string searchString, string sortBy, string sortDirection);
        Task<GetAllInActiveEmployeeListWithPaginationModel> GetAllInActiveEmployeesWithPagination(int? companyId, int page, int pageSize, string searchString, string sortBy, string sortDirection);
        Task<GetAllActiveCompanyOwnerListWithPaginationModel> GetAllActiveCompanyOwnerWithPagination(int? companyId, int page, int pageSize, string searchString, string sortBy, string sortDirection);
        Task<GetAllInActiveCompanyOwnerListWithPaginationModel> GetAllInActiveCompanyOwnerWithPagination(int? companyId, int page, int pageSize, string searchString, string sortBy, string sortDirection);
        Task<GetAllActiveMainCompanyEmployeeListWithPaginationModel> GetAllActiveMainCompanyEmployeesWithPagination(int? CompanyId, int page, int pageSize, string searchBy, string searchString, string sortBy, string sortDirection);
        Task<GetAlInlActiveMainCompanyEmployeeListWithPaginationModel> GetAllInActiveMainCompanyEmployeesWithPagination(int? CompanyId, int page, int pageSize, string searchBy, string searchString, string sortBy, string sortDirection);
    }
}
