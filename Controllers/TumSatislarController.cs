using MVC_StokTakip.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_StokTakip.Controllers
{
    public class TumSatislarController : Controller
    {
        // GET: TumSatislar
        StokTakipEntities db = new StokTakipEntities();
        [Authorize(Roles = "A")]
        public ActionResult Index(int sayfa = 1)
        {
            return View(db.Satislar.ToList().ToPagedList(sayfa,5));
        }
    }
}