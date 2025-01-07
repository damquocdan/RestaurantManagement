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

            // Lấy 20 menu mới nhất dựa trên trường CreatedDate (hoặc trường tương tự)
            var latestMenuItems = await menuItems
                .OrderByDescending(m => m.MenuId) // Sắp xếp theo thời gian giảm dần
                .Take(20) // Lấy 20 mục đầu tiên
                .ToListAsync();

            // Tính tổng giá theo từng Name
            var groupedData = latestMenuItems
                .GroupBy(m => m.Name)
                .Select(g => new
                {
                    Name = g.Key,
                    TotalPrice = g.Sum(m => m.Price)
                }).ToList();

            // Truyền dữ liệu vào ViewBag
            ViewBag.GroupedData = groupedData;
            ViewBag.Tables = await _context.Tables.ToListAsync();

            // Chuyển dữ liệu vào view
            return View(latestMenuItems); // Trả về danh sách 20 món ăn mới nhất
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
        public async Task<IActionResult> ExportToExcelAll()
        {
            // Lấy toàn bộ dữ liệu từ cơ sở dữ liệu
            var menuItems = _context.Menus.ToList();

            // Nhóm dữ liệu theo bàn
            var groupedData = menuItems
                .GroupBy(m => m.Name)
                .Select(g => new
                {
                    Name = g.Key,
                    Items = g.ToList(),
                    TotalPrice = g.Sum(m => m.Price)
                }).ToList();

            if (!groupedData.Any())
            {
                return Content("Không có dữ liệu để xuất Excel.");
            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("AllTables");
                worksheet.Cells[1, 1].Value = "Tên Bàn";
                worksheet.Cells[1, 2].Value = "Giá";
                worksheet.Cells[1, 3].Value = "Món";

                var row = 2;
                foreach (var group in groupedData)
                {
                    foreach (var item in group.Items)
                    {
                        worksheet.Cells[row, 1].Value = group.Name;
                        worksheet.Cells[row, 2].Value = item.Price;
                        worksheet.Cells[row, 3].Value = item.Description;
                        row++;
                    }

                    worksheet.Cells[row, 1].Value = "Tổng giá bàn " + group.Name;
                    worksheet.Cells[row, 2].Value = group.TotalPrice;
                    worksheet.Cells[row, 1, row, 3].Style.Font.Bold = true;
                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var fileContents = package.GetAsByteArray();
                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AllTables.xlsx");
            }
        }


        public async Task<IActionResult> ExportToExcel(string search)
        {
            // Kiểm tra nếu không có giá trị tìm kiếm
            if (string.IsNullOrEmpty(search))
            {
                return Content("Vui lòng chọn bàn trước khi xuất Excel.");
            }

            // Lấy dữ liệu từ cơ sở dữ liệu
            var menuItems = _context.Menus.AsQueryable();

            // Lọc dữ liệu theo tên bàn
            menuItems = menuItems.Where(m => m.Name.Contains(search));

            // Nhóm dữ liệu theo tên bàn
            var groupedData = menuItems
                .GroupBy(m => m.Name)
                .Select(g => new
                {
                    Name = g.Key,
                    Items = g.ToList(),
                    TotalPrice = g.Sum(m => m.Price)
                }).ToList();

            // Xử lý khi không tìm thấy dữ liệu
            if (!groupedData.Any())
            {
                return Content($"Không có dữ liệu nào cho bàn '{search}'. Vui lòng kiểm tra lại.");
            }

            // Tạo file Excel
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("MenuItems");
                worksheet.Cells[1, 1].Value = "Tên Bàn";
                worksheet.Cells[1, 2].Value = "Giá";
                worksheet.Cells[1, 3].Value = "Món";

                var row = 2;
                foreach (var group in groupedData)
                {
                    // Thêm dữ liệu món ăn
                    foreach (var item in group.Items)
                    {
                        worksheet.Cells[row, 1].Value = group.Name;   // Tên bàn
                        worksheet.Cells[row, 2].Value = item.Price;  // Giá của món
                        worksheet.Cells[row, 3].Value = item.Description; // Món ăn
                        row++;
                    }

                    // Thêm dòng tổng giá
                    worksheet.Cells[row, 1].Value = "Tổng giá bàn " + group.Name;
                    worksheet.Cells[row, 2].Value = group.TotalPrice;  // Tổng giá của bàn
                    worksheet.Cells[row, 1, row, 3].Style.Font.Bold = true;  // In đậm dòng tổng
                    row++;
                }

                // Tự động căn chỉnh cột
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Lưu file Excel dưới dạng byte array
                var fileContents = package.GetAsByteArray();
                var fileName = $"{search}_MenuItems.xlsx";

                // Trả về file Excel để tải về
                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }



        private bool MenuExists(int id)
        {
            return _context.Menus.Any(e => e.MenuId == id);
        }
    }
}
