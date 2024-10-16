using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DIBN.Areas.Admin.Data;
using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Areas.Admin.Repository
{
    public class BannerRepository : IBannerRepository
    {
        private readonly Connection _dataSetting;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BannerRepository(Connection dataSetting, IWebHostEnvironment webHostEnvironment)
        {
            _dataSetting = dataSetting;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Get Banners for Login Page                                                                                                          -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<BannerViewModel> GetBanners()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<BannerViewModel> banners = new List<BannerViewModel>();
                SqlCommand command = new SqlCommand("USP_Admin_BannerOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Show);
                con.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    BannerViewModel banner = new BannerViewModel();
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
                con.Open();
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

        /// <summary>
        /// Save New Banner Details                                                                                                             -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="banner"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int CreateNewBanner(BannerViewModel banner)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {

                int returnId = 0;
                string _getName = banner.formFile.FileName;
                var Name = banner.formFile.FileName.Split(".");
                string FileName = Name[0];
                var extn = Path.GetExtension(_getName);

                Byte[] bytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    banner.formFile.OpenReadStream().CopyTo(ms);
                    bytes = ms.ToArray();
                }

                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "assets/banners");
                string filePath = Path.Combine(uploadsFolder, _getName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    banner.formFile.CopyTo(fileStream);
                }

                Account account = new Account(
                  "dhokafmcn",
                  "719997125488579",
                  "Q2AYNHtja4v3W9CAiStymUP0dxI");

                Cloudinary cloudinary = new Cloudinary(account);
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(filePath),
                    UseFilename = true,
                    Folder = "assets/banners",
                    UniqueFilename = false,
                    Overwrite = true
                };
                var uploadResult = cloudinary.Upload(uploadParams);
                string path = uploadResult.SecureUri.AbsoluteUri;

                SqlCommand command = new SqlCommand("USP_Admin_BannerOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Insert);
                command.Parameters.AddWithValue("@FileName", FileName);
                command.Parameters.AddWithValue("@Extension", extn);
                command.Parameters.AddWithValue("@PictureBinary", bytes);
                command.Parameters.AddWithValue("@Path", path);
                command.Parameters.AddWithValue("@UserId", banner.UserId);
                command.Parameters.AddWithValue("@IsActive", 1);
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
        /// Active Banner Image to show on Login Page.                                                                       -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="banner"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int UpdateBanner(BannerViewModel banner)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                SqlCommand command = null;
                command = new SqlCommand("USP_Admin_BannerOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Update);
                command.Parameters.AddWithValue("@Id", banner.Id);
                command.Parameters.AddWithValue("@UserId", banner.UserId);
                command.Parameters.AddWithValue("@IsActive", banner.IsActive);
                con.Open();
                command.ExecuteNonQuery();
                con.Close();
                return 1;
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
        /// De-Activate Banner only If there is more than one Banner is stored.                                                                       -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="banner"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int DeActivateBanner(BannerViewModel banner)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                SqlCommand command = null;
                command = new SqlCommand("USP_Admin_GetCountOfBannersWithActiveStatus", con);
                command.CommandType = CommandType.StoredProcedure;
                con.Open();
                int bannerCount = (int)command.ExecuteScalar();
                con.Close();
                if (bannerCount > 1)
                {
                    command.Parameters.Clear();
                    command = new SqlCommand("USP_Admin_BannerOperation", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Status", Operation.Update);
                    command.Parameters.AddWithValue("@Id", banner.Id);
                    command.Parameters.AddWithValue("@UserId", banner.UserId);
                    command.Parameters.AddWithValue("@IsActive", banner.IsActive);
                    con.Open();
                    command.ExecuteNonQuery();
                    con.Close();
                    return 1;
                }
                return -1;
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
        /// Delete Banner only If there is more than one Banner is stored.                                                                      -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int DeleteBanner(int Id, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                SqlCommand command = null;
                command = new SqlCommand("USP_Admin_GetCountOfBannersWithActiveStatus", con);
                command.CommandType = CommandType.StoredProcedure;
                con.Open();
                int bannerCount = (int)command.ExecuteScalar();
                con.Close();
                if (bannerCount > 1)
                {
                    command.Parameters.Clear();
                    command = new SqlCommand("USP_Admin_BannerOperation", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Status", Operation.Delete);
                    command.Parameters.AddWithValue("@Id", Id);
                    command.Parameters.AddWithValue("@UserId", UserId);
                    con.Open();
                    command.ExecuteNonQuery();
                    con.Close();
                    return 1;
                }
                return -1;
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
