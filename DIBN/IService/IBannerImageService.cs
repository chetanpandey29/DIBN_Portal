using DIBN.Models;
using System.Collections.Generic;

namespace DIBN.IService
{
    public interface IBannerImageService
    {
        List<BannerImageViewModel> GetBanners();
    }
}
