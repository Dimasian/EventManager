using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EventManager;

namespace EventManager.Models
{ 
  public partial class EventTypeComparer:IEqualityComparer<EventType>
  {
    public bool Equals(EventType a, EventType b)
    {
      return (a.Id == b.Id);
    }

    public int GetHashCode(EventType a)
    {
      return a.Id ^ a.Id;
    }
  }
}