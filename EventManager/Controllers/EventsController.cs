using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EventManager.Models;
using EventManager.ViewModels;
using PagedList;

namespace EventManager.Controllers
{
  public class EventsController : Controller
  {
    private EventsRegistrationDBEntities db = new EventsRegistrationDBEntities();

    // GET: Events
    public ActionResult Index()
    {
      var events = db.Events.Include(e => e.EventType).Include(e => e.User);
      return View(events.ToList());
    }

    // GET: /Events/IndexById?userId=1 - Show Events for specified UserId
    public ActionResult IndexById(int userId, int? eventTypeId, string search, string sortBy, int? pageUserEvents, int? pageInvitations, int? pagePublicEvents)
    {

      EventListViewModel eventListViewModel = new EventListViewModel();
      eventListViewModel.user = db.Users.Where(u => u.Id == userId).FirstOrDefault();


      if (eventListViewModel.user == null)
      {
        return HttpNotFound();
      }

      // select only events for current user ID if he is participant
      var userEvents =
                      from e in db.Events
                      join p in db.Participants
                      on e.Id equals p.EventId
                      where p.UserId == userId
                      select e;

      // select only events for current user ID where hes invited
      var invitations =
                        from e in db.Events
                        join p in db.Invitations
                        on e.Id equals p.EventId
                        where p.UserId == userId
                        select e;

      // select only events for current user ID where hes not participant and not invited
      var publicEvents = db.Events.Where(e => e.IsPublic == true).Except(userEvents).Except(invitations);
     
      // REFINE IF 'SEARCH' is NOT EMPTY
      if (!string.IsNullOrEmpty(search))
      {
        //TODO: SEARCH
        userEvents = userEvents
                            .Where(e =>
                                  e.Name.ToLower().Contains(search.ToLower())
                                  || e.EventType.Type.ToLower().Contains(search.ToLower())
                                  || e.User.FirstName.ToLower().Contains(search.ToLower())
                                  || e.User.LastName.ToLower().Contains(search.ToLower()));


        invitations = invitations
                              .Where(e =>
                                  e.Name.ToLower().Contains(search.ToLower())
                                  || e.EventType.Type.ToLower().Contains(search.ToLower())
                                  || e.User.FirstName.ToLower().Contains(search.ToLower())
                                  || e.User.LastName.ToLower().Contains(search.ToLower()));

        publicEvents = publicEvents
                            .Where(e =>
                                  e.Name.ToLower().Contains(search.ToLower())
                                  || e.EventType.Type.ToLower().Contains(search.ToLower())
                                  || e.User.FirstName.ToLower().Contains(search.ToLower())
                                  || e.User.LastName.ToLower().Contains(search.ToLower()));

        eventListViewModel.Search = search;
      }


      // REFINE IF CATEGORY NOT EMPTY
      if (eventTypeId!=null && eventTypeId!=0)
      {
        userEvents =
                            from e in
                            userEvents
                            where e.EventType.Id==eventTypeId
                            select e;


        invitations =
                              from e in
                              invitations
                              where e.EventType.Id == eventTypeId
                              select e;

        publicEvents = publicEvents.Where(p => p.EventType.Id == eventTypeId);

        eventListViewModel.eventTypeId = (int)eventTypeId;
      }

      //group search results into categories and count how many items in each category
      // select all public events and all private events to which user is invited to, or is participant to.
      if (eventListViewModel.eventTypesWithCount==null)
      {
      var allUserEvents = userEvents.Concat(invitations).Concat(publicEvents);

      eventListViewModel.eventTypesWithCount =
        (from matchingEvents in allUserEvents
         group matchingEvents by matchingEvents.EventType.Id
        into catGroup
         select new EventTypeWithCount
         {
           EventTypeId = catGroup.Key,
           EventTypeName = db.EventTypes.Where(e => e.Id == catGroup.Key).FirstOrDefault().Type,
           EventsCount = catGroup.Count()// catGroup.Count()
        });
      }

      // SORT THE RESULTS
      switch (sortBy)
      {
        case "date_ascending":
          userEvents = userEvents.OrderBy(d => d.Date);
          invitations = invitations.OrderBy(d => d.Date);
          publicEvents = publicEvents.OrderBy(d => d.Date);
          break;
        case "date_descending":
          userEvents = userEvents.OrderByDescending(d => d.Date);
          invitations = invitations.OrderByDescending(d => d.Date);
          publicEvents = publicEvents.OrderByDescending(d => d.Date);
          break;
        default:
          // sort ascending by default
          userEvents = userEvents.OrderBy(d => d.Date);
          invitations = invitations.OrderBy(d => d.Date);
          publicEvents = publicEvents.OrderBy(d => d.Date);
          break;
      }

      // set the properties in the view model, so they can be passed to the view and back to the viewmodel
      eventListViewModel.pageItems = 3;
      eventListViewModel.pageUserEvents = pageUserEvents ?? 1;
      eventListViewModel.pageInvitations = pageInvitations ?? 1;
      eventListViewModel.pagePublicEvents = pagePublicEvents ?? 1;

      // convert DB data into IPagedList and assign to the viewmodel
      eventListViewModel.userEvents = userEvents.ToPagedList(eventListViewModel.pageUserEvents, eventListViewModel.pageItems);
      eventListViewModel.invitations = invitations.ToPagedList(eventListViewModel.pageInvitations, eventListViewModel.pageItems);
      eventListViewModel.publicEvents = publicEvents.ToPagedList(eventListViewModel.pagePublicEvents, eventListViewModel.pageItems);

      eventListViewModel.SortBy = sortBy;
      eventListViewModel.Sorts = new Dictionary<string, string>
        {
          {"Events that come sooner to the top", "date_ascending" },
          {"Events that come sooner to the bottom", "date_descending" }
        };

      return View(eventListViewModel);
    }

