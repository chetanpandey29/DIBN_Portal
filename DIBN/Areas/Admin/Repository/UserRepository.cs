using DIBN.Areas.Admin.Data;
using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Mvc.Routing;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO.Pipelines;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static DIBN.Areas.Admin.Data.DataSetting;
using static DIBN.Areas.Admin.Models.UserViewModel;

namespace DIBN.Areas.Admin.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly Connection _dataSetting;
        private readonly EncryptionService _encryptionService;
        public UserRepository(Connection dataSetting, EncryptionService encryptionService)
        {
            _dataSetting = dataSetting;
            _encryptionService = encryptionService;
        }
        /// <summary>
        /// Get User List                                   -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<UserViewModel> GetUsers()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<UserViewModel> users = new List<UserViewModel>();
                SqlCommand cmd = new SqlCommand("USP_Admin_UserOperation", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Status", Operation.Show);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    UserViewModel user = new UserViewModel();
                    user.Id = Convert.ToInt32(dr["ID"].ToString());
                    user.AccountNumber = dr["AccountNumber"].ToString();
                    user.FirstName = dr["FirstName"].ToString();
                    user.LastName = dr["LastName"].ToString();
                    user.Company = dr["Company"].ToString();
                    user.Nationality = dr["Nationality"].ToString();
                    user.EmailID = dr["EmailID"].ToString();
                    user.PhoneNumber = dr["PhoneNumber"].ToString();
                    user.CountryOfRecidence = dr["CountryOfRecidence"].ToString();
                    user.TelephoneNumber = dr["TelephoneNumber"].ToString();
                    user.PassportNumber = dr["PassportNumber"].ToString();
                    user.Designation = dr["Designation"].ToString();
                    user.VisaExpiryDate = dr["VisaExpiryDate"].ToString();
                    user.InsuranceCompany = dr["InsuranceCompany"].ToString();
                    user.InsuranceExpiryDate = dr["InsuranceExpiryDate"].ToString();
                    user.PassportExpiryDate = dr["PassportExpiryDate"].ToString();
                    user.IsLogin = Convert.ToBoolean(dr["IsLogin"].ToString());
                    user.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    user.IsDelete = Convert.ToBoolean(dr["IsDelete"].ToString());
                    user.CreatedOnUTC = dr["CreatedOnUTC"].ToString();
                    user.ModifyOnUTC = dr["ModifyOnUTC"].ToString();
                    users.Add(user);
                }
                con.Close();
                return users;
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

        /// <summary>
        /// Create New User                                 -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int CreateUser(UserViewModel user)
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
                cmd.Parameters.AddWithValue("@MCode", user.MCountry);
                cmd.Parameters.AddWithValue("@Designation", user.Designation);
                cmd.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                cmd.Parameters.AddWithValue("@CountryOfRecidence", user.CountryOfRecidence);
                cmd.Parameters.AddWithValue("@TelephoneNumber", user.TelephoneNumber);
                cmd.Parameters.AddWithValue("@PassportNumber", user.PassportNumber);
                if (user.PassportExpiryDate != "N/A" && user.PassportExpiryDate != "")
                    cmd.Parameters.AddWithValue("@PassportExpiryDate", user.PassportExpiryDate);
                if (user.VisaExpiryDate != "N/A" && user.VisaExpiryDate != "")
                    cmd.Parameters.AddWithValue("@VisaExpiryDate", user.VisaExpiryDate);
                if (user.InsuranceCompany != "N/A" && user.InsuranceCompany != "")
                    cmd.Parameters.AddWithValue("@InsuranceCompany", user.InsuranceCompany);
                if (user.InsuranceExpiryDate != "N/A" && user.InsuranceCompany != "")
                    cmd.Parameters.AddWithValue("@InsuranceExpiryDate", user.InsuranceExpiryDate);
                if (user.EmployeeCardExpiryDate != "N/A" && user.EmployeeCardExpiryDate != "")
                    cmd.Parameters.AddWithValue("@EmployeementCardExpiryDate", user.EmployeeCardExpiryDate);
                cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                cmd.Parameters.AddWithValue("@IsLogin", user.IsLogin);
                cmd.Parameters.AddWithValue("@CreatedBy", user.CreatedBy);
                con.Open();
                returnId = (int)cmd.ExecuteScalar();
                con.Close();
                cmd.Parameters.Clear();
                if (returnId > 0)
                {
                    if (user.RoleId != null)
                    {
                        if (user.RoleId.Count > 0)
                        {
                            for (int index = 0; index < user.RoleId.Count; index++)
                            {
                                cmd = new SqlCommand("USP_Admin_UserRole", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@Status", Operation.Insert);
                                cmd.Parameters.AddWithValue("@RoleID", user.RoleId[index]);
                                cmd.Parameters.AddWithValue("@UserId", returnId);
                                cmd.Parameters.AddWithValue("@CreatedBy", user.CreatedBy);
                                con.Open();
                                int _returnRole = (int)cmd.ExecuteScalar();
                                con.Close();
                                cmd.Parameters.Clear();
                            }
                        }
                    }
                }
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

        /// <summary>
        /// Create New Employee for any Company                 -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int CreateEmployee(SaveNewUser user)
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
                cmd.Parameters.AddWithValue("@MCode", user.MCountry);
                cmd.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                cmd.Parameters.AddWithValue("@IsLogin", user.IsLogin);
                cmd.Parameters.AddWithValue("@CreatedBy", user.CreatedBy);
                con.Open();
                returnId = (int)cmd.ExecuteScalar();
                con.Close();
                cmd.Parameters.Clear();
                if (returnId > 0)
                {
                    if (user.RoleId != null)
                    {
                        if (user.RoleId.Count > 0)
                        {
                            for (int index = 0; index < user.RoleId.Count; index++)
                            {
                                cmd = new SqlCommand("USP_Admin_UserRole", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@Status", Operation.Insert);
                                cmd.Parameters.AddWithValue("@RoleID", user.RoleId[index]);
                                cmd.Parameters.AddWithValue("@UserId", returnId);
                                cmd.Parameters.AddWithValue("@CreatedBy", user.CreatedBy);

                                con.Open();
                                int _returnRole = (int)cmd.ExecuteScalar();
                                con.Close();
                                cmd.Parameters.Clear();
                            }
                        }
                    }
                }
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

        /// <summary>
        /// Get Last Account Number of User/Employee                -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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

        /// <summary>
        /// Get Complete Detail of Any User By Id                   -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public UserViewModel GetUserDetail(int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                UserViewModel user = new UserViewModel();
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
                    user.Password = _encryptionService.DecryptText(user.Password);
                    user.FirstName = dr["FirstName"].ToString();
                    user.LastName = dr["LastName"].ToString();
                    user.CompanyId = Convert.ToInt32(dr["CompanyId"].ToString());
                    user.Company = dr["Company"].ToString();
                    user.CompanyType = dr["CompanyType"].ToString();
                    user.Nationality = dr["Nationality"].ToString();
                    user.EmailID = dr["EmailID"].ToString();
                    user.MCountry = dr["MCode"].ToString();
                    user.PhoneNumber = dr["PhoneNumber"].ToString();
                    user.CountryOfRecidence = dr["CountryOfRecidence"].ToString();
                    user.TelephoneNumber = dr["TelephoneNumber"].ToString();
                    user.Designation = dr["Designation"].ToString();
                    user.Role = dr["RoleName"].ToString();
                    //user.RoleId = Convert.ToInt32(dr["RoleId"].ToString());
                    user.PassportNumber = dr["PassportNumber"].ToString();
                    user.VisaExpiryDate = dr["VisaExpiryDate"].ToString();
                    user.InsuranceCompany = dr["InsuranceCompany"].ToString();
                    user.InsuranceExpiryDate = dr["InsuranceExpiryDate"].ToString();
                    user.EmployeeCardExpiryDate = dr["EmployeementCardExpiryDate"].ToString();
                    user.PassportExpiryDate = dr["PassportExpiryDate"].ToString();
                    user.IsLogin = Convert.ToBoolean(dr["IsLogin"].ToString());
                    user.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    user.IsDelete = Convert.ToBoolean(dr["IsDelete"].ToString());
                    user.CreatedOnUTC = dr["CreatedOnUTC"].ToString();
                    user.ModifyOnUTC = dr["ModifyOnUTC"].ToString();
                }
                user.RoleId = new List<int>();
                con.Close();
                cmd.Parameters.Clear();
                cmd = new SqlCommand("USP_Admin_UserRole", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Status", Operation.Show);
                cmd.Parameters.AddWithValue("@UserId", Id);
                con.Open();
                dr.Close();
                dr = cmd.ExecuteReader();
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        int RoleId = Convert.ToInt32(dr["RoleId"]);
                        user.RoleId.Add(RoleId);
                    }
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

        /// <summary>
        /// Update User Details                                     -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int UpdateUser(UserViewModel user)
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
                cmd.Parameters.AddWithValue("@Nationality", user.Nationality);
                cmd.Parameters.AddWithValue("@EmailID", user.EmailID);
                cmd.Parameters.AddWithValue("@MCode", user.MCountry);
                cmd.Parameters.AddWithValue("@Designation", user.Designation);
                cmd.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                if (user.Nationality != "N/A" && user.Nationality != "")
                    cmd.Parameters.AddWithValue("@CountryOfRecidence", user.CountryOfRecidence);
                if (user.TelephoneNumber != "N/A" && user.TelephoneNumber != "")
                    cmd.Parameters.AddWithValue("@TelephoneNumber", user.TelephoneNumber);
                if (user.PassportNumber != "N/A" && user.PassportNumber != "")
                    cmd.Parameters.AddWithValue("@PassportNumber", user.PassportNumber);
                if (user.PassportExpiryDate != "N/A" && user.PassportExpiryDate != "")
                    cmd.Parameters.AddWithValue("@PassportExpiryDate", user.PassportExpiryDate);
                if (user.VisaExpiryDate != "N/A" && user.VisaExpiryDate != "")
                    cmd.Parameters.AddWithValue("@VisaExpiryDate", user.VisaExpiryDate);
                if (user.InsuranceCompany != "N/A" && user.InsuranceCompany != "")
                    cmd.Parameters.AddWithValue("@InsuranceCompany", user.InsuranceCompany);
                if (user.InsuranceExpiryDate != "N/A" && user.InsuranceExpiryDate != "")
                    cmd.Parameters.AddWithValue("@InsuranceExpiryDate", user.InsuranceExpiryDate);
                if (user.EmployeeCardExpiryDate != "N/A" && user.EmployeeCardExpiryDate != "")
                    cmd.Parameters.AddWithValue("@EmployeementCardExpiryDate", user.EmployeeCardExpiryDate);
                cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@IsLogin", user.IsLogin);
                cmd.Parameters.AddWithValue("@CreatedBy", user.CreatedBy);
                con.Open();
                returnId = (int)cmd.ExecuteScalar();
                con.Close();
                cmd.Parameters.Clear();

                if (user.RoleId != null)
                {
                    if (user.RoleId.Count > 0)
                    {
                        cmd.Parameters.Clear();
                        cmd = new SqlCommand("USP_Admin_RemoveUserRoleMapping", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@userId", user.CreatedBy);
                        cmd.Parameters.AddWithValue("@id", user.Id);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();

                        Log.Information("Previously Assigned Role of " + user.FirstName + " " + user.LastName + " ( " + user.AccountNumber + " ) is Deleted Before adding new Assigned Roles");
                        cmd.Parameters.Clear();

                        for (int index = 0; index < user.RoleId.Count; index++)
                        {
                            cmd = new SqlCommand("USP_Admin_UserRole", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Status", Operation.Insert);
                            cmd.Parameters.AddWithValue("@RoleID", user.RoleId[index]);
                            cmd.Parameters.AddWithValue("@UserId", returnId);
                            cmd.Parameters.AddWithValue("@CreatedBy", user.CreatedBy);
                            con.Open();
                            int _returnRole = (int)cmd.ExecuteScalar();
                            con.Close();
                            cmd.Parameters.Clear();
                            Log.Information("New Assigned Role for " + user.FirstName + " " + user.LastName + " ( " + user.AccountNumber + " ) is added successfully.");
                        }
                    }
                }
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

        /// <summary>
        /// Delete Company User                                     -- Yashasvi TBC (26-11-2022)
        /// If User's Role is not Company Owner it will not Allow us to Delete that User if we only have one Company Owner assigned to this Company.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="mainCompany"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int DeleteUser(int Id, int? mainCompany, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand cmd = null;
                List<string> list = new List<string>();

                string Query = "";

                cmd = new SqlCommand("USP_Admin_GetAssignedRoleListByUserId", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", Id);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(reader["RoleName"].ToString());
                }
                con.Close();

                cmd.Parameters.Clear();
                cmd = new SqlCommand("USP_Admin_GetAssignedCompanyIdByUserId", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", Id);

                con.Open();
                int CompanyId = (int)cmd.ExecuteScalar();
                con.Close();

                cmd.Parameters.Clear();
                cmd = new SqlCommand("USP_Admin_GetAssignedUserByCompanyId", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);

                con.Open();
                List<int> users = new List<int>();
                SqlDataReader reader1 = cmd.ExecuteReader();
                while (reader1.Read())
                {
                    if (users.Count > 0)
                    {
                        if (!users.Contains(Convert.ToInt32(reader1["Id"])))
                        {
                            users.Add(Convert.ToInt32(reader1["Id"]));
                        }
                    }
                    else
                    {
                        users.Add(Convert.ToInt32(reader1["Id"]));
                    }
                }
                con.Close();

                if (users.Count > 1)
                {
                    cmd.Parameters.Clear();
                    cmd = new SqlCommand("USP_Admin_UserOperation", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Status", Operation.Delete);
                    cmd.Parameters.AddWithValue("@ID", Id);
                    cmd.Parameters.AddWithValue("@CreatedBy", UserId);
                    con.Open();
                    returnId = (int)cmd.ExecuteScalar();
                    con.Close();
                }
                else if (!list.Contains("Company Owner") || Convert.ToInt32(mainCompany) > 0)
                {
                    cmd.Parameters.Clear();
                    cmd = new SqlCommand("USP_Admin_UserOperation", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Status", Operation.Delete);
                    cmd.Parameters.AddWithValue("@ID", Id);
                    cmd.Parameters.AddWithValue("@CreatedBy", UserId);
                    con.Open();
                    returnId = (int)cmd.ExecuteScalar();
                    con.Close();
                }
                else
                {
                    returnId = -1;
                }

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

        /// <summary>
        /// Check Whether added Account Number is already Exists or Not             -- Yashasvi TBC (26-11-2022)
        /// Currently Removed by @Sunil Sir as they want to assign Same Account Number to Company as well as Company Owner.
        /// </summary>
        /// <param name="AccountNumber"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int CheckExistanceOfUserAccountNumber(string AccountNumber,int? userId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                if (AccountNumber != null)
                {
                    SqlCommand cmd = new SqlCommand("USP_Admin_CheckExistingUserAccountNumber", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AccountNumber", AccountNumber);
                    if(userId!=null)
                        cmd.Parameters.AddWithValue("@userId", userId.Value);
                    con.Open();
                    returnId = (int)cmd.ExecuteScalar();
                    con.Close();
                }

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

        /// <summary>
        /// Check whether added Email Address is already exists or not                          -- Yashasvi TBC (26-11-2022)
        /// Currently Removed by @Sunil Sir as They want to add Same Email Address for Company as well as Company Owner / Employee.
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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

        /// <summary>
        /// Create User For Any Company (At the time of Creating Any Company)                   -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int CreateUserForCompany(SaveUser user)
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
                cmd.Parameters.AddWithValue("@MCode", user.MCountry);
                cmd.Parameters.AddWithValue("@Password", _Password);
                cmd.Parameters.AddWithValue("@Nationality", user.Nationality);
                cmd.Parameters.AddWithValue("@EmailID", user.EmailID);
                cmd.Parameters.AddWithValue("@Designation", user.Designation);
                cmd.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                cmd.Parameters.AddWithValue("@CountryOfRecidence", user.CountryOfRecidence);
                cmd.Parameters.AddWithValue("@TelephoneNumber", user.TelephoneNumber);
                if (user.PassportNumber != "N/A" && user.PassportNumber != "")
                    cmd.Parameters.AddWithValue("@PassportNumber", user.PassportNumber);
                if (user.PassportExpiryDate != "N/A" && user.PassportExpiryDate != "")
                    cmd.Parameters.AddWithValue("@PassportExpiryDate", user.PassportExpiryDate);
                if (user.VisaExpiryDate != "N/A" && user.VisaExpiryDate != "")
                    cmd.Parameters.AddWithValue("@VisaExpiryDate", user.VisaExpiryDate);
                if (user.InsuranceCompany != "N/A" && user.InsuranceCompany != "")
                    cmd.Parameters.AddWithValue("@InsuranceCompany", user.InsuranceCompany);
                if (user.InsuranceExpiryDate != "N/A" && user.InsuranceExpiryDate != "")
                    cmd.Parameters.AddWithValue("@InsuranceExpiryDate", user.InsuranceExpiryDate);
                if (user.EmployeeCardExpiryDate != "N/A" && user.EmployeeCardExpiryDate != "")
                    cmd.Parameters.AddWithValue("@EmployeementCardExpiryDate", user.EmployeeCardExpiryDate);
                cmd.Parameters.AddWithValue("@IsLogin", user.IsLogin);
                cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                cmd.Parameters.AddWithValue("@CreatedBy", user.CreatedBy);
                if (user.CompanyId != 0)
                {
                    cmd.Parameters.AddWithValue("CompanyId", user.CompanyId);
                }
                con.Open();
                returnId = (int)cmd.ExecuteScalar();
                con.Close();
                cmd.Parameters.Clear();
                if (user.RoleId != null)
                {
                    if (user.RoleId.Count > 0)
                    {
                        for (int index = 0; index < user.RoleId.Count; index++)
                        {
                            cmd = new SqlCommand("USP_Admin_UserRole", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Status", Operation.Insert);
                            cmd.Parameters.AddWithValue("@RoleID", user.RoleId[index]);
                            cmd.Parameters.AddWithValue("@UserId", returnId);
                            cmd.Parameters.AddWithValue("@CreatedBy", user.CreatedBy);
                            con.Open();
                            int _returnRole = (int)cmd.ExecuteScalar();
                            con.Close();
                            cmd.Parameters.Clear();
                        }
                    }
                }
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
        /// <summary>
        /// Add New Company for User                                                            -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int AddNewCompanyForUser(SaveCompanyForUser company)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                var _Password = _encryptionService.EncryptText(company.CompanyPassword);
                SqlCommand command = new SqlCommand("USP_Admin_CompanyOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Insert);
                if (company.UserId != 0)
                {
                    command.Parameters.AddWithValue("@UserId", company.UserId);
                }
                command.Parameters.AddWithValue("@DIBNUserNumber", company.DIBNUserNumber);
                command.Parameters.AddWithValue("@CompanyName", company.CompanyName);
                command.Parameters.AddWithValue("@AccountNumber", company.AccountNumber);
                command.Parameters.AddWithValue("@MCountryCode", company.MainContactNumberCountry);
                command.Parameters.AddWithValue("@MobileNumber", company.MobileNumber);
                command.Parameters.AddWithValue("@ECountryCode", company.EmergencyContactNumberCountry);
                command.Parameters.AddWithValue("@EmergencyNumber", company.EmergencyNumber);
                command.Parameters.AddWithValue("@EmailID", company.EmailID);
                command.Parameters.AddWithValue("@City", company.City);
                command.Parameters.AddWithValue("@SecondEmailID", company.SecondEmailID);
                command.Parameters.AddWithValue("@CompanyType", company.CompanyTypeName);
                command.Parameters.AddWithValue("@CompanyStartingDate", company.CompanyStartingDate);
                command.Parameters.AddWithValue("@Cmp_pass", _Password);
                command.Parameters.AddWithValue("@IsActive", company.IsActive);
                command.Parameters.AddWithValue("@Country", company.Country);
                if (company.LabourFileNo != null)
                    command.Parameters.AddWithValue("@LabourFileNo", company.LabourFileNo);
                command.Parameters.AddWithValue("@CreatedBy", company.CreatedBy);
                con.Open();
                returnId = (int)command.ExecuteScalar();
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

        /// <summary>
        /// Get Assigned Company Id by User Id                                                  -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetCompanyId(int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int CompanyId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_GetCompanyId", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", UserId);
                con.Open();
                CompanyId = (int)command.ExecuteScalar();
                con.Close();
                return CompanyId;
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

        /// <summary>
        /// Get All In-Active Company Owner / Employee List                                     -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<GetInActiveEmployees> GetAllInActiveEmployees(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetInActiveEmployees> getInActiveEmployees = new List<GetInActiveEmployees>();
                List<GetInActiveEmployees> _allgetInActiveEmployees = new List<GetInActiveEmployees>();
                SqlCommand cmd = new SqlCommand("USP_Admin_GetInActiveEmployee", con);
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    GetInActiveEmployees InactiveEmployees = new GetInActiveEmployees();

                    InactiveEmployees.Id = Convert.ToInt32(reader["Id"].ToString());
                    InactiveEmployees.AccountNumber = reader["AccountNumber"].ToString();
                    InactiveEmployees.FirstName = reader["FirstName"].ToString();
                    InactiveEmployees.LastName = reader["LastName"].ToString();
                    InactiveEmployees.Nationality = reader["Nationality"].ToString();
                    InactiveEmployees.EmailID = reader["EmailID"].ToString();
                    InactiveEmployees.PhoneNumber = reader["PhoneNumber"].ToString();
                    InactiveEmployees.CountryOfRecidence = reader["CountryOfRecidence"].ToString();
                    InactiveEmployees.TelephoneNumber = reader["TelephoneNumber"].ToString();
                    InactiveEmployees.PassportNumber = reader["PassportNumber"].ToString();
                    InactiveEmployees.VisaExpiryDate = reader["VisaExpiryDate"].ToString();
                    InactiveEmployees.Designation = reader["Designation"].ToString();
                    InactiveEmployees.Role = reader["RoleName"].ToString();
                    InactiveEmployees.InsuranceCompany = reader["InsuranceCompany"].ToString();
                    InactiveEmployees.InsuranceExpiryDate = reader["InsuranceExpiryDate"].ToString();
                    InactiveEmployees.PassportExpiryDate = reader["PassportExpiryDate"].ToString();
                    InactiveEmployees.EmployeementCardExpiryDate = reader["EmployeementCardExpiryDate"].ToString();
                    InactiveEmployees.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    InactiveEmployees.IsLogin = Convert.ToBoolean(reader["IsLogin"]);
                    InactiveEmployees.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    InactiveEmployees.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    InactiveEmployees.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    InactiveEmployees.Company = reader["CompanyName"].ToString();

                    getInActiveEmployees.Add(InactiveEmployees);
                }
                con.Close();
                reader.Close();
                //cmd.Parameters.Clear();
                //SqlCommand command = new SqlCommand("USP_Admin_CheckEmployeeForMainCompany", con);
                //command.CommandType = CommandType.StoredProcedure;
                //con.Open();
                //if (_allgetInActiveEmployees.Count > 0)
                //{
                //    for (int index = 0; index < _allgetInActiveEmployees.Count; index++)
                //    {
                //        command.Parameters.AddWithValue("@UserId", _allgetInActiveEmployees[index].Id);
                //        int _returnId = (int)command.ExecuteScalar();
                //        if (_returnId > 0)
                //        {
                //            getInActiveEmployees.Add(_allgetInActiveEmployees[index]);
                //        }
                //        command.Parameters.Clear();
                //    }
                //}
                //con.Close();
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

        /// <summary>
        /// Get All Active Company Owner / Employee List                                        -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<GetActiveEmployees> GetAllActiveEmployees(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetActiveEmployees> getActiveEmployees = new List<GetActiveEmployees>();
                List<GetActiveEmployees> _allgetActiveEmployees = new List<GetActiveEmployees>();
                SqlCommand cmd = new SqlCommand("USP_Admin_GetActiveEmployee", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    GetActiveEmployees ActiveEmployees = new GetActiveEmployees();

                    ActiveEmployees.Id = Convert.ToInt32(reader["Id"].ToString());
                    ActiveEmployees.AccountNumber = reader["AccountNumber"].ToString();
                    ActiveEmployees.FirstName = reader["FirstName"].ToString();
                    ActiveEmployees.LastName = reader["LastName"].ToString();
                    ActiveEmployees.Nationality = reader["Nationality"].ToString();
                    ActiveEmployees.EmailID = reader["EmailID"].ToString();
                    ActiveEmployees.PhoneNumber = reader["PhoneNumber"].ToString();
                    ActiveEmployees.CountryOfRecidence = reader["CountryOfRecidence"].ToString();
                    ActiveEmployees.TelephoneNumber = reader["TelephoneNumber"].ToString();
                    ActiveEmployees.Role = reader["RoleName"].ToString();
                    ActiveEmployees.Designation = reader["Designation"].ToString();
                    ActiveEmployees.PassportNumber = reader["PassportNumber"].ToString();
                    ActiveEmployees.PassportExpiryDate = reader["PassportExpiryDate"].ToString();
                    ActiveEmployees.VisaExpiryDate = reader["VisaExpiryDate"].ToString();
                    ActiveEmployees.InsuranceCompany = reader["InsuranceCompany"].ToString();
                    ActiveEmployees.InsuranceExpiryDate = reader["InsuranceExpiryDate"].ToString();
                    ActiveEmployees.EmployeementCardExpiryDate = reader["EmployeementCardExpiryDate"].ToString();
                    ActiveEmployees.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    ActiveEmployees.IsLogin = Convert.ToBoolean(reader["IsLogin"]);
                    ActiveEmployees.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    ActiveEmployees.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    ActiveEmployees.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    ActiveEmployees.Company = reader["CompanyName"].ToString();

                    getActiveEmployees.Add(ActiveEmployees);
                }
                reader.Close();
                con.Close();
                return getActiveEmployees;
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

        /// <summary>
        /// Get All In-Active Company Owner / Employees Company Wise                                            -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<GetInActiveEmployees> GetInActiveEmployeesCompanyWise(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetInActiveEmployees> getInActiveEmployees = new List<GetInActiveEmployees>();
                List<GetInActiveEmployees> _allgetInActiveEmployees = new List<GetInActiveEmployees>();
                SqlCommand cmd = new SqlCommand("USP_Admin_GetInActiveEmployee", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    GetInActiveEmployees InactiveEmployees = new GetInActiveEmployees();

                    InactiveEmployees.Id = Convert.ToInt32(reader["Id"].ToString());
                    InactiveEmployees.AccountNumber = reader["AccountNumber"].ToString();
                    InactiveEmployees.FirstName = reader["FirstName"].ToString();
                    InactiveEmployees.LastName = reader["LastName"].ToString();
                    InactiveEmployees.Nationality = reader["Nationality"].ToString();
                    InactiveEmployees.EmailID = reader["EmailID"].ToString();
                    InactiveEmployees.PhoneNumber = reader["PhoneNumber"].ToString();
                    InactiveEmployees.CountryOfRecidence = reader["CountryOfRecidence"].ToString();
                    InactiveEmployees.TelephoneNumber = reader["TelephoneNumber"].ToString();
                    InactiveEmployees.PassportNumber = reader["PassportNumber"].ToString();
                    InactiveEmployees.VisaExpiryDate = reader["VisaExpiryDate"].ToString();
                    InactiveEmployees.Designation = reader["Designation"].ToString();
                    InactiveEmployees.Role = reader["RoleName"].ToString();
                    InactiveEmployees.InsuranceCompany = reader["InsuranceCompany"].ToString();
                    InactiveEmployees.InsuranceExpiryDate = reader["InsuranceExpiryDate"].ToString();
                    InactiveEmployees.PassportExpiryDate = reader["PassportExpiryDate"].ToString();
                    InactiveEmployees.EmployeementCardExpiryDate = reader["EmployeementCardExpiryDate"].ToString();
                    InactiveEmployees.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    InactiveEmployees.IsLogin = Convert.ToBoolean(reader["IsLogin"]);
                    InactiveEmployees.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    InactiveEmployees.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    InactiveEmployees.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    InactiveEmployees.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    InactiveEmployees.Company = reader["CompanyName"].ToString();
                    _allgetInActiveEmployees.Add(InactiveEmployees);
                }
                con.Close();
                reader.Close();
                cmd.Parameters.Clear();
                SqlCommand command = new SqlCommand("USP_Admin_CheckEmployeeForMainCompany", con);
                command.CommandType = CommandType.StoredProcedure;
                con.Open();
                if (_allgetInActiveEmployees.Count > 0)
                {
                    for (int index = 0; index < _allgetInActiveEmployees.Count; index++)
                    {
                        command.Parameters.AddWithValue("@UserId", _allgetInActiveEmployees[index].Id);
                        int _returnId = (int)command.ExecuteScalar();
                        if (_returnId > 0)
                        {
                            getInActiveEmployees.Add(_allgetInActiveEmployees[index]);
                        }
                        command.Parameters.Clear();
                    }
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

        /// <summary>
        /// Get Active Company Owner / Employees Company Wise                                                   -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<GetActiveEmployees> GetActiveEmployeesCompanyWise(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetActiveEmployees> getActiveEmployees = new List<GetActiveEmployees>();
                List<GetActiveEmployees> _allgetActiveEmployees = new List<GetActiveEmployees>();
                SqlCommand cmd = new SqlCommand("USP_GetActiveEmployeeByCompanyId", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    GetActiveEmployees ActiveEmployees = new GetActiveEmployees();

                    ActiveEmployees.Id = Convert.ToInt32(reader["Id"].ToString());
                    ActiveEmployees.AccountNumber = reader["AccountNumber"].ToString();
                    ActiveEmployees.FirstName = reader["FirstName"].ToString();
                    ActiveEmployees.LastName = reader["LastName"].ToString();
                    ActiveEmployees.Nationality = reader["Nationality"].ToString();
                    ActiveEmployees.EmailID = reader["EmailID"].ToString();
                    ActiveEmployees.PhoneNumber = reader["PhoneNumber"].ToString();
                    ActiveEmployees.CountryOfRecidence = reader["CountryOfRecidence"].ToString();
                    ActiveEmployees.TelephoneNumber = reader["TelephoneNumber"].ToString();
                    ActiveEmployees.Designation = reader["Designation"].ToString();
                    ActiveEmployees.Role = reader["RoleName"].ToString();
                    ActiveEmployees.PassportNumber = reader["PassportNumber"].ToString();
                    ActiveEmployees.PassportExpiryDate = reader["PassportExpiryDate"].ToString();
                    ActiveEmployees.VisaExpiryDate = reader["VisaExpiryDate"].ToString();
                    ActiveEmployees.InsuranceCompany = reader["InsuranceCompany"].ToString();
                    ActiveEmployees.InsuranceExpiryDate = reader["InsuranceExpiryDate"].ToString();
                    ActiveEmployees.EmployeementCardExpiryDate = reader["EmployeementCardExpiryDate"].ToString();
                    ActiveEmployees.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    ActiveEmployees.IsLogin = Convert.ToBoolean(reader["IsLogin"]);
                    ActiveEmployees.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    ActiveEmployees.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    ActiveEmployees.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    ActiveEmployees.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    ActiveEmployees.Company = reader["CompanyName"].ToString();

                    _allgetActiveEmployees.Add(ActiveEmployees);
                }
                reader.Close();
                con.Close();

                cmd.Parameters.Clear();
                SqlCommand command = new SqlCommand("USP_Admin_CheckEmployeeForMainCompany", con);
                command.CommandType = CommandType.StoredProcedure;
                con.Open();
                if (_allgetActiveEmployees.Count > 0)
                {
                    for (int index = 0; index < _allgetActiveEmployees.Count; index++)
                    {
                        command.Parameters.AddWithValue("@UserId", _allgetActiveEmployees[index].Id);
                        int _returnId = (int)command.ExecuteScalar();
                        if (_returnId > 0)
                        {
                            getActiveEmployees.Add(_allgetActiveEmployees[index]);
                        }
                        command.Parameters.Clear();
                    }
                }
                con.Close();
                return getActiveEmployees;
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

        /// <summary>
        /// Get Employee Count                                                                                  -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<int> GetEmployeesCount()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int totalemp=0,totalMainemp=0;
                List<int> CountofEmployee = new List<int>();
                SqlCommand cmd = null;
                cmd = new SqlCommand("USP_Admin_GetEmployeeCountForAdminDashboard", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["companyOwners"] != DBNull.Value)
                        totalMainemp = Convert.ToInt32(reader["companyOwners"]);
                }
                CountofEmployee.Add(totalMainemp);
                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["employees"] != DBNull.Value)
                        totalemp = Convert.ToInt32(reader["employees"]);
                }
                CountofEmployee.Add(totalemp);
                con.Close();
                var total = totalemp + totalMainemp;
                CountofEmployee.Add(total);
                return CountofEmployee;
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

        /// <summary>
        /// Get Employees List for Export                                                                       -- Yashasvi TBC (26-11-2022)
        /// If we pass Company Id it will only show that Company's User 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DataSet GetEmployeesForExport(int? ID)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand("USP_Admin_GetAllEmployeesDetailsForExport", con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (ID != null)
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

        /// <summary>
        /// Export Main Company Employee's List                                                                                                  -- Yashasvi TBC (30-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DataSet GetMainCompanyEmployeesForExport()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand("USP_Admin_GetAllEmployeesDetailsForExportMainCompany", con);
                cmd.CommandType = CommandType.StoredProcedure;
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
        /// <summary>
        /// Get Main Company Employee List                                                                      -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<GetMainCompanyEmployees> GetMainCompanyEmployees(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetMainCompanyEmployees> users = new List<GetMainCompanyEmployees>();
                SqlCommand cmd = new SqlCommand("USP_GetMainCompanyEmployeesByCompanyId", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["AccountNumber"].ToString() != "TBC01")
                    {
                        GetMainCompanyEmployees model = new GetMainCompanyEmployees();
                        model.Id = Convert.ToInt32(reader["Id"]);
                        model.FirstName = reader["FirstName"].ToString();
                        model.LastName = reader["LastName"].ToString();
                        model.AccountNumber = reader["AccountNumber"].ToString();
                        model.Nationality = reader["Nationality"].ToString();
                        model.EmailID = reader["EmailID"].ToString();
                        model.PhoneNumber = reader["PhoneNumber"].ToString();
                        model.CountryOfRecidence = reader["CountryOfRecidence"].ToString();
                        model.TelephoneNumber = reader["TelephoneNumber"].ToString();
                        model.PassportNumber = reader["PassportNumber"].ToString();
                        model.Designation = reader["Designation"].ToString();
                        model.PassportExpiryDate = reader["PassportExpiryDate"].ToString();
                        model.VisaExpiryDate = reader["VisaExpiryDate"].ToString();
                        model.InsuranceCompany = reader["InsuranceCompany"].ToString();
                        model.InsuranceExpiryDate = reader["InsuranceExpiryDate"].ToString();
                        model.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                        model.Company = reader["Company"].ToString();
                        model.IsActive = Convert.ToBoolean(reader["IsActive"]);
                        model.IsLogin = Convert.ToBoolean(reader["IsLogin"]);
                        model.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                        model.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                        model.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                        users.Add(model);
                    }

                }
                con.Close();
                return users;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {

            }
        }
        /// <summary>
        /// Get Assigned User name's to Company                                                                 -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<KeyValuePair<string, string>> GetUserNamesForAssign(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>();
                SqlCommand cmd = new SqlCommand("USP_Admin_GetAssignedUserNamesForCompany", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["Id"] != DBNull.Value && reader["Username"] != DBNull.Value)
                        values.Add(new KeyValuePair<string, string>(reader["Username"].ToString(), Convert.ToString(Convert.ToInt32(reader["Id"]))));
                }
                return values;
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

        /// <summary>
        /// Get User List of Passed Company Id                                                                  -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<UserListForCompany> GetUserListForCompany(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<UserListForCompany> users = new List<UserListForCompany>();
                List<UserListForCompany> _allUsers = new List<UserListForCompany>();
                SqlCommand cmd = new SqlCommand("USP_Admin_UserListForCompany", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Status", Operation.Insert);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    UserListForCompany user = new UserListForCompany();
                    user.Id = Convert.ToInt32(dr["Id"].ToString());
                    user.Username = Convert.ToString(dr["Username"].ToString());
                    user.AccountNumber = Convert.ToString(dr["AccountNumber"].ToString());
                    users.Add(user);
                }
                dr.Close();
                con.Close();
                cmd.Parameters.Clear();

                //cmd = new SqlCommand("USP_Admin_UserListForCompany", con);
                //cmd.CommandType = CommandType.StoredProcedure;
                //if (_allUsers.Count > 0)
                //{
                //    for(int index=0;index<_allUsers.Count;index++)
                //    {
                //        int _returnId = 0;
                //        cmd.Parameters.AddWithValue("@Status", Operation.Show);
                //        cmd.Parameters.AddWithValue("@UserId", _allUsers[index].Id);
                //        cmd.Parameters.AddWithValue("@CompanyId", CompanyId);

                //        con.Open();
                //        _returnId = (int) cmd.ExecuteScalar();
                //        con.Close();

                //        if(_returnId > 0)
                //        {
                //            users.Add(_allUsers[index]);
                //        }
                //        cmd.Parameters.Clear();
                //    }
                //}

                return users;
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

        /// <summary>
        /// Get Assigned User List for Any Company                                                              -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<UserListForCompany> GetAssignedUserListForCompany(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<UserListForCompany> users = new List<UserListForCompany>();
                List<UserListForCompany> _allUsers = new List<UserListForCompany>();
                SqlCommand cmd = new SqlCommand("USP_Admin_UserListForCompany", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Status", Operation.Show);
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    UserListForCompany user = new UserListForCompany();
                    user.Id = Convert.ToInt32(dr["Id"].ToString());
                    user.Username = Convert.ToString(dr["Username"].ToString());
                    user.AccountNumber = Convert.ToString(dr["AccountNumber"].ToString());
                    users.Add(user);
                }
                dr.Close();
                con.Close();
                cmd.Parameters.Clear();

                //cmd = new SqlCommand("USP_Admin_UserListForCompany", con);
                //cmd.CommandType = CommandType.StoredProcedure;
                //if (_allUsers.Count > 0)
                //{
                //    for(int index=0;index<_allUsers.Count;index++)
                //    {
                //        int _returnId = 0;
                //        cmd.Parameters.AddWithValue("@Status", Operation.Show);
                //        cmd.Parameters.AddWithValue("@UserId", _allUsers[index].Id);
                //        cmd.Parameters.AddWithValue("@CompanyId", CompanyId);

                //        con.Open();
                //        _returnId = (int) cmd.ExecuteScalar();
                //        con.Close();

                //        if(_returnId > 0)
                //        {
                //            users.Add(_allUsers[index]);
                //        }
                //        cmd.Parameters.Clear();
                //    }
                //}

                return users;
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

        /// <summary>
        /// Get Total Portal Balance of DIBN                                                                    -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public List<decimal> GetTotalPortalBalance()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<decimal> _totalPortalBalance = new List<decimal>();
                decimal _totalCredits = 0, _totalDebit = 0, _total = 0;

                
                SqlCommand command = new SqlCommand("USP_Admin_GetPortalBalanceOfMainCompanyForDashboard", con);
                command.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while(reader.Read())
                {
                    if (reader["Credit"] != DBNull.Value)
                        _totalCredits = Convert.ToDecimal(reader["Credit"]);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["Debit"] != DBNull.Value)
                        _totalDebit = Convert.ToDecimal(reader["Debit"]);
                }
                con.Close();

                _total = _totalCredits - _totalDebit;

                _totalPortalBalance.Add(_totalCredits);
                _totalPortalBalance.Add(_totalDebit);
                _totalPortalBalance.Add(_total);

                return _totalPortalBalance;
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

        // Remove Document By Document Id                                                                      -- Yashasvi TBC (26-11-2022)
        public int RemoveDocument(int DocumentId, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Admin_RemoveUserDocument", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocumentId", DocumentId);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return 1;
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

        /// <summary>
        /// Get Company Type (Mainland / Freezone)                                                              -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string GetCompanyType(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string returnId = null;
                SqlCommand cmd = new SqlCommand("USP_Admin_GetCompanyType", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    returnId = reader["CompanyType"].ToString();
                }
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

        /// <summary>
        /// Get Current Notification Count for Logged in User                                                   -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetCurrentNotificationCount(int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnCount = 0;
                SqlCommand command = new SqlCommand("USP_Admin_GetAllRequestNotificaion", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 10000;

                con.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (Convert.ToInt32(reader["ServiceCreatedById"]) != UserId && Convert.ToInt32(reader["AssignedById"]) != UserId)
                    {
                        returnCount++;
                    }
                }

                reader.NextResult();

                while (reader.Read())
                {
                    if (Convert.ToInt32(reader["SupportTicketCreatedById"]) != UserId && Convert.ToInt32(reader["AssignedById"]) != UserId)
                    {
                        returnCount++;
                    }
                }

                con.Close();

                return returnCount;
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
        public async Task<GetAllActiveEmployeeListWithPaginationModel> GetAllActiveEmployeesWithPagination(int? companyId,int page,int pageSize,string searchString,string sortBy,string sortDirection)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetAllActiveEmployeeListWithPaginationModel model = new GetAllActiveEmployeeListWithPaginationModel();
                List<GetActiveEmployees> getActiveEmployees = new List<GetActiveEmployees>();
                int totalActiveEmployee = 0;
                SqlCommand cmd = new SqlCommand("USP_Admin_GetAllActiveEmployeesWithPagination", con);
                cmd.CommandType = CommandType.StoredProcedure;
                if(companyId != null )
                {
                    cmd.Parameters.AddWithValue("@companyId", companyId.Value);
                }
                cmd.Parameters.AddWithValue("@skipRows", page);
                cmd.Parameters.AddWithValue("@RowsOfPage", pageSize);
                if(searchString != null)
                    cmd.Parameters.AddWithValue("@searchPrefix", searchString);
                if(sortBy != null)
                    cmd.Parameters.AddWithValue("@sortColumn", sortBy);
                if(sortDirection != null)
                    cmd.Parameters.AddWithValue("@sortDirection", sortDirection);
                con.Open();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetActiveEmployees ActiveEmployees = new GetActiveEmployees();
                    ActiveEmployees.Id = Convert.ToInt32(reader["Id"].ToString());
                    ActiveEmployees.AccountNumber = reader["AccountNumber"].ToString();
                    ActiveEmployees.FirstName = reader["FirstName"].ToString();
                    ActiveEmployees.LastName = reader["LastName"].ToString();
                    ActiveEmployees.Nationality = reader["Nationality"].ToString();
                    ActiveEmployees.EmailID = reader["EmailID"].ToString();
                    ActiveEmployees.PhoneNumber = reader["PhoneNumber"].ToString();
                    ActiveEmployees.Designation = reader["Designation"].ToString();
                    ActiveEmployees.PassportNumber = reader["PassportNumber"].ToString();
                    ActiveEmployees.PassportExpiryDate = reader["PassportExpiryDate"].ToString();
                    ActiveEmployees.VisaExpiryDate = reader["VisaExpiryDate"].ToString();
                    ActiveEmployees.EmployeementCardExpiryDate = reader["EmployeementCardExpiryDate"].ToString();
                    ActiveEmployees.Company = reader["CompanyName"].ToString();
                    getActiveEmployees.Add(ActiveEmployees);
                }
                con.Close();

                cmd.Parameters.Clear();
                cmd = new SqlCommand("USP_Admin_GetAllActiveEmployeesWithPaginationCount",con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (companyId != null)
                {
                    cmd.Parameters.AddWithValue("@companyId", companyId.Value);
                }
                if (searchString != null)
                    cmd.Parameters.AddWithValue("@searchPrefix", searchString);

                con.Open();
                reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["Id"] != DBNull.Value)
                        totalActiveEmployee++;
                }
                con.Close();

                model.totalActiveEmployees = totalActiveEmployee;
                model.GetActiveEmployees = getActiveEmployees;
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
        public async Task<GetAllInActiveEmployeeListWithPaginationModel> GetAllInActiveEmployeesWithPagination(int? companyId,int page, int pageSize,string searchString, string sortBy, string sortDirection)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetAllInActiveEmployeeListWithPaginationModel model = new GetAllInActiveEmployeeListWithPaginationModel();
                List<GetInActiveEmployees> getInActiveEmployees = new List<GetInActiveEmployees>();
                int totalInActiveEmployee = 0;
                SqlCommand cmd = new SqlCommand("USP_Admin_GetAllInActiveEmployeesWithPagination", con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (companyId != null)
                {
                    cmd.Parameters.AddWithValue("@companyId", companyId.Value);
                }
                cmd.Parameters.AddWithValue("@skipRows", page);
                cmd.Parameters.AddWithValue("@RowsOfPage", pageSize);
                if (searchString != null)
                    cmd.Parameters.AddWithValue("@searchPrefix", searchString);
                if (sortBy != null)
                    cmd.Parameters.AddWithValue("@sortColumn", sortBy);
                if (sortDirection != null)
                    cmd.Parameters.AddWithValue("@sortDirection", sortDirection);
                con.Open();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetInActiveEmployees InActiveEmployees = new GetInActiveEmployees();
                    InActiveEmployees.Id = Convert.ToInt32(reader["Id"].ToString());
                    InActiveEmployees.AccountNumber = reader["AccountNumber"].ToString();
                    InActiveEmployees.FirstName = reader["FirstName"].ToString();
                    InActiveEmployees.LastName = reader["LastName"].ToString();
                    InActiveEmployees.Nationality = reader["Nationality"].ToString();
                    InActiveEmployees.EmailID = reader["EmailID"].ToString();
                    InActiveEmployees.PhoneNumber = reader["PhoneNumber"].ToString();
                    InActiveEmployees.Designation = reader["Designation"].ToString();
                    InActiveEmployees.PassportNumber = reader["PassportNumber"].ToString();
                    InActiveEmployees.PassportExpiryDate = reader["PassportExpiryDate"].ToString();
                    InActiveEmployees.VisaExpiryDate = reader["VisaExpiryDate"].ToString();
                    InActiveEmployees.EmployeementCardExpiryDate = reader["EmployeementCardExpiryDate"].ToString();
                    InActiveEmployees.Company = reader["CompanyName"].ToString();
                    getInActiveEmployees.Add(InActiveEmployees);
                }
                con.Close();

                cmd.Parameters.Clear();
                cmd = new SqlCommand("USP_Admin_GetAllInActiveEmployeesWithPaginationCount", con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (companyId != null)
                {
                    cmd.Parameters.AddWithValue("@companyId", companyId.Value);
                }
                if (searchString != null)
                    cmd.Parameters.AddWithValue("@searchPrefix", searchString);

                con.Open();
                reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["Id"] != DBNull.Value)
                        totalInActiveEmployee++;
                }
                con.Close();
                model.totalInActiveEmployees = totalInActiveEmployee;
                model.GetInActiveEmployees = getInActiveEmployees;
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
        public async Task<GetAllActiveCompanyOwnerListWithPaginationModel> GetAllActiveCompanyOwnerWithPagination(int? companyId,int page, int pageSize,string searchString, string sortBy, string sortDirection)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetAllActiveCompanyOwnerListWithPaginationModel model = new GetAllActiveCompanyOwnerListWithPaginationModel();
                List<GetActiveEmployees> getActiveCompanyOwner = new List<GetActiveEmployees>();
                int totalActiveCompanyOwner = 0;
                SqlCommand cmd = new SqlCommand("USP_Admin_GetAllActiveCompanyOwnerWithPagination", con);
                cmd.CommandType = CommandType.StoredProcedure;
                if(companyId != null )
                {
                    cmd.Parameters.AddWithValue("@companyId", companyId.Value);
                }
                cmd.Parameters.AddWithValue("@skipRows", page);
                cmd.Parameters.AddWithValue("@RowsOfPage", pageSize);
                if (searchString != null)
                    cmd.Parameters.AddWithValue("@searchPrefix", searchString);
                if (sortBy != null)
                    cmd.Parameters.AddWithValue("@sortColumn", sortBy);
                if (sortDirection != null)
                    cmd.Parameters.AddWithValue("@sortDirection", sortDirection);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    GetActiveEmployees ActiveCompanyOwner = new GetActiveEmployees();
                    ActiveCompanyOwner.Id = Convert.ToInt32(reader["Id"].ToString());
                    ActiveCompanyOwner.AccountNumber = reader["AccountNumber"].ToString();
                    ActiveCompanyOwner.FirstName = reader["FirstName"].ToString();
                    ActiveCompanyOwner.LastName = reader["LastName"].ToString();
                    ActiveCompanyOwner.Nationality = reader["Nationality"].ToString();
                    ActiveCompanyOwner.EmailID = reader["EmailID"].ToString();
                    ActiveCompanyOwner.PhoneNumber = reader["PhoneNumber"].ToString();
                    ActiveCompanyOwner.Designation = reader["RoleName"].ToString();
                    ActiveCompanyOwner.Company = reader["CompanyName"].ToString();
                    getActiveCompanyOwner.Add(ActiveCompanyOwner);
                }
                con.Close();
                cmd.Parameters.Clear();
                cmd = new SqlCommand("USP_Admin_GetAllActiveCompanyOwnerWithPaginationCount", con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (companyId != null)
                {
                    cmd.Parameters.AddWithValue("@companyId", companyId.Value);
                }
                if (searchString != null)
                    cmd.Parameters.AddWithValue("@searchPrefix", searchString);

                con.Open();
                reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["Id"] != DBNull.Value)
                        totalActiveCompanyOwner++;
                }
                con.Close();

                model.totalActiveCompanyOwners = totalActiveCompanyOwner;
                model.GetActiveCompanyOwner = getActiveCompanyOwner;
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
        public async Task<GetAllInActiveCompanyOwnerListWithPaginationModel> GetAllInActiveCompanyOwnerWithPagination(int? companyId, int page, int pageSize, string searchString, string sortBy, string sortDirection)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetAllInActiveCompanyOwnerListWithPaginationModel model = new GetAllInActiveCompanyOwnerListWithPaginationModel();
                List<GetInActiveEmployees> getInActiveCompanyOwner = new List<GetInActiveEmployees>();
                int totalInActiveCompanyOwner = 0;
                SqlCommand cmd = new SqlCommand("USP_Admin_GetAllInActiveCompanyOwnerWithPagination", con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (companyId != null)
                {
                    cmd.Parameters.AddWithValue("@companyId", companyId.Value);
                }
                cmd.Parameters.AddWithValue("@skipRows", page);
                cmd.Parameters.AddWithValue("@RowsOfPage", pageSize);
                if (searchString != null)
                    cmd.Parameters.AddWithValue("@searchPrefix", searchString);
                if (sortBy != null)
                    cmd.Parameters.AddWithValue("@sortColumn", sortBy);
                if (sortDirection != null)
                    cmd.Parameters.AddWithValue("@sortDirection", sortDirection);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    GetInActiveEmployees InActiveCompanyOwner = new GetInActiveEmployees();
                    InActiveCompanyOwner.Id = Convert.ToInt32(reader["Id"].ToString());
                    InActiveCompanyOwner.AccountNumber = reader["AccountNumber"].ToString();
                    InActiveCompanyOwner.FirstName = reader["FirstName"].ToString();
                    InActiveCompanyOwner.LastName = reader["LastName"].ToString();
                    InActiveCompanyOwner.Nationality = reader["Nationality"].ToString();
                    InActiveCompanyOwner.EmailID = reader["EmailID"].ToString();
                    InActiveCompanyOwner.PhoneNumber = reader["PhoneNumber"].ToString();
                    InActiveCompanyOwner.Designation = reader["RoleName"].ToString();
                    InActiveCompanyOwner.Company = reader["CompanyName"].ToString();
                    getInActiveCompanyOwner.Add(InActiveCompanyOwner);
                }
                con.Close();

                cmd.Parameters.Clear();
                cmd = new SqlCommand("USP_Admin_GetAllInActiveCompanyOwnerWithPaginationCount",con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (companyId != null)
                {
                    cmd.Parameters.AddWithValue("@companyId", companyId.Value);
                }
                if (searchString != null)
                    cmd.Parameters.AddWithValue("@searchPrefix", searchString);

                con.Open();
                reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["Id"] != DBNull.Value)
                        totalInActiveCompanyOwner++;
                }
                con.Close();

                model.totalInActiveCompanyOwner = totalInActiveCompanyOwner;
                model.GetInActiveCompanyOwner = getInActiveCompanyOwner;
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
        public async Task<GetAllActiveMainCompanyEmployeeListWithPaginationModel> GetAllActiveMainCompanyEmployeesWithPagination(int? CompanyId,int page, int pageSize,string searchBy, string searchString, string sortBy, string sortDirection)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetAllActiveMainCompanyEmployeeListWithPaginationModel model = new GetAllActiveMainCompanyEmployeeListWithPaginationModel();
                List<GetMainCompanyEmployees> getActiveEmployees = new List<GetMainCompanyEmployees>();
                int totalActiveEmployee = 0;
                SqlCommand cmd = new SqlCommand("USP_Admin_GetAllActiveMainCompanyEmployeesWithPagination", con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (CompanyId != null)
                    cmd.Parameters.AddWithValue("@CompanyId", CompanyId.Value);
                cmd.Parameters.AddWithValue("@skipRows", page);
                cmd.Parameters.AddWithValue("@RowsOfPage", pageSize);
                if(searchString!=null && searchString != "")
                    cmd.Parameters.AddWithValue("@searchPrefix", searchString);
                if(sortBy!=null && sortBy !="")
                    cmd.Parameters.AddWithValue("@sortColumn", sortBy);
                if(sortDirection!=null && sortDirection!="")
                    cmd.Parameters.AddWithValue("@sortDirection", sortDirection);
                con.Open();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if(reader["AccountNumber"].ToString() != "TBC01")
                    {
                        GetMainCompanyEmployees ActiveEmployees = new GetMainCompanyEmployees();
                        ActiveEmployees.Id = Convert.ToInt32(reader["Id"].ToString());
                        ActiveEmployees.AccountNumber = reader["AccountNumber"].ToString();
                        ActiveEmployees.FirstName = reader["FirstName"].ToString();
                        ActiveEmployees.LastName = reader["LastName"].ToString();
                        ActiveEmployees.Nationality = reader["Nationality"].ToString();
                        ActiveEmployees.EmailID = reader["EmailID"].ToString();
                        ActiveEmployees.PhoneNumber = reader["PhoneNumber"].ToString();
                        ActiveEmployees.Designation = reader["Designation"].ToString();
                        ActiveEmployees.PassportNumber = reader["PassportNumber"].ToString();
                        ActiveEmployees.PassportExpiryDate = reader["PassportExpiryDate"].ToString();
                        ActiveEmployees.VisaExpiryDate = reader["VisaExpiryDate"].ToString();
                        ActiveEmployees.InsuranceCompany = reader["InsuranceCompany"].ToString();
                        ActiveEmployees.InsuranceExpiryDate = reader["InsuranceExpiryDate"].ToString();
                        ActiveEmployees.Company = reader["CompanyName"].ToString();
                        getActiveEmployees.Add(ActiveEmployees);
                    }
                }
                con.Close();

                cmd.Parameters.Clear();
                cmd = new SqlCommand("USP_Admin_GetAllActiveMainCompanyEmployeesWithPaginationCount", con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (CompanyId != null)
                    cmd.Parameters.AddWithValue("@CompanyId", CompanyId.Value);
                if (searchString != null && searchString != "")
                    cmd.Parameters.AddWithValue("@searchPrefix", searchString);
                con.Open();
                reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["AccountNumber"].ToString() != "TBC01")
                    {
                        totalActiveEmployee++;
                    }
                }
                con.Close();

                model.totalActiveEmployee = totalActiveEmployee;
                model.GetActiveEmployee = getActiveEmployees;
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
        public async Task<GetAlInlActiveMainCompanyEmployeeListWithPaginationModel> GetAllInActiveMainCompanyEmployeesWithPagination(int? CompanyId, int page, int pageSize,string searchBy, string searchString, string sortBy, string sortDirection)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetAlInlActiveMainCompanyEmployeeListWithPaginationModel model = new GetAlInlActiveMainCompanyEmployeeListWithPaginationModel();
                List<GetMainCompanyEmployees> getInActiveEmployees = new List<GetMainCompanyEmployees>();
                int totalInActiveEmployee = 0;
                SqlCommand cmd = new SqlCommand("USP_Admin_GetAllInActiveMainCompanyEmployeesWithPagination", con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (CompanyId != null)
                    cmd.Parameters.AddWithValue("@CompanyId", CompanyId.Value);
                cmd.Parameters.AddWithValue("@skipRows", page);
                cmd.Parameters.AddWithValue("@RowsOfPage", pageSize);
                if (searchString != null && searchString !="")
                    cmd.Parameters.AddWithValue("@searchPrefix", searchString);
                if (sortBy != null && sortBy != "")
                    cmd.Parameters.AddWithValue("@sortColumn", sortBy);
                if (sortDirection != null && sortDirection != "")
                    cmd.Parameters.AddWithValue("@sortDirection", sortDirection);
                con.Open();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetMainCompanyEmployees InActiveEmployees = new GetMainCompanyEmployees();
                    InActiveEmployees.Id = Convert.ToInt32(reader["Id"].ToString());
                    InActiveEmployees.AccountNumber = reader["AccountNumber"].ToString();
                    InActiveEmployees.FirstName = reader["FirstName"].ToString();
                    InActiveEmployees.LastName = reader["LastName"].ToString();
                    InActiveEmployees.Nationality = reader["Nationality"].ToString();
                    InActiveEmployees.EmailID = reader["EmailID"].ToString();
                    InActiveEmployees.PhoneNumber = reader["PhoneNumber"].ToString();
                    InActiveEmployees.Designation = reader["Designation"].ToString();
                    InActiveEmployees.PassportNumber = reader["PassportNumber"].ToString();
                    InActiveEmployees.PassportExpiryDate = reader["PassportExpiryDate"].ToString();
                    InActiveEmployees.VisaExpiryDate = reader["VisaExpiryDate"].ToString();
                    InActiveEmployees.Company = reader["CompanyName"].ToString();
                    InActiveEmployees.InsuranceCompany = reader["InsuranceCompany"].ToString();
                    InActiveEmployees.InsuranceExpiryDate = reader["InsuranceExpiryDate"].ToString();
                    getInActiveEmployees.Add(InActiveEmployees);
                }
                con.Close();

                cmd.Parameters.Clear();
                cmd = new SqlCommand("USP_Admin_GetAllInActiveMainCompanyEmployeesWithPaginationCount", con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (CompanyId != null)
                    cmd.Parameters.AddWithValue("@CompanyId", CompanyId.Value);
                if (searchString != null && searchString != "")
                    cmd.Parameters.AddWithValue("@searchPrefix", searchString);

                con.Open();
                reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {   
                    totalInActiveEmployee++;
                }
                con.Close();

                model.totalInActiveEmployee = totalInActiveEmployee;
                model.GetInActiveEmployee = getInActiveEmployees;
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

        public async Task<bool> GetCompanyEmployeeActiveStatus(int Id)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                bool isActive = false;
                SqlCommand command = new SqlCommand("USP_Admin_GetCompanyEmployeeActiveStatus", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", Id);

                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["IsActive"] != DBNull.Value)
                        isActive = Convert.ToBoolean(reader["IsActive"]);
                }
                connection.Close();
                return isActive;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
