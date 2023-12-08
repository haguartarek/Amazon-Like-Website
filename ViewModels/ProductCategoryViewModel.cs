using WebApplication2.Models;

namespace WebApplication2.ViewModels
{
    public class ProductCategoryViewModel 
    {
     
        public IReadOnlyList<Product> Products { get; }
        public IReadOnlyList<Category> Categories { get; }

        public ProductCategoryViewModel(IReadOnlyList<Product> products, IReadOnlyList<Category> categories)
        {
            this.Products = products;
            this.Categories = categories;
        }
        public string GetCategoryName(int categoryId)
        {
            var category = this.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
            if (category == null)
            {
                return "";
            }
            return category.Name;
        }

    }

}
