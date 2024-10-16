using DIBN.IService;
using DIBN.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Service
{
    public class ServicesFormService : IServicesFormService
    {
        private readonly Connection _dataSetting;
        public ServicesFormService(Connection dataSetting)
        {
            _dataSetting = dataSetting;
        }
        public FormDetail GetServiceFormDetails(int ServiceId, bool IsCompanyService, bool IsEmployeeService)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                FormDetail formDetail = new FormDetail();

                SqlCommand command = new SqlCommand("USP_Admin_GetServiceFormFieldCount", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
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
                    if (reader["FormName"] != DBNull.Value)
                        formDetail.FormName = Convert.ToString(reader["FormName"]);
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
        public List<GetServiceFormFieldModel> GetServiceFormFields(int ServiceId, int FormId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetServiceFormFieldModel> formFields = new List<GetServiceFormFieldModel>();

                SqlCommand command = new SqlCommand("USP_Admin_GetServiceFormField", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
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
                    formField.DisplayName = reader["DisplayName"].ToString();
                    formField.FormConstrains = reader["FormConstrains"].ToString();
                    formField.CompanyType = reader["CompanyType"].ToString();
                    formField.AllowDisplay = Convert.ToBoolean(reader["AllowDisplay"]);
                    formField.HasMultipleForms = Convert.ToBoolean(reader["HasMultipleForms"]);
                    formField.ModifyBy = Convert.ToInt32(reader["ModifyBy"]);
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

        public string SaveServiceFormData(GetFieldFormData formData)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int IsSalesPerson = 0,IsRMTeam =0;
                string _returnId = null;
                Byte[] bytes = null;
                string _getName = null;
                SqlCommand command = new SqlCommand("USP_Admin_SaveServiceFormFieldValues", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                if (formData.formFile != null)
                {
                    if (formData.formFile.Count > 0)
                    {
                        for (int i = 0; i < formData.formFile.Count; i++)
                        {
                            _getName = formData.formFile[i].FileName;
                            var Name = formData.formFile[i].FileName.Split(".");
                            string FileName = Name[0];
                            var extn = Path.GetExtension(_getName);

                            using (MemoryStream ms = new MemoryStream())
                            {
                                formData.formFile[i].OpenReadStream().CopyTo(ms);
                                bytes = ms.ToArray();
                            }
                            command.Parameters.AddWithValue("@ServiceId", formData.ServiceId);
                            command.Parameters.AddWithValue("@FormId", formData.FormId);
                            command.Parameters.AddWithValue("@FieldId", formData.FieldId);
                            command.Parameters.AddWithValue("@FieldValue", formData.FieldValue);
                            command.Parameters.AddWithValue("@SerialNumber", formData.SerialNumber);
                            if (formData.formFile != null)
                            {
                                command.Parameters.AddWithValue("@FileName", _getName);
                                command.Parameters.AddWithValue("@FieldFileValue", bytes);
                            }
                            command.Parameters.AddWithValue("@CompanyId", formData.CompanyId);
                            if (formData.SalesPersonId == 0 && formData.RMTeamId == 0)
                                command.Parameters.AddWithValue("@UserId", formData.UserId);
                            else if(formData.RMTeamId != 0)
                                command.Parameters.AddWithValue("@RMTeamId", formData.RMTeamId);
                            else
                                command.Parameters.AddWithValue("@SalesPerson", formData.SalesPersonId);
                            command.Parameters.AddWithValue("@Last", formData.Last);

                            con.Open();
                            _returnId = (string)command.ExecuteScalar();
                            con.Close();
                            command.Parameters.Clear();
                        }
                    }
                }
                else
                {
                    command.Parameters.AddWithValue("@ServiceId", formData.ServiceId);
                    command.Parameters.AddWithValue("@FormId", formData.FormId);
                    command.Parameters.AddWithValue("@FieldId", formData.FieldId);
                    command.Parameters.AddWithValue("@FieldValue", formData.FieldValue);
                    command.Parameters.AddWithValue("@SerialNumber", formData.SerialNumber);
                    if (formData.formFile != null)
                    {
                        command.Parameters.AddWithValue("@FileName", _getName);
                        command.Parameters.AddWithValue("@FieldFileValue", bytes);
                    }
                    command.Parameters.AddWithValue("@CompanyId", formData.CompanyId);
                    if (formData.SalesPersonId == 0 && formData.RMTeamId == 0)
                        command.Parameters.AddWithValue("@UserId", formData.UserId);
                    else if (formData.RMTeamId != 0)
                        command.Parameters.AddWithValue("@RMTeamId", formData.RMTeamId);
                    else
                        command.Parameters.AddWithValue("@SalesPerson", formData.SalesPersonId);
                    command.Parameters.AddWithValue("@Last", formData.Last);

                    con.Open();
                    _returnId = (string)command.ExecuteScalar();
                    con.Close();
                    command.Parameters.Clear();
                }

                if (_returnId != null && formData.Last != 0)
                {
                    string createdDate = "", Title = "";
                    command = new SqlCommand("USP_Admin_GetArabianCurrentDateTime", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 600;

                    con.Open();
                    createdDate = (string)command.ExecuteScalar();
                    con.Close();

                    command.Parameters.Clear();
                    command = new SqlCommand("USP_Admin_GetServiceRequestFormName", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 600;
                    command.Parameters.AddWithValue("@formId",formData.FormId);

                    con.Open();
                    Title = (string)command.ExecuteScalar();
                    con.Close();

                    command.Parameters.Clear();
                    command = new SqlCommand("USP_Admin_SaveRequestNotifications", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 600;
                    command.Parameters.AddWithValue("@Type", "Service Request");
                    command.Parameters.AddWithValue("@RequestCreatedOn", createdDate);
                    command.Parameters.AddWithValue("@SerialNumber", formData.SerialNumber);
                    command.Parameters.AddWithValue("@ServiceCreatedBy", formData.UserId);
                    command.Parameters.AddWithValue("@ServiceCreatedBySales", formData.SalesPersonId);
                    command.Parameters.AddWithValue("@CompanyId", formData.CompanyId);
                    command.Parameters.AddWithValue("@Title", Title);
                    command.Parameters.AddWithValue("@ResponseType", "Created");

                    con.Open();
                    int _notification = (int)command.ExecuteScalar();
                    con.Close();

                    command.Parameters.Clear();
                    if(formData.IsCompanyService)
                    {
                        if (formData.SalesPersonId != 0)
                        {
                            IsSalesPerson = 1;
                        }
                        if(formData.RMTeamId != 0)
                        {
                            IsRMTeam = 1;
                        }
                        command = new SqlCommand("USP_Admin_SaveCompanyServiceRequestDetails", con);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@SerialNumber", formData.SerialNumber);
                        command.Parameters.AddWithValue("@ServiceId", formData.ServiceId);
                        command.Parameters.AddWithValue("@RequestedService", formData.serviceName);
                        command.Parameters.AddWithValue("@CompanyId", formData.CompanyId);
                        if (formData.SalesPersonId == 0 && formData.RMTeamId == 0)
                            command.Parameters.AddWithValue("@UserId", formData.UserId);
                        else if (formData.RMTeamId != 0)
                            command.Parameters.AddWithValue("@UserId", formData.RMTeamId);
                        else
                            command.Parameters.AddWithValue("@UserId", formData.SalesPersonId);
                        command.Parameters.AddWithValue("@StatusId", 2);
                        command.Parameters.AddWithValue("@IsSalesPerson", IsSalesPerson);
                        command.Parameters.AddWithValue("@IsRMTeam", IsRMTeam);

                        con.Open();
                        command.ExecuteNonQuery();
                        con.Close();
                    }
                    else
                    {
                        if (formData.SalesPersonId != 0)
                        {
                            IsSalesPerson = 1;
                        }
                        if (formData.RMTeamId != 0)
                        {
                            IsRMTeam = 1;
                        }
                        command.Parameters.Clear();
                        command = new SqlCommand("USP_Admin_SaveEmployeeServiceRequestDetails", con);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@SerialNumber", formData.SerialNumber);
                        command.Parameters.AddWithValue("@ServiceId", formData.ServiceId);
                        command.Parameters.AddWithValue("@RequestedService", formData.serviceName);
                        command.Parameters.AddWithValue("@CompanyId", formData.CompanyId);
                        if (formData.SalesPersonId == 0 && formData.RMTeamId == 0)
                            command.Parameters.AddWithValue("@UserId", formData.UserId);
                        else if (formData.RMTeamId != 0)
                            command.Parameters.AddWithValue("@UserId", formData.RMTeamId);
                        else
                            command.Parameters.AddWithValue("@UserId", formData.SalesPersonId);
                        command.Parameters.AddWithValue("@StatusId", 2);
                        command.Parameters.AddWithValue("@IsSalesPerson", IsSalesPerson);
                        command.Parameters.AddWithValue("@IsRMTeam", IsRMTeam);

                        con.Open();
                        command.ExecuteNonQuery();
                        con.Close();

                        command.Parameters.Clear();
                        string applicantName = "";
                        command = new SqlCommand("USP_Admin_GetApplicantNameBySerialNumber", con);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@SerialNumber", formData.SerialNumber);

                        con.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                if (reader["ApplicantName"] != DBNull.Value)
                                    applicantName = reader["ApplicantName"].ToString();
                            }
                        }
                        con.Close();

                        command.Parameters.Clear();
                        command = new SqlCommand("USP_Admin_UpdateApplicantNameForEmployeeServiceRequest", con);
                        command.CommandType= CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@applicantName", applicantName);
                        command.Parameters.AddWithValue("@serialNumber", formData.SerialNumber);
                        con.Open();
                        command.ExecuteNonQuery();
                        con.Close();
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

        public string GetSerialNumber()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string _serialNumber = null;
                SqlCommand command = new SqlCommand("USP_Admin_GetSerialNumberOfService", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                con.Open();
                _serialNumber = (string)command.ExecuteScalar();
                con.Close();
                return _serialNumber;
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
        public List<GetRequests> GetAllRequests(int CompanyId, int UserId, string Role, string Company, int? Status)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetRequests> getRequests = new List<GetRequests>();
                List<GetRequests> Test = new List<GetRequests>();
                SqlCommand command = null;
                if (CompanyId == 1)
                {
                   
                }
                else
                {
                    command = new SqlCommand("USP_Admin_GetAllServiceRequestByCompanyId_Cmp", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 36000;
                    if (Role.StartsWith("Sales"))
                        command.Parameters.AddWithValue("@SalesPersonId", UserId);
                    if (CompanyId != 0)
                        command.Parameters.AddWithValue("@CompanyId", CompanyId);
                    if (Company != null && Company != "")
                        command.Parameters.AddWithValue("@Company", Company);
                    command.Parameters.AddWithValue("@Status", Status);

                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        GetRequests getRequest = new GetRequests();
                        getRequest.SerialNumber = reader["SerialNumber"].ToString();
                        getRequest.RequestedService = reader["RequestedService"].ToString();
                        getRequest.IsCompanyService = Convert.ToBoolean(reader["IsCompanyService"]);
                        getRequest.IsEmployeeService = Convert.ToBoolean(reader["IsEmployeeService"]);
                        getRequest.CompanyName = reader["CompanyName"].ToString();
                        getRequest.RequestedBy = reader["RequestedBy"].ToString();
                        getRequest.CreatedOn = reader["CreatedOn"].ToString();
                        getRequest.Status = reader["Status"].ToString();
                        getRequests.Add(getRequest);
                    }
                    con.Close();
                }
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

        public async Task<GetCompanyServiceRequestsByCompanyId> GetAllCompanyRequestByCompanyId(int CompanyId,int currentCompanyId,int userId, string Role, int? status, int page, int pageSize, string sortBy,
            string sortDirection, string searchString,int count)
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
                if (Role.StartsWith("Sales"))
                {
                    command.Parameters.AddWithValue("@SalesPersonId", userId);
                    if(CompanyId!=0)
                        command.Parameters.AddWithValue("@CompanyId", CompanyId);
                }
                else
                    command.Parameters.AddWithValue("@CompanyId", currentCompanyId);
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

                if (count == 0)
                {
                    command.Parameters.Clear();
                    command = new SqlCommand("USP_Admin_GetAllCompanyServiceRequestByCompanyIdWithPaginationCount", con);
                    command.CommandType = CommandType.StoredProcedure;
                    if (status != null)
                        command.Parameters.AddWithValue("@Status", status.Value);
                    if (Role.StartsWith("Sales"))
                    {
                        command.Parameters.AddWithValue("@SalesPersonId", userId);
                        if (CompanyId != 0)
                            command.Parameters.AddWithValue("@CompanyId", CompanyId);
                    }
                    else
                        command.Parameters.AddWithValue("@CompanyId", currentCompanyId);
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
        }

        public async Task<GetCompanyServiceRequestsByCompanyId> GetAllCompanyRequestByCompanyIdTemp(int CompanyId, int currentCompanyId, int userId, string Role, int? status, int page, int pageSize, string sortBy,
            string sortDirection, string searchString)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetCompanyServiceRequestsByCompanyId model = new GetCompanyServiceRequestsByCompanyId();
                List<GetRequests> getRequests = new List<GetRequests>();
                SqlCommand command = null;
                int totalRecords = 0;
                command = new SqlCommand("USP_Admin_GetAllCompanyServiceRequestByCompanyIdWithPaginationTempTable", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 36000;
                if (status != null)
                    command.Parameters.AddWithValue("@Status", status.Value);
                if (Role.StartsWith("Sales"))
                {
                    command.Parameters.AddWithValue("@SalesPersonId", userId);
                    if (CompanyId != 0)
                        command.Parameters.AddWithValue("@CompanyId", CompanyId);
                }
                else if (Role == "RM Team")
                {
                    command.Parameters.AddWithValue("@RmTeamId", userId);
                    if (CompanyId != 0)
                        command.Parameters.AddWithValue("@CompanyId", CompanyId);
                }
                else
                    command.Parameters.AddWithValue("@CompanyId", currentCompanyId);
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
                command = new SqlCommand("USP_Admin_GetAllCompanyServiceRequestByCompanyIdWithPaginationTempTableCount", con);
                command.CommandType = CommandType.StoredProcedure;
                if (status != null)
                    command.Parameters.AddWithValue("@Status", status.Value);
                if (Role.StartsWith("Sales"))
                {
                    command.Parameters.AddWithValue("@SalesPersonId", userId);
                    if (CompanyId != 0)
                        command.Parameters.AddWithValue("@CompanyId", CompanyId);
                }
                else if (Role == "RM Team")
                {
                    command.Parameters.AddWithValue("@RmTeamId", userId);
                    if (CompanyId != 0)
                        command.Parameters.AddWithValue("@CompanyId", CompanyId);
                }
                else
                    command.Parameters.AddWithValue("@CompanyId", currentCompanyId);
                command.Parameters.AddWithValue("@searchPrefix", searchString);

                con.Open();
                reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["ServiceRequestCount"] != null)
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
        public async Task<GetEmployeeServiceRequestsByCompanyId> GetAllEmployeeRequestByCompanyId(int CompanyId,int currentCompanyId, int userId, string Role, int? status, int page, int pageSize, string sortBy, string sortDirection, string searchString,int count)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetEmployeeServiceRequestsByCompanyId model = new GetEmployeeServiceRequestsByCompanyId();
                List<GetRequests> getRequests = new List<GetRequests>();
                SqlCommand command = null;
                int totalRecords = 0;
                command = new SqlCommand("USP_Admin_GetAllEmployeeServiceRequestByCompanyIdWithPagination", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 36000;
                if (status != null)
                    command.Parameters.AddWithValue("@Status", status.Value);
                if (Role.StartsWith("Sales"))
                {
                    command.Parameters.AddWithValue("@SalesPersonId", userId);
                    if (CompanyId != 0)
                        command.Parameters.AddWithValue("@CompanyId", CompanyId);
                }
                else if (Role == "RM Team")
                {
                    command.Parameters.AddWithValue("@RmTeamId", userId);
                    if (CompanyId != 0)
                        command.Parameters.AddWithValue("@CompanyId", CompanyId);
                }
                else
                    command.Parameters.AddWithValue("@CompanyId", currentCompanyId);
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
                if (count == 0)
                {
                    command = new SqlCommand("USP_Admin_GetAllEmployeeServiceRequestByCompanyIdWithCount", con);
                    command.CommandType = CommandType.StoredProcedure;
                    if (status != null)
                        command.Parameters.AddWithValue("@Status", status.Value);
                    if (Role.StartsWith("Sales"))
                    {
                        command.Parameters.AddWithValue("@SalesPersonId", userId);
                        if (CompanyId != 0)
                            command.Parameters.AddWithValue("@CompanyId", CompanyId);
                    }
                    else if (Role == "RM Team")
                    {
                        command.Parameters.AddWithValue("@RmTeamId", userId);
                        if (CompanyId != 0)
                            command.Parameters.AddWithValue("@CompanyId", CompanyId);
                    }
                    else
                        command.Parameters.AddWithValue("@CompanyId", currentCompanyId);
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
        }

        public async Task<GetEmployeeServiceRequestsByCompanyId> GetAllEmployeeRequestByCompanyIdTemp(int CompanyId, int currentCompanyId, int userId, string Role, int? status, int page, int pageSize, string sortBy, string sortDirection,
            string searchString)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetEmployeeServiceRequestsByCompanyId model = new GetEmployeeServiceRequestsByCompanyId();
                List<GetRequests> getRequests = new List<GetRequests>();
                SqlCommand command = null;
                int totalRecords = 0;
                command = new SqlCommand("USP_Admin_GetAllEmployeeServiceRequestByCompanyIdWithPaginationTempTable", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 36000;
                if (status != null)
                    command.Parameters.AddWithValue("@Status", status.Value);
                if (Role.StartsWith("Sales"))
                {
                    command.Parameters.AddWithValue("@SalesPersonId", userId);
                    if (CompanyId != 0)
                        command.Parameters.AddWithValue("@CompanyId", CompanyId);
                }
                else if (Role == "RM Team")
                {
                    command.Parameters.AddWithValue("@RmTeamId", userId);
                    if (CompanyId != 0)
                        command.Parameters.AddWithValue("@CompanyId", CompanyId);
                }
                else
                    command.Parameters.AddWithValue("@CompanyId", currentCompanyId);
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
                    getRequest.ApplicantName = reader["ApplicantName"].ToString();
                    getRequests.Add(getRequest);
                }
                con.Close();
                
                command = new SqlCommand("USP_Admin_GetAllEmployeeServiceRequestByCompanyIdWithPaginationTempTableCount", con);
                command.CommandType = CommandType.StoredProcedure;
                if (status != null)
                    command.Parameters.AddWithValue("@Status", status.Value);
                if (Role.StartsWith("Sales"))
                {
                    command.Parameters.AddWithValue("@SalesPersonId", userId);
                    if (CompanyId != 0)
                        command.Parameters.AddWithValue("@CompanyId", CompanyId);
                }
                else if (Role == "RM Team")
                {
                    command.Parameters.AddWithValue("@RmTeamId", userId);
                    if (CompanyId != 0)
                        command.Parameters.AddWithValue("@CompanyId", CompanyId);
                }
                else
                    command.Parameters.AddWithValue("@CompanyId", currentCompanyId);
                command.Parameters.AddWithValue("@searchPrefix", searchString);

                con.Open();
                reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["ServiceRequestCount"] != null)
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

        public async Task<GetAssignedCompanyServiceRequests> GetAllAssignedCompanyServiceRequest(int userId, int? status, int page, int pageSize, string sortBy, string sortDirection, string searchString,int count)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetAssignedCompanyServiceRequests model = new GetAssignedCompanyServiceRequests();
                List<GetRequests> getRequests = new List<GetRequests>();
                SqlCommand command = null;
                int totalRecords = 0;
                command = new SqlCommand("USP_Admin_GetAllCompanyAssignedServiceRequest", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 36000;
                command.Parameters.AddWithValue("@UserId", userId);
                if(status != null) 
                    command.Parameters.AddWithValue("@Status", status.Value);
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
                    getRequest.SalesPerson = reader["SalesPerson"].ToString();
                    getRequests.Add(getRequest);

                }
                con.Close();

                if (count == 0)
                {
                    command = new SqlCommand("USP_Admin_GetAllCompanyAssignedServiceRequestCount", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", userId);
                    if (status != null)
                        command.Parameters.AddWithValue("@Status", status.Value);
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
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<GetAssignedCompanyServiceRequests> GetAllAssignedCompanyServiceRequestTempData(int userId, int? status, int page, int pageSize, string sortBy, string sortDirection,string searchBy, string searchString)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetAssignedCompanyServiceRequests model = new GetAssignedCompanyServiceRequests();
                List<GetRequests> getRequests = new List<GetRequests>();
                SqlCommand command = null;
                int totalRecords = 0;
                command = new SqlCommand("USP_Admin_GetAllCompanyAssignedServiceRequestToDIBNTeam", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 36000;
                command.Parameters.AddWithValue("@UserId", userId);
                if (status != null)
                    command.Parameters.AddWithValue("@Status", status.Value);
                command.Parameters.AddWithValue("@skipRows", page);
                command.Parameters.AddWithValue("@RowsOfPage", pageSize);
                if(searchBy != null && searchBy!="")
                    command.Parameters.AddWithValue("@searchBy", searchBy);
                if(searchString!=null && searchString!="")
                    command.Parameters.AddWithValue("@searchPrefix", searchString);
                if(sortBy!=null && sortBy!="")
                    command.Parameters.AddWithValue("@sortColumn", sortBy);
                if(sortDirection != null && sortDirection != "")
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
                    getRequest.SalesPerson = reader["SalesPerson"].ToString();
                    getRequests.Add(getRequest);

                }
                con.Close();

                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_GetAllCompanyAssignedServiceRequestToDIBNTeamCount", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", userId);
                if (status != null)
                    command.Parameters.AddWithValue("@Status", status.Value);
                if(searchBy != null && searchBy != "")
                    command.Parameters.AddWithValue("@searchBy", searchBy);
                if(searchString != null && searchString != "")
                    command.Parameters.AddWithValue("@searchPrefix", searchString);

                con.Open();
                reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["ServiceRequestCount"] != null)
                        totalRecords = Convert.ToInt32(reader["ServiceRequestCount"]);
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

        public async Task<GetAssignedEmployeeServiceRequests> GetAllAssignedEmployeeServiceRequestTempData(int userId, int? status, int page, int pageSize, string sortBy, string sortDirection,string searchBy, string searchString)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetAssignedEmployeeServiceRequests model = new GetAssignedEmployeeServiceRequests();
                List<GetRequests> getRequests = new List<GetRequests>();
                SqlCommand command = null;
                int totalRecords = 0;
                command = new SqlCommand("USP_Admin_GetAllEmployeeAssignedServiceRequestToDIBNTeam", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 36000;
                command.Parameters.AddWithValue("@UserId", userId);
                if (status != null)
                    command.Parameters.AddWithValue("@Status", status.Value);
                command.Parameters.AddWithValue("@skipRows", page);
                command.Parameters.AddWithValue("@RowsOfPage", pageSize);
                if(searchBy != null && searchBy != "")
                    command.Parameters.AddWithValue("@searchBy", searchBy);
                if(searchString != null && searchString != "")
                    command.Parameters.AddWithValue("@searchPrefix", searchString);
                if(sortBy != null && sortBy != "")
                    command.Parameters.AddWithValue("@sortColumn", sortBy);
                if(sortDirection != null && sortDirection != "")
                    command.Parameters.AddWithValue("@sortDirection", sortDirection);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
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
                    getRequest.SalesPerson = reader["SalesPerson"].ToString();
                    getRequest.ApplicantName = reader["ApplicantName"].ToString();
                           
                    getRequests.Add(getRequest);
                }
                con.Close();

                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_GetAllEmployeeAssignedServiceRequestToDIBNTeamCount", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", userId);
                if (status != null)
                    command.Parameters.AddWithValue("@Status", status.Value);
                if(searchBy != null && searchBy != "")
                    command.Parameters.AddWithValue("@searchBy", searchBy);
                if(searchString != null && searchString != "")
                    command.Parameters.AddWithValue("@searchPrefix", searchString);

                con.Open();
                reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["ServiceRequestCount"] != null)
                        totalRecords = Convert.ToInt32(reader["ServiceRequestCount"]);
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

        //public async Task<GetAssignedCompanyServiceRequests> GetAllAssignedCompanyServiceRequestFilter(int userId, int? status, int page, int pageSize, string sortBy, string sortDirection, string searchBy, string searchString)
        //{
        //    SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
        //    try
        //    {
        //        GetAssignedCompanyServiceRequests model = new GetAssignedCompanyServiceRequests();
        //        List<GetRequests> getRequests = new List<GetRequests>();
        //        SqlCommand command = null;
        //        int totalRecords = 0;
        //        command = new SqlCommand("USP_Admin_GetAllCompanyAssignedServiceRequest", con);
        //        command.CommandType = CommandType.StoredProcedure;
        //        command.Parameters.AddWithValue("@UserId", userId);
        //        if (status != null)
        //            command.Parameters.AddWithValue("@Status", status.Value);
        //        command.Parameters.AddWithValue("@skipRows", page);
        //        command.Parameters.AddWithValue("@RowsOfPage", pageSize);
        //        command.Parameters.AddWithValue("@searchBy", searchBy);
        //        command.Parameters.AddWithValue("@searchPrefix", searchString);
        //        command.Parameters.AddWithValue("@sortColumn", sortBy);
        //        command.Parameters.AddWithValue("@sortDirection", sortDirection);

        //        con.Open();
        //        SqlDataReader reader = await command.ExecuteReaderAsync();
        //        while (reader.Read())
        //        {
        //            GetRequests getRequest = new GetRequests();
        //            getRequest.SerialNumber = reader["SerialNumber"].ToString();
        //            getRequest.RequestedService = reader["RequestedService"].ToString();
        //            getRequest.CompanyName = reader["CompanyName"].ToString();
        //            getRequest.RequestedBy = reader["RequestedBy"].ToString();
        //            getRequest.CreatedOn = reader["CreatedOn"].ToString();
        //            getRequest.Status = reader["Status"].ToString();
        //            getRequest.AssignedUser = reader["AssignedUser"].ToString();
        //            getRequest.SalesPerson = reader["SalesPerson"].ToString();
        //            getRequests.Add(getRequest);

        //        }
        //        con.Close();


        //        command = new SqlCommand("USP_Admin_GetAllCompanyAssignedServiceRequestCount", con);
        //        command.CommandType = CommandType.StoredProcedure;
        //        command.Parameters.AddWithValue("@UserId", userId);
        //        if (status != null)
        //            command.Parameters.AddWithValue("@Status", status.Value);
        //        command.Parameters.AddWithValue("@searchBy", searchBy);
        //        command.Parameters.AddWithValue("@searchPrefix", searchString);

        //        con.Open();
        //        reader = await command.ExecuteReaderAsync();
        //        while (reader.Read())
        //        {
        //            if (reader["SerialNumber"] != null)
        //                totalRecords++;
        //        }
        //        con.Close();

        //        model.totalRecords = totalRecords;

        //        model.totalRecords = totalRecords;
        //        model.getRequests = getRequests;
        //        return model;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        public async Task<GetAssignedEmployeeServiceRequests> GetAllAssignedEmployeeServiceRequest(int userId, int? status, int page, int pageSize, string sortBy, string sortDirection, string searchString,int count)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetAssignedEmployeeServiceRequests model = new GetAssignedEmployeeServiceRequests();
                List<GetRequests> getRequests = new List<GetRequests>();
                SqlCommand command = null;
                int totalRecords = 0;
                command = new SqlCommand("USP_Admin_GetAllEmployeeAssignedServiceRequest", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 36000;
                command.Parameters.AddWithValue("@UserId", userId);
                if (status != null)
                    command.Parameters.AddWithValue("@Status", status.Value);
                command.Parameters.AddWithValue("@skipRows", page);
                command.Parameters.AddWithValue("@RowsOfPage", pageSize);
                command.Parameters.AddWithValue("@searchPrefix", searchString);
                command.Parameters.AddWithValue("@sortColumn", sortBy);
                command.Parameters.AddWithValue("@sortDirection", sortDirection);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
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
                    getRequest.SalesPerson = reader["SalesPerson"].ToString();

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

                command = new SqlCommand("USP_Admin_GetAllEmployeeAssignedServiceRequestCount", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", userId);
                if (status != null)
                    command.Parameters.AddWithValue("@Status", status.Value);
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

        //public async Task<GetAssignedEmployeeServiceRequests> GetAllAssignedEmployeeServiceRequestFilter(int userId, int? status, int page, int pageSize, string sortBy, string sortDirection, string searchBy,string searchString)
        //{
        //    SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
        //    try
        //    {
        //        GetAssignedEmployeeServiceRequests model = new GetAssignedEmployeeServiceRequests();
        //        List<GetRequests> getRequests = new List<GetRequests>();
        //        SqlCommand command = null;
        //        int totalRecords = 0;
        //        command = new SqlCommand("USP_Admin_GetAllEmployeeAssignedServiceRequest", con);
        //        command.CommandType = CommandType.StoredProcedure;
        //        command.CommandTimeout = 36000;
        //        command.Parameters.AddWithValue("@UserId", userId);
        //        if (status != null)
        //            command.Parameters.AddWithValue("@Status", status.Value);
        //        command.Parameters.AddWithValue("@skipRows", page);
        //        command.Parameters.AddWithValue("@RowsOfPage", pageSize);
        //        command.Parameters.AddWithValue("@searchBy", searchBy);
        //        command.Parameters.AddWithValue("@searchPrefix", searchString);
        //        command.Parameters.AddWithValue("@sortColumn", sortBy);
        //        command.Parameters.AddWithValue("@sortDirection", sortDirection);

        //        con.Open();
        //        SqlDataReader reader = command.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            GetRequests getRequest = new GetRequests();
        //            getRequest.SerialNumber = reader["SerialNumber"].ToString();
        //            getRequest.RequestedService = reader["RequestedService"].ToString();
        //            getRequest.CompanyName = reader["CompanyName"].ToString();
        //            getRequest.RequestedBy = reader["RequestedBy"].ToString();
        //            getRequest.CreatedOn = reader["CreatedOn"].ToString();
        //            getRequest.Status = reader["Status"].ToString();
        //            getRequest.AssignedUser = reader["AssignedUser"].ToString();
        //            getRequest.SalesPerson = reader["SalesPerson"].ToString();

        //            SqlCommand command1 = new SqlCommand("USP_Admin_GetApplicantNameBySerialNumber", con);
        //            command1.CommandType = CommandType.StoredProcedure;
        //            command1.Parameters.AddWithValue("@SerialNumber", getRequest.SerialNumber);

        //            SqlDataReader reader1 = await command1.ExecuteReaderAsync();

        //            if (reader1.HasRows)
        //            {
        //                while (reader1.Read())
        //                {
        //                    if (reader1["ApplicantName"] != DBNull.Value)
        //                        getRequest.ApplicantName = reader1["ApplicantName"].ToString();
        //                    else
        //                        getRequest.ApplicantName = "N/A";
        //                }

        //            }
        //            else
        //            {
        //                getRequest.ApplicantName = "N/A";
        //            }

        //            getRequests.Add(getRequest);

        //        }
        //        con.Close();

        //        command = new SqlCommand("USP_Admin_GetAllEmployeeAssignedServiceRequestCount", con);
        //        command.CommandType = CommandType.StoredProcedure;
        //        command.Parameters.AddWithValue("@UserId", userId);
        //        if (status != null)
        //            command.Parameters.AddWithValue("@Status", status.Value);
        //        command.Parameters.AddWithValue("@searchBy", searchBy);
        //        command.Parameters.AddWithValue("@searchPrefix", searchString);

        //        con.Open();
        //        reader = await command.ExecuteReaderAsync();
        //        while (reader.Read())
        //        {
        //            if (reader["SerialNumber"] != null)
        //                totalRecords++;
        //        }
        //        con.Close();

        //        model.totalRecords = totalRecords;
               
        //        model.totalRecords = totalRecords;
        //        model.getRequests = getRequests;
        //        return model;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}
        public GetRequestCompleteDetails GetRequestDetail(string serialNumber)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetRequestCompleteDetails getRequestCompleteDetails = new GetRequestCompleteDetails();
                List<GetRequestDetails> getRequestDetails = new List<GetRequestDetails>();
                List<GetRequestResponses> getRequestResponses = new List<GetRequestResponses>();
                SqlCommand command = new SqlCommand("USP_Admin_GetRequestDetailsBySerialNumber_TEST", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                command.Parameters.AddWithValue("@SerialNumber", serialNumber);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetRequestDetails details = new GetRequestDetails();
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
                    GetRequestResponses model = new GetRequestResponses();
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
		public List<DownloadServiceRequestUploadedDocumentCmp> DownloadUploadedServiceRequestDocument(string serialNumber, int companyId, string FileName, int ServiceId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<DownloadServiceRequestUploadedDocumentCmp> getRequestCompleteDetails = new List<DownloadServiceRequestUploadedDocumentCmp>();
                SqlCommand command = new SqlCommand("USP_GetUploadedServiceRequestDocument", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@SerialNumber", serialNumber);
                command.Parameters.AddWithValue("@CompanyId", companyId);
                command.Parameters.AddWithValue("@FileName", FileName);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    DownloadServiceRequestUploadedDocumentCmp details = new DownloadServiceRequestUploadedDocumentCmp();
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
                    DownloadServiceRequestUploadedDocumentCmp details = new DownloadServiceRequestUploadedDocumentCmp();
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
        public int LastStepNumber(string SerialNumber)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_GetLastStepNumber", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
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
        public int ChangeStatusOfService(string SerialNumber, int Status, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string _query = "";
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_ChangeServiceStatus", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
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

                //GetRequestResponses model = new GetRequestResponses();
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
        public List<ServiceRequestDocument> GetServiceRequestDocument(int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<ServiceRequestDocument> serviceRequestDocument = new List<ServiceRequestDocument>();
                SqlCommand command = new SqlCommand("USP_Admin_GetServiceRequestDocumentById", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
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
        public int GetLatestStatusOfRequest(string SerialNumber)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_GetLatestStatusOfRequest", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
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
        public int GetCountOfResponse(string SerialNumber)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _responseCount = 0;

                SqlCommand command = new SqlCommand("USP_Admin_GetServiceRequestResponseCount", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
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
        public int SaveStatusOfService(GetRequestResponses getRequestResponses)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                string Title = "";

                SqlCommand command = null;

                command = new SqlCommand("USP_Admin_GetServiceRequestFormName", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                command.Parameters.AddWithValue("@formId", getRequestResponses.FormId);

                con.Open();
                Title = (string)command.ExecuteScalar();
                con.Close();

                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_SaveServiceRequestStatus", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                command.Parameters.AddWithValue("@SerialNumber", getRequestResponses.SerialNumber);
                command.Parameters.AddWithValue("@ServiceId", getRequestResponses.ServiceId);
                command.Parameters.AddWithValue("@FormId", getRequestResponses.FormId);
                command.Parameters.AddWithValue("@CompanyId", getRequestResponses.CompanyId);
                command.Parameters.AddWithValue("@UserId", getRequestResponses.UserId);
                command.Parameters.AddWithValue("@Title", getRequestResponses.Title);
                command.Parameters.AddWithValue("@Description", getRequestResponses.Description);
                command.Parameters.AddWithValue("@AssignedTo", getRequestResponses.AssignedTo);
                command.Parameters.AddWithValue("@Status", getRequestResponses.StatusId);
                command.Parameters.AddWithValue("@StepId", getRequestResponses.StepId);

                con.Open();
                _returnId = (int)command.ExecuteScalar();
                con.Close();
                if (_returnId > 0)
                {
                    if (getRequestResponses.formFile != null)
                    {
                        _returnId = SaveServiceRequestDocument(getRequestResponses.formFile, getRequestResponses.SerialNumber, getRequestResponses.ServiceId, getRequestResponses.CompanyId, _returnId, getRequestResponses.CreatedBy);
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
                command.CommandTimeout = 600;
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
        public string GetLastSerialNumber()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string _serialNumber = null;
                
                SqlCommand command = new SqlCommand("USP_Admin_GetLastServiceRequestNumber", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                con.Open();
                _serialNumber = (string)command.ExecuteScalar();
                con.Close();
                return _serialNumber;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<ServiceRequestStatusModel> GetServiceRequestStatus()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<ServiceRequestStatusModel> model = new List<ServiceRequestStatusModel>();

                SqlCommand command = new SqlCommand("USP_Admin_ServiceRequestStatusOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", 2);

                con.Open();

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ServiceRequestStatusModel data = new ServiceRequestStatusModel();
                    data.Id = Convert.ToInt32(reader["Id"]);
                    data.DisplayId = Convert.ToInt32(reader["DisplayId"]);
                    data.DisplayName = Convert.ToString(reader["Status"]);
                    data.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    data.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    data.CreatedBy = Convert.ToInt32(reader["CreatedBy"]);
                    data.ModifyBy = Convert.ToInt32(reader["ModifyBy"]);
                    data.CreatedOn = Convert.ToString(reader["CreatedOn"]);
                    data.ModifyOn = Convert.ToString(reader["ModifyOn"]);
                    model.Add(data);
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
