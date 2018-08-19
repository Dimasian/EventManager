using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManager.ViewModels
{
  public class UsersIndexViewModel
  {
    public IQueryable<User> Users { get; set; }
  }
}