using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using xfactor.Models;

namespace xfactor.Controllers
{
    public class LitigesEtProrogationsController : Controller
    {
        private XFactor_PRODEntities1 db = new XFactor_PRODEntities1();
        // GET: LitigesEtProrogations
        public ActionResult Litiges()
        {
            if (Session["UserLogin"] != null)
            {

                //var ListAdh = (from q in db.T_INDIVIDU
                //               join q2 in db.TJ_CIR
                //               on q.REF_IND equals q2.REF_IND_CIR
                //               where (q2.ID_ROLE_CIR == "ADH")
                //               select new { q.PRE_IND, q2.REF_CTR_CIR, q2.REF_IND_CIR });
                //List<NewDataCollection> op = new List<NewDataCollection>();
                //foreach (var item in ListAdh)
                //{
                //    op.Add(new NewDataCollection(item.REF_CTR_CIR, item.PRE_IND, item.REF_IND_CIR));
                //}
                ViewBag.ADH = db.Recherche_CTR_ADH().ToList();
                TempData["LitigesEtProrogations"] = "active";
                TempData["Litiges"] = "active";
                return View();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }

        [HttpPost]
        public ActionResult Litiges(T_LITIGE f,string MONT_LT)
        {
            try
            {
                TR_TVA tva;
                try
                {
                    tva = db.TR_TVA.OrderByDescending(p => p.ID_TVA).SingleOrDefault();
                }
                catch (Exception e)
                {
                    tva = null;
                }
                T_LITIGE lit = db.T_LITIGE.Where(p => p.ID_DET_BORD_LIT == f.ID_DET_BORD_LIT && p.ETAT_LIT==true).SingleOrDefault();
                if (lit == null)
                {
                    MONT_LT = MONT_LT.Replace(" ", "");
                    MONT_LT = MONT_LT.Replace(".", ",");
                    f.ETAT_LIT = true;
                    f.MONT_LT = decimal.Parse(MONT_LT);
                    db.T_LITIGE.Add(f);
                    db.SaveChanges();
                    decimal frais;
                    try { frais = (decimal)db.T_FRAIS_DIVERS.Where(p => p.TYP_FRAIS_DIVERS.Contains("Lit") && p.REF_CTR_FRAIS_DIVERS == f.REF_CTR_LIT).Select(p => p.MONT_PAR_UNIT_FRAIS_DIVERS).FirstOrDefault(); } catch (Exception) { frais = 0; }
                    T_MVT_DEBIT debit = new T_MVT_DEBIT();
                    debit.REF_CTR_DEBIT = f.REF_CTR_LIT;
                    debit.ABEV_DEBIT = "Litige";
                    debit.MONT_DEBIT = frais*tva.VAL_TVA;
                    debit.DATE_DEBIT = DateTime.Now;
                    db.T_MVT_DEBIT.Add(debit);
                    db.SaveChanges();


                    try
                    {
                        T_EXTRAIT extrait = new T_EXTRAIT();
                        extrait.REF_CTR_EXTRAIT = debit.REF_CTR_DEBIT;
                        extrait.LIB_EXTRAIT = "Debit " + debit.ABEV_DEBIT;
                        extrait.DEBIT_EXTRAIT = 0;
                        extrait.CREDIT_EXTRAIT = 0;
                        extrait.AUTRE_EXTRAIT = debit.MONT_DEBIT;
                        extrait.ENCOURFACT_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Encours_Facture).FirstOrDefault();
                        extrait.DISPONIBLE_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Disponible).FirstOrDefault();
                        extrait.DAT_OP_EXTRAIT = DateTime.Now;
                        extrait.DAT_VAL_EXTRAIT = debit.DATE_DEBIT;
                        extrait.TOTAL_FIN_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Total_Financement).FirstOrDefault();
                        db.T_EXTRAIT.Add(extrait);
                        db.SaveChanges();

                    }
                    catch (Exception) { }


                    TempData["notice"] = "save";
                }
                else
                {

                    TempData["error"] = "ce facture est deja en litige";
                }
            }
            catch (Exception) { TempData["error"] = "Erreur"; return RedirectToAction("InternalServerError", "Error"); }
            return RedirectToAction("Litiges");
        }

        public JsonResult RecherchCtrAch22(int id)
        {
            var ListNomInd = db.Recherche_CTR_ADH_ACH_Par_CTR2019(id);
            return Json(ListNomInd, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult RecherchFacture(int id, int id2)
        {
            ViewBag.test = db.ListeFactureValiderk(id, id2);
            return Json(ViewBag.test, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RecherchFactureVersion2(int id, int id2)
        {
            ViewBag.test = db.ListeFactureValider1(id, id2);
            return Json(ViewBag.test, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Prorogations()
        {
            if (Session["UserLogin"] != null)
            {

                //var ListAdh = (from q in db.T_INDIVIDU
                //               join q2 in db.TJ_CIR
                //               on q.REF_IND equals q2.REF_IND_CIR
                //               where (q2.ID_ROLE_CIR == "ADH")
                //               select new { q.PRE_IND, q2.REF_CTR_CIR, q2.REF_IND_CIR });
                //List<NewDataCollection> op = new List<NewDataCollection>();
                //foreach (var item in ListAdh)
                //{
                //    op.Add(new NewDataCollection(item.REF_CTR_CIR, item.PRE_IND, item.REF_IND_CIR));
                //}
                ViewBag.ADH = db.Recherche_CTR_ADH().ToList();
                TempData["LitigesEtProrogations"] = "active";
                TempData["Prorogations"] = "active";
                return View();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }

        [HttpPost]
        public ActionResult Prorogations(T_PROROGATION f,DateTime Echeance_Facture_Pro)
        {
            TR_TVA tva;
            try
            {
                tva = db.TR_TVA.OrderByDescending(p => p.ID_TVA).SingleOrDefault();
            }
            catch (Exception e)
            {
                tva = null;
            }
            try
            {
                f.ETAT_PROG = true;
                f.DAT_PROG = Echeance_Facture_Pro;
                db.T_PROROGATION.Add(f);
                db.SaveChanges();
                TimeSpan ts = f.ECH_PROG - Echeance_Facture_Pro;
                short diff = (short)ts.TotalDays;
                T_DET_BORD bord = db.T_DET_BORD.Where(p => p.ID_DET_BORD == f.ID_DET_BORD_PROG.ToString()).FirstOrDefault();
                bord.ECH_APR_PROROG_DET_BORD = Echeance_Facture_Pro;
                bord.ECH_DET_BORD += diff;
                db.Entry(bord).State = EntityState.Modified;
                db.SaveChanges();
                decimal frais;
                try { frais = (decimal)db.T_FRAIS_DIVERS.Where(p => p.TYP_FRAIS_DIVERS.Contains("ProL") && p.REF_CTR_FRAIS_DIVERS == f.REF_CTR_PROG).Select(p => p.MONT_PAR_UNIT_FRAIS_DIVERS).FirstOrDefault(); } catch (Exception) { frais = 0; }
                 //0.19
                T_MVT_DEBIT debit = new T_MVT_DEBIT();
                debit.REF_CTR_DEBIT = f.REF_CTR_PROG;
                debit.TYP_DEBIT = bord.ID_DET_BORD;
                debit.ABEV_DEBIT = "Prorogation Litige";
                debit.MONT_DEBIT = frais*tva.VAL_TVA;
                debit.DATE_DEBIT = DateTime.Now;
                db.T_MVT_DEBIT.Add(debit);
                db.SaveChanges();


                try
                {
                    T_EXTRAIT extrait = new T_EXTRAIT();
                    extrait.REF_CTR_EXTRAIT = debit.REF_CTR_DEBIT;
                    extrait.LIB_EXTRAIT = "Debit " + debit.ABEV_DEBIT;
                    extrait.DEBIT_EXTRAIT = 0;
                    extrait.CREDIT_EXTRAIT = 0;
                    extrait.AUTRE_EXTRAIT = debit.MONT_DEBIT;
                    extrait.ENCOURFACT_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Encours_Facture).FirstOrDefault();
                    extrait.DISPONIBLE_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Disponible).FirstOrDefault();
                    extrait.DAT_OP_EXTRAIT = DateTime.Now;
                    extrait.DAT_VAL_EXTRAIT = debit.DATE_DEBIT;
                    extrait.TOTAL_FIN_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Total_Financement).FirstOrDefault();
                    db.T_EXTRAIT.Add(extrait);
                    db.SaveChanges();

                }
                catch (Exception) { }
                try
                {
                    T_HISTORIQUE histo = new T_HISTORIQUE();
                    histo.ABREV_ROLE_HIST = "prorogation pour " + bord.ID_DET_BORD;
                    histo.ACTION = "Creation prorogation";
                    histo.ID_ENREGISTREMENT = bord.NUM_BORD.ToString();
                    histo.T_TABLE = "T_PROROGATION";
                    histo.REF_CTR_HIST = bord.REF_CTR_DET_BORD.ToString();
                    histo.REF_IND_HIST = db.TJ_CIR.Where(p=>p.REF_CTR_CIR==bord.REF_CTR_DET_BORD && p.ID_ROLE_CIR=="ADH").SingleOrDefault().REF_IND_CIR.ToString();
                    histo.LOGIN_USER = Session["UserLogin"].ToString();
                    histo.IP_PC = GetIp();
                    histo.NOM_PC = HttpContext.Request.UserHostName;
                    histo.DATE_ACTION = DateTime.Now;
                    db.T_HISTORIQUE.Add(histo);
                    db.SaveChanges();
                }
                catch (Exception e) { TempData["error"] = e.Message; }

                TempData["notice"] = " la prorogation de facture n°" + db.TJ_DOCUMENT_DET_BORD.Where(p => p.ID_DET_BORD == f.ID_DET_BORD_PROG.ToString()).Select(p => p.REF_DOCUMENT_DET_BORD).SingleOrDefault() + " de contart n° " + bord.REF_CTR_DET_BORD + " a ete sauvgarder ";
            }
            catch (Exception) { TempData["error"] = "Erreur"; return RedirectToAction("InternalServerError", "Error"); }
            return RedirectToAction("Prorogations");
        }
        public string GetIp()
        {
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;
        }

        public ActionResult ResolutionLitiges()
        {
            if (Session["UserLogin"] != null)
            {

                ViewBag.lit = db.usp_Rapport_Factures_En_Litige_VersionII().ToList();
                TempData["LitigesEtProrogations"] = "active";
                TempData["ResolutionLitiges"] = "active";
                return View();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }

        public ActionResult Resolu(int id)
        {
            try
            {
                T_LITIGE li = db.T_LITIGE.Where(p => p.ID_LITIGE == id).FirstOrDefault();
                li.ETAT_LIT = false;
                db.Entry(li).State = EntityState.Modified;
                db.SaveChanges();
                TempData["notice"] = "le litige est resolu";
            }
            catch (Exception) { TempData["error"] = "Erreur"; return RedirectToAction("InternalServerError", "Error"); }
       
            return   RedirectToAction("ResolutionLitiges");
        }
    }
}
//tva
//