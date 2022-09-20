using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using xfactor.Models;
using PagedList;
using PagedList.Mvc;
using Newtonsoft.Json;

namespace xfactor.Controllers
{
    public class IndividuController : Controller
    {
        private XFactor_PRODEntities1 db = new XFactor_PRODEntities1();

        // GET: Individu
        public ActionResult Index()
        {
            if (Session["UserLogin"] != null)
            {
                try
                {
                    ViewBag.listind = db.T_INDIVIDU.ToList();
                    TempData["Individu"] = "active";
                    TempData["Index"] = "active";

                    return View();
                }
                catch (Exception e)
                {
                  /*  ViewBag.listind = null;
                    TempData["Individu"] = "active";
                    TempData["Index"] = "active";
                    */
                    return RedirectToAction("InternalServerError", "Error");
                }

              
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }


        public ActionResult veriferIndividu(string id)
        {
            T_INDIVIDU ind;

try
            {
                ind = db.T_INDIVIDU.Where(p => p.NUM_DOC_ID_IND == id).SingleOrDefault();
            }
            catch (Exception) { ind = null; }

            if (ind == null) { return Json(true, JsonRequestBehavior.AllowGet); }
            else { return Json(false, JsonRequestBehavior.AllowGet); }
        }

        public ActionResult ModifierRib(string id)
        {
            TR_RIB rib;

            try
            {  rib = db.TR_RIB.Where(p=>p.RIB_RIB==id).SingleOrDefault(); }
            catch (Exception) { rib = null; }
            ViewBag.Rib = rib;
            List<ListeRib> listerib = new List<ListeRib>();
            foreach (var item in db.TR_RIB.Where(p => p.RIB_RIB == id))
            {
                string ban = db.TR_Ag_Bq.Where(p => p.Code_Bq_Ag == item.RIB_RIB.ToString().Substring(0, 5)).Select(p => p.Banque).FirstOrDefault();
                string agen = db.TR_Ag_Bq.Where(p => p.Code_Bq_Ag == item.RIB_RIB.ToString().Substring(0, 5)).Select(p => p.Agence).FirstOrDefault();
                listerib.Add(new ListeRib(item.RIB_RIB, ban, agen, item.ACTIF_RIB));
            }
            ViewBag.listerib = listerib.ToList();
            return PartialView();
        }


        // GET: Individu/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UserLogin"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                T_INDIVIDU t_INDIVIDU = db.T_INDIVIDU.Find(id);
                try { TempData["FormJuridique"] =  db.TR_LIST_VAL.Where(p => p.TYP_LIST_VAL == "Forme juridique" && p.ABR_LIST_VAL == t_INDIVIDU.FROM_JUR_IND).Select(p => p.LIB_LIST_VAL).First(); } catch (Exception) { TempData["FormJuridique"] = ""; }

                try { TempData["codepostale"] = db.TR_CP.Where(p => p.CP == t_INDIVIDU.CP_IND).Select(p => p.Ville).First(); } catch (Exception) { TempData["codepostale"] = ""; }

                try { TempData["langue"] = db.TR_NLDP.Where(p => p.ID_NLDP == t_INDIVIDU.ID_NLDP).Select(p => p.LIB_LANG).First(); } catch (Exception) { TempData["langue"] = ""; }

                if (t_INDIVIDU == null)
                {
                    return HttpNotFound();
                }
                // ViewBag.individu = listdesind;
                ViewBag.ListContact = db.T_CONTACT.Where(p => (int)p.REF_IND_CONTACT == id).ToList();
                ViewBag.logweb = db.TS_USERS_WEB.Where(p => p.REF_IND_WEB == id).ToList();
                List<ListeRib> listerib = new List<ListeRib>();
                foreach (var item in db.TR_RIB.Where(p => p.REF_IND_RIB == id))
                {
                    string ban = db.TR_Ag_Bq.Where(p => p.Code_Bq_Ag == item.RIB_RIB.ToString().Substring(0, 5)).Select(p => p.Banque).FirstOrDefault();
                    string agen = db.TR_Ag_Bq.Where(p => p.Code_Bq_Ag == item.RIB_RIB.ToString().Substring(0, 5)).Select(p => p.Agence).FirstOrDefault();
                    listerib.Add(new ListeRib(item.RIB_RIB, ban, agen, item.ACTIF_RIB));
                }
                ViewBag.listerib = listerib.ToList() ;
                return View(t_INDIVIDU);
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
        public ActionResult TVAAlgo(string id)
        {
            if (id.Length < 8)
            {
                String newTVA = "";
                if (id == null) { id = ""; }
                try
                {
                    int longChaineTva = id.Length;
                    if (longChaineTva < 7)
                    {
                        string ajoutzero = "";
                        for (int i = longChaineTva; i < 7; i++)
                        {
                            ajoutzero = ajoutzero + "0";


                        }
                        id = ajoutzero + id;
                    }
                    string C1 = id.Substring(0, 1);
                    string C2 = id.Substring(1, 1);
                    string C3 = id.Substring(2, 1);
                    string C4 = id.Substring(3, 1);
                    string C5 = id.Substring(4, 1);
                    string C6 = id.Substring(5, 1);
                    string C7 = id.Substring(6, 1);

                    Decimal DC1 = decimal.Parse(C1.Replace(".", ","));
                    Decimal DC2 = decimal.Parse(C2.Replace(".", ","));
                    Decimal DC3 = decimal.Parse(C3.Replace(".", ","));
                    Decimal DC4 = decimal.Parse(C4.Replace('.', ','));
                    Decimal DC5 = decimal.Parse(C5.Replace('.', ','));
                    Decimal DC6 = decimal.Parse(C6.Replace('.', ','));
                    Decimal DC7 = decimal.Parse(C7.Replace('.', ','));


                    Decimal Calcul = ((DC1 * 7) + (DC2 * 6) + (DC3 * 5) + (DC4 * 4) + (DC5 * 3) + (DC6 * 2) + (DC7 * 1));


                    Decimal ArrondiCalcul = ((Calcul / 23));

                    Decimal TArrondiCalcul = Math.Round(Math.Truncate(ArrondiCalcul));

                    Decimal CalTArrondiCalcul = (TArrondiCalcul * 23);

                    Decimal ResulMF = (Calcul - CalTArrondiCalcul + 1);

                    String SResulMF = ResulMF.ToString();

                    //    MessageBox.Show(SResulMF);



                    if (SResulMF == "1") { String CleMF; CleMF = "A"; newTVA = CleMF; }
                    if (SResulMF == "2") { String CleMF; CleMF = "B"; newTVA = CleMF; }
                    if (SResulMF == "3") { String CleMF; CleMF = "C"; newTVA = CleMF; }
                    if (SResulMF == "4") { String CleMF; CleMF = "D"; newTVA = CleMF; }
                    if (SResulMF == "5") { String CleMF; CleMF = "E"; newTVA = CleMF; }
                    if (SResulMF == "6") { String CleMF; CleMF = "F"; newTVA = CleMF; }
                    if (SResulMF == "7") { String CleMF; CleMF = "G"; newTVA = CleMF; }
                    if (SResulMF == "8") { String CleMF; CleMF = "H"; newTVA = CleMF; }
                    if (SResulMF == "9") { String CleMF; CleMF = "J"; newTVA = CleMF; }
                    if (SResulMF == "10") { String CleMF; CleMF = "K"; newTVA = CleMF; }
                    if (SResulMF == "11") { String CleMF; CleMF = "L"; newTVA = CleMF; }
                    if (SResulMF == "12") { String CleMF; CleMF = "M"; newTVA = CleMF; }
                    if (SResulMF == "13") { String CleMF; CleMF = "N"; newTVA = CleMF; }
                    if (SResulMF == "14") { String CleMF; CleMF = "P"; newTVA = CleMF; }
                    if (SResulMF == "15") { String CleMF; CleMF = "Q"; newTVA = CleMF; }
                    if (SResulMF == "16") { String CleMF; CleMF = "R"; newTVA = CleMF; }
                    if (SResulMF == "17") { String CleMF; CleMF = "S"; newTVA = CleMF; }
                    if (SResulMF == "18") { String CleMF; CleMF = "T"; newTVA = CleMF; }
                    if (SResulMF == "19") { String CleMF; CleMF = "V"; newTVA = CleMF; }
                    if (SResulMF == "20") { String CleMF; CleMF = "W"; newTVA = CleMF; }
                    if (SResulMF == "21") { String CleMF; CleMF = "X"; newTVA = CleMF; }
                    if (SResulMF == "22") { String CleMF; CleMF = "Y"; newTVA = CleMF; }
                    if (SResulMF == "23") { String CleMF; CleMF = "Z"; newTVA = CleMF; }
                }
                catch (Exception) { newTVA = ""; }
                return Json(id + newTVA, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(id, JsonRequestBehavior.AllowGet);

            }
           

        }
        public ActionResult ListeGroupe()
        {
            return Json(db.T_GROUPE.ToList(),JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjouterGroupe(string id)
        {
            if (id != null && id != "")
            {
                T_GROUPE gp = new T_GROUPE();
                gp.NOM_GROUP = id;
                db.T_GROUPE.Add(gp);
                db.SaveChanges();


                try
                {
                    T_HISTORIQUE histo = new T_HISTORIQUE();
                    histo.ABREV_ROLE_HIST = "Creation Groupe" + gp.NOM_GROUP;
                    histo.ACTION = "Creation Groupe";
                    histo.ID_ENREGISTREMENT = gp.ID_GROUP.ToString();
                    histo.T_TABLE = "T_GROUPE";
                    histo.REF_CTR_HIST = gp.ID_GROUP.ToString();
                    histo.LOGIN_USER = Session["UserLogin"].ToString();
                    histo.IP_PC = HttpContext.Request.UserHostAddress;
                    histo.NOM_PC = HttpContext.Request.UserHostName;
                    histo.DATE_ACTION = DateTime.Now;
                    db.T_HISTORIQUE.Add(histo);
                    db.SaveChanges();
                }
                catch (Exception) { }
                return Json(true, JsonRequestBehavior.AllowGet);



            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        // GET: Individu/Create
        public ActionResult Create()
        {
            if (Session["UserLogin"] != null)
            {
                try
                {
                    ViewBag.ListPiece = new SelectList(db.TR_LIST_VAL.Where(q => q.TYP_LIST_VAL == "Piece_Identite"), "ABR_LIST_VAL", "LIB_LIST_VAL");
                    ViewBag.LISTJURIDIQ = new SelectList(db.TR_LIST_VAL.Where(q => q.TYP_LIST_VAL == "Forme juridique"), "ABR_LIST_VAL", "LIB_LIST_VAL");
                    ViewBag.ListValProf = new SelectList(db.TR_ACTPROF_BCT, "Code_SousClasse", "SousClasse");
                    ViewBag.LISTPOS = new SelectList(db.TR_LIST_VAL.Where(q => q.TYP_LIST_VAL == "POSIND"), "LIB_LIST_VAL", "LIB_LIST_VAL");
                    ViewBag.ListValLang = db.TR_NLDP.Select(p => new SelectListItem
                    {
                        Text = p.ABRV_LANG,
                        Value = p.ID_NLDP.ToString()
                    });
                    ViewBag.ListeGroup = db.T_GROUPE.Select(p => new SelectListItem
                    {
                        Text = p.NOM_GROUP,
                        Value = p.ID_GROUP.ToString()
                    });
                    ViewBag.ListValcp = db.TR_CP.Select(p => new SelectListItem { Text = p.Ville + " | " + p.CP, Value = p.CP });
                    ViewBag.ListIndividu = db.T_INDIVIDU.ToList();
                    TempData["Individu"] = "active";
                    TempData["Create"] = "active";
                    return View();
                }
                catch (Exception) { return RedirectToAction("InternalServerError", "Error"); }
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
        public ActionResult veriferIndividuMF(string id)
        {
            T_INDIVIDU ind;

            try
            {
                ind = db.T_INDIVIDU.Where(p => p.MF_IND == id).SingleOrDefault();
            }
            catch (Exception) { ind = null; }

            if (ind == null) { return Json(true, JsonRequestBehavior.AllowGet); }
            else { return Json(false, JsonRequestBehavior.AllowGet); }
        }

        // POST: Individu/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(T_INDIVIDU individu, string NUM_DOC_ID_IND, List<T_CONTACT> cont, IndividuContactModel individucont, string test,String gnd,TS_USERS_WEB web,List<LisetDesRibs>ribs)
        {
            if (gnd == "PM")
            {
                if (ModelState.IsValid)
                {
                    individu.GENRE_IND = "PM";
                    individu.TYP_DOC_ID_IND = "Registre de commerce";
                    individu.RC_IND = NUM_DOC_ID_IND;
                    individu.INFO_IND = individu.REF_ADH_IND;
                    db.T_INDIVIDU.Add(individu);
                }

                db.SaveChanges();
                TempData["notice"] = "sauvegarde enregistrée avec succès";
            }
            else if (gnd == "PP")
            {
                if (ModelState.IsValid)
                {
                    individu.GENRE_IND = "PP";
                    individu.INFO_IND = individu.REF_ADH_IND;
                    db.T_INDIVIDU.Add(individu);
                    if (test == "true")
                    db.SaveChanges();
                    TempData["notice"] = "sauvegarde enregistrée avec succès";
                }
            }
            else {
                if (ModelState.IsValid)
                {
                    individu.GENRE_IND = "ASS";
                    individu.TYP_DOC_ID_IND = "Registre de commerce";
                    db.T_INDIVIDU.Add(individu);
                    db.SaveChanges();
                    TempData["notice"] = "sauvegarde enregistrée avec succès";

                }
            }
            if (cont!=null)
            {
                foreach (T_CONTACT con in cont)
                {
                    try
                    {
                        con.REF_IND_CONTACT = db.T_INDIVIDU.Where(p => p.NUM_DOC_ID_IND == individu.NUM_DOC_ID_IND).Select(p => p.REF_IND).FirstOrDefault();
                        db.T_CONTACT.Add(con);
                    }
                    catch (Exception)
                    { }
                }
            }
            if (web.LOGIN_WEB != null && web.PASSWORD_WEB !=null)
            {
                try
                {
                    web.REF_IND_WEB = db.T_INDIVIDU.Where(p => p.NUM_DOC_ID_IND == individu.NUM_DOC_ID_IND).Select(p => p.REF_IND).FirstOrDefault();
                    web.ACTIF_USER_WEB = true;
                    db.TS_USERS_WEB.Add(web);
                }
                catch (Exception) { }
            }
            if (ribs!= null){

              
                foreach (LisetDesRibs rib in ribs)
                {
                    TR_RIB ribb = new TR_RIB();
                    if (rib.RIB_RIB1 != "" && rib.RIB_RIB2 != "" && rib.RIB_RIB3 != "" && rib.RIB_RIB1 != null && rib.RIB_RIB2 != null && rib.RIB_RIB3 != null)
                    {
                        ribb.RIB_RIB = rib.RIB_RIB1 + rib.RIB_RIB2 + rib.RIB_RIB3;
                    }
                    else
                    {
                        ribb = null;
                    }
                    if (ribb != null)
                    {
                        try
                        {
                            ribb.REF_IND_RIB = db.T_INDIVIDU.Where(p => p.NUM_DOC_ID_IND == individu.NUM_DOC_ID_IND).Select(p => p.REF_IND).FirstOrDefault();
                            ribb.ACTIF_RIB = true;
                            db.TR_RIB.Add(ribb);
                        }
                        catch (Exception) { }
                    }
                }
                db.SaveChanges();
            }


            try
            {
                T_HISTORIQUE histo = new T_HISTORIQUE();
                histo.ABREV_ROLE_HIST = "Creation individu" + db.T_INDIVIDU.Where(p => p.NUM_DOC_ID_IND == individu.NUM_DOC_ID_IND).Select(p => p.NOM_IND).FirstOrDefault();
                histo.ACTION = "Creation Groupe";
                histo.ID_ENREGISTREMENT = db.T_INDIVIDU.Where(p => p.NUM_DOC_ID_IND == individu.NUM_DOC_ID_IND).Select(p => p.REF_IND).FirstOrDefault().ToString();
                histo.T_TABLE = "T_INDIVIDU";
                histo.REF_IND_HIST = db.T_INDIVIDU.Where(p => p.NUM_DOC_ID_IND == individu.NUM_DOC_ID_IND).Select(p => p.REF_IND).FirstOrDefault().ToString();
                histo.LOGIN_USER = Session["UserLogin"].ToString();
                histo.IP_PC = HttpContext.Request.UserHostAddress;
                histo.NOM_PC = HttpContext.Request.UserHostName;
                histo.DATE_ACTION = DateTime.Now;
                db.T_HISTORIQUE.Add(histo);
                db.SaveChanges();
            }
            catch (Exception) { }

            this.ViewBag.ListPiece = new SelectList(db.TR_LIST_VAL.Where(q => q.TYP_LIST_VAL == "Piece_Identite"), "ABR_LIST_VAL", "LIB_LIST_VAL");
            this.ViewBag.LISTJURIDIQ = new SelectList(db.TR_LIST_VAL.Where(q => q.TYP_LIST_VAL == "Forme juridique"), "ABR_LIST_VAL", "LIB_LIST_VAL");
            this.ViewBag.ListValProf = new SelectList(db.TR_ACTPROF_BCT, "Code_SousClasse", "SousClasse");
            this.ViewBag.LISTPOS = new SelectList(db.TR_LIST_VAL.Where(q => q.TYP_LIST_VAL == "POSIND"), "LIB_LIST_VAL", "LIB_LIST_VAL");
            this.ViewBag.ListValLang = db.TR_NLDP.Select(p => new SelectListItem
            {
                Text = p.LIB_NT + "(" + p.ABRV_LANG + ")",
                Value = p.ID_NLDP.ToString()
            });
            this.ViewBag.ListValcp = db.TR_CP.Select(p => new SelectListItem { Text = p.Ville + " | " + p.CP, Value = p.CP });
            ViewBag.ListIndividu = db.T_INDIVIDU.ToList();
            TempData["notice"] = "save";
            return RedirectToAction("Create");
        }


        public ActionResult Ajouter_Individu_Poste(T_INDIVIDU individu,string EXO_TVA,string GRP_IND, string NUM_DOC_ID_IND, List<T_CONTACT> cont, IndividuContactModel individucont, string test, String gnd, TS_USERS_WEB web, List<LisetDesRibs> ribs)
        {
            if (gnd == "PM")
            {

                if (GRP_IND == "0")
                {
                    individu.GRP_IND = false;
                }
                else
                {
                    individu.GRP_IND = true;
                }

                if (EXO_TVA == "0")
                {
                    individu.EXO_TVA = false;
                }
                else
                {
                    individu.EXO_TVA = true;
                }

                    individu.GENRE_IND = "PM";
                    individu.TYP_DOC_ID_IND = "Registre de commerce";
                    individu.INFO_IND = individu.REF_ADH_IND;
                    individu.RC_IND = NUM_DOC_ID_IND;
                    db.T_INDIVIDU.Add(individu);
                

                db.SaveChanges();
                TempData["notice"] = "sauvegarde enregistrée avec succès";
            }
            else if (gnd == "PP")
            {
                
                    individu.GENRE_IND = "PP";
                    db.T_INDIVIDU.Add(individu);
                    if (test == "true")
                        db.SaveChanges();
                    TempData["notice"] = "sauvegarde enregistrée avec succès";
                
            }
            else
            {
               
                    individu.GENRE_IND = "ASS";
                    individu.TYP_DOC_ID_IND = "Registre de commerce";
                    db.T_INDIVIDU.Add(individu);
                    db.SaveChanges();
                    TempData["notice"] = "sauvegarde enregistrée avec succès";

                
            }
            if (cont != null)
            {
                foreach (T_CONTACT con in cont)
                {
                    try
                    {
                        con.REF_IND_CONTACT = db.T_INDIVIDU.Where(p => p.NUM_DOC_ID_IND == individu.NUM_DOC_ID_IND).Select(p => p.REF_IND).FirstOrDefault();
                        db.T_CONTACT.Add(con);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {string c= e.Message; }
                }
            }
            if (web.LOGIN_WEB != null && web.PASSWORD_WEB != null)
            {
                try
                {
                    web.REF_IND_WEB = db.T_INDIVIDU.Where(p => p.NUM_DOC_ID_IND == individu.NUM_DOC_ID_IND).Select(p => p.REF_IND).FirstOrDefault();
                    web.ACTIF_USER_WEB = true;
                    db.TS_USERS_WEB.Add(web);
                    db.SaveChanges();
                    T_INDIVIDU ind;
                    try { ind = db.T_INDIVIDU.Where(p => p.NUM_DOC_ID_IND == individu.NUM_DOC_ID_IND).FirstOrDefault();
                        ind.ABRV_IND = web.LOGIN_WEB;
                        db.Entry(ind).State = EntityState.Modified;
                        db.SaveChanges();
                    } catch (Exception) { }
                    
                  
                
                }
                catch (Exception e)
                { string c = e.Message; }
            }
            if (ribs[0].RIB_RIB1 != null)
            {


                foreach (LisetDesRibs rib in ribs)
                {
                    TR_RIB ribb = new TR_RIB();
                    if (rib.RIB_RIB1 != "" && rib.RIB_RIB2 != "" && rib.RIB_RIB3 != "")
                    {
                        ribb.RIB_RIB = rib.RIB_RIB1 + rib.RIB_RIB2 + rib.RIB_RIB3;
                    }
                    else
                    {
                        ribb = null;
                    }
                    if (ribb != null)
                    {
                        try
                        {
                            ribb.REF_IND_RIB = db.T_INDIVIDU.Where(p => p.NUM_DOC_ID_IND == individu.NUM_DOC_ID_IND).Select(p => p.REF_IND).FirstOrDefault();
                            ribb.ACTIF_RIB = true;
                            db.TR_RIB.Add(ribb);
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        { string c = e.Message; }
                    }
                }
            
            }

            return Json("save", JsonRequestBehavior.AllowGet);
        }


        public ActionResult EditIndividuMoral(int? id)
        {
            
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                ViewBag.LISTJURIDIQ = new SelectList(db.TR_LIST_VAL.Where(q => q.TYP_LIST_VAL == "Forme juridique"), "ABR_LIST_VAL", "LIB_LIST_VAL");
                ViewBag.ListValProf = new SelectList(db.TR_ACTPROF_BCT, "Code_SousClasse", "SousClasse");
                ViewBag.LISTPOS = new SelectList(db.TR_LIST_VAL.Where(q => q.TYP_LIST_VAL == "POSIND"), "LIB_LIST_VAL", "LIB_LIST_VAL");
                ViewBag.ListValLang = db.TR_NLDP.Select(p => new SelectListItem
                {
                    Text = p.LIB_NT + "(" + p.ABRV_LANG + ")",
                    Value = p.ID_NLDP.ToString()
                });
                ViewBag.ListValcp = db.TR_CP.Select(p => new SelectListItem { Text = p.Ville + " | " + p.CP, Value = p.CP });
                ViewBag.TableDesContact = db.T_CONTACT.Where(p => p.REF_IND_CONTACT == id).ToList();
             ViewBag.ListeGroup = db.T_GROUPE.Select(p => new SelectListItem
                    {
                        Text = p.NOM_GROUP,
                        Value = p.ID_GROUP.ToString()
                    });
            List<ListeRib> listerib = new List<ListeRib>();
            foreach (var item in db.TR_RIB.Where(p => p.REF_IND_RIB == id))
            {
                string ban = db.TR_Ag_Bq.Where(p => p.Code_Bq_Ag == item.RIB_RIB.ToString().Substring(0, 5)).Select(p => p.Banque).FirstOrDefault();
                string agen = db.TR_Ag_Bq.Where(p => p.Code_Bq_Ag == item.RIB_RIB.ToString().Substring(0, 5)).Select(p => p.Agence).FirstOrDefault();
                listerib.Add(new ListeRib(item.RIB_RIB, ban, agen, item.ACTIF_RIB));
            }
            ViewBag.listerib = listerib.ToList();

            //ViewBag.ListWeb = db.TS_USERS_WEB.Where(p => p.REF_IND_WEB == id).ToList();
            TempData["login"] = db.TS_USERS_WEB.Where(p => p.REF_IND_WEB == id).Select(p => p.LOGIN_WEB).FirstOrDefault();
            TempData["mdp"] = db.TS_USERS_WEB.Where(p => p.REF_IND_WEB == id).Select(p => p.PASSWORD_WEB).FirstOrDefault();
            // string login = db.TS_USERS_WEB.Where(p => p.REF_IND_WEB == id).Select(p=>p.LOGIN_WEB).ToString();
            IndividuContactModel IndividuContactModel = new IndividuContactModel();
                IndividuContactModel.individu = db.T_INDIVIDU.Find(id);
                if (IndividuContactModel.individu == null)
                {
                    return HttpNotFound();
                }
                return View(IndividuContactModel);
          
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditIndividuMoral(IndividuContactModel IndividuContactModel ,DateTime DATE_DOC_ID_IND, TS_USERS_WEB web,bool GRP_IND,string ID_GROUPE, bool EXO_TVA,Nullable<DateTime> DAT_FIN_EXO, Nullable<DateTime> DAT_DEB_EXO)
        {
            
                T_INDIVIDU ind = db.T_INDIVIDU.Find(IndividuContactModel.individu.REF_IND);
                try { ind.GENRE_IND = IndividuContactModel.individu.GENRE_IND.TrimEnd(); } catch (Exception e) { }
            try { ind.TYP_DOC_ID_IND = IndividuContactModel.individu.TYP_DOC_ID_IND; } catch (Exception e) { }
            try { ind.RC_IND = IndividuContactModel.individu.RC_IND; } catch (Exception e) { }
            try { ind.FROM_JUR_IND = IndividuContactModel.individu.FROM_JUR_IND; } catch (Exception e) { }
            try { ind.NOM_IND = IndividuContactModel.individu.NOM_IND.TrimEnd(); } catch (Exception e) { }
            try { ind.PRE_IND = IndividuContactModel.individu.PRE_IND.TrimEnd(); } catch (Exception e) { }
            try { ind.NUM_DOC_ID_IND = IndividuContactModel.individu.NUM_DOC_ID_IND.TrimEnd(); } catch (Exception e) { }
            try { ind.COD_SCLAS = IndividuContactModel.individu.COD_SCLAS.TrimEnd(); } catch (Exception e) { }
            try { ind.COD_TVA_IND = IndividuContactModel.individu.COD_TVA_IND.TrimEnd(); } catch (Exception e) { }
            try { ind.CP_IND = IndividuContactModel.individu.CP_IND.TrimEnd(); } catch (Exception e) { }
          





            if (IndividuContactModel.individu.LIEU_DOC_ID_IND != null && IndividuContactModel.individu.LIEU_DOC_ID_IND != "")
            {
                ind.LIEU_DOC_ID_IND = IndividuContactModel.individu.LIEU_DOC_ID_IND.TrimEnd();
            }
               
                ind.GRP_IND = GRP_IND;
            if (ID_GROUPE != "" && ID_GROUPE!=null)
            {
                ind.ID_GROUPE = int.Parse(ID_GROUPE);
            }
                
                ind.DATE_DOC_ID_IND = DATE_DOC_ID_IND;
               
                ind.ID_NLDP = IndividuContactModel.individu.ID_NLDP;
                ind.EXO_TVA = EXO_TVA;
                ind.DAT_DEB_EXO = DAT_DEB_EXO;
                ind.DAT_FIN_EXO = DAT_FIN_EXO;
            if (IndividuContactModel.individu.REF_ADH_IND != null)
            {
                ind.REF_ADH_IND = IndividuContactModel.individu.REF_ADH_IND.TrimEnd();
                ind.INFO_IND = IndividuContactModel.individu.REF_ADH_IND.TrimEnd();
            }
            if (IndividuContactModel.individu.REF_ACH_IND != null)
            {
                ind.REF_ACH_IND = IndividuContactModel.individu.REF_ACH_IND;
            }
               
            if (IndividuContactModel.individu.TEL_IND != null)
            {
                ind.TEL_IND = IndividuContactModel.individu.TEL_IND.Replace(" ", "").TrimEnd();
            }
            if (IndividuContactModel.individu.FAX_IND != null)
            {
                ind.FAX_IND = IndividuContactModel.individu.FAX_IND.Replace(" ", "").TrimEnd();
            }
         
            
                ind.EMAIL_IND = IndividuContactModel.individu.EMAIL_IND;
             
          //  ind.REF_ADH_IND = IndividuContactModel.individu.REF_ADH_IND;

            if (web.LOGIN_WEB != null && web.PASSWORD_WEB != null)
            {
                try
                {
                    TS_USERS_WEB we = db.TS_USERS_WEB.Where(p => p.REF_IND_WEB == IndividuContactModel.individu.REF_IND).FirstOrDefault();
                    if (we == null)
                    {
                        web.REF_IND_WEB = IndividuContactModel.individu.REF_IND;
                        web.ACTIF_USER_WEB = true;
                        //  web.ID_USER_WEB = (int)Session["ID_USER"];
                        db.TS_USERS_WEB.Add(web);
                        ind.ABRV_IND = web.LOGIN_WEB;
                        db.SaveChanges();
                    }
                    else
                    {
                        we.LOGIN_WEB = web.LOGIN_WEB;
                        we.PASSWORD_WEB = web.PASSWORD_WEB;
                        ind.ABRV_IND = web.LOGIN_WEB;
                        db.Entry(we).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }
                catch (Exception e) { }
            }



            try
            {
                T_HISTORIQUE histo = new T_HISTORIQUE();
                histo.ABREV_ROLE_HIST = "Edit individu" + IndividuContactModel.individu.PRE_IND;
                histo.ACTION = "Edit Individu";
                histo.ID_ENREGISTREMENT = IndividuContactModel.individu.REF_IND.ToString();
                histo.T_TABLE = "T_INDIVIDU";
                histo.REF_IND_HIST = IndividuContactModel.individu.REF_IND.ToString();
                histo.LOGIN_USER = Session["UserLogin"].ToString();
                histo.IP_PC = HttpContext.Request.UserHostAddress;
                histo.NOM_PC = HttpContext.Request.UserHostName;
                histo.DATE_ACTION = DateTime.Now;
                db.T_HISTORIQUE.Add(histo);
                db.SaveChanges();
            }
            catch (Exception) { }

            db.Entry(ind).State = EntityState.Modified;
            db.SaveChanges();
            TempData["notice"] = "modification enregistrée avec succès";
                return RedirectToAction("EditIndividuMoral",new {id= IndividuContactModel.individu.REF_IND });
            
            this.ViewBag.LISTJURIDIQ = new SelectList(db.TR_LIST_VAL.Where(q => q.TYP_LIST_VAL == "Forme juridique"), "ABR_LIST_VAL", "LIB_LIST_VAL");
            this.ViewBag.ListValProf = new SelectList(db.TR_ACTPROF_BCT, "Code_SousClasse", "SousClasse");
            this.ViewBag.LISTPOS = new SelectList(db.TR_LIST_VAL.Where(q => q.TYP_LIST_VAL == "POSIND"), "LIB_LIST_VAL", "LIB_LIST_VAL");
            this.ViewBag.ListValLang = db.TR_NLDP.Select(p => new SelectListItem
            {
                Text = p.LIB_NT + "(" + p.ABRV_LANG + ")",
                Value = p.ID_NLDP.ToString()
            });
            this.ViewBag.TableDesContact = db.T_CONTACT.Where(p => p.REF_IND_CONTACT == IndividuContactModel.individu.REF_IND).ToList();
            this.ViewBag.ListValcp = db.TR_CP.Select(p => new SelectListItem { Text = p.Ville + " | " + p.CP, Value = p.CP });
            TempData["login"] = db.TS_USERS_WEB.Where(p => p.REF_IND_WEB == IndividuContactModel.individu.REF_IND).Select(p => p.LOGIN_WEB).FirstOrDefault();
            TempData["mdp"] = db.TS_USERS_WEB.Where(p => p.REF_IND_WEB == IndividuContactModel.individu.REF_IND).Select(p => p.PASSWORD_WEB).FirstOrDefault();
            return this.View(IndividuContactModel);
        }

   
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_INDIVIDU t_INDIVIDU = db.T_INDIVIDU.Find(id);
            if (t_INDIVIDU == null)
            {
                return HttpNotFound();
            }
            return View(t_INDIVIDU);
        }

        // POST: Individu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            T_INDIVIDU t_INDIVIDU = db.T_INDIVIDU.Find(id);
            db.T_INDIVIDU.Remove(t_INDIVIDU);
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

        /* contact **************************************************/
        public ActionResult CreateContact(int id)
        {
            if (Session["UserLogin"] != null)
            {
                T_CONTACT contact = new T_CONTACT();
                contact = db.T_CONTACT.Find(id);
                ViewBag.ListePoste = db.TR_CP.Select(p => new SelectListItem { Text = p.Ville + " | " + p.CP, Value = p.CP });
                return PartialView(contact);
            }
            else
            {
                return Json(RedirectToAction("login", "Login"));
            }
        }
        [HttpPost]
        public ActionResult CreateContact(T_CONTACT con,string POS_CONTACT)
        {
            con.POS_CONTACT = POS_CONTACT.Trim();
            db.Entry(con).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("EditIndividuMoral", new { id = con.REF_IND_CONTACT });
        }
        // POST: Individu/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.


        public ActionResult AjouterContact(int id)
        {

            if (Session["UserLogin"] != null)
            {
                ViewBag.ref_ind = id;
                ViewBag.ListePoste = db.TR_CP.Select(p => new SelectListItem { Text = p.Ville + " | " + p.CP, Value = p.CP });
                return PartialView();
            }
            else
            {
                return Json(RedirectToAction("login", "Login"));
            }

        }
        [HttpPost]
        public ActionResult AjouterContact(T_CONTACT con)
        {
            db.T_CONTACT.Add(con);
            db.SaveChanges();
            return RedirectToAction("EditIndividuMoral", new { id = con.REF_IND_CONTACT });
        }

        /* individu physqiue */


        /*** edit ind phy *////
        public ActionResult EditIndividuPhy(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.ListPiece = new SelectList(db.TR_LIST_VAL.Where(q => q.TYP_LIST_VAL == "Piece_Identite"), "ABR_LIST_VAL", "LIB_LIST_VAL");
            ViewBag.ListValProf = new SelectList(db.TR_ACTPROF_BCT, "Code_SousClasse", "SousClasse");
            ViewBag.LISTPOS = new SelectList(db.TR_LIST_VAL.Where(q => q.TYP_LIST_VAL == "POSIND"), "LIB_LIST_VAL", "LIB_LIST_VAL");
            ViewBag.ListValLang = db.TR_NLDP.Select(p => new SelectListItem
            {
                Text = p.LIB_NT + "(" + p.ABRV_LANG + ")",
                Value = p.ID_NLDP.ToString()
            });
            ViewBag.ListValcp = db.TR_CP.Select(p => new SelectListItem { Text = p.Ville + " | " + p.CP, Value = p.CP });

            ViewBag.TableDesContact = db.T_CONTACT.Where(p => p.REF_IND_CONTACT == id).ToList();
            IndividuContactModel IndividuContactModel = new IndividuContactModel();
            IndividuContactModel.individu = db.T_INDIVIDU.Find(id);
            if (IndividuContactModel.individu == null)
            {
                return HttpNotFound();
            }
            return View(IndividuContactModel);
        }

        // POST: Individu/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditIndividuPhy(IndividuContactModel IndividuContactModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(IndividuContactModel.individu).State = EntityState.Modified;
                db.SaveChanges();
                TempData["notice"] = "modification enregistrée avec succès";

                try
                {
                    T_HISTORIQUE histo = new T_HISTORIQUE();
                    histo.ABREV_ROLE_HIST = "Edit individu" + IndividuContactModel.individu.NOM_IND;
                    histo.ACTION = "Edit Individu";
                    histo.ID_ENREGISTREMENT = IndividuContactModel.individu.REF_IND.ToString();
                    histo.T_TABLE = "T_INDIVIDU";
                    histo.REF_IND_HIST = IndividuContactModel.individu.REF_IND.ToString();
                    histo.LOGIN_USER = Session["UserLogin"].ToString();
                    histo.IP_PC = HttpContext.Request.UserHostAddress;
                    histo.NOM_PC = HttpContext.Request.UserHostName;
                    histo.DATE_ACTION = DateTime.Now;
                    db.T_HISTORIQUE.Add(histo);
                    db.SaveChanges();
                }
                catch (Exception) { }
                return RedirectToAction("EditIndividuPhy", new { id = IndividuContactModel.individu.REF_IND });
            }

            this.ViewBag.ListPiece = new SelectList(db.TR_LIST_VAL.Where(q => q.TYP_LIST_VAL == "Piece_Identite"), "ABR_LIST_VAL", "LIB_LIST_VAL");
             this.ViewBag.ListValProf = new SelectList(db.TR_ACTPROF_BCT, "Code_SousClasse", "SousClasse");
             this.ViewBag.LISTPOS = new SelectList(db.TR_LIST_VAL.Where(q => q.TYP_LIST_VAL == "POSIND"), "LIB_LIST_VAL", "LIB_LIST_VAL");
            this.ViewBag.ListValLang = db.TR_NLDP.Select(p => new SelectListItem
            {
                Text = p.LIB_NT + "(" + p.ABRV_LANG + ")",
                Value = p.ID_NLDP.ToString()
            });
            this.ViewBag.TableDesContact = db.T_CONTACT.Where(p => p.REF_IND_CONTACT == IndividuContactModel.individu.REF_IND).ToList();
            this.ViewBag.ListValcp = db.TR_CP.Select(p => new SelectListItem { Text = p.Ville + " | " + p.CP, Value = p.CP });
            return this.View(IndividuContactModel);
        }
        public ActionResult DetailsIndividuPhy(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            T_INDIVIDU t_INDIVIDU = db.T_INDIVIDU.Find(id);
            try { TempData["codepostale"] = db.TR_CP.Where(p => p.CP == t_INDIVIDU.CP_IND).Select(p => p.Ville).First(); } catch (Exception) { TempData["codepostale"] = ""; }
            try
            { TempData["langue"] = db.TR_NLDP.Where(p => p.ID_NLDP == t_INDIVIDU.ID_NLDP).Select(p => p.LIB_LANG).First(); }
            catch (Exception) { TempData["langue"] = ""; }
            if (t_INDIVIDU == null)
            {
                return HttpNotFound();
            }
            ViewBag.ListContact = db.T_CONTACT.Where(p => p.REF_IND_CONTACT == id).ToList();
            return View(t_INDIVIDU);
        }

        /************ Asurence */////////////////////////
        public ActionResult CreateAssurance()
        {

            ViewBag.LISTJURIDIQ = new SelectList(db.TR_LIST_VAL.Where(q => q.TYP_LIST_VAL == "Forme juridique"), "ABR_LIST_VAL", "LIB_LIST_VAL");
            ViewBag.ListValLang = db.TR_NLDP.Select(p => new SelectListItem
            {
                Text = p.LIB_NT + "(" + p.ABRV_LANG + ")",
                Value = p.ID_NLDP.ToString()
            });
            ViewBag.ListValcp = db.TR_CP.Select(p => new SelectListItem { Text = p.Ville + " | " + p.CP, Value = p.CP });
            return View();

        }
            [HttpPost]
        public ActionResult CreateAssurance(T_INDIVIDU individu)
        {
            if (ModelState.IsValid)
            {
                individu.GENRE_IND = "ASS";
                db.T_INDIVIDU.Add(individu);
                db.SaveChanges();
                TempData["notice"] = "sauvegarde enregistrée avec succès";
                return RedirectToAction("CreateAssurance");
            }
            this.ViewBag.LISTJURIDIQ = new SelectList(db.TR_LIST_VAL.Where(q => q.TYP_LIST_VAL == "Forme juridique"), "ABR_LIST_VAL", "LIB_LIST_VAL");
            this.ViewBag.ListValLang = db.TR_NLDP.Select(p => new SelectListItem
            {
                Text = p.LIB_NT + "(" + p.ABRV_LANG + ")",
                Value = p.ID_NLDP.ToString()
            });
            this.ViewBag.ListValcp = db.TR_CP.Select(p => new SelectListItem { Text = p.Ville + " | " + p.CP, Value = p.CP });
            return View(individu);
        }

        public ActionResult SaisiRIB()
        {
            ViewBag.ADH = db.T_INDIVIDU.Select(p => new SelectListItem { Text = p.NOM_IND + " | " + p.PRE_IND, Value = p.REF_IND.ToString() });
            TempData["Individu"] = "active";
            TempData["SaisiRIB"] = "active";
            return View();
        }
        [HttpPost]
        public ActionResult SaisiRIB(TR_RIB rib , string RIB_RIB1,string RIB_RIB2,string RIB_RIB3)
        {
            try
            {
                rib.ACTIF_RIB = true;
                rib.RIB_RIB = RIB_RIB1 + RIB_RIB2 + RIB_RIB3;
                db.TR_RIB.Add(rib);
                db.SaveChanges();
                TempData["notice"] = "sauvegarde enregistrée avec succès";
            }
            catch (Exception) { return RedirectToAction("InternalServerError", "Error"); }
            return RedirectToAction("SaisiRIB");
        }



        public ActionResult RechercheBanque(string id)
        {
            string nom_bq = "";
            try {
                nom_bq = db.TR_Ag_Bq.Where(p => p.Code_Bq == id).Select(p=>p.Banque).FirstOrDefault();
            }
            catch (Exception) { nom_bq = ""; }
            return Json(nom_bq, JsonRequestBehavior.AllowGet);
        }


        public ActionResult RechercheAgence(string id)
        {
            string nom_bq = "";
            try
            {
                nom_bq = db.TR_Ag_Bq.Where(p => p.Code_Bq_Ag == id).Select(p => p.Agence).FirstOrDefault();
            }
            catch (Exception) { nom_bq = ""; }
            return Json(nom_bq, JsonRequestBehavior.AllowGet);
        }
    }
}
