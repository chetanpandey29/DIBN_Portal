using DIBN.Areas.Admin.Data;
using DIBN.IService;
using DIBN.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Service
{
    public class BannerImageService : IBannerImageService
    {
        private readonly Connection _dataSetting;
        public BannerImageService(Connection dataSetting)
        {
            _dataSetting = dataSetting;
        }
        public List<BannerImageViewModel> GetBanners()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<BannerImageViewModel> banners = new List<BannerImageViewModel>();
                SqlCommand command = new SqlCommand("USP_GetActiveBanners", con);
                command.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    BannerImageViewModel banner = new BannerImageViewModel();
                    banner.Id = Convert.ToInt32(dr["Id"].ToString());
                    banner.FileName = dr["FileName"].ToString();
                    banner.Extension = dr["Extension"].ToString();
                    banner.PictureBinary = (Byte[])dr["PictureBinary"];
                    banner.Path = dr["Path"].ToString();
                    banner.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    banner.IsDelete = Convert.ToBoolean(dr["IsDelete"].ToString());
                    banner.CreatedOnUtc = dr["CreatedOnUtc"].ToString();
                    banner.ModifyOnUtc = dr["ModifyOnUtc"].ToString();
                    banners.Add(banner);
                }
                con.Close();
                //con.Open();
                return banners;
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
