using DIBN.Areas.Admin.Data;
using DIBN.IService;
using DIBN.Models;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Service
{
    public class EmployeeServiceList : IEmployeeServiceList
    {
        private readonly Connection _dataSetting;
        public EmployeeServiceList(Connection dataSetting)
        {
            _dataSetting = dataSetting;
        }
        public List<EmployeeServices> GetAllEmployeeService(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<EmployeeServices> employeeServices = new List<EmployeeServices>();
                SqlCommand command = new SqlCommand("USP_Admin_GetAllParentEmployeeServiceByCompanyId", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    EmployeeServices service = new EmployeeServices();
                    service.getChildEmployeeService = new List<EmployeeServices>();
                    service.ID = Convert.ToInt32(reader["ID"]);
                    service.ParentId = Convert.ToInt32(reader["ParentId"]);
                    
                    service.ServiceName = reader["ServiceName"].ToString();
                    service.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    service.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    service.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    service.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    service.CompanyType = reader["CompanyType"].ToString();
                    service.getChildEmployeeService = GetAllChildEmployeeService(CompanyId,service.ID);
                    employeeServices.Add(service);
                }
                con.Close();
                return employeeServices;
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
        public List<EmployeeServices> GetAllChildEmployeeService(int CompanyId,int ParentId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<EmployeeServices> employeeServices = new List<EmployeeServices>();
                SqlCommand command = new SqlCommand("USP_Admin_GetAllEmployeeServiceByCompanyId", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@ParentId", ParentId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    EmployeeServices service = new EmployeeServices();

                    service.ID = Convert.ToInt32(reader["ID"]);
                    
                    service.ParentId = Convert.ToInt32(reader["ParentId"]);
                    service.ServiceName = reader["ServiceName"].ToString();
                    service.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    service.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    service.HasMultipleForm = Convert.ToBoolean(reader["HasMultipleForms"]);
                    service.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    service.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    service.FormConstrains = reader["FormConstrain"].ToString();

                    employeeServices.Add(service);
                }
                con.Close();
                List<EmployeeServices> _removeIndex = new List<EmployeeServices>();

                if (employeeServices != null)
                {
                    if (employeeServices.Count > 0)
                    {
                        for (int index = 0; index < employeeServices.Count; index++)
                        {
                            if (employeeServices[index].HasMultipleForm == true)
                            {
                                string[] _forms = { };
                                _forms = employeeServices[index].FormConstrains.Split("+");
                                if (_forms.Length > 0)
                                {
                                    for (int i = 0; i < _forms.Length; i++)
                                    {
                                        EmployeeServices _rowIndex = new EmployeeServices();
                                        _rowIndex = employeeServices.Find(x => x.ServiceName == _forms[i]);
                                        _removeIndex.Add(_rowIndex);
                                    }
                                }
                            }
                        }
                    }
                }
                if (_removeIndex != null && _removeIndex.Count > 0)
                {
                    for (int index = 0; index < _removeIndex.Count; index++)
                    {
                        if (_removeIndex[index] != null)
                        {
                            employeeServices.Remove(_removeIndex[index]);
                        }
                    }
                }
                return employeeServices;
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
        public int GetServiceId(string ServiceName, string companyType)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand cmd = new SqlCommand("USP_Admin_GetEmployeeServiceIdByCompanyType", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ServiceName", ServiceName);
                cmd.Parameters.AddWithValue("@companyType", companyType);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    _returnId = Convert.ToInt32(reader["ID"]);
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
        public List<EmployeeServiceRequest> GetEmployeeServiceRequests(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<EmployeeServiceRequest> serviceRequests = new List<EmployeeServiceRequest>();
                SqlCommand command = new SqlCommand("USP_Admin_EmployeeServiceRequestOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Show);
                command.Parameters.AddWithValue("@CompanyId",CompanyId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    EmployeeServiceRequest employeeServiceRequest = new EmployeeServiceRequest();
                    employeeServiceRequest.DocumentList = new List<EmployeeServiceRequestDocument>();
                    employeeServiceRequest.ID = Convert.ToInt32(reader["ID"]);
                    employeeServiceRequest.RequestStatusId = Convert.ToInt32(reader["RequestStatusId"]);
                    employeeServiceRequest.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    employeeServiceRequest.UserId = Convert.ToInt32(reader["UserId"]);
                    employeeServiceRequest.ServiceId = Convert.ToInt32(reader["ServiceId"]);
                    employeeServiceRequest.CompanyName = reader["CompanyName"].ToString();
                    employeeServiceRequest.Username = reader["Username"].ToString();
                    employeeServiceRequest.RequestNumber = reader["RequestNumber"].ToString();
                    
                    employeeServiceRequest.ServiceName = reader["ServiceName"].ToString();
                    employeeServiceRequest.Title = reader["Title"].ToString();
                    employeeServiceRequest.Description = reader["Description"].ToString();
                    employeeServiceRequest.RequestStatus = reader["RequestStatus"].ToString();
                    employeeServiceRequest.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    employeeServiceRequest.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    employeeServiceRequest.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    employeeServiceRequest.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    employeeServiceRequest.DocumentList = GetEmployeeServiceRequestDocuments(employeeServiceRequest.ID);
                    serviceRequests.Add(employeeServiceRequest);
                }
                con.Close();
                return serviceRequests;
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
        public List<EmployeeServiceRequest> GetEmployeeServiceRequestsDetails(string RequestId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<EmployeeServiceRequest> serviceRequests = new List<EmployeeServiceRequest>();
                SqlCommand command = new SqlCommand("USP_Admin_GetEmployeeServiceRequestByRequestId", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@RequestId", RequestId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    EmployeeServiceRequest employeeServiceRequest = new EmployeeServiceRequest();
                    employeeServiceRequest.DocumentList = new List<EmployeeServiceRequestDocument>();
                    employeeServiceRequest.ID = Convert.ToInt32(reader["ID"]);
                    employeeServiceRequest.RequestStatusId = Convert.ToInt32(reader["RequestStatusId"]);
                    employeeServiceRequest.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    employeeServiceRequest.UserId = Convert.ToInt32(reader["UserId"]);
                    employeeServiceRequest.ServiceId = Convert.ToInt32(reader["ServiceId"]);
                    employeeServiceRequest.CompanyName = reader["CompanyName"].ToString();
                    employeeServiceRequest.Username = reader["Username"].ToString();
                    employeeServiceRequest.RequestNumber = reader["RequestNumber"].ToString();
                    employeeServiceRequest.RequestStatus = reader["RequestStatus"].ToString();
                    
                    employeeServiceRequest.ServiceName = reader["ServiceName"].ToString();
                    employeeServiceRequest.Title = reader["Title"].ToString();
                    employeeServiceRequest.Description = reader["Description"].ToString();
                    employeeServiceRequest.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    employeeServiceRequest.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    employeeServiceRequest.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    employeeServiceRequest.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    employeeServiceRequest.NewRequestStatus = reader["NewRequestStatus"].ToString();
                    employeeServiceRequest.DocumentList = GetEmployeeServiceRequestDocuments(employeeServiceRequest.ID);
                    employeeServiceRequest.getResponseByParentIds = GetEmployeeResponseReply(employeeServiceRequest.ID);
                    serviceRequests.Add(employeeServiceRequest);
                }
                con.Close();
                return serviceRequests;
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
        public List<GetResponseByParentId> GetEmployeeResponseReply(int ParentId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetResponseByParentId> serviceRequests = new List<GetResponseByParentId>();
                SqlCommand command = new SqlCommand("USP_Admin_GetEmployeeServiceRequestResponse", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ParentId", ParentId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetResponseByParentId employeeServiceRequest = new GetResponseByParentId();
                    employeeServiceRequest.DocumentList = new List<EmployeeServiceRequestDocument>();
                    employeeServiceRequest.Id = Convert.ToInt32(reader["ID"]);
                    employeeServiceRequest.ParentId = Convert.ToInt32(reader["ParentId"]);
                    employeeServiceRequest.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    employeeServiceRequest.UserId = Convert.ToInt32(reader["UserId"]);
                    employeeServiceRequest.ServiceId = Convert.ToInt32(reader["ServiceId"]);
                    employeeServiceRequest.CompanyName = reader["CompanyName"].ToString();
                    employeeServiceRequest.Username = reader["Username"].ToString();
                    employeeServiceRequest.RequestNumber = reader["RequestNumber"].ToString();
                    employeeServiceRequest.RequestStatus = reader["RequestStatus"].ToString();
                    
                    employeeServiceRequest.ServiceName = reader["ServiceName"].ToString();
                    employeeServiceRequest.Title = reader["Title"].ToString();
                    employeeServiceRequest.Description = reader["Description"].ToString();
                    employeeServiceRequest.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    employeeServiceRequest.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    employeeServiceRequest.DocumentList = GetEmployeeServiceRequestDocuments(employeeServiceRequest.Id);
                    serviceRequests.Add(employeeServiceRequest);
                }
                con.Close();
                return serviceRequests;
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
        public int AddEmployeeServiceRequest(EmployeeServiceRequest request)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0,_returnDocumentId=0;
                SqlCommand command = new SqlCommand("USP_Admin_EmployeeServiceRequestOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Insert);
                command.Parameters.AddWithValue("@CompanyId", request.CompanyId);
                command.Parameters.AddWithValue("@UserId", request.UserId);
                command.Parameters.AddWithValue("@ServiceId", request.ServiceId);
                command.Parameters.AddWithValue("@RequestNumber", request.RequestNumber);
                command.Parameters.AddWithValue("@RequestStatus", request.RequestStatusId);
                command.Parameters.AddWithValue("@ParentId", request.ParentId);
                command.Parameters.AddWithValue("@Title", request.Title);
                command.Parameters.AddWithValue("@Description", request.Description);
                
                con.Open();
                _returnId = (int)command.ExecuteScalar();
                con.Close();
                if(request.FormFile != null)
                {
                    if (request.FormFile.Count > 0)
                    {
                        for (int index = 0; index < request.FormFile.Count; index++)
                        {
                            string _getName = request.FormFile[index].FileName;
                            int lastIndex = request.FormFile[index].FileName.LastIndexOf(".");
                            String Name = request.FormFile[index].FileName.Substring(0, lastIndex);
                            string FileName = Name;
                            var extn = Path.GetExtension(_getName);
                            Byte[] bytes = null;
                            using (MemoryStream ms = new MemoryStream())
                            {
                                request.FormFile[index].OpenReadStream().CopyTo(ms);
                                bytes = ms.ToArray();
                            }
                            command = new SqlCommand("USP_Admin_EmployeeServiceRequestDocument", con);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@Status", Operation.Insert);
                            command.Parameters.AddWithValue("@FileName", FileName);
                            command.Parameters.AddWithValue("@Extension", extn);
                            command.Parameters.AddWithValue("@DataBinary", bytes);
                            command.Parameters.AddWithValue("@EmployeeServiceRequestId", _returnId);
                            con.Open();
                            _returnDocumentId = (int)command.ExecuteScalar();
                            con.Close();
                        }
                    }
                }
                
                return _returnDocumentId == 0 ? _returnId : _returnDocumentId;
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
        public string GetRequestNumber()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string returnId = null;
                SqlCommand command = new SqlCommand("USP_Admin_EmployeeServiceRequestOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", 5);
                con.Open();
                returnId = (string)command.ExecuteScalar();
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
        public List<EmployeeServiceRequestDocument> GetEmployeeServiceRequestDocuments(int RequestId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<EmployeeServiceRequestDocument> documentList = new List<EmployeeServiceRequestDocument>();
                SqlCommand command = new SqlCommand("USP_Admin_EmployeeServiceRequestDocument", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Show);
                command.Parameters.AddWithValue("@EmployeeServiceRequestId", RequestId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    EmployeeServiceRequestDocument document = new EmployeeServiceRequestDocument();
                    document.FileName = reader["FileName"].ToString();
                    document.Extension = reader["Extension"].ToString();
                    document.EmployeeServiceRequestId = Convert.ToInt32(reader["EmployeeServiceRequestId"]);
                    document.DataBinary = (Byte[])reader["DataBinary"];
                    document.ID = Convert.ToInt32(reader["ID"]);
                    documentList.Add(document);
                }
                con.Close();
                return documentList;
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
        public EmployeeServiceRequestDocument GetEmployeeServiceRequestDocumentById(int RequestId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                EmployeeServiceRequestDocument document = new EmployeeServiceRequestDocument();
                SqlCommand command = new SqlCommand("USP_Admin_EmployeeServiceRequestDocument", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.GetById);
                command.Parameters.AddWithValue("@ID", RequestId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    document.FileName = reader["FileName"].ToString();
                    document.Extension = reader["Extension"].ToString();
                    document.EmployeeServiceRequestId = Convert.ToInt32(reader["EmployeeServiceRequestId"]);
                    document.DataBinary = (Byte[])reader["DataBinary"];
                    document.ID = Convert.ToInt32(reader["ID"]);
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
        public int GetOpenServiceCountForAssignedUser(int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _countOfService = 0;
                SqlCommand command = new SqlCommand("USP_Admin_GetAssignedOpenServiceRequestCount", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", UserId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while(reader.Read())
                {
                    if (reader["CompanyServiceRequests"] != DBNull.Value)
                        _countOfService = Convert.ToInt32(reader["CompanyServiceRequests"]);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["EmployeeServiceRequest"] != DBNull.Value)
                        _countOfService += Convert.ToInt32(reader["EmployeeServiceRequest"]);
                }
                con.Close();
                return _countOfService;
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
        public int GetCloseServiceCountForAssignedUser(int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _countOfService = 0;
                SqlCommand command = new SqlCommand("USP_Admin_GetAssignedClosedServiceRequestCount", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", UserId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["CompanyServiceRequests"] != DBNull.Value)
                        _countOfService = Convert.ToInt32(reader["CompanyServiceRequests"]);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["EmployeeServiceRequest"] != DBNull.Value)
                        _countOfService += Convert.ToInt32(reader["EmployeeServiceRequest"]);
                }
                con.Close();
                return _countOfService;
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
        public int GetRejectedServiceCountForAssignedUser(int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _countOfService = 0;
                SqlCommand command = new SqlCommand("USP_Admin_GetAssignedRejectedServiceRequestCount", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", UserId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["CompanyServiceRequests"] != DBNull.Value)
                        _countOfService = Convert.ToInt32(reader["CompanyServiceRequests"]);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["EmployeeServiceRequest"] != DBNull.Value)
                        _countOfService += Convert.ToInt32(reader["EmployeeServiceRequest"]);
                }
                con.Close();
                return _countOfService;
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
        public int GetOpenServiceCount(int CompanyId,int SalesPerson,int RMTeamId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                SqlCommand command = null;
                int _countOfService = 0;
                List<int> _companies = new List<int>();
                if(CompanyId > 0)
                {
                    command = new SqlCommand("USP_Admin_GetCompanyOpenServiceRequestCount", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CompanyId", CompanyId);
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader["SerialNumber"] != DBNull.Value)
                            _countOfService++;
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        if (reader["SerialNumber"] != DBNull.Value)
                            _countOfService++;
                    }
                    con.Close();
                }
                else if(SalesPerson > 0) 
                {
                    command = new SqlCommand("USP_Admin_GetSalesPersonOpenServiceRequestCount", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SalesPerson", SalesPerson);
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader["SerialNumber"] != DBNull.Value)
                            _countOfService++;
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        if (reader["SerialNumber"] != DBNull.Value)
                            _countOfService++;
                    }
                    con.Close();
                }
                else
                {
                    command = new SqlCommand("USP_Admin_GetRMTeamOpenServiceRequestCount", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@RMTeamId", RMTeamId);
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader["SerialNumber"] != DBNull.Value)
                            _countOfService++;
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        if (reader["SerialNumber"] != DBNull.Value)
                            _countOfService++;
                    }
                    con.Close();
                }

                return _countOfService;
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
        public int GetCloseServiceCount(int CompanyId,int SalesPerson, int RMTeamId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                SqlCommand command = null;
                int _countOfService = 0;
                List<int> _companies = new List<int>();
                if (CompanyId > 0)
                {
                    command = new SqlCommand("USP_Admin_GetCompanyCloseServiceRequestCount", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CompanyId", CompanyId);
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader["SerialNumber"] != DBNull.Value)
                            _countOfService++;
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        if (reader["SerialNumber"] != DBNull.Value)
                            _countOfService++;
                    }
                    con.Close();
                }
                else if(SalesPerson > 0) 
                {
                    command = new SqlCommand("USP_Admin_GetSalesPersonCloseServiceRequestCount", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SalesPerson", SalesPerson);
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader["SerialNumber"] != DBNull.Value)
                            _countOfService++;
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        if (reader["SerialNumber"] != DBNull.Value)
                            _countOfService++;
                    }
                    con.Close();
                }
                else
                {
                    command = new SqlCommand("USP_Admin_GetRMTeamCloseServiceRequestCount", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@RMTeamId", RMTeamId);
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader["SerialNumber"] != DBNull.Value)
                            _countOfService++;
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        if (reader["SerialNumber"] != DBNull.Value)
                            _countOfService++;
                    }
                    con.Close();
                }
                return _countOfService;
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
        public int GetRejectedServiceCount(int CompanyId,int SalesPerson,int RMTeamId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                SqlCommand command = null;
                int _countOfService = 0;
                List<int> _companies = new List<int>();
                if (CompanyId > 0)
                {
                    command = new SqlCommand("USP_Admin_GetCompanyRejectedServiceRequestCount", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CompanyId", CompanyId);
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader["SerialNumber"] != DBNull.Value)
                            _countOfService++;
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        if (reader["SerialNumber"] != DBNull.Value)
                            _countOfService++;
                    }
                    con.Close();
                }
                else if(SalesPerson > 0) 
                {
                    command = new SqlCommand("USP_Admin_GetSalesPersonRejectedServiceRequestCount", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SalesPerson", SalesPerson);
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader["SerialNumber"] != DBNull.Value)
                            _countOfService++;
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        if (reader["SerialNumber"] != DBNull.Value)
                            _countOfService++;
                    }
                    con.Close();
                }
                else
                {
                    command = new SqlCommand("USP_Admin_GetRMTeamRejectedServiceRequestCount", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@RMTeamId", RMTeamId);
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader["SerialNumber"] != DBNull.Value)
                            _countOfService++;
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        if (reader["SerialNumber"] != DBNull.Value)
                            _countOfService++;
                    }
                    con.Close();
                }

                return _countOfService;
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
        public int GetOpenSupportTicketCount(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _countOfService = 0;
                SqlCommand command = new SqlCommand("USP_Admin_GetCompanyOpenTicketCount", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                _countOfService = Convert.ToInt32(command.ExecuteScalar());
                con.Close();
                return _countOfService;
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

        public int GetCurrentNotificationCount(int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnCount = 0;
                SqlCommand command = new SqlCommand("USP_Admin_GetRequestNotificationByAssignId", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", UserId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (Convert.ToInt32(reader["SupportTicketCreatedById"]) != UserId)
                    {
                        returnCount++;
                    }

                }

                reader.NextResult();

                while (reader.Read())
                {
                    if (Convert.ToInt32(reader["ServiceCreatedById"]) != UserId)
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
        public int GetCloseSupportTicketCount(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _countOfService = 0;
                SqlCommand command = new SqlCommand("USP_Admin_GetCompanyCloseTicketCount", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                _countOfService = Convert.ToInt32(command.ExecuteScalar());
                con.Close();
                return _countOfService;
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
