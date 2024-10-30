using DIBN.Areas.Admin.Data;
using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static DIBN.Areas.Admin.Data.DataSetting;
using static DIBN.Areas.Admin.Models.CompanyViewModel;

namespace DIBN.Areas.Admin.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly Connection _dataSetting;
        private readonly EncryptionService _encryptionService;
        private readonly IMessageTemplateRepository _messageTemplateRepository;
        public CompanyRepository(Connection dataSetting, EncryptionService encryptionService, IMessageTemplateRepository messageTemplateRepository)
        {
            _dataSetting = dataSetting;
            _encryptionService = encryptionService;
            _messageTemplateRepository = messageTemplateRepository;
        }
        /// <summary>
        /// Get All Company List                                                                                    -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<CompanyViewModel> GetCompanies()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<CompanyViewModel> companies = new List<CompanyViewModel>();
                SqlCommand command = new SqlCommand("USP_Admin_CompanyOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Show);
                con.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    CompanyViewModel comp = new CompanyViewModel();
                    comp.Id = Convert.ToInt32(dr["Id"].ToString());
                    comp.DIBNUserNumber = dr["DIBNUserNumber"].ToString();
                    comp.AccountNumber = dr["AccountNumber"].ToString();
                    comp.CompanyName = dr["CompanyName"].ToString();
                    comp.ShareCapital = dr["ShareCapital"].ToString();
                    comp.CompanyRegistrationNumber = dr["CompanyRegistrationNumber"].ToString();
                    comp.MobileNumber = dr["MobileNumber"].ToString();
                    comp.EmailID = dr["EmailID"].ToString();
                    comp.SecondEmailID = dr["SecondEmailID"].ToString();
                    comp.CompanyTypeName = dr["CompanyType"].ToString();
                    comp.LicenseType = dr["LicenseType"].ToString();
                    comp.LicenseStatus = dr["LicenseStatus"].ToString();
                    comp.LicenseIssueDate = dr["LicenseIssueDate"].ToString();
                    comp.LicenseExpiryDate = dr["LicenseExpiryDate"].ToString();
                    comp.LeaseFacilityType = dr["LeaseFacilityType"].ToString();
                    comp.LeaseStartDate = dr["LeaseStartDate"].ToString();
                    comp.LeaseExpiryDate = dr["LeaseExpiryDate"].ToString();
                    comp.LeaseStatus = dr["LeaseStatus"].ToString();
                    comp.CompanyStartingDate = dr["CompanyStartingDate"].ToString();
                    comp.UnitLocation = dr["UnitLocation"].ToString();
                    comp.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    comp.IsDelete = Convert.ToBoolean(dr["IsDelete"].ToString());
                    comp.ShareholderIsActive = Convert.ToBoolean(dr["ShareholderIsActive"]);
                    comp.CreatedOn = dr["CreatedOn"].ToString();
                    comp.ModifyOn = dr["ModifyOn"].ToString();
                    comp.LabourFileNo = dr["LabourFileNo"].ToString();
                    comp.ShareholderName = dr["ShareholderName"].ToString();
                    comp.ShareholderSharePercentage = dr["ShareholderSharePercentage"].ToString();
                    comp.Country = dr["Country"].ToString();
                    comp.City = dr["City"].ToString();
                    comp.IsTRN = Convert.ToBoolean(dr["IsTRN"]);
                    comp.TRN = dr["TRN"].ToString();
                    comp.TRNCreationDate = dr["TRNCreationDate"].ToString();
                    companies.Add(comp);
                }
                con.Close();
                return companies;
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
        /// Get Main Company                                                                                        -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<CompanyViewModel> GetMainCompany()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<CompanyViewModel> companies = new List<CompanyViewModel>();
                SqlCommand command = new SqlCommand("USP_Admin_GetMainCompany", con);
                command.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    CompanyViewModel comp = new CompanyViewModel();
                    comp.Id = Convert.ToInt32(dr["Id"].ToString());
                    comp.DIBNUserNumber = dr["DIBNUserNumber"].ToString();
                    comp.AccountNumber = dr["AccountNumber"].ToString();
                    comp.CompanyName = dr["CompanyName"].ToString();
                    comp.ShareCapital = dr["ShareCapital"].ToString();
                    comp.CompanyRegistrationNumber = dr["CompanyRegistrationNumber"].ToString();
                    comp.MobileNumber = dr["MobileNumber"].ToString();
                    comp.EmailID = dr["EmailID"].ToString();
                    comp.SecondEmailID = dr["SecondEmailID"].ToString();
                    comp.LicenseType = dr["LicenseType"].ToString();
                    comp.LicenseStatus = dr["LicenseStatus"].ToString();
                    comp.LicenseIssueDate = dr["LicenseIssueDate"].ToString();
                    comp.LicenseExpiryDate = dr["LicenseExpiryDate"].ToString();
                    comp.LeaseFacilityType = dr["LeaseFacilityType"].ToString();
                    comp.LeaseStartDate = dr["LeaseStartDate"].ToString();
                    comp.LeaseExpiryDate = dr["LeaseExpiryDate"].ToString();
                    comp.LeaseStatus = dr["LeaseStatus"].ToString();
                    comp.UnitLocation = dr["UnitLocation"].ToString();
                    comp.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    comp.IsDelete = Convert.ToBoolean(dr["IsDelete"].ToString());
                    comp.CreatedOn = dr["CreatedOn"].ToString();
                    comp.ModifyOn = dr["ModifyOn"].ToString();
                    companies.Add(comp);
                }
                con.Close();
                return companies;
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
        /// Create New Company                                                                                      -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int AddNewCompany(SaveNewCompany company)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                var _Password = "";
                //_encryptionService.EncryptText(company.CompanyPassword);
                int returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_CompanyOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Insert);
                
                command.Parameters.AddWithValue("@CompanyName", company.CompanyName);
                command.Parameters.AddWithValue("@AccountNumber", company.AccountNumber);
                command.Parameters.AddWithValue("@MCountryCode", company.MainContactNumberCountry);
                command.Parameters.AddWithValue("@MobileNumber", company.MobileNumber);
                command.Parameters.AddWithValue("@EmailID", company.EmailID);
                command.Parameters.AddWithValue("@SecondEmailID", company.SecondEmailID);
                command.Parameters.AddWithValue("@CompanyStartingDate", company.CompanyStartingDate);
                command.Parameters.AddWithValue("@Cmp_pass", _Password);
                command.Parameters.AddWithValue("@IsActive", company.IsActive);
                command.Parameters.AddWithValue("@Country", company.Country);
                command.Parameters.AddWithValue("@City", company.City);
                command.Parameters.AddWithValue("@CreatedBy", company.CreatedBy);
                if (company.LabourFileNo != null)
                    command.Parameters.AddWithValue("@LabourFileNo", company.LabourFileNo);
                command.Parameters.AddWithValue("@CompanySubType", company.CompanySubType);

                con.Open();
                returnId = (int)command.ExecuteScalar();
                con.Close();
                if (returnId > 0)
                {
                    if (company.SalesPersonId != null)
                    {
                        if (company.SalesPersonId.Count > 0)
                        {
                            for (int index = 0; index < company.SalesPersonId.Count; index++)
                            {
                                command = new SqlCommand("USP_Admin_SaveAssignedSalesPerson", con);
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@Status", Operation.Insert);
                                command.Parameters.AddWithValue("@SalesPersonId", company.SalesPersonId[index]);
                                command.Parameters.AddWithValue("@CompanyId", returnId);
                                command.Parameters.AddWithValue("@UserId", company.CreatedBy);
                                con.Open();
                                var _returnCompanyId = (int)command.ExecuteScalar();
                                con.Close();
                                command.Parameters.Clear();
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
        /// Get Company Details By Id                                                                               -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public CompanyViewModel GetCompanyById(int id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                CompanyViewModel comp = new CompanyViewModel();
                SqlCommand command = new SqlCommand("USP_Admin_CompanyOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.GetById);
                command.Parameters.AddWithValue("@ID", id);
                con.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    comp.Id = Convert.ToInt32(dr["Id"].ToString());
                    comp.DIBNUserNumber = dr["DIBNUserNumber"].ToString();
                    comp.AccountNumber = dr["AccountNumber"].ToString();
                    comp.Username = dr["Username"].ToString();
                    comp.CompanyName = dr["CompanyName"].ToString();
                    comp.CompanyRegistrationNumber = dr["CompanyRegistrationNumber"].ToString();
                    comp.MobileNumber = dr["MobileNumber"].ToString();
                    comp.CompanyTypeName = dr["CompanyType"].ToString();
                    comp.EmailID = dr["EmailID"].ToString();
                    comp.CompanyTypeName = dr["CompanyType"].ToString();
                    comp.CompanyStartingDate = dr["CompanyStartingDate"].ToString();
                    comp.ShareholderIsActive = Convert.ToBoolean(dr["ShareholderIsActive"].ToString());
                    comp.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    comp.CreatedOn = dr["CreatedOn"].ToString();
                    comp.ModifyOn = dr["ModifyOn"].ToString();
                    comp.ShareholderName = dr["ShareholderName"].ToString();
                    comp.ShareholderSharePercentage = dr["ShareholderSharePercentage"].ToString();
                    comp.Country = dr["Country"].ToString();
                    comp.City = dr["City"].ToString();
                    comp.LabourFileNo = dr["LabourFileNo"].ToString();
                    comp.IsTRN = Convert.ToBoolean(dr["IsTRN"]);
                    if (dr["TRN"] != DBNull.Value && dr["TRN"].ToString() != "N/A")
                        comp.TRN = dr["TRN"].ToString();
                    else
                        comp.TRN = "---";
                    if (dr["TRNCreationDate"] != DBNull.Value && dr["TRNCreationDate"].ToString() != "N/A")
                        comp.TRNCreationDate = dr["TRNCreationDate"].ToString();
                    else
                        comp.TRNCreationDate = "---";
                    if (dr["IsCorportaeText"] != DBNull.Value)
                        comp.IsCorporateText = Convert.ToBoolean(dr["IsCorportaeText"]);
                    if (dr["CorporateText"] != DBNull.Value && dr["CorporateText"].ToString() != "N/A")
                        comp.CorporateText = dr["CorporateText"].ToString();
                    else
                        comp.CorporateText = "---";
                }
                dr.Close();
                con.Close();

                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_GetCompanyShareholders", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID", id);
                List<CompanyShareholders> companyShareholders = new List<CompanyShareholders>();
                con.Open();
                dr = command.ExecuteReader();
                while (dr.Read())
                {
                    CompanyShareholders shareholders = new CompanyShareholders();
                    shareholders.ShareholderName = dr["ShareholderName"].ToString();
                    shareholders.ShareholderSharePercentage = dr["ShareholderSharePercentage"].ToString();
                    companyShareholders.Add(shareholders);
                }
                dr.Close();
                con.Close();
                comp.companyShareholders = companyShareholders;

                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_GetCompanyUsers", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID", id);
                List<CompanyUsers> companyUsers = new List<CompanyUsers>();
                con.Open();
                dr = command.ExecuteReader();
                while (dr.Read())
                {
                    CompanyUsers companyUser = new CompanyUsers();
                    if (dr["IsActive"] != DBNull.Value)
                        companyUser.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    if (dr["IsLogin"] != DBNull.Value)
                        companyUser.IsLogin = Convert.ToBoolean(dr["IsLogin"]);
                    if (dr["UserId"] != DBNull.Value)
                        companyUser.UserId = Convert.ToInt32(dr["UserId"]);
                    if (dr["Username"] != DBNull.Value)
                        companyUser.UserName = Convert.ToString(dr["Username"]);
                    if (dr["UserAccountNumber"] != DBNull.Value)
                        companyUser.UserAccountNumber = Convert.ToString(dr["UserAccountNumber"]);
                    companyUsers.Add(companyUser);
                }
                dr.Close();
                con.Close();
                comp.companyUsers = companyUsers;
                return comp;
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
        /// Update Company Details                                                                                  -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int UpdateCompanyDetails(SaveCompany company)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                var _password = "";
                //_encryptionService.EncryptText(company.CompanyPassword);
                SqlCommand command = new SqlCommand("USP_Admin_CompanyOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Update);
                command.Parameters.AddWithValue("@ID", company.Id);
                if (company.DIBNUserNumber != "N/A")
                    command.Parameters.AddWithValue("@DIBNUserNumber", company.DIBNUserNumber);
                command.Parameters.AddWithValue("@AccountNumber", company.AccountNumber);
                command.Parameters.AddWithValue("@CompanyName", company.CompanyName);
                command.Parameters.AddWithValue("@Cmp_pass", _password);

                if (company.ShareCapital != "N/A")
                    command.Parameters.AddWithValue("@ShareCapital", company.ShareCapital);

                if (company.CompanyRegistartionNumber != "N/A")
                    command.Parameters.AddWithValue("@CompanyRegistrationNumber", company.CompanyRegistartionNumber);
                command.Parameters.AddWithValue("@MCountryCode", company.MainContactNumberCountry);
                command.Parameters.AddWithValue("@MobileNumber", company.MobileNumber);
                command.Parameters.AddWithValue("@ECountryCode", company.EmergencyContactNumberCountry);
                command.Parameters.AddWithValue("@EmergencyNumber", company.EmergencyNumber);
                command.Parameters.AddWithValue("@EmailID", company.EmailID);
                command.Parameters.AddWithValue("@SecondEmailID", company.SecondEmailID);
                command.Parameters.AddWithValue("@CompanyType", company.CompanyTypeName);
                command.Parameters.AddWithValue("@UserId", company.UserId);

                if (company.LicenseType != "N/A" && company.LicenseType != "")
                    command.Parameters.AddWithValue("@LicenseType", company.LicenseType);

                if (company.LicenseStatus != "N/A" && company.LicenseStatus != "")
                    command.Parameters.AddWithValue("@LicenseStatus", company.LicenseStatus);

                if (company.LicenseIssueDate != "N/A" && company.LicenseIssueDate != "")
                    command.Parameters.AddWithValue("@LicenseIssueDate", company.LicenseIssueDate);

                if (company.LicenseExpiryDate != "N/A" && company.LicenseExpiryDate != "")
                    command.Parameters.AddWithValue("@LicenseExpiryDate", company.LicenseExpiryDate);

                if (company.LeaseFacilityType != "N/A" && company.LeaseFacilityType != "")
                    command.Parameters.AddWithValue("@LeaseFacilityType", company.LeaseFacilityType);

                if (company.LeaseStartDate != "N/A" && company.LeaseStartDate != "")
                    command.Parameters.AddWithValue("@LeaseStartDate", company.LeaseStartDate);

                if (company.LeaseEndDate != "N/A" && company.LeaseEndDate != "")
                    command.Parameters.AddWithValue("@LeaseExpiryDate", company.LeaseEndDate);

                if (company.LeaseStatus != "N/A" && company.LeaseStatus != "")
                    command.Parameters.AddWithValue("@LeaseStatus", company.LeaseStatus);

                if (company.CompanyStartingDate != "N/A" && company.CompanyStartingDate != "")
                    command.Parameters.AddWithValue("@CompanyStartingDate", company.CompanyStartingDate);

                if (company.UnitLocation != "N/A" && company.UnitLocation != "")
                    command.Parameters.AddWithValue("@UnitLocation", company.UnitLocation);

                if (company.LabourFileNo != "N/A" && company.LabourFileNo != "")
                    command.Parameters.AddWithValue("@LabourFileNo", company.LabourFileNo);

                command.Parameters.AddWithValue("@IsActive", company.IsActive);
                command.Parameters.AddWithValue("@IsTRN", company.IsTRN);

                if (company.TRN != "N/A" && company.TRN != "")
                    command.Parameters.AddWithValue("@TRN", company.TRN);

                if (company.TRNCreationDate != "N/A" && company.TRNCreationDate != "")
                    command.Parameters.AddWithValue("@TRNCreationDate", company.TRNCreationDate);

                command.Parameters.AddWithValue("@Country", company.Country);
                command.Parameters.AddWithValue("@City", company.City);
                command.Parameters.AddWithValue("@CreatedBy", company.CreatedBy);
                if (company.IsCorporateText)
                    command.Parameters.AddWithValue("@IsCorporateText", company.IsCorporateText);
                if (company.CorporateText != "" && company.CorporateText != "N/A")
                    command.Parameters.AddWithValue("@CorporateText", company.CorporateText);
                if (company.CompanySubType != "" && company.CompanySubType != "N/A")
                    command.Parameters.AddWithValue("@CompanySubType", company.CompanySubType);

                con.Open();
                returnId = (int)command.ExecuteScalar();
                con.Close();
                if (returnId > 0)
                {
                    /////// Save Sales Person while Creating or Updating Company Details (based on client Requirement)                                                  -- Yashasvi TBC (22-11-2022)
                    command = new SqlCommand("USP_Admin_SaveAssignedSalesPerson", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Status", Operation.Delete);
                    command.Parameters.AddWithValue("@CompanyId", returnId);
                    command.Parameters.AddWithValue("@UserId", company.CreatedBy);
                    con.Open();
                    command.ExecuteNonQuery();
                    con.Close();
                    command.Parameters.Clear();

                    if (company.SalesPersonId != null)
                    {
                        if (company.SalesPersonId.Count > 0)
                        {
                            for (int index = 0; index < company.SalesPersonId.Count; index++)
                            {
                                command = new SqlCommand("USP_Admin_SaveAssignedSalesPerson", con);
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@Status", Operation.Insert);
                                command.Parameters.AddWithValue("@SalesPersonId", company.SalesPersonId[index]);
                                command.Parameters.AddWithValue("@CompanyId", returnId);
                                command.Parameters.AddWithValue("@UserId", company.CreatedBy);
                                con.Open();
                                var _returnCompanyId = (int)command.ExecuteScalar();
                                con.Close();
                                command.Parameters.Clear();
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
        /// Delete Any Company                                                                                      -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int DeleteCompany(int id, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_CompanyOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Delete);
                command.Parameters.AddWithValue("@ID", id);
                command.Parameters.AddWithValue("@CreatedBy", UserId);
                con.Open();
                returnId = (int)command.ExecuteScalar();
                con.Close();
                return returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally { con.Close(); }
        }

        /// <summary>
        /// Get Account Number of Company                                                                           -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<string> GetAccountNumber(int id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> _accountNumber = new List<string>();
                SqlCommand command = new SqlCommand("USP_Admin_CompanyOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", 5);
                command.Parameters.AddWithValue("@UserId", id);
                con.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    _accountNumber.Add(dr["AccountNumber"].ToString());
                    _accountNumber.Add(dr["EmailID"].ToString());
                }
                con.Close();
                return _accountNumber;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally { con.Close(); }
        }

        /// <summary>
        /// Get Company Details for Update                                                                          -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public SaveCompany GetCompanyDetails(int id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                SaveCompany comp = new SaveCompany();
                SqlCommand command = new SqlCommand("USP_Admin_GetCompanyDetail", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID", id);
                con.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    comp.Id = Convert.ToInt32(dr["Id"].ToString());
                    comp.DIBNUserNumber = dr["DIBNUserNumber"].ToString();
                    comp.AccountNumber = dr["AccountNumber"].ToString();
                    comp.UserId = Convert.ToInt32(dr["UserId"].ToString());
                    comp.Username = dr["Username"].ToString();
                    //comp.CompanyPassword = dr["Cmp_Pass"].ToString();
                    //comp.CompanyPassword = _encryptionService.DecryptText(comp.CompanyPassword);
                    comp.CompanyName = dr["CompanyName"].ToString();
                    comp.ShareCapital = dr["ShareCapital"].ToString();
                    comp.CompanyRegistartionNumber = dr["CompanyRegistrationNumber"].ToString();
                    comp.MainContactNumberCountry = dr["MCountryCode"].ToString();
                    comp.EmergencyContactNumberCountry = dr["ECountryCode"].ToString();
                    comp.EmergencyNumber = dr["EmergencyNumber"].ToString();
                    comp.MobileNumber = dr["MobileNumber"].ToString();
                    if (dr["EmailID"] != DBNull.Value && dr["EmailID"].ToString() != "N/A")
                        comp.EmailID = dr["EmailID"].ToString();
                    comp.SecondEmailID = dr["SecondEmailID"].ToString();
                    comp.CompanyTypeName = dr["CompanyType"].ToString();
                    comp.LicenseType = dr["LicenseType"].ToString();
                    comp.LicenseStatus = dr["LicenseStatus"].ToString();
                    comp.LicenseIssueDate = dr["LicenseIssueDate"].ToString();
                    comp.LicenseExpiryDate = dr["LicenseExpiryDate"].ToString();
                    comp.LeaseFacilityType = dr["LeaseFacilityType"].ToString();
                    comp.LeaseStartDate = dr["LeaseStartDate"].ToString();
                    comp.CompanyStartingDate = dr["CompanyStartingDate"].ToString();
                    comp.LeaseEndDate = dr["LeaseExpiryDate"].ToString();
                    comp.LeaseStatus = dr["LeaseStatus"].ToString();
                    comp.UnitLocation = dr["UnitLocation"].ToString();
                    comp.ShareholderName = dr["ShareholderName"].ToString();
                    comp.ShareholderSharePercentage = dr["ShareholderSharePercentage"].ToString();
                    comp.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    comp.Country = dr["Country"].ToString();
                    comp.City = dr["City"].ToString();
                    comp.IsTRN = Convert.ToBoolean(dr["IsTRN"]);
                    comp.TRN = dr["TRN"].ToString();
                    comp.LabourFileNo = dr["LabourFileNo"].ToString();
                    comp.TRNCreationDate = dr["TRNCreationDate"].ToString();
                    if (dr["IsCorportaeText"] != DBNull.Value)
                        comp.IsCorporateText = Convert.ToBoolean(dr["IsCorportaeText"]);
                    if (dr["CorporateText"] != DBNull.Value && dr["CorporateText"].ToString() != "N/A")
                        comp.CorporateText = Convert.ToString(dr["CorporateText"]);
                    if (dr["SubCompanyType"] != DBNull.Value && dr["SubCompanyType"].ToString() != "N/A")
                        comp.CompanySubType = Convert.ToString(dr["SubCompanyType"]);
                }
                con.Close();
                if(comp.EmailID != null && comp.EmailID != "")
                {
                    if (comp.EmailID.Contains(","))
                    {
                        string[] _emails = comp.EmailID.Split(',');
                        string _allEmail = "";
                        comp.EmailID = _emails[0];
                        comp.OtherEmailID = new List<string>();
                        for (int i = 1; i < _emails.Length; i++)
                        {
                            comp.OtherEmailID.Add(_emails[i]);
                        }

                        for (int i = 0; i < comp.OtherEmailID.Count; i++)
                        {
                            if (_allEmail == "")
                            {
                                _allEmail = comp.OtherEmailID[i];
                            }
                            else
                            {
                                _allEmail = _allEmail + "," + comp.OtherEmailID[i];
                            }
                        }
                        comp.OtherEmailIdValues = _allEmail;
                    }
                }
                

                if (comp.MobileNumber.Contains(","))
                {
                    string[] _mobileNumbers = comp.MobileNumber.Split(',');
                    string[] _mobileNumbersCode = comp.MainContactNumberCountry.Split(',');
                    string _allMobileNumbers = "", _allMainNumbersCode = "";
                    comp.MobileNumber = _mobileNumbers[0];
                    comp.MainContactNumberCountry = _mobileNumbersCode[0];
                    comp.OtherContactNumbers = new List<string>();
                    comp.OtherContactNumbersCode = new List<string>();
                    for (int i = 1; i < _mobileNumbers.Length; i++)
                    {
                        comp.OtherContactNumbers.Add(_mobileNumbers[i]);
                        comp.OtherContactNumbersCode.Add(_mobileNumbersCode[i]);
                    }

                    for (int i = 0; i < comp.OtherContactNumbers.Count; i++)
                    {
                        if (_allMobileNumbers == "")
                        {
                            _allMobileNumbers = comp.OtherContactNumbers[i];
                            _allMainNumbersCode = comp.OtherContactNumbersCode[i];
                        }
                        else
                        {
                            _allMobileNumbers = _allMobileNumbers + "," + comp.OtherContactNumbers[i];
                            _allMainNumbersCode = _allMainNumbersCode + "," + comp.OtherContactNumbersCode[i];
                        }
                    }
                    comp.EmergencyNumber = _allMobileNumbers;
                    comp.EmergencyContactNumberCountry = _allMainNumbersCode;
                }
                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_SaveAssignedSalesPerson", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.GetById);
                command.Parameters.AddWithValue("@CompanyId", id);
                con.Open();
                dr = command.ExecuteReader();
                comp.SalesPersonId = new List<int>();
                while (dr.Read())
                {
                    int salesPersonId = Convert.ToInt32(dr["SalesPersonId"]);
                    comp.SalesPersonId.Add(salesPersonId);
                }
                con.Close();
                return comp;
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
        /// Check Whether Company name Already Exists or not                                                        -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int CheckExistanceOfCompany(string name)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_CheckExistingCompany", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyName", name);
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
        /// Check whether passed Email Address already Exists or Not                                                -- Yashasvi TBC (26-11-2022)
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
                if (Email != null)
                {
                    SqlCommand cmd = new SqlCommand("USP_Admin_CheckExistingEmail", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", Email);
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
        /// Get Selected Company Details for Login from Admin Panel                                                 -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<string> GetSelectedCompanyDetails(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> result = new List<string>();
                SqlCommand cmd = new SqlCommand("USP_Admin_GetSelectedCompanyDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(dr["DIBNUserNumber"].ToString());
                    var _password = _encryptionService.DecryptText(dr["Password"].ToString());
                    result.Add(_password);
                }
                con.Close();
                return result;
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
        /// Download Document                                                                                       -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DocumentsViewModel DownloadDocument(int Id, int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                DocumentsViewModel document = new DocumentsViewModel();
                SqlCommand cmd = new SqlCommand("USP_User_DownloadDocument", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", Id);
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    document.Id = Convert.ToInt32(dr["Id"].ToString());
                    document.FileName = dr["FileName"].ToString();
                    document.Extension = dr["Extension"].ToString();
                    document.Data = (Byte[])dr["DataBinary"];
                    document.Title = dr["Title"].ToString();
                    document.Description = dr["Description"].ToString();
                }
                con.Close();
                return document;
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
        /// Get All Document List                                                                                   -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<DocumentsViewModel> GetAllDocuments(int CompanyId, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<DocumentsViewModel> documents = new List<DocumentsViewModel>();
                SqlCommand cmd = new SqlCommand("USP_User_GetDocument", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    DocumentsViewModel document = new DocumentsViewModel();
                    document.Id = Convert.ToInt32(dr["Id"].ToString());
                    document.FileName = dr["FileName"].ToString();
                    document.Extension = dr["Extension"].ToString();
                    document.Title = dr["Title"].ToString();
                    document.Description = dr["Description"].ToString();
                    document.CompanyId = Convert.ToInt32(dr["CompanyId"].ToString());
                    document.UserId = Convert.ToInt32(dr["UserId"].ToString());
                    document.SelectedDocumentType = Convert.ToInt32(dr["DocumentTypeId"].ToString());
                    documents.Add(document);
                }
                con.Close();
                return documents;
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
        /// Upload Document                                                                                         -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="document"></param>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int UploadSelectedFile(DocumentsViewModel document, int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                string _getName = document.formFile.FileName;
                int lastIndex = document.formFile.FileName.LastIndexOf(".");
                String Name = document.formFile.FileName.Substring(0, lastIndex);
                string FileName = Name;
                var extn = Path.GetExtension(_getName);

                Byte[] bytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    document.formFile.OpenReadStream().CopyTo(ms);
                    bytes = ms.ToArray();
                }

                SqlCommand cmd = new SqlCommand("USP_User_SaveDocument", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FileName", FileName);
                cmd.Parameters.AddWithValue("@Extension", extn);
                cmd.Parameters.AddWithValue("@DataBinary", bytes);
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                cmd.Parameters.AddWithValue("@Title", document.Title);
                cmd.Parameters.AddWithValue("@Description", document.Description);
                cmd.Parameters.AddWithValue("@EmployeeId", document.UserId);
                cmd.Parameters.AddWithValue("@UserId", document.CreatedBy);
                cmd.Parameters.AddWithValue("@SelectedDocumentId", document.SelectedDocumentType);
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
        /// Get User Details                                                                                        -- Yashasvi TBC (26-11-2022)
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
                    user.FirstName = dr["FirstName"].ToString();
                    user.LastName = dr["LastName"].ToString();
                    user.CompanyId = Convert.ToInt32(dr["CompanyId"].ToString());
                    user.Company = dr["Company"].ToString();
                    user.Nationality = dr["Nationality"].ToString();
                    user.EmailID = dr["EmailID"].ToString();
                    user.PhoneNumber = dr["PhoneNumber"].ToString();
                    user.CountryOfRecidence = dr["CountryOfRecidence"].ToString();
                    user.Role = dr["Role"].ToString();
                    user.TelephoneNumber = dr["TelephoneNumber"].ToString();
                    user.PassportNumber = dr["PassportNumber"].ToString();
                    user.VisaExpiryDate = dr["VisaExpiryDate"].ToString();
                    user.InsuranceCompany = dr["InsuranceCompany"].ToString();
                    user.InsuranceExpiryDate = dr["InsuranceExpiryDate"].ToString();
                    user.PassportExpiryDate = dr["PassportExpiryDate"].ToString();
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


        /// <summary>
        /// Get Company Documents                                                                                   -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<DocumentsViewModel> GetCompanyDocuments(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<DocumentsViewModel> companyDocuments = new List<DocumentsViewModel>();
                SqlCommand cmd = new SqlCommand("USP_Admin_GetCompanyDocuments", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DocumentsViewModel documents = new DocumentsViewModel();
                    documents.Id = Convert.ToInt32(reader["Id"].ToString());
                    documents.FileName = reader["FileName"].ToString();
                    documents.Extension = reader["Extension"].ToString();
                    documents.Title = reader["Title"].ToString();
                    documents.Description = reader["Description"].ToString();
                    documents.IssueDate = reader["IssueDate"].ToString();
                    documents.ExpiryDate = reader["ExpiryDate"].ToString();
                    documents.AuthorityName = reader["AuthorityName"].ToString();
                    documents.CompanyId = Convert.ToInt32(reader["CompanyId"].ToString());
                    documents.SelectedDocumentType = Convert.ToInt32(reader["DocumentTypeId"].ToString());
                    documents.DocumentTypeName = reader["DocumentTypeName"].ToString();
                    companyDocuments.Add(documents);
                }
                con.Close();
                return companyDocuments;
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
        /// Upload Company Document                                                                                 -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int UploadCompanyDocuments(GetCompanyDocuments document)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                string _getName = document.formFile.FileName;
                int lastIndex = document.formFile.FileName.LastIndexOf(".");
                String Name = document.formFile.FileName.Substring(0, lastIndex);
                string FileName = Name;
                var extn = Path.GetExtension(_getName);

                Byte[] bytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    document.formFile.OpenReadStream().CopyTo(ms);
                    bytes = ms.ToArray();
                }

                SqlCommand cmd = new SqlCommand("USP_Admin_SaveCompanyDocuments", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FileName", FileName);
                cmd.Parameters.AddWithValue("@Extension", extn);
                cmd.Parameters.AddWithValue("@DataBinary", bytes);
                cmd.Parameters.AddWithValue("@CompanyId", document.CompanyId);
                cmd.Parameters.AddWithValue("@Title", document.Title);
                cmd.Parameters.AddWithValue("@Description", document.Description);
                cmd.Parameters.AddWithValue("@IssueDate", document.IssueDate);
                cmd.Parameters.AddWithValue("@ExpiryDate", document.ExpiryDate);
                cmd.Parameters.AddWithValue("@AuthorityName", document.AuthorityName);
                cmd.Parameters.AddWithValue("@SelectedDocumentType", document.SelectedDocumentType);
                cmd.Parameters.AddWithValue("@UserId", document.CreatedBy);
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
        /// Download Company Document                                                                               -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public GetCompanyDocuments DownloadCompanyDocuments(int Id, int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetCompanyDocuments document = new GetCompanyDocuments();
                SqlCommand cmd = new SqlCommand("USP_Admin_GetCompanyDocuments", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", Id);
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    document.Id = Convert.ToInt32(dr["Id"].ToString());
                    document.FileName = dr["FileName"].ToString();
                    document.Extension = dr["Extension"].ToString();
                    document.Data = (Byte[])dr["DataBinary"];
                    document.Title = dr["Title"].ToString();
                    document.Description = dr["Description"].ToString();
                    document.IssueDate = dr["IssueDate"].ToString();
                    document.ExpiryDate = dr["ExpiryDate"].ToString();
                    document.AuthorityName = dr["AuthorityName"].ToString();
                    document.SelectedDocumentType = Convert.ToInt32(dr["DocumentTypeId"].ToString());
                }
                con.Close();
                return document;
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
        /// Get Total Company Count (Mainland / Freezone)                                                           -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<int> GetCompaniesCount()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<int> CountofCompany = new List<int>();
                int totalmainland = 0, totalFreezone = 0,totalCompany=0;
                SqlCommand cmd = null;
                cmd = new SqlCommand("USP_Admin_GetCountOfMainlandAndFreezoneCompany", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    totalmainland++;
                }
                reader.NextResult();
                while (reader.Read())
                {
                    totalFreezone++;
                }
                con.Close();

                totalCompany = totalmainland + totalFreezone;
                CountofCompany.Add(totalmainland);
                CountofCompany.Add(totalFreezone);
                CountofCompany.Add(totalCompany);
                return CountofCompany;
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
        /// Get Suggestion for Authority names while uploading Company Document                                     -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<string> GetAuthorityNames()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> Authority = new List<string>();
                
                SqlCommand cmd = new SqlCommand("USP_Admin_GetListOfDocumentAuthority", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Authority.Add(reader["AuthorityName"].ToString());
                }
                con.Close();
                return Authority;
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
        /// Get last Company Account Number                                                                         -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string GetLastAccountNumber()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string last = "";
                SqlCommand cmd = new SqlCommand("USP_Admin_CompanyOperation", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Status", 6);
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
        /// Check Whether passed Account Number is already Exists or Not.                                           -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="AccountNumber"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int CheckExistanceOfCompanyAccountNumber(string AccountNumber)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand cmd = new SqlCommand("USP_Admin_CheckExistingCompanyAccountNumber", con);
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

        /// <summary>
        /// Send Email to Company for Activation                                                                    -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="CompanyName"></param>
        /// <param name="Email"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [Obsolete]
        public async Task<string> sendMail(string CompanyName, string Email, string url)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _mailTemplateId = 0;
                
                SqlCommand cmd = new SqlCommand("USP_Admin_GetMessageTemplateIdByName", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@messageTemplateName", "Activate Company");

                con.Open();
                _mailTemplateId = (int)cmd.ExecuteScalar();
                con.Close();

                MessageTemplateViewModel model = new MessageTemplateViewModel();
                model = _messageTemplateRepository.GetMessageTemplateDetails(_mailTemplateId);
                string _MessageBody = model.Body;

                var message = new MailMessage();
                message.To.Add(new MailAddress(Email));
                message.From = new MailAddress(model.FromMail);
                message.Subject = model.Subject;

                _MessageBody = _MessageBody.Replace("%Url%", url);
                _MessageBody = _MessageBody.Replace("%CompanyName%", CompanyName);
                StringBuilder builder = new StringBuilder();
                builder.Append("<p style='color:#000;'>" + _MessageBody + "</p>");

                message.IsBodyHtml = true;
                message.Body = builder.ToString();
                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential()
                    {
                        UserName = model.FromMail,
                        Password = "DIBNPortal789"
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                    string result = "Email has been send successfully.";
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Send Email to Change Password                                                                           -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="CompanyName"></param>
        /// <param name="OldPassword"></param>
        /// <param name="NewPassword"></param>
        /// <param name="Email"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [Obsolete]
        public async Task<string> SendChangePasswordMail(string CompanyName, string OldPassword, string NewPassword, string Email)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _mailTemplateId = 0;
                
                SqlCommand cmd = new SqlCommand("USP_Admin_GetMessageTemplateIdByName", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@messageTemplateName", "Change Password");

                con.Open();
                _mailTemplateId = (int)cmd.ExecuteScalar();
                con.Close();

                MessageTemplateViewModel model = new MessageTemplateViewModel();
                model = _messageTemplateRepository.GetMessageTemplateDetails(_mailTemplateId);
                string _MessageBody = model.Body;

                var message = new MailMessage();
                message.To.Add(new MailAddress(Email));
                message.From = new MailAddress(model.FromMail);
                message.Subject = model.Subject;

                _MessageBody = _MessageBody.Replace("%OldPassword%", OldPassword);
                _MessageBody = _MessageBody.Replace("%NewPassword%", NewPassword);
                _MessageBody = _MessageBody.Replace("%Name%", CompanyName);
                _MessageBody = _MessageBody.Replace("%Email%", Email);
                StringBuilder builder = new StringBuilder();
                builder.Append("<p style='color:#000;'>" + _MessageBody + "</p>");

                message.IsBodyHtml = true;
                message.Body = builder.ToString();
                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential()
                    {
                        UserName = model.FromMail,
                        Password = "DIBNPortal789"
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                    string result = "Email has been send successfully.";
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Activate Company                                                                                        -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="ActiveId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string ChangestatusOfCompany(int ActiveId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string returnId = null;
                SqlCommand cmd = new SqlCommand("USP_Admin_ChangeStatusOfCompany", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId", ActiveId);
                con.Open();
                returnId = (string)cmd.ExecuteScalar();
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
        /// Get All user's List associate to Passed Company                                                         -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public CompanyAssociationData CheckCompanyAssociation(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                CompanyAssociationData data = new CompanyAssociationData();
                SqlCommand cmd = new SqlCommand("USP_Admin_CheckCompanyAssociation", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                List<int> userList = new List<int>();
                while (reader.Read())
                {
                    if (userList.Count > 0)
                    {
                        if (!userList.Contains(Convert.ToInt32(reader["Users"])))
                        {
                            userList.Add(Convert.ToInt32(reader["Users"]));
                        }
                    }
                    else
                    {
                        userList.Add(Convert.ToInt32(reader["Users"]));
                    }
                }
                data.Users = userList.Count;
                reader.NextResult();
                while (reader.Read())
                {
                    data.Shareholders = Convert.ToInt32(reader["Shareholders"]);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    data.Documents = Convert.ToInt32(reader["Documents"]);
                }
                con.Close();
                return data;
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
        /// Remove Company Document                                                                                -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="DocumentId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int RemoveCompanyDocuments(int DocumentId, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                
                SqlCommand cmd = new SqlCommand("USP_Admin_RemoveCompanyDocument", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@documentId", DocumentId);
                cmd.Parameters.AddWithValue("@userId", UserId);
                con.Open();
                returnId = Convert.ToInt32(cmd.ExecuteScalar());
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
        /// Send Mail                                                                                               -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [Obsolete]
        public async Task<string> SendCompanyMail(EmailViewModel model)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                var message = new MailMessage();

                if (model.ToMail.Contains(","))
                {
                    string[] _sendMailsTo = model.ToMail.Split(",");
                    foreach (string _sendMailTo in _sendMailsTo)
                    {
                        message.To.Add(new MailAddress(_sendMailTo));
                    }
                }
                else
                {
                    message.To.Add(new MailAddress(model.ToMail));
                }
                if (model.Documents != null)
                {
                    if (model.Documents.Count > 0)
                    {
                        for (int i = 0; i < model.Documents.Count; i++)
                        {
                            string fileName = Path.GetFileName(model.Documents[i].FileName);
                            message.Attachments.Add(new Attachment(model.Documents[i].OpenReadStream(), fileName));
                        }
                    }
                }


                message.From = new MailAddress(model.FromMail);
                message.Subject = model.Subject;

                StringBuilder builder = new StringBuilder();
                builder.Append("<p style='color:#000;'>" + model.Body + "</p>");

                message.IsBodyHtml = true;
                message.Body = builder.ToString();
                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential()
                    {
                        UserName = model.FromMail,
                        Password = model.Password
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                    string result = "Email has been send successfully.";
                    var password = _encryptionService.EncryptText(model.Password);
                    SqlCommand cmd = new SqlCommand("USP_Admin_SaveEmailInfo", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MailFrom", model.FromMail);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@MailTo", model.ToMail);
                    cmd.Parameters.AddWithValue("@Subject", model.Subject);
                    cmd.Parameters.AddWithValue("@MailBody", model.Body);
                    cmd.Parameters.AddWithValue("@CompanyId", model.CompanyId);
                    cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get Company Name                                                                                        -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string GetCompanyName(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string Company = "";
                SqlCommand cmd = new SqlCommand("USP_Admin_GetCompanyNameById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Company = reader["CompanyName"].ToString();
                }
                con.Close();
                return Company;
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
        /// Get Company Details for Account Summary                                                                 -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<GetCompanyForAccountSummaryWithPagination> GetCompanyDetailForAccountSummary(int skip, int pageSize, string sortBy, string sortDirection, string searchBy, string searchString)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int totalRecords = 0;
                GetCompanyForAccountSummaryWithPagination cmpModel = new GetCompanyForAccountSummaryWithPagination();
                int index = 0;
                if (skip == 0)
                {
                    index = 1;
                }
                else
                {
                    index = skip + 1;
                }
                List<GetCompaniesForAccountSummary> companies = new List<GetCompaniesForAccountSummary>();
                SqlCommand cmd = new SqlCommand("USP_GetCompaniesForAccountSummary", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@skipRows", skip);
                cmd.Parameters.AddWithValue("@RowsOfPage", pageSize);
                if(searchString!= null && searchString != "")
                    cmd.Parameters.AddWithValue("@searchPrefix", searchString);
                if(sortBy != null && sortBy != "")
                    cmd.Parameters.AddWithValue("@sortColumn", sortBy);
                if (sortDirection!= null && sortDirection !="")
                    cmd.Parameters.AddWithValue("@sortDirection", sortDirection);

                cmd.CommandTimeout = 36000;
                con.Open();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetCompaniesForAccountSummary model = new GetCompaniesForAccountSummary();
                    if (reader["AccountNumber"] != DBNull.Value)
                        model.AccountNumber = reader["AccountNumber"].ToString();
                    if (reader["Id"] != DBNull.Value)
                        model.CompanyId = Convert.ToInt32(reader["Id"]);
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["EmailID"] != DBNull.Value)
                        model.EmailID = reader["EmailID"].ToString();
                    if (reader["SalesPerson"] != DBNull.Value)
                        model.SalesPerson = reader["SalesPerson"].ToString();
                    if (reader["PortalBalance"] != DBNull.Value)
                        model.PortalBalance = Math.Round(Convert.ToDecimal(reader["PortalBalance"]), 2);
                    model.index = index;
                    companies.Add(model);
                    index++;
                }
                con.Close();

                cmd.Parameters.Clear();
                cmd = new SqlCommand("USP_GetCompaniesForAccountSummaryCount", con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (searchString != null && searchString != "")
                    cmd.Parameters.AddWithValue("@searchPrefix", searchString);

                con.Open();
                reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["AccountNumber"] != DBNull.Value)
                        totalRecords++;
                }
                con.Close();
                cmpModel.getCompaniesForAccounts = companies;
                cmpModel.totalCompanies = totalRecords;
                return cmpModel;
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
        /// Get Logs Company wise                                                                                   -- Yashasvi TBC (30-11-2022)
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<GetCompanyLog> GetAllLogByCompanyId(int companyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetCompanyLog> logs = new List<GetCompanyLog>();
                SqlCommand cmd = new SqlCommand("USP_Admin_GetCompanyWiseLog", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 36000;
                cmd.Parameters.AddWithValue("@CompanyId", companyId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["CreatedOnDate"] != DBNull.Value && reader["CreatedOnDate"].ToString() != "N/A"
                        && reader["CreatedByFirstName"] != DBNull.Value && reader["CreatedByFirstName"].ToString() != "N/A")
                    {
                        GetCompanyLog log = new GetCompanyLog();
                        DateTime createdOn = Convert.ToDateTime(reader["CreatedOnTime"]);
                        string createdTime = createdOn.ToString("hh:mm:ss tt");
                        string message = reader["CompanyName"].ToString() + "(" + reader["AccountNumber"].ToString() + ") is created by " +
                            reader["CreatedByFirstName"].ToString() + " " + reader["CreatedByLastName"].ToString() + "(" + reader["CreatedByAccountNumber"].ToString() + ") on "
                            + reader["CreatedOnDate"].ToString() + " " + createdTime;

                        log.LogMessage = message;
                        log.CreatedBy = reader["CreatedByFirstName"].ToString() + " " + reader["CreatedByLastName"].ToString() + "(" + reader["CreatedByAccountNumber"].ToString() + ")";
                        log.CreatedOnDate = reader["CreatedOnDate"].ToString();
                        log.CreatedOnTime = createdTime;
                        log.DateOnUtc = Convert.ToDateTime(reader["CreatedOnTime"]);
                        logs.Add(log);
                    }
                }

                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["ModifyOnDate"] != DBNull.Value && reader["ModifyOnDate"].ToString() != "N/A" &&
                        reader["ModifyByFirstName"] != DBNull.Value && reader["ModifyByFirstName"].ToString() != "N/A")
                    {
                        GetCompanyLog log = new GetCompanyLog();
                        DateTime modifyOn = Convert.ToDateTime(reader["ModifyOnTime"]);
                        string modifyTime = modifyOn.ToString("hh:mm:ss tt");
                        string message = reader["CompanyName"].ToString() + "(" + reader["AccountNumber"].ToString() + ") is amendment by " +
                            reader["ModifyByFirstName"].ToString() + " " + reader["ModifyByLastName"].ToString() + "(" + reader["ModifyByAccountNumber"].ToString() + ") on "
                            + reader["ModifyOnDate"].ToString() + " " + modifyTime;

                        log.LogMessage = message;
                        log.ModifyBy = reader["ModifyByFirstName"].ToString() + " " + reader["ModifyByLastName"].ToString() + "(" + reader["ModifyByAccountNumber"].ToString() + ")";
                        log.ModifyOnDate = reader["ModifyOnDate"].ToString();
                        log.ModifyOnTime = modifyTime;
                        log.DateOnUtc = Convert.ToDateTime(reader["ModifyOnTime"]);
                        logs.Add(log);
                    }
                }

                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["CreatedOnDate"] != DBNull.Value && reader["CreatedOnDate"].ToString() != "N/A"
                        && reader["CreatedByFirstName"] != DBNull.Value && reader["CreatedByFirstName"].ToString() != "N/A")
                    {
                        GetCompanyLog log = new GetCompanyLog();
                        DateTime createdOn = Convert.ToDateTime(reader["CreatedOnTime"]);
                        string createdTime = createdOn.ToString("hh:mm:ss tt");
                        string message = "";

                        message = "New Company Document " + reader["DocumentFileName"].ToString() + " of " + reader["DocumentType"].ToString() + " Document Type is Uploaded by " +
                        reader["CreatedByFirstName"].ToString() + " " + reader["CreatedByLastName"].ToString() + "(" + reader["CreatedByAccountNumber"].ToString() + ") on "
                        + reader["CreatedOnDate"].ToString() + " " + createdTime;

                        log.LogMessage = message;
                        log.CreatedBy = reader["CreatedByFirstName"].ToString() + " " + reader["CreatedByLastName"].ToString() + "(" + reader["CreatedByAccountNumber"].ToString() + ")";
                        log.CreatedOnDate = reader["CreatedOnDate"].ToString();
                        log.CreatedOnTime = createdTime;
                        log.DateOnUtc = Convert.ToDateTime(reader["CreatedOnTime"]);
                        logs.Add(log);
                    }
                }

                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["ModifyOnDate"] != DBNull.Value && reader["ModifyOnDate"].ToString() != "N/A" &&
                        reader["ModifyByFirstName"] != DBNull.Value && reader["ModifyByFirstName"].ToString() != "N/A")
                    {
                        GetCompanyLog log = new GetCompanyLog();
                        DateTime modifyOn = Convert.ToDateTime(reader["ModifyOnTime"]);
                        string modifyTime = modifyOn.ToString("hh:mm:ss tt");
                        string message = "";
                        if (reader["IsDelete"] != DBNull.Value && Convert.ToBoolean(reader["IsDelete"]) == true)
                        {
                            message = "Company Document " + reader["DocumentFileName"].ToString() + " of " + reader["DocumentType"].ToString() + " Document Type is Deleted by " +
                            reader["ModifyByFirstName"].ToString() + " " + reader["ModifyByLastName"].ToString() + "(" + reader["ModifyByAccountNumber"].ToString() + ") on "
                            + reader["ModifyOnDate"].ToString() + " " + modifyTime;
                        }
                        log.LogMessage = message;
                        log.ModifyBy = reader["ModifyByFirstName"].ToString() + " " + reader["ModifyByLastName"].ToString() + "(" + reader["ModifyByAccountNumber"].ToString() + ")";
                        log.ModifyOnDate = reader["ModifyOnDate"].ToString();
                        log.ModifyOnTime = modifyTime;
                        log.DateOnUtc = Convert.ToDateTime(reader["ModifyOnTime"]);
                        logs.Add(log);
                    }
                }

                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["CreatedOnDate"] != DBNull.Value && reader["CreatedOnDate"].ToString() != "N/A"
                        && reader["CreatedByFirstName"] != DBNull.Value && reader["CreatedByFirstName"].ToString() != "N/A")
                    {
                        GetCompanyLog log = new GetCompanyLog();
                        DateTime createdOn = Convert.ToDateTime(reader["CreatedOnTime"]);
                        string createdTime = createdOn.ToString("hh:mm:ss tt");
                        string message = "New shareholder " + reader["ShareholderFirstName"].ToString() + " " + reader["ShareholderLastName"].ToString() + " with "
                            + reader["ShareholderSharePercentage"].ToString() + "% share is created for "
                            + reader["AssignedCompanyName"].ToString() + " Company by " +
                            reader["CreatedByFirstName"].ToString() + " " + reader["CreatedByLastName"].ToString() + "(" + reader["CreatedByAccountNumber"].ToString() + ") on "
                            + reader["CreatedOnDate"].ToString() + " " + createdTime;

                        log.LogMessage = message;
                        log.CreatedBy = reader["CreatedByFirstName"].ToString() + " " + reader["CreatedByLastName"].ToString() + "(" + reader["CreatedByAccountNumber"].ToString() + ")";
                        log.CreatedOnDate = reader["CreatedOnDate"].ToString();
                        log.CreatedOnTime = createdTime;
                        log.DateOnUtc = Convert.ToDateTime(reader["CreatedOnTime"]);
                        logs.Add(log);
                    }
                }

                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["ModifyOnDate"] != DBNull.Value && reader["ModifyOnDate"].ToString() != "N/A" &&
                        reader["ModifyByFirstName"] != DBNull.Value && reader["ModifyByFirstName"].ToString() != "N/A")
                    {
                        GetCompanyLog log = new GetCompanyLog();
                        DateTime modifyOn = Convert.ToDateTime(reader["ModifyOnTime"]);
                        string modifyTime = modifyOn.ToString("hh:mm:ss tt");
                        string message = "";
                        if (reader["IsDelete"] != DBNull.Value && Convert.ToBoolean(reader["IsDelete"]) == true)
                        {
                            message = "Shareholder " + reader["ShareholderFirstName"].ToString() + " " + reader["ShareholderLastName"].ToString() + " with "
                                + reader["ShareholderSharePercentage"].ToString() + "% share details is amendment of "
                            + reader["AssignedCompanyName"].ToString() + " Company is Deleted by " +
                            reader["ModifyByFirstName"].ToString() + " " + reader["ModifyByLastName"].ToString() + "(" + reader["ModifyByAccountNumber"].ToString() + ") on "
                            + reader["ModifyOnDate"].ToString() + " " + modifyTime;
                        }
                        else
                        {
                            message = "Shareholder " + reader["ShareholderFirstName"].ToString() + " " + reader["ShareholderLastName"].ToString() + " with "
                                + reader["ShareholderSharePercentage"].ToString() + "% share's details is amendment of "
                            + reader["AssignedCompanyName"].ToString() + " Company by " +
                            reader["ModifyByFirstName"].ToString() + " " + reader["ModifyByLastName"].ToString() + "(" + reader["ModifyByAccountNumber"].ToString() + ") on "
                            + reader["ModifyOnDate"].ToString() + " " + modifyTime;
                        }

                        log.LogMessage = message;
                        log.ModifyBy = reader["ModifyByFirstName"].ToString() + " " + reader["ModifyByLastName"].ToString() + "(" + reader["ModifyByAccountNumber"].ToString() + ")";
                        log.ModifyOnDate = reader["ModifyOnDate"].ToString();
                        log.ModifyOnTime = modifyTime;
                        log.DateOnUtc = Convert.ToDateTime(reader["ModifyOnTime"]);
                        logs.Add(log);
                    }
                }

                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["CreatedOnDate"] != DBNull.Value && reader["CreatedOnDate"].ToString() != "N/A"
                        && reader["CreatedByFirstName"] != DBNull.Value && reader["CreatedByFirstName"].ToString() != "N/A")
                    {
                        GetCompanyLog log = new GetCompanyLog();
                        DateTime createdOn = Convert.ToDateTime(reader["CreatedOnTime"]);
                        string createdTime = createdOn.ToString("hh:mm:ss tt");
                        string message = "New Document " + reader["FileName"].ToString() + " with Title ='" + reader["Title"].ToString() + "' is uploaded by " +
                            reader["CreatedByFirstName"].ToString() + " " + reader["CreatedByLastName"].ToString() + "(" + reader["CreatedByAccountNumber"].ToString() + ") on "
                            + reader["CreatedOnDate"].ToString() + " " + createdTime + " for " + reader["ShareholderFirstName"].ToString() + " " + reader["ShareholderLastName"].ToString() + " with "
                            + reader["ShareholderSharePercentage"].ToString() + "% share.";

                        log.LogMessage = message;
                        log.CreatedBy = reader["CreatedByFirstName"].ToString() + " " + reader["CreatedByLastName"].ToString() + "(" + reader["CreatedByAccountNumber"].ToString() + ")";
                        log.CreatedOnDate = reader["CreatedOnDate"].ToString();
                        log.CreatedOnTime = createdTime;
                        log.DateOnUtc = Convert.ToDateTime(reader["CreatedOnTime"]);
                        logs.Add(log);
                    }
                }

                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["ModifyOnDate"] != DBNull.Value && reader["ModifyOnDate"].ToString() != "N/A" &&
                        reader["ModifyByFirstName"] != DBNull.Value && reader["ModifyByFirstName"].ToString() != "N/A")
                    {
                        GetCompanyLog log = new GetCompanyLog();
                        DateTime modifyOn = Convert.ToDateTime(reader["ModifyOnTime"]);
                        string modifyTime = modifyOn.ToString("hh:mm:ss tt");
                        string message = "";
                        if (reader["IsDelete"] != DBNull.Value && Convert.ToBoolean(reader["IsDelete"]) == true)
                        {
                            message = "Document " + reader["FileName"].ToString() + " with Title ='" + reader["Title"].ToString() + "' is Deleted by " +
                            reader["ModifyByFirstName"].ToString() + " " + reader["ModifyByLastName"].ToString() + "(" + reader["ModifyByAccountNumber"].ToString() + ") on "
                            + reader["ModifyOnDate"].ToString() + " " + modifyTime + " for " + reader["ShareholderFirstName"].ToString() + " " + reader["ShareholderLastName"].ToString() + " with "
                            + reader["ShareholderSharePercentage"].ToString() + "% share.";
                        }
                        
                        log.LogMessage = message;
                        log.ModifyBy = reader["ModifyByFirstName"].ToString() + " " + reader["ModifyByLastName"].ToString() + "(" + reader["ModifyByAccountNumber"].ToString() + ")";
                        log.ModifyOnDate = reader["ModifyOnDate"].ToString();
                        log.ModifyOnTime = modifyTime;
                        log.DateOnUtc = Convert.ToDateTime(reader["ModifyOnTime"]);
                        logs.Add(log);
                    }
                }

                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["CreatedOnDate"] != DBNull.Value && reader["CreatedOnDate"].ToString() != "N/A"
                        && reader["CreatedByFirstName"] != DBNull.Value && reader["CreatedByFirstName"].ToString() != "N/A")
                    {
                        GetCompanyLog log = new GetCompanyLog();
                        DateTime createdOn = Convert.ToDateTime(reader["CreatedOnTime"]);
                        string createdTime = createdOn.ToString("hh:mm:ss tt");

                        string message = "";
                        if(reader["AssignedRole"].ToString() != "N/A")
                        {
                            message = "New Employee " + reader["UserFirstName"].ToString() + " " + reader["UserLastName"].ToString() + " (" + reader["UserAccountNumber"].ToString() + ")"
                                                        + " with " + reader["AssignedRole"].ToString() + " Role and Active Status equals to " + Convert.ToBoolean(reader["IsActive"]) + " is Created by " +
                                                        reader["CreatedByFirstName"].ToString() + " " + reader["CreatedByLastName"].ToString() + "(" + reader["CreatedByAccountNumber"].ToString() + ") on "
                                                        + reader["CreatedOnDate"].ToString() + " " + createdTime;
                        }
                        else
                        {
                            message = "New Employee " + reader["UserFirstName"].ToString() + " " + reader["UserLastName"].ToString() + " (" + reader["UserAccountNumber"].ToString() + ")"
                            + " with Active Status equals to " + Convert.ToBoolean(reader["IsActive"]) + " is Created by " +
                            reader["CreatedByFirstName"].ToString() + " " + reader["CreatedByLastName"].ToString() + "(" + reader["CreatedByAccountNumber"].ToString() + ") on "
                            + reader["CreatedOnDate"].ToString() + " " + createdTime;
                        }
                        

                        log.LogMessage = message;
                        log.CreatedBy = reader["CreatedByFirstName"].ToString() + " " + reader["CreatedByLastName"].ToString() + "(" + reader["CreatedByAccountNumber"].ToString() + ")";
                        log.CreatedOnDate = reader["CreatedOnDate"].ToString();
                        log.CreatedOnTime = createdTime;
                        log.DateOnUtc = Convert.ToDateTime(reader["CreatedOnTime"]);
                        logs.Add(log);
                    }
                }

                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["ModifyOnDate"] != DBNull.Value && reader["ModifyOnDate"].ToString() != "N/A" &&
                        reader["ModifyByFirstName"] != DBNull.Value && reader["ModifyByFirstName"].ToString() != "N/A")
                    {
                        GetCompanyLog log = new GetCompanyLog();
                        DateTime modifyOn = Convert.ToDateTime(reader["ModifyOnTime"]);
                        string modifyTime = modifyOn.ToString("hh:mm:ss tt");
                        string message = "";
                        if (reader["IsDelete"] != DBNull.Value && Convert.ToBoolean(reader["IsDelete"]) == true)
                        {
                            if(reader["AssignedRole"].ToString() != "N/A")
                            {
                                message = "Employee " + reader["UserFirstName"].ToString() + " " + reader["UserLastName"].ToString() + " (" + reader["UserAccountNumber"].ToString() + ")"
                                + " with " + reader["AssignedRole"].ToString() + " Role and Active Status equals to " + Convert.ToBoolean(reader["IsActive"]) + " is Deleted by " +
                                reader["ModifyByFirstName"].ToString() + " " + reader["ModifyByLastName"].ToString() + "(" + reader["ModifyByAccountNumber"].ToString() + ") on "
                                + reader["ModifyOnDate"].ToString() + " " + modifyTime;
                            }
                            else
                            {
                                message = "Employee " + reader["UserFirstName"].ToString() + " " + reader["UserLastName"].ToString() + " (" + reader["UserAccountNumber"].ToString() + ")"
                                + " with Active Status equals to " + Convert.ToBoolean(reader["IsActive"]) + " is Deleted by " +
                                reader["ModifyByFirstName"].ToString() + " " + reader["ModifyByLastName"].ToString() + "(" + reader["ModifyByAccountNumber"].ToString() + ") on "
                                + reader["ModifyOnDate"].ToString() + " " + modifyTime;
                            }
                            
                        }
                        else
                        {
                            if (reader["AssignedRole"].ToString() != "N/A")
                            {
                                message = "Details of Employee " + reader["UserFirstName"].ToString() + " " + reader["UserLastName"].ToString() + " (" + reader["UserAccountNumber"].ToString() + ")"
                                + " with Active Status equals to " + Convert.ToBoolean(reader["IsActive"]) + " is amendment by " +
                                reader["ModifyByFirstName"].ToString() + " " + reader["ModifyByLastName"].ToString() + "(" + reader["ModifyByAccountNumber"].ToString() + ") on "
                                + reader["ModifyOnDate"].ToString() + " " + modifyTime;
                            }
                            else
                            {
                                message = "Details of Employee " + reader["UserFirstName"].ToString() + " " + reader["UserLastName"].ToString() + " (" + reader["UserAccountNumber"].ToString() + ")"
                                + " with Active Status equals to " + Convert.ToBoolean(reader["IsActive"]) + " is amendment by " +
                                reader["ModifyByFirstName"].ToString() + " " + reader["ModifyByLastName"].ToString() + "(" + reader["ModifyByAccountNumber"].ToString() + ") on "
                                + reader["ModifyOnDate"].ToString() + " " + modifyTime;
                            }
                        }

                        log.LogMessage = message;
                        log.ModifyBy = reader["ModifyByFirstName"].ToString() + " " + reader["ModifyByLastName"].ToString() + "(" + reader["ModifyByAccountNumber"].ToString() + ")";
                        log.ModifyOnDate = reader["ModifyOnDate"].ToString();
                        log.ModifyOnTime = modifyTime;
                        log.DateOnUtc = Convert.ToDateTime(reader["ModifyOnTime"]);
                        logs.Add(log);
                    }
                }

                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["CreatedOnDate"] != DBNull.Value && reader["CreatedOnDate"].ToString() != "N/A"
                        && reader["CreatedByFirstName"] != DBNull.Value && reader["CreatedByFirstName"].ToString() != "N/A")
                    {
                        GetCompanyLog log = new GetCompanyLog();
                        DateTime createdOn = Convert.ToDateTime(reader["CreatedOnTime"]);
                        string createdTime = createdOn.ToString("hh:mm:ss tt");
                        string message = "";

                        message = "New Sales Person " + reader["SalesPersonFirstName"].ToString() + " " + reader["SalesPersonLastName"].ToString() + " is assigned to Company by " +
                        reader["CreatedByFirstName"].ToString() + " " + reader["CreatedByLastName"].ToString() + "(" + reader["CreatedByAccountNumber"].ToString() + ") on "
                        + reader["CreatedOnDate"].ToString() + " " + createdTime;

                        log.LogMessage = message;
                        log.CreatedBy = reader["CreatedByFirstName"].ToString() + " " + reader["CreatedByLastName"].ToString() + "(" + reader["CreatedByAccountNumber"].ToString() + ")";
                        log.CreatedOnDate = reader["CreatedOnDate"].ToString();
                        log.CreatedOnTime = createdTime;
                        log.DateOnUtc = Convert.ToDateTime(reader["CreatedOnTime"]);
                        logs.Add(log);
                    }
                }

                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["ModifyOnDate"] != DBNull.Value && reader["ModifyOnDate"].ToString() != "N/A" &&
                        reader["ModifyByFirstName"] != DBNull.Value && reader["ModifyByFirstName"].ToString() != "N/A")
                    {
                        GetCompanyLog log = new GetCompanyLog();
                        DateTime modifyOn = Convert.ToDateTime(reader["ModifyOnTime"]);
                        string modifyTime = modifyOn.ToString("hh:mm:ss tt");
                        string message = "";
                        if (reader["IsDelete"] != DBNull.Value && Convert.ToBoolean(reader["IsDelete"]) == true)
                        {
                            message = "Sales Person " + reader["SalesPersonFirstName"].ToString() + " " + reader["SalesPersonLastName"].ToString() + " is Removed from Company by " +
                            reader["ModifyByFirstName"].ToString() + " " + reader["ModifyByLastName"].ToString() + "(" + reader["ModifyByAccountNumber"].ToString() + ") on "
                            + reader["ModifyOnDate"].ToString() + " " + modifyTime;
                        }
                        log.LogMessage = message;
                        log.ModifyBy = reader["ModifyByFirstName"].ToString() + " " + reader["ModifyByLastName"].ToString() + "(" + reader["ModifyByAccountNumber"].ToString() + ")";
                        log.ModifyOnDate = reader["ModifyOnDate"].ToString();
                        log.ModifyOnTime = modifyTime;
                        log.DateOnUtc = Convert.ToDateTime(reader["ModifyOnTime"]);
                        logs.Add(log);
                    }
                }

                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["RequestedService"] != DBNull.Value && reader["RequestedService"].ToString() != "N/A"
                        && reader["ServiceRequestCreatedOn"] != DBNull.Value && reader["ServiceRequestCreatedOn"].ToString() != "N/A")
                    {
                        GetCompanyLog log = new GetCompanyLog();
                        DateTime createdOn = Convert.ToDateTime(reader["ServiceRequestCreatedOnTime"]);
                        string createdTime = createdOn.ToString("hh:mm:ss tt");

                        DateTime responseOn = Convert.ToDateTime(reader["ServiceRequestResponseCreatedOnTime"]);
                        string responseTime = createdOn.ToString("hh:mm:ss tt");

                        if (reader["RequestedService"] != DBNull.Value && reader["RequestedService"].ToString() != "N/A" &&
                            reader["Description"] != DBNull.Value && reader["Description"].ToString() != "N/A")
                        {
                            string message = "";
                            if (reader["RequestedService"].ToString() == reader["Description"].ToString())
                            {
                                message = "New Service Request - "+reader["RequestedService"].ToString() + "(" + reader["SerialNumber"].ToString() + ") is created by " +
                                         reader["RequestedBy"].ToString() + " on "
                                         + reader["ServiceRequestCreatedOn"].ToString() + " " + createdTime;

                                log.LogMessage = message;
                                log.CreatedBy = reader["RequestedBy"].ToString();
                                log.CreatedOnDate = reader["ServiceRequestCreatedOn"].ToString();
                                log.CreatedOnTime = createdTime;
                                log.DateOnUtc = Convert.ToDateTime(reader["ServiceRequestCreatedOnTime"]);
                                logs.Add(log);
                            }
                            else
                            {
                                message = "New response added to "+reader["RequestedService"].ToString() + "(" + reader["SerialNumber"].ToString() + ") by " +
                                        reader["ServiceRequestResponseCreatedBy"].ToString() + " on "
                                        + reader["ServiceRequestResponseCreatedOn"].ToString() + " " + responseTime + " with response message equals to " 
                                        + reader["Description"].ToString();

                                log.LogMessage = message;
                                log.CreatedBy = reader["ServiceRequestResponseCreatedBy"].ToString();
                                log.CreatedOnDate = reader["ServiceRequestResponseCreatedOn"].ToString();
                                log.CreatedOnTime = createdTime;
                                log.DateOnUtc = Convert.ToDateTime(reader["ServiceRequestResponseCreatedOnTime"]);
                                logs.Add(log);
                            }
                        }

                    }
                }

                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["ServiceRequestResponseCreatedBy"] != DBNull.Value && reader["ServiceRequestResponseCreatedBy"].ToString() != "N/A"
                        && reader["ServiceRequestResponseCreatedOn"] != DBNull.Value && reader["ServiceRequestResponseCreatedOn"].ToString() != "N/A")
                    {
                        GetCompanyLog log = new GetCompanyLog();
                        DateTime createdOn = Convert.ToDateTime(reader["ServiceRequestResponseCreatedOnTime"]);
                        string createdTime = createdOn.ToString("hh:mm:ss tt");

                        string message = "";
                        if (reader["IsDelete"] != DBNull.Value && Convert.ToBoolean(reader["IsDelete"]) == true)
                        {
                            message = "Service Request - " + reader["RequestedService"].ToString() + "(" + reader["SerialNumber"].ToString() + ") is Deleted by " +
                                        reader["ServiceRequestResponseCreatedBy"].ToString() + " on "
                                        + reader["ServiceRequestResponseCreatedOn"].ToString() + " " + createdTime;

                            log.LogMessage = message;
                            log.CreatedBy = reader["ServiceRequestResponseCreatedBy"].ToString();
                            log.CreatedOnDate = reader["ServiceRequestResponseCreatedOn"].ToString();
                            log.CreatedOnTime = createdTime;
                            log.DateOnUtc = Convert.ToDateTime(reader["ServiceRequestCreatedOnTime"]);
                            logs.Add(log);
                        }
                    }
                }

                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["CreatedOn"] != DBNull.Value && reader["CreatedOn"].ToString() != "N/A"
                        && reader["RequestedBy"] != DBNull.Value && reader["RequestedBy"].ToString() != "N/A")
                    {
                        GetCompanyLog log = new GetCompanyLog();
                        DateTime createdOn = Convert.ToDateTime(reader["CreatedOnTime"]);
                        string createdTime = createdOn.ToString("hh:mm:ss tt");
                        string message = "New Support Ticket - "+reader["Title"].ToString() + "(" + reader["TrackingNumber"].ToString() + ") is created by " +
                            reader["RequestedBy"].ToString() + " on "
                            + reader["CreatedOn"].ToString() + " " + createdTime;

                        log.LogMessage = message;
                        log.CreatedBy = reader["RequestedBy"].ToString();
                        log.CreatedOnDate = reader["CreatedOn"].ToString();
                        log.CreatedOnTime = createdTime;
                        log.DateOnUtc = Convert.ToDateTime(reader["CreatedOnTime"]);
                        logs.Add(log);
                    }
                }

                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["CreatedOn"] != DBNull.Value && reader["CreatedOn"].ToString() != "N/A"
                        && reader["RequestedBy"] != DBNull.Value && reader["RequestedBy"].ToString() != "N/A")
                    {
                        GetCompanyLog log = new GetCompanyLog();
                        DateTime createdOn = Convert.ToDateTime(reader["CreatedOnTime"]);
                        string createdTime = createdOn.ToString("hh:mm:ss tt");
                        string message = "New Support Ticket Response to " + reader["TrackingNumber"].ToString() + " with response message equals to "+ reader["Description"].ToString() 
                            + " is added by " +
                            reader["RequestedBy"].ToString() + " on "
                            + reader["CreatedOn"].ToString() + " " + createdTime;

                        log.LogMessage = message;
                        log.CreatedBy = reader["RequestedBy"].ToString();
                        log.CreatedOnDate = reader["CreatedOn"].ToString();
                        log.CreatedOnTime = createdTime;
                        log.DateOnUtc = Convert.ToDateTime(reader["CreatedOnTime"]);
                        logs.Add(log);
                    }
                }

                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["ModifyOn"] != DBNull.Value && reader["ModifyOn"].ToString() != "N/A" &&
                        reader["ModifyBy"] != DBNull.Value && reader["ModifyBy"].ToString() != "N/A")
                    {
                        GetCompanyLog log = new GetCompanyLog();
                        DateTime modifyOn = Convert.ToDateTime(reader["ModifyOnTime"]);
                        string modifyTime = modifyOn.ToString("hh:mm:ss tt");
                        string message = "";
                        if (reader["IsDelete"] != DBNull.Value && Convert.ToBoolean(reader["IsDelete"]) == true)
                        {
                            message = reader["RequestedService"].ToString() + "(" + reader["TrackingNumber"].ToString() + ") is deleted by " +
                            reader["ModifyBy"].ToString() + " on "
                            + reader["ModifyOn"].ToString() + " " + modifyTime;

                            log.LogMessage = message;
                            log.ModifyBy = reader["ModifyBy"].ToString();
                            log.ModifyOnDate = reader["ModifyOn"].ToString();
                            log.ModifyOnTime = modifyTime;
                            log.DateOnUtc = Convert.ToDateTime(reader["ModifyOnTime"]);
                            logs.Add(log);
                        }
                    }
                }

                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["CreatedOnDate"] != DBNull.Value && reader["CreatedOnDate"].ToString() != "N/A"
                        && reader["CreatedOnTime"] != DBNull.Value && reader["CreatedOnTime"].ToString() != "N/A")
                    {
                        GetCompanyLog log = new GetCompanyLog();
                        DateTime createdOn = Convert.ToDateTime(reader["CreatedOnTime"]);
                        string createdTime = createdOn.ToString("hh:mm:ss tt");
                        string message = "New Document " + reader["DocumentFileName"].ToString() + " with Title ='" + reader["DocumentType"].ToString() + "' is uploaded by " +
                            reader["CreatedByFirstName"].ToString() + " " + reader["CreatedByLastName"].ToString() + "(" + reader["CreatedByAccountNumber"].ToString() + ") on "
                            + reader["CreatedOnDate"].ToString() + " " + createdTime + " for " + reader["Employee"].ToString() + ".";
                        log.LogMessage = message;
                        log.CreatedBy = reader["CreatedByFirstName"].ToString() + " " + reader["CreatedByLastName"].ToString() + "(" + reader["CreatedByAccountNumber"].ToString() + ")";
                        log.CreatedOnDate = reader["CreatedOnDate"].ToString();
                        log.CreatedOnTime = createdTime;
                        log.DateOnUtc = Convert.ToDateTime(reader["CreatedOnTime"]);
                        logs.Add(log);
                    }
                }

                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["ModifyOn"] != DBNull.Value && reader["ModifyOn"].ToString() != "N/A"
                        && reader["ModifyOnTime"] != DBNull.Value && reader["ModifyOnTime"].ToString() != "N/A")
                    {
                        GetCompanyLog log = new GetCompanyLog();
                        DateTime createdOn = Convert.ToDateTime(reader["ModifyOnTime"]);
                        string createdTime = createdOn.ToString("hh:mm:ss tt");
                        if (reader["IsDelete"] != DBNull.Value && Convert.ToBoolean(reader["IsDelete"]) == true)
                        {
                            string message = "Document " + reader["DocumentFileName"].ToString() + " with Title ='" + reader["DocumentType"].ToString() + "' is Deleted by " +
                            reader["ModifyByFirstName"].ToString() + " " + reader["ModifyByLastName"].ToString() + "(" + reader["ModifyByAccountNumber"].ToString() + ") on "
                            + reader["ModifyOn"].ToString() + " " + createdTime + " for " + reader["Employee"].ToString() + ".";
                            log.LogMessage = message;
                            log.CreatedBy = reader["ModifyByFirstName"].ToString() + " " + reader["ModifyByLastName"].ToString() + "(" + reader["ModifyByAccountNumber"].ToString() + ")";
                            log.CreatedOnDate = reader["ModifyOn"].ToString();
                            log.CreatedOnTime = createdTime;
                            log.DateOnUtc = Convert.ToDateTime(reader["ModifyOnTime"]);
                            logs.Add(log);
                        }
                    }
                }

                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["CreatedOnDate"] != DBNull.Value && reader["CreatedOnDate"].ToString() != "N/A"
                        && reader["CreatedOnTime"] != DBNull.Value && reader["CreatedOnTime"].ToString() != "N/A")
                    {
                        GetCompanyLog log = new GetCompanyLog();
                        DateTime createdOn = Convert.ToDateTime(reader["CreatedOnTime"]);
                        string createdTime = createdOn.ToString("hh:mm:ss tt");
                        string message = "New Invoice "+reader["InvoiceNumber"].ToString()+" with " +reader["InvoiceTotal"].ToString() + " is created by " + reader["CreatedByFirstName"].ToString() 
                            +" " + reader["CreatedByLastName"] + " ("+ reader["CreatedByAccountNumber"] +") on "+ reader["CreatedOnDate"].ToString() + " " + createdTime;
                        log.LogMessage = message;
                        log.CreatedBy = reader["CreatedByFirstName"].ToString() + " " + reader["CreatedByLastName"].ToString() + "(" + reader["CreatedByAccountNumber"].ToString() + ")";
                        log.CreatedOnDate = reader["CreatedOnDate"].ToString();
                        log.CreatedOnTime = createdTime;
                        log.DateOnUtc = Convert.ToDateTime(reader["CreatedOnTime"]);
                        logs.Add(log);
                    }
                }

                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["ModifyOn"] != DBNull.Value && reader["ModifyOn"].ToString() != "N/A"
                        && reader["ModifyOnTime"] != DBNull.Value && reader["ModifyOnTime"].ToString() != "N/A")
                    {
                        GetCompanyLog log = new GetCompanyLog();
                        DateTime createdOn = Convert.ToDateTime(reader["ModifyOnTime"]);
                        string createdTime = createdOn.ToString("hh:mm:ss tt");
                        string message = "Invoice " + reader["InvoiceNumber"].ToString() + " with " + reader["InvoiceTotal"].ToString() + " is Deleted by " + reader["ModifyByFirstName"].ToString()
                            + " " + reader["ModifyByLastName"] + " (" + reader["ModifyByAccountNumber"] + ") on " + reader["ModifyOn"].ToString() + " " + createdTime;
                        log.LogMessage = message;
                        log.CreatedBy = reader["ModifyByFirstName"].ToString() + " " + reader["ModifyByLastName"].ToString() + "(" + reader["ModifyByAccountNumber"].ToString() + ")";
                        log.CreatedOnDate = reader["ModifyOn"].ToString();
                        log.CreatedOnTime = createdTime;
                        log.DateOnUtc = Convert.ToDateTime(reader["ModifyOnTime"]);
                        logs.Add(log);
                    }
                }

                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["ModifyOn"] != DBNull.Value && reader["ModifyOn"].ToString() != "N/A"
                        && reader["ModifyOnTime"] != DBNull.Value && reader["ModifyOnTime"].ToString() != "N/A")
                    {
                        GetCompanyLog log = new GetCompanyLog();
                        DateTime createdOn = Convert.ToDateTime(reader["ModifyOnTime"]);
                        string createdTime = createdOn.ToString("hh:mm:ss tt");
                        string message = "";
                        if (reader["IsDelete"] != DBNull.Value &&  Convert.ToBoolean(reader["IsDelete"]) == true)
                        {
                            message = "Invoice " + reader["InvoiceNumber"].ToString() + "'s one entry is Deleted by " + reader["ModifyByFirstName"].ToString()
                            + " " + reader["ModifyByLastName"] + " (" + reader["ModifyByAccountNumber"] + ") on " + reader["ModifyOn"].ToString() + " " + createdTime + " with Amount equals to "+
                            reader["Amount"].ToString() + ", Quantity equals to " + reader["Quantity"].ToString() + " , Total Amount equals to "+ reader["TotalAmount"].ToString() +
                            ", Vat equals to "+reader["Vat"].ToString() + ", Vat Amount equals to "+ reader["VatAmount"].ToString() +" and Grand Total equals to "+ reader["GrandTotal"].ToString();
                        }
                        else
                        {
                            message = "Invoice " + reader["InvoiceNumber"].ToString() + "'s one entry is Updated by " + reader["ModifyByFirstName"].ToString()
                            + " " + reader["ModifyByLastName"] + " (" + reader["ModifyByAccountNumber"] + ") on " + reader["ModifyOn"].ToString() + " " + createdTime + " with Amount equals to " +
                            reader["Amount"].ToString() + ", Quantity equals to " + reader["Quantity"].ToString() + " , Total Amount equals to " + reader["TotalAmount"].ToString() +
                            ", Vat equals to " + reader["Vat"].ToString() + ", Vat Amount equals to " + reader["VatAmount"].ToString() + " and Grand Total equals to " + reader["GrandTotal"].ToString();
                        }
                        
                        log.LogMessage = message;
                        log.CreatedBy = reader["ModifyByFirstName"].ToString() + " " + reader["ModifyByLastName"].ToString() + "(" + reader["ModifyByAccountNumber"].ToString() + ")";
                        log.CreatedOnDate = reader["ModifyOn"].ToString();
                        log.CreatedOnTime = createdTime;
                        log.DateOnUtc = Convert.ToDateTime(reader["ModifyOnTime"]);
                        logs.Add(log);
                    }
                }

                var data = (from log in logs
                            orderby log.DateOnUtc descending
                            select log).ToList();

                con.Close();
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Get Company List With Pagination                                                                                     -- Yashasvi TBC (22-12-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<GetCompanyDetailsWithPaginationModel> GetCompanyListWithPagination(int page,int pageSize,string sortBy,string sortingDirection,string searchValue)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetCompanyDetailsWithPaginationModel model = new GetCompanyDetailsWithPaginationModel();
                int totalCompanies = 0;
                List<GetCompanyDetailsWithPagination> companies = new List<GetCompanyDetailsWithPagination>();
                SqlCommand command = new SqlCommand("USP_Admin_GetAllCompanies", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@skipRows", page);
                command.Parameters.AddWithValue("@RowsOfPage", pageSize);
                command.Parameters.AddWithValue("@searchPrefix", searchValue);
                command.Parameters.AddWithValue("@sortColumn", sortBy);
                command.Parameters.AddWithValue("@sortDirection", sortingDirection);
                con.Open();
                SqlDataReader dr = await command.ExecuteReaderAsync();
                while (dr.Read())
                {
                    GetCompanyDetailsWithPagination comp = new GetCompanyDetailsWithPagination();
                    comp.Id = Convert.ToInt32(dr["Id"].ToString());
                    if (dr["AccountNumber"] != DBNull.Value && dr["AccountNumber"].ToString() != "N/A")
                        comp.AccountNumber = dr["AccountNumber"].ToString();
                    else
                        comp.AccountNumber = "---";
                    if (dr["CompanyName"] != DBNull.Value && dr["CompanyName"].ToString() != "N/A")
                        comp.CompanyName = dr["CompanyName"].ToString();
                    else
                        comp.CompanyName = "---";
                    if (dr["CompanyType"] != DBNull.Value && dr["CompanyType"].ToString() != "N/A")
                        comp.CompanyType = dr["CompanyType"].ToString();
                    else
                        comp.CompanyType = "---";
                    if (dr["MobileNumber"] != DBNull.Value && dr["MobileNumber"].ToString() != "N/A")
                        comp.MobileNumber = dr["MobileNumber"].ToString();
                    else
                        comp.MobileNumber = "---";
                    if (dr["EmailID"] != DBNull.Value && dr["EmailID"].ToString() != "N/A")
                        comp.EmailID = dr["EmailID"].ToString();
                    else
                        comp.EmailID = "---";
                    if (dr["LicenseType"] != DBNull.Value && dr["LicenseType"].ToString() != "N/A")
                        comp.LicenseType = dr["LicenseType"].ToString();
                    else
                        comp.LicenseType = "---";
                    if (dr["ShareholderName"] != DBNull.Value && dr["ShareholderName"].ToString() != "N/A")
                        comp.ShareholderName = dr["ShareholderName"].ToString();
                    else
                        comp.ShareholderName = "---";
                    if (dr["ShareholderSharePercentage"] != DBNull.Value && dr["ShareholderSharePercentage"].ToString() != "N/A")
                        comp.ShareholderSharePercentage = dr["ShareholderSharePercentage"].ToString();
                    else
                        comp.ShareholderSharePercentage = "---";
                    comp.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    comp.ShareholderIsActive = Convert.ToBoolean(dr["ShareholderIsActive"]);
                    companies.Add(comp);
                }
                con.Close();

                command.Parameters.Clear();

                command = new SqlCommand("USP_Admin_GetAllCompaniesCount", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@searchPrefix", searchValue);
                con.Open();
                dr = await command.ExecuteReaderAsync();
                while (dr.Read())
                {
                    if (dr["Id"] != DBNull.Value)
                        totalCompanies++;
                }
                con.Close();
                model.totalCompanies = totalCompanies;
                model.getCompanyDetails = companies;
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
        public List<GetCompanyListForExport> GetCompanyListForExcelExport()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetCompanyListForExport> getCompanyLists= new List<GetCompanyListForExport>();
                SqlCommand command = new SqlCommand("USP_Admin_GetCompanyListForExport", con);
                command.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while(reader.Read())
                {
                    GetCompanyListForExport model = new GetCompanyListForExport();
                    if (reader["CompanyCode"] != DBNull.Value)
                        model.CompanyCode = reader["CompanyCode"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    getCompanyLists.Add(model);
                }
                con.Close();
                return getCompanyLists;
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
        /// <summary>
        /// Company sub type                                                                                                                                    -- Yashasvi (10-10-2024)
        /// </summary>
        /// <param name="companyType"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<string>> GetCompanySubTypePrefix(string prefix)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> subType = new List<string>();

                SqlCommand command = new SqlCommand("USP_Admin_GetCompanySubType", connection);
                command.CommandType = CommandType.StoredProcedure;
                if (prefix != null && prefix != "")
                    command.Parameters.AddWithValue("@prefix", prefix);

                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["SubCompanyType"] != DBNull.Value)
                        if (!subType.Contains(reader["SubCompanyType"].ToString()))
                            subType.Add(reader["SubCompanyType"].ToString());
                }
                connection.Close();

                return subType;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
        /// <summary>
        /// Get weekly company sub type report                                                                                                  -- Yashasvi (11-10-2024)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<GetWeeklyCompanySubTypeReportModel>> GetWeeklyCompanySubTypeReport()
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetWeeklyCompanySubTypeReportModel> report = new List<GetWeeklyCompanySubTypeReportModel>();
                List<string> totalSubType = new List<string>();

                SqlCommand command = new SqlCommand("USP_Admin_GetCompanySubTypes", connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["SubCompanyType"] != DBNull.Value)
                        if (!totalSubType.Contains(reader["SubCompanyType"].ToString()))
                            totalSubType.Add(reader["SubCompanyType"].ToString());
                }
                connection.Close();
                if(totalSubType.Count > 0)
                {
                    foreach (string subType in totalSubType)
                    {
                        GetWeeklyCompanySubTypeReportModel model = new GetWeeklyCompanySubTypeReportModel();
                        command.Parameters.Clear();
                        command = new SqlCommand("USP_Admin_GetWeeklyCompanySubTypeReport", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@subType", subType);

                        connection.Open();
                        reader = await command.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            if (reader["TotalCompanies"] != DBNull.Value)
                                model.TotalMainlandCompanies = Convert.ToInt32(reader["TotalCompanies"]);
                            string color = String.Format("#{0:X6}", new Random().Next(0x1000000));
                            model.color = color;
                        }
                        connection.Close();
                        model.CompanySubType = subType;
                        report.Add(model);
                    }
                }
                
                return report;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Get monthly company sub type report                                                                                                  -- Yashasvi (11-10-2024)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<GetMonthlyCompanySubTypeReportModel>> GetMonthlyCompanySubTypeReport()
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetMonthlyCompanySubTypeReportModel> report = new List<GetMonthlyCompanySubTypeReportModel>();
                List<string> totalSubType = new List<string>();

                SqlCommand command = new SqlCommand("USP_Admin_GetCompanySubTypes", connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["SubCompanyType"] != DBNull.Value)
                        if (!totalSubType.Contains(reader["SubCompanyType"].ToString()))
                            totalSubType.Add(reader["SubCompanyType"].ToString());
                }
                connection.Close();
                if (totalSubType.Count > 0)
                {
                    foreach (string subType in totalSubType)
                    {
                        GetMonthlyCompanySubTypeReportModel model = new GetMonthlyCompanySubTypeReportModel();
                        command.Parameters.Clear();
                        command = new SqlCommand("USP_Admin_GetMonthlyCompanySubTypeReport", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@subType", subType);

                        connection.Open();
                        reader = await command.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            if (reader["TotalCompanies"] != DBNull.Value)
                                model.TotalMainlandCompanies = Convert.ToInt32(reader["TotalCompanies"]);
                            string color = String.Format("#{0:X6}", new Random().Next(0x1000000));
                            model.color = color;
                        }

                        connection.Close();
                        model.CompanySubType = subType;
                        report.Add(model);
                    }
                }

                return report;
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

        /// <summary>
        /// Get yearly company sub type report                                                                                                  -- Yashasvi (11-10-2024)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<GetYearlyCompanySubTypeReportModel>> GetYearlyCompanySubTypeReport()
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetYearlyCompanySubTypeReportModel> report = new List<GetYearlyCompanySubTypeReportModel>();
                List<string> totalSubType = new List<string>();

                SqlCommand command = new SqlCommand("USP_Admin_GetCompanySubTypes", connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["SubCompanyType"] != DBNull.Value)
                        if (!totalSubType.Contains(reader["SubCompanyType"].ToString()))
                            totalSubType.Add(reader["SubCompanyType"].ToString());
                }
                connection.Close();
                if (totalSubType.Count > 0)
                {
                    foreach (string subType in totalSubType)
                    {
                        GetYearlyCompanySubTypeReportModel model = new GetYearlyCompanySubTypeReportModel();
                        command.Parameters.Clear();
                        command = new SqlCommand("USP_Admin_GetYearlyCompanySubTypeReport", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@subType", subType);

                        connection.Open();
                        reader = await command.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            if (reader["TotalCompanies"] != DBNull.Value)
                                model.TotalMainlandCompanies = Convert.ToInt32(reader["TotalCompanies"]);
                            string color = String.Format("#{0:X6}", new Random().Next(0x1000000));
                            model.color = color;
                        }

                        connection.Close();
                        model.CompanySubType = subType;
                        report.Add(model);
                    }
                }

                return report;
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

        /// <summary>
        /// Create new company                                                                                                                                                  -- Yashasvi (29-10-2024)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<int> Create(SaveCompanyModel model)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _companyId = 0;

                SqlCommand command = new SqlCommand("USP_Admin_Insert_Update_Company", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyType", model.CompanySubType);
                command.Parameters.AddWithValue("@CompanyCode", model.AccountNumber);
                command.Parameters.AddWithValue("@OwnerId", model.CompanyOwnerId);
                command.Parameters.AddWithValue("@OwnerName", model.CompanyOwner);
                command.Parameters.AddWithValue("@CompanyName", model.CompanyName);
                if(model.MobileNumber != null && model.MobileNumber != "")
                {
                    command.Parameters.AddWithValue("@MobileCountry", model.MainContactNumberCountry);
                    command.Parameters.AddWithValue("@MobileNumber", model.MobileNumber);
                }
                if(model.EmailID != null && model.EmailID != "")
                    command.Parameters.AddWithValue("@EmailAddress", model.EmailID);
                if(model.CompanyStartingDate != null && model.CompanyStartingDate != "")
                    command.Parameters.AddWithValue("@CompanyCreationDate", model.CompanyStartingDate);
                if(model.City != null && model.City != "")
                    command.Parameters.AddWithValue("@City", model.City);
                if(model.Country != null && model.Country != null)
                    command.Parameters.AddWithValue("@Country", model.Country);
                if (model.IsTRN)
                {
                    command.Parameters.AddWithValue("@IsTRNNumber", model.IsTRN);
                    command.Parameters.AddWithValue("@TRNNumber", model.TRN);
                    command.Parameters.AddWithValue("@TRNCreation", model.TRNCreationDate);
                }
                if (model.IsCorporateText)
                {
                    command.Parameters.AddWithValue("@IsCorporateText", model.IsCorporateText);
                    command.Parameters.AddWithValue("@CorporateText", model.CorporateText);
                }
                
                command.Parameters.AddWithValue("@IsActive", model.IsActive);
                command.Parameters.AddWithValue("@UserId", model.CreatedBy);

                connection.Open();
                _companyId = Convert.ToInt32(await command.ExecuteScalarAsync());
                connection.Close();

                if (_companyId > 0)
                {
                    if (model.SalesPersonId != null)
                    {
                        if (model.SalesPersonId.Count > 0)
                        {
                            for (int index = 0; index < model.SalesPersonId.Count; index++)
                            {
                                command = new SqlCommand("USP_Admin_SaveAssignedSalesPerson", connection);
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@Status", Operation.Insert);
                                command.Parameters.AddWithValue("@SalesPersonId", model.SalesPersonId[index]);
                                command.Parameters.AddWithValue("@CompanyId", _companyId);
                                command.Parameters.AddWithValue("@UserId", model.CreatedBy);
                                connection.Open();
                                var _returnCompanyId = (int)command.ExecuteScalar();
                                connection.Close();
                                command.Parameters.Clear();
                            }
                        }
                    }
                    if (model.RMPersonId != null)
                    {
                        if (model.RMPersonId.Count > 0)
                        {
                            for (int index = 0; index < model.RMPersonId.Count; index++)
                            {
                                command.Parameters.Clear();
                                command = new SqlCommand("USP_Admin_Insert_AssignedCompanyRMTeam", connection);
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@Id", model.RMPersonId[index]);
                                command.Parameters.AddWithValue("@CompanyId", _companyId);
                                command.Parameters.AddWithValue("@UserId", model.CreatedBy);

                                connection.Open();
                                await command.ExecuteNonQueryAsync();
                                connection.Close();
                            }
                        }
                    }
                }

                return _companyId;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Get Company Details For Update                                                                                                                                 -- Yashasvi (29-10-2024)
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<UpdateCompanyModel> GetCompanyDetailsByCompanyId(int companyId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                UpdateCompanyModel model = new UpdateCompanyModel();

                SqlCommand command = new SqlCommand("USP_Select_GetCompanyDetailsById", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", companyId);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["CompanyName"] != DBNull.Value && reader["CompanyName"].ToString() != "N/A")
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["CompanyType"] != DBNull.Value && reader["CompanyType"].ToString() != "N/A")
                        model.CompanySubType = reader["CompanyType"].ToString();
                    if (reader["CompanyCode"] != DBNull.Value && reader["CompanyCode"].ToString() != "N/A")
                        model.AccountNumber = reader["CompanyCode"].ToString();
                    if (reader["MainContactNumberCountry"] != DBNull.Value && reader["MainContactNumberCountry"].ToString() != "N/A")
                        model.MainContactNumberCountry = reader["MainContactNumberCountry"].ToString();
                    if (reader["MobileNumber"] != DBNull.Value && reader["MobileNumber"].ToString() != "N/A")
                        model.MobileNumber = reader["MobileNumber"].ToString();
                    if (reader["EmailID"] != DBNull.Value && reader["EmailID"].ToString() != "N/A")
                        model.EmailID = reader["EmailID"].ToString();
                    if (reader["CompanyStartingDate"] != DBNull.Value && reader["CompanyStartingDate"].ToString() != "N/A")
                        model.CompanyStartingDate = reader["CompanyStartingDate"].ToString();
                    if (reader["City"] != DBNull.Value && reader["City"].ToString() != "N/A")
                        model.City = reader["City"].ToString();
                    if (reader["Country"] != DBNull.Value && reader["Country"].ToString() != "N/A")
                        model.Country = reader["Country"].ToString();
                    if (reader["IsTRN"] != DBNull.Value)
                        model.IsTRN = Convert.ToBoolean(reader["IsTRN"]);
                    if (reader["TRN"] != DBNull.Value && reader["TRN"].ToString() != "N/A")
                        model.TRN = reader["TRN"].ToString();
                    if (reader["TRNCreationDate"] != DBNull.Value && reader["TRNCreationDate"].ToString() != "N/A")
                        model.TRNCreationDate = reader["TRNCreationDate"].ToString();
                    if (reader["IsCorporateText"] != DBNull.Value)
                        model.IsCorporateText = Convert.ToBoolean(reader["IsCorporateText"]);
                    if (reader["CorporateText"] != DBNull.Value && reader["CorporateText"].ToString() != "N/A")
                        model.CorporateText = reader["CorporateText"].ToString();
                    if (reader["IsActive"] != DBNull.Value)
                        model.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    if (reader["Ownername"] != DBNull.Value && reader["Ownername"].ToString() != "N/A")
                        model.CompanyOwner = reader["Ownername"].ToString();
                    if (reader["OwnerId"] != DBNull.Value)
                        model.CompanyOwnerId = Convert.ToInt32(reader["OwnerId"]);
                }

                connection.Close();

                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_SaveAssignedSalesPerson", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.GetById);
                command.Parameters.AddWithValue("@CompanyId", companyId);
                connection.Open();
                reader = command.ExecuteReader();
                model.SalesPersonId = new List<int>();
                while (reader.Read())
                {
                    int salesPersonId = Convert.ToInt32(reader["SalesPersonId"]);
                    model.SalesPersonId.Add(salesPersonId);
                }
                connection.Close();

                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_Select_GetAssignedRMTeamToCompany", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", companyId);
                connection.Open();
                reader = command.ExecuteReader();
                model.RMPersonId = new List<int>();
                while (reader.Read())
                {
                    int Id = Convert.ToInt32(reader["Id"]);
                    model.RMPersonId.Add(Id);
                }
                connection.Close();

                return model;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
        /// <summary>
        /// Update company                                                                                                                                                  -- Yashasvi (29-10-2024)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<int> Edit(UpdateCompanyModel model)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _companyId = 0;

                SqlCommand command = new SqlCommand("USP_Admin_Insert_Update_Company", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", model.Id);
                command.Parameters.AddWithValue("@CompanyType", model.CompanySubType);
                command.Parameters.AddWithValue("@CompanyCode", model.AccountNumber);
                command.Parameters.AddWithValue("@OwnerName", model.CompanyOwner);
                command.Parameters.AddWithValue("@OwnerId", model.CompanyOwnerId);
                command.Parameters.AddWithValue("@CompanyName", model.CompanyName);
                if (model.MobileNumber != null && model.MobileNumber != "")
                {
                    command.Parameters.AddWithValue("@MobileCountry", model.MainContactNumberCountry);
                    command.Parameters.AddWithValue("@MobileNumber", model.MobileNumber);
                }
                if (model.EmailID != null && model.EmailID != "")
                    command.Parameters.AddWithValue("@EmailAddress", model.EmailID);
                if (model.CompanyStartingDate != null && model.CompanyStartingDate != "")
                    command.Parameters.AddWithValue("@CompanyCreationDate", model.CompanyStartingDate);
                if (model.City != null && model.City != "")
                    command.Parameters.AddWithValue("@City", model.City);
                if (model.Country != null && model.Country != null)
                    command.Parameters.AddWithValue("@Country", model.Country);
                if (model.IsTRN)
                {
                    command.Parameters.AddWithValue("@IsTRNNumber", model.IsTRN);
                    command.Parameters.AddWithValue("@TRNNumber", model.TRN);
                    command.Parameters.AddWithValue("@TRNCreation", model.TRNCreationDate);
                }
                if (model.IsCorporateText)
                {
                    command.Parameters.AddWithValue("@IsCorporateText", model.IsCorporateText);
                    command.Parameters.AddWithValue("@CorporateText", model.CorporateText);
                }

                command.Parameters.AddWithValue("@IsActive", model.IsActive);
                command.Parameters.AddWithValue("@UserId", model.CreatedBy);

                connection.Open();
                _companyId = Convert.ToInt32(await command.ExecuteScalarAsync());
                connection.Close();

                if (_companyId > 0)
                {
                    if (model.SalesPersonId != null)
                    {
                        if (model.SalesPersonId.Count > 0)
                        {
                            for (int index = 0; index < model.SalesPersonId.Count; index++)
                            {
                                command = new SqlCommand("USP_Admin_SaveAssignedSalesPerson", connection);
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@Status", Operation.Insert);
                                command.Parameters.AddWithValue("@SalesPersonId", model.SalesPersonId[index]);
                                command.Parameters.AddWithValue("@CompanyId", _companyId);
                                command.Parameters.AddWithValue("@UserId", model.CreatedBy);
                                connection.Open();
                                await command.ExecuteNonQueryAsync();
                                connection.Close();
                                command.Parameters.Clear();
                            }
                        }
                    }
                    if (model.RMPersonId != null)
                    {
                        if (model.RMPersonId.Count > 0)
                        {
                            for (int index = 0; index < model.RMPersonId.Count; index++)
                            {
                                command.Parameters.Clear();
                                command = new SqlCommand("USP_Admin_Insert_AssignedCompanyRMTeam", connection);
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@Id", model.RMPersonId[index]);
                                command.Parameters.AddWithValue("@CompanyId", _companyId);
                                command.Parameters.AddWithValue("@UserId", model.CreatedBy);

                                connection.Open();
                                await command.ExecuteNonQueryAsync();
                                connection.Close();
                            }
                        }
                    }
                }

                return _companyId;
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
