namespace EcommerceMVCAdo.Models
{
    public class OrderItemModel
    {
        public int OrderItemId { get; set; }

        public int OrderId { get; set; }
        public OrderModel Order { get; set; } = null!;

        public int ProductId { get; set; }
        public ProductModel Product { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}
