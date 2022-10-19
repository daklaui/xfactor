using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using xfactor.Models;

namespace xfactor.Controllers
{
    public class ParametrageController : Controller
    {
        private XFactor_PRODEntities1 db = new XFactor_PRODEntities1();
       
        // GET: Parametrage
        public ActionResult Factor()
        {
            if (Session["UserLogin"] != null)
            {
                ViewBag.Devis = new SelectList(db.TR_NLDP.ToList(), "ID_NLDP", "LIB_DEVISE");
                ViewBag.Lang = new SelectList(db.TR_NLDP.ToList(), "ID_NLDP", "ABRV_LANG");

                TempData["Parametrage"] = "active";
                TempData["Factor"] = "active";
                // ViewBag.message = "";
                return View();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
        [HttpPost]
        public ActionResult Factor(T_FACTOR fact)
        {
            db.T_FACTOR.Add(fact);
            db.SaveChanges();
            ViewBag.Devis = new SelectList(db.TR_NLDP.ToList(), "ID_NLDP", "LIB_DEVISE");
            ViewBag.Lang = new SelectList(db.TR_NLDP.ToList(), "ID_NLDP", "ABRV_LANG");
            T_HISTORIQUE hist = new T_HISTORIQUE();
            hist.DATE_ACTION = DateTime.Now;
            hist.ACTION = "Ajout";
            hist.T_TABLE = "T_FACTOR";
            hist.ID_ENREGISTREMENT = db.T_FACTOR.Where(p => p.RC == fact.RC).Select(p => p.ID_FACTOR.ToString()).FirstOrDefault();
            hist.LOGIN_USER = Session["ID_USER"].ToString();
            hist.NOM_PC = Dns.GetHostName();
            hist.IP_PC= Dns.GetHostByName(hist.NOM_PC).AddressList[0].ToString();
            db.T_HISTORIQUE.Add(hist);
            db.SaveChanges();
            TempData["message"] = "save";
            return RedirectToAction("Factor");
        }
        public ActionResult ListFactor()
        {
            if (Session["UserLogin"] != null)
            {
                return View(db.T_FACTOR.ToList());
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }


        public ActionResult ListVal()
        {
            if (Session["UserLogin"] != null)
            {
                ViewBag.ListValeur = db.TR_LIST_VAL.Where(m => m.TYP_LIST_VAL == "Statut_Contrat").ToList();
                ViewBag.VilleGouvLang = db.TR_CP.ToList();
                ViewBag.Profe = db.TR_ACTPROF_BCT.ToList();
                ViewBag.agb = db.TR_Ag_Bq.ToList();
                ViewBag.commf = db.TR_COMM_FACTORING.ToList();
                ViewBag.nat = db.TR_NLDP.ToList();
                ViewBag.frais_d = db.TR_LISTE_FRAIS_DIVERS.ToList();
                ViewBag.interetfin = db.TR_INT_FINANCEMENT.ToList();
                ViewBag.tmm = db.TR_TMM.ToList();

                TempData["Parametrage"] = "active";
                TempData["ListVal2"] = "active";
                return View();
              
            }
            else
            {
                return RedirectToAction("login", "Login");
            }

        }

        public ActionResult DisplayTMM()
        {
         
            return PartialView(db.TR_TMM.ToList());

        }

        public ActionResult PartieListValeur()
        {
            if (Session["UserLogin"] != null)
            {
                return PartialView();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
        [HttpPost]
        public ActionResult PartieListValeur(TR_LIST_VAL val)
        {
            db.TR_LIST_VAL.Add(val);
            db.SaveChanges();
            T_HISTORIQUE hist = new T_HISTORIQUE();
            hist.DATE_ACTION = DateTime.Now;
            hist.ACTION = "Ajout";
            hist.T_TABLE = "TR_LIST_VAL";
            hist.ID_ENREGISTREMENT = db.TR_LIST_VAL.Max(p=>p.ID_LIST_VAL).ToString();
            hist.LOGIN_USER = Session["ID_USER"].ToString();
            hist.NOM_PC = Dns.GetHostName();
            hist.IP_PC = Dns.GetHostByName(hist.NOM_PC).AddressList[0].ToString();
            db.T_HISTORIQUE.Add(hist);
            db.SaveChanges();
            TempData["listval"] = "save";
            TempData["listevaleur"] = "active";
            return RedirectToAction("ListVal");
        }

        public ActionResult PartieUpdateListVal(int id)
        {
            if (Session["UserLogin"] != null)
            {
                TR_LIST_VAL val = new TR_LIST_VAL();
                val = db.TR_LIST_VAL.Find(id);
                return PartialView(val);
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
        [HttpPost]
        public ActionResult PartieUpdateListVal(TR_LIST_VAL val)
        {
            db.Entry(val).State = EntityState.Modified;
            db.SaveChanges();
            T_HISTORIQUE hist = new T_HISTORIQUE();
            hist.DATE_ACTION = DateTime.Now;
            hist.ACTION = "Modification";
            hist.T_TABLE = "TR_LIST_VAL";
            hist.ID_ENREGISTREMENT = db.TR_LIST_VAL.Where(p => p.ID_LIST_VAL == val.ID_LIST_VAL).Select(p => p.ID_LIST_VAL).FirstOrDefault().ToString();
            hist.LOGIN_USER = Session["ID_USER"].ToString();
            hist.NOM_PC = Dns.GetHostName();
            hist.IP_PC = Dns.GetHostByName(hist.NOM_PC).AddressList[0].ToString();
            db.T_HISTORIQUE.Add(hist);
            db.SaveChanges();
            TempData["listval"] = "save";
            TempData["listevaleur"] = "active";
            return RedirectToAction("ListVal");
        }

        public ActionResult PartieListCp()
        {
            if (Session["UserLogin"] != null)
            {
                return PartialView();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
        [HttpPost]
        public ActionResult PartieListCp(TR_CP cp)
        {
           
                db.TR_CP.Add(cp);
                db.SaveChanges();
                T_HISTORIQUE hist = new T_HISTORIQUE();
                hist.DATE_ACTION = DateTime.Now;
                hist.ACTION = "Ajout";
                hist.T_TABLE = "TR_CP";
                hist.ID_ENREGISTREMENT = db.TR_CP.Max(p => p.ID_CP).ToString();
                hist.LOGIN_USER = Session["ID_USER"].ToString();
                hist.NOM_PC = Dns.GetHostName();
                hist.IP_PC = Dns.GetHostByName(hist.NOM_PC).AddressList[0].ToString();
                db.T_HISTORIQUE.Add(hist);
                db.SaveChanges();
                TempData["listval"] = "save";
            TempData["CodeCp"] = "active";
            return RedirectToAction("ListVal");
         
        }

        public ActionResult PartieUpdateCp(int id)
        {
            if (Session["UserLogin"] != null)
            {
                TR_CP val = new TR_CP();
                val = db.TR_CP.Find(id);
                return PartialView(val);
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
        [HttpPost]
        public ActionResult PartieUpdateCp(TR_CP val)
        {
            db.Entry(val).State = EntityState.Modified;
            db.SaveChanges();
            TempData["listval"] = "save";
            TempData["CodeCp"] = "active";
            return RedirectToAction("ListVal");
        }


        public ActionResult PartieListProf()
        {
            if (Session["UserLogin"] != null)
            {
                return PartialView();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
        [HttpPost]
        public ActionResult PartieListProf(TR_ACTPROF_BCT cp)
        {
            db.TR_ACTPROF_BCT.Add(cp);
            db.SaveChanges();
            T_HISTORIQUE hist = new T_HISTORIQUE();
            hist.DATE_ACTION = DateTime.Now;
            hist.ACTION = "Ajout";
            hist.T_TABLE = "TR_ACTPROF_BCT";
            hist.ID_ENREGISTREMENT = db.TR_ACTPROF_BCT.Max(p => p.Code_SousClasse).ToString();
            hist.LOGIN_USER = Session["ID_USER"].ToString();
            hist.NOM_PC = Dns.GetHostName();
            hist.IP_PC = Dns.GetHostByName(hist.NOM_PC).AddressList[0].ToString();
            db.T_HISTORIQUE.Add(hist);
            db.SaveChanges();
            TempData["listval"] = "save";
            TempData["Prof"] = "active";
            return RedirectToAction("ListVal");
        }

        public ActionResult PartieUpdateProf(string id)
        {
            if (Session["UserLogin"] != null)
            {
                TR_ACTPROF_BCT val = new TR_ACTPROF_BCT();
                val = db.TR_ACTPROF_BCT.Find(id);
                return PartialView(val);
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
        [HttpPost]
        public ActionResult PartieUpdateProf(TR_ACTPROF_BCT val)
        {
            db.Entry(val).State = EntityState.Modified;
            db.SaveChanges();
            TempData["listval"] = "save";
            TempData["Prof"] = "active";
            return RedirectToAction("ListVal");
        }




        public ActionResult PartieListTva()
        {
            if (Session["UserLogin"] != null)
            {
                return PartialView();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
        [HttpPost]
        public ActionResult PartieListTva(TR_TVA cp)
        {
            db.TR_TVA.Add(cp);
            db.SaveChanges();
            T_HISTORIQUE hist = new T_HISTORIQUE();
            hist.DATE_ACTION = DateTime.Now;
            hist.ACTION = "Ajout";
            hist.T_TABLE = "TR_TVA";
            hist.ID_ENREGISTREMENT = db.TR_TVA.Max(p => p.ID_TVA).ToString();
            hist.LOGIN_USER = Session["ID_USER"].ToString();
            hist.NOM_PC = Dns.GetHostName();
            hist.IP_PC = Dns.GetHostByName(hist.NOM_PC).AddressList[0].ToString();
            db.T_HISTORIQUE.Add(hist);
            db.SaveChanges();
            TempData["listval"] = "save";
            return RedirectToAction("ListVal");
        }

        public ActionResult PartieUpdateTva(int id)
        {
            if (Session["UserLogin"] != null)
            {
                TR_TVA val = new TR_TVA();
                val = db.TR_TVA.Find(id);
                return PartialView(val);
            }
            else
            {
                return RedirectToAction("login", "Login");
            }

        }
        [HttpPost]
        public ActionResult PartieUpdateTva(TR_TVA val)
        {
            db.Entry(val).State = EntityState.Modified;
            db.SaveChanges();
            TempData["listval"] = "save";
            return RedirectToAction("ListVal");
        }



        public ActionResult PartieListBq()
        {
            if (Session["UserLogin"] != null)
            {
                return PartialView();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
        [HttpPost]
        public ActionResult PartieListBq(TR_Ag_Bq cp)
        {
            try {
                cp.Code_Bq_Ag = cp.Code_Bq + cp.Code_Ag;
            db.TR_Ag_Bq.Add(cp);
            db.SaveChanges();
            T_HISTORIQUE hist = new T_HISTORIQUE();
            hist.DATE_ACTION = DateTime.Now;
            hist.ACTION = "Ajout";
            hist.T_TABLE = "TR_Ag_Bq";
            hist.ID_ENREGISTREMENT = db.TR_Ag_Bq.Max(p => p.Code_Bq_Ag).ToString();
            hist.LOGIN_USER = Session["ID_USER"].ToString();
            hist.NOM_PC = Dns.GetHostName();
            hist.IP_PC = Dns.GetHostByName(hist.NOM_PC).AddressList[0].ToString();
            db.T_HISTORIQUE.Add(hist);
            db.SaveChanges();
            TempData["listval"] = "save";
                TempData["BQ"] = "active";
            }
            catch (Exception) { TempData["error"] = "Erreur"; }
            return RedirectToAction("ListVal");
        }

        public ActionResult PartieUpdateBq(string id)
        {
            if (Session["UserLogin"] != null)
            {
                TR_Ag_Bq val = new TR_Ag_Bq();
                val = db.TR_Ag_Bq.Find(id);
                return PartialView(val);
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
        [HttpPost]
        public ActionResult PartieUpdateBq(TR_Ag_Bq val)
        {
            try
            {
                db.Entry(val).State = EntityState.Modified;
                db.SaveChanges();
                TempData["listval"] = "save";
                TempData["BQ"] = "active";
            }
            catch (Exception) { TempData["error"] = "Erreur"; }
            return RedirectToAction("ListVal");
           
        }



        public ActionResult PartieListFraisD()
        {
            if (Session["UserLogin"] != null)
            {
                return PartialView();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
        [HttpPost]
        public ActionResult PartieListFraisD(TR_LISTE_FRAIS_DIVERS cp)
        {
            try
            {
                db.TR_LISTE_FRAIS_DIVERS.Add(cp);
                db.SaveChanges();
                T_HISTORIQUE hist = new T_HISTORIQUE();
                hist.DATE_ACTION = DateTime.Now;
                hist.ACTION = "Ajout";
                hist.T_TABLE = "TR_LISTE_FRAIS_DIVERS";
                hist.ID_ENREGISTREMENT = db.TR_LISTE_FRAIS_DIVERS.Max(p => p.ID_ListeFraisDivers).ToString();
                hist.LOGIN_USER = Session["ID_USER"].ToString();
                hist.NOM_PC = Dns.GetHostName();
                hist.IP_PC = Dns.GetHostByName(hist.NOM_PC).AddressList[0].ToString();
                db.T_HISTORIQUE.Add(hist);
                db.SaveChanges();
                TempData["listval"] = "save";
            }
            catch (Exception) { TempData["error"] = "Erreur"; }
            return RedirectToAction("ListVal");
        }

        public ActionResult PartieUpdateFraisD(int id)
        {
            if (Session["UserLogin"] != null)
            {
                TR_LISTE_FRAIS_DIVERS val = new TR_LISTE_FRAIS_DIVERS();
                val = db.TR_LISTE_FRAIS_DIVERS.Find(id);
                return PartialView(val);
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
        [HttpPost]
        public ActionResult PartieUpdateFraisD(TR_LISTE_FRAIS_DIVERS val)
        {
            try
            {
                db.Entry(val).State = EntityState.Modified;
                db.SaveChanges();
                TempData["listval"] = "save";
            }
            catch (Exception) { TempData["error"] = "Erreur"; }
            return RedirectToAction("ListVal");
        }



        public ActionResult PartieListInteret()
        {
            if (Session["UserLogin"] != null)
            {
                return PartialView();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }

        }
        [HttpPost]
        public ActionResult PartieListInteret(TR_INT_FINANCEMENT cp)
        {
            try
            {
                db.TR_INT_FINANCEMENT.Add(cp);
                db.SaveChanges();
                T_HISTORIQUE hist = new T_HISTORIQUE();
                hist.DATE_ACTION = DateTime.Now;
                hist.ACTION = "Ajout";
                hist.T_TABLE = "TR_INT_FINANCEMENT";
                hist.ID_ENREGISTREMENT = db.TR_INT_FINANCEMENT.Max(p => p.ID_TR_INT_FIN).ToString();
                hist.LOGIN_USER = Session["ID_USER"].ToString();
                hist.NOM_PC = Dns.GetHostName();
                hist.IP_PC = Dns.GetHostByName(hist.NOM_PC).AddressList[0].ToString();
                db.T_HISTORIQUE.Add(hist);
                db.SaveChanges();
                TempData["listval"] = "save";
            }
            catch (Exception) { TempData["error"] = "Erreur"; }
            return RedirectToAction("ListVal");
        }

        public ActionResult PartieUpdateInteret(int id)
        {
            if (Session["UserLogin"] != null)
            {
                TR_INT_FINANCEMENT val = new TR_INT_FINANCEMENT();
                val = db.TR_INT_FINANCEMENT.Find(id);
                return PartialView(val);
            }
            else
            {
                return RedirectToAction("login", "Login");
            }

        }
        [HttpPost]
        public ActionResult PartieUpdateInteret(TR_INT_FINANCEMENT val)
        {
            try
            {
                db.Entry(val).State = EntityState.Modified;
                db.SaveChanges();
                TempData["listval"] = "save";
            }
            catch (Exception) { TempData["error"] = "Erreur"; }
            return RedirectToAction("ListVal");
        }




        public ActionResult PartieListComm()
        {
            if (Session["UserLogin"] != null)
            {
                return PartialView();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
        [HttpPost]
        public ActionResult PartieListComm(TR_COMM_FACTORING cp)
        {
            db.TR_COMM_FACTORING.Add(cp);
            db.SaveChanges();
            T_HISTORIQUE hist = new T_HISTORIQUE();
            hist.DATE_ACTION = DateTime.Now;
            hist.ACTION = "Ajout";
            hist.T_TABLE = "TR_COMM_FACTORING";
            hist.ID_ENREGISTREMENT = db.TR_COMM_FACTORING.Max(p => p.ID_COMM_FACT).ToString();
            hist.LOGIN_USER = Session["ID_USER"].ToString();
            hist.NOM_PC = Dns.GetHostName();
            hist.IP_PC = Dns.GetHostByName(hist.NOM_PC).AddressList[0].ToString();
            db.T_HISTORIQUE.Add(hist);
            db.SaveChanges();
            TempData["listval"] = "save";
            return RedirectToAction("ListVal");
        }

        public ActionResult PartieUpdateComm(int id)
        {
            if (Session["UserLogin"] != null)
            {
                TR_COMM_FACTORING val = new TR_COMM_FACTORING();
                val = db.TR_COMM_FACTORING.Find(id);
                return PartialView(val);
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
        [HttpPost]
        public ActionResult PartieUpdateComm(TR_INT_FINANCEMENT val)
        {
            db.Entry(val).State = EntityState.Modified;
            db.SaveChanges();
            TempData["listval"] = "save";
            return RedirectToAction("ListVal");
        }




        public ActionResult PartieListTMM()
        {
            if (Session["UserLogin"] != null)
            {
                return PartialView();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
        [HttpPost]
        public ActionResult PartieListTMM(TR_TMM cp,string TAUX_TMM)
        {
            try
            {
                cp.TAUX_TMM = Convert.ToDecimal(TAUX_TMM.Replace(".", ","));
                db.TR_TMM.Add(cp);
                db.SaveChanges();
                T_HISTORIQUE hist = new T_HISTORIQUE();
                hist.DATE_ACTION = DateTime.Now;
                hist.ACTION = "Ajout";
                hist.T_TABLE = "TR_TMM";
                hist.ID_ENREGISTREMENT = db.TR_TMM.Max(p => p.ID_TMM).ToString();
                hist.LOGIN_USER = Session["ID_USER"].ToString();
                hist.NOM_PC = Dns.GetHostName();
                hist.IP_PC = Dns.GetHostByName(hist.NOM_PC).AddressList[0].ToString();
                db.T_HISTORIQUE.Add(hist);
                db.SaveChanges();
                TempData["listval"] = "save";
                TempData["TMM"] = "active";
            }
            catch (Exception) { TempData["error"] = "Erreur"; }
            return RedirectToAction("ListVal");
        }

        public ActionResult PartieUpdateTMM(int id)
        {
            if (Session["UserLogin"] != null)
            {
                TR_TMM val = new TR_TMM();

                val = db.TR_TMM.Find(id);
            
                return PartialView(val);
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
        [HttpPost]
        public ActionResult PartieUpdateTMM(TR_TMM val,string TAUX_TMM)
        {
            try
            {
                val.TAUX_TMM = Convert.ToDecimal(TAUX_TMM.Replace(".", ","));
                db.Entry(val).State = EntityState.Modified;
                db.SaveChanges();
                TempData["listval"] = "save";
                TempData["TMM"] = "active";
            }
            catch (Exception) { TempData["error"] = "Erreur"; }
            return RedirectToAction("ListVal");
        }


        public ActionResult PartieListLang()
        {
            if (Session["UserLogin"] != null)
            {
                return PartialView();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
        [HttpPost]
        public ActionResult PartieListLang(TR_NLDP cp)
        {
            try
            {
                db.TR_NLDP.Add(cp);
                db.SaveChanges();
                T_HISTORIQUE hist = new T_HISTORIQUE();
                hist.DATE_ACTION = DateTime.Now;
                hist.ACTION = "Ajout";
                hist.T_TABLE = "TR_NLDP";
                hist.ID_ENREGISTREMENT = db.TR_NLDP.Max(p => p.ID_NLDP).ToString();
                hist.LOGIN_USER = Session["ID_USER"].ToString();
                hist.NOM_PC = Dns.GetHostName();
                hist.IP_PC = Dns.GetHostByName(hist.NOM_PC).AddressList[0].ToString();
                db.T_HISTORIQUE.Add(hist);
                db.SaveChanges();
                TempData["listval"] = "save";
            }
            catch (Exception) { TempData["error"] = "Erreur"; }
            return RedirectToAction("ListVal");
        }

        public ActionResult PartieUpdateLang(int id)
        {
            if (Session["UserLogin"] != null)
            {
                TR_NLDP val = new TR_NLDP();
                val = db.TR_NLDP.Find(id);
                return PartialView(val);
            }
            else
            {
                return RedirectToAction("login", "Login");
            }

        }
        [HttpPost]
        public ActionResult PartieUpdateLang(TR_NLDP val)
        {
            db.Entry(val).State = EntityState.Modified;
            db.SaveChanges();
            TempData["listval"] = "save";
            return RedirectToAction("ListVal");
        }

        public ActionResult Agenda()
        {
            if (Session["UserLogin"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }

        public ActionResult ModifierFacture()
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
                TempData["Parametrage"] = "active";
                TempData["ModifierFacture"] = "active";
                return View();
               
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
        public JsonResult RecherchCtrAch(int id)
        {

            var ListNomInd = (from q in db.T_INDIVIDU
                              join q2 in db.TJ_CIR
                              on q.REF_IND equals q2.REF_IND_CIR
                              where (q2.ID_ROLE_CIR == "ACH" && q2.REF_CTR_CIR == id)
                              select new { q.PRE_IND, q.REF_IND });
            // ViewBag.ADH = new SelectList(ListNomInd, "REF_IND", "PRE_IND");
            return Json(ListNomInd, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecherchFact(int id,int id2)
        {
            ViewBag.test = db.ListeFactureValiderk(id, id2);
            return Json(ViewBag.test, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetValDetBord(string id)
        {
            TJ_DOCUMENT_DET_BORD tjdet = db.TJ_DOCUMENT_DET_BORD.Where(p => p.REF_DOCUMENT_DET_BORD == id).FirstOrDefault();
            T_DET_BORD det = db.T_DET_BORD.Where(p => p.ID_DET_BORD == tjdet.ID_DET_BORD).FirstOrDefault();
            var rest = new
            {
                TYP_DET_BORD=det.TYP_DET_BORD,
                DAT_DET_BORD=det.DAT_DET_BORD,
                MONT_TTC_DET_BORD=det.MONT_TTC_DET_BORD,
                DEVISE_DET_BORD=det.DEVISE_DET_BORD,
                ECH_DET_BORD=det.ECH_DET_BORD,
                MONT_OUV_DET_BORD=det.MONT_OUV_DET_BORD,
                RETENU_DET_BORD =det.RETENU_DET_BORD,
                MODE_REG_DET_BORD=det.MODE_REG_DET_BORD,
                ID_DET_BORD=det.ID_DET_BORD
            };
            return Json(rest, JsonRequestBehavior.AllowGet);

        }

        public ActionResult updateDet(T_DET_BORD bord,TJ_DOCUMENT_DET_BORD doc,string RefAch)
        {
            T_DET_BORD bor = db.T_DET_BORD.Where(p => p.ID_DET_BORD == bord.ID_DET_BORD).FirstOrDefault();
            if (bord.DAT_DET_BORD.ToString() !="")
            {
                bor.DAT_DET_BORD = bord.DAT_DET_BORD;
            }
            if (RefAch != "")
            {
                bor.REF_IND_DET_BORD = int.Parse(RefAch);
            }
            db.Entry(bor).State = EntityState.Modified;
            TJ_DOCUMENT_DET_BORD d = db.TJ_DOCUMENT_DET_BORD.Where(p => p.ID_DET_BORD == doc.ID_DET_BORD).FirstOrDefault();
            if (doc.REF_DOCUMENT_DET_BORD.ToString() != "")
            {
                d.REF_DOCUMENT_DET_BORD = doc.REF_DOCUMENT_DET_BORD;
            }
            db.Entry(d).State = EntityState.Modified;
            db.SaveChanges();

            T_HISTORIQUE hist = new T_HISTORIQUE();
            hist.DATE_ACTION = DateTime.Now;
            hist.ACTION = "Modification";
            hist.T_TABLE = "T_DET_BORD";
            hist.ID_ENREGISTREMENT = bor.ID_DET_BORD.ToString();
            hist.LOGIN_USER = Session["ID_USER"].ToString();
            hist.NOM_PC = Dns.GetHostName();
            hist.IP_PC = Dns.GetHostByName(hist.NOM_PC).AddressList[0].ToString();
            hist.REF_CTR_HIST = bor.REF_CTR_DET_BORD.ToString();
            hist.REF_IND_HIST = bor.REF_IND_DET_BORD.ToString();
            db.T_HISTORIQUE.Add(hist);


            T_HISTORIQUE hist2 = new T_HISTORIQUE();
            hist2.DATE_ACTION = DateTime.Now;
            hist2.ACTION = "Modification";
            hist2.T_TABLE = "TJ_DOCUMENT_DET_BORD";
            hist2.ID_ENREGISTREMENT = d.ID_DOCUMENT_DET_BORD.ToString();
            hist2.LOGIN_USER = Session["ID_USER"].ToString();
            hist2.NOM_PC = Dns.GetHostName();
            hist2.IP_PC = Dns.GetHostByName(hist.NOM_PC).AddressList[0].ToString();
            hist2.REF_CTR_HIST = d.REF_CTR_DET_BORD.ToString();
            db.T_HISTORIQUE.Add(hist2);


            db.SaveChanges();
            TempData["notice"] = " la modifcation de facture n° "+d.REF_DOCUMENT_DET_BORD +" de contrat n° "+d.REF_CTR_DET_BORD +" est bien enregistrer";
            return RedirectToAction("ModifierFacture");
        }


        public ActionResult AnnulerLettrage()
        {
            if (Session["UserLogin"] != null)
            {
                ViewBag.ENC = db.T_ENCAISSEMENT_MATERIALISER_Non_Rec();
                TempData["Parametrage"] = "active";
                TempData["AnnulerLettrage"] = "active";
                return View();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }

        public ActionResult PartieDeuxAnullerLetrrage(int id)
        {
            ViewBag.List = db.AnnulerLettrageParRefEnc(id);
            return PartialView();
        }


        public ActionResult DetailleLettrage(int id)
        {
            ViewBag.List = db.AnnulerLettrageParRefEnc(id);
            return PartialView();
        }

        public JsonResult AnnulerLetr(int id,int id2)
        {
            TJ_LETTRAGE let = db.TJ_LETTRAGE.Where(p => p.ID_DET_BORD_LET == id && p.ID_ENC_LET == id2).SingleOrDefault();
                db.TJ_LETTRAGE.Remove(let);
            db.SaveChanges();

            T_DET_BORD bot = db.T_DET_BORD.Where(p => p.ID_DET_BORD == id.ToString()).FirstOrDefault();
            if (bot.RETENU_DET_BORD == null) { bot.RETENU_DET_BORD = 0; }
         
            db.Entry(bot).State = EntityState.Modified;

            int id_det = int.Parse(bot.ID_DET_BORD);

            Anuulationretenu(id_det);


            //T_HISTORIQUE hist = new T_HISTORIQUE();
            //hist.DATE_ACTION = DateTime.Now;
            //hist.ACTION = "suppression";
            //hist.T_TABLE = "TJ_LETTRAGE";
            //hist.ID_ENREGISTREMENT = let.ID_DET_BORD_LET.ToString()+"|"+ let.ID_ENC_LET.ToString();
            //hist.LOGIN_USER = Session["ID_USER"].ToString();
            //hist.NOM_PC = Dns.GetHostName();
            //hist.IP_PC = Dns.GetHostByName(hist.NOM_PC).AddressList[0].ToString();
            //hist.REF_CTR_HIST = let.ID_DET_BORD_LET.ToString();

            //db.T_HISTORIQUE.Add(hist);

            //T_HISTORIQUE hist2 = new T_HISTORIQUE();
            //hist2.DATE_ACTION = DateTime.Now;
            //hist2.ACTION = "Modification";
            //hist2.T_TABLE = "T_DET_BORD";
            //hist2.ID_ENREGISTREMENT = bot.ID_DET_BORD.ToString();
            //hist2.LOGIN_USER = Session["ID_USER"].ToString();
            //hist2.NOM_PC = Dns.GetHostName();
            //hist2.IP_PC = Dns.GetHostByName(hist.NOM_PC).AddressList[0].ToString();
            //hist2.REF_CTR_HIST = bot.REF_CTR_DET_BORD.ToString();
            //db.T_HISTORIQUE.Add(hist2);
            return Json(db.SaveChanges(),JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaisiRIB_Factor()
        {
          
            return View();
        }
        [HttpPost]
        public ActionResult SaisiRIB_Factor(T_RIB_FACTOR rib, string RIB_RIB1, string RIB_RIB2, string RIB_RIB3)
        {
            try
            {
                rib.VALID_RIB_FACTOR = true;
                rib.RIB_FACTOR = RIB_RIB1 + RIB_RIB2 + RIB_RIB3;
                db.T_RIB_FACTOR.Add(rib);
                db.SaveChanges();
                TempData["notice"] = "sauvegarde enregistrée avec succès";
            }
            catch (Exception) { return RedirectToAction("InternalServerError", "Error"); }
            return RedirectToAction("SaisiRIB_Factor");
        }

        public ActionResult Email()
        {
            List<ListeDesGroupesMail> listeDesGroupesMail = new List<ListeDesGroupesMail>();
            List<TR_LIST_VAL> li = db.TR_LIST_VAL.Where(p => p.TYP_LIST_VAL == "EMAIL").ToList();
            foreach (TR_LIST_VAL val in li)
            {
                ListeDesGroupesMail l = new ListeDesGroupesMail();
                l.Titre = val.LIB_LIST_VAL;
                l.Nomber_Users = db.T_EMAIL.Where(p => p.TITRE_GROUPE.Contains(val.LIB_LIST_VAL)).Count();
                listeDesGroupesMail.Add(l);
            }

            ViewBag.listeGroupesMail = listeDesGroupesMail;
            TempData["Parametrage"] = "active";
            TempData["Email"] = "active";
            return View();
        }


        public ActionResult ListeDesUtilisateursParMailGroupe(string id)
        {
             string sqlReq1 = "select * from TS_USER where ID_USER not in (select ID_USER from T_email where Titre_groupe='"+ id + "') and ACTIF_USER=1 and (MAIL_USER is not NULL and Len(MAIL_USER)>0)";
            string sqlReq = "select TS_USER.*,ID_Email from TS_USER,T_email where  TS_USER.ID_USER=T_email.ID_USER and Titre_groupe='" + id.Trim()+"' ";
          
            List<UnionUserEmail> users = db.Database.SqlQuery<UnionUserEmail>(sqlReq).ToList();
            List<TS_USER> usersListe = db.Database.SqlQuery<TS_USER>(sqlReq1).ToList();
            ViewBag.users = users;
            ViewBag.usersListe = usersListe;

            return PartialView();
        }

        public ActionResult ADDListeDesUtilisateursParMailGroupe(string id)
        {
            string sqlReq1 = "select * from TS_USER where ID_USER not in (select ID_USER from T_email where Titre_groupe='" + id + "') and ACTIF_USER=1 and (MAIL_USER is not NULL and Len(MAIL_USER)>0)";
          //  string sqlReq = "select TS_USER.*,ID_Email from TS_USER,T_email where TS_USER.ID_USER  in (select ID_USER from T_email where Titre_groupe='" + id.Trim() + "') ";

          //  List<UnionUserEmail> users = db.Database.SqlQuery<UnionUserEmail>(sqlReq).ToList();
            List<TS_USER> usersListe = db.Database.SqlQuery<TS_USER>(sqlReq1).ToList();
           // ViewBag.users = users;
            ViewBag.usersListe = usersListe;

            return PartialView();
        }


        public ActionResult AddUserToGroupeMail(int id,string groupeusers)
        {
            TS_USER user = db.TS_USER.Find(id);
            try {
                T_EMAIL email = new T_EMAIL();
                email.ID_USER = user.ID_USER;
                email.TITRE_GROUPE = groupeusers;
                db.T_EMAIL.Add(email);
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            } catch (Exception e) { return Json(false, JsonRequestBehavior.AllowGet); }
         
        }

        public ActionResult RemoveUserFromGroupeMail(int id)
        {
            
            try
            {
                T_EMAIL em = db.T_EMAIL.Where(p=>p.ID_EMAIL==id).FirstOrDefault();
                db.T_EMAIL.Remove(em);
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)

            { return Json(false, JsonRequestBehavior.AllowGet); }

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

        public bool Anuulationretenu(int id_det_bord)
        {
            decimal x = 0;

           T_DET_BORD facture = db.T_DET_BORD.Where(p=>p.ID_DET_BORD==id_det_bord.ToString()).SingleOrDefault();
            x = (decimal)facture.RETENU_DET_BORD;
            if (facture.RETENU_DET_BORD != null && facture.RETENU_DET_BORD > 0)
            {
                List<TJ_LETTRAGE> ListeDesFactures = db.TJ_LETTRAGE.Where(p => p.ID_DET_BORD_LET == id_det_bord).ToList();
                if (ListeDesFactures.Count ==0)
                {
                    //Update facture
                    facture.RETENU_DET_BORD = 0;
                    facture.MONT_OUV_DET_BORD = facture.MONT_TTC_DET_BORD;
                    facture.MONT_FDG_DET_BORD = (facture.MONT_TTC_DET_BORD * facture.TX_FDG_DET_BORD) / 100;
                    facture.MONT_FDG_LIBERE_DET_BORD = 0;
                    db.SaveChanges();

                    //Add Historique
                    try
                    {
                        T_HISTORIQUE histo = new T_HISTORIQUE();
                        histo.DATE_ACTION = DateTime.Now;
                        histo.ACTION = "Modifictaion";
                        histo.T_TABLE = "T_DET_BORD";
                        histo.ID_ENREGISTREMENT = id_det_bord.ToString();
                        histo.LOGIN_USER = Session["UserLogin"].ToString();
                        histo.IP_PC = GetIp();
                        histo.NOM_PC = HttpContext.Request.UserHostName;
                        histo.REF_CTR_HIST = facture.REF_CTR_DET_BORD.ToString();
                        histo.REF_IND_HIST = db.TJ_CIR.Where(p => p.REF_CTR_CIR == facture.REF_CTR_DET_BORD && p.ID_ROLE_CIR == "ADH").SingleOrDefault().REF_IND_CIR.ToString();
                        histo.ABREV_ROLE_HIST = "Annulation RS facture " + db.TJ_DOCUMENT_DET_BORD.Where(p => p.ID_DET_BORD == facture.ID_DET_BORD).SingleOrDefault().REF_DOCUMENT_DET_BORD + " - montant : " + x;
                        db.T_HISTORIQUE.Add(histo);
                        db.SaveChanges();
                    }
                    catch (Exception)
                    {
                    }


                    //Add Extrait
                    try
                    {
                        T_EXTRAIT extrait = new T_EXTRAIT();
                        extrait.REF_CTR_EXTRAIT = facture.REF_CTR_DET_BORD;
                        extrait.LIB_EXTRAIT = "Annulation RS " + db.TJ_DOCUMENT_DET_BORD.Where(p => p.ID_DET_BORD == facture.ID_DET_BORD).SingleOrDefault().REF_DOCUMENT_DET_BORD + " - montant : " + x;
                        extrait.DEBIT_EXTRAIT = 0;
                        extrait.CREDIT_EXTRAIT = x;
                        extrait.AUTRE_EXTRAIT = 0;
                        extrait.ENCOURFACT_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Encours_Facture).FirstOrDefault();
                        extrait.DISPONIBLE_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Disponible).FirstOrDefault();
                        extrait.DAT_OP_EXTRAIT = DateTime.Now;
                        extrait.DAT_VAL_EXTRAIT = DateTime.Now;
                        extrait.TOTAL_FIN_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Total_Financement).FirstOrDefault();
                        db.T_EXTRAIT.Add(extrait);
                        db.SaveChanges();

                    }
                    catch (Exception) { }


                    //Add Credit

                    try
                    {
                        T_MVT_CREDIT credit = new T_MVT_CREDIT();
                        credit.MONT_CREDIT = 0;
                        credit.REF_CTR_CREDIT = facture.REF_CTR_DET_BORD;
                        credit.LIBELLE_LIBRE_CREDIT = "Annulation RS " + db.TJ_DOCUMENT_DET_BORD.Where(p => p.ID_DET_BORD == facture.ID_DET_BORD).SingleOrDefault().REF_DOCUMENT_DET_BORD + " - montant : " + x;
                        credit.ABRV_CREDIT = "6";
                        credit.DATE_CREDIT = DateTime.Now;
                        credit.DAT_VAL_ENC_CREDIT = DateTime.Now;
                        db.T_MVT_CREDIT.Add(credit);
                        db.SaveChanges();

                    }
                    catch (Exception) { }
                    //Send Mail

                    try
                    {
                        SendEmail email = new SendEmail("Notifiaction_Bordereau", "Notifiaction Annulation RS ", "Annulation RS " + db.TJ_DOCUMENT_DET_BORD.Where(p => p.ID_DET_BORD == facture.ID_DET_BORD).SingleOrDefault().REF_DOCUMENT_DET_BORD + " - montant : " + x);

                        email.SendEmailXpert();
                    }
                    catch (Exception) { }



                    return true;
                }
                else
                {

                    return false;
                }

            }

            else
            {
                return false;
            }



            // return true;
        }

    }
}