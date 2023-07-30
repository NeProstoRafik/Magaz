namespace Magaz.Models.ViewModels
{
    public class DetailsVM
    {
        public DetailsVM()
        {
                Product product = new Product();
        }
        public Product Product { get; set; }
        public bool IsExist { get; set; }
    }
}
