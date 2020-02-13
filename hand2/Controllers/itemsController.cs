using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using hand2.Models;
using System.IO;

namespace hand2.Controllers
{
    public class itemsController : Controller
    {
        private Entities1 db = new Entities1();

        // GET: items
        public ActionResult Index()
        {
            var items = db.items.Include(i => i.Area).Include(i => i.Category).Include(i => i.User);
            ViewBag.areas = db.Areas.Where(a => a.ParentAreaId == 1 || a.ParentAreaId == null);
            ViewBag.categories = db.Categories.Where(c => c.CategoryParentId == null || c.CategoryParentId == 21);
            ViewBag.citiestf = false;
            return View(items.ToList());
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(User u)
        {
            var user = db.Users.FirstOrDefault(a => a.userName == u.userName);

            if (user != null)
            {
                if (user.UserPassword == u.UserPassword)
                {
                    Session.Add("userId", user.UserID.ToString());
                    //Session.Add(u.UserID.ToString(), "userId");
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("register");
       
        }
        [HttpPost]
        public ActionResult Search(int? min_price, int? max_price, int? select_cities, string dis, int? subcategoty, int? status)
        {
            var items = db.items.ToList();
            if (min_price != null)
                items = db.items.Where(i => i.price >= min_price).ToList();
            if (max_price != null)
                items =items.Where(i => i.price <= max_price).ToList();
            if (select_cities != null)
                items = items.Where(i => i.AreaID == select_cities).ToList();
            if (dis != null && !string.IsNullOrEmpty(dis))
                items = items.Where(i => i.Description.Contains(dis)).ToList();
            if (subcategoty != null)
                items = items.Where(i => i.CategoryID == subcategoty).ToList();
            if (status != null)
                items = items.Where(i => i.status == status).ToList();
            return PartialView("_ListItems", items);
        }
        [HttpGet]
        public ActionResult register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult register(User u)
        {

            db.Users.Add(u);
            db.SaveChanges();
            Session.Add("UserId", u.UserID);
            return RedirectToAction("Index");
        }
        public ActionResult GetCities(int ID)
        {
            ViewBag.citiestf = false;
            var l = db.Areas.Where(a => a.ParentAreaId == ID && ID != 1).Select(area => new { Text = area.AreaName, Value = area.AreaId.ToString() }).ToList();
            if (l != null)
            {
                ViewBag.citiestf = true;
            }
            return Json(l, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetCategoties(int ID)
        {
            ViewBag.subcategoties = false;
            var cate = db.Categories.Where(c => c.CategoryParentId == ID && ID != 21).Select(Category => new { Text = Category.CategoryName, Value = Category.CategoryId.ToString() }).ToList();

            if (cate != null)
            {
                ViewBag.subcategoties = true;
            }
            return Json(cate, JsonRequestBehavior.AllowGet);
        }

        // GET: items/Details/5
        public ActionResult _Details(int? id)
        {
            ViewBag.clear = false;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            item item = db.items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Details", item);
        }

        // GET: items/Create
        public ActionResult Create()
        {
            ViewBag.AreaID = new SelectList(db.Areas, "AreaId", "AreaName");
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryId", "CategoryName");
            ViewBag.userID = new SelectList(db.Users, "UserID", "userName");

            return View();
        }
        public void ClearDetails()
        {
            ViewBag.clear = true;
        }
        // POST: items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ItemID,Description,price,date,picture,CategoryID,AreaID,userID,Company,status")] item item, HttpPostedFileBase file)
        {
            string file_name = "";
            if (ModelState.IsValid)
            {
                db.items.Add(item);

                if (file.ContentLength > 0)
                {
                    file_name = Path.GetFileName(file.FileName);
                    string path = Path.Combine(Server.MapPath("~/pictures"), file_name);
                    file.SaveAs(path);
                    item.picture = file_name;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AreaID = new SelectList(db.Areas, "AreaId", "AreaName", item.AreaID);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryId", "CategoryName", item.CategoryID);
            ViewBag.userID = new SelectList(db.Users, "UserID", "userName", item.userID);
            return View(item);
        }

        // GET: items/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            item item = db.items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            ViewBag.AreaID = new SelectList(db.Areas, "AreaId", "AreaName", item.AreaID);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryId", "CategoryName", item.CategoryID);
            ViewBag.userID = new SelectList(db.Users, "UserID", "userName", item.userID);
            return View(item);
        }
        

        // POST: items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ItemID,Description,price,date,picture,CategoryID,AreaID,userID,Company,status")] item item)
        {
            if (ModelState.IsValid)
            {
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AreaID = new SelectList(db.Areas, "AreaId", "AreaName", item.AreaID);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryId", "CategoryName", item.CategoryID);
            ViewBag.userID = new SelectList(db.Users, "UserID", "userName", item.userID);
            return View(item);
        }

        // GET: items/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            item item = db.items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }
        public ActionResult DeleteN()
        {
            return View("Delete");
        }

        // POST: items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            item item = db.items.Find(id);
            db.items.Remove(item);
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
