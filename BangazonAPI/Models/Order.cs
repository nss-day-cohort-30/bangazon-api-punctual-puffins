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

        [Required]
        Customer Customer { get; set; }
        PaymentType PaymentType { get; set; }
        List<Product> Products { get; set; } = new List<Product>();
    }
}
