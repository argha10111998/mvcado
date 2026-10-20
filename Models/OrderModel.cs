namespace EcommerceMVCAdo.Models
{
    public class OrderModel
    {
        public int OrderId { get; set; }

        public int UserId { get; set; }
        public UserModel User { get; set; } = null!;

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public List<OrderItemModel> OrderItems { get; set; } = new();
    }
}
