using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RestaurantManagement.Models;
using System.IO;

namespace RestaurantManagement.Areas.AdminManagement.Controllers
{
    [Area("AdminManagement")]
    public class MenusController : Controller
    {
        private readonly RestaurantManagementContext _context;

        public MenusController(RestaurantManagementContext context)
        {
            _context = context;
        }

        // GET: AdminManagement/Menus
        public async Task<IActionResult> Index(string search)
        {
            // Lấy dữ liệu từ cơ sở dữ liệu
            var menuItems = _context.Menus.AsQueryable();

            // Nếu có tham số tìm kiếm, lọc theo tên món
            if (!string.IsNullOrEmpty(search))
            {
                menuItems = menuItems.Where(m => m.Name.Contains(search)); // Tìm kiếm theo tên món
            }

            // Tính tổng giá theo từng Name
            var groupedData = menuItems
                .GroupBy(m => m.Name)
                .Select(g => new
                {
                    Name = g.Key,
                    TotalPrice = g.Sum(m => m.Price)
                }).ToList();

            // Truyền dữ liệu vào ViewBag
            ViewBag.GroupedData = groupedData;
            ViewBag.Tables = _context.Tables.ToList();

            // Chuyển dữ liệu vào view
            return View(await menuItems.ToListAsync()); // Thực thi truy vấn và trả về kết quả
        }


        // GET: AdminManagement/Menus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.MenuId == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // GET: AdminManagement/Menus/Create
        public IActionResult Create()
        {
            ViewBag.Tables = _context.Tables.ToList();
            ViewBag.MenuItems = _context.MenuItems.ToList();
            return View();
        }

        // POST: AdminManagement/Menus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MenuId,Name,Price,Description")] Menu menu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(menu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(menu);
        }

        // GET: AdminManagement/Menus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound();
            }
            return View(menu);
        }

        // POST: AdminManagement/Menus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MenuId,Name,Price,Description")] Menu menu)
        {
            if (id != menu.MenuId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(menu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuExists(menu.MenuId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(menu);
        }

        // GET: AdminManagement/Menus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.MenuId == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // POST: AdminManagement/Menus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu != null)
            {
                _context.Menus.Remove(menu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ExportToExcel(string search)
        {
            // Lấy dữ liệu từ cơ sở dữ liệu
            var menuItems = _context.Menus.AsQueryable();

            // Nếu có tham số tìm kiếm, lọc theo tên bàn (tên món ăn)
            if (!string.IsNullOrEmpty(search))
            {
                menuItems = menuItems.Where(m => m.Name.Contains(search));
            }

            // Lấy danh sách dữ liệu đã được nhóm theo bàn (menu)
            var groupedData = menuItems
                .GroupBy(m => m.Name)  // Nhóm theo tên bàn
                .Select(g => new
                {
                    Name = g.Key,
                    Items = g.ToList(),  // Lấy tất cả các món ăn theo bàn
                    TotalPrice = g.Sum(m => m.Price)  // Tính tổng giá của bàn
                }).ToList();

            // Nếu không có dữ liệu tìm kiếm được, trả về thông báo (tùy chọn)
            if (!groupedData.Any())
            {
                return Content("Không tìm thấy dữ liệu cho bàn: " + search);
            }

            // Tạo file Excel
            using (var package = new ExcelPackage())
            {
                // Thêm một worksheet cho bảng chi tiết và tổng giá
                var worksheet = package.Workbook.Worksheets.Add("MenuItemsAndGroupedData");
                worksheet.Cells[1, 1].Value = "Tên Bàn";
                worksheet.Cells[1, 2].Value = "Giá";
                worksheet.Cells[1, 3].Value = "Món";
                worksheet.Cells[1, 4].Value = "Hành động";

                // Điền dữ liệu vào worksheet
                var row = 2;
                foreach (var group in groupedData)
                {
                    // Điền tất cả các món ăn của bàn
                    foreach (var item in group.Items)
                    {
                        worksheet.Cells[row, 1].Value = group.Name;  // Tên bàn
                        worksheet.Cells[row, 2].Value = item.Price;  // Giá của món
                        worksheet.Cells[row, 3].Value = item.Description;  // Món ăn
                        worksheet.Cells[row, 4].Value = "Xóa";  // Hành động: "Xóa"
                        row++;
                    }

                    // Thêm một dòng với tổng giá của bàn
                    worksheet.Cells[row, 1].Value = "Tổng giá bàn " + group.Name;
                    worksheet.Cells[row, 2].Value = group.TotalPrice;  // Tổng giá của bàn
                    worksheet.Cells[row, 3].Value = "";  // Món ăn rỗng
                    worksheet.Cells[row, 4].Value = "Xóa";  // Hành động: "Xóa"
                    row++;
                }

                // Định dạng cho các cột
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Lưu file vào bộ nhớ
                var fileContents = package.GetAsByteArray();

                // Trả về file Excel dưới dạng download
                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", search + "_MenuItems.xlsx");
            }
        }



        private bool MenuExists(int id)
        {
            return _context.Menus.Any(e => e.MenuId == id);
        }
    }
}
