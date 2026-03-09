namespace RestaurantManagementSystem.Models
{
    public class DashboardViewModel
    {
        public int TotalMenuItems { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSales { get; set; }
        public int TodayOrders { get; set; }
        public int AvailableTables { get; set; }
        public int OccupiedTables { get; set; }
        public List<Order> RecentOrders { get; set; } = new List<Order>();
        public decimal TodaySales { get; set; }
    }
}
