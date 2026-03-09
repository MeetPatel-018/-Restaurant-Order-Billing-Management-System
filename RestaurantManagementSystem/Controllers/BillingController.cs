using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManagementSystem.Data;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Controllers
{
    public class BillingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BillingController> _logger;

        public BillingController(ApplicationDbContext context, IConfiguration configuration, ILogger<BillingController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        // GET: Billing
        public async Task<IActionResult> Index()
        {
            var bills = await _context.Bills
                .Include(b => b.Order)
                .ThenInclude(o => o.Table)
                .OrderByDescending(b => b.BillDate)
                .ToListAsync();
            return View(bills);
        }

        // GET: Billing/GenerateBill/5
        public async Task<IActionResult> GenerateBill(int? orderId)
        {
            if (orderId == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.OrderID == orderId);

            if (order == null)
            {
                return NotFound();
            }

            // Check if bill already exists
            var existingBill = await _context.Bills.FirstOrDefaultAsync(b => b.OrderID == orderId);
            if (existingBill != null)
            {
                return RedirectToAction("Details", new { id = existingBill.BillID });
            }

            // Calculate bill details
            var taxRate = _configuration.GetValue<double>("TaxRate", 5.0);
            var subtotal = order.TotalAmount;
            var tax = subtotal * (decimal)(taxRate / 100);
            var finalAmount = subtotal + tax;

            var bill = new Bill
            {
                OrderID = order.OrderID,
                SubTotal = subtotal,
                Tax = tax,
                FinalAmount = finalAmount,
                PaymentMethod = "Cash",
                BillDate = DateTime.Now
            };

            ViewBag.Order = order;
            ViewBag.TaxRate = taxRate;

            return View(bill);
        }

        // POST: Billing/GenerateBill
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateBill([Bind("OrderID,SubTotal,Tax,FinalAmount,PaymentMethod")] Bill bill)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Set bill date
                    bill.BillDate = DateTime.Now;
                    
                    // Add bill to database
                    _context.Bills.Add(bill);
                    
                    // Update order status to completed
                    var order = await _context.Orders.FindAsync(bill.OrderID);
                    if (order != null)
                    {
                        order.OrderStatus = "Completed";
                        
                        // Free up the table
                        var table = await _context.Tables.FindAsync(order.TableID);
                        if (table != null)
                        {
                            table.Status = "Available";
                        }
                    }

                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Bill generated successfully for Order {OrderId}", bill.OrderID);
                    TempData["Success"] = "Bill generated successfully!";
                    return RedirectToAction("Details", new { id = bill.BillID });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error generating bill for Order {OrderId}: {Error}", bill.OrderID, ex.Message);
                    ModelState.AddModelError("", "Unable to generate bill. Please try again.");
                }
            }
            
            // If we got this far, something failed, redisplay form
            var orderDetails = await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.OrderID == bill.OrderID);
            ViewBag.Order = orderDetails;
            ViewBag.TaxRate = _configuration.GetValue<double>("TaxRate", 5.0);
            
            return View(bill);
        }

        // GET: Billing/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bills
                .Include(b => b.Order)
                .ThenInclude(o => o.Table)
                .Include(b => b.Order.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(m => m.BillID == id);

            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        // GET: Billing/PrintBill/5
        public async Task<IActionResult> PrintBill(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bills
                .Include(b => b.Order)
                .ThenInclude(o => o.Table)
                .Include(b => b.Order.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(m => m.BillID == id);

            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        // GET: Billing/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bills
                .Include(b => b.Order)
                .ThenInclude(o => o.Table)
                .FirstOrDefaultAsync(m => m.BillID == id);

            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        // POST: Billing/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bill = await _context.Bills.FindAsync(id);
            if (bill != null)
            {
                try
                {
                    // Update order status back to pending
                    var order = await _context.Orders.FindAsync(bill.OrderID);
                    if (order != null)
                    {
                        order.OrderStatus = "Pending";
                    }

                    _context.Bills.Remove(bill);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Bill deleted successfully!";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Unable to delete bill. Please try again.";
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
