using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class Order
    {
        [Required]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int PaymentTypeId { get; set; }

        [Required]
        public Customer Customer { get; set; }
        public PaymentType PaymentType { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
