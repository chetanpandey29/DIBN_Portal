using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Areas.Admin.Repository
{
    public class ModuleRepository : IModuleRepository
    {
        private readonly Connection _dataSetting;
        public ModuleRepository(Connection dataSetting)
        {
            _dataSetting = dataSetting;
        }

        public List<ModuleViewModel> GetModules()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<ModuleViewModel> modules = new List<ModuleViewModel>();
                SqlCommand command = new SqlCommand("USP_Admin_GetModules", con);
                command.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    ModuleViewModel module = new ModuleViewModel();
                    module.ModuleId = Convert.ToInt32(dr["ModuleId"].ToString());
                    module.ModuleName = dr["ModuleName"].ToString();
                    module.ModuleKeyword = dr["ModuleKeyword"].ToString();
                    module.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    module.IsDelete = Convert.ToBoolean(dr["IsDelete"].ToString());
                    module.CreatedOn = dr["CreatedOn"].ToString();
                    module.ModifyOn = dr["ModifyOn"].ToString();
                    modules.Add(module);
                }
                con.Close();
                return modules;
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
    }
}
