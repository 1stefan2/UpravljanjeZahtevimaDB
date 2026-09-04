using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrezentacioniSloj.Controllers
{


    public class PocetnaController : Controller
    {
        public ActionResult Index() { return View(); }
        public ActionResult O_Softveru() { return View(); }
        public ActionResult O_Meni() { return View(); }
        public ActionResult Kontakt() { return View(); }
    }


}