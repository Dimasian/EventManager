using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManager.ViewModels
{
  public class EventWithDetailsViewModel
  {
    public Event myEvent { get; set; }
    public IEnumerable<Participant> participants { get; set; }
  }
}