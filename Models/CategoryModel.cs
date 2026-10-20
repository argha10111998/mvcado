using System.ComponentModel.DataAnnotations;

namespace EcommerceMVCAdo.Models
{
    public class CategoryModel
    {

        public int CategoryId { get; set; }
        [Required]
        public string? Name { get; set; }
    }
}
