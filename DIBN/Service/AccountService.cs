using DIBN.Areas.Admin.Data;
using DIBN.IService;
using DIBN.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static DIBN.Areas.Admin.Data.DataSetting;
using static DIBN.Models.AccountViewModel;

namespace DIBN.Service
{
    public class AccountService : IAccountService
    {
        private readonly Connection _dataSetting;
        private readonly EncryptionService _encryptionService;
        public AccountService(Connection dataSetting,EncryptionService encryptionService)
        {
            _dataSetting = dataSetting;
            _encryptionService = encryptionService;
        }
        public List<string> Login(LoginViewModel model)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string emailAddress = "";
                Regex re = new Regex(@"(@)(.+)$");
                Match result = re.Match(model.Email);
                if (result.Success)
                {
                    emailAddress = model.Email;
                }
                var _password = _encryptionService.EncryptText(model.Password);
                List<string> returnId = new List<string>();
                List<LoginReturnResult> returnResults = new List<LoginReturnResult>();
                SqlCommand cmd = new SqlCommand("USP_Login", con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (result.Success)
                {
                    cmd.Parameters.AddWithValue("@EmailId", model.Email);
                }
                else{
                    cmd.Parameters.AddWithValue("@AccountNumber", model.Email);
                }
                cmd.Parameters.AddWithValue("@Password", _password);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                
                while (dr.Read())
                {
                    LoginReturnResult loginReturnResult = new LoginReturnResult();
                    loginReturnResult.CompanyId = dr["CompanyId"].ToString();
                    loginReturnResult.Role = dr["RoleName"].ToString();
                    loginReturnResult.countOfPermission = Convert.ToInt16(dr["CountOfPermission"].ToString());
                    returnResults.Add(loginReturnResult);   
                }
                int _previousCount = 0;
                if (returnResults != null)
                {
                    if(returnResults.Count > 1)
                    {
                        for(int index=0; index<returnResults.Count; index++)
                        {
                            if(_previousCount == 0)
                            {
                                _previousCount = returnResults[index].countOfPermission;
                                if (returnId != null && returnId.Count > 0)
                                {
                                    returnId.RemoveAt(1);  returnId.RemoveAt(0); 
                                }
                                
                                string _role = returnResults[index].Role;
                                string _companyId = returnResults[index].CompanyId;
                                returnId.Add(_role);
                                returnId.Add(_companyId);
                            }
                            else
                            {
                                if(returnResults[index].countOfPermission > _previousCount)
                                {
                                    _previousCount = returnResults[index].countOfPermission;
                                    if (returnId != null && returnId.Count>0)
                                    {
                                        returnId.RemoveAt(1); returnId.RemoveAt(0); 
                                    }
                                    string _role = returnResults[index].Role;
                                    string _companyId = returnResults[index].CompanyId;
                                    returnId.Add(_role);
                                    returnId.Add(_companyId);
                                }
                            }
                            
                        }
                    }
                    else
                    {
                        for (int index = 0; index < returnResults.Count; index++)
                        {
                            string _role = returnResults[index].Role;
                            string _companyId = returnResults[index].CompanyId;
                            returnId.Add(_role);
                            returnId.Add(_companyId);

                        }
                    }
                }

                con.Close();
                return returnId;
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
        public List<string> SalesPersonLogin(LoginViewModel model)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string emailAddress = "";
                Regex re = new Regex(@"(@)(.+)$");
                Match result = re.Match(model.Email);
                if (result.Success)
                {
                    emailAddress = model.Email;
                }
                var _password = _encryptionService.EncryptText(model.Password);
                List<string> returnId = new List<string>();
                List<LoginReturnResult> returnResults = new List<LoginReturnResult>();
                SqlCommand cmd = new SqlCommand("USP_Admin_SalesPersonLogin", con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (result.Success)
                {
                    cmd.Parameters.AddWithValue("@EmailId", model.Email);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@AccountNumber", model.Email);
                }
                cmd.Parameters.AddWithValue("@Password", _password);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    LoginReturnResult loginReturnResult = new LoginReturnResult();
                    loginReturnResult.CompanyId = dr["CompanyId"].ToString();
                    loginReturnResult.Role = dr["RoleName"].ToString();
                    loginReturnResult.countOfPermission = Convert.ToInt16(dr["CountOfPermission"].ToString());
                    returnResults.Add(loginReturnResult);
                }
                int _previousCount = 0;
                if (returnResults != null)
                {
                    if (returnResults.Count > 1)
                    {
                        for (int index = 0; index < returnResults.Count; index++)
                        {
                            if (_previousCount == 0)
                            {
                                _previousCount = returnResults[index].countOfPermission;
                                if (returnId != null && returnId.Count > 0)
                                {
                                    returnId.RemoveAt(0); returnId.RemoveAt(1);
                                }

                                string _role = returnResults[index].Role;
                                string _companyId = returnResults[index].CompanyId;
                                returnId.Add(_role);
                                returnId.Add(_companyId);
                            }
                            else
                            {
                                if (returnResults[index].countOfPermission > _previousCount)
                                {
                                    _previousCount = returnResults[index].countOfPermission;
                                    if (returnId != null && returnId.Count > 0)
                                    {
                                        returnId.RemoveAt(1); returnId.RemoveAt(0);
                                    }
                                    string _role = returnResults[index].Role;
                                    string _companyId = returnResults[index].CompanyId;
                                    returnId.Add(_role);
                                    returnId.Add(_companyId);
                                }
                            }

                        }
                    }
                    else
                    {
                        for (int index = 0; index < returnResults.Count; index++)
                        {
                            string _role = returnResults[index].Role;
                            string _companyId = returnResults[index].CompanyId;
                            returnId.Add(_role);
                            returnId.Add(_companyId);

                        }
                    }
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
        public async Task<string> sendMail(string CustomerName, string Email, string url)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _mailTemplateId = 0;
                SqlCommand cmd = new SqlCommand("USP_Admin_GetMessageTemplateIdByName", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@messageTemplateName", "Forget Password");

                con.Open();
                _mailTemplateId = (int)cmd.ExecuteScalar();
                con.Close();

                MessageTemplateViewModel model = new MessageTemplateViewModel();
                model = GetMessageTemplateDetails(_mailTemplateId);
                string _MessageBody = model.Body;

                var message = new MailMessage();
                message.To.Add(new MailAddress(Email));
                message.From = new MailAddress(model.FromMail);
                message.Subject = model.Subject;

                _MessageBody = _MessageBody.Replace("%Url%", url);
                _MessageBody = _MessageBody.Replace("%CustomerName%", CustomerName);
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
                    string result = "1";
                    return result;
                }
            }
            catch (Exception ex)
            {
                return "-1";
            }
        }

