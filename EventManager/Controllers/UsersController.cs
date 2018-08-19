using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EventManager;
using EventManager.ViewModels;

namespace EventManager.Controllers
{
  public class UsersController : Controller
  {
    private EventsRegistrationDBEntities db = new EventsRegistrationDBEntities();

    // GET: Users
    public ActionResult Index()
    {
      UsersIndexViewModel uvm = new UsersIndexViewModel();
      uvm.Users = db.Users.Include(u => u.UserImageMappings);

      return View(uvm);
    }

    // GET: Users/Details/5
    public ActionResult Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      User user = db.Users.Find(id);
      if (user == null)
      {
        return HttpNotFound();
      }
      return View(user);
    }

    // GET: Users/Create
    public ActionResult Create()
    {
      UserViewModel viewModel = new UserViewModel();

      viewModel.ImageLists = new List<SelectList>();

      for (int i = 0; i < Constants.NumberOfUserImages; i++)
      {
        viewModel.ImageLists.Add(new SelectList(db.UserImages, "ID", "FileName"));
      }
      return View(viewModel);
    }

    // POST: Users/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(UserViewModel viewModel)
    {
      User user = new User();
      user.FirstName = viewModel.FirstName;
      user.MiddleName = viewModel.MiddleName;
      user.LastName = viewModel.LastName;
      user.NickName = viewModel.NickName;
      user.Email = viewModel.Email;
      user.Phone = viewModel.Phone;
      user.RegistrationDate = DateTime.Now;

      user.UserImageMappings = new List<UserImageMapping>();

      //get a list of selected images without any blanks
      string[] userImages = viewModel.UserImages.Where(pi => !string.IsNullOrEmpty(pi)).ToArray();
      for (int i = 0; i < userImages.Length; i++)
      {
        user.UserImageMappings.Add(new UserImageMapping
        {
          UserImage = db.UserImages.Find(int.Parse(userImages[i])),
          ImageNumber = i
        });
      }
      if (ModelState.IsValid)
      {
        db.Users.Add(user);
        db.SaveChanges();
        return RedirectToAction("Index");
      }

      viewModel.ImageLists = new List<SelectList>();
      for (int i = 0; i < Constants.NumberOfUserImages; i++)
      {
        viewModel.ImageLists.Add(new SelectList(db.UserImages, "ID", "FileName", viewModel.UserImages[i]));
      }
      return View(viewModel);
    }

    // GET: Users/Edit/5
    public ActionResult Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      User user = db.Users.Find(id);
      if (user == null)
      {
        return HttpNotFound();
      }

      UserViewModel viewModel = new UserViewModel();
      viewModel.ImageLists = new List<SelectList>();
      foreach (var imageMapping in user.UserImageMappings.OrderBy(pim => pim.ImageNumber))
      {
        viewModel.ImageLists.Add(new SelectList(db.UserImages, "ID", "FileName", imageMapping.UserImageID));
      }
      for (int i = viewModel.ImageLists.Count; i < Constants.NumberOfUserImages; i++)
      {
        viewModel.ImageLists.Add(new SelectList(db.UserImages, "ID", "FileName"));
      }
      viewModel.ID = user.Id;
      viewModel.FirstName = user.FirstName;
      viewModel.MiddleName = user.MiddleName;
      viewModel.LastName = user.LastName;
      viewModel.NickName = user.NickName;
      viewModel.Email = user.Email;
      viewModel.Phone = user.Phone;
      return View(viewModel);
    }

    // POST: Users/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(UserViewModel viewModel)
    {
      var userToUpdate = db.Users.Include(p => p.UserImageMappings).Where(p => p.Id == viewModel.ID).Single();
      if (TryUpdateModel(userToUpdate, "", new string[] { "FirstName", "MiddleName", "LastName", "NickName", "Email", "Phone" }))
      {
        if (userToUpdate.UserImageMappings == null)
        {
          userToUpdate.UserImageMappings = new List<UserImageMapping>();
        }
        //get a list of selected images without any blanks
        string[] productImages = viewModel.UserImages.Where(pi =>
        !string.IsNullOrEmpty(pi)).ToArray();
        for (int i = 0; i < productImages.Length; i++)
        {
          //get the image currently stored
          var imageMappingToEdit = userToUpdate.UserImageMappings.Where(pim => pim.
          ImageNumber == i).FirstOrDefault();
          //find the new image
          var image = db.UserImages.Find(int.Parse(productImages[i]));
          //if there is nothing stored then we need to add a new mapping
          if (imageMappingToEdit == null)
          {
            //add image to the imagemappings
            userToUpdate.UserImageMappings.Add(new UserImageMapping
            {
              ImageNumber = i,
              UserImage  = image,
              UserImageID = image.Id
            });
          }
          //else it's not a new file so edit the current mapping
          else
          {
            //if they are not the same
            if (imageMappingToEdit.UserImageID != int.Parse(productImages[i]))
            {
              //assign image property of the image mapping
              imageMappingToEdit.UserImage = image;
            }
          }
        }

        //delete any other imagemappings that the user did not include in their
        //selections for the product
        for (int i = productImages.Length; i < Constants.NumberOfUserImages; i++)
        {
          var imageMappingToEdit = userToUpdate.UserImageMappings.Where(pim =>
          pim.ImageNumber == i).FirstOrDefault();
          //if there is something stored in the mapping
          if (imageMappingToEdit != null)
          {
            //delete the record from the mapping table directly.
            //just calling productToUpdate.ProductImageMappings.Remove(imageMappingToEdit)
            //results in a FK error
            db.UserImageMappings.Remove(imageMappingToEdit);
          }
        }
        db.SaveChanges();
        return RedirectToAction("Index");
      }
      return View(viewModel);
    }

    // GET: Users/Delete/5
    public ActionResult Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      User user = db.Users.Find(id);
      if (user == null)
      {
        return HttpNotFound();
      }
      return View(user);
    }

    // POST: Users/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
      User user = db.Users.Find(id);

      db.Users.Remove(user);
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
  }
}
