using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MaMent_v2.Models;

namespace MaMent_v2.Controllers
{
    public class ROLETABLEsController : Controller
    {
        private MaMent_DB_v2Entities db = new MaMent_DB_v2Entities();

        // GET: ROLETABLEs
        public ActionResult Index()
        {
            //cek session login
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "USERTABLEs", new { area = "" });
            }

            //cek role user
            if (Session["userRole"].ToString() != "admin")
            {
                return RedirectToAction("Index", "CONTENTTABLEs", new { area = "" });
            }
            var rOLETABLEs = db.ROLETABLEs.Include(r => r.STATUSTABLE);
            return View(rOLETABLEs.ToList());
        }

        // GET: ROLETABLEs/Details/5
        public ActionResult Details(int? id)
        {
            //cek session login
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "USERTABLEs", new { area = "" });
            }

            //cek role user
            if (Session["userRole"].ToString() != "admin")
            {
                return RedirectToAction("Index", "CONTENTTABLEs", new { area = "" });
            }

            //cek id di parameter
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ROLETABLE rOLETABLE = db.ROLETABLEs.Find(id);

            //cek id di database
            if (rOLETABLE == null)
            {
                return HttpNotFound();
            }
            return View(rOLETABLE);
        }

        // GET: ROLETABLEs/Create
        public ActionResult Create()
        {
            //cek session login
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "USERTABLEs", new { area = "" });
            }

            //cek role user
            if (Session["userRole"].ToString() != "admin")
            {
                return RedirectToAction("Index", "CONTENTTABLEs", new { area = "" });
            }
            ViewBag.STATUSID = new SelectList(db.STATUSTABLEs, "STATUSID", "STATUSNAME");
            return View();
        }

        // POST: ROLETABLEs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ROLEID,STATUSID,ROLENAME,ROLEDESCRIPTION")] ROLETABLE rOLETABLE)
        {
            //cek session login
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "USERTABLEs", new { area = "" });
            }

            //cek role user
            if (Session["userRole"].ToString() != "admin")
            {
                return RedirectToAction("Index", "CONTENTTABLEs", new { area = "" });
            }
            rOLETABLE.STATUSID = 1;

            //cek ketersediaan role name
            if (db.ROLETABLEs.Where(m => m.ROLENAME == rOLETABLE.ROLENAME).FirstOrDefault() != null)
            {
                //role name tidak tersedia
                ViewBag.ROLENAMENOT = "Role Name exists";
                ViewBag.STATUSID = new SelectList(db.STATUSTABLEs, "STATUSID", "STATUSNAME");
                return View();
            }
            if (ModelState.IsValid)
            {
                //menambahkan role ke database
                db.ROLETABLEs.Add(rOLETABLE);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.STATUSID = new SelectList(db.STATUSTABLEs, "STATUSID", "STATUSNAME", rOLETABLE.STATUSID);
            return View(rOLETABLE);
        }

        // GET: ROLETABLEs/Edit/5
        public ActionResult Edit(int? id)
        {
            //cek session login
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "USERTABLEs", new { area = "" });
            }

            //cek role user
            if (Session["userRole"].ToString() != "admin")
            {
                return RedirectToAction("Index", "CONTENTTABLEs", new { area = "" });
            }

            //cek id di parameter
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ROLETABLE rOLETABLE = db.ROLETABLEs.Find(id);

            //cek id di database
            if (rOLETABLE == null)
            {
                return HttpNotFound();
            }

            //hanya super admin yang bisa mengedit akun ber-ID 1
            if (Convert.ToInt32(Session["userId"]) != 1 && id == 1)
            {
                return RedirectToAction("Index", "USERTABLEs");
            }
            ViewBag.STATUSID = new SelectList(db.STATUSTABLEs, "STATUSID", "STATUSNAME", rOLETABLE.STATUSID);
            return View(rOLETABLE);
        }

        // POST: ROLETABLEs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ROLEID,STATUSID,ROLENAME,ROLEDESCRIPTION")] ROLETABLE rOLETABLE)
        {
            //cek ketersediaan role name
            if (db.ROLETABLEs.Where(m => m.ROLENAME == rOLETABLE.ROLENAME && m.ROLEID != rOLETABLE.ROLEID).FirstOrDefault() != null)
            {
                ViewBag.ROLENAMENOT = "Role Name exists";
                ViewBag.STATUSID = new SelectList(db.STATUSTABLEs, "STATUSID", "STATUSNAME");
                return View(rOLETABLE);
            }
            if (ModelState.IsValid)
            {
                //save perubahan ke database
                db.Entry(rOLETABLE).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.STATUSID = new SelectList(db.STATUSTABLEs, "STATUSID", "STATUSNAME", rOLETABLE.STATUSID);
            return View(rOLETABLE);
        }

        // GET: ROLETABLEs/Delete/5
        public ActionResult Delete(int? id)
        {
            //cek id di parameter
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //cek session login
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "USERTABLEs", new { area = "" });
            }

            //cek role user
            if (Session["userRole"].ToString() != "admin")
            {
                return RedirectToAction("Index", "CONTENTTABLEs", new { area = "" });
            }

            //role ber-ID 1 tidak bisa dihapus
            if (id == 1)
            {
                return RedirectToAction("Index", "ROLETABLEs", new { area = "" });
            }
            ROLETABLE rOLETABLE = db.ROLETABLEs.Find(id);

            //cel id di database
            if (rOLETABLE == null)
            {
                return HttpNotFound();
            }
            return View(rOLETABLE);
        }

        // POST: ROLETABLEs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //hapus role di database
            ROLETABLE rOLETABLE = db.ROLETABLEs.Find(id);
            db.ROLETABLEs.Remove(rOLETABLE);
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
