using ADUserStatistics.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace ADUserStatistics.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PasswordPolicy _passwordPolicy;
        private readonly Config _config;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="passwordPolicy"></param>
        /// <param name="config"></param>
        public HomeController(ILogger<HomeController> logger, PasswordPolicy passwordPolicy, Config config)
        {
            _logger = logger;
            _passwordPolicy = passwordPolicy;
            _config = config;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {            
            return View(_passwordPolicy);
        }
        

        public IActionResult UsersBlocked()
        {
            UserBlocked userBlocked = new UserBlocked(_passwordPolicy, _config.LdapPath);
            ViewBag.PasswordPolicy = _passwordPolicy;           
            return Json(userBlocked.Results
                    .OrderBy(t => t.SAMaccountName?.Length >= 5 ? t.SAMaccountName.Substring(0, 5) : t.SAMaccountName)
                    .ThenBy(t => t.DisplayName));
        }

        public IActionResult UsersPasswordEperied()
        {
            UserPasswordExpired userPasswordExpired = new UserPasswordExpired(_passwordPolicy, _config.LdapPath);
            ViewBag.PasswordPolicy = _passwordPolicy;           
            return Json(userPasswordExpired.Results
                    .OrderBy(t => t.SAMaccountName?.Length >= 5 ? t.SAMaccountName.Substring(0, 5) : t.SAMaccountName)
                    .ThenBy(t => t.DisplayName));
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}