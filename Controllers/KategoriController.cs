using MVC_StokTakip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MVC_StokTakip.Controllers
{
    [Authorize(Roles = "A")]
    public class KategoriController : Controller
    {
        // GET: Kategori
        StokTakipEntities db = new StokTakipEntities();
        
        public ActionResult Index()
        {
            return View(db.Kategori.Where(x => x.Durum==true).ToList());
        }
        
        public ActionResult Ekle()
        {
            return View();
        }
        [HttpPost]
        
        public ActionResult Ekle(Kategori data)
        {
            db.Kategori.Add(data);
            data.Durum = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        public ActionResult Sil(int id)
        {
            var kategori = db.Kategori.Where(x=> x.ID==id).FirstOrDefault();
            db.Kategori.Remove(kategori);
            kategori.Durum = false;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Guncelle(int id)
        {
            var kategori = db.Kategori.FirstOrDefault(x => x.ID == id);
            if (kategori == null)
            {
                return HttpNotFound();
            }
            return View(kategori);
        }

        [HttpPost]
        public ActionResult Guncelle(Kategori data)
        {
            try
            {
                var guncelle = db.Kategori.FirstOrDefault(x => x.ID == data.ID);
                if (guncelle != null)
                {
                    guncelle.Aciklama = data.Aciklama;
                    guncelle.Ad = data.Ad;
                    db.SaveChanges();
                    return RedirectToAction("Index"); // Başarılı güncelleme durumunda index sayfasına yönlendirme
                }
                else
                {
                    ModelState.AddModelError("", "Güncelleme için uygun kayıt bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Bir hata oluştu: " + ex.Message);
            }

            return View(data); // Hata durumunda güncelleme sayfasını tekrar gösterme
        }



    }
}