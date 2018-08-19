using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace EventManager.ViewModels
{
  public class EventListViewModel
  {
    public User user { get; set; }
    public IPagedList<Event> userEvents { get; set; }
    public IPagedList<Event> invitations { get; set; }
    public IPagedList<Event> publicEvents { get; set; }
    public string SortBy { get; set; }
    public Dictionary<string, string> Sorts { get; set; }
    public string Search { get; set; }

    //paging
    public int pageItems { get; set; }
    public int pageUserEvents { get; set; }
    public int pageInvitations{ get; set; }
    public int pagePublicEvents { get; set; }


    public IEnumerable<EventTypeWithCount> eventTypesWithCount{ get; set; }
    public int eventTypeId { get; set; }// for filtering by event type
    public IEnumerable<SelectListItem> EventTypesFilterItems
    {
      get
      {
        var allEventTypes = eventTypesWithCount.Select(cc => new SelectListItem
        { 
          Value = cc.EventTypeId.ToString(),
          Text = cc.EventTypeNameWithCount
        });
        return allEventTypes;
      }
    }

  }


  // Event types with count
  public class EventTypeWithCount
  {
    public int EventsCount { get; set; }
    public string EventTypeName { get; set; }
    public int EventTypeId { get; set; }
    public string EventTypeNameWithCount
    {
      get
      {
        return EventTypeName + " (" + EventsCount.ToString() + ")";
      }
    }
  }
}