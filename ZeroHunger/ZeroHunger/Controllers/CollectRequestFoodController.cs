using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZeroHunger.DTOs;
using ZeroHunger.EF;
namespace ZeroHunger.Controllers
{
    public class CollectRequestFoodController : Controller
    {
        // GET: CollectRequestFood
        public ActionResult Index()
        {
            var db = new ZeroHungerEntities();
            var data = db.CollectRequestsFoodItems.ToList();
            return View(data);

        }


        public ActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CollectRequestsFoodItemId,RequestId,ItemName,Quantity,ExpiryDate,Description")] CollectRequestsFooditemDTO collectRequestsFooditem)
        {
            if (ModelState.IsValid)
            {
                var db = new ZeroHungerEntities();
                var config = new MapperConfiguration(cfg => {
                    cfg.CreateMap<CollectRequestsFooditemDTO, CollectRequestsFoodItem>();
                });
                var mapper = new Mapper(config);
                var data2 = mapper.Map<CollectRequestsFoodItem>(collectRequestsFooditem);

                db.CollectRequestsFoodItems.Add(data2);
                db.SaveChanges();
                return RedirectToAction("Details");
            }

            return View(collectRequestsFooditem);
        }

        public ActionResult Details(int id)
        {
            var db = new ZeroHungerEntities();
            var CollectRequestsFooditem = db.CollectRequestsFoodItems.Find(id);

            return View(CollectRequestsFooditem);

        }



    }
}