        public MessageTemplateViewModel GetMessageTemplateDetails(int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                MessageTemplateViewModel messageTemplate = new MessageTemplateViewModel();
                SqlCommand command = new SqlCommand("USP_Admin_GetAllMessageTemplate", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", Id);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    messageTemplate.Id = Convert.ToInt32(reader["Id"]);
                    messageTemplate.Name = reader["Name"].ToString();
                    messageTemplate.Subject = reader["Subject"].ToString();
                    messageTemplate.FromMail = reader["MailFrom"].ToString();
                    messageTemplate.Body = reader["Body"].ToString();
                    messageTemplate.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    messageTemplate.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    messageTemplate.CreatedOn = reader["CreatedOn"].ToString();
                    messageTemplate.ModifyOn = reader["ModifyOn"].ToString();
                    messageTemplate.CreatedBy = Convert.ToInt32(reader["CreatedBy"]);
                    messageTemplate.ModifyBy = Convert.ToInt32(reader["ModifyBy"]);
                }
                con.Close();

                return messageTemplate;
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
        public List<string> CheckExistanceOfEmail(string Email)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> returnId = new List<string>();
                SqlCommand cmd = new SqlCommand("USP_Admin_GetUserIdByEmail", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", Email);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    returnId.Add(reader["Id"].ToString());
                    returnId.Add(reader["Username"].ToString());
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

        public string GetAccountType(string Email)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string returnId = "";
                SqlCommand cmd = new SqlCommand("USP_Admin_GetTypeOfAccountByEmail", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", Email);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    returnId=reader["Type"].ToString();
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

        public int ChangePassword(ChangePasswordModel model)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;

                string _password = _encryptionService.EncryptText(model.NewPassword);
                
                SqlCommand cmd = new SqlCommand("USP_Admin_ChangePassword", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AccountType", model.AccountType);
                cmd.Parameters.AddWithValue("@Password", _password);
                cmd.Parameters.AddWithValue("@Id", model.Id);

                con.Open();
                cmd.ExecuteNonQuery();
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
        public int CheckSalesPerson(string Email)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand cmd = new SqlCommand("USP_Admin_CheckSalesPerson", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmailId", Email);
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
    }
}
