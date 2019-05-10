using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class Employee
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(55)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(55)]
        public string LastName { get; set; }
        //public int Supervisor { get; set; } = 0;
        Department Department { get; set; }
        Computer Computer { get; set; }


    }

}
