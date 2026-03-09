using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagementSystem.Models
{
    public class Bill
    {
        [Key]
        public int BillID { get; set; }

        [Required(ErrorMessage = "Order is required")]
        [ForeignKey("Order")]
        public int OrderID { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal SubTotal { get; set; }

        [Required]
        [Column(TypeName = "TaxAmount")]
        public decimal Tax { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal FinalAmount { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string PaymentMethod { get; set; } = "Cash"; // Cash / Card / Online

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime BillDate { get; set; } = DateTime.Now;

        // Navigation property
        public virtual Order? Order { get; set; }
    }
}
