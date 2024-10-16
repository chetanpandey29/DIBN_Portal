using DIBN.Areas.Admin.Data;
using DIBN.IService;
using DIBN.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using static DIBN.Areas.Admin.Data.DataSetting;
using static DIBN.Models.UserEmployeeViewModel;

namespace DIBN.Service
{
    public class UserEmployeesService:IUserEmployeesService
    {
        private readonly Connection _dataSetting;
        private readonly EncryptionService _encryptionService;
        public UserEmployeesService(Connection dataSetting, EncryptionService encryptionService)
        {
            _dataSetting = dataSetting;
            _encryptionService = encryptionService;
        }

        public int CreateUser(UserEmployeeViewModel user)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                var _Password = _encryptionService.EncryptText(user.Password);
                int returnId = 0;
                SqlCommand cmd = new SqlCommand("USP_Admin_UserOperation", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Status", Operation.Insert);
                cmd.Parameters.AddWithValue("@AccountNumber", user.AccountNumber);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                cmd.Parameters.AddWithValue("@CompanyId", user.CompanyId);
                cmd.Parameters.AddWithValue("@Password", _Password);
                cmd.Parameters.AddWithValue("@Nationality", user.Nationality);
                cmd.Parameters.AddWithValue("@EmailID", user.EmailID);
                cmd.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                cmd.Parameters.AddWithValue("@CountryOfRecidence", user.CountryOfRecidence);
                //cmd.Parameters.AddWithValue("@CompanyAddress", user.CompanyAddress);
                cmd.Parameters.AddWithValue("@TelephoneNumber", user.TelephoneNumber);
                cmd.Parameters.AddWithValue("@RoleId", user.RoleId);
                con.Open();
                returnId = (int)cmd.ExecuteScalar();
                con.Close();
                return returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public string GetLastAccountNumber()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string last = "";
                SqlCommand cmd = new SqlCommand("USP_Admin_UserOperation", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Status", Operation.Show);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    last = dr["AccountNumber"].ToString();
                }
                con.Close();
                return last;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public UserEmployeeViewModel GetUserDetail(int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                UserEmployeeViewModel user = new UserEmployeeViewModel();
                SqlCommand cmd = new SqlCommand("USP_Admin_UserOperation", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Status", Operation.GetById);
                cmd.Parameters.AddWithValue("@ID", Id);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    user.Id = Convert.ToInt32(dr["ID"].ToString());
                    user.AccountNumber = dr["AccountNumber"].ToString();
                    user.Password = dr["Password"].ToString();
                    user.FirstName = dr["FirstName"].ToString();
                    user.LastName = dr["LastName"].ToString();
                    user.Company = dr["Company"].ToString();
                    user.Nationality = dr["Nationality"].ToString();
                    user.EmailID = dr["EmailID"].ToString();
                    user.PhoneNumber = dr["PhoneNumber"].ToString();
                    user.CountryOfRecidence = dr["CountryOfRecidence"].ToString();
                    user.TelephoneNumber = dr["TelephoneNumber"].ToString();
                    user.PassportNumber = dr["PassportNumber"].ToString();
                    user.PassportExpiryDate = dr["PassportExpiryDate"].ToString();
                    user.Designation = dr["Designation"].ToString();
                    user.VisaExpiryDate = dr["VisaExpiryDate"].ToString();
                    user.InsuranceCompany = dr["InsuranceCompany"].ToString();
                    user.InsuranceExpiryDate = dr["InsuranceExpiryDate"].ToString();
                    user.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    user.IsDelete = Convert.ToBoolean(dr["IsDelete"].ToString());
                    user.CreatedOnUTC = dr["CreatedOnUTC"].ToString();
                    user.ModifyOnUTC = dr["ModifyOnUTC"].ToString();
                }
                con.Close();
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public int UpdateUser(UserEmployeeViewModel user)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand cmd = new SqlCommand("USP_Admin_UserOperation", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Status", Operation.Insert);
                cmd.Parameters.AddWithValue("@ID", user.Id);
                cmd.Parameters.AddWithValue("@AccountNumber", user.AccountNumber);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                cmd.Parameters.AddWithValue("@CompanyId", user.CompanyId);
                //cmd.Parameters.AddWithValue("@CompanyName", user.CompanyName);
                cmd.Parameters.AddWithValue("@Nationality", user.Nationality);
                cmd.Parameters.AddWithValue("@EmailID", user.EmailID);
                cmd.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                cmd.Parameters.AddWithValue("@CountryOfRecidence", user.CountryOfRecidence);
                //cmd.Parameters.AddWithValue("@CompanyAddress", user.CompanyAddress);
                cmd.Parameters.AddWithValue("@TelephoneNumber", user.TelephoneNumber);
                cmd.Parameters.AddWithValue("@RoleId", user.RoleId);
                con.Open();
                returnId = (int)cmd.ExecuteScalar();
                con.Close();
                return returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public int DeleteUser(int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand cmd = new SqlCommand("USP_Admin_UserOperation", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Status", Operation.Delete);
                cmd.Parameters.AddWithValue("@ID", Id);
                con.Open();
                returnId = (int)cmd.ExecuteScalar();
                con.Close();
                return returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public int CheckExistanceOfUserAccountNumber(string AccountNumber)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand cmd = new SqlCommand("USP_Admin_CheckExistingUserAccountNumber", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AccountNumber", AccountNumber);
                con.Open();
                returnId = (int)cmd.ExecuteScalar();
                con.Close();
                return returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public int CheckExistanceOfEmail(string Email)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand cmd = new SqlCommand("USP_Admin_CheckExistingEmail", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", Email);
                con.Open();
                returnId = (int)cmd.ExecuteScalar();
                con.Close();
                return returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public List<GetRoles> GetActiveRoles()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetRoles> roles = new List<GetRoles>();
                SqlCommand cmd = new SqlCommand("USP_Admin_GetActiveRoles", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    GetRoles role = new GetRoles();
                    role.RoleID = Convert.ToInt32(dr["RoleID"].ToString());
                    role.RoleName = dr["RoleName"].ToString();
                    role.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    role.IsDelete = Convert.ToBoolean(dr["IsDelete"].ToString());
                    role.CreatedOn = dr["CreatedOn"].ToString();
                    role.ModifyOn = dr["ModifyOn"].ToString();
                    if(!role.RoleName.Equals("Super Admin"))
                        roles.Add(role);
                }
                con.Close();
                return roles;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public List<GetActiveEmployees> GetAllActiveEmployees(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetActiveEmployees> getActiveEmployees = new List<GetActiveEmployees>();
                SqlCommand cmd = new SqlCommand("USP_GetActiveEmployeeByCompanyId_Cmp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    GetActiveEmployees activeEmployees = new GetActiveEmployees();
                    
                    activeEmployees.Id = Convert.ToInt32(reader["Id"].ToString());
                    activeEmployees.CompanyId = Convert.ToInt32(reader["CompanyId"].ToString());
                    activeEmployees.AccountNumber = reader["AccountNumber"].ToString();
                    activeEmployees.FirstName = reader["FirstName"].ToString();
                    activeEmployees.LastName = reader["LastName"].ToString();
                    activeEmployees.Nationality = reader["Nationality"].ToString();
                    activeEmployees.EmailID = reader["EmailID"].ToString();
                    activeEmployees.PhoneNumber = reader["PhoneNumber"].ToString();
                    activeEmployees.CountryOfRecidence = reader["CountryOfRecidence"].ToString();
                    activeEmployees.TelephoneNumber = reader["TelephoneNumber"].ToString();
                    activeEmployees.Company = reader["CompanyName"].ToString();
                    activeEmployees.PassportNumber = reader["PassportNumber"].ToString();
                    activeEmployees.Designation = reader["Designation"].ToString();
                    activeEmployees.Role = reader["RoleName"].ToString();
                    activeEmployees.VisaExpiryDate = reader["VisaExpiryDate"].ToString();
                    activeEmployees.PassportExpiryDate = reader["PassportExpiryDate"].ToString();
                    activeEmployees.InsuranceCompany = reader["InsuranceCompany"].ToString();
                    activeEmployees.InsuranceExpiryDate = reader["InsuranceExpiryDate"].ToString();
                    activeEmployees.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    activeEmployees.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    activeEmployees.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    activeEmployees.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    getActiveEmployees.Add(activeEmployees);
                }
                con.Close();
                return getActiveEmployees;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public List<GetInActiveEmployees> GetAllInActiveEmployees(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetInActiveEmployees> getInActiveEmployees = new List<GetInActiveEmployees>();
                SqlCommand cmd = new SqlCommand("USP_GetInActiveEmployeeByCompanyId_Cmp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    GetInActiveEmployees InactiveEmployees = new GetInActiveEmployees();

                    InactiveEmployees.Id = Convert.ToInt32(reader["Id"].ToString());
                    InactiveEmployees.CompanyId = Convert.ToInt32(reader["CompanyId"].ToString());
                    InactiveEmployees.AccountNumber = reader["AccountNumber"].ToString();
                    InactiveEmployees.FirstName = reader["FirstName"].ToString();
                    InactiveEmployees.LastName = reader["LastName"].ToString();
                    InactiveEmployees.Nationality = reader["Nationality"].ToString();
                    InactiveEmployees.EmailID = reader["EmailID"].ToString();
                    InactiveEmployees.PhoneNumber = reader["PhoneNumber"].ToString();
                    InactiveEmployees.CountryOfRecidence = reader["CountryOfRecidence"].ToString();
                    InactiveEmployees.TelephoneNumber = reader["TelephoneNumber"].ToString();
                    InactiveEmployees.Company= reader["CompanyName"].ToString();
                    InactiveEmployees.PassportNumber= reader["PassportNumber"].ToString();
                    InactiveEmployees.Designation= reader["Designation"].ToString();
                    InactiveEmployees.Role = reader["RoleName"].ToString();
                    InactiveEmployees.VisaExpiryDate= reader["VisaExpiryDate"].ToString();
                    InactiveEmployees.PassportExpiryDate= reader["PassportExpiryDate"].ToString();
                    InactiveEmployees.InsuranceCompany= reader["InsuranceCompany"].ToString();
                    InactiveEmployees.InsuranceExpiryDate= reader["InsuranceExpiryDate"].ToString();
                    InactiveEmployees.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    InactiveEmployees.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    InactiveEmployees.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    InactiveEmployees.ModifyOnUtc = reader["ModifyOnUtc"].ToString();

                    getInActiveEmployees.Add(InactiveEmployees);
                }
                con.Close();
                return getInActiveEmployees;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public int GetCountOfManagers(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int countOfManagers = 0;
                SqlCommand cmd = new SqlCommand("USP_Admin_GetCountofManager", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                countOfManagers = (int)cmd.ExecuteScalar();
                con.Close();
                return countOfManagers;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public int GetCountOfEmployees(int CompanyId, int UserId) /*,int UserId*/
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int countOfEmployee = 0;

                SqlCommand cmd = null;

                cmd = new SqlCommand("USP_Admin_CheckEmployeeCountByCompanyId", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                con.Open();
                int _result = (int)cmd.ExecuteScalar();
                con.Close();

                if (_result > 0)
                {
                    cmd = new SqlCommand("USP_Admin_GetCompanyEmployeeCount", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                    con.Open();
                    countOfEmployee = (int)cmd.ExecuteScalar();
                    con.Close();
                }
                else
                {
                    countOfEmployee = 1;
                }


                return countOfEmployee;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public int GetCountOfShareholders(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int countOfShareholders = 0;
                SqlCommand cmd = new SqlCommand("USP_Admin_GetCompanyShareholderCount", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                countOfShareholders = (int)cmd.ExecuteScalar();
                con.Close();
                return countOfShareholders;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public DataSet GetEmployeesForExport(int ID)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand("USP_Admin_GetAllEmployeesDetailsForExport_Cmp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId", ID);
                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds);
                con.Close();
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public LoggedinStatus GetLoggedinStatus(int CompanyId, string Role, string LoggedInUser)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                LoggedinStatus model = new LoggedinStatus();
                Regex re = new Regex(@"(@)(.+)$");
                Match result = re.Match(LoggedInUser);
                SqlCommand command = new SqlCommand("USP_Admin_CheckLoggedInUserStatus", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@Role", Role);
                if (result.Success)
                    command.Parameters.AddWithValue("@EmailId", LoggedInUser);
                else
                    command.Parameters.AddWithValue("@AccountNumber", LoggedInUser);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["IsDelete"] != DBNull.Value)
                        model.IsDelete = Convert.ToInt32(reader["IsDelete"]);
                    if (reader["IsActive"] != DBNull.Value)
                        model.IsActive = Convert.ToInt32(reader["IsActive"]);
                    if (reader["IsLoggedIn"] != DBNull.Value)
                        model.IsLoggedIn = Convert.ToInt32(reader["IsLoggedIn"]);
                }
                con.Close();
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
    }
}