    // GET: Events/Details/5 - Show Events details for specified eventId
    public ActionResult Details(int eventId)
    {
      try
      {
        Event foundEvent = db.Events.Find(eventId);
        EventWithDetailsViewModel eventWithDetails = new EventWithDetailsViewModel();
        eventWithDetails.myEvent = foundEvent;
        eventWithDetails.participants = foundEvent.Participants;

        if (foundEvent == null)
        {
          return HttpNotFound();
        }
        return View(eventWithDetails);

      }
      catch (Exception ex)
      {
        return HttpNotFound(ex.Message);
      }
    }

    // GET: Events/Create
    public ActionResult Create(int? userId)
    {
      if (userId == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }

      Event @event = new Event();
      @event.User = db.Users.Where(u => u.Id == userId).FirstOrDefault();
      @event.ManagerId = @event.User.Id;
      ViewBag.EventTypes = new SelectList(db.EventTypes, "Id", "Type");
      return View(@event);
    }

    // POST: Events/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Id,Name,ManagerId,Date,EventTypeId")] Event @event)
    {
      if (ModelState.IsValid)
      {
        db.Events.Add(@event);
        db.SaveChanges();

        // add record to Participants table, as event manager is also a participant with role="manager"
        Participant participant = new Participant();
        participant.UserId = @event.ManagerId;
        participant.EventId = @event.Id;
        participant.ParticipantTypeId = 1;//manager
        db.Participants.Add(participant);
        db.SaveChanges();

        return RedirectToAction("IndexById", new { userId = @event.ManagerId });
      }

      return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    }

    // GET: Events/Edit/5
    public ActionResult Edit(int eventId, int userId)
    {
      if (eventId <= 0 || userId <= 0)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }

