using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventManager.ViewModels
{
  public class UserViewModel
  {
    public int ID { get; set; }

    [Required(ErrorMessage = "The user name cannot be blank")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Please enter a user name between 3 and 50 characters in length")]
    [RegularExpression(@"^[a-zA-Z0-9'-'\s]*$", ErrorMessage = "Please enter a user name made up of letters and numbers only")]
    public string FirstName { get; set; }

    [StringLength(50, MinimumLength = 3, ErrorMessage = "Please enter a user name between 3 and 50 characters in length")]
    [RegularExpression(@"^[a-zA-Z0-9'-'\s]*$", ErrorMessage = "Please enter a user name made up of letters and numbers only")]
    public string MiddleName { get; set; }

    [Required(ErrorMessage = "The user name cannot be blank")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Please enter a user name between 3 and 50 characters in length")]
    [RegularExpression(@"^[a-zA-Z0-9'-'\s]*$", ErrorMessage = "Please enter a user name made up of letters and numbers only")]
    public string LastName { get; set; }

    [StringLength(50, MinimumLength = 3, ErrorMessage = "Please enter a user name between 3 and 50 characters in length")]
    [RegularExpression(@"^[a-zA-Z0-9'-'\s]*$", ErrorMessage = "Please enter a user name made up of letters and numbers only")]
    public string NickName { get; set; }

    [Required(ErrorMessage = "The user name cannot be blank")]
    public string Email { get; set; }
    public string Phone { get; set; }

    public List<SelectList> ImageLists { get; set; }

    public string[] UserImages { get; set; }
  }
}