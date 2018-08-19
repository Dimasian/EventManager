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
  public class InvitationsController : Controller
  {
    private EventsRegistrationDBEntities db = new EventsRegistrationDBEntities();

    // GET: Invitations
    public ActionResult Index()
    {
      var invitations = db.Invitations.Include(i => i.Event).Include(i => i.ParticipantType).Include(i => i.User);
      return View(invitations.ToList());
    }

    // GET: Invitations/Details/5
    public ActionResult Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Invitation invitation = db.Invitations.Find(id);
      if (invitation == null)
      {
        return HttpNotFound();
      }
      return View(invitation);
    }

    // GET: Invitations/Create
    public ActionResult Create(int userId, int eventId)
    {
      Invitation invitation = new Invitation();
      invitation.UserId = userId;
      invitation.EventId = eventId;
      invitation.Date = db.Events.Where(e => e.Id == eventId).FirstOrDefault().Date;
      ViewBag.ParticipantTypes = new SelectList(db.ParticipantTypes.Where(p => p.Id != 1), "Id", "Type");
      // user email will be sent from View to [HttpPost] Create method via model

      return View(invitation);
    }

    // POST: Invitations/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "EventId,Email,ParticipantTypeId")] Invitation invitation)
    {
      // Set invitation date
      invitation.Date = DateTime.Now;
      // Does invited user exist in the DB? If yes, set his id, otherwise -> null.
      User findUser = db.Users.Where(u => u.Email == invitation.Email).FirstOrDefault();
      invitation.UserId = findUser == null ? default(int?) : findUser.Id;

      // event manager cannot invite himself to the meeting - otherwise duplicate records to follow
      if (findUser != null)
      {
        if (db.Events.Where(e => e.Id == invitation.EventId).FirstOrDefault().ManagerId == findUser.Id)
        {
          return RedirectToAction("IndexById", "Events");
        }
      }

      if (ModelState.IsValid)
      {
        db.Invitations.Add(invitation);
        db.SaveChanges();
        return RedirectToAction("Details", "Events", new { eventId = invitation.EventId });
      }

      return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    }

    // ACCEPT INVITATION
    public ActionResult AcceptInvitation(int userId, int eventId)
    {
      try
      {
        // add record to Participants
        Participant participant = new Participant();
        participant.UserId = userId;
        participant.EventId = eventId;
        participant.ParticipantTypeId = db.Invitations.Where(i => i.UserId == userId).FirstOrDefault().ParticipantTypeId;
        db.Participants.Add(participant);
        db.SaveChanges();

        // remove record from Invitations
        Invitation invitation =
          db.Invitations.Where(i => i.EventId == eventId)
          .Where(i => i.Email == db.Users.Where(u => u.Id == userId).FirstOrDefault().Email).FirstOrDefault();
        db.Invitations.Remove(invitation);
        db.SaveChanges();

        // redirect to EventsById
        return RedirectToAction("IndexById", "Events", new { userId = userId });
      }
      catch (Exception ex)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }

    }

    // ACCEPT INVITATION
    public ActionResult DeclineInvitation(int userId, int eventId)
    {
      try
      {
        // remove record from Invitations
        Invitation invitation =
          db.Invitations.Where(i => i.EventId == eventId)
          .Where(i => i.Email == db.Users.Where(u => u.Id == userId).FirstOrDefault().Email).FirstOrDefault();
        db.Invitations.Remove(invitation);
        db.SaveChanges();

        // redirect to EventsById
        return RedirectToAction("IndexById", "Events", new { userId = userId });
      }
      catch (Exception ex)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }

    }



    // GET: Invitations/Edit/5
    public ActionResult Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Invitation invitation = db.Invitations.Find(id);
      if (invitation == null)
      {
        return HttpNotFound();
      }
      ViewBag.EventId = new SelectList(db.Events, "Id", "Name", invitation.EventId);
      ViewBag.ParticipTypeId = new SelectList(db.ParticipantTypes, "Id", "Type", invitation.ParticipantTypeId);
      ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", invitation.UserId);
      return View(invitation);
    }

    // POST: Invitations/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,EventId,UserId,Date,ParticipTypeId")] Invitation invitation)
    {
      if (ModelState.IsValid)
      {
        db.Entry(invitation).State = EntityState.Modified;
        db.SaveChanges();
        return RedirectToAction("Index");
      }
      ViewBag.EventId = new SelectList(db.Events, "Id", "Name", invitation.EventId);
      ViewBag.ParticipTypeId = new SelectList(db.ParticipantTypes, "Id", "Type", invitation.ParticipantTypeId);
      ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", invitation.UserId);
      return View(invitation);
    }

    // GET: Invitations/Delete/5
    public ActionResult Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Invitation invitation = db.Invitations.Find(id);
      if (invitation == null)
      {
        return HttpNotFound();
      }
      return View(invitation);
    }

    // POST: Invitations/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
      Invitation invitation = db.Invitations.Find(id);
      db.Invitations.Remove(invitation);
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
