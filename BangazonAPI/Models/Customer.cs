using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BangazonAPI.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        List<Product> SellingProducts { get; set; } = new List<Product>();
        List<PaymentType> PaymentTypes { get; set; } = new List<PaymentType>();
    }
}