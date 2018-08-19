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
  public class ParticipantsController : Controller
  {
    private EventsRegistrationDBEntities db = new EventsRegistrationDBEntities();

    // GET: Participants
    public ActionResult Index()
    {
      var participants = db.Participants.Include(p => p.Event).Include(p => p.ParticipantType).Include(p => p.User);
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

    // GET: Participants/Create - create participant for specific event
    public ActionResult Create(int? eventId)
    {
      if (eventId == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      ViewBag.EventId = eventId;//new SelectList(db.Events, "Id", "Name");
      ViewBag.TypeId = new SelectList(db.ParticipantTypes, "Id", "Type");

      // пока для приглашения нового участника доступен весь перечень пользователей
      // потом нужно ограничить его перечнем "друзей" менедежера данного события
      // или осуществлять приглашение по адресу эл.почты
      ViewBag.UserId = new SelectList(db.Users, "Id", "FullName");
      ViewBag.EventName = db.Events.Where(e => e.Id == eventId).Single().Name;
      return View();
    }

    

    // POST: Participants/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Id,UserId,EventId,TypeId")] Participant participant)
    {
      if (ModelState.IsValid)
      {
        db.Participants.Add(participant);
        db.SaveChanges();
        return RedirectToAction("Details", "Events", new { eventId = participant.EventId });
      }

      ViewBag.EventId = new SelectList(db.Events, "Id", "Name", participant.EventId);
      ViewBag.TypeId = new SelectList(db.ParticipantTypes, "Id", "Type", participant.ParticipantTypeId);
      ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", participant.UserId);

      return RedirectToAction("Index", "Events");
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
      ViewBag.EventId = new SelectList(db.Events, "Id", "Name", participant.EventId);
      ViewBag.TypeId = new SelectList(db.ParticipantTypes, "Id", "Type", participant.ParticipantTypeId);
      ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", participant.UserId);
      return View(participant);
    }

    // POST: Participants/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,UserId,EventId,TypeId")] Participant participant)
    {
      if (ModelState.IsValid)
      {
        db.Entry(participant).State = EntityState.Modified;
        db.SaveChanges();
        return RedirectToAction("Index");
      }
      ViewBag.EventId = new SelectList(db.Events, "Id", "Name", participant.EventId);
      ViewBag.TypeId = new SelectList(db.ParticipantTypes, "Id", "Type", participant.ParticipantTypeId);
      ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", participant.UserId);
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
