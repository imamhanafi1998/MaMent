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
    public class CONTENTTYPETABLEsController : Controller
    {
        private MaMent_DB_v2Entities db = new MaMent_DB_v2Entities();

        // GET: CONTENTTYPETABLEs
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
            var cONTENTTYPETABLEs = db.CONTENTTYPETABLEs.Include(c => c.STATUSTABLE);
            return View(cONTENTTYPETABLEs.ToList());
        }

        // GET: CONTENTTYPETABLEs/Details/5
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

            //cek parameter id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CONTENTTYPETABLE cONTENTTYPETABLE = db.CONTENTTYPETABLEs.Find(id);

            //cel id di database
            if (cONTENTTYPETABLE == null)
            {
                return HttpNotFound();
            }
            return View(cONTENTTYPETABLE);
        }

        // GET: CONTENTTYPETABLEs/Create
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

        // POST: CONTENTTYPETABLEs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CONTENTTYPEID,STATUSID,CONTENTTYPENAME")] CONTENTTYPETABLE cONTENTTYPETABLE)
        {
            cONTENTTYPETABLE.STATUSID = 1;

            // cek content type name ke database
            if (db.CONTENTTYPETABLEs.Where(m => m.CONTENTTYPENAME == cONTENTTYPETABLE.CONTENTTYPENAME).FirstOrDefault() != null)
            {
                ViewBag.CONTENTTYPENAMENOT = "Content Type Name exists";
                ViewBag.STATUSID = new SelectList(db.STATUSTABLEs, "STATUSID", "STATUSNAME");
                return View();
            }
            if (ModelState.IsValid)
            {
                db.CONTENTTYPETABLEs.Add(cONTENTTYPETABLE);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.STATUSID = new SelectList(db.STATUSTABLEs, "STATUSID", "STATUSNAME", cONTENTTYPETABLE.STATUSID);
            return View(cONTENTTYPETABLE);
        }

        // GET: CONTENTTYPETABLEs/Edit/5
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
            CONTENTTYPETABLE cONTENTTYPETABLE = db.CONTENTTYPETABLEs.Find(id);

            //id tidak ada di database
            if (cONTENTTYPETABLE == null)
            {
                return HttpNotFound();
            }
            ViewBag.STATUSID = new SelectList(db.STATUSTABLEs, "STATUSID", "STATUSNAME", cONTENTTYPETABLE.STATUSID);
            return View(cONTENTTYPETABLE);
        }

        // POST: CONTENTTYPETABLEs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CONTENTTYPEID,STATUSID,CONTENTTYPENAME")] CONTENTTYPETABLE cONTENTTYPETABLE)
        {
            //cek ketersediaan content type name
            if (db.CONTENTTYPETABLEs.Where(m => m.CONTENTTYPENAME == cONTENTTYPETABLE.CONTENTTYPENAME && m.CONTENTTYPEID != cONTENTTYPETABLE.CONTENTTYPEID).FirstOrDefault() != null)
            {
                //content type name tidak terssedia
                ViewBag.CONTENTTYPENAMENOT = "Content Type Name exists";
                ViewBag.STATUSID = new SelectList(db.STATUSTABLEs, "STATUSID", "STATUSNAME");
                return View();
            }
            if (ModelState.IsValid)
            {
                //save content type ke database
                db.Entry(cONTENTTYPETABLE).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.STATUSID = new SelectList(db.STATUSTABLEs, "STATUSID", "STATUSNAME", cONTENTTYPETABLE.STATUSID);
            return View(cONTENTTYPETABLE);
        }

        // GET: CONTENTTYPETABLEs/Delete/5
        public ActionResult Delete(int? id)
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
            CONTENTTYPETABLE cONTENTTYPETABLE = db.CONTENTTYPETABLEs.Find(id);

            //cek id di database
            if (cONTENTTYPETABLE == null)
            {
                return HttpNotFound();
            }
            return View(cONTENTTYPETABLE);
        }

        // POST: CONTENTTYPETABLEs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //hapus content type berdasarkan id
            CONTENTTYPETABLE cONTENTTYPETABLE = db.CONTENTTYPETABLEs.Find(id);
            db.CONTENTTYPETABLEs.Remove(cONTENTTYPETABLE);
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
