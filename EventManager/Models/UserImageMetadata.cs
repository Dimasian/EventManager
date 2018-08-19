using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace EventManager
{
  [MetadataType(typeof(UserImageMetadata))]
  public partial class UserImage
  {

  }
  public partial class UserImageMetadata
  {
    [Required]
    [Display(Name = "File Name")]
    public string FileName;
  }

}