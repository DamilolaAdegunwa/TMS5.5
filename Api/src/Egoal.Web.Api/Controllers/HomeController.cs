using Egoal.Models;
using Egoal.Mvc.Uow;
using Egoal.Settings;
using Egoal.Threading.Lock;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Egoal.Web.Api.Controllers
{
    [Route("/")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IHostingEnvironment _environment;
        private readonly ISettingAppService _settingAppService;
        private readonly IDistributedLockFactory _lockFactory;

        public HomeController(
            IHostingEnvironment environment,
            ISettingAppService settingAppService,
            IDistributedLockFactory lockFactory)
        {
            _environment = environment;
            _settingAppService = settingAppService;
            _lockFactory = lockFactory;
        }

        [HttpGet]
        [AllowAnonymous]
        [DontWrapResult]
        [UnitOfWork]
        public async Task<ActionResult> Get()
        {
            try
            {
                var options = await _settingAppService.GetOrderNoticeAsync();

                using (await _lockFactory.LockAsync("LockTest")) { }

                if (System.IO.File.Exists(Path.Combine(_environment.WebRootPath, "admin", "index.html")))
                {
                    return Redirect("/admin/index.html");
                }

                return new ObjectResult($"{options.ScenicName}WebApi接口V5.5.5");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
