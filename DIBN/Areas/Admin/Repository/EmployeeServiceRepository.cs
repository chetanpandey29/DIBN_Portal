using DIBN.Areas.Admin.Data;
using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Areas.Admin.Repository
{
    public class EmployeeServiceRepository : IEmployeeServiceRepository
    {
        private readonly Connection _dataSetting;
        public EmployeeServiceRepository(Connection dataSetting)
        {
            _dataSetting = dataSetting;
        }

        /// <summary>
        /// Get All Employee Service List                                                                                       -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<EmployeeServicesModel> GetAllEmployeeService()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<EmployeeServicesModel> employeeServices = new List<EmployeeServicesModel>();
                SqlCommand command = new SqlCommand("USP_Admin_EmployeeServiceOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Show);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    EmployeeServicesModel service = new EmployeeServicesModel();

                    service.ID = Convert.ToInt32(reader["ID"]);
                    service.ParentId = Convert.ToInt32(reader["ParentId"]);
                    service.ServiceName = reader["ServiceName"].ToString();
                    service.CompanyTypeName = reader["CompanyType"].ToString();
                    service.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    service.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    service.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    service.ModifyOnUtc = reader["ModifyOnUtc"].ToString();

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

        /// <summary>
        /// Get All Parent Employee Service List                                                                                -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<EmployeeServicesModel> GetAllParentEmployeeService()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<EmployeeServicesModel> employeeServices = new List<EmployeeServicesModel>();
                SqlCommand command = new SqlCommand("USP_Admin_GetAllParentEmployeeServiceByCompanyId", con);
                command.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    EmployeeServicesModel service = new EmployeeServicesModel();

                    service.ID = Convert.ToInt32(reader["ID"]);
                    service.ParentId = Convert.ToInt32(reader["ParentId"]);
                    service.ServiceName = reader["ServiceName"].ToString();
                    service.CompanyTypeName = reader["CompanyType"].ToString();
                    service.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    service.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    service.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    service.ModifyOnUtc = reader["ModifyOnUtc"].ToString();

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

        /// <summary>
        /// Get Employee Service Details By Id                                                                                  -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public EmployeeServicesModel GetEmployeeServiceById(int ID)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                EmployeeServicesModel service = new EmployeeServicesModel();
                SqlCommand command = new SqlCommand("USP_Admin_EmployeeServiceOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.GetById);
                command.Parameters.AddWithValue("@ID", ID);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    service.ID = Convert.ToInt32(reader["ID"]);
                    service.ParentId = Convert.ToInt32(reader["ParentId"]);
                    service.ServiceName = reader["ServiceName"].ToString();
                    service.CompanyTypeName = reader["CompanyType"].ToString();
                    service.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    service.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    service.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    service.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                }
                con.Close();
                return service;
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
        /// Create New Employee Service                                                                                         -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int CreateNew(EmployeeServicesModel service)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_EmployeeServiceOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Insert);
                command.Parameters.AddWithValue("@ServiceName", service.ServiceName);
                command.Parameters.AddWithValue("@CompanyType", service.CompanyTypeName);
                command.Parameters.AddWithValue("@ParentId", service.ParentId);
                command.Parameters.AddWithValue("@IsActive", service.IsActive);
                command.Parameters.AddWithValue("@UserId", service.UserId);
                con.Open();
                returnId = (int)command.ExecuteScalar();
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

        /// <summary>
        /// Update Employee Service Details                                                                                     -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int UpdateEmployeeService(EmployeeServicesModel service)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_EmployeeServiceOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Insert);
                command.Parameters.AddWithValue("@ID", service.ID);
                command.Parameters.AddWithValue("@ServiceName", service.ServiceName);
                command.Parameters.AddWithValue("@CompanyType", service.CompanyTypeName);
                command.Parameters.AddWithValue("@ParentId", service.ParentId);
                command.Parameters.AddWithValue("@IsActive", service.IsActive);
                command.Parameters.AddWithValue("@UserId", service.UserId);
                con.Open();
                returnId = (int)command.ExecuteScalar();
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

        /// <summary>
        /// Delete Employee Service                                                                                             -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int DeleteEmployeeService(int ID, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_EmployeeServiceOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Delete);
                command.Parameters.AddWithValue("@ID", ID);
                command.Parameters.AddWithValue("@UserId", UserId);
                con.Open();
                returnId = (int)command.ExecuteScalar();
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

        /// <summary>
        /// Get Serial Number                                                                                                 -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string GetSerialNumber()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string returnId = null;
                SqlCommand command = new SqlCommand("USP_Admin_EmployeeServiceOperation", con);
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

        /// <summary>
        /// Get Open Service Request Count                                                                                    -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetOpenServiceCount()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _countOfService = 0;
                SqlCommand command = new SqlCommand("USP_Admin_GetOpenServiceCountForAdmin", con);
                command.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["CompanyServices"] != DBNull.Value)
                        _countOfService = Convert.ToInt32(reader["CompanyServices"]);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["EmployeeServices"] != DBNull.Value)
                        _countOfService += Convert.ToInt32(reader["EmployeeServices"]);
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

        /// <summary>
        /// Get Close Service Request Count                                                                                     -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetCloseServiceCount()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _countOfService = 0;
                
                SqlCommand command = new SqlCommand("USP_Admin_GetCloseServiceCountForAdmin", con);
                command.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["CompanyServices"] != DBNull.Value)
                        _countOfService = Convert.ToInt32(reader["CompanyServices"]);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["EmployeeServices"] != DBNull.Value)
                        _countOfService += Convert.ToInt32(reader["EmployeeServices"]);
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

        /// <summary>
        /// Get Rejected Service Request Count                                                                                  -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetRejectedServiceCount()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _countOfService = 0;
                
                SqlCommand command = new SqlCommand("USP_Admin_GetRejectedServiceCountForAdmin", con);
                command.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["CompanyServices"] != DBNull.Value)
                        _countOfService = Convert.ToInt32(reader["CompanyServices"]);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["EmployeeServices"] != DBNull.Value)
                        _countOfService += Convert.ToInt32(reader["EmployeeServices"]);
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

        /// <summary>
        /// Get Open Support Ticket Count                                                                                       -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetOpenSupportTicketCount()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _countOfService = 0;
                SqlCommand command = new SqlCommand("USP_Admin_GetOpenSupportTicketCountForAdmin", con);
                command.CommandType = CommandType.StoredProcedure;
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

        /// <summary>
        /// Get Close Support Ticket Count                                                                                      -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetCloseSupportTicketCount()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _countOfService = 0;
                SqlCommand command = new SqlCommand("USP_Admin_GetCloseSupportTicketCountForAdmin", con);
                command.CommandType = CommandType.StoredProcedure;
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

        /// <summary>
        /// Get Employee Details For Yearly Chart                                                                                -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<GetEmployeeDetailsforChart> GetEmployeeDetailsForChartsYearly()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetEmployeeDetailsforChart> getEmployeeDetailsforCharts = new List<GetEmployeeDetailsforChart>();
                SqlCommand command = new SqlCommand("USP_GetEmployeesCountForCharting", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", 1);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetEmployeeDetailsforChart getEmployeeDetailsforChart = new GetEmployeeDetailsforChart();
                    if (reader["CountOfUsers"] != DBNull.Value)
                        getEmployeeDetailsforChart.CountOfUsers = Convert.ToInt32(reader["CountOfUsers"]);
                    if (reader["UserYear"] != DBNull.Value)
                        getEmployeeDetailsforChart.UserYear = Convert.ToInt32(reader["UserYear"]);
                    if (reader["CountOfSalesPersons"] != DBNull.Value)
                        getEmployeeDetailsforChart.CountOfSalesPersons = Convert.ToInt32(reader["CountOfSalesPersons"]);
                    if (reader["SalesPersonYear"] != DBNull.Value)
                    {
                        if (getEmployeeDetailsforCharts.Count > 0)
                        {
                            for (int i = 0; i < getEmployeeDetailsforCharts.Count; i++)
                            {
                                if (getEmployeeDetailsforCharts[i].UserYear != Convert.ToInt32(reader["SalesPersonYear"]))
                                {
                                    getEmployeeDetailsforChart.SalesPersonYear = Convert.ToInt32(reader["SalesPersonYear"]);
                                }
                            }
                        }
                        else if (getEmployeeDetailsforChart.UserYear != Convert.ToInt32(reader["SalesPersonYear"]))
                        {
                            getEmployeeDetailsforChart.SalesPersonYear = Convert.ToInt32(reader["SalesPersonYear"]);
                        }
                    }

                    getEmployeeDetailsforCharts.Add(getEmployeeDetailsforChart);
                }
                con.Close();
                return getEmployeeDetailsforCharts;
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
        /// Get Employee Details For Monthly Chart                                                                              -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<GetEmployeeDetailsforChart> GetEmployeeDetailsForChartsMonthly()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetEmployeeDetailsforChart> getEmployeeDetailsforCharts = new List<GetEmployeeDetailsforChart>();
                SqlCommand command = new SqlCommand("USP_GetEmployeesCountForCharting", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", 2);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetEmployeeDetailsforChart getEmployeeDetailsforChart = new GetEmployeeDetailsforChart();
                    if (reader["CountOfUsers"] != DBNull.Value)
                        getEmployeeDetailsforChart.CountOfUsers = Convert.ToInt32(reader["CountOfUsers"]);
                    if (reader["Month"] != DBNull.Value)
                        getEmployeeDetailsforChart.Month = reader["Month"].ToString();
                    getEmployeeDetailsforCharts.Add(getEmployeeDetailsforChart);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    GetEmployeeDetailsforChart getEmployeeDetailsforChart = new GetEmployeeDetailsforChart();
                    if (reader["CountOfSalesPersons"] != DBNull.Value)
                        getEmployeeDetailsforChart.CountOfSalesPersons = Convert.ToInt32(reader["CountOfSalesPersons"]);
                    if (reader["Month"] != DBNull.Value)
                    {
                        if (getEmployeeDetailsforCharts.Count > 0)
                        {
                            for (int index = 0; index < getEmployeeDetailsforCharts.Count; index++)
                            {
                                if (Convert.ToString(reader["Month"]) != getEmployeeDetailsforCharts[index].Month)
                                {
                                    getEmployeeDetailsforChart.Month = reader["Month"].ToString();
                                }
                            }
                        }
                        else
                        {
                            getEmployeeDetailsforChart.Month = reader["Month"].ToString();
                        }
                    }
                    getEmployeeDetailsforCharts.Add(getEmployeeDetailsforChart);
                }
                con.Close();
                return getEmployeeDetailsforCharts;
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
        /// Get Yearly Service Request Count                                                                                     -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public GetServicesCountForChart GetServicesCountYearly()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetServicesCountForChart getServicesCountForChart = new GetServicesCountForChart();
                getServicesCountForChart.closeServiceChartYears = new List<CloseServiceChartYear>();
                getServicesCountForChart.openServiceChartYears = new List<OpenServiceChartYear>();
                SqlCommand command = new SqlCommand("USP_Admin_GetServiceRequestsForChart", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", 1);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    CloseServiceChartYear closeServiceChartYear = new CloseServiceChartYear();
                    if (reader["CloseServiceCount"] != DBNull.Value)
                        closeServiceChartYear.CloseServiceCount = Convert.ToInt32(reader["CloseServiceCount"]);
                    if (reader["CloseServiceYear"] != DBNull.Value)
                        closeServiceChartYear.CloseServiceYear = Convert.ToInt32(reader["CloseServiceYear"]);
                    getServicesCountForChart.closeServiceChartYears.Add(closeServiceChartYear);
                }
                reader.Close();
                con.Close();


                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_GetServiceRequestsForChart", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", 2);

                con.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    OpenServiceChartYear openServiceChartYear = new OpenServiceChartYear();
                    if (reader["OpenServiceCount"] != DBNull.Value)
                        openServiceChartYear.OpenServiceCount = Convert.ToInt32(reader["OpenServiceCount"]);
                    if (reader["OpenServiceYear"] != DBNull.Value)
                    {
                        if (getServicesCountForChart.closeServiceChartYears.Count > 0)
                        {
                            for (int i = 0; i < getServicesCountForChart.closeServiceChartYears.Count; i++)
                            {
                                if (getServicesCountForChart.closeServiceChartYears[i].CloseServiceYear != Convert.ToInt32(reader["OpenServiceYear"]))
                                {
                                    openServiceChartYear.OpenServiceYear = Convert.ToInt32(reader["OpenServiceYear"]);
                                }
                            }
                        }
                        else
                        {
                            openServiceChartYear.OpenServiceYear = Convert.ToInt32(reader["OpenServiceYear"]);
                        }

                    }

                    getServicesCountForChart.openServiceChartYears.Add(openServiceChartYear);
                }
                con.Close();

                return getServicesCountForChart;
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
        /// Get Monthly Service Request Count                                                                                    -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public GetServicesCountForChart GetServicesCountMonthly()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetServicesCountForChart getServicesCountForChart = new GetServicesCountForChart();
                getServicesCountForChart.closeServiceChartMonths = new List<CloseServiceChartMonth>();
                getServicesCountForChart.openServiceChartMonths = new List<OpenServiceChartMonth>();
                SqlCommand command = new SqlCommand("USP_Admin_GetServiceRequestsForChart", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", 3);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    CloseServiceChartMonth closeServiceChartMonth = new CloseServiceChartMonth();
                    if (reader["CloseServiceCount"] != DBNull.Value)
                        closeServiceChartMonth.CloseServiceCount = Convert.ToInt32(reader["CloseServiceCount"]);
                    if (reader["CloseServiceMonth"] != DBNull.Value)
                        closeServiceChartMonth.CloseServiceMonth = Convert.ToString(reader["CloseServiceMonth"]);
                    getServicesCountForChart.closeServiceChartMonths.Add(closeServiceChartMonth);
                }
                reader.Close();
                con.Close();


                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_GetServiceRequestsForChart", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", 4);

                con.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    OpenServiceChartMonth openServiceChartMonth = new OpenServiceChartMonth();
                    if (reader["OpenServiceCount"] != DBNull.Value)
                        openServiceChartMonth.OpenServiceCount = Convert.ToInt32(reader["OpenServiceCount"]);
                    if (reader["OpenServiceMonth"] != DBNull.Value)
                    {
                        if (getServicesCountForChart.closeServiceChartMonths.Count > 0)
                        {
                            for (int i = 0; i < getServicesCountForChart.closeServiceChartMonths.Count; i++)
                            {
                                if (getServicesCountForChart.closeServiceChartMonths[i].CloseServiceMonth != Convert.ToString(reader["OpenServiceMonth"]))
                                {
                                    openServiceChartMonth.OpenServiceMonth = Convert.ToString(reader["OpenServiceMonth"]);
                                }
                            }
                        }
                        else
                        {
                            openServiceChartMonth.OpenServiceMonth = Convert.ToString(reader["OpenServiceMonth"]);
                        }

                    }

                    getServicesCountForChart.openServiceChartMonths.Add(openServiceChartMonth);
                }
                con.Close();

                return getServicesCountForChart;
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
