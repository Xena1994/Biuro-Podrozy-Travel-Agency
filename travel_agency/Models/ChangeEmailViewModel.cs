using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace travel_agency.Models
{
    public class ChangeEmailViewModel
    {
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "UserName")]
        [DataType(DataType.EmailAddress)]
        public string NewAddress { get; set; }
    }
}