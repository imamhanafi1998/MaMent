using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MaMent_v2.Models;
using MaMent_v2.ViewModels;
using System.IO;
using System.Configuration;

namespace MaMent_v2.Controllers
{
    public class CONTENTTABLEsController : Controller
    {
        private MaMent_DB_v2Entities db = new MaMent_DB_v2Entities();

        // GET: CONTENTTABLEs
        public ActionResult Index()
        {
            //cek session user yang login
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "USERTABLEs", new { area = "" });
            }

            //cek role
            if (Session["userRole"].ToString() == "admin")
            {
                // role admin bisa melihat semua content tanpa batasan
                var cONTENTTABLEs = db.CONTENTTABLEs.Include(c => c.CONTENTTYPETABLE).Include(c => c.STATUSTABLE);
                return View(cONTENTTABLEs.ToList());
            }
            else
            {
                // role non-admin hanya bisa melihat content sesuai role yang dimilikinya
                var uss = Convert.ToInt32(Session["userId"]);
                var vv = from c in db.CONTENTTABLEs
                         join cr in db.CONTENTROLEs
                         on c.CONTENTID equals cr.CONTENTID
                         join ur in db.USERROLEs
                         on cr.ROLEID equals ur.ROLEID
                         join u in db.USERTABLEs
                         on ur.USERID equals u.USERID
                         where ur.USERID == uss
                         select c;
                var disc = Enumerable.Distinct(vv);
                var cONTENTTABLEs = from all in disc select all;
                return View(cONTENTTABLEs.ToList());
            }
        }

        // GET: CONTENTTABLEs/Details/5
        public ActionResult Details(int? id)
        {
            //cek session user yang login
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "USERTABLEs", new { area = "" });
            }

            //cek id di parameter
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //ada parameter idnya

            //ambil role yang dimiliki user
            var uss = Convert.ToInt32(Session["userId"]);
            var konten = from c in db.CONTENTTABLEs
                         join cr in db.CONTENTROLEs
                         on c.CONTENTID equals cr.CONTENTID
                         join ur in db.USERROLEs
                         on cr.ROLEID equals ur.ROLEID
                         where (ur.USERID == uss) & (cr.CONTENTID == id)
                         select c;
            var disc = Enumerable.Distinct(konten);
            var hasil = from all in disc select all;

            //cek konten yang bisa diakses oleh user
            
            if (hasil.Count() == 0)
            //tiada konten yang bisa diakses karena berbeda role
            {
                //kalau admin bisa lihat semuanya
                if (Session["userRole"].ToString() != "admin")
                {
                    return RedirectToAction("Index");
                }
            }

            //ada role konten yang sama dengan role usernya
            var cONTENTTABLE = db.CONTENTTABLEs.Where(m => m.CONTENTID == id).Include(m => m.STATUSTABLE).Include(m => m.CONTENTTYPETABLE).FirstOrDefault();
            if (cONTENTTABLE == null)
            {
                return HttpNotFound();
            }
            var Results = from r in db.ROLETABLEs
                          select new
                          {
                              r.ROLEID,
                              r.ROLENAME,
                              Checked = ((from ur in db.CONTENTROLEs
                                          where (ur.CONTENTID == id) & (ur.ROLEID == r.ROLEID)
                                          select ur).Count() > 0)
                          };
            var user = db.USERTABLEs.Where(c => c.USERID == cONTENTTABLE.USERID).Select(c => c.USERNAME).FirstOrDefault();
            var myViewModel = new CONTENTTABLEVIEW();
            myViewModel.contentId = id.Value;
            myViewModel.contentTitle = cONTENTTABLE.CONTENTTITLE;
            myViewModel.contentDescription = cONTENTTABLE.CONTENTDESCRIPTION;
            myViewModel.contentLink = cONTENTTABLE.CONTENTLINK;
            myViewModel.contentFilePath = cONTENTTABLE.CONTENTFILEPATH;
            myViewModel.contentTypeName = cONTENTTABLE.CONTENTTYPETABLE.CONTENTTYPENAME;
            myViewModel.userName = user;
            myViewModel.statusName = cONTENTTABLE.STATUSTABLE.STATUSNAME;
            //System.Diagnostics.Debug.WriteLine("view : " + myViewModel.contentKeyword);
            //System.Diagnostics.Debug.WriteLine("model : " + cONTENTTABLE.CONTENTKEYWORD);
            myViewModel.contentKeyword = cONTENTTABLE.CONTENTKEYWORD;
            myViewModel.contentDate = cONTENTTABLE.CONTENTDATE;
            myViewModel.newDate = cONTENTTABLE.CONTENTDATE.ToString("dd MMMM yyyy");

            //isi content role ke checkbox
            var myCheckBoxList = new List<CHECKBOXVIEW>();
            foreach (var item in Results)
            {
                myCheckBoxList.Add(new CHECKBOXVIEW { Id = item.ROLEID, Name = item.ROLENAME, Checked = item.Checked });
            }
            myViewModel.contentRole = myCheckBoxList;
            return View(myViewModel);
        }

        // GET: CONTENTTABLEs/Create
        public ActionResult Create()
        {

            //cek session login
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "USERTABLEs", new { area = "" });
            }

            //ada session loginnya
            CONTENTTABLE cONTENTTABLE = new CONTENTTABLE();
            var Results = from r in db.ROLETABLEs
                          select new
                          {
                              r.ROLEID,
                              r.ROLENAME,
                              Checked = ((from ur in db.CONTENTROLEs
                                          where (ur.CONTENTID == 0) & (ur.ROLEID == r.ROLEID)
                                          select ur).Count() > 0)
                          };
            var myViewModel = new CONTENTTABLEVIEW();
            ViewBag.CONTENTTYPEID = new SelectList(db.CONTENTTYPETABLEs, "CONTENTTYPEID", "CONTENTTYPENAME");
            ViewBag.STATUSID = new SelectList(db.STATUSTABLEs, "STATUSID", "STATUSNAME");

            //masukkan semua role ke checkbox
            var myCheckBoxList = new List<CHECKBOXVIEW>();
            foreach (var item in Results)
            {
                myCheckBoxList.Add(new CHECKBOXVIEW { Id = item.ROLEID, Name = item.ROLENAME, Checked = item.Checked });
            }
            myViewModel.contentRole = myCheckBoxList;
            return View(myViewModel);
        }

        // POST: CONTENTTABLEs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CONTENTTABLEVIEW contentTableView, HttpPostedFileBase contentFilePath)
        {
            string path = Server.MapPath("~/UserContentFiles/");

            //kalau direktori tidak ada, dibuatkan
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //cek title apakah ada yang sama
            if (db.CONTENTTABLEs.Where(c => c.CONTENTTITLE == contentTableView.contentTitle).FirstOrDefault() != null)
            {
                //title sudah ada
                ViewBag.CONTENTTITLENOT = "Content Title exists";
                CONTENTTABLE cONTENTTABLE = new CONTENTTABLE();
                var Results = from r in db.ROLETABLEs
                              select new
                              {
                                  r.ROLEID,
                                  r.ROLENAME,
                                  Checked = ((from ur in db.CONTENTROLEs
                                              where (ur.CONTENTID == 0) & (ur.ROLEID == r.ROLEID)
                                              select ur).Count() > 0)
                              };
                var myViewModel = new CONTENTTABLEVIEW();
                ViewBag.CONTENTTYPEID = new SelectList(db.CONTENTTYPETABLEs, "CONTENTTYPEID", "CONTENTTYPENAME");
                ViewBag.STATUSID = new SelectList(db.STATUSTABLEs, "STATUSID", "STATUSNAME");

                var myCheckBoxList = new List<CHECKBOXVIEW>();
                foreach (var item in Results)
                {
                    myCheckBoxList.Add(new CHECKBOXVIEW { Id = item.ROLEID, Name = item.ROLENAME, Checked = item.Checked });
                }
                myViewModel.contentRole = myCheckBoxList;
                return View(myViewModel);
            }

            //title tersedia

            //cek apakah user insert file
            if (contentFilePath != null || contentTableView.contentFilePath != null)
            {

                //user insert file
                HttpPostedFileBase File = Request.Files["contentFilePath"];
                var supportedTypes = new[] { "txt", "doc", "docx", "pdf", "xls", "xlsx" };
                var fileExt = System.IO.Path.GetExtension(File.FileName).Substring(1);

                //cek extention
                if (!supportedTypes.Contains(fileExt))
                {
                    //bukan tipe dokumen
                    ModelState.AddModelError("CONTENTFILEPATH", "Only document file type is allowed.");
                    CONTENTTABLE cONTENTTABLE = new CONTENTTABLE();
                    var Results = from r in db.ROLETABLEs
                                  select new
                                  {
                                      r.ROLEID,
                                      r.ROLENAME,
                                      Checked = ((from ur in db.CONTENTROLEs
                                                  where (ur.CONTENTID == 0) & (ur.ROLEID == r.ROLEID)
                                                  select ur).Count() > 0)
                                  };
                    var myViewModel = new CONTENTTABLEVIEW();
                    ViewBag.CONTENTTYPEID = new SelectList(db.CONTENTTYPETABLEs, "CONTENTTYPEID", "CONTENTTYPENAME");
                    ViewBag.STATUSID = new SelectList(db.STATUSTABLEs, "STATUSID", "STATUSNAME");

                    var myCheckBoxList = new List<CHECKBOXVIEW>();
                    foreach (var item in Results)
                    {
                        myCheckBoxList.Add(new CHECKBOXVIEW { Id = item.ROLEID, Name = item.ROLENAME, Checked = item.Checked });
                    }
                    myViewModel.contentRole = myCheckBoxList;
                    return View(myViewModel);
                }

                //cek ukuran file
                if (contentFilePath.ContentLength > 10000000)
                {

                    //ukuran file > 10mb
                    ModelState.AddModelError("CONTENTFILEPATH", "Only for <= 10MB file is allowed.");
                    CONTENTTABLE cONTENTTABLE = new CONTENTTABLE();
                    var Results = from r in db.ROLETABLEs
                                  select new
                                  {
                                      r.ROLEID,
                                      r.ROLENAME,
                                      Checked = ((from ur in db.CONTENTROLEs
                                                  where (ur.CONTENTID == 0) & (ur.ROLEID == r.ROLEID)
                                                  select ur).Count() > 0)
                                  };
                    var myViewModel = new CONTENTTABLEVIEW();
                    ViewBag.CONTENTTYPEID = new SelectList(db.CONTENTTYPETABLEs, "CONTENTTYPEID", "CONTENTTYPENAME");
                    ViewBag.STATUSID = new SelectList(db.STATUSTABLEs, "STATUSID", "STATUSNAME");

                    var myCheckBoxList = new List<CHECKBOXVIEW>();
                    foreach (var item in Results)
                    {
                        myCheckBoxList.Add(new CHECKBOXVIEW { Id = item.ROLEID, Name = item.ROLENAME, Checked = item.Checked });
                    }
                    myViewModel.contentRole = myCheckBoxList;
                    return View(myViewModel);
                }
                else
                {

                    //filenya dokumen dan ukurannya <= 10mb
                    string ext = Path.GetExtension(File.FileName);
                    System.Diagnostics.Debug.WriteLine(ext);

                    //menamakan file
                    var nameNew = DateTime.Now.ToString("H-mm-ss_dd-MM-yyyy");

                    //save file ke lokal
                    contentFilePath.SaveAs(path + nameNew.ToString() + Path.GetExtension(File.FileName));
                    string path_relative = VirtualPathUtility.ToAbsolute("~/UserContentFiles/").ToString() + nameNew.ToString() + Path.GetExtension(File.FileName).ToString();
                    System.Diagnostics.Debug.WriteLine(path_relative);

                    //masukkan path ke objek
                    contentTableView.contentFilePath = path_relative.ToString();
                }
            }
            else
            {
                //do nothing
            }

            //insert ke tabel content
            db.CONTENTTABLEs.Add(new CONTENTTABLE() { CONTENTTITLE = contentTableView.contentTitle, CONTENTDESCRIPTION = contentTableView.contentDescription, CONTENTLINK = contentTableView.contentLink, CONTENTFILEPATH = contentTableView.contentFilePath, STATUSID = 1, USERID = Convert.ToInt32(Session["userId"]), CONTENTTYPEID = contentTableView.CONTENTTYPEID.Value, CONTENTKEYWORD = contentTableView.contentKeyword, CONTENTDATE = DateTime.Now});
            foreach (var item in contentTableView.contentRole)
            {
                if (item.Checked)
                {
                    //masukkan ke tabel content role
                    if (db.CONTENTTABLEs.Where(m => m.CONTENTTITLE == contentTableView.contentTitle && m.CONTENTDESCRIPTION == contentTableView.contentDescription) != null)
                    {
                        db.CONTENTROLEs.Add(new CONTENTROLE() { CONTENTID = contentTableView.contentId, ROLEID = item.Id });
                    }
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: CONTENTTABLEs/Edit/5
        public ActionResult Edit(int? id)
        {

            // cek session login
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "USERTABLEs", new { area = "" });
            }

            //cek parameter id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //ambil userrole dari id user
            var uss = Convert.ToInt32(Session["userId"]);
            var konten = from c in db.CONTENTTABLEs
                         join cr in db.CONTENTROLEs
                         on c.CONTENTID equals cr.CONTENTID
                         join ur in db.USERROLEs
                         on cr.ROLEID equals ur.ROLEID
                         where (ur.USERID == uss) & (cr.CONTENTID == id)
                         select c;
            var disc = Enumerable.Distinct(konten);
            var hasil = from all in disc select all;

            //cek apakah role content dimiliki oleh role user
            if (hasil.Count() == 0)
            {
                //user tidak punya role yang sama dengan content
                if (Session["userRole"].ToString() != "admin")
                {
                    //kalau bukan admin, dilempar
                    return RedirectToAction("Index");
                }
            }

            //kalau admin bisa semua

            //cek apakah user berhak mengeditnya
            if (db.CONTENTTABLEs.Where(m => m.CONTENTID == id).Select(c => c.USERID).FirstOrDefault() != uss)
            {
                // kalau contentnya bukan miliknya, ya gak bisa lah
                if (Session["userRole"].ToString() != "admin")
                {
                    //kalau admin, semua content bisa dieditnya
                    return RedirectToAction("Index");
                }

            }

            //divider
            CONTENTTABLE cONTENTTABLE = db.CONTENTTABLEs.Find(id);
            if (cONTENTTABLE == null)
            {
                return HttpNotFound();
            }

            var Results = from r in db.ROLETABLEs
                          select new
                          {
                              r.ROLEID,
                              r.ROLENAME,
                              Checked = ((from ur in db.CONTENTROLEs
                                          where (ur.CONTENTID == id) & (ur.ROLEID == r.ROLEID)
                                          select ur).Count() > 0)
                          };

            //masukkan semua data dari model ke viewmodel
            var myViewModel = new CONTENTTABLEVIEW();
            myViewModel.contentId = id.Value;
            myViewModel.contentTitle = cONTENTTABLE.CONTENTTITLE;
            myViewModel.contentDescription = cONTENTTABLE.CONTENTDESCRIPTION;
            myViewModel.contentLink = cONTENTTABLE.CONTENTLINK;
            myViewModel.contentFilePath = null;
            myViewModel.CONTENTTYPEID = cONTENTTABLE.CONTENTTYPEID;
            myViewModel.USERID = cONTENTTABLE.USERID;
            myViewModel.STATUSID = cONTENTTABLE.STATUSID;
            myViewModel.contentDate = cONTENTTABLE.CONTENTDATE;
            myViewModel.contentKeyword = cONTENTTABLE.CONTENTKEYWORD;

            ViewBag.STATUSID = new SelectList(db.STATUSTABLEs, "STATUSID", "STATUSNAME", cONTENTTABLE.STATUSID);
            ViewBag.CONTENTTYPEID = new SelectList(db.CONTENTTYPETABLEs, "CONTENTTYPEID", "CONTENTTYPENAME", cONTENTTABLE.CONTENTTYPEID);

            var myCheckBoxList = new List<CHECKBOXVIEW>();
            foreach (var item in Results)
            {
                myCheckBoxList.Add(new CHECKBOXVIEW { Id = item.ROLEID, Name = item.ROLENAME, Checked = item.Checked });
            }
            myViewModel.contentRole = myCheckBoxList;
            return View(myViewModel);
        }

        // POST: CONTENTTABLEs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CONTENTTABLEVIEW cONTENTTABLE, HttpPostedFileBase CONTENTFILEPATH, int id)
        {
            string path = Server.MapPath("~/UserContentFiles/");
            if (!Directory.Exists(path))
            {
                //buat direktori apabila tidak ada
                Directory.CreateDirectory(path);
            }

            //cek content title
            if (db.CONTENTTABLEs.Where(m => m.CONTENTTITLE == cONTENTTABLE.contentTitle && m.CONTENTID != cONTENTTABLE.contentId).FirstOrDefault() != null)
            {
                //content title sudah ada
                ViewBag.CONTENTTITLENOT = "Content Title exists";
                var Results = from r in db.ROLETABLEs
                              select new
                              {
                                  r.ROLEID,
                                  r.ROLENAME,
                                  Checked = ((from ur in db.CONTENTROLEs
                                              where (ur.CONTENTID == 0) & (ur.ROLEID == r.ROLEID)
                                              select ur).Count() > 0)
                              };
                var myViewModel = new CONTENTTABLEVIEW();
                ViewBag.CONTENTTYPEID = new SelectList(db.CONTENTTYPETABLEs, "CONTENTTYPEID", "CONTENTTYPENAME", cONTENTTABLE.CONTENTTYPEID);
                ViewBag.STATUSID = new SelectList(db.STATUSTABLEs, "STATUSID", "STATUSNAME", cONTENTTABLE.STATUSID);

                var myCheckBoxList = new List<CHECKBOXVIEW>();
                foreach (var item2 in Results)
                {
                    myCheckBoxList.Add(new CHECKBOXVIEW { Id = item2.ROLEID, Name = item2.ROLENAME, Checked = item2.Checked });
                }
                myViewModel.contentRole = myCheckBoxList;
                return View(myViewModel);
            }

            //cek apakah user input file
            if (CONTENTFILEPATH != null)
            {
                HttpPostedFileBase File = Request.Files["contentFilePath"];
                var supportedTypes = new[] { "txt", "doc", "docx", "pdf", "xls", "xlsx" };
                var fileExt = System.IO.Path.GetExtension(File.FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt))
                {
                    //file bukan dokumen
                    ModelState.AddModelError("contentFilePath", "Only document file types allowed.");
                    var Results = from r in db.ROLETABLEs
                                  select new
                                  {
                                      r.ROLEID,
                                      r.ROLENAME,
                                      Checked = ((from ur in db.CONTENTROLEs
                                                  where (ur.CONTENTID == 0) & (ur.ROLEID == r.ROLEID)
                                                  select ur).Count() > 0)
                                  };
                    var myViewModel = new CONTENTTABLEVIEW();
                    ViewBag.CONTENTTYPEID = new SelectList(db.CONTENTTYPETABLEs, "CONTENTTYPEID", "CONTENTTYPENAME", cONTENTTABLE.CONTENTTYPEID);
                    ViewBag.STATUSID = new SelectList(db.STATUSTABLEs, "STATUSID", "STATUSNAME", cONTENTTABLE.STATUSID);

                    var myCheckBoxList = new List<CHECKBOXVIEW>();
                    foreach (var item2 in Results)
                    {
                        myCheckBoxList.Add(new CHECKBOXVIEW { Id = item2.ROLEID, Name = item2.ROLENAME, Checked = item2.Checked });
                    }
                    myViewModel.contentRole = myCheckBoxList;
                    return View(myViewModel);
                }

                //cek ukuran file
                if (CONTENTFILEPATH.ContentLength > 10000000)
                {

                    //ukuran file > 10mb
                    ModelState.AddModelError("contentFilePath", "Only for <= 10MB file allowed.");
                    var Results = from r in db.ROLETABLEs
                                  select new
                                  {
                                      r.ROLEID,
                                      r.ROLENAME,
                                      Checked = ((from ur in db.CONTENTROLEs
                                                  where (ur.CONTENTID == 0) & (ur.ROLEID == r.ROLEID)
                                                  select ur).Count() > 0)
                                  };
                    var myViewModel = new CONTENTTABLEVIEW();
                    ViewBag.CONTENTTYPEID = new SelectList(db.CONTENTTYPETABLEs, "CONTENTTYPEID", "CONTENTTYPENAME", cONTENTTABLE.CONTENTTYPEID);
                    ViewBag.STATUSID = new SelectList(db.STATUSTABLEs, "STATUSID", "STATUSNAME", cONTENTTABLE.STATUSID);

                    var myCheckBoxList = new List<CHECKBOXVIEW>();
                    foreach (var item2 in Results)
                    {
                        myCheckBoxList.Add(new CHECKBOXVIEW { Id = item2.ROLEID, Name = item2.ROLENAME, Checked = item2.Checked });
                    }
                    myViewModel.contentRole = myCheckBoxList;
                    return View(myViewModel);
                }
                else
                {
                    //tipe file dokumen dan ukurannya <= 10mb
                    CONTENTTABLE cONTENTTABLE2 = db.CONTENTTABLEs.Find(id);
                    string path2 = Server.MapPath(cONTENTTABLE2.CONTENTFILEPATH);
                    if (System.IO.File.Exists(path2))
                    {
                        System.IO.File.Delete(path2);
                    }
                    //get started here
                    string ext = Path.GetExtension(File.FileName);
                    var nameNew = DateTime.Now.ToString("H-mm-ss_dd-MM-yyyy");
                    CONTENTFILEPATH.SaveAs(path + nameNew.ToString() + Path.GetExtension(File.FileName));
                    string path_relative = VirtualPathUtility.ToAbsolute("~/UserContentFiles/").ToString() + nameNew.ToString() + Path.GetExtension(File.FileName).ToString();
                    cONTENTTABLE.contentFilePath = path_relative.ToString();
                }
            }
            else
            {
                //hapus file lama di lokal
                CONTENTTABLE cONTENTTABLE3 = db.CONTENTTABLEs.Find(id);
                string path3 = Server.MapPath(cONTENTTABLE3.CONTENTFILEPATH);
                if (System.IO.File.Exists(path3))
                {
                    System.IO.File.Delete(path3);
                }
                cONTENTTABLE.contentFilePath = null;
            }

            //save data lama
            var item = db.CONTENTTABLEs.Find(cONTENTTABLE.contentId);
            var oldUser = item.USERID;
            var oldContent = item.CONTENTID;
            var oldDate = item.CONTENTDATE;
            //hapus row lama
            db.Entry(item).State = EntityState.Deleted;
            db.SaveChanges();
            //System.Diagnostics.Debug.WriteLine("inserted keyword : " + cONTENTTABLE.contentKeyword);

            //masukkan row baru
            db.CONTENTTABLEs.Add(new CONTENTTABLE() { CONTENTTITLE = cONTENTTABLE.contentTitle, CONTENTDESCRIPTION = cONTENTTABLE.contentDescription, CONTENTLINK = cONTENTTABLE.contentLink, CONTENTFILEPATH = cONTENTTABLE.contentFilePath, CONTENTTYPEID = cONTENTTABLE.CONTENTTYPEID.Value, USERID = Convert.ToInt32(oldUser), STATUSID = cONTENTTABLE.STATUSID.Value, CONTENTDATE = oldDate, CONTENTKEYWORD = cONTENTTABLE.contentKeyword });
            db.SaveChanges();
            var newId = db.CONTENTTABLEs.Where(m => m.CONTENTTITLE == cONTENTTABLE.contentTitle).FirstOrDefault();
            //masukkan role ke content role
            foreach (var item3 in cONTENTTABLE.contentRole)
            {
                if (item3.Checked)
                {
                    db.CONTENTROLEs.Add(new CONTENTROLE() { CONTENTID = newId.CONTENTID, ROLEID = item3.Id });
                }
            }
            //end
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: CONTENTTABLEs/Delete/5
        public ActionResult Delete(int? id)
        {
            //cek session login
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "USERTABLEs", new { area = "" });
            }
            //cek parameter id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //menyamakan role dari user dengan role dari content
            var uss = Convert.ToInt32(Session["userId"]);
            var konten = from c in db.CONTENTTABLEs
                         join cr in db.CONTENTROLEs
                         on c.CONTENTID equals cr.CONTENTID
                         join ur in db.USERROLEs
                         on cr.ROLEID equals ur.ROLEID
                         where (ur.USERID == uss) & (cr.CONTENTID == id)
                         select c;
            var disc = Enumerable.Distinct(konten);
            var hasil = from all in disc select all;
            if (hasil.Count() == 0)
            {
                //role dari konten dengan role dari user berbeda

                //kalau bukan admin tidak diperbolehkan, dilempar
                if (Session["userRole"].ToString() != "admin")
                {
                    return RedirectToAction("Index");
                }
            }

            //cek kepemilikan content
            if (db.CONTENTTABLEs.Where(m => m.CONTENTID == id).Select(c => c.USERID).FirstOrDefault() != uss)
            {
                //kalau bukan admin ataupun konten tersebut miliknya, dilempar
                if (Session["userRole"].ToString() != "admin")
                {
                    return RedirectToAction("Index");
                }

            }

            //cek id di database
            CONTENTTABLE cONTENTTABLE = db.CONTENTTABLEs.Find(id);
            if (cONTENTTABLE == null)
            {
                //id tidak ada di database
                return HttpNotFound();
            }
            return View(cONTENTTABLE);
        }

        // POST: CONTENTTABLEs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CONTENTTABLE cONTENTTABLE = db.CONTENTTABLEs.Find(id);
            string path = Server.MapPath(cONTENTTABLE.CONTENTFILEPATH);
            if (System.IO.File.Exists(path))
            {
                //kalau ada filenya, hapus file di lokal
                System.IO.File.Delete(path);
            }
            db.CONTENTTABLEs.Remove(cONTENTTABLE);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Search(string q)
        {
            //cek paramater untuk search
            if (q == null)
            {
                //parameter kosong
                return Redirect(Url.Action("Index", "CONTENTTABLEs"));
            }
            else
            {
                //cek role
                var uss = Convert.ToInt32(Session["userId"]);
                if (Session["userRole"].ToString() == "admin")
                {
                    //apabila admin, tidak ada batasan terhadap role content
                    var konten = from c in db.CONTENTTABLEs
                                 where ((c.CONTENTTITLE.Contains(q)) | (c.CONTENTDESCRIPTION.Contains(q)) | (c.CONTENTKEYWORD.Contains(q)))
                                 select c;
                    var disc = Enumerable.Distinct(konten);
                    var cONTENTTABLEs = from all in disc select all;
                    return View("Index", cONTENTTABLEs.ToList());
                }
                else
                {
                    //ambil kontent yang sesuai dengan rolenya
                    var konten = from c in db.CONTENTTABLEs
                                 join cr in db.CONTENTROLEs
                                 on c.CONTENTID equals cr.CONTENTID
                                 join ur in db.USERROLEs
                                 on cr.ROLEID equals ur.ROLEID
                                 where (ur.USERID == uss) & ((c.CONTENTTITLE.Contains(q)) | (c.CONTENTDESCRIPTION.Contains(q)) | (c.CONTENTKEYWORD.Contains(q)))
                                 select c;
                    var disc = Enumerable.Distinct(konten);
                    var cONTENTTABLEs = from all in disc select all;
                    return View("Index", cONTENTTABLEs.ToList());
                }
            }
        }

        [HttpGet]
        public FileResult Attachment(int? id)
        {
            //cek parameter id
            if (id == null)
            {
                return null;
            }
            else
            {
                //ada id di parameternya
                var uss = Convert.ToInt32(Session["userId"]);
                var konten = from c in db.CONTENTTABLEs
                             join cr in db.CONTENTROLEs
                             on c.CONTENTID equals cr.CONTENTID
                             join ur in db.USERROLEs
                             on cr.ROLEID equals ur.ROLEID
                             where (ur.USERID == uss) & (cr.CONTENTID == id)
                             select c;
                var disc = Enumerable.Distinct(konten);
                var hasil = from all in disc select all;
                if (hasil.Count() != 0 || Session["userRole"].ToString() == "admin")
                //role user sesuai dengan role konten || user adalah admin
                {
                    CONTENTTABLE att = db.CONTENTTABLEs.Where(m => m.CONTENTID == id).FirstOrDefault();
                    //cek id ke database
                    if (att != null)
                    {
                        string filename = att.CONTENTFILEPATH.ToString();
                        filename = filename.Remove(0, 18);
                        System.Diagnostics.Debug.WriteLine(filename);
                        var reg = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(Path.GetExtension(filename).ToLower());
                        string contentType = "application/unknown";

                        //cek file
                        if (reg != null)
                        {
                            //file ada
                            string registryContentType = reg.GetValue("Content Type") as string;
                            if (!String.IsNullOrWhiteSpace(registryContentType))
                            {
                                //tampilkan file
                                contentType = registryContentType;
                                return new FilePathResult("~/UserContentFiles/" + filename, contentType);
                            }
                        }
                        return null;
                    }
                    return null;
                }
                else
                {
                    //System.Diagnostics.Debug.WriteLine("wes gak admin, role e dee gak cocok sisan karo konten e");
                    return null;
                }
            }
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
