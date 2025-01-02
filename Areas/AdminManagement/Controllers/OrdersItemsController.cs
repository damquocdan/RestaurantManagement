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
    public class OrdersItemsController : Controller
    {
        private readonly RestaurantManagementContext _context;

        public OrdersItemsController(RestaurantManagementContext context)
        {
            _context = context;
        }

        // GET: AdminManagement/OrdersItems
        public async Task<IActionResult> Index()
        {
            var restaurantManagementContext = _context.OrdersItems.Include(o => o.Item).Include(o => o.Menu);
            return View(await restaurantManagementContext.ToListAsync());
        }

        // GET: AdminManagement/OrdersItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordersItem = await _context.OrdersItems
                .Include(o => o.Item)
                .Include(o => o.Menu)
                .FirstOrDefaultAsync(m => m.OrdersItemId == id);
            if (ordersItem == null)
            {
                return NotFound();
            }

            return View(ordersItem);
        }

        // GET: AdminManagement/OrdersItems/Create
        public IActionResult Create()
        {
            ViewData["ItemId"] = new SelectList(_context.MenuItems, "ItemId", "ItemId");
            ViewData["MenuId"] = new SelectList(_context.Menus, "MenuId", "MenuId");
            return View();
        }

        // POST: AdminManagement/OrdersItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrdersItemId,MenuId,ItemId,Quantity,Price")] OrdersItem ordersItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ordersItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemId"] = new SelectList(_context.MenuItems, "ItemId", "ItemId", ordersItem.ItemId);
            ViewData["MenuId"] = new SelectList(_context.Menus, "MenuId", "MenuId", ordersItem.MenuId);
            return View(ordersItem);
        }

        // GET: AdminManagement/OrdersItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordersItem = await _context.OrdersItems.FindAsync(id);
            if (ordersItem == null)
            {
                return NotFound();
            }
            ViewData["ItemId"] = new SelectList(_context.MenuItems, "ItemId", "ItemId", ordersItem.ItemId);
            ViewData["MenuId"] = new SelectList(_context.Menus, "MenuId", "MenuId", ordersItem.MenuId);
            return View(ordersItem);
        }

        // POST: AdminManagement/OrdersItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrdersItemId,MenuId,ItemId,Quantity,Price")] OrdersItem ordersItem)
        {
            if (id != ordersItem.OrdersItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ordersItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdersItemExists(ordersItem.OrdersItemId))
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
            ViewData["ItemId"] = new SelectList(_context.MenuItems, "ItemId", "ItemId", ordersItem.ItemId);
            ViewData["MenuId"] = new SelectList(_context.Menus, "MenuId", "MenuId", ordersItem.MenuId);
            return View(ordersItem);
        }

        // GET: AdminManagement/OrdersItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordersItem = await _context.OrdersItems
                .Include(o => o.Item)
                .Include(o => o.Menu)
                .FirstOrDefaultAsync(m => m.OrdersItemId == id);
            if (ordersItem == null)
            {
                return NotFound();
            }

            return View(ordersItem);
        }

        // POST: AdminManagement/OrdersItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ordersItem = await _context.OrdersItems.FindAsync(id);
            if (ordersItem != null)
            {
                _context.OrdersItems.Remove(ordersItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdersItemExists(int id)
        {
            return _context.OrdersItems.Any(e => e.OrdersItemId == id);
        }
    }
}
