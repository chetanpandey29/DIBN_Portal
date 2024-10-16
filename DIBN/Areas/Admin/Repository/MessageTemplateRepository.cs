using DIBN.Areas.Admin.Data;
using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Areas.Admin.Repository
{
    public class MessageTemplateRepository : IMessageTemplateRepository
    {
        private readonly Connection _dataSetting;
        private readonly EncryptionService _encryptionService;

        public MessageTemplateRepository(Connection dataSetting, EncryptionService encryptionService)
        {
            _dataSetting = dataSetting;
            _encryptionService = encryptionService;
        }

        public int SaveMessageTemplate(MessageTemplateViewModel model)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_SaveMessageTemplate", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Name",model.Name);
                command.Parameters.AddWithValue("@MailFrom", model.FromMail);
                command.Parameters.AddWithValue("@Body",model.Body);
                command.Parameters.AddWithValue("@Subject", model.Subject);
                command.Parameters.AddWithValue("@IsActive", model.IsActive);
                command.Parameters.AddWithValue("@UserId", model.CreatedBy);

                con.Open();
                _returnId = Convert.ToInt32(command.ExecuteScalar());
                con.Close();

                return _returnId;
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

        public List<MessageTemplateViewModel> GetAllMessageTemplate()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<MessageTemplateViewModel> messageTemplates = new List<MessageTemplateViewModel>();
                SqlCommand command = new SqlCommand("USP_Admin_GetAllMessageTemplate", con);
                command.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    MessageTemplateViewModel messageTemplate = new MessageTemplateViewModel();
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
                    messageTemplates.Add(messageTemplate);
                }
                con.Close();

                return messageTemplates;
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
        public int UpdateMessageTemplate(MessageTemplateViewModel model)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_SaveMessageTemplate", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", model.Id);
                command.Parameters.AddWithValue("@Name", model.Name);
                command.Parameters.AddWithValue("@MailFrom", model.FromMail);
                command.Parameters.AddWithValue("@Body", model.Body);
                command.Parameters.AddWithValue("@Subject", model.Subject);
                command.Parameters.AddWithValue("@IsActive", model.IsActive);
                command.Parameters.AddWithValue("@UserId", model.ModifyBy);

                con.Open();
                _returnId = Convert.ToInt32(command.ExecuteScalar());
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
        public int RemoveMessageTemplate(int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_RemoveMessageTemplate", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@id", Id);
                con.Open();
                _returnId = Convert.ToInt32(command.ExecuteScalar());
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
        public List<MailHistory> GetAllMailHistory()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<MailHistory> messageTemplates = new List<MailHistory>();
                SqlCommand command = new SqlCommand("USP_Admin_GetEmailList", con);
                command.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    MailHistory messageTemplate = new MailHistory();
                    messageTemplate.Id = Convert.ToInt32(reader["Id"]);
                    messageTemplate.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    messageTemplate.Subject = reader["Subject"].ToString();
                    messageTemplate.FromMail = reader["MailFrom"].ToString();
                    messageTemplate.Password = reader["MailFromPassword"].ToString();
                    messageTemplate.ToMail = reader["MailTo"].ToString();
                    messageTemplate.Body = reader["MailBody"].ToString();
                    messageTemplate.Company = reader["Company"].ToString();
                    messageTemplate.Username = reader["Username"].ToString();
                    messageTemplate.CreatedOn = reader["CreatedOn"].ToString();
                    messageTemplate.ModifyOn = reader["ModifyOn"].ToString();
                    messageTemplate.CreatedBy = Convert.ToInt32(reader["CreatedBy"]);
                    messageTemplates.Add(messageTemplate);
                }
                con.Close();

                return messageTemplates;
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
        public MailHistory GetMailById(int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                MailHistory messageTemplate = new MailHistory();
                SqlCommand command = new SqlCommand("USP_Admin_GetEmailList", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", Id);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    messageTemplate.Id = Convert.ToInt32(reader["Id"]);
                    messageTemplate.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    messageTemplate.Subject = reader["Subject"].ToString();
                    messageTemplate.FromMail = reader["MailFrom"].ToString();
                    messageTemplate.Password = reader["MailFromPassword"].ToString();
                    messageTemplate.ToMail = reader["MailTo"].ToString();
                    messageTemplate.Body = reader["MailBody"].ToString();
                    messageTemplate.Company = reader["Company"].ToString();
                    messageTemplate.Username = reader["Username"].ToString();
                    messageTemplate.CreatedOn = reader["CreatedOn"].ToString();
                    messageTemplate.ModifyOn = reader["ModifyOn"].ToString();
                    messageTemplate.CreatedBy = Convert.ToInt32(reader["CreatedBy"]);
                    messageTemplate.emailDocuments = GetEmailDocuments(messageTemplate.Id);
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
        [Obsolete]
        public async Task<string> ReSendCompanyMail(MailHistory model)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                var message = new MailMessage();
                //string _emails =null;
                //string query = "SELECT EmailID FROM [dbo].[Tbl_Company] WHERE Id ="+ model.CompanyId;

                //SqlCommand cmd = new SqlCommand(query, con);
                //cmd.CommandType = CommandType.Text;

                //con.Open();
                //_emails = (string)cmd.ExecuteScalar();
                //con.Close();

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
                if (model.emailDocuments != null)
                {
                    if (model.emailDocuments.Count > 0)
                    {
                        for (int i = 0; i < model.emailDocuments.Count; i++)
                        {
                            string fileName = Path.GetFileName(model.emailDocuments[i].Filename);
                            MemoryStream ms = new MemoryStream();
                            ms.Write(model.emailDocuments[i].DataBinary,0, model.emailDocuments[i].DataBinary.Length);
                            message.Attachments.Add(new Attachment(ms, fileName));
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
                        Password = _encryptionService.DecryptText(model.Password)
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                    int returnId = 0;
                    string result = "Email has been send successfully.";
                    //var password = _encryptionService.EncryptText(model.Password);
                    SqlCommand cmd = new SqlCommand("USP_Admin_SaveEmailInfo", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MailFrom", model.FromMail);
                    cmd.Parameters.AddWithValue("@Password", model.Password);
                    cmd.Parameters.AddWithValue("@MailTo", model.ToMail);
                    cmd.Parameters.AddWithValue("@Subject", model.Subject);
                    cmd.Parameters.AddWithValue("@MailBody", model.Body);
                    cmd.Parameters.AddWithValue("@CompanyId", model.CompanyId);
                    cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);
                    con.Open();
                    returnId =(int) cmd.ExecuteScalar();
                    con.Close();
                    if (model.emailDocuments != null)
                    {
                        if (model.emailDocuments.Count > 0)
                        {
                            for (int i = 0; i < model.emailDocuments.Count; i++)
                            {
                                string _getName = model.emailDocuments[i].Filename;
                                int lastIndex = model.emailDocuments[i].Filename.LastIndexOf(".");
                                String Name = model.emailDocuments[i].Filename.Substring(0, lastIndex);
                                string FileName = Name;
                                var extn = Path.GetExtension(_getName);

                                cmd = new SqlCommand("USP_Admin_SaveEmailDocuments", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@EmailId", returnId);
                                cmd.Parameters.AddWithValue("@FileName", _getName);
                                cmd.Parameters.AddWithValue("@Extension", extn);
                                cmd.Parameters.AddWithValue("@DateBinary", model.emailDocuments[i].DataBinary);
                                cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);
                                con.Open();
                                returnId = (int)cmd.ExecuteScalar();
                                con.Close();
                                cmd.Parameters.Clear();
                            }
                        }
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [Obsolete]
        public async Task<string> SendCompanyMail(MailHistory model)
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
                    //var password = _encryptionService.EncryptText(model.Password);
                    int returnId = 0;
                    
                    SqlCommand cmd = new SqlCommand("USP_Admin_SaveEmailInfo", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MailFrom", model.FromMail);
                    cmd.Parameters.AddWithValue("@Password",_encryptionService.EncryptText(model.Password));
                    cmd.Parameters.AddWithValue("@MailTo", model.ToMail);
                    cmd.Parameters.AddWithValue("@Subject", model.Subject);
                    cmd.Parameters.AddWithValue("@MailBody", model.Body);
                    cmd.Parameters.AddWithValue("@CompanyId", model.CompanyId);
                    cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);
                    con.Open();
                    returnId = (int) cmd.ExecuteScalar();
                    con.Close();

                    cmd.Parameters.Clear();

                    if (model.Documents != null)
                    {
                        if (model.Documents.Count > 0)
                        {
                            for (int i = 0; i < model.Documents.Count; i++)
                            {
                                string _getName = model.Documents[i].FileName;
                                int lastIndex = model.Documents[i].FileName.LastIndexOf(".");
                                String Name = model.Documents[i].FileName.Substring(0, lastIndex);
                                string FileName = Name;
                                var extn = Path.GetExtension(_getName);

                                Byte[] bytes = null;
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    model.Documents[i].OpenReadStream().CopyTo(ms);
                                    bytes = ms.ToArray();
                                }
                                cmd = new SqlCommand("USP_Admin_SaveEmailDocuments", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@EmailId", returnId);
                                cmd.Parameters.AddWithValue("@FileName", _getName);
                                cmd.Parameters.AddWithValue("@Extension", extn);
                                cmd.Parameters.AddWithValue("@DateBinary", bytes);
                                cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);
                                con.Open();
                                returnId = (int)cmd.ExecuteScalar();
                                con.Close();
                                cmd.Parameters.Clear();
                            }
                        }
                    }
                    
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<EmailDocument> GetEmailDocuments(int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<EmailDocument> messageTemplates = new List<EmailDocument>();
                SqlCommand command = new SqlCommand("USP_Admin_GetEmailDocuments", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@EmailId", Id);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    EmailDocument messageTemplate = new EmailDocument();
                    messageTemplate.Id = Convert.ToInt32(reader["Id"]);
                    messageTemplate.EmailId = Convert.ToInt32(reader["EmailId"]);
                    messageTemplate.DataBinary = (byte[])reader["DataBinary"];
                    messageTemplate.Filename = reader["Filename"].ToString();
                    messageTemplate.Extension= reader["Extension"].ToString();
                    messageTemplate.CreatedOn = reader["CreatedOn"].ToString();
                    messageTemplate.CreatedBy = Convert.ToInt32(reader["CreatedBy"]);
                    messageTemplates.Add(messageTemplate);
                }
                con.Close();

                return messageTemplates;
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
        public EmailDocument GetEmailDocumentsById(int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                 EmailDocument messageTemplate = new EmailDocument();
                SqlCommand command = new SqlCommand("USP_Admin_GetEmailDocuments", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", Id);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    messageTemplate.Id = Convert.ToInt32(reader["Id"]);
                    messageTemplate.EmailId = Convert.ToInt32(reader["EmailId"]);
                    messageTemplate.DataBinary = (byte[])reader["DataBinary"];
                    messageTemplate.Filename = reader["Filename"].ToString();
                    messageTemplate.Extension = reader["Extension"].ToString();
                    messageTemplate.CreatedOn = reader["CreatedOn"].ToString();
                    messageTemplate.CreatedBy = Convert.ToInt32(reader["CreatedBy"]);
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

        public int DeleteEmail(int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int messageTemplate = 1;
                SqlCommand command = new SqlCommand("USP_Admin_RemoveEmail", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@id", Id);
                con.Open();
                command.ExecuteNonQuery();
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
    }
}
