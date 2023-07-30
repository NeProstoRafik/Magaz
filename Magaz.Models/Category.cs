using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Magaz.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [DisplayName("Заказ")]
        [Required]
        [Range(1, int.MaxValue,ErrorMessage ="от 1 до хз")]
        public int DisplayOrder  { get; set; }

     

    }
}
