using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using travel_agency.DAL;

namespace travel_agency.Models
{
    
    [Table("Orders")]
    public class Orders
    {
        [Key]
        public int ID { get; set; }
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Nazwa użytkownika: ")]
        public string UserName { get; set; }
        public int OfferID { get; set; }
        [Range(0, 50)]
        [Display(Name = "liczba dzieci: ")]
        public int NumberOfChildern { get; set; }
        [Range(0, 50)]
        [Display(Name = "liczba dorosłych: ")]
        public int NumberOfAdult { get; set; }
        public Status status { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data Transakcji: ")]
        public string TransactionDate { get; set; }
        [Display(Name = "Cena PLN/OS: ")]
        public double costs { get; set; }
        [ForeignKey("OfferID")]
        public virtual Offer offer { get; set; }
   
        public virtual Profile profile { get; set; }
        public enum Status
        {
            nieopłacone,
            opłacone
        }
        private travelContext db = new travelContext();

        public double CostOFTheTrip(int NumberOfChildern, int NumberOfAdult , int OfferID)
        {
            Offer o = db.Offers.Single(p => p.ID.Equals(OfferID));
            return (NumberOfChildern * o.PricePerPerson * 0.5) + (NumberOfAdult * o.PricePerPerson);
        }
        public int SearchID(string userName)
        {
            Profile user = db.Profiles.Single(o => o.UserName.Equals(userName));
            return user.ID;
        }
    }
}