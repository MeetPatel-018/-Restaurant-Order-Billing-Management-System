using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagementSystem.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemID { get; set; }

        [Required(ErrorMessage = "Order is required")]
        [ForeignKey("Order")]
        public int OrderID { get; set; }

        [Required(ErrorMessage = "Menu item is required")]
        [ForeignKey("MenuItem")]
        public int ItemID { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 999, ErrorMessage = "Quantity must be between 1 and 999")]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal SubTotal { get; set; }

        // Navigation properties
        public virtual Order Order { get; set; } = null!;
        public virtual MenuItem MenuItem { get; set; } = null!;
    }
}
