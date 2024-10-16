using DIBN.Areas.Admin.Data;
using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Areas.Admin.Repository
{
    public class SupportTicketRepository : ISupportTicketRepository
    {
        private readonly Connection _dataSetting;

        public SupportTicketRepository(Connection dataSetting)
        {
            _dataSetting = dataSetting;
        }

        public List<SupportTicketRequest> GetAllSupportTickets(int CompanyId, int? Status)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<SupportTicketRequest> tickets = new List<SupportTicketRequest>();
                List<SupportTicketRequest> supportTickets = new List<SupportTicketRequest>();
                SqlCommand sqlCommand = new SqlCommand("USP_Admin_SupportTicketOperation", con);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Status", Operation.Show);
                if (CompanyId > 0)
                {
                    sqlCommand.Parameters.AddWithValue("@CompanyId", CompanyId);
                }
                con.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    SupportTicketRequest ticket = new SupportTicketRequest();
                    ticket.ID = Convert.ToInt32(reader["ID"]);
                    ticket.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    ticket.UserId = Convert.ToInt32(reader["UserId"]);
                    ticket.CompanyName = reader["CompanyName"].ToString();
                    ticket.Username = reader["Username"].ToString();
                    ticket.TrackingNumber = reader["TrackingNumber"].ToString();
                    ticket.Title = reader["Title"].ToString();
                    ticket.Description = reader["Description"].ToString();
                    ticket.TicketStatusId = Convert.ToInt32(reader["TicketStatusId"]);
                    ticket.TicketStatus = reader["TicketStatus"].ToString();
                    ticket.ParentId = Convert.ToInt32(reader["ParentId"]);
                    ticket.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    ticket.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    ticket.CreatedOn = reader["CreatedOn"].ToString();
                    ticket.CreatedBy = Convert.ToInt32(reader["CreatedBy"]);
                    ticket.ModifyOn = reader["ModifyOn"].ToString();
                    ticket.ModifyBy = Convert.ToInt32(reader["ModifyBy"]);
                    ticket.AssignedUser = reader["AssignedUser"].ToString();
                    ticket.AssignedOn = reader["AssignedOn"].ToString();
                    supportTickets.Add(ticket);
                }
                con.Close();

                if (Status != null)
                {
                    if (Status == 1)
                    {
                        var open = from data in supportTickets
                                   where data.TicketStatus.Contains("Open")
                                   select data;

                        tickets = open.ToList();
                    }
                    else if (Status == 2)
                    {
                        var close = from data in supportTickets
                                    where data.TicketStatus.Contains("Close")
                                    select data;

                        tickets = close.ToList();
                    }
                }
                else
                {
                    tickets = supportTickets;
                }

                return tickets;
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

        public SupportTicketsWithPagination GetSupportTicketWithPagination(int? Status,int pageNumber,int rowsofPage,string searchValue,string sortBy,string sortDire)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int totalSupportTickets = 0;
                SupportTicketsWithPagination model = new SupportTicketsWithPagination();
                List<GetSupportTickets> tickets = new List<GetSupportTickets>();
                List<GetSupportTickets> supportTickets = new List<GetSupportTickets>();
                SqlCommand sqlCommand = new SqlCommand("USP_Admin_GetAllSupportTicketWithPagination", con);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@PageNumber", pageNumber);
                sqlCommand.Parameters.AddWithValue("@RowsOfPage", rowsofPage);
                sqlCommand.Parameters.AddWithValue("@searchPrefix", searchValue);
                sqlCommand.Parameters.AddWithValue("@sortColumn", sortBy);
                sqlCommand.Parameters.AddWithValue("@sortDirection", sortDire);
                con.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    GetSupportTickets ticket = new GetSupportTickets();
                    ticket.ID = Convert.ToInt32(reader["ID"]);
                    ticket.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    ticket.UserId = Convert.ToInt32(reader["UserId"]);
                    ticket.CompanyName = reader["CompanyName"].ToString();
                    ticket.Username = reader["Username"].ToString();
                    ticket.TrackingNumber = reader["TrackingNumber"].ToString();
                    ticket.Title = reader["Title"].ToString();
                    ticket.Description = reader["Description"].ToString();
                    ticket.TicketStatusId = Convert.ToInt32(reader["TicketStatusId"]);
                    ticket.TicketStatus = reader["TicketStatus"].ToString();
                    ticket.ParentId = Convert.ToInt32(reader["ParentId"]);
                    ticket.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    ticket.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    ticket.CreatedOn = reader["CreatedOn"].ToString();
                    ticket.CreatedBy = Convert.ToInt32(reader["CreatedBy"]);
                    ticket.AssignedUser = reader["AssignedUser"].ToString();
                    ticket.AssignedOn = reader["AssignedOn"].ToString();
                    supportTickets.Add(ticket);
                }

                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["TrackingNumber"] != DBNull.Value)
                        totalSupportTickets += 1;
                }
                con.Close();

                if (Status != null)
                {
                    if (Status == 1)
                    {
                        var open = from data in supportTickets
                                   where data.TicketStatus.Contains("Open")
                                   select data;

                        tickets = open.ToList().Skip(pageNumber).Take(rowsofPage).ToList();
                        totalSupportTickets = open.Count();
                    }
                    else if (Status == 2)
                    {
                        var close = from data in supportTickets
                                    where data.TicketStatus.Contains("Close")
                        select data;

                        tickets = close.ToList().Skip(pageNumber).Take(rowsofPage).ToList();
                        totalSupportTickets = close.Count();
                    }
                }
                else
                {
                    tickets = supportTickets.Skip(pageNumber).Take(rowsofPage).ToList();
                }
                model.totalSupportTickets = totalSupportTickets;
                model.supportTickets = tickets;
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
        public SupportTicketDocunents GetUploadedDocumets(int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                SupportTicketDocunents ticketDocunents = new SupportTicketDocunents();
                SqlCommand sqlCommand = new SqlCommand("USP_Admin_SupportTicketDocuments", con);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Status", Operation.GetById);
                sqlCommand.Parameters.AddWithValue("@ID", Id);
                con.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {

                    ticketDocunents.ID = Convert.ToInt32(reader["ID"]);
                    ticketDocunents.FileName = reader["FileName"].ToString();
                    ticketDocunents.Extension = reader["Extension"].ToString();
                    ticketDocunents.TicketId = reader["TicketId"].ToString();
                    ticketDocunents.DataBinary = (Byte[])reader["DataBinary"];
                }
                con.Close();
                return ticketDocunents;
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
        public List<SupportTicketRequest> GetSupportTicketDetail(string TrackingNumber)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<SupportTicketRequest> tickets = new List<SupportTicketRequest>();
                SqlCommand sqlCommand = new SqlCommand("USP_Admin_GetSupportTicketsByTrackingNumber", con);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@TrackingNumber", TrackingNumber);
                con.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    SupportTicketRequest ticket = new SupportTicketRequest();
                    ticket.ID = Convert.ToInt32(reader["ID"]);
                    ticket.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    ticket.UserId = Convert.ToInt32(reader["UserId"]);
                    ticket.CompanyName = reader["CompanyName"].ToString();
                    if (ticket.UserId == 0)
                        ticket.Username = "";
                    else
                        ticket.Username = reader["Username"].ToString();
                    ticket.TrackingNumber = reader["TrackingNumber"].ToString();
                    ticket.Title = reader["Title"].ToString();
                    ticket.Description = reader["Description"].ToString();
                    ticket.TicketStatusId = Convert.ToInt32(reader["TicketStatusId"]);
                    ticket.TicketStatus = reader["TicketStatus"].ToString();
                    ticket.NewTicketStatus = reader["NewTicketStatus"].ToString();
                    ticket.ParentId = Convert.ToInt32(reader["ParentId"]);
                    ticket.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    ticket.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    ticket.CreatedOn = reader["CreatedOn"].ToString();
                    ticket.CreatedTime = reader["CreatedTime"].ToString();
                    ticket.CreatedBy = Convert.ToInt32(reader["CreatedBy"]);
                    ticket.ModifyOn = reader["ModifyOn"].ToString();
                    ticket.ModifyBy = Convert.ToInt32(reader["ModifyBy"]);
                    ticket.SalesPersonId = Convert.ToInt32(reader["SalesPersonId"]);
                    if (ticket.SalesPersonId == 0)
                        ticket.SalesPersonName = "";
                    else
                        ticket.SalesPersonName = reader["SalesPersonName"].ToString();
                    ticket.DocumentList = GetSupportTicketUploadedDocuments(ticket.ID);
                    ticket.getResponseByParentIds = GetTicketReplyOfAnyResponse(ticket.ID);
                    tickets.Add(ticket);
                }
                con.Close();
                return tickets;
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
        public List<GetSupportTicketResponseByParentId> GetTicketReplyOfAnyResponse(int ParentId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetSupportTicketResponseByParentId> ticketResponses = new List<GetSupportTicketResponseByParentId>();
                SqlCommand sqlCommand = new SqlCommand("USP_Admin_SupportTicketOperation", con);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Status", Operation.GetById);
                sqlCommand.Parameters.AddWithValue("@ParentId", ParentId);
                con.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    GetSupportTicketResponseByParentId response = new GetSupportTicketResponseByParentId();
                    response.ID = Convert.ToInt32(reader["ID"]);
                    response.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    response.UserId = Convert.ToInt32(reader["UserId"]);
                    response.CompanyName = reader["CompanyName"].ToString();
                    if (response.UserId == 0)
                        response.Username = "";
                    else
                        response.Username = reader["Username"].ToString();
                    response.TrackingNumber = reader["TrackingNumber"].ToString();
                    response.Title = reader["Title"].ToString();
                    response.Description = reader["Description"].ToString();
                    response.TicketStatusId = Convert.ToInt32(reader["TicketStatusId"]);
                    response.TicketStatus = reader["TicketStatus"].ToString();
                    response.ParentId = Convert.ToInt32(reader["ParentId"]);
                    response.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    response.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    response.CreatedBy = Convert.ToInt32(reader["CreatedBy"]);
                    response.CreatedOn = reader["CreatedOn"].ToString();
                    response.ModifyBy = Convert.ToInt32(reader["ModifyBy"]);
                    response.ModifyOn = reader["ModifyOn"].ToString();
                    response.SalesPersonId = Convert.ToInt32(reader["SalesPersonId"]);
                    if (response.SalesPersonId == 0)
                        response.SalesPersonName = "";
                    else
                        response.SalesPersonName = reader["SalesPersonName"].ToString();
                    response.DocumentList = GetSupportTicketUploadedDocuments(response.ID);
                    ticketResponses.Add(response);
                }
                con.Close();
                return ticketResponses;
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
        public int AddNewSupportTicket(SupportTicketRequest supportTicket)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0, _returnDocumentId = 0;
                SqlCommand sqlCommand = new SqlCommand("USP_Admin_SupportTicketOperation", con);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Status", Operation.Insert);
                sqlCommand.Parameters.AddWithValue("@CompanyId", supportTicket.CompanyId);
                sqlCommand.Parameters.AddWithValue("@UserId", supportTicket.UserId);
                sqlCommand.Parameters.AddWithValue("@TrackingNumber", supportTicket.TrackingNumber);
                sqlCommand.Parameters.AddWithValue("@Title", supportTicket.Title);
                sqlCommand.Parameters.AddWithValue("@Description", supportTicket.Description);
                sqlCommand.Parameters.AddWithValue("@TicketStatus", supportTicket.TicketStatusId);
                sqlCommand.Parameters.AddWithValue("@CreatedBy", supportTicket.CreatedBy);
                sqlCommand.Parameters.AddWithValue("@ParentId", supportTicket.ParentId);

                con.Open();
                _returnId = (int)sqlCommand.ExecuteScalar();
                con.Close();
                if (supportTicket.FormFile != null && _returnId > 0)
                {
                    if (supportTicket.FormFile.Count > 0)
                    {
                        for (int index = 0; index < supportTicket.FormFile.Count; index++)
                        {
                            _returnDocumentId = UploadTicketDocuments(supportTicket.FormFile[index], supportTicket.TrackingNumber, _returnId, supportTicket.CreatedBy);
                        }
                    }
                }

                sqlCommand.Parameters.Clear();
                if (_returnId > 0)
                {
                    string createdDate = "";
                    sqlCommand = new SqlCommand("USP_Admin_GetArabianCurrentDateTime", con);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandTimeout = 600;

                    con.Open();
                    createdDate = (string)sqlCommand.ExecuteScalar();
                    con.Close();

                    sqlCommand.Parameters.Clear();

                    string trackingNumber = "";
                    sqlCommand = new SqlCommand("USP_Admin_GetTrackingNumberById", con);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@id", _returnId);
                    sqlCommand.CommandTimeout = 600;

                    con.Open();
                    trackingNumber = (string)sqlCommand.ExecuteScalar();
                    con.Close();


                    sqlCommand.Parameters.Clear();
                    sqlCommand = new SqlCommand("USP_Admin_SaveRequestNotifications", con);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandTimeout = 600;
                    sqlCommand.Parameters.AddWithValue("@Type", "Support Ticket");
                    sqlCommand.Parameters.AddWithValue("@RequestCreatedOn", createdDate);
                    if (supportTicket.TrackingNumber != null)
                        sqlCommand.Parameters.AddWithValue("@TrackingNumber", supportTicket.TrackingNumber);
                    else
                        sqlCommand.Parameters.AddWithValue("@TrackingNumber", trackingNumber);
                    sqlCommand.Parameters.AddWithValue("@SupportTicketCreatedBy", supportTicket.UserId);
                    sqlCommand.Parameters.AddWithValue("@Title", supportTicket.Title);
                    sqlCommand.Parameters.AddWithValue("@Description", supportTicket.Description);
                    sqlCommand.Parameters.AddWithValue("@CompanyId", supportTicket.CompanyId);
                    if (supportTicket.TrackingNumber != null)
                        sqlCommand.Parameters.AddWithValue("@ResponseType", "Response");
                    else
                        sqlCommand.Parameters.AddWithValue("@ResponseType", "Created");

                    con.Open();
                    int _notification = (int)sqlCommand.ExecuteScalar();
                    con.Close();
                }

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
        public int UploadTicketDocuments(IFormFile formFile, string Id, int ReplyId, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;

                string _getName = formFile.FileName;
                int lastIndex = formFile.FileName.LastIndexOf(".");
                String Name = formFile.FileName.Substring(0, lastIndex);
                string FileName = Name;
                var extn = Path.GetExtension(_getName);
                Byte[] bytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    formFile.OpenReadStream().CopyTo(ms);
                    bytes = ms.ToArray();
                }
                SqlCommand sqlCommand = new SqlCommand("USP_Admin_SupportTicketDocuments", con);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Status", Operation.Insert);
                sqlCommand.Parameters.AddWithValue("@FileName", FileName);
                sqlCommand.Parameters.AddWithValue("@Extension", extn);
                sqlCommand.Parameters.AddWithValue("@DataBinary", bytes);
                sqlCommand.Parameters.AddWithValue("@TicketId", Id);
                sqlCommand.Parameters.AddWithValue("@ReplyId", ReplyId);
                sqlCommand.Parameters.AddWithValue("@CreatedBy", UserId);
                con.Open();
                _returnId = (int)sqlCommand.ExecuteScalar();
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
        public List<SupportTicketDocunents> GetUploadedDocumetsById(int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<SupportTicketDocunents> documentList = new List<SupportTicketDocunents>();
                SqlCommand sqlCommand = new SqlCommand("USP_Admin_SupportTicketDocuments", con);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Status", Operation.Show);
                sqlCommand.Parameters.AddWithValue("@ReplyId", Id);
                con.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    SupportTicketDocunents ticketDocunents = new SupportTicketDocunents();
                    ticketDocunents.ID = Convert.ToInt32(reader["ID"]);
                    ticketDocunents.FileName = reader["FileName"].ToString();
                    ticketDocunents.Extension = reader["Extension"].ToString();
                    ticketDocunents.TicketId = reader["TicketId"].ToString();
                    ticketDocunents.ReplyId = Convert.ToInt32(reader["ReplyId"]);
                    ticketDocunents.DataBinary = (Byte[])reader["DataBinary"];
                    documentList.Add(ticketDocunents);
                }
                con.Close();
                return documentList;
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

        public List<SupportTicketDocunents> GetSupportTicketUploadedDocuments(int TicketId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<SupportTicketDocunents> documentList = new List<SupportTicketDocunents>();
                SqlCommand sqlCommand = new SqlCommand("USP_Admin_SupportTicketDocuments", con);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandTimeout = 600;
                sqlCommand.Parameters.AddWithValue("@Status", Operation.Delete);
                sqlCommand.Parameters.AddWithValue("@ReplyId", TicketId);
                con.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    SupportTicketDocunents ticketDocunents = new SupportTicketDocunents();
                    ticketDocunents.ID = Convert.ToInt32(reader["ID"]);
                    ticketDocunents.FileName = reader["FileName"].ToString();
                    ticketDocunents.Extension = reader["Extension"].ToString();
                    ticketDocunents.TicketId = reader["TicketId"].ToString();
                    documentList.Add(ticketDocunents);
                }
                con.Close();
                return documentList;
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
        public string GetTrackingNumber()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string returnId = null;
                SqlCommand command = new SqlCommand("USP_Admin_SupportTicketOperation", con);
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
        public int SaveAssignUserOfSupportTicket(SaveAssignUserOfSupportTikcet saveAssignUser)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                string _query = "", _assignedUsers = "";

                SqlCommand command = null;

                command = new SqlCommand("USP_Admin_CheckSupportTicketAssigned", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TrackingNumber", saveAssignUser.TrackingNumber);

                con.Open();
                command.ExecuteNonQuery();
                con.Close();

                command.Parameters.Clear();

                if (saveAssignUser.UserId != null)
                {
                    if (saveAssignUser.UserId.Length > 0)
                    {
                        command = new SqlCommand("USP_Admin_SaveAssignedUserOfSupportTicket", con);

                        command.CommandType = CommandType.StoredProcedure;
                        for (int index = 0; index < saveAssignUser.UserId.Length; index++)
                        {
                            command.Parameters.AddWithValue("@UserId", saveAssignUser.UserId[index]);
                            command.Parameters.AddWithValue("@TrackingNumber", saveAssignUser.TrackingNumber);
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

                        command = new SqlCommand("USP_Admin_GetTitleOfSupportTicketByTrackingNumber", con);
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 600;
                        command.Parameters.AddWithValue("@trackingNumber", saveAssignUser.TrackingNumber);
                        con.Open();
                        Title = (string)command.ExecuteScalar();
                        con.Close();

                        command.Parameters.Clear();


                        command = new SqlCommand("USP_Admin_SaveRequestNotifications", con);
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 600;
                        for (int index = 0; index < saveAssignUser.UserId.Length; index++)
                        {
                            command.Parameters.AddWithValue("@Type", "Support Ticket");
                            command.Parameters.AddWithValue("@RequestCreatedOn", createdDate);
                            command.Parameters.AddWithValue("@TrackingNumber", saveAssignUser.TrackingNumber);
                            command.Parameters.AddWithValue("@AssignedOn", createdDate);
                            command.Parameters.AddWithValue("@AssignedUserId", saveAssignUser.UserId[index]);
                            command.Parameters.AddWithValue("@AssignedBy", saveAssignUser.CreatedBy);
                            command.Parameters.AddWithValue("@SupportTicketCreatedBy", saveAssignUser.CreatedBy);
                            command.Parameters.AddWithValue("@Title", Title);
                            command.Parameters.AddWithValue("@Description", "Your request is now assigned to " + _assignedUsers + ".");
                            command.Parameters.AddWithValue("@ResponseType", "Response");
                            con.Open();
                            int _notification = (int)command.ExecuteScalar();
                            con.Close();

                            command.Parameters.Clear();
                        }


                        SupportTicketRequest model = new SupportTicketRequest();
                        command.Parameters.Clear();
                        command = new SqlCommand("USP_Admin_GetBasicSupportTicketDetails", con);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TrackingNumber", saveAssignUser.TrackingNumber);
                        con.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            if (reader["TrackingNumber"] != DBNull.Value)
                                model.TrackingNumber = reader["TrackingNumber"].ToString();
                            if (reader["SalesPersonId"] != DBNull.Value)
                                model.SalesPersonId = Convert.ToInt32(reader["SalesPersonId"]);
                            if (reader["CompanyId"] != DBNull.Value)
                                model.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                            if (reader["ParentId"] != DBNull.Value)
                                model.ParentId = Convert.ToInt32(reader["ParentId"]);
                            if (reader["Title"] != DBNull.Value)
                                model.Title = reader["Title"].ToString();
                            model.Description = "Your request is now assigned to " + _assignedUsers + ".";
                            if (reader["UserId"] != DBNull.Value)
                                model.UserId = saveAssignUser.CreatedBy;
                            if (reader["TicketStatus"] != DBNull.Value)
                                model.TicketStatusId = Convert.ToInt32(reader["TicketStatus"]);
                            if (reader["CreatedBy"] != DBNull.Value)
                                model.CreatedBy = saveAssignUser.CreatedBy;
                        }
                        con.Close();
                        Log.Information("Support Ticket (" + model.TrackingNumber + ") added is Assigned to " + _assignedUsers + ".");
                        AddNewSupportTicket(model);
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
        public List<int> GetAssignedUserOfSupportTicket(string TrackingNumber)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<int> AssignedUser = new List<int>();
                SqlCommand command = new SqlCommand("USP_Admin_GetAssignedUserOfSupportTicket", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TrackingNumber", TrackingNumber);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    AssignedUser.Add(Convert.ToInt32(reader["UserId"]));
                }
                con.Close();
                return AssignedUser;
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
        public int RemoveSupportTicketRequest(string TrackingNumber, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_DeleteSupportTicket", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TrackingNumber", TrackingNumber);
                command.Parameters.AddWithValue("@UserId", UserId);

                con.Open();
                _returnId = (int)command.ExecuteScalar();
                con.Close();
                Log.Information("Support Ticket " + TrackingNumber + " is Deleted.");
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
        public List<int> GetAllAssignedUsers(string TrackingNumber)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<int> _returnList = new List<int>();
                SqlCommand command = new SqlCommand("USP_Admin_GetAssignedUserOfSupportTicket", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TrackingNumber", TrackingNumber);

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
                con.Close();
            }
        }
        public async Task<int> GetLastStatusOfSupportTicket(string trackingNumber)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int ticketStatus = 0;
                SqlCommand command = new SqlCommand("USP_Admin_GetLastStatusOfSupportTicket", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TrackingNumber", trackingNumber);

                connection.Open();

                ticketStatus = (int) await command.ExecuteScalarAsync();

                connection.Close();

                return ticketStatus;
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
        public async Task<GetSupportsTicketsWithPaginationForAdminModel> GetSupportsTicketsWithPaginationForAdmin(int? Status,int skipRows,int rowsOfPage,string? searchPrefix,
            string? sortColumn,string? sortDirection
            )
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int totalSupportTickets = 0;
                GetSupportsTicketsWithPaginationForAdminModel model = new GetSupportsTicketsWithPaginationForAdminModel();
                List<SupportTicketListModel> supportTickets = new List<SupportTicketListModel>();
                SqlCommand command = new SqlCommand("USP_Admin_GetSupportsTicketsWithPaginationForAdmin", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PageNumber", skipRows);
                command.Parameters.AddWithValue("@RowsOfPage", rowsOfPage);
                if(searchPrefix != null && searchPrefix != "")
                    command.Parameters.AddWithValue("@searchPrefix", searchPrefix);
                if (sortColumn != null && sortColumn != "")
                    command.Parameters.AddWithValue("@sortColumn", sortColumn);
                if (sortDirection != null && sortDirection != "")
                    command.Parameters.AddWithValue("@sortDirection", sortDirection);
                if (Status != null)
                    command.Parameters.AddWithValue("@Status", Status.Value);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    SupportTicketListModel supportTicket = new SupportTicketListModel();

                    if (reader["TrackingNumber"] != DBNull.Value)
                        supportTicket.TrackingNumber = reader["TrackingNumber"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        supportTicket.CompanyName = reader["CompanyName"].ToString();
                    if (reader["UserName"] != DBNull.Value)
                        supportTicket.Username = reader["UserName"].ToString();
                    if (reader["Title"] != DBNull.Value)
                        supportTicket.Title = reader["Title"].ToString();
                    if (reader["CreatedOn"] != DBNull.Value)
                        supportTicket.CreatedOn = reader["CreatedOn"].ToString();
                    if (reader["TicketStatusId"] != DBNull.Value)
                        supportTicket.TicketStatusId = Convert.ToInt32(reader["TicketStatusId"]);
                    if (reader["TicketStatus"] != DBNull.Value)
                        supportTicket.TicketStatus = reader["TicketStatus"].ToString();
                    if (reader["AssignedUser"] != DBNull.Value)
                        supportTicket.AssignedUser = reader["AssignedUser"].ToString();
                    if (reader["AssignedOn"] != DBNull.Value)
                        supportTicket.AssignedOn = reader["AssignedOn"].ToString();

                    supportTickets.Add(supportTicket);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["TotalSupportTickets"] != DBNull.Value)
                        totalSupportTickets = Convert.ToInt32(reader["TotalSupportTickets"]);
                }

                connection.Close();

                model.supportTickets = supportTickets;
                model.totalSupportTickets = totalSupportTickets;
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
    }
}
