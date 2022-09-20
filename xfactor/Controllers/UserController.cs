using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using xfactor.Models;

namespace xfactor.Controllers
{
    public class UserController : Controller
    {
        private XFactor_PRODEntities1 db = new XFactor_PRODEntities1();
        // GET: User
        
        public ActionResult Users()
        {
            if (Session["UserLogin"] != null)
            {
               // TempData["message"] = "";
                ViewBag.ListUsers = db.TS_USER.ToList();
                ViewBag.ListUser = null;
              //  TempData["User"] = "active";
                TempData["Users"] = "active";
                return View();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }

        public ActionResult Partial1(int id)
        {
            if (Session["UserLogin"] != null)
            {
                TS_USER user = new TS_USER();
                user = db.TS_USER.Find(id);
                ViewBag.listGroupUser = new SelectList(db.TS_GRP_USER.ToList(), "ID_GRP_USER", "LIB_GRP_USER");
                return PartialView("Partial1", user);
            }
            else
            {
                return RedirectToAction("login", "Login");
            }

        }
        [HttpPost]
        public ActionResult Partial1(TS_USER users)
        {
          

            if (users.ImageFile != null)
            {
                string filename = Path.GetFileNameWithoutExtension(users.ImageFile.FileName);
                string extention = Path.GetExtension(users.ImageFile.FileName);
                users.PHOTO_USER = "/Content/img/" + filename + extention;
                filename = Path.Combine(Server.MapPath("~/Content/img/"), filename + extention);
                users.ImageFile.SaveAs(filename);
                users.PHOTO_USER = "/Content/img/" + users.ImageFile.FileName;
            }

            string sql = "update TS_USER set ID_GRP_USER={0} , NOM_USER={1}, PRE_USER={2},LOGIN_USER={3},MDP_USER={4},ACTIF_USER={5},FONCTION_USER={6},SERVICE_USER={7},DIRECTION_USER={8},AGENCE_USER={9},MAIL_USER={10},TEL_FIXE_USER={11},MOBILE_USER={12},PHOTO_USER={13} where ID_USER={14}";
          db.Database.ExecuteSqlCommand(sql, users.ID_GRP_USER, users.NOM_USER, users.PRE_USER, users.LOGIN_USER, users.MDP_USER, users.ACTIF_USER, users.FONCTION_USER, users.SERVICE_USER, users.DIRECTION_USER, users.AGENCE_USER, users.MAIL_USER, users.TEL_FIXE_USER, users.MOBILE_USER, users.PHOTO_USER, users.ID_USER);
            ViewBag.listGroupUser = new SelectList(db.TS_GRP_USER.ToList(), "ID_GRP_USER", "LIB_GRP_USER");
            TempData["message"] = "modification a été effectué avec succès";
            return RedirectToAction("Users");
        }

        public ActionResult AddUserPartial()
        {
            if (Session["UserLogin"] != null)
            {
                ViewBag.listGroupUser = new SelectList(db.TS_GRP_USER.ToList(), "ID_GRP_USER", "LIB_GRP_USER");
                return PartialView();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }

        }
        [HttpPost]
        public ActionResult AddUserPartial(TS_USER user)
        {
            if (user.ImageFile != null)
            {
                string filename = Path.GetFileNameWithoutExtension(user.ImageFile.FileName);
                string extention = Path.GetExtension(user.ImageFile.FileName);
                user.PHOTO_USER = "/Content/img/" + filename + extention;
                filename = Path.Combine(Server.MapPath("~/Content/img/"), filename + extention);
                user.ImageFile.SaveAs(filename);
            }
            else
            {

                user.PHOTO_USER = "/Content/img/user_med.png";

            }
            string sql = "insert into TS_USER (ID_GRP_USER,NOM_USER,PRE_USER,LOGIN_USER,MDP_USER,ACTIF_USER,FONCTION_USER,SERVICE_USER,DIRECTION_USER,AGENCE_USER,MAIL_USER,TEL_FIXE_USER,MOBILE_USER,PHOTO_USER ) values({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13})";
            try
            {
                db.Database.ExecuteSqlCommand(sql, user.ID_GRP_USER, user.NOM_USER, user.PRE_USER, user.LOGIN_USER, user.MDP_USER, user.ACTIF_USER, user.FONCTION_USER, user.SERVICE_USER, user.DIRECTION_USER, user.AGENCE_USER, user.MAIL_USER, user.TEL_FIXE_USER, user.MOBILE_USER, user.PHOTO_USER);
            }
            catch (Exception e) { }
                ViewBag.listGroupUser = new SelectList(db.TS_GRP_USER.ToList(), "ID_GRP_USER", "LIB_GRP_USER");
            TempData["message"] = "save";
            return RedirectToAction("Users");
        }
    }
}