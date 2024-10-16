using DIBN.Areas.Admin.Data;
using DIBN.IService;
using DIBN.Models;
using DocumentFormat.OpenXml.Presentation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Service
{
    public class UserCompanyService: IUserCompanyService
    {
        private readonly Connection _dataSetting;
        public UserCompanyService(Connection dataSetting)
        {
            _dataSetting = dataSetting;
        }
        public List<UserCompanyViewModel> GetCompanies()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<UserCompanyViewModel> companies = new List<UserCompanyViewModel>();
                SqlCommand command = new SqlCommand("USP_Admin_CompanyOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Show);
                con.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    UserCompanyViewModel comp = new UserCompanyViewModel();
                    comp.Id = Convert.ToInt32(dr["Id"].ToString());
                    comp.DIBNUserNumber = dr["DIBNUserNumber"].ToString();
                    comp.AccountNumber = dr["AccountNumber"].ToString();
                    //comp.Username = dr["Username"].ToString();
                    comp.CompanyName = dr["CompanyName"].ToString();
                    comp.ShareCapital = dr["ShareCapital"].ToString();
                    comp.CompanyRegistrationNumber = dr["CompanyRegistrationNumber"].ToString();
                    comp.MobileNumber = dr["MobileNumber"].ToString();
                    comp.EmailID = dr["EmailID"].ToString();
                    comp.SecondEmailID = dr["SecondEmailID"].ToString();
                    comp.CompanyType = dr["CompanyType"].ToString();
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
                    comp.ShareholderName = dr["ShareholderName"].ToString();
                    comp.ShareholderSharePercentage = dr["ShareholderSharePercentage"].ToString();
                    comp.Country = dr["Country"].ToString();
                    comp.City = dr["City"].ToString();
                    comp.LabourFileNo = dr["LabourFileNo"].ToString();
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
        public UserCompanyViewModel GetCompanyById(int id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                UserCompanyViewModel comp = new UserCompanyViewModel();
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
                    comp.ShareCapital = dr["ShareCapital"].ToString();
                    comp.CompanyRegistrationNumber = dr["CompanyRegistrationNumber"].ToString();
                    comp.MobileNumber = dr["MobileNumber"].ToString();
                    comp.EmergencyNumber = dr["EmergencyNumber"].ToString();
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
                    comp.CompanyType= dr["CompanyType"].ToString();
                    comp.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    comp.IsDelete = Convert.ToBoolean(dr["IsDelete"].ToString());
                    comp.ShareholderIsActive = Convert.ToBoolean(dr["ShareholderIsActive"].ToString());
                    comp.CreatedOn = dr["CreatedOn"].ToString();
                    comp.LabourFileNo = dr["LabourFileNo"].ToString();
                    comp.ModifyOn = dr["ModifyOn"].ToString();
                    comp.ShareholderName = dr["ShareholderName"].ToString();
                    comp.ShareholderSharePercentage = dr["ShareholderSharePercentage"].ToString();
                }
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
        public List<UserCompanyViewModel> GetComapnyBySalesPersonId(int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<UserCompanyViewModel> userCompanies = new List<UserCompanyViewModel>();
                SqlCommand command = new SqlCommand("USP_Admin_GetCompanyDetailsBySalesPerson", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@SalesPersonId", Id);
                con.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    UserCompanyViewModel comp = new UserCompanyViewModel();
                    comp.Id = Convert.ToInt32(dr["Id"].ToString());
                    comp.DIBNUserNumber = dr["DIBNUserNumber"].ToString();
                    comp.AccountNumber = dr["AccountNumber"].ToString();
                    comp.CompanyName = dr["CompanyName"].ToString();
                    comp.ShareCapital = dr["ShareCapital"].ToString();
                    comp.CompanyRegistrationNumber = dr["CompanyRegistrationNumber"].ToString();
                    comp.MobileNumber = dr["MobileNumber"].ToString();
                    comp.EmergencyNumber = dr["EmergencyNumber"].ToString();
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
                    comp.CompanyType = dr["CompanyType"].ToString();
                    comp.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    comp.IsDelete = Convert.ToBoolean(dr["IsDelete"].ToString());
                    comp.ShareholderIsActive = Convert.ToBoolean(dr["ShareholderIsActive"].ToString());
                    comp.CreatedOn = dr["CreatedOn"].ToString();
                    comp.ModifyOn = dr["ModifyOn"].ToString();
                    comp.ShareholderName = dr["ShareholderName"].ToString();
                    comp.ShareholderSharePercentage = dr["ShareholderSharePercentage"].ToString();
                    userCompanies.Add(comp);
                }
                con.Close();
                return userCompanies;
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

        public List<UserCompanyViewModel> GetCompanyByRMTeamId(int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<UserCompanyViewModel> userCompanies = new List<UserCompanyViewModel>();
                SqlCommand command = new SqlCommand("USP_Admin_GetCompanyDetailsByRMTeam", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@RMTeamId", Id);
                con.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    UserCompanyViewModel comp = new UserCompanyViewModel();
                    comp.Id = Convert.ToInt32(dr["Id"].ToString());
                    comp.DIBNUserNumber = dr["DIBNUserNumber"].ToString();
                    comp.AccountNumber = dr["AccountNumber"].ToString();
                    comp.CompanyName = dr["CompanyName"].ToString();
                    comp.ShareCapital = dr["ShareCapital"].ToString();
                    comp.CompanyRegistrationNumber = dr["CompanyRegistrationNumber"].ToString();
                    comp.MobileNumber = dr["MobileNumber"].ToString();
                    comp.EmergencyNumber = dr["EmergencyNumber"].ToString();
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
                    comp.CompanyType = dr["CompanyType"].ToString();
                    comp.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    comp.IsDelete = Convert.ToBoolean(dr["IsDelete"].ToString());
                    comp.ShareholderIsActive = Convert.ToBoolean(dr["ShareholderIsActive"].ToString());
                    comp.CreatedOn = dr["CreatedOn"].ToString();
                    comp.ModifyOn = dr["ModifyOn"].ToString();
                    comp.ShareholderName = dr["ShareholderName"].ToString();
                    comp.ShareholderSharePercentage = dr["ShareholderSharePercentage"].ToString();
                    userCompanies.Add(comp);
                }
                con.Close();
                return userCompanies;
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
        public List<GetAllCompanyInvoices> GetAllCompanyInvoice(int CompanyId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetAllCompanyInvoices> invoices = new List<GetAllCompanyInvoices>();
                SqlCommand command = new SqlCommand("USP_Admin_GetCompanyInvoiceDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", 3);
                command.Parameters.AddWithValue("@CompanyId", CompanyId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetAllCompanyInvoices invoice = new GetAllCompanyInvoices();
                    invoice.CompanyName = reader["CompanyName"].ToString();
                    invoice.InvoiceNumber = reader["InvoiceNumber"].ToString();
                    invoice.Date = reader["Date"].ToString();
                    invoice.CreatedBy = Convert.ToInt32(reader["CreatedBy"].ToString());
                    invoice.Username = reader["Username"].ToString();
                    invoice.TotalAmount = reader["GrandTotal"].ToString();
                    invoice.Currency = reader["Currency"].ToString();
                    invoice.Service = reader["Service"].ToString();
                    invoices.Add(invoice);
                }
                connection.Close();
                return invoices;
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

        public async Task<GetCompanyProformaInvoiceByCompanyIdWithPaginationModel> GetCompanyProformaInvoiceByCompanyIdWithPagination(int CompanyId,int SkipRows,int RowsOfPage,string searchBy,string searchPrefix,string sortBy,string sortingDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int totalInvoices = 0;
                GetCompanyProformaInvoiceByCompanyIdWithPaginationModel model = new GetCompanyProformaInvoiceByCompanyIdWithPaginationModel();
                List<GetAllCompanyInvoices> invoices = new List<GetAllCompanyInvoices>();

                SqlCommand command = new SqlCommand("USP_Admin_GetCompanyInvoiceDetailByCompanyIdWithPagination",connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@skipRows", SkipRows);
                command.Parameters.AddWithValue("@rowsOfPage", RowsOfPage);
                if (searchPrefix != null && searchPrefix != "")
                    command.Parameters.AddWithValue("@searchPrefix", searchPrefix);
                if (sortBy != null && sortBy != "")
                    command.Parameters.AddWithValue("@sortBy", sortBy);
                if (sortingDirection != null && sortingDirection != "")
                    command.Parameters.AddWithValue("@sortDirection", sortingDirection);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAllCompanyInvoices invoice = new GetAllCompanyInvoices();
                    if (reader["InvoiceNumber"] != DBNull.Value)
                        invoice.InvoiceNumber = reader["InvoiceNumber"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        invoice.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Service"] != DBNull.Value)
                        invoice.Service = reader["Service"].ToString();
                    if (reader["Date"] != DBNull.Value)
                        invoice.Date = reader["Date"].ToString();
                    if (reader["GrandTotal"] != DBNull.Value)
                        invoice.TotalAmount = reader["GrandTotal"].ToString();
                    if (reader["Currency"] != DBNull.Value)
                        invoice.Currency = reader["Currency"].ToString();
                    if (reader["CreatedBy"] != DBNull.Value)
                        invoice.CreatedBy = Convert.ToInt32(reader["CreatedBy"]);
                    if (reader["Username"] != DBNull.Value)
                        invoice.Username = reader["Username"].ToString();
                    invoices.Add(invoice);
                }
                connection.Close();

                command.Parameters.Clear();

                command = new SqlCommand("USP_Admin_GetCompanyInvoiceDetailByCompanyIdWithPaginationCount", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                if (searchPrefix != null && searchPrefix != "")
                    command.Parameters.AddWithValue("@searchPrefix", searchPrefix);

                connection.Open();
                reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["InvoiceNumber"] != DBNull.Value)
                        totalInvoices++;
                }
                connection.Close();

                model.totalInvoices = totalInvoices;
                model.invoices = invoices;
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
        
        public async Task<GetCompanyFinalInvoiceByCompanyIdWithPaginationModel> GetCompanyFinalInvoiceByCompanyIdWithPagination(int CompanyId, int SkipRows, int RowsOfPage, string searchBy, string searchPrefix, string sortBy, string sortingDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetCompanyFinalInvoiceByCompanyIdWithPaginationModel model = new GetCompanyFinalInvoiceByCompanyIdWithPaginationModel();
                List<SaveFinalPdf> invoices = new List<SaveFinalPdf>();
                int totalInvoices = 0;  
                SqlCommand command = new SqlCommand("USP_Admin_GetAllFinalInvoicesByCompanyIdWithPagination", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@skipRows", SkipRows);
                command.Parameters.AddWithValue("@rowsOfPage", RowsOfPage);
                if (searchPrefix != null && searchPrefix != "")
                    command.Parameters.AddWithValue("@searchPrefix", searchPrefix);
                if (sortBy != null && sortBy != "")
                    command.Parameters.AddWithValue("@sortBy", sortBy);
                if (sortingDirection != null && sortingDirection != "")
                    command.Parameters.AddWithValue("@sortDirection", sortingDirection);

                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    SaveFinalPdf invoice = new SaveFinalPdf();
                    if (reader["Id"] != DBNull.Value)
                        invoice.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["Name"] != DBNull.Value)
                        invoice.PdfName = reader["Name"].ToString();
                    if (reader["Extension"] != DBNull.Value)
                        invoice.Extension = reader["Extension"].ToString();
                    if (reader["InvoiceNumber"] != DBNull.Value)
                        invoice.InvoiceNumber = reader["InvoiceNumber"].ToString();
                    if (reader["Company"] != DBNull.Value)
                        invoice.CompanyName = reader["Company"].ToString();
                    if (reader["CreatedOn"] != DBNull.Value)
                        invoice.CreatedOn = reader["CreatedOn"].ToString();
                    if (reader["CreatedBy"] != DBNull.Value)
                        invoice.CreatedBy = Convert.ToInt32(reader["CreatedBy"]);
                    if (reader["Username"] != DBNull.Value)
                        invoice.Username = reader["Username"].ToString();
                    invoices.Add(invoice);
                }
                connection.Close();

                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_GetAllFinalInvoicesByCompanyIdWithPaginationCount", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                if (searchPrefix != null && searchPrefix != "")
                    command.Parameters.AddWithValue("@searchPrefix", searchPrefix);

                connection.Open();
                reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["Id"] != DBNull.Value)
                        totalInvoices++;
                }
                connection.Close();

                model.totalInvoices = totalInvoices;
                model.invoices = invoices;
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
        public List<SaveFinalPdf> GetAllFinalPdf(int CompanyId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<SaveFinalPdf> saveFinalPdfs = new List<SaveFinalPdf>();
                SqlCommand command = new SqlCommand("USP_Admin_SaveCompanyInvoicePdf", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", 2);
                command.Parameters.AddWithValue("@CompanyId", CompanyId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    SaveFinalPdf saveFinalPdf = new SaveFinalPdf();
                    saveFinalPdf.Id = Convert.ToInt32(reader["Id"]);
                    saveFinalPdf.PdfName = reader["Name"].ToString();
                    saveFinalPdf.Extension = reader["Extension"].ToString();
                    saveFinalPdf.DataBinary = (byte[])reader["DataBinary"];
                    saveFinalPdf.InvoiceNumber = reader["InvoiceNumber"].ToString();
                    saveFinalPdf.CompanyName = reader["Company"].ToString();
                    saveFinalPdf.CreatedBy = Convert.ToInt32(reader["CreatedBy"].ToString());
                    saveFinalPdf.Username = reader["Username"].ToString();
                    saveFinalPdfs.Add(saveFinalPdf);
                }
                connection.Close();

                return saveFinalPdfs;
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
        public SaveFinalPdf GetFinalPdfById(int Id)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                SaveFinalPdf saveFinalPdf = new SaveFinalPdf();
                SqlCommand command = new SqlCommand("USP_Admin_SaveCompanyInvoicePdf", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", 2);
                command.Parameters.AddWithValue("@Id", Id);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    saveFinalPdf.Id = Convert.ToInt32(reader["Id"]);
                    saveFinalPdf.PdfName = reader["Name"].ToString();
                    saveFinalPdf.Extension = reader["Extension"].ToString();
                    saveFinalPdf.DataBinary = (byte[])reader["DataBinary"];
                    saveFinalPdf.InvoiceNumber = reader["InvoiceNumber"].ToString();
                    saveFinalPdf.CompanyName = reader["Company"].ToString();
                    saveFinalPdf.CreatedBy = Convert.ToInt32(reader["CreatedBy"].ToString());
                    saveFinalPdf.Username = reader["Username"].ToString();
                }
                connection.Close();

                return saveFinalPdf;
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
        public GetInvoiceDeatils GetInvoiceDeatils(string InvoiceNumber)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetInvoiceDeatils getInvoiceDeatils = new GetInvoiceDeatils();
                List<CompanyInvoiceModel> details = new List<CompanyInvoiceModel>();
                List<InvoiceVatDetails> vatDetails = new List<InvoiceVatDetails>();
                SqlCommand command = new SqlCommand("USP_Admin_GetCompanyInvoiceDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", 1);
                command.Parameters.AddWithValue("@InvoiceNumber", InvoiceNumber);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["CompanyName"] != DBNull.Value)
                        getInvoiceDeatils.CompanyName = reader["CompanyName"].ToString();
                    if (reader["TRNCreationDate"] != DBNull.Value)
                        getInvoiceDeatils.TRNCreationDate = reader["TRNCreationDate"].ToString();
                    if (reader["Currency"] != DBNull.Value)
                        getInvoiceDeatils.Currency = reader["Currency"].ToString();
                    if (reader["CreatedOn"] != DBNull.Value)
                        getInvoiceDeatils.InvoiceCreatedOn = reader["CreatedOn"].ToString();
                    if (reader["IsTRN"] != DBNull.Value)
                        getInvoiceDeatils.IsTRN = Convert.ToBoolean(reader["IsTRN"]);
                    if (reader["TRN"] != DBNull.Value)
                        getInvoiceDeatils.TRN = reader["TRN"].ToString();
                    if (reader["Country"] != DBNull.Value)
                        getInvoiceDeatils.Country = reader["Country"].ToString();
                    if (reader["UnitLocation"] != DBNull.Value)
                        getInvoiceDeatils.Location = reader["UnitLocation"].ToString();
                    if (reader["Service"] != DBNull.Value)
                        getInvoiceDeatils.Service = reader["Service"].ToString();
                    if (reader["Date"] != DBNull.Value)
                        getInvoiceDeatils.Date = reader["Date"].ToString();
                }
                reader.NextResult();
                while (reader.Read())
                {
                    CompanyInvoiceModel model = new CompanyInvoiceModel();
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"].ToString());
                    if (reader["ProductName"] != DBNull.Value)
                    {
                        model.ProductName = reader["ProductName"].ToString();
                        model.ProductName = model.ProductName.Replace("\t", "");
                        model.ProductName = model.ProductName.Replace("\n", "");
                        model.ProductName = model.ProductName.Replace("\r", "");
                    }
                    if (reader["Quantity"] != DBNull.Value)
                        model.Quantity = Convert.ToInt32(reader["Quantity"]);
                    if (reader["Amount"] != DBNull.Value)
                        model.Amount = reader["Amount"].ToString();
                    if (reader["TotalAmount"] != DBNull.Value)
                        model.TotalAmount = reader["TotalAmount"].ToString();
                    if (reader["Vat"] != DBNull.Value)
                        model.Vat = reader["Vat"].ToString();
                    if (reader["VatAmount"] != DBNull.Value)
                        model.VatAmount = reader["VatAmount"].ToString();
                    if (reader["CreatedBy"] != DBNull.Value)
                        model.CreatedBy = Convert.ToInt32(reader["CreatedBy"].ToString());
                    if (reader["Username"] != DBNull.Value)
                        model.Username = reader["Username"].ToString();
                    model.InvoiceNumber = InvoiceNumber;

                    details.Add(model);
                }
                getInvoiceDeatils.invoiceModels = details;
                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["GrandTotal"] != DBNull.Value)
                        getInvoiceDeatils.GrandTotal = reader["GrandTotal"].ToString();
                    if (reader["VatAmount"] != DBNull.Value)
                        getInvoiceDeatils.VatAmount = reader["VatAmount"].ToString();
                    if (reader["Vat"] != DBNull.Value)
                        getInvoiceDeatils.Vat = reader["Vat"].ToString();
                }
                reader.NextResult();
                while (reader.Read())
                {
                    InvoiceVatDetails invoiceVat = new InvoiceVatDetails();
                    if (reader["Id"] != DBNull.Value)
                        invoiceVat.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TotalAmount"] != DBNull.Value)
                        invoiceVat.TotalAmount = reader["TotalAmount"].ToString();
                    if (reader["VatAmount"] != DBNull.Value)
                        invoiceVat.VatAmount = reader["VatAmount"].ToString();
                    if (reader["Vat"] != DBNull.Value)
                        invoiceVat.Vat = reader["Vat"].ToString();
                    vatDetails.Add(invoiceVat);
                }
                getInvoiceDeatils.invoiceVatDetails = vatDetails;
                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["InvoiceNumber"] != DBNull.Value)
                        getInvoiceDeatils.InvoiceNumber = reader["InvoiceNumber"].ToString();
                }
                reader.Close();
                connection.Close();

                decimal _taxableAmount = 0, _vatAmount = 0;
                if (getInvoiceDeatils.invoiceVatDetails != null)
                {
                    if (getInvoiceDeatils.invoiceVatDetails.Count > 1)
                    {
                        for (int i = 0; i < getInvoiceDeatils.invoiceVatDetails.Count; i++)
                        {
                            _taxableAmount = _taxableAmount + Convert.ToDecimal(getInvoiceDeatils.invoiceVatDetails[i].TotalAmount);
                            _vatAmount = _vatAmount + Convert.ToDecimal(getInvoiceDeatils.invoiceVatDetails[i].VatAmount);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < getInvoiceDeatils.invoiceVatDetails.Count; i++)
                        {
                            _taxableAmount = _taxableAmount + Convert.ToDecimal(getInvoiceDeatils.invoiceVatDetails[i].TotalAmount);
                            _vatAmount = _vatAmount + Convert.ToDecimal(getInvoiceDeatils.invoiceVatDetails[i].VatAmount);
                        }
                    }
                    getInvoiceDeatils.TotalTaxableAmount = _taxableAmount.ToString();
                    getInvoiceDeatils.VatAmount = _vatAmount.ToString();
                }

                decimal _previousTax = 0; decimal _previousAmount = 0; decimal _previousVatAmount = 0;
                if (getInvoiceDeatils.invoiceVatDetails != null)
                {
                    //if (getInvoiceDeatils.invoiceVatDetails.Count > 0)
                    //{
                    //    for (int i = 0; i < getInvoiceDeatils.invoiceVatDetails.Count; i++)
                    //    {
                    //        if (_previousTax == 0 && _previousAmount == 0)
                    //        {
                    //            _previousTax = Convert.ToDecimal(getInvoiceDeatils.invoiceVatDetails[i].Vat);
                    //            _previousAmount = Convert.ToDecimal(getInvoiceDeatils.invoiceVatDetails[i].TotalAmount);
                    //        }
                    //        else if (_previousTax == Convert.ToDecimal(getInvoiceDeatils.invoiceVatDetails[i].Vat) && _previousAmount == Convert.ToDecimal(getInvoiceDeatils.invoiceVatDetails[i].TotalAmount))
                    //        {
                    //            getInvoiceDeatils.invoiceVatDetails.RemoveAt(i);
                    //        }
                    //    }
                    //}

                    List<InvoiceVatDetails> vatDetails1 = new List<InvoiceVatDetails>();
                    _previousTax = 0; _previousAmount = 0;
                    List<Decimal> totalTax = new List<decimal>();
                    if (getInvoiceDeatils.invoiceVatDetails != null)
                    {
                        int _length = vatDetails.Count;
                        if (getInvoiceDeatils.invoiceVatDetails.Count > 0)
                        {
                            for (int i = 0; i < _length; i++)
                            {
                                if (_previousTax == 0 && _previousAmount == 0)
                                {
                                    _previousTax = Convert.ToDecimal(vatDetails[i].Vat);
                                    totalTax.Add(_previousTax);
                                    List<InvoiceVatDetails> item = new List<InvoiceVatDetails>();
                                    item = getInvoiceDeatils.invoiceVatDetails.FindAll(x => Convert.ToDecimal(x.Vat) == _previousTax);
                                    for (int j = 0; j < item.Count; j++)
                                    {
                                        _previousAmount = _previousAmount + Convert.ToDecimal(item[j].TotalAmount);
                                        _previousVatAmount = _previousVatAmount + Convert.ToDecimal(item[j].VatAmount);
                                    }
                                    vatDetails1.Add(new InvoiceVatDetails
                                    {
                                        Id = vatDetails[i].Id,
                                        TotalAmount = _previousAmount.ToString(),
                                        Vat = _previousTax.ToString(),
                                        VatAmount = _previousVatAmount.ToString()
                                    });
                                }
                                else if (Convert.ToDecimal(vatDetails[i].Vat) != _previousTax && !totalTax.Contains(Convert.ToDecimal(vatDetails[i].Vat)))   /*&& _previousAmount == Convert.ToDecimal(getInvoiceDeatils.invoiceVatDetails[i].TotalAmount)*/
                                {
                                    _previousAmount = 0; _previousVatAmount = 0;
                                    _previousTax = Convert.ToDecimal(vatDetails[i].Vat);
                                    totalTax.Add(_previousTax);
                                    List<InvoiceVatDetails> item = new List<InvoiceVatDetails>();
                                    item = getInvoiceDeatils.invoiceVatDetails.FindAll(x => Convert.ToDecimal(x.Vat) == _previousTax);
                                    for (int j = 0; j < item.Count; j++)
                                    {
                                        _previousAmount = _previousAmount + Convert.ToDecimal(item[j].TotalAmount);
                                        _previousVatAmount = _previousVatAmount + Convert.ToDecimal(item[j].VatAmount);
                                    }
                                    vatDetails1.Add(new InvoiceVatDetails
                                    {
                                        Id = vatDetails[i].Id,
                                        TotalAmount = _previousAmount.ToString(),
                                        Vat = _previousTax.ToString(),
                                        VatAmount = _previousVatAmount.ToString()
                                    });
                                }
                            }
                            getInvoiceDeatils.invoiceVatDetails = vatDetails1;
                        }
                        getInvoiceDeatils.TotalTaxableAmount = _taxableAmount.ToString();
                        getInvoiceDeatils.VatAmount = _vatAmount.ToString();
                    }


                    getInvoiceDeatils.TotalTaxableAmount = _taxableAmount.ToString();
                    getInvoiceDeatils.VatAmount = _vatAmount.ToString();
                }

                return getInvoiceDeatils;
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
        public List<CompanyDocumentTypeModel> GetCompanyDocuments()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<CompanyDocumentTypeModel> models = new List<CompanyDocumentTypeModel>();
                SqlCommand command = new SqlCommand("USP_Admin_CompayDocumentType", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Show);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    CompanyDocumentTypeModel documentTypeModel = new CompanyDocumentTypeModel();
                    documentTypeModel.ID = Convert.ToInt32(reader["ID"].ToString());
                    documentTypeModel.DocumentName = reader["Name"].ToString();
                    documentTypeModel.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    documentTypeModel.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    documentTypeModel.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    documentTypeModel.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    models.Add(documentTypeModel);
                }
                con.Close();
                return models;
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
