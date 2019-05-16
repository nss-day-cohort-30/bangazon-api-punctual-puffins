using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class PaymentType
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int AcctNumber { get; set; }
        [Required]
        [StringLength(55)]
        public string Name { get; set; }

    }
}
