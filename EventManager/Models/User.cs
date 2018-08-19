using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManager
{
  public partial class User
  {

    public string FullName {
      get { return FirstName + " " + LastName; } }
  }
}