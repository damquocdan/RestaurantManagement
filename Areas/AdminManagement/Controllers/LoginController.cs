
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Areas.AdminManagement.Models;
using RestaurantManagement.Models;
using System.Collections.Generic;


namespace RestaurantManagement.Areas.AdminManagement.Controllers
{
    [Area("AdminManagement")]
    public class LoginController : Controller
    {
        public RestaurantManagementContext _context;
        public LoginController(RestaurantManagementContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost] // POST -> khi submit form
        public IActionResult Index(Login model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không hợp lệ.");
                return View(model);
            }

            var pass = model.Password;
            var dataLogin = _context.Admins.FirstOrDefault(x => x.Email.Equals(model.Email) && x.Password.Equals(pass));
            if (dataLogin != null)
            {
                HttpContext.Session.SetString("AdminLogin", model.Email);
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không chính xác.");
                return View(model);
            }

        }
        [HttpGet]// thoát đăng nhập, huỷ session
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminLogin"); // huỷ session với key AdminLogin đã lưu trước đó

            return RedirectToAction("Index");
        }
    }
}

