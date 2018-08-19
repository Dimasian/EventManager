using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace EventManager
{
  [MetadataType(typeof(EventMetaData))]
  public partial class Event
  {
  }

  public class EventMetaData
  {
    [Required]
    [Display(Name = "Event Name")]
    public string Name;

    [Required]
    public System.DateTime Date { get; set; }

    [Required]
    public int ManagerId { get; set; }

    [Required]
    public int EventTypeId { get; set; }


    public bool IsPublic { get; set; }
  }
}