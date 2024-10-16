using DIBN.Areas.Admin.Data;
using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using DocumentFormat.OpenXml.Office.Word;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Areas.Admin.Repository
{
    public class SalesPersonRepository : ISalesPersonRepository
    {
        private readonly Connection _dataSetting;
        private readonly EncryptionService _encryptionService;
        public SalesPersonRepository(Connection dataSetting, EncryptionService encryptionService)
        {
            _dataSetting = dataSetting;
            _encryptionService = encryptionService;
        }
        /// <summary>
        /// Get All Sales Person List                                                                                   -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<SalesPersonViewModel> GetAllSalesPersons()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<SalesPersonViewModel> salesPersons = new List<SalesPersonViewModel>();
                SqlCommand command = new SqlCommand("USP_Admin_SalesPersonOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Show);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    SalesPersonViewModel salesPerson = new SalesPersonViewModel();
                    salesPerson.Id = Convert.ToInt32(reader["Id"]);
                    salesPerson.FirstName = reader["FirstName"].ToString();
                    salesPerson.LastName = reader["LastName"].ToString();
                    salesPerson.EmailId = reader["EmailId"].ToString();
                    salesPerson.PhoneNumber = reader["PhoneNumber"].ToString();
                    salesPerson.CountryOfRecidence = reader["CountryOfRecidence"].ToString();
                    salesPerson.PassportNumber = reader["PassportNumber"].ToString();
                    salesPerson.PassportExpiryDate = reader["PassportExpiryDate"].ToString();
                    salesPerson.Designation = Convert.ToInt32(reader["Designation"].ToString());
                    salesPerson.Role = reader["Role"].ToString();
                    salesPerson.VisaExpiryDate = reader["VisaExpiryDate"].ToString();
                    salesPerson.InsuarnceCompany = reader["InsuranceCompany"].ToString();
                    salesPerson.InsuranceExpiryDate = reader["InsuarnceExpiryDate"].ToString();
                    salesPerson.Company = reader["Company"].ToString();
                    salesPerson.IsLogin = Convert.ToBoolean(reader["IsLogin"]);
                    salesPerson.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    salesPerson.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    salesPerson.CreatedOn = reader["CreatedOn"].ToString();
                    salesPerson.ModifyOn = reader["ModifyOn"].ToString();
                    salesPersons.Add(salesPerson);
                }
                con.Close();
                return salesPersons;
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
        /// Create New Sales Person                                                                                     -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="salesPerson"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int CreateNew(SalesPersonViewModel salesPerson)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                var _Password = _encryptionService.EncryptText(salesPerson.Password);
                int _returnId = 0, _returnCompanyId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_SalesPersonOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Insert);
                command.Parameters.AddWithValue("@FirstName", salesPerson.FirstName);
                command.Parameters.AddWithValue("@LastName", salesPerson.LastName);
                command.Parameters.AddWithValue("@EmailId", salesPerson.EmailId);
                command.Parameters.AddWithValue("@MCode", salesPerson.MCountry);
                command.Parameters.AddWithValue("@PhoneNumber", salesPerson.PhoneNumber);
                command.Parameters.AddWithValue("@CountryOfRecidence", salesPerson.CountryOfRecidence);
                if (salesPerson.PassportNumber != "N/A" && salesPerson.PassportNumber != "")
                    command.Parameters.AddWithValue("@PassportNumber", salesPerson.PassportNumber);
                if (salesPerson.PassportExpiryDate != "N/A" && salesPerson.PassportExpiryDate != "")
                    command.Parameters.AddWithValue("@PassportExpiryDate", salesPerson.PassportExpiryDate);
                command.Parameters.AddWithValue("@Designation", salesPerson.Designation);
                if (salesPerson.VisaExpiryDate != "N/A" && salesPerson.VisaExpiryDate != "")
                    command.Parameters.AddWithValue("@VisaExpiryDate", salesPerson.VisaExpiryDate);
                if (salesPerson.InsuarnceCompany != "N/A" && salesPerson.InsuarnceCompany != "")
                    command.Parameters.AddWithValue("@InsuranceCompany", salesPerson.InsuarnceCompany);
                if (salesPerson.InsuranceExpiryDate != "N/A" && salesPerson.InsuranceExpiryDate != "")
                    command.Parameters.AddWithValue("@InsuranceExpiryDate", salesPerson.InsuranceExpiryDate);
                command.Parameters.AddWithValue("@Password", _Password);
                command.Parameters.AddWithValue("@IsLogin", salesPerson.IsLogin);
                command.Parameters.AddWithValue("@IsActive", salesPerson.IsActive);
                command.Parameters.AddWithValue("@CreatedBy", salesPerson.CreatedBy);
                con.Open();
                _returnId = (int)command.ExecuteScalar();
                con.Close();
                command.Parameters.Clear();
                if (_returnId > 0)
                {
                    for (int index = 0; index < salesPerson.CompanyId.Count; index++)
                    {
                        command = new SqlCommand("USP_Admin_SaveAssignedSalesPerson", con);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Status", Operation.Insert);
                        command.Parameters.AddWithValue("@SalesPersonId", _returnId);
                        command.Parameters.AddWithValue("@UserId", salesPerson.CreatedBy);
                        command.Parameters.AddWithValue("@CompanyId", salesPerson.CompanyId[index]);
                        con.Open();
                        _returnCompanyId = (int)command.ExecuteScalar();
                        con.Close();
                    }

                }

                return _returnCompanyId != 0 ? _returnCompanyId : _returnId;
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
        /// Get Sales Person's Complete Details By Id                                                                   -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public SalesPersonViewModel GetSalesPersonDetail(int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                SalesPersonViewModel details = new SalesPersonViewModel();
                SqlCommand command = new SqlCommand("USP_Admin_SalesPersonOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.GetById);
                command.Parameters.AddWithValue("@Id", Id);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    details.Id = Convert.ToInt32(reader["Id"]);
                    details.FirstName = reader["FirstName"].ToString();
                    details.LastName = reader["LastName"].ToString();
                    details.EmailId = reader["EmailId"].ToString();
                    details.MCountry = reader["MCode"].ToString();
                    details.PhoneNumber = reader["PhoneNumber"].ToString();
                    details.PassportNumber = reader["PassportNumber"].ToString();
                    details.PassportExpiryDate = reader["PassportExpiryDate"].ToString();
                    details.VisaExpiryDate = reader["VisaExpiryDate"].ToString();
                    details.InsuranceExpiryDate = reader["InsuarnceExpiryDate"].ToString();
                    details.InsuarnceCompany = reader["InsuranceCompany"].ToString();
                    details.CountryOfRecidence = reader["CountryOfRecidence"].ToString();
                    details.Designation = Convert.ToInt32(reader["Designation"].ToString());
                    details.Password = reader["Password"].ToString();
                    details.Company = reader["Company"].ToString();
                    var _Password = _encryptionService.DecryptText(details.Password);
                    details.Password = _Password;
                    details.IsLogin = Convert.ToBoolean(reader["IsLogin"]);
                    details.IsActive = Convert.ToBoolean(reader["IsActive"]);
                }
                con.Close();
                command.Parameters.Clear();
                reader.Close();
                command = new SqlCommand("USP_Admin_SaveAssignedSalesPerson", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Show);
                command.Parameters.AddWithValue("@SalesPersonId", Id);
                con.Open();
                reader = command.ExecuteReader();
                details.CompanyId = new List<int>();
                while (reader.Read())
                {
                    int CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    details.CompanyId.Add(CompanyId);
                }
                con.Close();
                return details;
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


        public async Task<GetSalesPersonDetailsForLoginModel> GetSalesPersonDetailsForLogin(int Id)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetSalesPersonDetailsForLoginModel model = new GetSalesPersonDetailsForLoginModel();
                SqlCommand command = new SqlCommand("USP_Admin_GetSalesPersonDetailsForLogin", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", Id);
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    model.Id = Convert.ToInt32(reader["Id"]);
                    model.Email = reader["EmailId"].ToString();
                    model.Password = reader["Password"].ToString();
                    var _Password = _encryptionService.DecryptText(model.Password);
                    model.Password = _Password;
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
        /// Update Sales Person's Details                                                                               -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="salesPerson"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int Update(SalesPersonViewModel salesPerson)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                var _Password = _encryptionService.EncryptText(salesPerson.Password);
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_SalesPersonOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Insert);
                command.Parameters.AddWithValue("@Id", salesPerson.Id);
                command.Parameters.AddWithValue("@FirstName", salesPerson.FirstName);
                command.Parameters.AddWithValue("@LastName", salesPerson.LastName);
                command.Parameters.AddWithValue("@EmailId", salesPerson.EmailId);
                command.Parameters.AddWithValue("@MCode", salesPerson.MCountry);
                command.Parameters.AddWithValue("@PhoneNumber", salesPerson.PhoneNumber);
                command.Parameters.AddWithValue("@IsActive", salesPerson.IsActive);
                command.Parameters.AddWithValue("@CountryOfRecidence", salesPerson.CountryOfRecidence);
                if (salesPerson.PassportNumber != "N/A" && salesPerson.PassportNumber != "")
                    command.Parameters.AddWithValue("@PassportNumber", salesPerson.PassportNumber);
                if (salesPerson.PassportExpiryDate != "N/A" && salesPerson.PassportExpiryDate != "")
                    command.Parameters.AddWithValue("@PassportExpiryDate", salesPerson.PassportExpiryDate);
                command.Parameters.AddWithValue("@Designation", salesPerson.Designation);
                if (salesPerson.VisaExpiryDate != "N/A" && salesPerson.VisaExpiryDate != "")
                    command.Parameters.AddWithValue("@VisaExpiryDate", salesPerson.VisaExpiryDate);
                if (salesPerson.InsuarnceCompany != "N/A" && salesPerson.InsuarnceCompany != "")
                    command.Parameters.AddWithValue("@InsuranceCompany", salesPerson.InsuarnceCompany);
                if (salesPerson.InsuranceExpiryDate != "N/A" && salesPerson.InsuranceExpiryDate != "")
                    command.Parameters.AddWithValue("@InsuranceExpiryDate", salesPerson.InsuranceExpiryDate);
                command.Parameters.AddWithValue("@Password", _Password);
                command.Parameters.AddWithValue("@IsLogin", salesPerson.IsLogin);
                command.Parameters.AddWithValue("@CreatedBy", salesPerson.CreatedBy);
                con.Open();
                _returnId = (int)command.ExecuteScalar();
                con.Close();
                command.Parameters.Clear();

                if(_returnId > 0)
                {
                    command = new SqlCommand("USP_Admin_RemoveSalesPersonCompanyMapping", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@salesPersonId", salesPerson.Id);
                    con.Open();
                    command.ExecuteNonQuery();
                    con.Close();

                    command.Parameters.Clear();
                    if (_returnId > 0)
                    {
                        for (int index = 0; index < salesPerson.CompanyId.Count; index++)
                        {
                            command = new SqlCommand("USP_Admin_SaveAssignedSalesPerson", con);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@Status", Operation.Insert);
                            command.Parameters.AddWithValue("@UserId", salesPerson.CreatedBy);
                            command.Parameters.AddWithValue("@SalesPersonId", salesPerson.Id);
                            command.Parameters.AddWithValue("@CompanyId", salesPerson.CompanyId[index]);
                            con.Open();
                            _returnId = (int)command.ExecuteScalar();
                            con.Close();
                        }

                    }
                }
                return _returnId;
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
        /// Delete Sales Person                                                                                         -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int Delete(int Id, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_SalesPersonOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Delete);
                command.Parameters.AddWithValue("@Id", Id);
                command.Parameters.AddWithValue("@CreatedBy", UserId);
                con.Open();
                _returnId = (int)command.ExecuteScalar();
                con.Close();
                return _returnId;
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
        public GetAllSalesPersonsWithPaginationModel GetAllSalesPersonsWithPagination(int page,int pageSize,string searchValue,string sortBy,string sortDirection)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetAllSalesPersonsWithPaginationModel model = new GetAllSalesPersonsWithPaginationModel();
                List<SalesPersonViewModel> salesPersons = new List<SalesPersonViewModel>();
                int totalSalesPersons = 0;
                SqlCommand command = new SqlCommand("USP_Admin_GetAllSalesPersonListWithPagination", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PageNumber", page);
                command.Parameters.AddWithValue("@RowsOfPage", pageSize);
                command.Parameters.AddWithValue("@searchPrefix", searchValue);
                command.Parameters.AddWithValue("@sortColumn", sortBy);
                command.Parameters.AddWithValue("@sortDirection", sortDirection);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    SalesPersonViewModel salesPerson = new SalesPersonViewModel();
                    salesPerson.Id = Convert.ToInt32(reader["Id"]);
                    salesPerson.FirstName = reader["FirstName"].ToString();
                    salesPerson.LastName = reader["LastName"].ToString();
                    salesPerson.EmailId = reader["EmailId"].ToString();
                    salesPerson.CountryOfRecidence = reader["CountryOfRecidence"].ToString();
                    salesPerson.PassportNumber = reader["PassportNumber"].ToString();
                    salesPerson.PassportExpiryDate = reader["PassportExpiryDate"].ToString();
                    salesPerson.Role = reader["Role"].ToString();
                    salesPerson.VisaExpiryDate = reader["VisaExpiryDate"].ToString();
                    salesPerson.InsuarnceCompany = reader["InsuranceCompany"].ToString();
                    salesPerson.InsuranceExpiryDate = reader["InsuarnceExpiryDate"].ToString();
                    salesPerson.Company = reader["Company"].ToString();
                    salesPersons.Add(salesPerson);
                }

                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["Id"] != DBNull.Value)
                        totalSalesPersons += 1;
                }
                con.Close();
                model.totalSalesPersons = totalSalesPersons;
                model.salesPersons = salesPersons;
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
