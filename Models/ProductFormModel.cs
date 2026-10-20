namespace EcommerceMVCAdo.Models
{
    public class ProductFormModel
    {
        public ProductModel ProductModel { get; set; } = new ProductModel();
        public List<CategoryModel> Categories { get; set; } = new List<CategoryModel>();
    }
}
