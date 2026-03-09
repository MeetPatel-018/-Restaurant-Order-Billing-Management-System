namespace RestaurantManagementSystem.Models
{
    public class OrderViewModel
    {
        public int OrderID { get; set; }
        public int TableID { get; set; }
        public string? TableNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public List<CreateOrderItemViewModel> AvailableMenuItems { get; set; } = new List<CreateOrderItemViewModel>();
        public List<Table> AvailableTables { get; set; } = new List<Table>();
    }

    public class CreateOrderItemViewModel
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal SubTotal => Price * Quantity;
    }
}
