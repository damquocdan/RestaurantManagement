using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Models;

namespace RestaurantManagement.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly RestaurantManagementContext _context;

        public ReservationsController(RestaurantManagementContext context)
        {
            _context = context;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var restaurantManagementContext = _context.Reservations.Include(r => r.Customer).Include(r => r.Table);
            return View(await restaurantManagementContext.ToListAsync());
        }
        [HttpGet]
        public async Task<IActionResult> Add(int id)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                TempData["Error"] = "Bạn cần đăng nhập để sử dụng chức năng này.";
                return RedirectToAction("Index", "LoginC");
            }
            // Kiểm tra bàn tồn tại
            var table = await _context.Tables.FindAsync(id);
            if (table == null)
            {
                TempData["ErrorMessage"] = "Bàn không tồn tại!";
                return RedirectToAction("Index", "Tables");
            }

            // Kiểm tra trạng thái bàn
            if (table.IsAvailable == false)
            {
                TempData["ErrorMessage"] = $"Bàn {table.TableNumber} đã được đặt!";
                return RedirectToAction("Index", "Tables");
            }

            // Tạo một bản ghi đặt bàn
            var reservation = new Reservation
            {
                TableId = id,
                CustomerId = customerId.Value,
                ReservationDate = DateTime.Now
            };

            // Đánh dấu bàn đã được đặt
            table.IsAvailable = false;

            // Lưu thay đổi
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã đặt bàn {table.TableNumber} thành công!";
            return RedirectToAction("Index", "Tables");
        }
        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Table)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            ViewData["TableId"] = new SelectList(_context.Tables, "TableId", "TableId");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservationId,CustomerId,TableId,ReservationDate,Status,CreatedAt,UpdatedAt")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", reservation.CustomerId);
            ViewData["TableId"] = new SelectList(_context.Tables, "TableId", "TableId", reservation.TableId);
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", reservation.CustomerId);
            ViewData["TableId"] = new SelectList(_context.Tables, "TableId", "TableId", reservation.TableId);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReservationId,CustomerId,TableId,ReservationDate,Status,CreatedAt,UpdatedAt")] Reservation reservation)
        {
            if (id != reservation.ReservationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.ReservationId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", reservation.CustomerId);
            ViewData["TableId"] = new SelectList(_context.Tables, "TableId", "TableId", reservation.TableId);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Table)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.ReservationId == id);
        }
    }
}
