using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using travel_agency.DAL;

namespace travel_agency.Models
{
    [Table("Offers")]
    public class Offer
    {
        [Key]
        public int ID { get; set; }
        [Display(Name = "Nazwa Oferty: ")]
        public string NameOffer { get; set; }
        [Display(Name = "Cel Podróży: ")]
        public string TravelDestination { get; set; }
        [Display(Name = "Okres pobytu: ")]
        public LengthOfStay lengthOfStay { get; set; }
        [Display(Name = "kategoria wyjazdu: ")]
        public TripCategory tripCategory { get; set; }
        [StringLength(500, ErrorMessage = "Travel Destination can not be longer than 500 characters.")]
        [Display(Name = "Opis oferty: ")]
        public string TripDescription { get; set; }
        [Display(Name = "liczba miejsc:")]
        public int NumberOfFreePlaces { get; set; }
        [Display(Name = "Zajęte miejsca: ")]
        public int NumberOfOccupiedPlaces { get; set; }
        [Display(Name = "Cena PLN/OS: ")]
        public double PricePerPerson { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Rozpoczęcie wyjazdu: ")]
        public DateTime startDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Zakończenie wyjazdu: ")]
        public DateTime EndDate { get; set; }
        [Display(Name = "Transport: ")]
        public string transport { get; set; }
        [Display(Name = "Zakaterowanie: ")]
        public string accommodation { get; set; }
        public bool AllInclusive { get; set; }

        public string Image { get; set; }
        [ForeignKey("OfferID")]
        public virtual ICollection<Participant> participants { get; set; }

        [ForeignKey("OfferID")]
        public virtual ICollection<Orders> orders { get; set; }

        private travelContext db = new travelContext();

        public int OccupiedPlaces(int NumberOfOccupiedPlaces , int tripID)
        {
            var order = db.Orders.Where(o => o.OfferID.Equals(tripID));
            foreach (var item in order)
            {
                NumberOfOccupiedPlaces = NumberOfOccupiedPlaces + item.NumberOfChildern + item.NumberOfAdult;
            }
            return NumberOfOccupiedPlaces;
        }
    }
    public enum LengthOfStay
    {
        jednodniowy,
        kilkudniowy,
        miesięczny
    }
    public enum TripCategory
    {
       krajowa,
       zagraniczna,
    }

   

}
