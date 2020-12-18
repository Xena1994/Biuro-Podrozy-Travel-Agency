using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using travel_agency.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace travel_agency.DAL
{
    public class Initializer : DropCreateDatabaseIfModelChanges<travelContext>
    {
      
        protected override void Seed(travelContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            roleManager.Create(new IdentityRole("Admin"));

            var user = new ApplicationUser { UserName = "random" };
            string password = "random";

            userManager.Create(user, password);

            userManager.AddToRole(user.Id, "Admin");

            var profiles = new List<Profile>
            {
                new Profile
                {
                    UserName = "random",
                    Password = "random",
                    Name = "random",
                    Surname = "random",
                    City = "random"
                }

            };
            profiles.ForEach(o => context.Profiles.Add(o));
            context.SaveChanges();

            var offers = new List<Offer>
            {
                new Offer
                {
                              NameOffer = "Słoneczne Ateny" ,
                              TravelDestination = "Grecja",
                              lengthOfStay =  LengthOfStay.kilkudniowy,
                              tripCategory =  TripCategory.zagraniczna,
                              TripDescription = "Słoneczne Wakacje w Atenach dla całej rodziny ! Kto chętny zwiedzić olimp i spotkać Zeusa? Atene? " +
                              "Zapraszamy do odwiedzenia mitycznych Aten gdzie " +
                              "narodziły się demokracja, teatr i olimpiady pod okiem naszych Przewodników!",
                              NumberOfFreePlaces = 30,
                              NumberOfOccupiedPlaces =0,
                              PricePerPerson = 1400,
                              startDate = DateTime.Parse("20-06-2020"),
                              EndDate = DateTime.Parse("26-06-2020"),
                              transport = "lecimy samolotem linii : https://www.skyscanner.pl/loty-do/ath/tanie-loty-do-ateny-international-lotnisko.html",
                              accommodation =  " hotel : https://www.eliaermouhotel.com/ " ,
                              AllInclusive = true,
                              Image = "ateny2.jpg"
                },
                new Offer
                {
                              NameOffer = "Paryż" ,
                              TravelDestination = "Francja",
                              lengthOfStay =  LengthOfStay.kilkudniowy,
                              tripCategory =  TripCategory.zagraniczna,
                              TripDescription = "Któż chętny zobaczyć jedną z najpiękniejszych stolic sztuki gdzie znajdziecie wieżę Aifla? Zapraszamy do Paryża!",
                              NumberOfFreePlaces = 30,
                              NumberOfOccupiedPlaces =0,
                              PricePerPerson = 1200,
                              startDate = DateTime.Parse("25-06-2020"),
                              EndDate = DateTime.Parse("30-06-2020"),
                              transport = "lecimy samolotem https://tiny.pl/ts34d",
                              accommodation =  " hotel : https://www.agoda.com/pl-pl/metropol-hotel/hotel/paris-fr.html?cid=-42",
                              AllInclusive = false,
                              Image = "paris.jpg"
                }
            };
            offers.ForEach(o => context.Offers.Add(o));
            context.SaveChanges(); 


        }
    }

}