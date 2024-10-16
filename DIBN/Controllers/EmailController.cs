using DIBN.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DIBN.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class EmailController : Controller
    {
        private readonly ICompanyServiceList _companyServiceList;
        public EmailController (ICompanyServiceList companyServiceList)
        {
            _companyServiceList = companyServiceList;
        }
        [HttpGet]
        [Route("ActivateCompany")]
        [AllowAnonymous]
        public IActionResult ActivateCompany(int ActiveId)
        {
            string returnId = null;
            returnId = _companyServiceList.ChangestatusOfCompany(ActiveId);
            return View();
        }
    }
}
