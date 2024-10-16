using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DIBN.Areas.Admin.Controllers
{
    [Authorize(Policy = "AllowView")]
    [Authorize]
    [Area("Admin")]
    [Route("admin/[controller]")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    #nullable enable
    public class LogController : Controller
    {
        private readonly ILogRepository _logRepository; 

        public LogController(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(string? name)
        {
            List<LogsModel> logs = new List<LogsModel>();
            logs = _logRepository.GetLogs();
            foreach(LogsModel log in logs)
            {
                log.Module = name;
            }
            return View(logs);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult RemoveLog(int Id)
        {
            int _returnId = 0;
            _returnId = _logRepository.RemoveLogDetails(Id);
            return new JsonResult(_returnId);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult RemoveAllLog()
        {
            int _returnId = 0;
            _returnId = _logRepository.RemoveAllLogDetails();
            return new JsonResult(_returnId);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult RemoveSelectedLogs(int[] logIds)
        {
            int _returnId = 0;
            for(int i = 0; i < logIds.Length; i++)
            {
                _returnId = _logRepository.RemoveLogDetails(logIds[i]);
            }
            
            return new JsonResult(_returnId);
        }
    }
}
