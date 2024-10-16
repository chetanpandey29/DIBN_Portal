using DIBN.Areas.Admin.Data;
using DIBN.IService;
using DIBN.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Service
{
    public class CompanyServiceList : ICompanyServiceList
    {
        private readonly Connection _dataSetting;
        public CompanyServiceList(Connection dataSetting)
        {
            _dataSetting = dataSetting;
        }
        
        public List<CompanyServices> GetAllCompanyService(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<CompanyServices> CompanyServices = new List<CompanyServices>();
                SqlCommand command = new SqlCommand("USP_Admin_GetAllParentCompanyServiceByCompanyId", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    CompanyServices service = new CompanyServices();
                    service.getChildCompanyServices = new List<CompanyServices>();
                    service.ID = Convert.ToInt32(reader["ID"]);
                    service.ParentId = Convert.ToInt32(reader["ParentId"]);
                    service.ServiceName = reader["ServiceName"].ToString();
                    service.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    service.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    service.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    service.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    service.CompanyType = reader["CompanyType"].ToString();
                    service.getChildCompanyServices = GetAllChildCompanyService(CompanyId, service.ID);
                    CompanyServices.Add(service);

                }
                con.Close();
                return CompanyServices;
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
        public List<CompanyServices> GetAllChildCompanyService(int CompanyId, int ParentId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<CompanyServices> CompanyServices = new List<CompanyServices>();
                SqlCommand command = new SqlCommand("USP_Admin_GetAllCompanyServiceByCompanyId", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@ParentId", ParentId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    CompanyServices service = new CompanyServices();

                    service.ID = Convert.ToInt32(reader["ID"]);
                    service.ParentId = Convert.ToInt32(reader["ParentId"]);
                    service.ServiceName = reader["ServiceName"].ToString();
                    service.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    service.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    service.HasMultipleForm = Convert.ToBoolean(reader["HasMultipleForms"]);
                    service.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    service.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    service.FormConstrains = reader["FormConstrain"].ToString();

                    CompanyServices.Add(service);
                }
                con.Close();
                reader.Close();

                List<CompanyServices> _removeIndex = new List<CompanyServices>();

                if (CompanyServices != null)
                {
                    if (CompanyServices.Count > 0)
                    {
                        for (int index = 0; index < CompanyServices.Count; index++)
                        {
                            if (CompanyServices[index].HasMultipleForm == true)
                            {
                                string[] _forms = { };
                                _forms = CompanyServices[index].FormConstrains.Split("+");
                                if (_forms.Length > 0)
                                {
                                    for (int i = 0; i < _forms.Length; i++)
                                    {
                                        CompanyServices _rowIndex = new CompanyServices();
                                        _rowIndex = CompanyServices.Find(x => x.ServiceName == _forms[i]);
                                        _removeIndex.Add(_rowIndex);
                                    }
                                }
                            }
                        }
                    }
                }
                if (_removeIndex != null && _removeIndex.Count>0)
                {
                    for (int index = 0; index < _removeIndex.Count; index++)
                    {
                        if (_removeIndex[index] != null)
                        {
                            CompanyServices.Remove(_removeIndex[index]);
                        }  
                    }
                }
                
                return CompanyServices;
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
        [Obsolete]
        public async Task<string> sendConfirmationMail(string Email)
        {
            try
            {
                var message = new MailMessage();
                message.To.Add(new MailAddress(Email));
                message.From = new MailAddress("no-reply@gmail.com");
                message.Subject = "Activate Your Company";
                StringBuilder builder = new StringBuilder();
                builder.Append("<p style='color:#000; background-color:#fff;'>Dear Customer, </p>");
                builder.Append("<p style='color:#000; background-color:#fff;'> Congratulation , Your Account has been Activated.</p>");
                message.IsBodyHtml = true;
                message.Body = builder.ToString();
                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential()
                    {
                        //UserName = "no-reply@dibnbusiness.com",
                        //Password = "DIBN@1901"
                        UserName = "sharmayashasvi202@gmail.com",
                        Password = "yashasvi@123"
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
        public List<GetCompanyServiceRequest> GetCompanyServiceRequest(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetCompanyServiceRequest> requests = new List<GetCompanyServiceRequest>();
                SqlCommand cmd = new SqlCommand("USP_Admin_GetCompanyServiceRequestByCompanyId", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    GetCompanyServiceRequest request = new GetCompanyServiceRequest();
                    request.ServiceId = Convert.ToInt32(reader["ServiceId"].ToString());
                    request.SerialNumber = reader["SerialNumber"].ToString();
                    request.Form = reader["Form"].ToString();
                    request.CompanyName = reader["CompanyName"].ToString();
                    request.RequestedBy = reader["RequestedBy"].ToString();
                    requests.Add(request);
                }
                con.Close();
                return requests;
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
        public List<GetCompanyServiceRequestDetails> GetCompanyServiceRequestDetails(int CompanyId, string SerialNumber)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetCompanyServiceRequestDetails> requestDetails = new List<GetCompanyServiceRequestDetails>();
                SqlCommand cmd = new SqlCommand("USP_Admin_GetCompanyServiceRequestDetailsByCompanyId", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                cmd.Parameters.AddWithValue("@SerialNumber", SerialNumber);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    GetCompanyServiceRequestDetails requestDetail = new GetCompanyServiceRequestDetails();
                    if (reader["SerialNumber"] != DBNull.Value)
                        requestDetail.SerialNumber = reader["SerialNumber"].ToString();
                    if (reader["Form"] != DBNull.Value)
                        requestDetail.Form = reader["Form"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        requestDetail.CompanyName = reader["CompanyName"].ToString();
                    if (reader["RequestedBy"] != DBNull.Value)
                        requestDetail.RequestedBy = reader["RequestedBy"].ToString();
                    if (reader["FieldName"] != DBNull.Value)
                        requestDetail.FieldName = reader["FieldName"].ToString();
                    if (reader["FieldValue"] != DBNull.Value)
                        requestDetail.FieldValue = reader["FieldValue"].ToString();
                    if (reader["FileName"] != DBNull.Value)
                        requestDetail.FileName = reader["FileName"].ToString();
                    if (reader["FieldFileValue"] != DBNull.Value)
                        requestDetail.FieldFileValue = (byte[])reader["FieldFileValue"];
                    if (reader["CreatedOn"] != DBNull.Value)
                        requestDetail.CreatedOn = reader["CreatedOn"].ToString();
                    requestDetails.Add(requestDetail);
                }
                con.Close();
                return requestDetails;
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
        public List<string> GetServiceListBySearchName(string prefix)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> Services = new List<string>();
                SqlCommand cmd = new SqlCommand("USP_Admin_SearchServices", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ServiceName", prefix);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["ServiceName"] != DBNull.Value)
                        Services.Add(reader["ServiceName"].ToString());
                }

                reader.NextResult();

                while (reader.Read())
                {
                    if (reader["ServiceName"] != DBNull.Value)
                        Services.Add(reader["ServiceName"].ToString());
                }

                con.Close();
                return Services;
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
        public List<string> GetServiceIdByName(string Service)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> Services = new List<string>();
                SqlCommand cmd = new SqlCommand("USP_Admin_GetServiceIdByName", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ServiceName", Service);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["ServiceId"] != DBNull.Value)
                        Services.Add(reader["ServiceId"].ToString());
                    if (reader["IsCompanyService"] != DBNull.Value)
                        Services.Add(reader["IsCompanyService"].ToString());
                    if (reader["IsEmployeeService"] != DBNull.Value)
                        Services.Add(reader["IsEmployeeService"].ToString());
                }

                reader.NextResult();

                while (reader.Read())
                {
                    if (reader["ServiceId"] != DBNull.Value)
                        Services.Add(reader["ServiceId"].ToString());
                    if (reader["IsCompanyService"] != DBNull.Value)
                        Services.Add(reader["IsCompanyService"].ToString());
                    if (reader["IsEmployeeService"] != DBNull.Value)
                        Services.Add(reader["IsEmployeeService"].ToString());
                }

                con.Close();
                return Services;
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
                SqlCommand cmd = new SqlCommand("USP_Admin_GetCompanyServiceIdByCompanyType", con);
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
    }
}
