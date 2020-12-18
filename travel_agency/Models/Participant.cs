using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace travel_agency.Models
{
    [Table("Participants")]
    public class Participant
    {
        [Key]
        public int ID { get; set; }
        [Display(Name = "Nazwa Oferty: ")]
        public int OfferID { get; set; }
        [Display(Name = "Imię: ")]
        public string Name { get; set; }
        [StringLength(40, ErrorMessage = "Surname can not be longer than 40 characters.")]
        [Display(Name = "Nazwisko: ")]
        public string Surname { get; set; }
        [StringLength(40, ErrorMessage = "City can not be longer than 40 characters.")]
        [Display(Name = "Miasto: ")]
        public string City { get; set; }
        [StringLength(40, ErrorMessage = "City can not be longer than 40 characters.")]
        [Display(Name = "Ulica: ")]
        public string Street { get; set; }
        [Display(Name = "Numer Domu: ")]
        public int NumberOfHouse { get; set; }
        [Display(Name = "Wiek: ")]
        public int Age { get; set; }
        [ForeignKey("OfferID")]
        public virtual Offer offer { get; set; }


    }
}