﻿using MVC_StokTakip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MVC_StokTakip.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        StokTakipEntities db = new StokTakipEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login","Account");
        }
        [HttpPost]
        public ActionResult Login(Kullanici p)
        {
            var bilgiler = db.Kullanici.FirstOrDefault(x => x.Email == p.Email && x.Sifre == p.Sifre);
            if (bilgiler!= null) 
            {
                FormsAuthentication.SetAuthCookie(bilgiler.Email, false);
                Session["Mail"] = bilgiler.Email.ToString();
                Session["Ad"] = bilgiler.Ad.ToString();
                Session["SoyAd"] = bilgiler.Soyad.ToString();

                return RedirectToAction("Index","Home");

            }
            else
            {
                ViewBag.hata = "Kullancı Adı Veya Şifre Hatalı";
            }
                return View();
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(Kullanici data)
        {
            db.Kullanici.Add(data);
            data.Rol = "U";
            db.SaveChanges();
            return RedirectToAction("Login","Account");
        }

        public ActionResult Guncelle()
        {
            var kullanicilar = (string)Session["Mail"];
            var degerler = db.Kullanici.FirstOrDefault(x => x.Email == kullanicilar);
            return View(degerler);
        }
        [HttpPost]
        public ActionResult Guncelle(Kullanici data)
        { 
            var kullanicilar = (string)Session["Mail"];
            var user = db.Kullanici.Where(x => x.Email == kullanicilar).FirstOrDefault();
            user.Ad = data.Ad;
            user.Soyad = data.Soyad;
            user.Email = data.Email;
            user.KullaniciAd = data.KullaniciAd;
            user.Sifre = data.Sifre;
            user.SifreTekrar = data.SifreTekrar;
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}