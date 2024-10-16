using DIBN.Areas.Admin.Data;
using DIBN.IService;
using DIBN.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Service
{
    public class SupportTicketService : ISupportTicketService
    {
        private readonly Connection _dataSetting;

        public SupportTicketService(Connection dataSetting)
        {
            _dataSetting = dataSetting;
        }

        public List<SupportTicketViewModel> GetAllSupportTickets(int CompanyId, int _UserId, string Role, int? Status)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                SqlCommand sqlCommand = null;
                List<SupportTicketViewModel> tickets = new List<SupportTicketViewModel>();
                List<SupportTicketViewModel> supportTickets = new List<SupportTicketViewModel>();
                if (Role.StartsWith("Sales"))
                {
                    sqlCommand = new SqlCommand("USP_Admin_GetAllSupportTicketsBySalesPerson", con);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandTimeout = 36000;
                    sqlCommand.Parameters.AddWithValue("@SalesPersonId", _UserId);
                }
                else
                {
                    if (CompanyId == 1 && _UserId > 0)
                    {
                        sqlCommand = new SqlCommand("USP_Admin_GetAllAssignedSupportTicket", con);
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.CommandTimeout = 36000;
                        sqlCommand.Parameters.AddWithValue("@UserId", _UserId);
                    }
                    else
                    {
                        sqlCommand = new SqlCommand("USP_Cmp_GetAllSupportTicket", con);
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.CommandTimeout = 36000;
                        sqlCommand.Parameters.AddWithValue("@CompanyId", CompanyId);
                    }
                }
                con.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    SupportTicketViewModel ticket = new SupportTicketViewModel();
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
                    //ticket.DocumentList = GetUploadedDocumetsById(ticket.ID);
                    //ticket.getResponseByParentIds = GetTicketReplyOfAnyResponse(ticket.CompanyId, ticket.ParentId);
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

        public GetAllAssignedSupportTicketWithPagination GetAllAssignedSupportTicket(int _UserId, int? Status, int pageNumber, int rowOfpage, string searchValue, string sortBy, string sortingDir)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetAllAssignedSupportTicketWithPagination model = new GetAllAssignedSupportTicketWithPagination();
                SqlCommand sqlCommand = null;
                int totalSupportTicket = 0;
                List<AssignedSupportTickets> tickets = new List<AssignedSupportTickets>();
                List<AssignedSupportTickets> supportTickets = new List<AssignedSupportTickets>();

                sqlCommand = new SqlCommand("USP_Admin_GetAssignedSupportTicketWithPagination", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandTimeout = 36000;
                sqlCommand.Parameters.AddWithValue("@UserId", _UserId);
                sqlCommand.Parameters.AddWithValue("@PageNumber", pageNumber);
                sqlCommand.Parameters.AddWithValue("@RowsOfPage", rowOfpage);
                sqlCommand.Parameters.AddWithValue("@searchPrefix", searchValue);
                sqlCommand.Parameters.AddWithValue("@sortColumn", sortBy);
                sqlCommand.Parameters.AddWithValue("@sortDirection", sortingDir);

                connection.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    AssignedSupportTickets ticket = new AssignedSupportTickets();
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
                    supportTickets.Add(ticket);
                }

                reader.NextResult();

                while (reader.Read())
                {
                    if (reader["TrackingNumber"] != DBNull.Value)
                        totalSupportTicket += 1;
                }

                connection.Close();
                if (Status != null)
                {
                    if (Status == 1)
                    {
                        var open = from data in supportTickets
                                   where data.TicketStatus.Contains("Open")
                                   select data;

                        tickets = open.ToList().Skip(pageNumber).Take(rowOfpage).ToList();

                        totalSupportTicket = open.Count();
                    }
                    else if (Status == 2)
                    {
                        var close = from data in supportTickets
                                    where data.TicketStatus.Contains("Close")
                                    select data;

                        tickets = close.ToList().Skip(pageNumber).Take(rowOfpage).ToList();

                        totalSupportTicket = close.Count();
                    }
                }
                else
                {
                    tickets = supportTickets.Skip(pageNumber).Take(rowOfpage).ToList();
                }

                model.totalSupportTicket = totalSupportTicket;
                model.supportTickets = tickets;
                return model;
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

        public GetAllAssignedSupportTicketWithPagination GetCompanyWiseSupportTicket(int CompanyId, int? Status, int pageNumber, int rowOfpage, string searchValue, string sortBy, string sortingDir)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetAllAssignedSupportTicketWithPagination model = new GetAllAssignedSupportTicketWithPagination();
                SqlCommand sqlCommand = null;
                int totalSupportTicket = 0;
                List<AssignedSupportTickets> tickets = new List<AssignedSupportTickets>();
                List<AssignedSupportTickets> supportTickets = new List<AssignedSupportTickets>();

                sqlCommand = new SqlCommand("USP_Admin_GetCompanySupportTicketWithPagination", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandTimeout = 36000;
                sqlCommand.Parameters.AddWithValue("@CompanyId", CompanyId);
                sqlCommand.Parameters.AddWithValue("@PageNumber", pageNumber);
                sqlCommand.Parameters.AddWithValue("@RowsOfPage", rowOfpage);
                sqlCommand.Parameters.AddWithValue("@searchPrefix", searchValue);
                sqlCommand.Parameters.AddWithValue("@sortColumn", sortBy);
                sqlCommand.Parameters.AddWithValue("@sortDirection", sortingDir);

                connection.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    AssignedSupportTickets ticket = new AssignedSupportTickets();
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
                    supportTickets.Add(ticket);
                }

                reader.NextResult();

                while (reader.Read())
                {
                    if (reader["TrackingNumber"] != DBNull.Value)
                        totalSupportTicket += 1;
                }

                connection.Close();
                if (Status != null)
                {
                    if (Status == 1)
                    {
                        var open = from data in supportTickets
                                   where data.TicketStatus.Contains("Open")
                                   select data;

                        tickets = open.ToList().Skip(pageNumber).Take(rowOfpage).ToList();

                        totalSupportTicket = open.Count();
                    }
                    else if (Status == 2)
                    {
                        var close = from data in supportTickets
                                    where data.TicketStatus.Contains("Close")
                                    select data;

                        tickets = close.ToList().Skip(pageNumber).Take(rowOfpage).ToList();

                        totalSupportTicket = close.Count();
                    }
                }
                else
                {
                    tickets = supportTickets.Skip(pageNumber).Take(rowOfpage).ToList();
                }

                model.totalSupportTicket = totalSupportTicket;
                model.supportTickets = tickets;
                return model;
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
        public GetAllAssignedSupportTicketWithPagination GetCompanySupportTicketBySalesPerson(int SalesPerson, int? Status, int pageNumber, int rowOfpage, string searchValue, string sortBy, string sortingDir)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetAllAssignedSupportTicketWithPagination model = new GetAllAssignedSupportTicketWithPagination();
                SqlCommand sqlCommand = null;
                int totalSupportTicket = 0;
                List<AssignedSupportTickets> tickets = new List<AssignedSupportTickets>();
                List<AssignedSupportTickets> supportTickets = new List<AssignedSupportTickets>();

                sqlCommand = new SqlCommand("USP_Admin_GetCompanySupportTicketWithPaginationBySalesPerson", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandTimeout = 36000;
                sqlCommand.Parameters.AddWithValue("@SalesPerson", SalesPerson);
                sqlCommand.Parameters.AddWithValue("@PageNumber", pageNumber);
                sqlCommand.Parameters.AddWithValue("@RowsOfPage", rowOfpage);
                sqlCommand.Parameters.AddWithValue("@searchPrefix", searchValue);
                sqlCommand.Parameters.AddWithValue("@sortColumn", sortBy);
                sqlCommand.Parameters.AddWithValue("@sortDirection", sortingDir);

                connection.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    AssignedSupportTickets ticket = new AssignedSupportTickets();
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
                    supportTickets.Add(ticket);
                }

                reader.NextResult();

                while (reader.Read())
                {
                    if (reader["TrackingNumber"] != DBNull.Value)
                        totalSupportTicket += 1;
                }

                connection.Close();
                if (Status != null)
                {
                    if (Status == 1)
                    {
                        var open = from data in supportTickets
                                   where data.TicketStatus.Contains("Open")
                                   select data;

                        tickets = open.ToList().Skip(pageNumber).Take(rowOfpage).ToList();

                        totalSupportTicket = open.Count();
                    }
                    else if (Status == 2)
                    {
                        var close = from data in supportTickets
                                    where data.TicketStatus.Contains("Close")
                                    select data;

                        tickets = close.ToList().Skip(pageNumber).Take(rowOfpage).ToList();

                        totalSupportTicket = close.Count();
                    }
                }
                else
                {
                    tickets = supportTickets.Skip(pageNumber).Take(rowOfpage).ToList();
                }

                model.totalSupportTicket = totalSupportTicket;
                model.supportTickets = tickets;
                return model;
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
        public List<SupportTicketViewModel> GetSupportTicketDetail(string TrackingNumber)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<SupportTicketViewModel> tickets = new List<SupportTicketViewModel>();
                SqlCommand sqlCommand = new SqlCommand("USP_Admin_GetSupportTicketsByTrackingNumber", con);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandTimeout = 600;
                sqlCommand.Parameters.AddWithValue("@TrackingNumber", TrackingNumber);
                con.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    SupportTicketViewModel ticket = new SupportTicketViewModel();
                    ticket.ID = Convert.ToInt32(reader["ID"]);
                    ticket.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    if (ticket.UserId == 0)
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
                    ticket.getResponseByParentIds = GetTicketReplyOfAnyResponse(ticket.CompanyId, ticket.ID);
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
        public List<GetSupportTicketResponseByParentId> GetTicketReplyOfAnyResponse(int CompanyId, int ParentId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetSupportTicketResponseByParentId> ticketResponses = new List<GetSupportTicketResponseByParentId>();
                SqlCommand sqlCommand = new SqlCommand("USP_Admin_SupportTicketOperation", con);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandTimeout = 600;
                sqlCommand.Parameters.AddWithValue("@Status", Operation.GetById);
                sqlCommand.Parameters.AddWithValue("@CompanyId", CompanyId);
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
        public int AddNewSupportTicket(SupportTicketViewModel supportTicket)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0, _returnDocumentId = 0;
                SqlCommand sqlCommand = new SqlCommand("USP_Admin_SupportTicketOperation", con);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandTimeout = 600;
                sqlCommand.Parameters.AddWithValue("@Status", Operation.Insert);
                sqlCommand.Parameters.AddWithValue("@CompanyId", supportTicket.CompanyId);
                sqlCommand.Parameters.AddWithValue("@UserId", supportTicket.UserId);
                sqlCommand.Parameters.AddWithValue("@Title", supportTicket.Title);
                sqlCommand.Parameters.AddWithValue("@Description", supportTicket.Description);
                sqlCommand.Parameters.AddWithValue("@TicketStatus", supportTicket.TicketStatusId);
                if (supportTicket.TrackingNumber != null)
                    sqlCommand.Parameters.AddWithValue("@TrackingNumber", supportTicket.TrackingNumber);
                sqlCommand.Parameters.AddWithValue("@ParentId", supportTicket.ParentId);
                sqlCommand.Parameters.AddWithValue("@CreatedBy", supportTicket.CreatedBy);
                sqlCommand.Parameters.AddWithValue("@SalesPersonId", supportTicket.SalesPersonId);

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
                    sqlCommand.CommandTimeout = 600;
                    sqlCommand.Parameters.AddWithValue("@id", _returnId);

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
                    sqlCommand.Parameters.AddWithValue("@SupportTicketCreatedBySales", supportTicket.SalesPersonId);
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
                sqlCommand.CommandTimeout = 600;
                sqlCommand.Parameters.AddWithValue("@Status", Operation.Insert);
                sqlCommand.Parameters.AddWithValue("@FileName", FileName);
                sqlCommand.Parameters.AddWithValue("@Extension", extn);
                sqlCommand.Parameters.AddWithValue("@DataBinary", bytes);
                sqlCommand.Parameters.AddWithValue("@TicketId", Id);
                sqlCommand.Parameters.AddWithValue("@CreatedBy", UserId);
                sqlCommand.Parameters.AddWithValue("@ReplyId", ReplyId);
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
        public List<SupportTicketDocunents> GetUploadedDocumetsById(int TicketId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<SupportTicketDocunents> documentList = new List<SupportTicketDocunents>();
                SqlCommand sqlCommand = new SqlCommand("USP_Admin_SupportTicketDocuments", con);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandTimeout = 600;
                sqlCommand.Parameters.AddWithValue("@Status", Operation.Show);
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
        public SupportTicketDocunents GetUploadedDocumets(int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                SupportTicketDocunents ticketDocunents = new SupportTicketDocunents();
                SqlCommand sqlCommand = new SqlCommand("USP_Admin_SupportTicketDocuments", con);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandTimeout = 600;
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
        public string GetTrackingNumber()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string returnId = null;
                SqlCommand command = new SqlCommand("USP_Admin_SupportTicketOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
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

                ticketStatus = (int)await command.ExecuteScalarAsync();

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
        public async Task<GetAssignedSupportTicketsByUserWithPaginationModel> GetAssignedSupportTicketsByUserWithPagination(
            int userId, int? Status, int skipRows, int rowsOfPage, string? searchPrefix,string? sortColumn, string? sortDirection
        )
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int totalSupportTickets = 0;
                GetAssignedSupportTicketsByUserWithPaginationModel model = new GetAssignedSupportTicketsByUserWithPaginationModel();
                List<GetAssignedSupportTicketListByUserModel> supportTickets = new List<GetAssignedSupportTicketListByUserModel>();

                SqlCommand command = new SqlCommand("USP_Admin_GetAssignedUserSupportTicketWithPagination", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PageNumber", skipRows);
                command.Parameters.AddWithValue("@RowsOfPage", rowsOfPage);
                if(searchPrefix != null && searchPrefix !="")
                    command.Parameters.AddWithValue("@searchPrefix", searchPrefix);
                if (sortColumn != null && sortColumn != "")
                    command.Parameters.AddWithValue("@sortColumn", sortColumn);
                if (sortDirection != null && sortDirection != "")
                    command.Parameters.AddWithValue("@sortDirection", sortDirection);
                if (Status != null)
                    command.Parameters.AddWithValue("@Status", Status.Value);
                command.Parameters.AddWithValue("@UserId", userId);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAssignedSupportTicketListByUserModel supportTicket = new GetAssignedSupportTicketListByUserModel();

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
                    if (reader["totalSupportTickets"] != DBNull.Value)
                        totalSupportTickets = Convert.ToInt32(reader["totalSupportTickets"]);
                }

                connection.Close();
                
                model.totalSupportTickets = totalSupportTickets;
                model.supportTickets = supportTickets;
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

        public async Task<GetSupportTicketsBySalesPersonWithPaginationModel> GetSupportTicketsBySalesPersonWithPagination(
            int salesPersonId, int? Status, int skipRows, int rowsOfPage, string? searchPrefix, string? sortColumn, string? sortDirection
        )
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int totalSupportTickets = 0;
                GetSupportTicketsBySalesPersonWithPaginationModel model = new GetSupportTicketsBySalesPersonWithPaginationModel();
                List<GetSupportTicketListBySalesPersonModel> supportTickets = new List<GetSupportTicketListBySalesPersonModel>();

                SqlCommand command = new SqlCommand("USP_Admin_GetCompanySupportTicketWithPaginationBySalesPersonId", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PageNumber", skipRows);
                command.Parameters.AddWithValue("@RowsOfPage", rowsOfPage);
                if (searchPrefix != null && searchPrefix != "")
                    command.Parameters.AddWithValue("@searchPrefix", searchPrefix);
                if (sortColumn != null && sortColumn != "")
                    command.Parameters.AddWithValue("@sortColumn", sortColumn);
                if (sortDirection != null && sortDirection != "")
                    command.Parameters.AddWithValue("@sortDirection", sortDirection);
                if (Status != null)
                    command.Parameters.AddWithValue("@Status", Status.Value);
                command.Parameters.AddWithValue("@SalesPersonId", salesPersonId);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetSupportTicketListBySalesPersonModel supportTicket = new GetSupportTicketListBySalesPersonModel();

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

                model.totalSupportTickets = totalSupportTickets;
                model.supportTickets = supportTickets;
                return model;
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

        public async Task<GetSupportTicketsByCompanyIdWithPaginationModel> GetSupportTicketsByCompanyIdWithPagination(
            int companyId, int? Status, int skipRows, int rowsOfPage, string? searchPrefix, string? sortColumn, string? sortDirection
        )
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int totalSupportTickets = 0;
                GetSupportTicketsByCompanyIdWithPaginationModel model = new GetSupportTicketsByCompanyIdWithPaginationModel();
                List<GetSupportTicketListByCompanyIdModel> supportTickets = new List<GetSupportTicketListByCompanyIdModel>();

                SqlCommand command = new SqlCommand("USP_Admin_GetCompanyWiseSupportTicketWithPaginationByCompanyId", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PageNumber", skipRows);
                command.Parameters.AddWithValue("@RowsOfPage", rowsOfPage);
                if (searchPrefix != null && searchPrefix != "")
                    command.Parameters.AddWithValue("@searchPrefix", searchPrefix);
                if (sortColumn != null && sortColumn != "")
                    command.Parameters.AddWithValue("@sortColumn", sortColumn);
                if (sortDirection != null && sortDirection != "")
                    command.Parameters.AddWithValue("@sortDirection", sortDirection);
                if (Status != null)
                    command.Parameters.AddWithValue("@Status", Status.Value);
                command.Parameters.AddWithValue("@CompanyId", companyId);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetSupportTicketListByCompanyIdModel supportTicket = new GetSupportTicketListByCompanyIdModel();

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

                model.totalSupportTickets = totalSupportTickets;
                model.supportTickets = supportTickets;
                return model;
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
