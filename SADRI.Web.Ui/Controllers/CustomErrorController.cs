using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SADRI.Web.Ui.Controllers
{
    public class CustomErrorController : Controller
    {
        // GET: CustomError
        public ActionResult Error403()
        {
            return View();
        }
    }
}