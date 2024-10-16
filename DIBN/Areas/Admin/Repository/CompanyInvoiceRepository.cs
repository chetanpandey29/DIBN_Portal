using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using Org.BouncyCastle.Crypto.Engines;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading.Tasks;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Areas.Admin.Repository
{
    public class CompanyInvoiceRepository : ICompanyInvoiceRepository
    {
        private readonly Connection _dataSetting;
        public CompanyInvoiceRepository(Connection dataSetting)
        {
            _dataSetting = dataSetting;
        }

        /// <summary>
        /// Save Company Invoice Details                                                                                -- Yashasvi TBC(26-11-2022)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string SaveCompanyInvoiceDetails(List<SaveInvoiceData> model)
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
                    int companyId = Convert.ToInt32(model[lastIndex].CompanyId);
                    string Currency = model[lastIndex].Currency;
                    string GrandTotal = model[lastIndex].GrandTotal;
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
                    int _count = model[lastIndex].Count;
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
                        command = new SqlCommand("USP_Admin_SaveCompanyInvoiceDeatils", connection);
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
                        command.Parameters.AddWithValue("@CompanyId", companyId);
                        command.Parameters.AddWithValue("@CreatedBy", model[index].CreatedBy);
                        command.Parameters.AddWithValue("@Service", model[index].Service);
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
        /// Get Last Invoice Number                                                                                     -- Yashasvi TBC(26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string GetLastInvoiceNumber()
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string _invoiceNumber = "";
                SqlCommand command = new SqlCommand("USP_Admin_GetCompanyInvoiceDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", 2);

                connection.Open();
                _invoiceNumber = Convert.ToString(command.ExecuteScalar());
                connection.Close();
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
        /// Get Invoice Details                                                                                         -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="InvoiceNumber"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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
                    //        else if (_previousTax == Convert.ToDecimal(getInvoiceDeatils.invoiceVatDetails[i].Vat))/* && _previousAmount == Convert.ToDecimal(getInvoiceDeatils.invoiceVatDetails[i].TotalAmount)*/
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

        /// <summary>
        /// Get All Company Invoice                                                                                     -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<GetAllCompanyInvoices> GetAllCompanyInvoice(int companyId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetAllCompanyInvoices> invoices = new List<GetAllCompanyInvoices>();
                SqlCommand command = new SqlCommand("USP_Admin_GetCompanyInvoiceDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", 3);
                command.Parameters.AddWithValue("@CompanyId", companyId);

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
        ///Get Company Invoice Details By Id                                                                            -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public CompanyInvoiceModel GetCompanyInvoiceDetailsById(int Id)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                CompanyInvoiceModel model = new CompanyInvoiceModel();
                SqlCommand command = new SqlCommand("USP_Admin_GetCompanyInvoiceDetailsById", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", Id);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["ProductName"] != DBNull.Value)
                        model.ProductName = reader["ProductName"].ToString();
                    if (reader["Quantity"] != DBNull.Value)
                        model.Quantity = Convert.ToInt32(reader["Quantity"]);
                    if (reader["Amount"] != DBNull.Value)
                        model.Amount = reader["Amount"].ToString();
                    if (reader["TotalAmount"] != DBNull.Value)
                        model.TotalAmount = reader["TotalAmount"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["CompanyId"] != DBNull.Value)
                        model.CompanyId = Convert.ToInt16(reader["CompanyId"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = reader["GrandTotal"].ToString();
                    if (reader["InvoiceNumber"] != DBNull.Value)
                        model.InvoiceNumber = reader["InvoiceNumber"].ToString();
                    if (reader["Vat"] != DBNull.Value)
                        model.Vat = reader["Vat"].ToString();
                    if (reader["VatAmount"] != DBNull.Value)
                        model.VatAmount = reader["VatAmount"].ToString();
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

        /// <summary>
        /// Get Company Invoice Details By Invoice Number                                                               -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="Invoice"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public CompanyInvoiceModel GetCompanyInvoiceDetailsByInvoice(string Invoice)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                CompanyInvoiceModel model = new CompanyInvoiceModel();
                SqlCommand command = new SqlCommand("USP_Admin_GetCompanyInvoiceDetailsByInvoice", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Invoice", Invoice);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["CompanyId"] != DBNull.Value)
                        model.CompanyId = Convert.ToInt16(reader["CompanyId"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = reader["GrandTotal"].ToString();
                    if (reader["InvoiceNumber"] != DBNull.Value)
                        model.InvoiceNumber = reader["InvoiceNumber"].ToString();
                    if (reader["Services"] != DBNull.Value)
                        model.Service = reader["Services"].ToString();
                    if (reader["InvoiceCreatedOn"] != DBNull.Value)
                        model.InvoiceDate = reader["InvoiceCreatedOn"].ToString();
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

        /// <summary>
        /// Update Company Invoice Details                                                                              -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int UpdateCompanyInvoiceDetails(CompanyInvoiceModel model)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_UpdateCompanyInvoiceDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ProductName", model.ProductName);
                command.Parameters.AddWithValue("@Quantity", model.Quantity);
                command.Parameters.AddWithValue("@Amount", Convert.ToDecimal(model.Amount));
                command.Parameters.AddWithValue("@TotalAmount", Convert.ToDecimal(model.TotalAmount));
                command.Parameters.AddWithValue("@GrandTotal", Convert.ToDecimal(model.GrandTotal));
                command.Parameters.AddWithValue("@InvoiceNumber", model.InvoiceNumber);
                command.Parameters.AddWithValue("@CompanyId", model.CompanyId);
                command.Parameters.AddWithValue("@Vat", model.Vat);
                command.Parameters.AddWithValue("@VatAmount", model.VatAmount);
                command.Parameters.AddWithValue("@Id", model.Id);
                command.Parameters.AddWithValue("@UserId", model.CreatedBy);

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
        /// Save Final Invoice                                                                                          -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="pdf"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int SaveCompanyFinalInvoice(SaveFinalPdf pdf)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_SaveCompanyInvoicePdf", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", 1);
                command.Parameters.AddWithValue("@Name", pdf.PdfName);
                command.Parameters.AddWithValue("@Extension", pdf.Extension);
                command.Parameters.AddWithValue("@DataBinary", pdf.DataBinary);
                command.Parameters.AddWithValue("@InvoiceNumber", pdf.InvoiceNumber);
                command.Parameters.AddWithValue("@InvoiceDate", pdf.InvoiceDate);
                command.Parameters.AddWithValue("@CreatedBy", pdf.CreatedBy);

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
        /// Get All Final PDF                                                                                           -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<SaveFinalPdf> GetAllFinalPdf(int companyId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<SaveFinalPdf> saveFinalPdfs = new List<SaveFinalPdf>();
                SqlCommand command = new SqlCommand("USP_Admin_SaveCompanyInvoicePdf", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", 2);
                command.Parameters.AddWithValue("@CompanyId", companyId);

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

        /// <summary>
        /// Get Final PDF By Id                                                                                         -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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

        /// <summary>
        /// Remove Proforma Invoice                                                                                     -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="InvoiceNumber"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int RemovePI(string InvoiceNumber, int UserId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_RemoveCompanyInvoice", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", 1);
                command.Parameters.AddWithValue("@UserId", UserId);
                command.Parameters.AddWithValue("@InvoiceNumber", InvoiceNumber);

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
        /// Remove Final Invoice                                                                                        -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="InvoiceNumber"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int RemoveFinalInvoice(string InvoiceNumber, int UserId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_RemoveCompanyInvoice", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", 2);
                command.Parameters.AddWithValue("@UserId", UserId);
                command.Parameters.AddWithValue("@InvoiceNumber", InvoiceNumber);

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
        /// Check Whether we already have final Invoice generated of passed Invoice Number                              -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="InvoiceNumber"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int FindFinalInvoice(string InvoiceNumber)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                
                SqlCommand command = new SqlCommand("USP_Admin_CheckWhetherFinalInvoiceExists", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@InvoiceNumber", InvoiceNumber);

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
        /// Get Task Suggestion from already Added Invoice Details                                                      -- Yashasvi TBC (26-11-2022)
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
                    SqlCommand command = new SqlCommand("GetProductsForInvoiceByPrefix", con);
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
        /// Get Service List for Invoice                                                                                -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<string> GetServicesList(string prefix)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> _returnId = new List<string>();
                if(prefix != null && prefix != "")
                {
                    SqlCommand command = new SqlCommand("USP_Admin_GetServiceSuggestionForInvoice", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@prefix", prefix);
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        _returnId.Add(reader["ServiceName"].ToString());
                    }
                    reader.Close();
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
        /// Remove Invoice Detail                                                                                       -- Yashasvi TBC (26-11-2022)
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
                SqlCommand command = new SqlCommand("USP_Admin_DeleteInvoiceDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", Id);
                command.Parameters.AddWithValue("@TotalAmount", Amount);
                command.Parameters.AddWithValue("@VatAmount", Vat);
                command.Parameters.AddWithValue("@UserId", UserId);
                command.Parameters.AddWithValue("@InvoiceNumber", InvoiceNumber);

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
        public async Task<GetProformaInvoiceWithPaginationModel> GetAllCompanyInvoiceWithPagination(int? companyId,int page,int pageSize,string searchString,string sortBy,string sortDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetProformaInvoiceWithPaginationModel model = new GetProformaInvoiceWithPaginationModel();
                int totalInvoices = 0;
                List<GetAllCompanyInvoices> invoices = new List<GetAllCompanyInvoices>();
                SqlCommand command = new SqlCommand("USP_Admin_GetAllCompanyInvoice", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 36000;
                if(companyId!=null)
                    command.Parameters.AddWithValue("@CompanyId", companyId.Value);
                command.Parameters.AddWithValue("@skipRows", page);
                command.Parameters.AddWithValue("@RowsOfPage", pageSize);
                command.Parameters.AddWithValue("@searchPrefix", searchString);
                command.Parameters.AddWithValue("@sortColumn", sortBy);
                command.Parameters.AddWithValue("@sortDirection", sortDirection);

                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAllCompanyInvoices invoice = new GetAllCompanyInvoices();
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

                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_GetAllCompanyInvoiceCount", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@searchPrefix", searchString);
                if (companyId != null)
                    command.Parameters.AddWithValue("@CompanyId", companyId.Value);

                connection.Open();
                reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["InvoiceNumber"] != DBNull.Value)
                        totalInvoices++;
                }
                connection.Close();
                model.totalProformaInvoice = totalInvoices;
                model.getAllCompanyInvoices = invoices;
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
        public async Task<GetFinalInvoiceWithPaginationModel> GetAllCompanyFinalInvoice(int? companyId,int page, int pageSize, string searchString, string sortBy, string sortDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int index = 0;
                index = page + 1;
                GetFinalInvoiceWithPaginationModel model = new GetFinalInvoiceWithPaginationModel();
                int totalInvoices = 0;
                List<SaveFinalPdf> invoices = new List<SaveFinalPdf>();
                SqlCommand command = new SqlCommand("USP_Admin_GetAllCompanyFinalInvoice", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@skipRows", page);
                command.Parameters.AddWithValue("@RowsOfPage", pageSize);
                command.Parameters.AddWithValue("@searchPrefix", searchString);
                command.Parameters.AddWithValue("@sortColumn", sortBy);
                command.Parameters.AddWithValue("@sortDirection", sortDirection);
                if(companyId != null)
                    command.Parameters.AddWithValue("@CompanyId", companyId.Value);

                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    SaveFinalPdf saveFinalPdf = new SaveFinalPdf();
                    saveFinalPdf.Id = Convert.ToInt32(reader["Id"]);
                    saveFinalPdf.PdfName = reader["Name"].ToString();
                    saveFinalPdf.Extension = reader["Extension"].ToString();
                    saveFinalPdf.InvoiceNumber = reader["InvoiceNumber"].ToString();
                    saveFinalPdf.CompanyName = reader["Company"].ToString();
                    saveFinalPdf.CreatedBy = Convert.ToInt32(reader["CreatedBy"].ToString());
                    saveFinalPdf.Username = reader["Username"].ToString();
                    saveFinalPdf.CreatedOn = reader["CreatedOn"].ToString();
                    saveFinalPdf.Index = index;
                    invoices.Add(saveFinalPdf);
                    index++;
                }
                connection.Close();

                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_GetAllCompanyFinalInvoiceCount", connection);
                command.CommandType = CommandType.StoredProcedure;
                if (companyId != null)
                    command.Parameters.AddWithValue("@CompanyId", companyId.Value);
                command.Parameters.AddWithValue("@searchPrefix", searchString);
                connection.Open();

                reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["Id"] != DBNull.Value)
                        totalInvoices++;
                }
                connection.Close();
                model.totalFinalInvoice = totalInvoices;
                
                model.getAllCompanyInvoices = invoices;
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
        public async Task<CheckWhetherInvoiceDetailsIsDeletedModel> CheckWhetherInvoiceDetailsIsDeleted(int Id)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                CheckWhetherInvoiceDetailsIsDeletedModel model = new CheckWhetherInvoiceDetailsIsDeletedModel();
                SqlCommand command = new SqlCommand("USP_Admin_CheckInvoiceDataIsDeleted", connection);
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
            catch(Exception ex) 
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<CheckWhetherInvoiceDetailsIsDeletedModel> CheckWhetherInvoiceIsDeleted(string invoiceNumber)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                CheckWhetherInvoiceDetailsIsDeletedModel model = new CheckWhetherInvoiceDetailsIsDeletedModel();
                SqlCommand command = new SqlCommand("USP_Admin_CheckIsInvoiceDeleted", connection);
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