      //TODO: if userId!=event.managerId - quit
      try
      {
        int mngrId = db.Events.Where(e => e.ManagerId == userId).FirstOrDefault().ManagerId;
        if (mngrId != userId)
        {
          return RedirectToAction("IndexById", "Events", new { userId = userId });
        }

        Event @event = db.Events.Where(n => n.Id == eventId).Include(n => n.Participants).Include(n => n.Invitations).Include(n => n.EventType).Include(n => n.User).FirstOrDefault();

        ViewBag.EventTypes = new SelectList(db.EventTypes, "Id", "Type", @event.EventType);

        return View(@event);
      }
      catch (Exception)
      {
        return HttpNotFound(@"Event with id {eventId} and managerId {userId} not found!");
        throw;
      }


    }

    // POST: Events/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,Name,Date,ManagerId,EventTypeId,ParticipantId,IsPublic")] Event @event)
    {
      if (ModelState.IsValid)
      {
        db.Entry(@event).State = EntityState.Modified;
        db.SaveChanges();
        return RedirectToAction("Index");
      }
      ViewBag.EventTypes = new SelectList(db.EventTypes, "Id", "Type", @event.EventType);

      return View(@event);
    }

    // GET: Events/Delete/5

    public ActionResult Delete(int userId, int eventId)
    {
      Event @event = db.Events.Find(eventId);
      if (@event.ManagerId == userId)
      {

        return View(@event);
      }
      return HttpNotFound();
    }

    // POST: Events/Delete/5
    // TODO: only manager should be able to delete event, effectively deleting all the participants record and the event record
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int userId, int eventId)
    {
      if (userId != db.Events.Find(eventId).ManagerId)
      {
        return HttpNotFound();
      }

      //delete all participants with such eventId
      IEnumerable<Participant> participants = db.Participants.Where(p => p.EventId == eventId).ToList();
      foreach (Participant p in participants)
      {
        db.Participants.Remove(p);
        db.SaveChanges();
      }
      // delete event record
      Event ev = db.Events.Where(e => e.Id == eventId).FirstOrDefault();
      db.Events.Remove(ev);
      db.SaveChanges();

      return RedirectToAction("IndexById", "Events", new { userId = userId });
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        db.Dispose();
      }
      base.Dispose(disposing);
    }

    // GET: Events/LeaveEvent
    public ActionResult LeaveEvent(int userId, int eventId)
    {
      // if event manager want's to leave event - it is just as good as deleting event, also deleting all its participants
      if (db.Events.Where(e => e.Id == eventId).FirstOrDefault().ManagerId == userId)
      {
        return RedirectToAction("Delete", "Events");
      }

      // if any other user than manager - just remove him from the participants table and update IndexById view
      try
      {
        // remove record from Invitations
        Participant participant = db.Participants.Where(i => i.EventId == eventId).Where(i => i.UserId == userId).FirstOrDefault();
        db.Participants.Remove(participant);
        db.SaveChanges();

        // redirect to EventsById
        return RedirectToAction("IndexById", "Events", new { userId = userId });
      }
      catch (Exception ex)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
    }


    // GET: Events/Join?userId=2&eventId=1
    public ActionResult JoinEvent(int userId, int eventId)
    {
      // if event is public - OK
      if (db.Events.Find(eventId).IsPublic == true)
      {

        ViewBag.EventId = eventId;//new SelectList(db.Events, "Id", "Name");
        ViewBag.UserId = new SelectList(new List<int> { userId });
        ViewBag.TypeId = new SelectList(db.ParticipantTypes.Where(type => type.Id != 1), "Id", "Type");
        ViewBag.EventName = db.Events.Find(eventId).Name;
        return View();
      }

      return RedirectToAction("IndexById", "Events", new { userId = userId });
    }


    // POST: Events/JoinEvent?userId=387&eventId=88
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult JoinEvent([Bind(Include = "UserId,EventId,ParticipantTypeId")] Participant participant)
    {
      if (ModelState.IsValid)
      {
        db.Participants.Add(participant);
        db.SaveChanges();
        return RedirectToAction("IndexById", "Events", new { userId = participant.UserId });
      }

      return HttpNotFound();
    }


  }
}
