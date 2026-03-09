using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManagementSystem.Data;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin
        // GET: Admin/Index
        public async Task<IActionResult> Index()
        {
            return await Dashboard();
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var viewModel = new DashboardViewModel
            {
                TotalMenuItems = await _context.MenuItems.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),
                TotalSales = await _context.Bills.SumAsync(b => b.FinalAmount),
                TodayOrders = await _context.Orders
                    .Where(o => o.OrderDate.Date == DateTime.Today)
                    .CountAsync(),
                AvailableTables = await _context.Tables
                    .Where(t => t.Status == "Available")
                    .CountAsync(),
                OccupiedTables = await _context.Tables
                    .Where(t => t.Status == "Occupied")
                    .CountAsync(),
                TodaySales = await _context.Bills
                    .Where(b => b.BillDate.Date == DateTime.Today)
                    .SumAsync(b => b.FinalAmount),
                RecentOrders = await _context.Orders
                    .Include(o => o.Table)
                    .Include(o => o.OrderItems)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // GET: Admin/CreateMenuItem
        public IActionResult CreateMenuItem()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        // POST: Admin/CreateMenuItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMenuItem([Bind("ItemName,CategoryID,Price,Description,Status")] MenuItem menuItem)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(menuItem);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Menu item created successfully!";
                    return RedirectToAction(nameof(Dashboard));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Unable to create menu item. Please try again.");
                }
            }
            ViewBag.Categories = _context.Categories.ToList();
            return View(menuItem);
        }

        // GET: Admin/CreateOrder
        public async Task<IActionResult> CreateOrder()
        {
            var viewModel = new OrderViewModel
            {
                AvailableMenuItems = await _context.MenuItems
                    .Where(m => m.Status == "Available")
                    .Include(m => m.Category)
                    .Select(m => new CreateOrderItemViewModel
                    {
                        ItemID = m.ItemID,
                        ItemName = m.ItemName,
                        CategoryName = m.Category.CategoryName,
                        Price = m.Price,
                        Quantity = 1
                    })
                    .ToListAsync(),
                AvailableTables = await _context.Tables
                    .Where(t => t.Status == "Available")
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // POST: Admin/CreateOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrder([Bind("TableID,OrderItems")] OrderViewModel viewModel)
        {
            if (ModelState.IsValid && viewModel.OrderItems != null && viewModel.OrderItems.Any())
            {
                try
                {
                    // Create the order
                    var order = new Order
                    {
                        TableID = viewModel.TableID,
                        OrderDate = DateTime.Now,
                        OrderStatus = "Pending",
                        TotalAmount = 0
                    };

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    // Add order items
                    decimal totalAmount = 0;
                    foreach (var item in viewModel.OrderItems.Where(oi => oi.Quantity > 0))
                    {
                        var menuItem = await _context.MenuItems.FindAsync(item.ItemID);
                        if (menuItem != null)
                        {
                            var orderItem = new OrderItem
                            {
                                OrderID = order.OrderID,
                                ItemID = item.ItemID,
                                Quantity = item.Quantity,
                                Price = menuItem.Price,
                                SubTotal = menuItem.Price * item.Quantity
                            };

                            _context.OrderItems.Add(orderItem);
                            totalAmount += orderItem.SubTotal;
                        }
                    }

                    // Update order total
                    order.TotalAmount = totalAmount;
                    
                    // Update table status
                    var table = await _context.Tables.FindAsync(order.TableID);
                    if (table != null)
                    {
                        table.Status = "Occupied";
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Order created successfully!";
                    return RedirectToAction(nameof(Dashboard));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Unable to create order. Please try again.");
                }
            }

            // If we got this far, something failed, redisplay form
            viewModel.AvailableMenuItems = await _context.MenuItems
                .Where(m => m.Status == "Available")
                .Include(m => m.Category)
                .Select(m => new CreateOrderItemViewModel
                {
                    ItemID = m.ItemID,
                    ItemName = m.ItemName,
                    CategoryName = m.Category.CategoryName,
                    Price = m.Price,
                    Quantity = 1
                })
                .ToListAsync();
            viewModel.AvailableTables = await _context.Tables
                .Where(t => t.Status == "Available")
                .ToListAsync();

            if (viewModel.OrderItems == null || !viewModel.OrderItems.Any())
            {
                ModelState.AddModelError("", "Please select at least one menu item.");
            }

            return View(viewModel);
        }
    }
}
