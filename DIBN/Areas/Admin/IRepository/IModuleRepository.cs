using DIBN.Areas.Admin.Models;
using System.Collections.Generic;

namespace DIBN.Areas.Admin.IRepository
{
    public interface IModuleRepository
    {
        List<ModuleViewModel> GetModules();
    }
}
