using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using RestaurantManagement.Models;

namespace RestaurantManagement.Controllers
{
    public class LoginCController : Controller
    {
        public RestaurantManagementContext _context;
        public LoginCController(RestaurantManagementContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost] // POST -> khi submit form
        public IActionResult Index(LoginC model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không hợp lệ.");
                return View(model);
            }

            var pass = model.Password;
            var dataLogin = _context.Customers.FirstOrDefault(x => x.Email.Equals(model.Email) && x.Password.Equals(pass));
            if (dataLogin != null)
            {
                HttpContext.Session.SetString("CustomerLogin", model.Email);
                HttpContext.Session.SetInt32("CustomerId", dataLogin.CustomerId);
                return RedirectToAction("Index", "Home");
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
            HttpContext.Session.Remove("CustomerLogin"); // huỷ session với key AdminLogin đã lưu trước đó

            return RedirectToAction("Index");
        }
    }
}

