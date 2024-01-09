using MVC_StokTakip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace MVC_StokTakip.Controllers
{
    public class SatisController : Controller
    {
        // GET: Satis
        StokTakipEntities db = new StokTakipEntities();
        public ActionResult Index(int sayfa = 1)
        {
            
            if (User.Identity.IsAuthenticated) 
            {
                var kullaniciadi = User.Identity.Name;
                var kullanici = db.Kullanici.FirstOrDefault(x=> x.Email == kullaniciadi);
                var model = db.Satislar.Where(x => x.KullaniciID == kullanici.ID).ToList().ToPagedList(sayfa, 5);
                return View(model); 
            } 

            return HttpNotFound();
        }

        public ActionResult SatinAl(int id) 
        {
            var model = db.Sepet.FirstOrDefault(x => x.ID == id);
            return View(model);
        }
        [HttpPost]
        public ActionResult SatinAl2(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var model = db.Sepet.FirstOrDefault(x => x.ID == id);

                    var satis = new Satislar
                    {
                        KullaniciID = model.KullaniciID,
                        UrunID = model.UrunID,
                        Adet = model.Adet,
                        Resim = model.Resim,
                        Fiyat = model.Fiyat,
                        Tarih = model.Tarih,
                    };
                    db.Sepet.Remove(model);
                    db.Satislar.Add(satis);
                    db.SaveChanges();
                    ViewBag.islem = "Satın Alma İşlemi Başarılı Bir Şekilde Gerçekleşmiştir";
                }
            }
            catch (Exception)
            {

                ViewBag.islem = "Satın Alma Başarısız";
            }
            return View("İslem");
        }

        public ActionResult HepsiniSatinAl(decimal? Tutar)
        {
            if (User.Identity.IsAuthenticated)
            {
                var kullaniciadi = User.Identity.Name;
                var kullanici = db.Kullanici.FirstOrDefault(x => x.Email == kullaniciadi);
                var model = db.Sepet.Where(x => x.KullaniciID == kullanici.ID).ToList();
                var kid = db.Sepet.FirstOrDefault(x => x.KullaniciID == kullanici.ID);
                if (model != null) 
                {
                    if (kid == null) 
                    {
                        ViewBag.Tutar = "Sepetinizde Ürün Bulunmamaktadır";
                    }
                    else if(kid != null)
                    {
                        Tutar = db.Sepet.Where(x => x.KullaniciID == kid.KullaniciID).Sum(x => x.Urun.Fiyat * x.Adet);
                        ViewBag.Tutar = ("Toplam Tutar = ") + Tutar + "TL";
                    }
                    return View(model);
                }
                return View();
            }
            return HttpNotFound();
        }


        [HttpPost]
        public ActionResult HepsiniSatinAl2()
        {
            var username = User.Identity.Name;
            var kullanici = db.Kullanici.FirstOrDefault(x => x.Email == username);
            var model = db.Sepet.Where(x => x.KullaniciID == kullanici.ID).ToList();
            int satir = 0;
            foreach ( var item in model) 
            {
                var Satis = new Satislar
                {
                    KullaniciID = model[satir].KullaniciID,
                    UrunID = model[satir].UrunID,
                    Adet = model[satir].Adet,
                    Fiyat = model[satir].Fiyat,
                    Resim = model[satir].Urun.Resim,
                    Tarih = DateTime.Now,
                };
                db.Satislar.Add(Satis);
                db.SaveChanges();
                satir++;
            }
            db.Sepet.RemoveRange(model);
            db.SaveChanges();
            return RedirectToAction("Index" , "Sepet");
        }
    }
}