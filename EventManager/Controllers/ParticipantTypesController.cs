using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EventManager;

namespace EventManager.Controllers
{
    public class ParticipantTypesController : Controller
    {
        private EventsRegistrationDBEntities db = new EventsRegistrationDBEntities();

        // GET: ParticipantTypes
        public ActionResult Index()
        {
            return View(db.ParticipantTypes.ToList());
        }

        // GET: ParticipantTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ParticipantType participantType = db.ParticipantTypes.Find(id);
            if (participantType == null)
            {
                return HttpNotFound();
            }
            return View(participantType);
        }

        // GET: ParticipantTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ParticipantTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Type")] ParticipantType participantType)
        {
            if (ModelState.IsValid)
            {
                db.ParticipantTypes.Add(participantType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(participantType);
        }

        // GET: ParticipantTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ParticipantType participantType = db.ParticipantTypes.Find(id);
            if (participantType == null)
            {
                return HttpNotFound();
            }
            return View(participantType);
        }

        // POST: ParticipantTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Type")] ParticipantType participantType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(participantType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(participantType);
        }

        // GET: ParticipantTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ParticipantType participantType = db.ParticipantTypes.Find(id);
            if (participantType == null)
            {
                return HttpNotFound();
            }
            return View(participantType);
        }

        // POST: ParticipantTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ParticipantType participantType = db.ParticipantTypes.Find(id);
            db.ParticipantTypes.Remove(participantType);
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
