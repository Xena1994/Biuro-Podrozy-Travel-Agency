using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace travel_agency.Models
{

    [Table("Profiles")]
    public class Profile
    {
        [Key]
        public int ID { get; set; }
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Nazwa użytkownika: ")]
        public string UserName { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        [StringLength(40, ErrorMessage = "Name can not be longer than 40 characters.")]
        [Display(Name = "Imię: ")]
        public string Name { get; set; }
        [StringLength(40, ErrorMessage = "Surname can not be longer than 40 characters.")]
        [Display(Name = "Nazwisko: ")]
        public string Surname { get; set; }
        [StringLength(40, ErrorMessage = "City can not be longer than 40 characters.")]
        [Display(Name = "Miasto: ")]
        public string City { get; set; }
        public virtual ICollection<Orders> orders { get; set; }
    
    }
}