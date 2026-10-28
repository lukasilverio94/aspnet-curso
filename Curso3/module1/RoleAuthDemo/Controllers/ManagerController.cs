using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RoleAuthDemo.Controllers
{
    public class ManagerController : Controller
    {
        [Authorize(Roles = "Manager,Admin")]
        public IActionResult Dashboard()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminOnly()
        {
            return View();
        }

        public IActionResult PublicAction()
        {
            return View();
        }
    }
}