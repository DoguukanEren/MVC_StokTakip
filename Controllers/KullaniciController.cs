using MVC_StokTakip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace MVC_StokTakip.Controllers
{

    public class KullaniciController : Controller
    {
        // GET: Kullanici
        StokTakipEntities db = new StokTakipEntities();

        public ActionResult SifreReset()
        {
            return View();
        }



        [HttpPost]
        public ActionResult SifreReset(string eposta)
        {
            var mail = db.Kullanici.FirstOrDefault(x => x.Email == eposta); // Okunabilirlik için FirstOrDefault kullanın
            if (mail != null)
            {
                Random rnd = new Random();
                int yenisifre = rnd.Next(100000, 999999); // 6 haneli rastgele bir şifre oluştur
                mail.Sifre = Crypto.Hash(yenisifre.ToString(), "MD5"); // Şifreyi hashle
                db.SaveChanges();

                // SMTP sunucu adresindeki yazım hatasını düzelt ("gmail" yerine "gamil")
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.EnableSsl = true;
                WebMail.UserName = "projemvcmail@gmail.com";
                WebMail.Password = "sifreniz_buraya"; // Gerçek Gmail hesap şifrenizi kullanın
                WebMail.SmtpPort = 587;

                try
                {
                    // Yeni şifre ile e-posta gönder
                    WebMail.Send(
                        eposta,
                        "Giriş Şifreniz",
                        "Şifreniz: " + yenisifre.ToString()
                    );

                    ViewBag.uyari = "Şifreniz Başarıyla Gönderilmiştir.";
                }
                catch (Exception ex)
                {
                    // Oluşabilecek istisnaları ele alın, örneğin hata mesajını gösterin
                    ViewBag.uyari = "Mail gönderirken bir hata oluştu: " + ex.Message;
                }
            }
            else
            {
                ViewBag.uyari = "Hata Oluştu Tekrar Deneyiniz";
            }
            return View();
        }

    }
}