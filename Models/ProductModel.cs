namespace EcommerceMVCAdo.Models
{
    public class ProductModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public decimal ProductPrice { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }

    }
}
