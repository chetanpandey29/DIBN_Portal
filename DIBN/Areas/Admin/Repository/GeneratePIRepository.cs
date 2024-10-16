using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Areas.Admin.Repository
{
    public class GeneratePIRepository : IGeneratePIRepository
    {
        private readonly Connection _dataSetting;

        public GeneratePIRepository(Connection dataSetting)
        {
            _dataSetting = dataSetting;
        }

        /// <summary>
        /// Save New Company Invoice Details                                                                                    -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string SaveCompanyInvoiceDetails(List<SaveGeneratePIInvoiceData> model)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string _returnId = null;
                SqlCommand command = null;
                string _invoiceNumber = null;
                if (model.Count > 0 && model != null)
                {
                    int lastIndex = (model.Count) - 1;
                    string Currency = model[lastIndex].Currency;
                    string GrandTotal = model[lastIndex].GrandTotal;
                    int _count = model[lastIndex].Count;
                    decimal grandTotal = 0, currentGrandTotal = 0;
                    for (int index = 0; index < model.Count; index++)
                    {
                        if (model[index].VatAmount != 0)
                        {
                            grandTotal = grandTotal + Convert.ToDecimal(model[index].TotalAmount);
                            grandTotal = grandTotal + Convert.ToDecimal(model[index].VatAmount);
                        }
                        else
                        {
                            grandTotal = grandTotal + Convert.ToDecimal(model[index].TotalAmount);
                        }
                    }
                    if (model.Count != 1)
                    {
                        model.RemoveAt(lastIndex);
                    }
                    for (int index = 0; index < model.Count; index++)
                    {
                        if (model[index].VatAmount != 0)
                        {
                            currentGrandTotal = currentGrandTotal + Convert.ToDecimal(model[index].TotalAmount);
                            currentGrandTotal = currentGrandTotal + Convert.ToDecimal(model[index].VatAmount);
                        }
                        else
                        {
                            currentGrandTotal = currentGrandTotal + Convert.ToDecimal(model[index].TotalAmount);
                        }
                        command = new SqlCommand("USP_Admin_SaveNewCompanyInvoiceDetails", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ProductName", model[index].Product);
                        command.Parameters.AddWithValue("@Quantity", model[index].Quantity);
                        command.Parameters.AddWithValue("@Amount", model[index].Amount);
                        command.Parameters.AddWithValue("@TotalAmount", model[index].TotalAmount);
                        if (grandTotal != Convert.ToDecimal(GrandTotal))
                        {
                            grandTotal = Convert.ToDecimal(GrandTotal);
                            command.Parameters.AddWithValue("@GrandTotal", grandTotal);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@GrandTotal", currentGrandTotal);
                        }
                        if (_invoiceNumber != null)
                            command.Parameters.AddWithValue("@InvoiceNumber", _invoiceNumber);
                        if (model[index].InvoiceNumber != null)
                            command.Parameters.AddWithValue("@InvoiceNumber", model[index].InvoiceNumber);
                        command.Parameters.AddWithValue("@InvoiceDate", model[index].InvoiceDate);
                        command.Parameters.AddWithValue("@Vat", model[index].Vat);
                        command.Parameters.AddWithValue("@VatAmount", model[index].VatAmount);
                        command.Parameters.AddWithValue("@CreatedBy", model[index].CreatedBy);
                        command.Parameters.AddWithValue("@Service", model[index].Service);
                        command.Parameters.AddWithValue("@CompanyName", model[index].CompanyName);
                        command.Parameters.AddWithValue("@CompanyAddress", model[index].CompanyAddress);
                        command.Parameters.AddWithValue("@CompanyCity", model[index].CompanyCity);
                        command.Parameters.AddWithValue("@CompanyCountry", model[index].CompanyCountry);
                        command.Parameters.AddWithValue("@ContactNumber", model[index].ContactNumber);
                        command.Parameters.AddWithValue("@Currency", Currency);

                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            if (_invoiceNumber == null)
                            {
                                _invoiceNumber = reader["InvoiceNumber"].ToString();
                            }
                        }
                        connection.Close();
                        command.Parameters.Clear();
                    }
                }


                return _invoiceNumber;
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

        /// <summary>
        /// Get All Invoices                                                                                                    -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<GetAllNewCompanyInvoices> GetAllCompanyInvoice()
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetAllNewCompanyInvoices> invoices = new List<GetAllNewCompanyInvoices>();
                SqlCommand command = new SqlCommand("USP_Admin_GetNewCompanyInvoiceDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", 3);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetAllNewCompanyInvoices invoice = new GetAllNewCompanyInvoices();
                    invoice.CompanyName = reader["CompanyName"].ToString();
                    invoice.InvoiceNumber = reader["InvoiceNumber"].ToString();
                    invoice.Date = reader["Date"].ToString();
                    invoice.CreatedBy = Convert.ToInt32(reader["CreatedBy"].ToString());
                    invoice.Username = reader["Username"].ToString();
                    invoice.TotalAmount = reader["GrandTotal"].ToString();
                    invoice.Service = reader["Service"].ToString();
                    invoice.Currency = reader["Currency"].ToString();
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

        /// <summary>
        /// Get Invoice Details                                                                                                 -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="InvoiceNumber"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public GetNewCompanyInvoiceDeatils GetInvoiceDeatils(string InvoiceNumber)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetNewCompanyInvoiceDeatils getInvoiceDeatils = new GetNewCompanyInvoiceDeatils();
                List<NewCompanyInvoiceModel> details = new List<NewCompanyInvoiceModel>();
                List<NewCompanyInvoiceVatDetails> vatDetails = new List<NewCompanyInvoiceVatDetails>();
                SqlCommand command = new SqlCommand("USP_Admin_GetNewCompanyInvoiceDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", 1);
                command.Parameters.AddWithValue("@InvoiceNumber", InvoiceNumber);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["CompanyName"] != DBNull.Value)
                        getInvoiceDeatils.CompanyName = reader["CompanyName"].ToString();
                    if (reader["CreatedOn"] != DBNull.Value)
                        getInvoiceDeatils.InvoiceCreatedOn = reader["CreatedOn"].ToString();
                    if (reader["Currency"] != DBNull.Value)
                        getInvoiceDeatils.Currency = reader["Currency"].ToString();
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
                    NewCompanyInvoiceModel model = new NewCompanyInvoiceModel();
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
                    NewCompanyInvoiceVatDetails invoiceVat = new NewCompanyInvoiceVatDetails();
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

                    List<NewCompanyInvoiceVatDetails> vatDetails1 = new List<NewCompanyInvoiceVatDetails>();
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
                                    List<NewCompanyInvoiceVatDetails> item = new List<NewCompanyInvoiceVatDetails>();
                                    item = getInvoiceDeatils.invoiceVatDetails.FindAll(x => Convert.ToDecimal(x.Vat) == _previousTax);
                                    for (int j = 0; j < item.Count; j++)
                                    {
                                        _previousAmount = _previousAmount + Convert.ToDecimal(item[j].TotalAmount);
                                        _previousVatAmount = _previousVatAmount + Convert.ToDecimal(item[j].VatAmount);
                                    }
                                    vatDetails1.Add(new NewCompanyInvoiceVatDetails
                                    {
                                        Id = vatDetails[i].Id,
                                        TotalAmount = _previousAmount.ToString(),
                                        Vat = _previousTax.ToString(),
                                        VatAmount = _previousVatAmount.ToString()
                                    });
                                }
                                else if (Convert.ToDecimal(vatDetails[i].Vat) != _previousTax && !totalTax.Contains(Convert.ToDecimal(vatDetails[i].Vat)))  /*&& _previousAmount == Convert.ToDecimal(getInvoiceDeatils.invoiceVatDetails[i].TotalAmount)*/
                                {

                                    _previousAmount = 0; _previousVatAmount = 0;
                                    _previousTax = Convert.ToDecimal(vatDetails[i].Vat);
                                    totalTax.Add(_previousTax);
                                    List<NewCompanyInvoiceVatDetails> item = new List<NewCompanyInvoiceVatDetails>();
                                    item = getInvoiceDeatils.invoiceVatDetails.FindAll(x => Convert.ToDecimal(x.Vat) == _previousTax);
                                    for (int j = 0; j < item.Count; j++)
                                    {
                                        _previousAmount = _previousAmount + Convert.ToDecimal(item[j].TotalAmount);
                                        _previousVatAmount = _previousVatAmount + Convert.ToDecimal(item[j].VatAmount);
                                    }
                                    vatDetails1.Add(new NewCompanyInvoiceVatDetails
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

        /// <summary>
        /// Get Task Suggestions                                                                                                -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<string> GetTaskList(string prefix)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> _returnId = new List<string>();
                if(prefix != null && prefix != "")
                {
                    SqlCommand command = new SqlCommand("GetProductsForPIInvoiceByPrefix", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@prefix", prefix);
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        _returnId.Add(reader["ProductName"].ToString());
                    }
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
        /// Delete Proforma Invoice                                                                                             -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="InvoiceNumber"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int DeleteNewCompanyPI(string InvoiceNumber, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_GetNewCompanyInvoiceDetails", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", 4);
                command.Parameters.AddWithValue("@UserId", UserId);
                command.Parameters.AddWithValue("@InvoiceNumber", InvoiceNumber);
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
        /// Get Invoice Details By Id                                                                                           -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public NewCompanyInvoiceModel GetNewInvoiceDetailsById(int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                NewCompanyInvoiceModel model = new NewCompanyInvoiceModel();
                SqlCommand command = new SqlCommand("USP_Admin_GetNewCompanyInvoiceDetailsById", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", Id);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["ProductName"] != DBNull.Value)
                        model.ProductName = Convert.ToString(reader["ProductName"]);
                    if (reader["Quantity"] != DBNull.Value)
                        model.Quantity = Convert.ToInt32(reader["Quantity"]);
                    if (reader["Amount"] != DBNull.Value)
                        model.Amount = reader["Amount"].ToString();
                    if (reader["TotalAmount"] != DBNull.Value)
                        model.TotalAmount = reader["TotalAmount"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Vat"] != DBNull.Value)
                        model.Vat = reader["Vat"].ToString();
                    if (reader["VatAmount"] != DBNull.Value)
                        model.VatAmount = reader["VatAmount"].ToString();
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = reader["GrandTotal"].ToString();
                    if (reader["InvoiceNumber"] != DBNull.Value)
                        model.InvoiceNumber = reader["InvoiceNumber"].ToString();
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

        /// <summary>
        /// Update Company Invoice Details                                                                                      -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int UpdateCompanyInvoiceDetails(NewCompanyInvoiceModel model)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_UpdateNewCompanyInvoiceDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ProductName", model.ProductName);
                command.Parameters.AddWithValue("@Quantity", model.Quantity);
                command.Parameters.AddWithValue("@Amount", Convert.ToDecimal(model.Amount));
                command.Parameters.AddWithValue("@TotalAmount", Convert.ToDecimal(model.TotalAmount));
                command.Parameters.AddWithValue("@GrandTotal", Convert.ToDecimal(model.GrandTotal));
                command.Parameters.AddWithValue("@InvoiceNumber", model.InvoiceNumber);
                command.Parameters.AddWithValue("@Vat", model.Vat);
                command.Parameters.AddWithValue("@VatAmount", model.VatAmount);
                command.Parameters.AddWithValue("@UserId", model.UserId);
                command.Parameters.AddWithValue("@Id", model.Id);

                connection.Open();
                _returnId = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
                return _returnId;
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

        /// <summary>
        /// Remove Invoice Details                                                                                              -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="InvoiceNumber"></param>
        /// <param name="Amount"></param>
        /// <param name="Vat"></param>
        /// <param name="Id"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int RemoveInvoiceDetails(string InvoiceNumber, string Amount, string Vat, int Id, int UserId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 1;
                SqlCommand command = new SqlCommand("USP_Admin_GetNewCompanyInvoiceDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", Id);
                command.Parameters.AddWithValue("@TotalAmount", Amount);
                command.Parameters.AddWithValue("@VatAmount", Vat);
                command.Parameters.AddWithValue("@InvoiceNumber", InvoiceNumber);
                command.Parameters.AddWithValue("@Status", 5);
                command.Parameters.AddWithValue("@UserId", UserId);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                return _returnId;
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

        /// <summary>
        /// Get Invoice Details By Invoice Number                                                                               -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="Invoice"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public NewCompanyInvoiceModel GetCompanyInvoiceDetailsByInvoice(string Invoice)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                NewCompanyInvoiceModel model = new NewCompanyInvoiceModel();
                SqlCommand command = new SqlCommand("USP_Admin_GetNewCompanyInvoiceDetailsByInvoice", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Invoice", Invoice);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = reader["GrandTotal"].ToString();
                    if (reader["InvoiceNumber"] != DBNull.Value)
                        model.InvoiceNumber = reader["InvoiceNumber"].ToString();
                    if (reader["Services"] != DBNull.Value)
                        model.Service = reader["Services"].ToString();
                    if (reader["InvoiceCreatedOn"] != DBNull.Value)
                        model.InvoiceDate = reader["InvoiceCreatedOn"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["CompanyAddress"] != DBNull.Value)
                        model.CompanyAddress = reader["CompanyAddress"].ToString();
                    if (reader["CompanyCity"] != DBNull.Value)
                        model.CompanyCity = reader["CompanyCity"].ToString();
                    if (reader["CompanyCountry"] != DBNull.Value)
                        model.CompanyCountry = reader["CompanyCountry"].ToString();
                    if (reader["CompanyContactNumber"] != DBNull.Value)
                        model.ContactNumber = reader["CompanyContactNumber"].ToString();
                }
                connection.Close();

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
        public GetNewCompanyInvoicesWithPagination GetAllCompanyInvoiceWithPagination(int page, int pageSize, string searchString, string sortBy, string sortDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetNewCompanyInvoicesWithPagination model = new GetNewCompanyInvoicesWithPagination();
                int totalInvoices = 0;
                List<GetAllNewCompanyInvoices> invoices = new List<GetAllNewCompanyInvoices>();
                SqlCommand command = new SqlCommand("USP_Admin_GetAllNewCompanyInvoice", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PageNumber", page);
                command.Parameters.AddWithValue("@RowsOfPage", pageSize);
                command.Parameters.AddWithValue("@searchPrefix", searchString);
                command.Parameters.AddWithValue("@sortColumn", sortBy);
                command.Parameters.AddWithValue("@sortDirection", sortDirection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetAllNewCompanyInvoices invoice = new GetAllNewCompanyInvoices();
                    invoice.CompanyName = reader["CompanyName"].ToString();
                    invoice.InvoiceNumber = reader["InvoiceNumber"].ToString();
                    invoice.Date = reader["Date"].ToString();
                    invoice.CreatedBy = Convert.ToInt32(reader["CreatedBy"].ToString());
                    invoice.Username = reader["Username"].ToString();
                    invoice.TotalAmount = reader["GrandTotal"].ToString();
                    invoice.Service = reader["Service"].ToString();
                    invoice.Currency = reader["Currency"].ToString();
                    invoices.Add(invoice);
                }

                reader.NextResult();

                while (reader.Read())
                {
                    if (reader["InvoiceNumber"] != DBNull.Value)
                        totalInvoices += 1;
                }
                connection.Close();
                model.totalInvoices = totalInvoices;
                model.getAllNewCompanyInvoices = invoices;
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
        public async Task<CheckWhetherNewInvoiceDetailsIsDeletedModel> CheckWhetherInvoiceDetailsIsDeleted(int Id)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                CheckWhetherNewInvoiceDetailsIsDeletedModel model = new CheckWhetherNewInvoiceDetailsIsDeletedModel();
                SqlCommand command = new SqlCommand("USP_Admin_CheckNewPIInvoiceDataIsDeleted", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", Id);

                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    if (reader["IsDelete"] != DBNull.Value)
                        model.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    if (reader["ModifyBy"] != DBNull.Value)
                        model.ModifyBy = reader["ModifyBy"].ToString();
                    if (reader["ModifyOnDate"] != DBNull.Value)
                        model.ModifyOnDate = reader["ModifyOnDate"].ToString();
                    if (reader["ModifyOnTime"] != DBNull.Value)
                        model.ModifyOnTime = reader["ModifyOnTime"].ToString();
                }
                connection.Close();

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

        public async Task<CheckWhetherNewInvoiceDetailsIsDeletedModel> CheckWhetherInvoiceIsDeleted(string invoiceNumber)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                CheckWhetherNewInvoiceDetailsIsDeletedModel model = new CheckWhetherNewInvoiceDetailsIsDeletedModel();
                SqlCommand command = new SqlCommand("USP_Admin_CheckIsNewCompanyInvoiceDeleted", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@InvoiceNumber", invoiceNumber);

                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    if (reader["IsDelete"] != DBNull.Value && Convert.ToInt16(reader["IsDelete"]) != -1)
                        model.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    if (reader["ModifyBy"] != DBNull.Value && Convert.ToString(reader["ModifyBy"]) != "-1")
                        model.ModifyBy = reader["ModifyBy"].ToString();
                    if (reader["ModifyOnDate"] != DBNull.Value && Convert.ToString(reader["ModifyOnDate"]) != "-1")
                        model.ModifyOnDate = reader["ModifyOnDate"].ToString();
                    if (reader["ModifyOnTime"] != DBNull.Value && Convert.ToString(reader["ModifyOnTime"]) != "-1")
                        model.ModifyOnTime = reader["ModifyOnTime"].ToString();
                }
                connection.Close();

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
