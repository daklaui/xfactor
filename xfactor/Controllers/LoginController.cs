using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using xfactor.Models;
namespace xfactor.Controllers
{
   
    public class LoginController : Controller
    {
        private XFactor_PRODEntities1 db = new XFactor_PRODEntities1();
        // GET: Login
        public ActionResult login()
        {
            return View();
        }
        static bool IsValid(string value)
        {
            return Regex.IsMatch(value, @"^[a-zA-Z0-9]*$");
        }
        [HttpPost]
        public ActionResult login(TS_USER user)
        {
            if (IsValid(user.LOGIN_USER) && IsValid(user.MDP_USER))
            {
                TS_USER use = new TS_USER();
                try
                {
                    use = db.TS_USER.Where(p => p.LOGIN_USER == user.LOGIN_USER && p.MDP_USER == user.MDP_USER && p.ACTIF_USER == true).FirstOrDefault();
                  //  use.ONE_SIGNAL_PLAYER_ID = user.ONE_SIGNAL_PLAYER_ID;
                  //  db.Entry(use).State = System.Data.Entity.EntityState.Modified;
                 //   db.SaveChanges();

                }
                catch (Exception e) { TempData["message"] = "veuillez vérifier votre connexion réseau ou bien contacter l'administrateur";

                    return View(); 
                }
                if (use != null)
                {

                    try
                    {
                        Session.Timeout = 200;
                        Session["Notification"] = db.T_BORDEREAU.Where(p => p.DAT_SAISIE_BORD == DateTime.Today).Count();
                        Session["UserLogin"] = use.LOGIN_USER.ToString();
                        Session["GroupeName"] = use.DIRECTION_USER.ToString();
                        Session["ID_USER"] = use.ID_USER.ToString();
                        Session["GED_ATTV"] = db.T_DOC_GED_VALID().Count();
                        Session["GED_ATT"] = db.T_DOC_GED_SCAN().Count();
                        if (use.PHOTO_USER != null)
                        {
                            Session["img"] = use.PHOTO_USER.ToString();
                        }
                        else
                        {
                            Session["img"] = "~/Content/img/user_med.png";
                        }
                        Session["CountNotification"] = db.T_RELANCE.Where(e => e.DATE_RELANCE == DateTime.Today).Count();
                        try
                        {
                            Session["derniercnx"] = db.T_HISTORIQUE.Where(p => p.LOGIN_USER == use.ID_USER.ToString() && p.ACTION == "Connexion").OrderByDescending(p => p.DATE_ACTION).Select(p => p.DATE_ACTION).FirstOrDefault();
                        }
                        catch (Exception e)
                        {
                            Session["derniercnx"] = DateTime.Now;
                        }
                        Session["UserPass"] = use.MDP_USER.ToString();
                        T_HISTORIQUE hist = new T_HISTORIQUE();
                        hist.DATE_ACTION = DateTime.Now;
                        hist.ACTION = "Connexion";
                        hist.T_TABLE = "T_USER";
                        // hist.ID_ENREGISTREMENT = db.T_FACTOR.Where(p => p.RC == fact.RC).Select(p => p.ID_FACTOR.ToString()).FirstOrDefault();
                        hist.LOGIN_USER = Session["ID_USER"].ToString();
                        hist.NOM_PC = Dns.GetHostName();
                        hist.IP_PC = Dns.GetHostByName(hist.NOM_PC).AddressList[0].ToString();
                        db.T_HISTORIQUE.Add(hist);
                        //   db.SaveChanges();
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }
                    catch (Exception e)
                    {
                        TempData["message"] = "verifier votre login ou mot de passe";
                        return View();
                    }

                  

                }

                else
                {
                    TempData["message"] = "verifier votre login ou mot de passe";

                }

            }
            else
            {
                TempData["message"] = "Le format de la chaine d'entrée est incorrect";

            }
            
           
            return View();
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            T_HISTORIQUE hist = new T_HISTORIQUE();
            hist.DATE_ACTION = DateTime.Now;
            hist.ACTION = "Déconnexion";
            hist.T_TABLE = "T_USER";
            // hist.ID_ENREGISTREMENT = db.T_FACTOR.Where(p => p.RC == fact.RC).Select(p => p.ID_FACTOR.ToString()).FirstOrDefault();
            hist.LOGIN_USER = Session["ID_USER"].ToString();
            hist.NOM_PC = Dns.GetHostName();
            hist.IP_PC = Dns.GetHostByName(hist.NOM_PC).AddressList[0].ToString();
            db.T_HISTORIQUE.Add(hist);
            //db.SaveChanges();
            return RedirectToAction("login", "Login");
        }
    }
}