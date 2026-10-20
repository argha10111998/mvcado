namespace EcommerceMVCAdo.Models
{
    public class CategoryFormModel
    {
        public CategoryModel CatModel { get; set; } = new CategoryModel();
        public List<CategoryModel> Categories { get; set; } = new List<CategoryModel>();
    }
}
