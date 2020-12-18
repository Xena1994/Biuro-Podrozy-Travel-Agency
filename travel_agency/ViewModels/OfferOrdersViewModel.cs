using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using travel_agency.Models;
using System.Collections;
namespace travel_agency.ViewModels
{
    public class OfferOrdersViewModel 
    {
        public IEnumerable<Orders> OrdersVME { get; set; }
        public Offer OfferVM { get; set; }
        public Orders  OrdersVM { get; set; }

    }
    
}