using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using travel_agency.DAL;
using travel_agency.Models;
using System.Net.Mail;

namespace travel_agency.Controllers
{
    public class OrdersController : Controller
    {
        private travelContext db = new travelContext();

        // GET: Orders
        public ActionResult Index(string sortOrder)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DoubleSortParm = sortOrder == "Double" ? "double_desc" : "Double";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            var orders = from a in db.Orders select a;
            switch (sortOrder)
            {
                case "double_desc":
                    orders = orders.OrderByDescending(a => a.costs);
                    break;
                case "Double":
                    orders = orders.OrderBy(a => a.costs);
                    break;
                case "name_desc":
                    orders = orders.OrderByDescending(a => a.UserName);
                    break;
                case "Date":
                    orders = orders.OrderBy(a => a.TransactionDate);
                    break;
                case "date_desc":
                    orders = orders.OrderByDescending(a => a.TransactionDate);
                    break;
                default:
                    orders = orders.OrderBy(a => a.UserName);
                    break;


            }
            if (User.IsInRole("Admin"))
            {
                return View(orders.ToList());
            }
            else
            {
                return View(orders.Where(o => o.UserName == User.Identity.Name).ToList());
            }
                
        }

        public ActionResult Create()
        {
            ViewBag.OfferID = new SelectList(db.Offers, "ID", "NameOffer");
            return View();
        }
        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }
        [HttpPost]
        public ActionResult SendConfirmMessage(Orders orders)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var senderEmail = new MailAddress("random");
                    var receiverEmail = new MailAddress(orders.UserName, "Receiver");
                    var password = "random";
                    Profile profile = db.Profiles.Single(p => p.UserName.Equals(orders.UserName));
                    var subject = "Potwierdzenie Zamówienia";
                    var body = $"Szanowny Kliencie! " + "\n" +
                        $"Przesyłam potwierdzenie zamówienia na wycieczkę {orders.offer.NameOffer}  " +
                        $"dla dzieci : {orders.NumberOfChildern} oraz {orders.NumberOfAdult} osób dorosłych." + "\n" +
                        $"Koszt w sumie za wszystkich który Państwo uiścili to  {orders.costs} PLN." + $"W razie wszelkich pytań porosimy o kontakt." + "\n" + "\n" +
                        $"Prosimy o przesłanie Imion , nazwisk , adresu i wieku osób towarzyszących aby dodać je do listy uczestników." + "\n" + "\n" +
                        $"Spotykamy się zawsze pod naszym biurem o godzinie 8 skąd ruszają nasze wycieczki." + "\n" + "\n" +
                        $"Miłego wypoczynku życzy Biuro podróży Veracruz!" + "\n" + "\n" +
                        $"____________________________FAKTURA___________________________________" + "\n" +
                        $"Pan/ Pani :  {profile.Name}  {profile.Surname}" + "\n" +
                        $"zamieszkały/ła w {profile.City}" + "\n" +
                        $"Zamówienie na wyjazd do : {orders.offer.TravelDestination} " + "\n" +
                        $"Nazwa oferty:  {orders.offer.NameOffer}" + "\n" +
                        $"W terminie od dnia {orders.offer.startDate.ToString("dd/MM/yyyy")} do dnia {orders.offer.EndDate.ToString("dd/MM/yyyy")}" + "\n" +
                        $"Osoby dorosłe : {orders.NumberOfAdult}" + "\n" +
                        $"Dzieci : {orders.NumberOfChildern}" + "\n" +
                        $"Koszt : {orders.costs} PLN";

                    var smtp = new SmtpClient
                    {
                        Host = "poczta.int.pl",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(senderEmail.Address, password)
                    };
                    using (var mess = new MailMessage(senderEmail, receiverEmail)
                    {
                        Subject = subject,
                        Body = body
                    })
                    {
                        smtp.Send(mess);
                    }
                    return View();
                }
            }
            catch (Exception)
            {
                ViewBag.Error = "Some Error";
            }
            return View();
        }

        // GET: Orders/Create


        // POST: Orders/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Prefix = "OrdersVM", Include = "ID,OfferID,NumberOfChildern,NumberOfAdult,status,TransactionDate,costs")] Orders orders)
        {
            if (ModelState.IsValid)
            {
                orders.status = Orders.Status.nieopłacone;
                orders.UserName = User.Identity.Name;
                orders.TransactionDate = DateTime.Now.ToString("dd/MM/yyyy");
                orders.costs = orders.CostOFTheTrip(orders.NumberOfChildern, orders.NumberOfAdult, orders.OfferID);
                orders.offer = db.Offers.Single(o => o.ID.Equals(orders.OfferID));
                orders.offer.NumberOfOccupiedPlaces = orders.offer.NumberOfOccupiedPlaces + orders.NumberOfAdult + orders.NumberOfChildern;
                db.Orders.Add(orders);
                db.Entry(orders.offer).State = EntityState.Modified;
                db.SaveChanges();

               
                return RedirectToAction("Index");
            }

            ViewBag.OfferID = new SelectList(db.Offers, "ID", "NameOffer", orders.OfferID);
            ViewBag.costs = orders.CostOFTheTrip(orders.NumberOfChildern, orders.NumberOfAdult, orders.OfferID);
            ViewBag.profile.ID = orders.SearchID(orders.UserName);
            return View(orders);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            ViewBag.OfferID = new SelectList(db.Offers, "ID", "NameOffer", orders.OfferID);
            return View(orders);
        }

        // POST: Orders/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserName,OfferID,NumberOfChildern,NumberOfAdult,status,TransactionDate,costs")] Orders orders)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orders).State = EntityState.Modified;
                orders.UserName = User.Identity.Name;
                orders.TransactionDate = DateTime.Now.ToString("dd/MM/yyyy");
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OfferID = new SelectList(db.Offers, "ID", "NameOffer", orders.OfferID);
            return View(orders);
        }
        public ActionResult Pay([Bind(Include = "ID")] Orders orders)
        {
            if (ModelState.IsValid)
            {
                orders = db.Orders.Single(o => o.ID.Equals(orders.ID));
                db.Entry(orders).State = EntityState.Modified;
                orders.status = Orders.Status.opłacone;
                db.SaveChanges();
                SendConfirmMessage(orders);
                return RedirectToAction("Index");
            }
            return View(orders);
        }
        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Orders orders = db.Orders.Find(id);
            orders.offer.NumberOfOccupiedPlaces = orders.offer.NumberOfOccupiedPlaces - (orders.NumberOfChildern + orders.NumberOfAdult);
            db.Orders.Remove(orders);
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
