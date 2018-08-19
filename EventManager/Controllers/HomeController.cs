using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventManager.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult About()
    {
      ViewBag.Message = "Система он-лайн регистрации участников. Служит для создания мероприятий, встреч и приглашения к участию в них.";

      return View();
    }

    public ActionResult Contact()
    {
      ViewBag.Message = "Контактные данные:";

      return View();
    }
  }
}