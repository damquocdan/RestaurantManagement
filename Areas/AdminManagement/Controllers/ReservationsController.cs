using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Models;

namespace RestaurantManagement.Areas.AdminManagement.Controllers
{
    [Area("AdminManagement")]
    public class ReservationsController : Controller
    {
        private readonly RestaurantManagementContext _context;
        
        public ReservationsController(RestaurantManagementContext context)
        {
            _context = context;
        }

        // GET: AdminManagement/Reservations
        public async Task<IActionResult> Index()
        {
            var restaurantManagementContext = _context.Reservations.Include(r => r.Customer).Include(r => r.Table);
            return View(await restaurantManagementContext.ToListAsync());
        }

        // GET: AdminManagement/Reservations/Details/5
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

        // GET: AdminManagement/Reservations/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            ViewData["TableId"] = new SelectList(_context.Tables, "TableId", "TableId");
            return View();
        }

        // POST: AdminManagement/Reservations/Create
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

        // GET: AdminManagement/Reservations/Edit/5
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

        // POST: AdminManagement/Reservations/Edit/5
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

        // GET: AdminManagement/Reservations/Delete/5
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

        // POST: AdminManagement/Reservations/Delete/5
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditStatus(int reservationId)
        {
            var reservation = _context.Reservations.Find(reservationId);

            if (reservation != null)
            {
                reservation.Status = reservation.Status == "Arrived" ? "Chưa đến" : "Arrived";
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }


    }
}
