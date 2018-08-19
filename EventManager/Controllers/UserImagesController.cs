using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using EventManager;

namespace EventManager.Controllers
{
  public class UserImagesController : Controller
  {
    private EventsRegistrationDBEntities db = new EventsRegistrationDBEntities();

    // GET: UserImages
    public ActionResult Index()
    {
      return View(db.UserImages.ToList());
    }

    // GET: UserImages/Details/5
    public ActionResult Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      UserImage userImage = db.UserImages.Find(id);
      if (userImage == null)
      {
        return HttpNotFound();
      }
      return View(userImage);
    }

    // GET: UserImages/Create
    public ActionResult Upload()
    {
      return View();
    }

    // POST: UserImages/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Upload(HttpPostedFileBase file)
    {
      //check the user has entered a file
      if (file != null)
      {
        //check if the file is valid
        if (ValidateFile(file))
        {
          try
          {
            SaveFileToDisk(file);
          }
          catch (Exception)
          {
            ModelState.AddModelError("FileName", "VAlidation error: " +
              "Sorry an error occurred saving the file to disk, please try again");
          }
        }
        else
        {
          ModelState.AddModelError("FileName", "The file must be gif, png, jpeg or jpg and less than 2MB in size");
        }
      }
      else
      {
        //if the user has not entered a file return an error message
        ModelState.AddModelError("FileName", "Please choose a file");
      }

      if (ModelState.IsValid)
      {
        db.UserImages.Add(new UserImage { FileName = file.FileName });
        try
        {
          db.SaveChanges();
        }
        catch (DbUpdateException ex)
        {
          SqlException innerException = ex.InnerException.InnerException as SqlException;
          if (innerException != null && innerException.Number == 2601)
          {
            ModelState.AddModelError("FileName", "The file " + file.FileName +
            " already exists in the system. Please delete it and try again if you wish to re - add it");
          }
          else
          {
            ModelState.AddModelError("FileName", "Sorry an error has occurred saving to the database, please try again");
          }
          return View();
        }
        return RedirectToAction("Index");
      }
      return View();
    }


    // GET: UserImages/Edit/5
    public ActionResult Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      UserImage userImage = db.UserImages.Find(id);
      if (userImage == null)
      {
        return HttpNotFound();
      }
      return View(userImage);
    }

    // POST: UserImages/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,FileName")] UserImage userImage)
    {
      if (ModelState.IsValid)
      {
        db.Entry(userImage).State = EntityState.Modified;
        db.SaveChanges();
        return RedirectToAction("Index");
      }
      return View(userImage);
    }

    // GET: UserImages/Delete/5
    public ActionResult Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      UserImage userImage = db.UserImages.Find(id);
      if (userImage == null)
      {
        return HttpNotFound();
      }
      return View(userImage);
    }

    // POST: UserImages/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
      UserImage userImage = db.UserImages.Find(id);

      //find all the mappings for this image
      var mappings = userImage.UserImageMappings.Where(pim => pim.UserImageID == id);
      foreach (var mapping in mappings)
      {
        //find all mappings for any product containing this image
        var mappingsToUpdate = db.UserImageMappings.Where(pim => pim.UserID == mapping.UserID);
        //for each image in each product change its imagenumber to one lower if it is higher
        //than the current image
        foreach (var mappingToUpdate in mappingsToUpdate)
        {
          if (mappingToUpdate.ImageNumber > mapping.ImageNumber)
          {
            mappingToUpdate.ImageNumber--;
          }
        }
      }

      //delete user image from file system
      System.IO.File.Delete(Request.MapPath(Constants.UsersImagePath + userImage.FileName));
      System.IO.File.Delete(Request.MapPath(Constants.UsersThumbnailPath +userImage.FileName));

      db.UserImages.Remove(userImage);
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


    private bool ValidateFile(HttpPostedFileBase file)
    {
      string fileExtension = System.IO.Path.GetExtension(file.FileName).ToLower();
      string[] allowedFileTypes = { ".gif", ".png", ".jpeg", ".jpg" };
      if ((file.ContentLength > 0 && file.ContentLength < 2097152) && allowedFileTypes.Contains(fileExtension))
      {
        return true;
      }
      return false;
    }

    private void SaveFileToDisk(HttpPostedFileBase file)
    {
      WebImage img = new WebImage(file.InputStream);
      string fileExtension = System.IO.Path.GetExtension(file.FileName).ToLower().Replace(".", "");

      if (img.Width > 190)
      {
        img.Resize(190, img.Height);
      }
      img.Save(Constants.UsersImagePath + file.FileName, fileExtension);
      if (img.Width > 50)
      {
        img.Resize(50, img.Height);
      }
      img.Save(Constants.UsersThumbnailPath + file.FileName, fileExtension);
    }
  }
}
