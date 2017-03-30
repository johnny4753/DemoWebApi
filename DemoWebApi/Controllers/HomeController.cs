using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DemoWebApi.Models;

namespace DemoWebApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HomeUploadViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var imagefile = viewModel.Imagefile;
            var fileExt = imagefile.FileName.Split('.').Last();
            var fileName = "UploadImageFile."+ fileExt;
            var path = Path.Combine(Server.MapPath("~/Pictures"), fileName);
            imagefile.SaveAs(path);
            return RedirectToAction("Index");
        }
    }
}
