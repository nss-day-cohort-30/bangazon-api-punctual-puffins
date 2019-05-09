﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class Computer
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public DateTime PurachseDate { get; set; }
        public DateTime DecommisionDate { get; set; }
        [Required]
        [StringLength(55)]
        public String Make { get; set; }
        [Required]
        [StringLength(55)]
        public string Manufacturer { get; set; }
    }
}
