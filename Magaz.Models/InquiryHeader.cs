using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magaz.Models
{
    public class InquiryHeader
    {
        [Key]
        public int id { get; set; }
        public string ApllicationUserId { get; set; }
        [ForeignKey("ApllicationUserId")]
        public IdentityUser apllicationUser { get; set; }
        public DateTime InquiryDate { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
