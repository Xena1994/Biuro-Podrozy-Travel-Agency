using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using travel_agency.DAL;
using travel_agency.Models;
using travel_agency.ViewModels;
using PagedList;

namespace travel_agency.Controllers
{
    public class OffersController : Controller
    {
        private travelContext db = new travelContext();

         // GET: Offers
       [HttpGet]
        public ActionResult Index(string sortOrder, string searchString, string price, string fromDate, string toDate, int? page, string currentFilter)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.OfferSortParm = sortOrder == "Offer" ? "offer_desc" : "Offer";
            ViewBag.DoubleSortParm = sortOrder == "Double" ? "double_desc" : "Double";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.DatesSortParm = sortOrder == "Dates" ? "dates_desc" : "Dates";
            var offers = from a in db.Offers select a;
            if (!String.IsNullOrEmpty(searchString) && !String.IsNullOrEmpty(price) && !String.IsNullOrEmpty(fromDate) && !String.IsNullOrEmpty(toDate))
            {
                DateTime startTime = DateTime.Parse(fromDate);
                DateTime endTime = DateTime.Parse(toDate);
                double pricePerPerson = double.Parse(price);
                offers = db.Offers.Where(a => a.TravelDestination.Contains(searchString) && a.startDate >= startTime && a.EndDate <= endTime && a.PricePerPerson <= pricePerPerson || a.NameOffer.Contains(searchString) && a.startDate >= startTime && a.EndDate <= endTime && a.PricePerPerson <= pricePerPerson);
            }

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            switch (sortOrder)
            {
                case "offer_desc":
                    offers = offers.OrderByDescending(a => a.NameOffer);
                    break;
                case "Offer":
                    offers = offers.OrderBy(a => a.NameOffer);
                    break;
                case "double_desc":
                    offers = offers.OrderByDescending(a => a.PricePerPerson);
                    break;
                case "Double":
                    offers = offers.OrderBy(a => a.PricePerPerson);
                    break;
                case "name_desc":
                    offers = offers.OrderByDescending(a => a.TravelDestination);
                    break;
                case "Date":
                    offers = offers.OrderBy(a => a.startDate);
                    break;
                case "date_desc":
                    offers = offers.OrderByDescending(a => a.startDate);
                    break;
                case "Dates":
                    offers = offers.OrderBy(a => a.EndDate);
                    break;
                case "dates_desc":
                    offers = offers.OrderByDescending(a => a.EndDate);
                    break;

                default:
                    offers = offers.OrderBy(a => a.TravelDestination);
                    break;


            }
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(offers.ToPagedList(pageNumber, pageSize));
        }
        // GET: Offers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Offer offer = db.Offers.Find(id);
            if (offer == null)
            {
                return HttpNotFound();
            }
            Orders orders = new Orders
            {
                OfferID = offer.ID
            };
            IEnumerable<Orders> ordersEn = db.Orders.Where(o => o.OfferID.Equals(offer.ID));

            OfferOrdersViewModel viewModel = new OfferOrdersViewModel
            {
                OfferVM = offer,
                OrdersVM = orders,
                OrdersVME = ordersEn

            };
            return View(viewModel);
        }

        // GET: Offers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Offers/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NameOffer,TravelDestination,lengthOfStay,tripCategory,TripDescription,NumberOfFreePlaces,NumberOfOccupiedPlaces,PricePerPerson,startDate,EndDate,transport,accommodation,AllInclusive,Image")] Offer offer)
        {
            if (ModelState.IsValid)
            {
                UpdateModel(offer);
                HttpPostedFileBase file = Request.Files["fileWithImage"];
                if (file != null && file.ContentLength > 0)
                {
                    offer.Image = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    file.SaveAs(HttpContext.Server.MapPath("~/Images/") + offer.Image);
                }
                db.Offers.Add(offer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.NumberOfOccupiedPlaces = offer.OccupiedPlaces(offer.NumberOfOccupiedPlaces, offer.ID);
            return View(offer);
        }

        // GET: Offers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Offer offer = db.Offers.Find(id);
            if (offer == null)
            {
                return HttpNotFound();
            }
            return View(offer);
        }

        // POST: Offers/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NameOffer,TravelDestination,lengthOfStay,tripCategory,TripDescription,NumberOfFreePlaces,NumberOfOccupiedPlaces,PricePerPerson,startDate,EndDate,transport,accommodation,AllInclusive,Image")] Offer offer)
        {
            if (ModelState.IsValid)
            {
                UpdateModel(offer);
                HttpPostedFileBase file = Request.Files["fileWithImage"];
                if (file != null && file.ContentLength > 0)
                {
                    offer.Image = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    file.SaveAs(HttpContext.Server.MapPath("~/Images/") + offer.Image);
                }
                db.Entry(offer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.NumberOfOccupiedPlaces = offer.OccupiedPlaces(offer.NumberOfOccupiedPlaces, offer.ID);
            return View(offer);
        }


        // GET: Offers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Offer offer = db.Offers.Find(id);
            if (offer == null)
            {
                return HttpNotFound();
            }
            return View(offer);
        }

        // POST: Offers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Offer offer = db.Offers.Find(id);
            db.Offers.Remove(offer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
