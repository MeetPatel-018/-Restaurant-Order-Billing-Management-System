using System.ComponentModel.DataAnnotations;

namespace RestaurantManagementSystem.Models
{
    public class OrderViewModel
    {
        public int OrderID { get; set; }
        
        [Required(ErrorMessage = "Table is required")]
        public int TableID { get; set; }
        
        public string? TableNumber { get; set; }
        
        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;
        
        [Required(ErrorMessage = "Order status is required")]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        public string OrderStatus { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Total amount is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Total amount must be non-negative")]
        public decimal TotalAmount { get; set; }
        
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public List<CreateOrderItemViewModel> AvailableMenuItems { get; set; } = new List<CreateOrderItemViewModel>();
        public List<Table> AvailableTables { get; set; } = new List<Table>();
    }

    public class CreateOrderItemViewModel
    {
        public int ItemID { get; set; }
        
        [Required(ErrorMessage = "Item name is required")]
        [StringLength(100, ErrorMessage = "Item name cannot exceed 100 characters")]
        public string ItemName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(50, ErrorMessage = "Category name cannot exceed 50 characters")]
        public string CategoryName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
        
        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; } = 1;
        
        public decimal SubTotal => Price * Quantity;
    }
}
