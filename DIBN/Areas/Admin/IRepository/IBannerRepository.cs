using DIBN.Areas.Admin.Models;
using System.Collections.Generic;

namespace DIBN.Areas.Admin.IRepository
{
    public interface IBannerRepository
    {
        List<BannerViewModel> GetBanners();
        int CreateNewBanner(BannerViewModel banner);
        int UpdateBanner(BannerViewModel banner);
        int DeActivateBanner(BannerViewModel banner);
        int DeleteBanner(int Id, int UserId);
    }
}
