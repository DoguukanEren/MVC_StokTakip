using MVC_StokTakip.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.Xml.Schema;
using System.Web.Helpers;

namespace MVC_StokTakip.Controllers
{
    public class UrunController : Controller
    {
        // GET: Urun
        StokTakipEntities db = new StokTakipEntities();
        [Authorize]
 
        public ActionResult Index(string ara)
        {
            var list = db.Urun.ToList();
            if (!string.IsNullOrEmpty(ara))
            {
                ara = ara.ToLower(); // Aranan metni küçük harfe dönüştürüyoruz
                list = list.Where(x => x.Ad.ToLower().Contains(ara) || x.Aciklama.ToLower().Contains(ara)).ToList();
                // Karşılaştırma yapmadan önce Ad ve Aciklama alanlarını da küçük harfe dönüştürüyoruz
            }
            return View(list);
        }



        [Authorize(Roles = "A")]
        public ActionResult Ekle() 
        { 
            List<SelectListItem> deger1 = (from x in db.Kategori.ToList()
                                       
                                       select new SelectListItem
                                       {
                                           Text = x.Ad,
                                           Value=x.ID.ToString(),
                                       }).ToList();
            ViewBag.ktgr = deger1;
            return View();  
        }
        [Authorize(Roles = "A")]
        [HttpPost]
        public ActionResult Ekle(Urun Data,HttpPostedFileBase File)
        {
            string path = Path.Combine("/Content/Image" + File.FileName);
            File.SaveAs(Server.MapPath(path));
            Data.Resim=File.FileName.ToString();
            db.Urun.Add(Data);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "A")]
        public ActionResult Sil(int id)
        {
            try
            {
                var urun = db.Urun.Where(x => x.ID == id).FirstOrDefault();
                if (urun != null)
                {
                    db.Urun.Remove(urun);
                    db.SaveChanges();
                }
                else
                {
                    ModelState.AddModelError("", "Silinecek ürün bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Bir hata oluştu: " + ex.Message);
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "A")]
        public ActionResult Guncelle(int id)
        {
            var guncelle = db.Urun.Where(x => x.ID == id).FirstOrDefault();
            List<SelectListItem> deger1 = (from x in db.Kategori.ToList()

                                           select new SelectListItem
                                           {
                                               Text = x.Ad,
                                               Value = x.ID.ToString(),
                                           }).ToList();
            ViewBag.ktgr = deger1;
            return View(guncelle);
        }
        [Authorize(Roles = "A")]
        [HttpPost]
        public ActionResult Guncelle(Urun model, HttpPostedFile File)
        {
            var urun = db.Urun.Find(model.ID);
            if (File == null)
            {
                urun.Ad = model.Ad;
                urun.Aciklama = model.Aciklama;
                urun.Fiyat = model.Fiyat;
                urun.Stok = model.Stok;
                urun.Popüler = model.Popüler;
                urun.KategoriID = model.KategoriID;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                urun.Resim = File.FileName.ToString();
                urun.Ad = model.Ad;
                urun.Aciklama = model.Aciklama;
                urun.Fiyat = model.Fiyat;
                urun.Stok = model.Stok;
                urun.Popüler = model.Popüler;
                urun.KategoriID = model.KategoriID;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

        }

        [Authorize(Roles ="A")]
        public ActionResult KritikStok()
        {
            var kritik=db.Urun.Where(x => x.Stok <= 50).ToList();
            return View(kritik);
        }

        public PartialViewResult StokCount()
        {
            if (User.Identity.IsAuthenticated)
            {
                var count = db.Urun.Where(x => x.Stok < 50).Count();
                ViewBag.count = count;
                var azalan = db.Urun.Where(x => x.Stok == 50).Count(); 
                ViewBag.azalan = azalan;

            }
            return PartialView();
        }

        public ActionResult StokGrafik() 
        {

            ArrayList deger1 = new ArrayList();
            ArrayList deger2 = new ArrayList();
            var veriler = db.Urun.ToList();
            veriler.ToList().ForEach(x => deger1.Add(x.Ad));
            veriler.ToList().ForEach(x => deger2.Add(x.Stok));
            var grafik = new Chart(width: 500, height: 500).AddTitle("Ürün-Stok-Grafiği").AddSeries(chartType: "Column", name: "Ad", xValue: deger1, yValues: deger2);
            return File(grafik.ToWebImage().GetBytes(),"image/jpeg");
        
        }

    }
}