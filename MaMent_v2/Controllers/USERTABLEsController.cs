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
using System.Security.Cryptography;
using MaMent_v2.Helper;
using System.Configuration;

namespace MaMent_v2.Controllers
{
    public class USERTABLEsController : Controller
    {
        private MaMent_DB_v2Entities db = new MaMent_DB_v2Entities();

        // GET: USERTABLEs
        public ActionResult Index()
        {
            //cek session login
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "USERTABLEs", new { area = "" });
            }
            //kalau bukan admin, lempar
            if (Session["userRole"].ToString() != "admin")
            {
                return RedirectToAction("Index", "CONTENTTABLEs", new { area = "" });
            }
            var uSERTABLEs = db.USERTABLEs.Include(c => c.STATUSTABLE);
            return View(uSERTABLEs.ToList());
        }

        // GET: USERTABLEs/Details/5
        public ActionResult Details(int? id)
        {
            // cek id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //cek session login
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "USERTABLEs", new { area = "" });
            }

            //cek user role
            if (Session["userRole"].ToString() != "admin")
            {
                //bukan admin, lempar
                return RedirectToAction("Index", "CONTENTTABLEs", new { area = "" });
            }
            USERTABLE uSERTABLE = db.USERTABLEs.Where(m => m.USERID == id).Include(m => m.STATUSTABLE).FirstOrDefault();

            //cek hasil query
            if (uSERTABLE == null)
            {
                return HttpNotFound();
            }

            //alasan keamanan
            uSERTABLE.USERPASSWORD = null;

            //cek role user
            var Results = from r in db.ROLETABLEs
                          select new
                          {
                              r.ROLEID,
                              r.ROLENAME,
                              Checked = ((from ur in db.USERROLEs
                                          where (ur.USERID == id) & (ur.ROLEID == r.ROLEID)
                                          select ur).Count() > 0)
                          };
            var myViewModel = new USERTABLEVIEW2();
            myViewModel.userId = id.Value;
            myViewModel.userName = uSERTABLE.USERNAME;
            myViewModel.userPassword = null;
            myViewModel.STATUSNAME = uSERTABLE.STATUSTABLE.STATUSNAME;
            ViewBag.STATUSID = new SelectList(db.STATUSTABLEs, "STATUSID", "STATUSNAME", uSERTABLE.STATUSID);

            //memasukkan role user ke checkbox
            var myCheckBoxList = new List<CHECKBOXVIEW>();
            foreach (var item in Results)
            {
                myCheckBoxList.Add(new CHECKBOXVIEW { Id = item.ROLEID, Name = item.ROLENAME, Checked = item.Checked });
            }
            myViewModel.userRole = myCheckBoxList;
            return View(myViewModel);
        }

        // GET: USERTABLEs/Create
        [HttpGet]
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
            USERTABLE uSERTABLE = new USERTABLE();

            //masukkan data dari model role ke view model user
            var Results = from r in db.ROLETABLEs
                          select new
                          {
                              r.ROLEID,
                              r.ROLENAME,
                              Checked = false
                          };
            var myViewModel = new USERTABLEVIEW2();
            ViewBag.STATUSID = new SelectList(db.STATUSTABLEs, "STATUSID", "STATUSNAME", uSERTABLE.STATUSID);

            var myCheckBoxList = new List<CHECKBOXVIEW>();
            foreach (var item in Results)
            {
                myCheckBoxList.Add(new CHECKBOXVIEW { Id = item.ROLEID, Name = item.ROLENAME, Checked = item.Checked });
            }
            myViewModel.userRole = myCheckBoxList;
            return View(myViewModel);
        }

        // POST: USERTABLEs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(USERTABLEVIEW2 userTableView2)
        {
            //cek role user
            if (Session["userRole"].ToString() != "admin")
            {
                return RedirectToAction("Index", "CONTENTTABLEs", new { area = "" });
            }

            //cek ketersediaan username
            if (db.USERTABLEs.Where(m => m.USERNAME == userTableView2.userName).FirstOrDefault() != null)
            {
                //username tidak tersedia
                ViewBag.USERNAMENOT = "Username exists";
                var Results = from r in db.ROLETABLEs
                              select new
                              {
                                  r.ROLEID,
                                  r.ROLENAME,
                                  Checked = false
                              };
                var myCheckBoxList = new List<CHECKBOXVIEW>();
                foreach (var item in Results)
                {
                    myCheckBoxList.Add(new CHECKBOXVIEW { Id = item.ROLEID, Name = item.ROLENAME, Checked = item.Checked });
                }
                userTableView2.userRole = myCheckBoxList;
                return View(userTableView2);
            }
            else
            {
                //username tersedia

                //hashing
                var hashed_pw = PasswordHelper.ComputeHash(userTableView2.userPassword, "SHA512", GetBytes("my_secret"));
                db.USERTABLEs.Add(new USERTABLE() { USERNAME = userTableView2.userName, USERPASSWORD = hashed_pw, STATUSID = 1 });
                db.SaveChanges();
                var newUser = db.USERTABLEs.Where(m => m.USERNAME == userTableView2.userName).Select(m => m.USERID).FirstOrDefault();
                //insert role user
                foreach (var item in userTableView2.userRole)
                {
                    if (item.Checked)
                    {
                        db.USERROLEs.Add(new USERROLE() { USERID = newUser, ROLEID = item.Id });
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        // GET: USERTABLEs/Edit/5
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
                //bukan admin, dilempar ke index content
                return RedirectToAction("Index", "CONTENTTABLEs", new { area = "" });
            }

            //cek parameter id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //cari user melalui id
            USERTABLE uSERTABLE = db.USERTABLEs.Find(id);

            //cek user id di database
            if (uSERTABLE == null)
            {
                return HttpNotFound();
            }

            //user ber-ID 1 adalah super admin sehingga tidak diperbolehkan diedit kecuali dari user ber-ID 1 itu sendiri
            if (Convert.ToInt32(Session["userId"]) != 1 && id == 1)
            {
                return RedirectToAction("Index", "USERTABLEs");
            }

            //simpan data dari tabel role ke view model user
            var Results = from r in db.ROLETABLEs
                          select new
                          {
                              r.ROLEID,
                              r.ROLENAME,
                              Checked = ((from ur in db.USERROLEs
                                          where (ur.USERID == id) & (ur.ROLEID == r.ROLEID)
                                          select ur).Count() > 0)
                          };
            var myViewModel = new USERTABLEVIEW2();
            myViewModel.userId = id.Value;
            myViewModel.userName = uSERTABLE.USERNAME;
            myViewModel.userPassword = null;
            ViewBag.STATUSID = new SelectList(db.STATUSTABLEs, "STATUSID", "STATUSNAME", uSERTABLE.STATUSID);

            //madusukkan role ke checkbox
            var myCheckBoxList = new List<CHECKBOXVIEW>();
            foreach (var item in Results)
            {
                myCheckBoxList.Add(new CHECKBOXVIEW { Id = item.ROLEID, Name = item.ROLENAME, Checked = item.Checked });
            }
            myViewModel.userRole = myCheckBoxList;
            return View(myViewModel);
        }

        // POST: USERTABLEs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(USERTABLEVIEW2 uSERTABLE)
        {
            //cek role user, apabila bukan user maka tidak diperbolehkan
            if (Session["userRole"].ToString() != "admin")
            {
                return RedirectToAction("Index", "CONTENTTABLEs", new { area = "" });
            }

            //cek ketersediaan username
            if (db.USERTABLEs.Where(m => m.USERNAME == uSERTABLE.userName && m.USERID != uSERTABLE.userId).FirstOrDefault() != null)
            {
                ViewBag.USERNAMENOT = "Username exists";
                ViewBag.STATUSID = new SelectList(db.STATUSTABLEs, "STATUSID", "STATUSNAME", uSERTABLE.STATUSID);
                uSERTABLE.userPassword = null;
                return View(uSERTABLE);
            }


            if (ModelState.IsValid)
            {
                //cek row lama di database
                var myUser = db.USERTABLEs.Find(uSERTABLE.userId);
                myUser.USERNAME = uSERTABLE.userName;
                //hashing
                myUser.USERPASSWORD = PasswordHelper.ComputeHash(uSERTABLE.userPassword, "SHA512", GetBytes("my_secret"));

                //opsional, user ber-ID 1 akan selalu berstatus aktif
                if (myUser.USERID == 1)
                {
                    myUser.STATUSID = 1;
                }
                else
                {
                    //user ber-ID selain 1 akan mengikuti status yang telah dipilih di form sebelumnya
                    myUser.STATUSID = uSERTABLE.STATUSID.Value;
                }

                //hapus role user di database
                foreach (var item in db.USERROLEs)
                {
                    if (item.USERID == uSERTABLE.userId)
                    {
                        db.Entry(item).State = EntityState.Deleted;
                    }
                }

                //masukkan role user yang baru
                foreach (var item in uSERTABLE.userRole)
                {
                    if (item.Checked)
                    {
                        db.USERROLEs.Add(new USERROLE() { USERID = uSERTABLE.userId, ROLEID = item.Id });
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.STATUSID = new SelectList(db.STATUSTABLEs, "STATUSID", "STATUSNAME", uSERTABLE.STATUSID);
            return View(uSERTABLE);
        }

        // GET: USERTABLEs/Delete/5
        public ActionResult Delete(int? id)
        {
            //cek parameter id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            USERTABLE uSERTABLE = db.USERTABLEs.Find(id);
            //cek id user di database
            if (uSERTABLE == null)
            {
                // id user di database tidak ditemukan
                return HttpNotFound();
            }

            //cek super admin
            if (id == 1)
            {
                //super admin tidak bisa didelete
                return RedirectToAction("Index", "USERTABLEs");
            }
            return View(uSERTABLE);
        }

        // POST: USERTABLEs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            USERTABLE uSERTABLE = db.USERTABLEs.Find(id);
            //hapus user
            db.USERTABLEs.Remove(uSERTABLE);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Login()
        {
            //cek session login
            if (Session["userId"] != null)
            {
                //apabila session login telah ada, redirect ke index content
                return RedirectToAction("Index", "CONTENTTABLEs", new { area = "" });
            }

            USERTABLEVIEW userTableView = new USERTABLEVIEW();
            userTableView.USER = new USERTABLE();
            return View("Login", userTableView);
        }

        [HttpPost]
        public ActionResult Login(USERTABLEVIEW userTableView)
        {
            //cek inputan user (kosong)
            if (userTableView.USER.USERNAME == null || userTableView.USER.USERPASSWORD == null)
            {
                if (userTableView.USER.USERNAME == null)
                {
                    ViewBag.USERNAME = "Username is required";
                }
                if (userTableView.USER.USERPASSWORD == null)
                {
                    ViewBag.USERPASSWORD = "Password is required";
                }
                return View();
            }
            userTableView.USER.STATUSID = 1;
            //cek username di database
            USERTABLE accountDB = db.USERTABLEs.Where(m => m.USERNAME == userTableView.USER.USERNAME && m.STATUSID == 1).FirstOrDefault();
            if (accountDB != null)
            {
                //username ada

                //verifikasi password
                if (PasswordHelper.VerifyHash(userTableView.USER.USERPASSWORD, "SHA512", accountDB.USERPASSWORD))
                {
                    //simpan id user ke session
                    Session["userId"] = Convert.ToInt32(accountDB.USERID);

                    //cek role dari user
                    if (db.USERROLEs.Where(m => m.USERID == accountDB.USERID && m.ROLEID == 1).Select(m => m.ROLEID).FirstOrDefault() != null)
                    {
                        //user merupakan admin
                        Session["userRole"] = "admin";
                    }
                    else
                    {
                        //user bukan admmin
                        Session["userRole"] = "non-admin";
                    }
                    return RedirectToAction("Index", "CONTENTTABLEs", new { area = "" });
                }
                else
                {
                    //password salah
                    ViewBag.ErrorMessage = "Login Failed";
                    return View();
                }
            }
            else
            {
                //username tidak ada
                ViewBag.ErrorMessage = "Login Failed";
                return View();
            }
        }

        private static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        [HttpGet]
        public ActionResult Logout()
        {
            //hapus session
            Session.Abandon();
            return RedirectToAction("Login");
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
