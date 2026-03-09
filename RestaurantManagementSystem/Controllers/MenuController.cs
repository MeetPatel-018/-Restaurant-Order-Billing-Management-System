using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurantManagementSystem.Data;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Controllers
{
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MenuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Menu
        public async Task<IActionResult> Index()
        {
            var menuItems = await _context.MenuItems
                .Include(m => m.Category)
                .ToListAsync();
            return View(menuItems);
        }

        // GET: Menu/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menuItem = await _context.MenuItems
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.ItemID == id);

            if (menuItem == null)
            {
                return NotFound();
            }

            return View(menuItem);
        }

        // GET: Menu/Create
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Menu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemName,CategoryID,Price,Description,Status")] MenuItem menuItem)
        {
            // Debug logging
            Console.WriteLine("=== Menu Create POST Started ===");
            Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
            Console.WriteLine($"ItemName: {menuItem.ItemName}");
            Console.WriteLine($"CategoryID: {menuItem.CategoryID}");
            Console.WriteLine($"Price: {menuItem.Price}");
            Console.WriteLine($"Description: {menuItem.Description}");
            Console.WriteLine($"Status: {menuItem.Status}");
            
            foreach (var modelState in ModelState)
            {
                foreach (var error in modelState.Value.Errors)
                {
                    Console.WriteLine($"Model Error - {modelState.Key}: {error.ErrorMessage}");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    menuItem.Status = "Available"; // Set default status
                    Console.WriteLine("Adding menu item to context...");
                    _context.Add(menuItem);
                    Console.WriteLine("Saving changes...");
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Save successful!");
                    TempData["Success"] = "Menu item created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occurred: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    ModelState.AddModelError("", "Unable to create menu item. Please try again.");
                }
            }
            else
            {
                Console.WriteLine("ModelState is invalid - returning to view");
            }
            
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryID", "CategoryName");
            return View(menuItem);
        }

        // GET: Menu/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryID", "CategoryName", menuItem.CategoryID);
            return View(menuItem);
        }

        // POST: Menu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ItemID,ItemName,CategoryID,Price,Description,Status")] MenuItem menuItem)
        {
            if (id != menuItem.ItemID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(menuItem);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Menu item updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuItemExists(menuItem.ItemID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Unable to update menu item. Please try again.");
                }
            }
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryID", "CategoryName", menuItem.CategoryID);
            return View(menuItem);
        }

        // GET: Menu/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            return View(menuItem);
        }

        // POST: Menu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem != null)
            {
                try
                {
                    _context.MenuItems.Remove(menuItem);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Menu item deleted successfully!";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Unable to delete menu item. Please try again.";
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MenuItemExists(int id)
        {
            return _context.MenuItems.Any(e => e.ItemID == id);
        }
    }
}
