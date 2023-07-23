using Microsoft.AspNetCore.Mvc.Rendering;

namespace Magaz.Models.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }
        public IEnumerable<SelectListItem>? SelectListItems { get; set; }
        public IEnumerable<SelectListItem>? ApplicationSelectList{ get; set; }
    }
}
