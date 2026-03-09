using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurantManagementSystem.Data;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Bill)
                .FirstOrDefaultAsync(m => m.OrderID == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Order/Create
        public async Task<IActionResult> Create()
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

        // POST: Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TableID")] Order order, IFormCollection form)
        {
            // Validate table availability before proceeding
            var table = await _context.Tables.FindAsync(order.TableID);
            if (table == null || table.Status != "Available")
            {
                ModelState.AddModelError("", "Selected table is not available. Please choose another table.");
                TempData["Error"] = "The selected table is currently occupied. Please select an available table.";
            }

            var selectedItems = new List<OrderItemRequest>();
            
            // Extract selected items and quantities from form
            foreach (var key in form.Keys)
            {
                if (key.StartsWith("Quantities[") && key.EndsWith("]"))
                {
                    var itemIdStr = key.Substring(11, key.Length - 12);
                    if (int.TryParse(itemIdStr, out int itemId))
                    {
                        var quantityStr = form[key];
                        if (int.TryParse(quantityStr, out int quantity) && quantity > 0)
                        {
                            selectedItems.Add(new OrderItemRequest
                            {
                                ItemID = itemId,
                                Quantity = quantity
                            });
                        }
                    }
                }
            }
            
            if (ModelState.IsValid && selectedItems.Count > 0 && table?.Status == "Available")
            {
                try
                {
                    // Create the order
                    order.OrderDate = DateTime.Now;
                    order.OrderStatus = "Pending";
                    order.TotalAmount = 0;

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    // Add order items
                    decimal totalAmount = 0;
                    foreach (var itemRequest in selectedItems)
                    {
                        var menuItem = await _context.MenuItems.FindAsync(itemRequest.ItemID);
                        if (menuItem != null)
                        {
                            var orderItem = new OrderItem
                            {
                                OrderID = order.OrderID,
                                ItemID = itemRequest.ItemID,
                                Quantity = itemRequest.Quantity,
                                Price = menuItem.Price,
                                SubTotal = menuItem.Price * itemRequest.Quantity
                            };

                            _context.OrderItems.Add(orderItem);
                            totalAmount += orderItem.SubTotal;
                        }
                    }

                    // Update order total
                    order.TotalAmount = totalAmount;
                    
                    // Update table status
                    if (table != null)
                    {
                        table.Status = "Occupied";
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Order created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log error (in production, use proper logging)
                    ModelState.AddModelError("", "Unable to create order. Please try again.");
                }
            }
            else
            {
                if (selectedItems.Count == 0)
                {
                    ModelState.AddModelError("", "Please select at least one menu item.");
                }
            }

            // If we got this far, something failed, redisplay form
            if (selectedItems.Count == 0)
            {
                ModelState.AddModelError("", "Please select at least one menu item.");
            }

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

        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(m => m.OrderID == id);

            if (order == null)
            {
                return NotFound();
            }

            var tables = await _context.Tables.ToListAsync();

            ViewBag.Statuses = new[] { "Pending", "Completed", "Cancelled" };
            ViewBag.Tables = new SelectList(_context.Tables, "TableID", "TableNumber", order.TableID);
            return View(order);
        }

        // POST: Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderID,TableID,OrderDate,OrderStatus,TotalAmount")] Order order)
        {
            if (id != order.OrderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();

                    // If order is completed or cancelled, free up the table
                    if (order.OrderStatus == "Completed" || order.OrderStatus == "Cancelled")
                    {
                        var table = await _context.Tables.FindAsync(order.TableID);
                        if (table != null)
                        {
                            table.Status = "Available";
                            await _context.SaveChangesAsync();
                        }
                    }

                    TempData["Success"] = "Order updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderID))
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
                    ModelState.AddModelError("", "Unable to update order. Please try again.");
                }
            }
            ViewBag.Statuses = new[] { "Pending", "Completed", "Cancelled" };
            return View(order);
        }

        // GET: Order/Cancel/5
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderID == id);

            if (order == null)
            {
                return NotFound();
            }

            order.OrderStatus = "Cancelled";
            
            // Free up the table
            var table = await _context.Tables.FindAsync(order.TableID);
            if (table != null)
            {
                table.Status = "Available";
                await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Order cancelled successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.OrderID == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                try
                {
                    // Free up the table
                    var table = await _context.Tables.FindAsync(order.TableID);
                    if (table != null)
                    {
                        table.Status = "Available";
                    }

                    _context.Orders.Remove(order);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Order deleted successfully!";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Unable to delete order. Please try again.";
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderID == id);
        }
    }

    public class OrderItemRequest
    {
        public int ItemID { get; set; }
        public int Quantity { get; set; }
    }
}
