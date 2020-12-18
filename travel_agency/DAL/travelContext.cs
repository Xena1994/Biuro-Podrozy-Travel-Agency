using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity;
using travel_agency.Models;

namespace travel_agency.DAL
{
    public class travelContext : DbContext
    {
        public travelContext() : base("DefaultConnection")
        {

        }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Topicality> Topicalities { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
      
        }
    }

}