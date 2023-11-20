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
    public class UserController : Controller
    {
        private ZeroHungerEntities db = new ZeroHungerEntities();


        // GET: User
        public ActionResult Index()
        {
            return View();
           
        }

        [HttpPost]
        public ActionResult Index(UserDTO userDTO)
        {
            var existingUser = db.Users.FirstOrDefault(c => c.UserName == userDTO.UserName);

            if (existingUser != null && userDTO.Password == existingUser.Password && existingUser.Role == "Employee")
            {
                HttpCookie EmployeeCookie = new HttpCookie("EmployeeInfo");
                EmployeeCookie["UserName"] = existingUser.UserName;
                EmployeeCookie["UserId"] = existingUser.UserId.ToString();
                EmployeeCookie.Expires = DateTime.Now.AddMinutes(40);
                Response.Cookies.Add(EmployeeCookie);

                return RedirectToAction("Index", "Employee");
            }
            else if (existingUser != null && userDTO.Password == existingUser.Password && existingUser.Role == "Admin")
            {
                HttpCookie adminCookie = new HttpCookie("AdminInfo");
                adminCookie["UserName"] = existingUser.UserName;
                adminCookie["UserId"] = existingUser.UserId.ToString();
                adminCookie.Expires = DateTime.Now.AddMinutes(40);
                Response.Cookies.Add(adminCookie);

                return RedirectToAction("Dashboard", "Admin");
            }
            else if (existingUser != null && userDTO.Password == existingUser.Password && existingUser.Role == "Restaurant")
            {
                HttpCookie RestaurantCookie = new HttpCookie("RestaurantInfo");
                RestaurantCookie["UserName"] = existingUser.UserName;
                RestaurantCookie["UserId"] = existingUser.UserId.ToString();
                RestaurantCookie.Expires = DateTime.Now.AddMinutes(40);
                Response.Cookies.Add(RestaurantCookie);

                return RedirectToAction("Dashboard", "Restaurant");
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }
        }



        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(UserDTO c)
        {
            var db = new ZeroHungerEntities();
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<UserDTO, User>();
            });
            var mapper = new Mapper(config);
            var signupData = mapper.Map<User>(c);
            db.Users.Add(signupData);
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            return View((user));
        }

        public ActionResult Logout()
        {
            if (Request.Cookies["AdminInfo"] != null)
            {
                var adminCookie = new HttpCookie("AdminInfo");
                adminCookie.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(adminCookie);
            }
            else if (Request.Cookies["EmployeeInfo"] != null)
            {
                var EmployeeCookie = new HttpCookie("EmployeeInfo");
                EmployeeCookie.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(EmployeeCookie);
            }
            else if (Request.Cookies["RestaurantInfo"] != null)
            {
                var RestaurantCookie = new HttpCookie("RestaurantInfo");
                RestaurantCookie.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(RestaurantCookie);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,UserName,Password,Role,Name,ContactInfo,Location")] UserDTO user)
        {
            if (ModelState.IsValid)
            {
                var db = new ZeroHungerEntities();
                var config = new MapperConfiguration(cfg => {
                    cfg.CreateMap<UserDTO, User>();
                });
                var mapper = new Mapper(config);
                var data2 = mapper.Map<User>(user);

                db.Users.Add(data2);
                db.SaveChanges();

                switch (user.Role)
                {
                    case "Admin":
                        return RedirectToAction("Dashboard", "Admin");
                    case "Employee":
                        return RedirectToAction("Index", "Employee");
                    case "Restaurant":
                        return RedirectToAction("FoodInfo", "Restaurant");
                    default:
                        // Handle other roles or provide a default redirection
                        return RedirectToAction("Index", "Home");
                }
                /*return RedirectToAction("Index","Restaurant");*/
                // return RedirectToAction("Index", "Admin");
            }

            return View(user);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            var db = new ZeroHungerEntities();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<User, UserDTO>();
            });
            var mapper = new Mapper(config);
            var data2 = mapper.Map<UserDTO>(user);


            return View(data2);
        }
        [HttpPost]
        public ActionResult Edit([Bind(Include = "UserId,UserName,Password,Role,Name,ContactInfo,Location")] UserDTO user)
        {
            if (ModelState.IsValid)
            {
                var db = new ZeroHungerEntities();
                var config = new MapperConfiguration(cfg => {
                    cfg.CreateMap<UserDTO, User>();
                });
                var mapper = new Mapper(config);
                var data2 = mapper.Map<User>(user);

                db.Entry(data2).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            return View((user));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
