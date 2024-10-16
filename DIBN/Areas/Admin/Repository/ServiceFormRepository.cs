using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Areas.Admin.Repository
{
    public class ServiceFormRepository : IServiceFormRepository
    {
        private readonly Connection _dataSetting;
        private readonly IUserRepository _userRepository;
        public ServiceFormRepository(Connection dataSetting, IUserRepository userRepository)
        {
            _dataSetting = dataSetting;
            _userRepository = userRepository;
        }
        /// <summary>
        /// Get Service Form Details By Service Id                                                                                          -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="ServiceId"></param>
        /// <param name="IsCompanyService"></param>
        /// <param name="IsEmployeeService"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public FormDetail GetServiceFormDetails(int ServiceId, bool IsCompanyService, bool IsEmployeeService)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                FormDetail formDetail = new FormDetail();

                SqlCommand command = new SqlCommand("USP_Admin_GetServiceFormFieldCount", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ServiceId", ServiceId);
                command.Parameters.AddWithValue("@IsCompayService", IsCompanyService);
                command.Parameters.AddWithValue("@IsEmployeeService", IsEmployeeService);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["FormServiceId"] != DBNull.Value)
                        formDetail.ServiceFormId = Convert.ToInt32(reader["FormServiceId"]);
                    if (reader["CountOfFields"] != DBNull.Value)
                        formDetail.CountOfFields = Convert.ToInt32(reader["CountOfFields"]);
                    if (reader["FormId"] != DBNull.Value)
                        formDetail.FormId = Convert.ToInt32(reader["FormId"]);
                }
                con.Close();

                return formDetail;
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
        /// Create New Form                                                                                                             -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int CreateForm(ServiceFormModel model)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_SaveServiceForm", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ServiceId", model.ServiceId);
                command.Parameters.AddWithValue("@FormName", model.FormName);
                command.Parameters.AddWithValue("@UserId", model.UserId);

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

        /// <summary>
        /// Get Service RequestForm Fields                                                                                              -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="ServiceId"></param>
        /// <param name="FormId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<GetServiceFormFieldModel> GetServiceFormFields(int ServiceId, int FormId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetServiceFormFieldModel> formFields = new List<GetServiceFormFieldModel>();

                SqlCommand command = new SqlCommand("USP_Admin_GetServiceFormField", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ServiceId", ServiceId);
                command.Parameters.AddWithValue("@FormId", FormId);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetServiceFormFieldModel formField = new GetServiceFormFieldModel();
                    formField.ServiceId = Convert.ToInt32(reader["ServiceId"]);
                    formField.FormId = Convert.ToInt32(reader["FormId"]);
                    formField.FieldId = Convert.ToInt32(reader["FieldId"]);
                    formField.FieldName = reader["FieldName"].ToString();
                    formField.FormName = reader["FormName"].ToString();
                    formField.IsRequired = Convert.ToBoolean(reader["IsRequired"]);
                    formField.IsRequiredMessage = reader["IsRequiredMessage"].ToString();
                    formField.IsDocumentUpload = Convert.ToBoolean(reader["IsDocumentUpload"]);
                    formField.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    formField.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    formField.CreatedOn = reader["CreatedOn"].ToString();
                    formField.CreatedBy = Convert.ToInt32(reader["CreatedBy"]);
                    formField.ModifyOn = reader["ModifyOn"].ToString();
                    formField.ModifyBy = Convert.ToInt32(reader["ModifyBy"]);
                    formField.StepNumber = Convert.ToInt32(reader["StepNumber"]);
                    formFields.Add(formField);
                }
                con.Close();

                return formFields;
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
        /// Create Service Request Form Fields                                                                                          -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int CreateFormField(SaveFormFieldsModel model)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_AddServiceFormField", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ServiceId", model.ServiceId);
                command.Parameters.AddWithValue("@FormId", model.FormId);
                command.Parameters.AddWithValue("@StepNumber", model.StepNumber);
                command.Parameters.AddWithValue("@FieldName", model.FieldName);
                command.Parameters.AddWithValue("@IsRequired", model.IsRequired);
                command.Parameters.AddWithValue("@IsRequiredMessage", model.RequiredMessage);
                command.Parameters.AddWithValue("@IsDocumentUpload", model.IsDocumentUpload);
                command.Parameters.AddWithValue("@UserId", model.UserId);

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

        /// <summary>
        /// Get All Service Request List                                                                                                -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<GetRequestsModel> GetAllRequests(int CompanyId, int? Status)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetRequestsModel> getRequests = new List<GetRequestsModel>();
                SqlCommand command = new SqlCommand("USP_Admin_GetAllServiceRequestByCompanyId", con);
                command.CommandType = CommandType.StoredProcedure;
                if (CompanyId != 0)
                    command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@Status", Status);
                command.CommandTimeout = 10000;
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetRequestsModel getRequest = new GetRequestsModel();
                    getRequest.SerialNumber = reader["SerialNumber"].ToString();
                    getRequest.RequestedService = reader["RequestedService"].ToString();
                    getRequest.IsCompanyService = Convert.ToBoolean(reader["IsCompanyService"]);
                    getRequest.ServiceId = Convert.ToInt32(reader["ServiceId"]);
                    getRequest.IsEmployeeService = Convert.ToBoolean(reader["IsEmployeeService"]);
                    getRequest.CompanyName = reader["CompanyName"].ToString();
                    getRequest.RequestedBy = reader["RequestedBy"].ToString();
                    getRequest.CreatedOn = reader["CreatedOn"].ToString();
                    getRequest.AssignedUser = reader["AssignedUser"].ToString();
                    getRequest.StatusId = Convert.ToInt32(reader["StatusId"].ToString());
                    getRequest.Status = reader["Status"].ToString();
                    getRequest.ApplicantName = reader["ApplicantName"].ToString();
                    getRequest.AssignedOn = reader["AssignOn"].ToString();
                    getRequests.Add(getRequest);
                }
                con.Close();

                return getRequests;
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
        /// Get All Company Service Request                                                                                             -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<GetRequestsModel> GetAllCompanyRequests(int CompanyId, int? Status)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetRequestsModel> getRequests = new List<GetRequestsModel>();
                SqlCommand command = new SqlCommand("USP_Admin_GetCompanyServicesRequestByCompanyId", con);
                command.CommandType = CommandType.StoredProcedure;
                if (CompanyId != 0)
                    command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@Status", Status);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetRequestsModel getRequest = new GetRequestsModel();
                    getRequest.SerialNumber = reader["SerialNumber"].ToString();
                    getRequest.RequestedService = reader["RequestedService"].ToString();
                    getRequest.ServiceId = Convert.ToInt32(reader["ServiceId"]);
                    getRequest.CompanyName = reader["CompanyName"].ToString();
                    getRequest.RequestedBy = reader["RequestedBy"].ToString();
                    getRequest.CreatedOn = reader["CreatedOn"].ToString();
                    getRequest.AssignedUser = reader["AssignedUser"].ToString();
                    getRequest.StatusId = Convert.ToInt32(reader["StatusId"].ToString());
                    getRequest.Status = reader["Status"].ToString();
                    getRequest.AssignedOn = reader["AssignOn"].ToString();
                    getRequest.SalesPerson = reader["SalesPerson"].ToString();
                    getRequests.Add(getRequest);
                }
                con.Close();

                return getRequests;
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
        /// Get All Employee Service Request                                                                                            -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<GetRequestsModel> GetAllEmployeeRequests(int CompanyId, int? Status)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetRequestsModel> getRequests = new List<GetRequestsModel>();
                SqlCommand command = new SqlCommand("USP_Admin_GetEmployeeServiceRequestByCompanyId", con);
                command.CommandType = CommandType.StoredProcedure;
                if (CompanyId != 0)
                    command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@Status", Status);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetRequestsModel getRequest = new GetRequestsModel();
                    getRequest.SerialNumber = reader["SerialNumber"].ToString();
                    getRequest.RequestedService = reader["RequestedService"].ToString();
                    getRequest.ServiceId = Convert.ToInt32(reader["ServiceId"]);
                    getRequest.CompanyName = reader["CompanyName"].ToString();
                    getRequest.RequestedBy = reader["RequestedBy"].ToString();
                    getRequest.CreatedOn = reader["CreatedOn"].ToString();
                    getRequest.AssignedUser = reader["AssignedUser"].ToString();
                    getRequest.ApplicantName = reader["ApplicantName"].ToString();
                    getRequest.StatusId = Convert.ToInt32(reader["StatusId"].ToString());
                    getRequest.Status = reader["Status"].ToString();
                    getRequest.AssignedOn = reader["AssignOn"].ToString();
                    getRequest.SalesPerson = reader["SalesPerson"].ToString();
                    getRequests.Add(getRequest);
                }
                con.Close();

                return getRequests;
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

        public GetAllCompanyServiceRequestModel GetAllCompanyServiceRequests(int? Status,int page, int pageSize, string sortBy, string sortDirection, string searchString,int count)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetAllCompanyServiceRequestModel model = new GetAllCompanyServiceRequestModel();
                int totalRecords = 0;
                List<GetRequestsModel> getRequests = new List<GetRequestsModel>();
                SqlCommand command = new SqlCommand("USP_Admin_GetAllCompanyServicesRequestByCompanyId", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Status);
                command.Parameters.AddWithValue("@PageNumber", page);
                command.Parameters.AddWithValue("@RowsOfPage", pageSize);
                if(searchString!= null && searchString!="")
                    command.Parameters.AddWithValue("@searchPrefix", searchString);
                command.Parameters.AddWithValue("@sortColumn", sortBy);
                command.Parameters.AddWithValue("@sortDirection", sortDirection);
                command.CommandTimeout = 36000;
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetRequestsModel getRequest = new GetRequestsModel();
                    getRequest.SerialNumber = reader["SerialNumber"].ToString();
                    getRequest.RequestedService = reader["RequestedService"].ToString();
                    getRequest.ServiceId = Convert.ToInt32(reader["ServiceId"]);
                    getRequest.CompanyName = reader["CompanyName"].ToString();
                    getRequest.RequestedBy = reader["RequestedBy"].ToString();
                    getRequest.CreatedOn = reader["CreatedOn"].ToString();
                    getRequest.AssignedUser = reader["AssignedUser"].ToString();
                    getRequest.StatusId = Convert.ToInt32(reader["StatusId"].ToString());
                    getRequest.Status = reader["Status"].ToString();
                    getRequest.AssignedOn = reader["AssignOn"].ToString();
                    getRequest.SalesPerson = reader["SalesPerson"].ToString();
                    getRequests.Add(getRequest);
                }

                con.Close();
                command.Parameters.Clear();
                
                command = new SqlCommand("USP_Admin_GetAllCompanyServicesRequestByCompanyIdCount", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Status);
                if (searchString != null && searchString != "")
                    command.Parameters.AddWithValue("@searchPrefix", searchString);
                command.CommandTimeout = 36000;

                con.Open();
                reader = command.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                totalRecords = dt.Rows.Count;
                con.Close();

                model.totalRecords = totalRecords;
                model.getRequests = getRequests;
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

        public GetAllCompanyServiceRequestModel GetAllCompanyServiceRequests_TEST(int? Status, int page, int pageSize, string sortBy, string sortDirection, string searchString, int count)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetAllCompanyServiceRequestModel model = new GetAllCompanyServiceRequestModel();
                int totalRecords = 0;
                List<GetRequestsModel> getRequests = new List<GetRequestsModel>();
                SqlCommand command = new SqlCommand("USP_Admin_GetAllCompanyServicesRequestByCompanyId_TEST", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 36000;
                command.Parameters.AddWithValue("@Status", Status);
                command.Parameters.AddWithValue("@PageNumber", page);
                command.Parameters.AddWithValue("@RowsOfPage", pageSize);
                if(searchString!= null && searchString!="")
                    command.Parameters.AddWithValue("@searchPrefix", searchString);
                command.Parameters.AddWithValue("@sortColumn", sortBy);
                command.Parameters.AddWithValue("@sortDirection", sortDirection);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetRequestsModel getRequest = new GetRequestsModel();
                    getRequest.SerialNumber = reader["SerialNumber"].ToString();
                    getRequest.RequestedService = reader["RequestedService"].ToString();
                    getRequest.ServiceId = Convert.ToInt32(reader["ServiceId"]);
                    getRequest.CompanyName = reader["CompanyName"].ToString();
                    getRequest.RequestedBy = reader["RequestedBy"].ToString();
                    getRequest.CreatedOn = reader["CreatedOn"].ToString();
                    getRequest.AssignedUser = reader["AssignedUser"].ToString();
                    getRequest.StatusId = Convert.ToInt32(reader["StatusId"].ToString());
                    getRequest.Status = reader["Status"].ToString();
                    getRequest.AssignedOn = reader["AssignOn"].ToString();
                    getRequest.SalesPerson = reader["SalesPerson"].ToString();
                    getRequests.Add(getRequest);
                }

                con.Close();
                command.Parameters.Clear();
                if (count == 0)
                {
                    command = new SqlCommand("USP_Admin_GetAllCompanyServicesRequestByCompanyIdCount_TEST", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Status", Status);
                    if (searchString != null && searchString != "")
                        command.Parameters.AddWithValue("@searchPrefix", searchString);

                    con.Open();
                    reader = command.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    totalRecords = dt.Rows.Count;
                    con.Close();

                    model.totalRecords = totalRecords;
                }
                else
                {
                    model.totalRecords = count;
                }

                model.getRequests = getRequests;
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
        public async Task<GetAllCompanyServiceRequestModel> GetAllCompanyServiceRequestFilter(int? Status, int skip, int take, string sortBy, string sortDirection, string searchBy, string searchingStr)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetAllCompanyServiceRequestModel model = new GetAllCompanyServiceRequestModel();
                int totalRecords = 0;
                List<GetRequestsModel> getRequests = new List<GetRequestsModel>();
                List<GetRequestsModel> getTotalRequests = new List<GetRequestsModel>();
                SqlCommand command = new SqlCommand("USP_Admin_GetAllCompanyServicesRequestByCompanyIdWithFilter", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 36000;
                command.Parameters.AddWithValue("@Status", Status);
                con.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetRequestsModel getRequest = new GetRequestsModel();
                    getRequest.SerialNumber = reader["SerialNumber"].ToString();
                    getRequest.SerialNo = Convert.ToInt32(reader["SerialNo"]);
                    getRequest.RequestedService = reader["RequestedService"].ToString();
                    getRequest.ServiceId = Convert.ToInt32(reader["ServiceId"]);
                    getRequest.CompanyName = reader["CompanyName"].ToString();
                    getRequest.RequestedBy = reader["RequestedBy"].ToString();
                    getRequest.CreatedOn = reader["CreatedOn"].ToString();
                    getRequest.AssignedUser = reader["AssignedUser"].ToString();
                    getRequest.StatusId = Convert.ToInt32(reader["StatusId"].ToString());
                    getRequest.Status = reader["Status"].ToString();
                    getRequest.AssignedOn = reader["AssignOn"].ToString();
                    getRequest.SalesPerson = reader["SalesPerson"].ToString();
                    if (reader["CreatedOnUTC"] != DBNull.Value)
                        getRequest.CreatedOnUtc = Convert.ToDateTime(reader["CreatedOnUTC"]);
                    if (reader["AssignOnUTC"] != DBNull.Value)
                        getRequest.AssignOnUtc = Convert.ToDateTime(reader["AssignOnUTC"]);
                    getTotalRequests.Add(getRequest);
                }
                totalRecords = getTotalRequests.Count;

                if (searchBy != null && searchBy != null)
                {
                    if (searchBy == "Serial Number")
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(searchingStr, "^[0-9]*$"))
                        {
                            var getalldata = from data in getTotalRequests
                                             where data.SerialNo == Convert.ToInt32(searchingStr)
                                             select data;
                            getTotalRequests = getalldata.ToList();
                            totalRecords = getTotalRequests.Count;
                        }
                        else
                        {
                            Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
                            Match result = re.Match(searchingStr);
                            string alphaPart = result.Groups[1].Value.ToUpper();
                            var margeStr = alphaPart + result.Groups[2].Value;
                            var getalldata = from data in getTotalRequests
                                             where data.SerialNumber == searchingStr.ToString() ||
                                             data.SerialNumber == margeStr
                                             select data;
                            getTotalRequests = getalldata.ToList();
                            totalRecords = getTotalRequests.Count;
                        }
                    }
                    else if (searchBy == "Created By")
                    {
                        var search = searchingStr.Substring(0, 1).ToUpper();
                        var searchStr = searchingStr.Substring(1, searchingStr.Length - 1);
                        var searchStr1 = search + searchStr;

                        var getalldata = from data in getTotalRequests
                                         where data.RequestedBy.Contains(searchingStr.ToString()) ||
                                               data.RequestedBy.Contains(searchingStr.ToLower()) ||
                                               data.RequestedBy.Contains(searchingStr.ToUpper()) ||
                                               data.RequestedBy.Contains(searchStr1)
                                         select data;
                        getTotalRequests = getalldata.ToList();
                        totalRecords = getTotalRequests.Count;
                    }
                    else if (searchBy == "Company")
                    {
                        var search = searchingStr.Substring(0, 1).ToUpper();
                        var searchStr = searchingStr.Substring(1, searchingStr.Length - 1);
                        var searchStr1 = search + searchStr;

                        var getalldata = from data in getTotalRequests
                                         where data.CompanyName.Contains(searchingStr.ToString()) ||
                                               data.CompanyName.Contains(searchingStr.ToLower()) ||
                                               data.CompanyName.Contains(searchingStr.ToUpper()) ||
                                               data.CompanyName.Contains(searchStr1)
                                         select data;
                        getTotalRequests = getalldata.ToList();
                        totalRecords = getTotalRequests.Count;
                    }
                    else if (searchBy == "Sales Person")
                    {
                        var search = searchingStr.Substring(0, 1).ToUpper();
                        var searchStr = searchingStr.Substring(1, searchingStr.Length - 1);
                        var searchStr1 = search + searchStr;

                        var getalldata = from data in getTotalRequests
                                         where data.SalesPerson.Contains(searchingStr.ToString()) ||
                                               data.SalesPerson.Contains(searchingStr.ToLower()) ||
                                               data.SalesPerson.Contains(searchingStr.ToUpper()) ||
                                               data.SalesPerson.Contains(searchStr1)
                                         select data;
                        getTotalRequests = getalldata.ToList();
                        totalRecords = getTotalRequests.Count;
                    }
                    else if (searchBy == "Service Request Type")
                    {
                        var search = searchingStr.Substring(0, 1).ToUpper();
                        var searchStr = searchingStr.Substring(1, searchingStr.Length - 1);
                        var searchStr1 = search + searchStr;

                        var getalldata = from data in getTotalRequests
                                         where data.RequestedService.Contains(searchingStr.ToString()) ||
                                               data.RequestedService.Contains(searchingStr.ToLower()) ||
                                               data.RequestedService.Contains(searchingStr.ToUpper()) ||
                                               data.RequestedService.Contains(searchStr1)
                                         select data;
                        getTotalRequests = getalldata.ToList();
                        totalRecords = getTotalRequests.Count;
                    }
                    else if (searchBy == "Status")
                    {
                        var search = searchingStr.Substring(0, 1).ToUpper();
                        var searchStr = searchingStr.Substring(1, searchingStr.Length - 1);
                        var searchStr1 = search + searchStr;

                        var getalldata = from data in getTotalRequests
                                         where data.Status.Contains(searchingStr.ToString()) ||
                                               data.Status.Contains(searchingStr.ToLower()) ||
                                               data.Status.Contains(searchingStr.ToUpper()) ||
                                               data.Status.Contains(searchStr1)
                                         select data;
                        getTotalRequests = getalldata.ToList();
                        totalRecords = getTotalRequests.Count;
                    }
                    else if (searchBy == "Created On")
                    {
                        DateTime dt = DateTime.ParseExact(searchingStr, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                        var getalldata = from data in getTotalRequests
                                         where data.CreatedOnUtc == dt
                                         select data;
                        getTotalRequests = getalldata.ToList();
                        totalRecords = getTotalRequests.Count();
                    }
                    else if (searchBy == "Assigned Date")
                    {
                        DateTime dt = DateTime.ParseExact(searchingStr, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                        var getalldata = from data in getTotalRequests
                                         where data.AssignOnUtc == dt
                                         select data;
                        getTotalRequests = getalldata.ToList();
                        totalRecords = getTotalRequests.Count();
                    }
                }

                if (sortDirection != null && sortDirection == "desc")
                {
                    if (sortBy == "Serial Number")
                    {
                        var getalldata = from data in getTotalRequests
                                         orderby data.SerialNo descending
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.Skip(skip).Take(take).ToList();
                        getRequests = getHistoryOfCompanyExpense;
                        totalRecords = getTotalRequests.Count;
                    }
                    if (sortBy == "Created On")
                    {
                        var getalldata = from data in getTotalRequests
                                         orderby data.CreatedOnUtc descending
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.Skip(skip).Take(take).ToList();
                        getRequests = getHistoryOfCompanyExpense;
                        totalRecords = getTotalRequests.Count;
                    }
                    if (sortBy == "Created By")
                    {
                        var getalldata = from data in getTotalRequests
                                         orderby data.RequestedBy descending
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.Skip(skip).Take(take).ToList();
                        getRequests = getHistoryOfCompanyExpense;
                        totalRecords = getTotalRequests.Count;
                    }
                    if (sortBy == "Assigned Date")
                    {
                        var getalldata = from data in getTotalRequests
                                         orderby data.AssignOnUtc descending
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.Skip(skip).Take(take).ToList();
                        getRequests = getHistoryOfCompanyExpense;
                        totalRecords = getTotalRequests.Count;
                    }
                    if (sortBy == "Company")
                    {
                        var getalldata = from data in getTotalRequests
                                         orderby data.CompanyName descending
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.Skip(skip).Take(take).ToList();
                        getRequests = getHistoryOfCompanyExpense;
                        totalRecords = getTotalRequests.Count;
                    }
                    if (sortBy == "Sales Person")
                    {
                        var getalldata = from data in getTotalRequests
                                         orderby data.SalesPerson descending
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.Skip(skip).Take(take).ToList();
                        getRequests = getHistoryOfCompanyExpense;
                        totalRecords = getTotalRequests.Count;
                    }
                    if (sortBy == "Service Request Type")
                    {
                        var getalldata = from data in getTotalRequests
                                         orderby data.RequestedService descending
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.Skip(skip).Take(take).ToList();
                        getRequests = getHistoryOfCompanyExpense;
                        totalRecords = getTotalRequests.Count;
                    }
                    if (sortBy == "Status")
                    {
                        var getalldata = from data in getTotalRequests
                                         orderby data.Status descending
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.Skip(skip).Take(take).ToList();
                        getRequests = getHistoryOfCompanyExpense;
                        totalRecords = getTotalRequests.Count;
                    }
                }
                else
                {
                    if (sortBy == "Serial Number")
                    {
                        var getalldata = from data in getTotalRequests
                                         orderby data.SerialNo
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.Skip(skip).Take(take).ToList();
                        getRequests = getHistoryOfCompanyExpense;
                        totalRecords = getTotalRequests.Count;
                    }
                    if (sortBy == "Created On")
                    {
                        var getalldata = from data in getTotalRequests
                                         orderby data.CreatedOnUtc
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.Skip(skip).Take(take).ToList();
                        getRequests = getHistoryOfCompanyExpense;
                        totalRecords = getTotalRequests.Count;
                    }
                    if (sortBy == "Created By")
                    {
                        var getalldata = from data in getTotalRequests
                                         orderby data.RequestedBy
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.Skip(skip).Take(take).ToList();
                        getRequests = getHistoryOfCompanyExpense;
                        totalRecords = getTotalRequests.Count;
                    }
                    if (sortBy == "Assigned Date")
                    {
                        var getalldata = from data in getTotalRequests
                                         orderby data.AssignOnUtc
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.Skip(skip).Take(take).ToList();
                        getRequests = getHistoryOfCompanyExpense;
                        totalRecords = getTotalRequests.Count;
                    }
                    if (sortBy == "Company")
                    {
                        var getalldata = from data in getTotalRequests
                                         orderby data.CompanyName
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.Skip(skip).Take(take).ToList();
                        getRequests = getHistoryOfCompanyExpense;
                        totalRecords = getTotalRequests.Count;
                    }
                    if (sortBy == "Sales Person")
                    {
                        var getalldata = from data in getTotalRequests
                                         orderby data.SalesPerson
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.Skip(skip).Take(take).ToList();
                        getRequests = getHistoryOfCompanyExpense;
                        totalRecords = getTotalRequests.Count;
                    }
                    if (sortBy == "Service Request Type")
                    {
                        var getalldata = from data in getTotalRequests
                                         orderby data.RequestedService
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.Skip(skip).Take(take).ToList();
                        getRequests = getHistoryOfCompanyExpense;
                        totalRecords = getTotalRequests.Count;
                    }
                    if (sortBy == "Status")
                    {
                        var getalldata = from data in getTotalRequests
                                         orderby data.Status
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.Skip(skip).Take(take).ToList();
                        getRequests = getHistoryOfCompanyExpense;
                        totalRecords = getTotalRequests.Count;
                    }
                }

                model.getRequests = getRequests;
                model.totalRecords = totalRecords;
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

        public async Task<GetAllEmployeeServiceRequestModel> GetAllEmployeeServiceRequests(int? Status, int skip, int pageSize, string sortBy, string sortDirection, string searchString,int count)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetAllEmployeeServiceRequestModel model = new GetAllEmployeeServiceRequestModel();
                int totalRecords = 0;
                List<GetRequestsModel> getRequests = new List<GetRequestsModel>();
                SqlCommand command = new SqlCommand("USP_Admin_GetAllEmployeeServicesRequestByCompanyId", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 36000;
                command.Parameters.AddWithValue("@Status", Status);
                command.Parameters.AddWithValue("@skipRows", skip);
                command.Parameters.AddWithValue("@RowsOfPage", pageSize);
                if (searchString != null && searchString != "")
                    command.Parameters.AddWithValue("@searchPrefix", searchString);
                command.Parameters.AddWithValue("@sortColumn", sortBy);
                command.Parameters.AddWithValue("@sortDirection", sortDirection);
                con.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetRequestsModel getRequest = new GetRequestsModel();
                    getRequest.SerialNumber = reader["SerialNumber"].ToString();
                    getRequest.RequestedService = reader["RequestedService"].ToString();
                    getRequest.ServiceId = Convert.ToInt32(reader["ServiceId"]);
                    getRequest.CompanyName = reader["CompanyName"].ToString();
                    getRequest.RequestedBy = reader["RequestedBy"].ToString();
                    getRequest.AssignedUser = reader["AssignedUser"].ToString();
                    getRequest.CreatedOn = reader["CreatedOn"].ToString();
                    getRequest.StatusId = Convert.ToInt32(reader["StatusId"].ToString());
                    getRequest.AssignedOn = reader["AssignOn"].ToString();
                    getRequest.SalesPerson = reader["SalesPerson"].ToString();
                    getRequest.AssignedUser = reader["AssignedUser"].ToString();
                    getRequest.Status = reader["Status"].ToString();

                    SqlCommand command1 = new SqlCommand("USP_Admin_GetApplicantNameBySerialNumber", con);
                    command1.CommandType = CommandType.StoredProcedure;
                    command1.Parameters.AddWithValue("@SerialNumber", getRequest.SerialNumber);

                    SqlDataReader reader1 = await command1.ExecuteReaderAsync();
                    if(reader1.HasRows)
                    {
                        while (reader1.Read())
                        {
                            if (reader1["ApplicantName"] != DBNull.Value)
                                getRequest.ApplicantName = reader1["ApplicantName"].ToString();
                            else
                                getRequest.ApplicantName = "N/A";
                        }
                    }
                    else
                    {
                        getRequest.ApplicantName = "N/A";
                    }
                    getRequests.Add(getRequest);
                }

                con.Close();

                command.Parameters.Clear();

                command = new SqlCommand("USP_Admin_GetAllEmployeeServicesRequestByCompanyIdCount", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 36000;
                command.Parameters.AddWithValue("@Status", Status);
                if (searchString != null && searchString != "")
                    command.Parameters.AddWithValue("@searchPrefix", searchString);

                con.Open();
                reader = await command.ExecuteReaderAsync();
                DataTable dt = new DataTable();
                dt.Load(reader);
                totalRecords = dt.Rows.Count;
                con.Close();

                model.totalRecords = totalRecords;
                model.getRequests = getRequests;
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

        public async Task<GetAllEmployeeServiceRequestModel> GetAllEmployeeServiceRequests_TEST(int? Status, int skip, int pageSize, string sortBy, string sortDirection, string searchString, int count)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetAllEmployeeServiceRequestModel model = new GetAllEmployeeServiceRequestModel();
                int totalRecords = 0;
                List<GetRequestsModel> getRequests = new List<GetRequestsModel>();
                SqlCommand command = new SqlCommand("USP_Admin_GetAllEmployeeServicesRequestByCompanyId_TEST", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 36000;
                command.Parameters.AddWithValue("@Status", Status);
                command.Parameters.AddWithValue("@skipRows", skip);
                command.Parameters.AddWithValue("@RowsOfPage", pageSize);
                if (searchString != null && searchString != "")
                    command.Parameters.AddWithValue("@searchPrefix", searchString);
                command.Parameters.AddWithValue("@sortColumn", sortBy);
                command.Parameters.AddWithValue("@sortDirection", sortDirection);
                con.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetRequestsModel getRequest = new GetRequestsModel();
                    getRequest.SerialNumber = reader["SerialNumber"].ToString();
                    getRequest.RequestedService = reader["RequestedService"].ToString();
                    getRequest.ServiceId = Convert.ToInt32(reader["ServiceId"]);
                    getRequest.CompanyName = reader["CompanyName"].ToString();
                    getRequest.RequestedBy = reader["RequestedBy"].ToString();
                    getRequest.AssignedUser = reader["AssignedUser"].ToString();
                    getRequest.CreatedOn = reader["CreatedOn"].ToString();
                    getRequest.StatusId = Convert.ToInt32(reader["StatusId"].ToString());
                    getRequest.AssignedOn = reader["AssignOn"].ToString();
                    getRequest.SalesPerson = reader["SalesPerson"].ToString();
                    getRequest.AssignedUser = reader["AssignedUser"].ToString();
                    getRequest.Status = reader["Status"].ToString();

                    SqlCommand command1 = new SqlCommand("USP_Admin_GetApplicantNameBySerialNumber", con);
                    command1.CommandType = CommandType.StoredProcedure;
                    command1.Parameters.AddWithValue("@SerialNumber", getRequest.SerialNumber);

                    SqlDataReader reader1 = await command1.ExecuteReaderAsync();
                    while (reader1.Read())
                    {
                        if (reader1["ApplicantName"] != DBNull.Value)
                            getRequest.ApplicantName = reader1["ApplicantName"].ToString();
                        else
                            getRequest.ApplicantName = "N/A";
                    }
                    getRequests.Add(getRequest);
                }

                con.Close();

                command.Parameters.Clear();

                if (count == 0)
                {
                    command = new SqlCommand("USP_Admin_GetAllEmployeeServicesRequestByCompanyIdCount_TEST", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Status", Status);
                    if (searchString != null && searchString != "")
                        command.Parameters.AddWithValue("@searchPrefix", searchString);

                    con.Open();
                    reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        if (reader["SerialNumber"] != null)
                            totalRecords++;
                    }
                    con.Close();

                    model.totalRecords = totalRecords;
                }
                else
                {
                    model.totalRecords = count;
                }

                model.getRequests = getRequests;
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

        public async Task<GetAllEmployeeServiceRequestModel> GetAllEmployeeServiceRequestFilter(int? Status, int skip, int pageSize, string sortBy, string sortDirection, string searchBy, string searchString)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetAllEmployeeServiceRequestModel model = new GetAllEmployeeServiceRequestModel();
                int totalRecords = 0;
                List<GetRequestsModel> getRequests = new List<GetRequestsModel>();
                List<GetRequestsModel> getTotalRequests = new List<GetRequestsModel>();
                SqlCommand command = new SqlCommand("USP_Admin_GetAllEmployeeServicesRequestByCompanyId_TEST", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Status);
                command.Parameters.AddWithValue("@skipRows", skip);
                command.Parameters.AddWithValue("@RowsOfPage", pageSize);
                command.Parameters.AddWithValue("@searchBy", searchBy);
                command.Parameters.AddWithValue("@searchPrefix", searchString);
                command.Parameters.AddWithValue("@sortColumn", sortBy);
                command.Parameters.AddWithValue("@sortDirection", sortDirection);
                command.CommandTimeout = 36000;
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetRequestsModel getRequest = new GetRequestsModel();
                    getRequest.SerialNumber = reader["SerialNumber"].ToString();
                    getRequest.RequestedService = reader["RequestedService"].ToString();
                    getRequest.ServiceId = Convert.ToInt32(reader["ServiceId"]);
                    getRequest.CompanyName = reader["CompanyName"].ToString();
                    getRequest.RequestedBy = reader["RequestedBy"].ToString();
                    getRequest.AssignedUser = reader["AssignedUser"].ToString();
                    getRequest.CreatedOn = reader["CreatedOn"].ToString();
                    getRequest.StatusId = Convert.ToInt32(reader["StatusId"].ToString());
                    getRequest.AssignedOn = reader["AssignOn"].ToString();
                    getRequest.SalesPerson = reader["SalesPerson"].ToString();
                    getRequest.AssignedUser = reader["AssignedUser"].ToString();
                    getRequest.Status = reader["Status"].ToString();

                    SqlCommand command1 = new SqlCommand("USP_Admin_GetApplicantNameBySerialNumber", con);
                    command1.CommandType = CommandType.StoredProcedure;
                    command1.Parameters.AddWithValue("@SerialNumber", getRequest.SerialNumber);

                    SqlDataReader reader1 = await command1.ExecuteReaderAsync();
                    while (reader1.Read())
                    {
                        if (reader1["ApplicantName"] != DBNull.Value)
                            getRequest.ApplicantName = reader1["ApplicantName"].ToString();
                        else
                            getRequest.ApplicantName = "N/A";
                    }

                    getRequests.Add(getRequest);
                }
                reader.NextResult();
                con.Close();

                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_GetAllEmployeeServicesRequestByCompanyIdCount_TEST", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Status);
                command.Parameters.AddWithValue("@searchBy", searchBy);
                command.Parameters.AddWithValue("@searchPrefix", searchString);

                con.Open();
                reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["SerialNumber"] != null)
                        totalRecords++;
                }
                con.Close();

                model.getRequests = getRequests;
                model.totalRecords = totalRecords;
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

        public async Task<GetAllCompanyServiceRequestModel> GetAllCompanyServiceRequestTempData(int? Status, int skip, int take, string sortBy, string sortDirection, string searchBy, string searchingStr)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetAllCompanyServiceRequestModel model = new GetAllCompanyServiceRequestModel();
                int totalRecords = 0;
                List<GetRequestsModel> getRequests = new List<GetRequestsModel>();
                SqlCommand command = new SqlCommand("USP_Admin_GetAllAdminCompanyServicesRequestFromTempTable", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 36000;
                if(Status != null)
                    command.Parameters.AddWithValue("@Status", Status);
                command.Parameters.AddWithValue("@skipRows", skip);
                command.Parameters.AddWithValue("@RowsOfPage", take);
                if(searchBy!=null)
                    command.Parameters.AddWithValue("@searchBy", searchBy);
                if(searchingStr!=null)
                    command.Parameters.AddWithValue("@searchPrefix", searchingStr);
                if(sortBy!=null)
                    command.Parameters.AddWithValue("@sortColumn", sortBy);
                if(sortDirection != null)
                    command.Parameters.AddWithValue("@sortDirection", sortDirection);
                con.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetRequestsModel getRequest = new GetRequestsModel();
                    getRequest.SerialNumber = reader["SerialNumber"].ToString();
                    getRequest.RequestedService = reader["RequestedService"].ToString();
                    getRequest.ServiceId = Convert.ToInt32(reader["ServiceId"]);
                    getRequest.CompanyName = reader["CompanyName"].ToString();
                    getRequest.RequestedBy = reader["RequestedBy"].ToString();
                    getRequest.CreatedOn = reader["CreatedOn"].ToString();
                    getRequest.AssignedUser = reader["AssignedUser"].ToString();
                    getRequest.StatusId = Convert.ToInt32(reader["StatusId"].ToString());
                    getRequest.Status = reader["Status"].ToString();
                    getRequest.AssignedOn = reader["AssignOn"].ToString();
                    getRequest.SalesPerson = reader["SalesPerson"].ToString();
                    getRequests.Add(getRequest);
                }

                con.Close();

                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_GetAllAdminCompanyServicesRequestFromTempTableCount", con);
                command.CommandType = CommandType.StoredProcedure;
                if(Status!=null)
                    command.Parameters.AddWithValue("@Status", Status);
                if (searchBy != null)
                    command.Parameters.AddWithValue("@searchBy", searchBy);
                if(searchingStr!= null)
                    command.Parameters.AddWithValue("@searchPrefix", searchingStr);

                con.Open();
                reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["ServiceRequestCount"] != null)
                        totalRecords = Convert.ToInt32(reader["ServiceRequestCount"]);
                }
                con.Close();

                model.getRequests = getRequests;
                model.totalRecords = totalRecords;
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
        public async Task<GetAllEmployeeServiceRequestModel> GetAllEmployeeServiceRequestTempData(int? Status, int skip, int pageSize, string sortBy, string sortDirection, string searchBy, string searchString)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetAllEmployeeServiceRequestModel model = new GetAllEmployeeServiceRequestModel();
                int totalRecords = 0;
                List<GetRequestsModel> getRequests = new List<GetRequestsModel>();
                List<GetRequestsModel> getTotalRequests = new List<GetRequestsModel>();
                SqlCommand command = new SqlCommand("USP_Admin_GetAllAdminEmployeeServicesRequestFromTempTable", con);
                command.CommandType = CommandType.StoredProcedure;
                if(Status!=null)
                    command.Parameters.AddWithValue("@Status", Status);
                command.Parameters.AddWithValue("@skipRows", skip);
                command.Parameters.AddWithValue("@RowsOfPage", pageSize);
                if(searchBy!=null)
                    command.Parameters.AddWithValue("@searchBy", searchBy);
                if(searchString!=null)
                    command.Parameters.AddWithValue("@searchPrefix", searchString);
                if(sortBy !=null)
                    command.Parameters.AddWithValue("@sortColumn", sortBy);
                if(sortDirection!=null)
                    command.Parameters.AddWithValue("@sortDirection", sortDirection);
                command.CommandTimeout = 36000;
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetRequestsModel getRequest = new GetRequestsModel();
                    getRequest.SerialNumber = reader["SerialNumber"].ToString();
                    getRequest.RequestedService = reader["RequestedService"].ToString();
                    getRequest.ServiceId = Convert.ToInt32(reader["ServiceId"]);
                    getRequest.CompanyName = reader["CompanyName"].ToString();
                    getRequest.RequestedBy = reader["RequestedBy"].ToString();
                    getRequest.AssignedUser = reader["AssignedUser"].ToString();
                    getRequest.CreatedOn = reader["CreatedOn"].ToString();
                    getRequest.StatusId = Convert.ToInt32(reader["StatusId"].ToString());
                    getRequest.AssignedOn = reader["AssignOn"].ToString();
                    getRequest.SalesPerson = reader["SalesPerson"].ToString();
                    getRequest.AssignedUser = reader["AssignedUser"].ToString();
                    getRequest.Status = reader["Status"].ToString();
                    getRequest.ApplicantName = reader["ApplicantName"].ToString();

                    getRequests.Add(getRequest);
                }
                con.Close();

                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_GetAllAdminEmployeeServicesRequestFromTempTableCount", con);
                command.CommandType = CommandType.StoredProcedure;
                if(Status!=null)
                    command.Parameters.AddWithValue("@Status", Status);
                if(searchBy!=null)
                    command.Parameters.AddWithValue("@searchBy", searchBy);
                if(searchString != null)
                    command.Parameters.AddWithValue("@searchPrefix", searchString);

                con.Open();
                reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["ServiceRequestCount"] != null)
                        totalRecords = Convert.ToInt32(reader["ServiceRequestCount"]);
                }
                con.Close();

                model.getRequests = getRequests;
                model.totalRecords = totalRecords;
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

        /// <summary>
        /// Get Service Request Details By Serial Number                                                                                -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public GetRequestCompleteDetails GetRequestDetail(string serialNumber)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetRequestCompleteDetails getRequestCompleteDetails = new GetRequestCompleteDetails();
                List<GetRequestDetailsModel> getRequestDetails = new List<GetRequestDetailsModel>();
                List<GetRequestResponsesModel> getRequestResponses = new List<GetRequestResponsesModel>();
                SqlCommand command = new SqlCommand("USP_Admin_GetRequestDetailsBySerialNumber_TEST", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@SerialNumber", serialNumber);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetRequestDetailsModel details = new GetRequestDetailsModel();
                    details.ServiceId = Convert.ToInt32(reader["ServiceId"]);
                    details.FormId = Convert.ToInt32(reader["FormId"]);
                    details.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    details.UserId = Convert.ToInt32(reader["UserId"]);
                    details.SerialNumber = reader["SerialNumber"].ToString();
                    details.RequestedService = reader["RequestedService"].ToString();
                    details.IsCompanyService = Convert.ToBoolean(reader["IsCompanyService"]);
                    details.IsEmployeeService = Convert.ToBoolean(reader["IsEmployeeService"]);
                    details.CompanyName = reader["CompanyName"].ToString();
                    details.RequestedBy = reader["RequestedBy"].ToString();
                    details.FieldName = reader["FieldName"].ToString();
                    if (reader["FieldValue"] != DBNull.Value)
                        details.FieldValue = reader["FieldValue"].ToString();
                    if (reader["FileName"] != DBNull.Value && reader["FileName"].ToString() != "N/A")
                        details.FileName = reader["FileName"].ToString();
                    details.CreatedOn = reader["CreatedOn"].ToString();
                    details.CreatedTime = reader["CreatedTime"].ToString();
                    details.Status = reader["Status"].ToString();
                    details.StatusId = Convert.ToInt32(reader["StatusId"]);
                    getRequestDetails.Add(details);
                }
                getRequestCompleteDetails.getRequestDetails = getRequestDetails;
                reader.NextResult();
                while (reader.Read())
                {
                    GetRequestResponsesModel model = new GetRequestResponsesModel();
                    model.Id = Convert.ToInt32(reader["Id"]);
                    model.ServiceId = Convert.ToInt32(reader["ServiceId"]);
                    model.FormId = Convert.ToInt32(reader["FormId"]);
                    model.Service = reader["RequestedService"].ToString();
                    model.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    model.CompanyName = reader["CompanyName"].ToString();
                    model.UserId = Convert.ToInt32(reader["UserId"]);
                    model.RequestedBy = reader["RequestedBy"].ToString();
                    model.SerialNumber = reader["SerialNumber"].ToString();
                    model.Title = reader["Title"].ToString();
                    model.Description = reader["Description"].ToString();
                    model.AssignedTo = Convert.ToInt32(reader["AssignedTo"]);
                    model.StatusId = Convert.ToInt32(reader["StatusId"]);
                    model.Status = reader["Status"].ToString();
                    model.CreatedOn = reader["CreatedOn"].ToString();
                    model.CreatedTime = reader["CreatedTime"].ToString();
                    model.serviceRequestDocuments = GetServiceRequestDocument(model.Id);
                    getRequestResponses.Add(model);
                }
                getRequestCompleteDetails.getRequestResponses = getRequestResponses;
                con.Close();

                return getRequestCompleteDetails;
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
		/// Get Service Request Details By Serial Number                                                                                -- Yashasvi TBC (16-12-2022)
		/// </summary>
		/// <param name="serialNumber"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public List<DownloadServiceRequestUploadedDocument> DownloadUploadedServiceRequestDocument(string serialNumber, int companyId, string FileName, int ServiceId)
		{
			SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
			try
			{
				List<DownloadServiceRequestUploadedDocument> getRequestCompleteDetails = new List<DownloadServiceRequestUploadedDocument>();
				SqlCommand command = new SqlCommand("USP_GetUploadedServiceRequestDocument", con);
				command.CommandType = CommandType.StoredProcedure;
				command.Parameters.AddWithValue("@SerialNumber", serialNumber);
				command.Parameters.AddWithValue("@CompanyId", companyId);
				command.Parameters.AddWithValue("@FileName", FileName);
                command.CommandTimeout = 3600;
				con.Open();
				SqlDataReader reader = command.ExecuteReader();
				while (reader.Read())
				{
					DownloadServiceRequestUploadedDocument details = new DownloadServiceRequestUploadedDocument();
					if (reader["FileName"] != DBNull.Value && reader["FileName"].ToString() != "N/A")
						details.FileName = reader["FileName"].ToString();
					if (reader["FieldFileValue"] != DBNull.Value && reader["FieldFileValue"].ToString() != "N/A")
						details.FieldFileValue = (byte[])reader["FieldFileValue"];
					getRequestCompleteDetails.Add(details);
				}
				con.Close();
				command.Parameters.Clear();
				command = new SqlCommand("USP_Admin_GetServiceRequestDocumentById", con);
				command.CommandType = CommandType.StoredProcedure;
				command.Parameters.AddWithValue("@ServiceId", ServiceId);
				con.Open();
				reader = command.ExecuteReader();
				while (reader.Read())
				{
					DownloadServiceRequestUploadedDocument details = new DownloadServiceRequestUploadedDocument();
					if (reader["FileName"] != DBNull.Value && reader["FileName"].ToString() != "N/A")
						details.FileName = reader["FileName"].ToString();
					if (reader["DataBinary"] != DBNull.Value && reader["DataBinary"].ToString() != "N/A")
						details.FieldFileValue = (byte[])reader["DataBinary"];
					getRequestCompleteDetails.Add(details);
				}
				con.Close();
				return getRequestCompleteDetails;
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
		/// Get Uploaded Document of Service Request by Document Id                                                                     -- Yashasvi TBC (28-11-2022)
		/// </summary>
		/// <param name="Id"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public List<ServiceRequestDocument> GetServiceRequestDocument(int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<ServiceRequestDocument> serviceRequestDocument = new List<ServiceRequestDocument>();
                SqlCommand command = new SqlCommand("USP_Admin_GetServiceRequestDocumentById", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ServiceId", Id);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ServiceRequestDocument document = new ServiceRequestDocument();
                    document.Id = Convert.ToInt32(reader["Id"]);
                    document.ServiceResponseId = Convert.ToInt32(reader["ServiceResponseId"]);
                    document.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    document.ServiceId = Convert.ToInt32(reader["ServiceId"]);
                    document.FileName = reader["FileName"].ToString();
                    document.DataBinary = (byte[])reader["DataBinary"];
                    serviceRequestDocument.Add(document);
                }
                con.Close();
                return serviceRequestDocument;
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
        /// Get Count of Response                                                                                                       -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="SerialNumber"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetCountOfResponse(string SerialNumber)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _responseCount = 0;

                SqlCommand command = new SqlCommand("USP_Admin_GetServiceRequestResponseCount", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@SerialNumber", SerialNumber);

                con.Open();
                _responseCount = (int)command.ExecuteScalar();
                con.Close();

                return _responseCount;
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
        /// Save Status of Service Request                                                                                              -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="getRequestResponses"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int SaveStatusOfService(GetRequestResponsesModel getRequestResponses)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0, _returnDocumentId = 0;
                string Query = "", Title = "";

                SqlCommand command = null;
                command = new SqlCommand("USP_Admin_GetServiceRequestFormName", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@formId", getRequestResponses.FormId);
                command.CommandTimeout = 600;

                con.Open();
                Title = (string)command.ExecuteScalar();
                con.Close();

                command.Parameters.Clear();

                command = new SqlCommand("USP_Admin_SaveServiceRequestStatus", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@SerialNumber", getRequestResponses.SerialNumber);
                command.Parameters.AddWithValue("@ServiceId", getRequestResponses.ServiceId);
                command.Parameters.AddWithValue("@FormId", getRequestResponses.FormId);
                command.Parameters.AddWithValue("@CompanyId", getRequestResponses.CompanyId);
                command.Parameters.AddWithValue("@UserId", getRequestResponses.UserId);
                command.Parameters.AddWithValue("@Title", Title);
                command.Parameters.AddWithValue("@Description", getRequestResponses.Description);
                command.Parameters.AddWithValue("@AssignedTo", getRequestResponses.AssignedTo);
                command.Parameters.AddWithValue("@Status", getRequestResponses.StatusId);
                command.Parameters.AddWithValue("@StepId", getRequestResponses.StepId);
                command.Parameters.AddWithValue("@CreatedBy", getRequestResponses.CreatedBy);

                con.Open();
                _returnId = (int)command.ExecuteScalar();
                con.Close();
                if (_returnId > 0)
                {
                    if (getRequestResponses.formFile != null)
                    {
                        _returnDocumentId = SaveServiceRequestDocument(getRequestResponses.formFile, getRequestResponses.SerialNumber, getRequestResponses.ServiceId, getRequestResponses.CompanyId, _returnId, getRequestResponses.CreatedBy);
                    }
                }
                if (_returnId > 0)
                {
                    command.Parameters.Clear();
                    string createdDate = "";
                    command = new SqlCommand("USP_Admin_GetArabianCurrentDateTime", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 600;

                    con.Open();
                    createdDate = (string)command.ExecuteScalar();
                    con.Close();

                    command.Parameters.Clear();

                    command = new SqlCommand("USP_Admin_SaveRequestNotifications", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 600;

                    command.Parameters.AddWithValue("@Type", "Service Request");
                    command.Parameters.AddWithValue("@RequestCreatedOn", createdDate);
                    command.Parameters.AddWithValue("@SerialNumber", getRequestResponses.SerialNumber);
                    command.Parameters.AddWithValue("@Title", Title);
                    command.Parameters.AddWithValue("@Description", getRequestResponses.Description);
                    command.Parameters.AddWithValue("@ServiceCreatedBy", getRequestResponses.UserId);
                    command.Parameters.AddWithValue("@ResponseType", "Response");

                    con.Open();
                    int _notification = (int)command.ExecuteScalar();
                    con.Close();

                    command.Parameters.Clear();
                }

                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_UpdateServiceRequestStatus", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", getRequestResponses.StatusId);
                command.Parameters.AddWithValue("@SerialNumber", getRequestResponses.SerialNumber);

                con.Open();
                command.ExecuteNonQuery();
                con.Close();

                return _returnDocumentId != 0 ? _returnDocumentId : _returnId;
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
        /// Save Service Request Document                                                                                               -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="SerialNumber"></param>
        /// <param name="ServiceId"></param>
        /// <param name="CompanyId"></param>
        /// <param name="ServiceResponseId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int SaveServiceRequestDocument(IFormFile formFile, string SerialNumber, int ServiceId, int CompanyId, int ServiceResponseId, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                Byte[] bytes = null;
                string _getName = null;
                if (formFile != null)
                {
                    if (formFile.Length > 0)
                    {
                        _getName = formFile.FileName;
                        var Name = formFile.FileName.Split(".");
                        string FileName = Name[0];
                        var extn = Path.GetExtension(_getName);

                        using (MemoryStream ms = new MemoryStream())
                        {
                            formFile.OpenReadStream().CopyTo(ms);
                            bytes = ms.ToArray();
                        }
                    }
                }


                SqlCommand command = new SqlCommand("USP_Admin_SaveServiceRequestDocument", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ServiceId", ServiceId);
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@ServiceResponseId", ServiceResponseId);
                command.Parameters.AddWithValue("@FileName", _getName);
                command.Parameters.AddWithValue("@DataBinary", bytes);
                command.Parameters.AddWithValue("@UserId", UserId);

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

        /// <summary>
        /// Change Status of Servicece Request                                                                                          -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="SerialNumber"></param>
        /// <param name="Status"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int ChangeStatusOfService(string SerialNumber, int Status, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string _query = "";
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_ChangeServiceStatus", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@SerialNumber", SerialNumber);
                command.Parameters.AddWithValue("@Status", Status);
                command.Parameters.AddWithValue("@UserId", UserId);

                con.Open();
                _returnId = (int)command.ExecuteScalar();
                con.Close();

                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_UpdateServiceRequestStatus", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Status);
                command.Parameters.AddWithValue("@SerialNumber", SerialNumber);

                con.Open();
                command.ExecuteNonQuery();
                con.Close();

                //GetRequestResponsesModel model = new GetRequestResponsesModel();
                //command.Parameters.Clear();
                //_query = "SELECT TOP(1) Id AS Id, SerialNumber AS SerialNumber, " +
                //    "ServiceId AS ServiceId," +
                //    " FormId AS FormId , " +
                //    "CompanyId As CompanyId ," +
                //    "Title As Title ," +
                //    "Status As Status ," +
                //    "StepId AS StepId" +
                //    " FROM [dbo].[Tbl_ServiceRequest] WHERE SerialNumber LIKE '" + SerialNumber + "' AND IsDelete <> 1 AND IsActive <> 0 " +
                //    " GROUP BY Id,SerialNumber,ServiceId,FormId,CompanyId,Title,Status,StepId " +
                //    " ORDER BY Id DESC";

                //command = new SqlCommand(_query, con);
                //command.CommandType = CommandType.Text;
                //con.Open();
                //SqlDataReader reader = command.ExecuteReader();
                //while (reader.Read())
                //{

                //    model.SerialNumber = reader["SerialNumber"].ToString();
                //    model.ServiceId = Convert.ToInt32(reader["ServiceId"]);
                //    model.FormId = Convert.ToInt32(reader["FormId"]);
                //    model.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                //    model.Title = reader["Title"].ToString();
                //    model.Description = "Service Request Approved by Finance Department.";
                //    model.UserId = UserId;
                //    model.StatusId = Convert.ToInt32(reader["Status"]);
                //    model.StepId = Convert.ToInt32(reader["StepId"]) + 1;
                //    model.CreatedBy = UserId;
                //}
                //con.Close();
                //SaveStatusOfService(model);
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
        /// Last Step Number of Any Service Request By Serial Number                                                                    -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="SerialNumber"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int LastStepNumber(string SerialNumber)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_GetLastStepNumber", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@SerialNumber", SerialNumber);

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

        /// <summary>
        /// Get Latest Status of Service Request By Serial Number                                                                       -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="SerialNumber"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetLatestStatusOfRequest(string SerialNumber)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_GetLatestStatusOfRequest", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@SerialNumber", SerialNumber);

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

        /// <summary>
        /// Save Assigned User of Service Request                                                                                       -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="saveAssignUser"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int SaveAssignUserOfService(SaveAssignUser saveAssignUser)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                string _query = "", _assignedUsers = "";
                SqlCommand command = null;

                command = new SqlCommand("USP_Admin_CheckWhetherServiceRequestAssignedOrNot", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@serialNumber", saveAssignUser.SerialNumber);

                con.Open();
                command.ExecuteNonQuery();
                con.Close();

                command.Parameters.Clear();

                if (saveAssignUser.UserId != null)
                {
                    if (saveAssignUser.UserId.Length > 0)
                    {
                        command = new SqlCommand("USP_Admin_SaveAssignServiceRequestUser", con);
                        command.CommandType = CommandType.StoredProcedure;
                        for (int index = 0; index < saveAssignUser.UserId.Length; index++)
                        {
                            command.Parameters.AddWithValue("@UserId", saveAssignUser.UserId[index]);
                            command.Parameters.AddWithValue("@ServiceId", saveAssignUser.ServiceId);
                            command.Parameters.AddWithValue("@SerialNumber", saveAssignUser.SerialNumber);
                            command.Parameters.AddWithValue("@CreatedBy", saveAssignUser.CreatedBy);

                            con.Open();
                            _returnId = (int)command.ExecuteScalar();
                            con.Close();
                            command.Parameters.Clear();
                        }
                        for (int index = 0; index < saveAssignUser.UserId.Length; index++)
                        {
                            command.Parameters.Clear();
                            command = new SqlCommand("USP_Admin_GetUsernameForServiceRequest", con);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@userId", saveAssignUser.UserId[index]);
                            con.Open();
                            if (_assignedUsers == "")
                            {
                                _assignedUsers = (string)command.ExecuteScalar();
                            }
                            else
                            {
                                _assignedUsers = _assignedUsers + " , " + (string)command.ExecuteScalar();
                            }
                            con.Close();
                        }


                        command.Parameters.Clear();
                        string createdDate = ""; string Title = "";
                        command = new SqlCommand("USP_Admin_GetArabianCurrentDateTime", con);
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 600;

                        con.Open();
                        createdDate = (string)command.ExecuteScalar();
                        con.Close();

                        command.Parameters.Clear();

                        command = new SqlCommand("USP_Admin_GetServiceRequestFormName", con);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@serialNumber", saveAssignUser.SerialNumber);
                        command.CommandTimeout = 600;

                        con.Open();
                        Title = (string)command.ExecuteScalar();
                        con.Close();

                        command.Parameters.Clear();


                        command = new SqlCommand("USP_Admin_SaveRequestNotifications", con);
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 600;
                        for (int index = 0; index < saveAssignUser.UserId.Length; index++)
                        {
                            command.Parameters.AddWithValue("@Type", "Service Request");
                            command.Parameters.AddWithValue("@RequestCreatedOn", createdDate);
                            command.Parameters.AddWithValue("@AssignedOn", createdDate);
                            command.Parameters.AddWithValue("@SerialNumber", saveAssignUser.SerialNumber);
                            command.Parameters.AddWithValue("@AssignedUserId", saveAssignUser.UserId[index]);
                            command.Parameters.AddWithValue("@AssignedBy", saveAssignUser.CreatedBy);
                            command.Parameters.AddWithValue("@Title", Title);
                            command.Parameters.AddWithValue("@Description", "Your request is now assigned to " + _assignedUsers + ".");
                            command.Parameters.AddWithValue("@ResponseType", "Response");

                            con.Open();
                            int _notification = (int)command.ExecuteScalar();
                            con.Close();

                            command.Parameters.Clear();
                        }

                    }
                    GetRequestResponsesModel model = new GetRequestResponsesModel();
                    command.Parameters.Clear();
                    command = new SqlCommand("USP_Admin_GetServiceRequestResponseDetailsForNotification", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@serialNumber", saveAssignUser.SerialNumber);
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        model.SerialNumber = reader["SerialNumber"].ToString();
                        model.ServiceId = Convert.ToInt32(reader["ServiceId"]);
                        model.FormId = Convert.ToInt32(reader["FormId"]);
                        model.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                        model.Title = reader["Title"].ToString();
                        model.Description = "Your request is now assigned to " + _assignedUsers + ".";
                        model.UserId = saveAssignUser.CreatedBy;
                        model.StatusId = Convert.ToInt32(reader["Status"]);
                        model.StepId = Convert.ToInt32(reader["StepId"]) + 1;
                        model.CreatedBy = saveAssignUser.CreatedBy;
                    }
                    con.Close();
                    Log.Information("Service Request " + model.SerialNumber + " is assigned to " + _assignedUsers + ".");
                    command.Parameters.Clear();
                    command = new SqlCommand("USP_Admin_UpdateServiceRequestAssignedUser", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SerialNumber", model.SerialNumber);
                    con.Open();
                    command.ExecuteNonQuery();
                    con.Close();

                    SaveStatusOfService(model);
                }
                else
                {
                    GetRequestResponsesModel model = new GetRequestResponsesModel();
                    command.Parameters.Clear();
                    command = new SqlCommand("USP_Admin_GetServiceRequestBasicDetailsForNotification", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@serialNumber", saveAssignUser.SerialNumber);
                    con.Open();
                    SqlDataReader reader1 = command.ExecuteReader();
                    while (reader1.Read())
                    {
                        model.SerialNumber = reader1["SerialNumber"].ToString();
                    }
                    con.Close();

                    command.Parameters.Clear();
                    command = new SqlCommand("USP_Admin_UpdateServiceRequestAssignedUser", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SerialNumber", model.SerialNumber);
                    con.Open();
                    command.ExecuteNonQuery();
                    con.Close();

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
        /// Remove Service Request                                                                                                      -- Yashasvi TBC (28-11-2022)                                                                    
        /// </summary>
        /// <param name="SerialNumber"></param>
        /// <param name="ServiceId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int RemoveServiceRequest(string SerialNumber, int ServiceId, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_DeleteServiceRequest", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@SerialNumber", SerialNumber);
                command.Parameters.AddWithValue("@ServiceId", ServiceId);
                command.Parameters.AddWithValue("@UserId", UserId);

                con.Open();
                _returnId = (int)command.ExecuteScalar();
                con.Close();
                Log.Information("Service Request " + SerialNumber + " is Deleted By User Id = " + UserId);
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
        /// Get Form Name By Form Id                                                                                                    -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="FormId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string GetFormNameById(int FormId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string _returnId = null;
                SqlCommand command = new SqlCommand("USP_Admin_GetServiceRequestFormName", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@formId", FormId);

                con.Open();
                _returnId = (string)command.ExecuteScalar();
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

        /// <summary>
        /// Get All Assigned Users                                                                                                      -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="SerialNumber"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<int> GetAllAssignedUsers(string SerialNumber)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<int> _returnList = new List<int>();
                
                SqlCommand command = new SqlCommand("USP_Admin_GetAssignedUsersIdOfServiceRequest", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@serialNumber", SerialNumber);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    _returnList.Add(Convert.ToInt32(reader["UserId"]));
                }
                con.Close();

                return _returnList;
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
        /// Get Last Step Number of Form                                                                                                -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="ServiceId"></param>
        /// <param name="FormId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetlastStepNumber(int ServiceId, int FormId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_GetLastStepnumberByServiceId", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ServiceId", ServiceId);
                command.Parameters.AddWithValue("@FormId", FormId);

                con.Open();
                try
                {
                    _returnId = (int)command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    _returnId = 0;
                }

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
        public async Task<GetCompanyServiceRequestsByCompanyId> GetAllCompanyRequestByCompanyId(int CompanyId,int? status, int page, int pageSize, string sortBy,
            string sortDirection,string searchBy, string searchString)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetCompanyServiceRequestsByCompanyId model = new GetCompanyServiceRequestsByCompanyId();
                List<GetRequests> getRequests = new List<GetRequests>();
                SqlCommand command = null;
                int totalRecords = 0;
                command = new SqlCommand("USP_Admin_GetAllCompanyServiceRequestByCompanyIdWithPagination", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 36000;
                if (status != null)
                    command.Parameters.AddWithValue("@Status", status.Value);

                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@skipRows", page);
                command.Parameters.AddWithValue("@RowsOfPage", pageSize);
                command.Parameters.AddWithValue("@searchPrefix", searchString);
                command.Parameters.AddWithValue("@sortColumn", sortBy);
                command.Parameters.AddWithValue("@sortDirection", sortDirection);

                con.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetRequests getRequest = new GetRequests();
                    getRequest.SerialNumber = reader["SerialNumber"].ToString();
                    getRequest.RequestedService = reader["RequestedService"].ToString();
                    getRequest.CompanyName = reader["CompanyName"].ToString();
                    getRequest.RequestedBy = reader["RequestedBy"].ToString();
                    getRequest.CreatedOn = reader["CreatedOn"].ToString();
                    getRequest.Status = reader["Status"].ToString();
                    getRequest.AssignedUser = reader["AssignedUser"].ToString();
                    getRequests.Add(getRequest);
                }
                con.Close();

                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_GetAllCompanyServiceRequestByCompanyIdWithCount", con);
                command.CommandType = CommandType.StoredProcedure;
                if (status != null)
                    command.Parameters.AddWithValue("@Status", status.Value);
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@searchPrefix", searchString);

                con.Open();
                reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["SerialNumber"] != null)
                        totalRecords++;
                }
                con.Close();

                model.totalRecords = totalRecords;
                model.getRequests = getRequests;
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<GetCompanyServiceRequestsByCompanyId> GetAllEmployeeRequestByCompanyId(int CompanyId, int? status, int page, int pageSize, string sortBy,
            string sortDirection, string searchBy, string searchString)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetCompanyServiceRequestsByCompanyId model = new GetCompanyServiceRequestsByCompanyId();
                List<GetRequests> getRequests = new List<GetRequests>();
                SqlCommand command = null;
                int totalRecords = 0;
                command = new SqlCommand("USP_Admin_GetAllEmployeeServiceRequestByCompanyIdWithPagination", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 36000;
                if (status != null)
                    command.Parameters.AddWithValue("@Status", status.Value);

                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@skipRows", page);
                command.Parameters.AddWithValue("@RowsOfPage", pageSize);
                command.Parameters.AddWithValue("@searchPrefix", searchString);
                command.Parameters.AddWithValue("@sortColumn", sortBy);
                command.Parameters.AddWithValue("@sortDirection", sortDirection);

                con.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetRequests getRequest = new GetRequests();
                    getRequest.SerialNumber = reader["SerialNumber"].ToString();
                    getRequest.RequestedService = reader["RequestedService"].ToString();
                    getRequest.CompanyName = reader["CompanyName"].ToString();
                    getRequest.RequestedBy = reader["RequestedBy"].ToString();
                    getRequest.CreatedOn = reader["CreatedOn"].ToString();
                    getRequest.Status = reader["Status"].ToString();
                    getRequest.AssignedUser = reader["AssignedUser"].ToString();

                    SqlCommand command1 = new SqlCommand("USP_Admin_GetApplicantNameBySerialNumber", con);
                    command1.CommandType = CommandType.StoredProcedure;
                    command1.Parameters.AddWithValue("@SerialNumber", getRequest.SerialNumber);

                    SqlDataReader reader1 = await command1.ExecuteReaderAsync();
                    if (reader1.HasRows)
                    {
                        while (reader1.Read())
                        {
                            if (reader1["ApplicantName"] != DBNull.Value)
                                getRequest.ApplicantName = reader1["ApplicantName"].ToString();
                            else
                                getRequest.ApplicantName = "N/A";
                        }

                    }
                    else
                    {
                        getRequest.ApplicantName = "N/A";
                    }
                    getRequests.Add(getRequest);
                }
                con.Close();

                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_GetAllEmployeeServiceRequestByCompanyIdWithCount", con);
                command.CommandType = CommandType.StoredProcedure;
                if (status != null)
                    command.Parameters.AddWithValue("@Status", status.Value);
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@searchPrefix", searchString);

                con.Open();
                reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["SerialNumber"] != null)
                        totalRecords++;
                }
                con.Close();

                model.totalRecords = totalRecords;
                model.getRequests = getRequests;
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
