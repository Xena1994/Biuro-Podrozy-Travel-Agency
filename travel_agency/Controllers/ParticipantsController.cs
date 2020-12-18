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
using _Excel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace travel_agency.Controllers
{
    public class ParticipantsController : Controller
    {
        private travelContext db = new travelContext();

        // GET: Participants
        public ActionResult Index(string sortOrder)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.OfferSortParm = String.IsNullOrEmpty(sortOrder) ? "offer_desc" : "";
            ViewBag.DoubleSortParm = sortOrder == "Double" ? "double_desc" : "Double";
            var participants = from a in db.Participants select a;
            switch (sortOrder)
            {
                case "offer_desc":
                    participants = participants.OrderByDescending(a => a.offer.NameOffer);
                    break;
                case "offer":
                    participants = participants.OrderBy(a => a.offer.NameOffer);
                    break;
                case "double_desc":
                    participants = participants.OrderByDescending(a => a.Age);
                    break;
                case "Double":
                    participants = participants.OrderBy(a => a.Age);
                    break;
                case "name_desc":
                    participants = participants.OrderByDescending(a => a.Surname);
                    break;
                default:
                    participants = participants.OrderBy(a => a.Surname);
                    break;
            }

            return View(participants.ToList());
        }

        // GET: Participants/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participant participant = db.Participants.Find(id);
            if (participant == null)
            {
                return HttpNotFound();
            }
            return View(participant);
        }
        [HttpGet]
        public ActionResult OpenFile()
        {
            return View();
        }
        [HttpPost]
        public ActionResult OpenFile(HttpPostedFileBase excelFile)
        {
            try
            {
                string filename = Path.GetExtension(excelFile.FileName);
                if (excelFile != null && (filename.EndsWith(".xls") || filename.EndsWith(".xlsx")))
                {
                    string path = Server.MapPath("~/Content/") + Guid.NewGuid() + filename;
                    excelFile.SaveAs(path);
                    _Excel.Application application = new _Excel.Application();
                    _Excel.Workbook workbook = application.Workbooks.Open(path);
                    _Excel.Worksheet worksheet = workbook.ActiveSheet;
                    _Excel.Range range = worksheet.UsedRange;
                    List<Participant> participants = new List<Participant>();
                    for (int row = 2; row <= range.Rows.Count; row++)
                    {
                        Participant p = new Participant();
                        p.OfferID= int.Parse(((Microsoft.Office.Interop.Excel.Range)range.Cells[row, 1]).Text);
                        p.Name = ((_Excel.Range)range.Cells[row, 2]).Text;
                        p.Surname = ((_Excel.Range)range.Cells[row, 3]).Text;
                        p.City = ((_Excel.Range)range.Cells[row, 4]).Text;
                        p.Street = ((_Excel.Range)range.Cells[row, 5]).Text;
                        p.NumberOfHouse = int.Parse(((Microsoft.Office.Interop.Excel.Range)range.Cells[row, 6]).Text);
                        p.Age = int.Parse(((Microsoft.Office.Interop.Excel.Range)range.Cells[row, 7]).Text);
                        participants.Add(p);

                    }
                    foreach (var item in participants)
                    {
                        db.Participants.Add(item);
                    }

                    db.SaveChanges();
                    workbook.Close(0);
                    application.Quit();
                    return RedirectToAction("Index");
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Nie znaleziono pliku" + e);
            }
            var parti = from a in db.Participants select a;
            return View(parti.ToList());
        }
        // GET: Participants/Create
        public ActionResult Create()
        {
            ViewBag.OfferID = new SelectList(db.Offers, "ID", "NameOffer");
            return View();
        }

        // POST: Participants/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,OfferID,Name,Surname,City,Street,NumberOfHouse,Age")] Participant participant)
        {
            if (ModelState.IsValid)
            {
                db.Participants.Add(participant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OfferID = new SelectList(db.Offers, "ID", "NameOffer", participant.OfferID);
            return View(participant);
        }

        // GET: Participants/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participant participant = db.Participants.Find(id);
            if (participant == null)
            {
                return HttpNotFound();
            }
            ViewBag.OfferID = new SelectList(db.Offers, "ID", "NameOffer", participant.OfferID);
            return View(participant);
        }

        // POST: Participants/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,OfferID,Name,Surname,City,Street,NumberOfHouse,Age")] Participant participant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(participant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OfferID = new SelectList(db.Offers, "ID", "NameOffer", participant.OfferID);
            return View(participant);
        }

        // GET: Participants/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participant participant = db.Participants.Find(id);
            if (participant == null)
            {
                return HttpNotFound();
            }
            return View(participant);
        }

        // POST: Participants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Participant participant = db.Participants.Find(id);
            db.Participants.Remove(participant);
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
