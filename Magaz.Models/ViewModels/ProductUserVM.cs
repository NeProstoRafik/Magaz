using Microsoft.AspNetCore.Identity;

namespace Magaz.Models.ViewModels
{
    public class ProductUserVM
    {
        public ProductUserVM()
        {
            ProductList=new List<Product>();
        }
        public IdentityUser ApplicationUser { get; set; }
        //public ApplicationUser ApplicationUser { get; set; }
        public IList<Product> ProductList { get; set; }
       
    }
}
