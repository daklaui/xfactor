using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using xfactor.Models;
using System.IO;
using System.Configuration;
using System.Data;
using System.Net;
using System.Web.Script.Serialization;
using System.Text;

namespace xfactor.Controllers
{
    public class BordereauController : Controller
    {
        // GET: Bordereau
        private XFactor_PRODEntities1 db = new XFactor_PRODEntities1();


        internal static SqlCommand command = null;
        internal static SqlDependency dependency = null;


        public ActionResult BordereauAuto()
        {
            if (Session["UserLogin"] != null)
            {
                TempData["BordereauAuto"] = "active";
                return View();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }

        }

        public JsonResult GetNotif()
        {
            return Json(NotificaionService.GetNotification(), JsonRequestBehavior.AllowGet);
        }
        

        public JsonResult GetNameAdh(int id)
        {
            return Json(db.T_INDIVIDU.Find(id).PRE_IND, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNameAch(int id)
        {
            return Json(db.T_INDIVIDU.Find(id).PRE_IND, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjouterBordereau()
        {
            if (Session["UserLogin"] != null)
            {
              
                ViewBag.ADH = db.Recherche_CTR_ADH().GroupBy(p => p.RefAdh).Select(p => p.FirstOrDefault()).ToList();

                TempData["Bordereau"] = "active";
                TempData["AjouterBordereau"] = "active";
                return View();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
      //  [HttpPost]
        public ActionResult AjouterBorderea(int REF_B,int REF_CTR_B,string NUM_B,string ANNEE_B,int NB_DOC_B,string MONT_TOT_B,string DAT_SAISIE_BO, List<T_DET_BORD> Det_Bord)
        {
            try
            {
                //   int a = 0;
                // enregistrement de bordereau (entéte)
                T_BORDEREAU bord = new T_BORDEREAU();
                bord.REF_CTR_BORD = REF_CTR_B;
                bord.REF_ADH_BORD = REF_B;
                bord.NUM_BORD = NUM_B;
                bord.ANNEE_BORD = ANNEE_B;
                bord.NB_DOC_BORD = (short)NB_DOC_B;
                bord.MONT_TOT_BORD = decimal.Parse(MONT_TOT_B.Replace(".",","));
                bord.VALIDE_BORD = false;
                bord.DAT_BORD= DateTime.Parse(DAT_SAISIE_BO);
                bord.DAT_SAISIE_BORD = DateTime.Today;
                db.T_BORDEREAU.Add(bord);
                db.SaveChanges();
                // enregistrement des détails bord
                foreach (T_DET_BORD bo in Det_Bord)
                {
                    string ref_det_bord = bo.REF_DET_BORD;
                    bo.REF_DET_BORD = "";
                    // les listes des documents repeter sur meme bordereau 
                    List<TJ_DOCUMENT_DET_BORD> docdetbord = db.TJ_DOCUMENT_DET_BORD.Where(p => p.NUM_BORD == bo.NUM_BORD && p.REF_CTR_DET_BORD == REF_CTR_B && p.REF_DOCUMENT_DET_BORD == ref_det_bord).ToList();
                    // les listes des documents repeter sur meme contrat
                    List<TJ_DOCUMENT_DET_BORD> docdetbord2 = db.TJ_DOCUMENT_DET_BORD.Where( p=>p.REF_CTR_DET_BORD == REF_CTR_B && p.REF_DOCUMENT_DET_BORD == ref_det_bord).ToList();

                    if (docdetbord.Count == 0 && docdetbord2.Count==0)
                    {
                        int ref_max = 0;
                        bo.MONT_TTC_DET_BORD = decimal.Parse(bo.COMM_DET_BORD.Replace(".", ","));
                        string x = "";
                        switch (bo.TYP_DET_BORD.ToString())
                        {
                            case "fact": x = "F"; break;
                            case "Caut": x = "C"; break;
                            case "bdc": x = "BC"; break;
                            case "march": x = "M"; break;
                        }
                        T_FOND_GARANTIE f = db.T_FOND_GARANTIE.Where(p => p.REF_CTR_FDG == REF_CTR_B && p.TYP_FDG == x).SingleOrDefault();
                        T_COMM_FACTORING cf = db.T_COMM_FACTORING.Where(p => p.REF_CTR_COMM_FACTORING == REF_CTR_B && p.TYP_COMM_FACTORING == x).SingleOrDefault();

                        if (cf.TX_COMM_FACTORING.ToString() == "")
                        {
                            cf.TX_COMM_FACTORING = 0;
                        }
                        if (f.TX_FDG.ToString() == "")
                        {
                            f.TX_FDG = 0;
                        }
                        //Recuperation des Taux du FDG et de la comm. de Factoring depuis le contrat selectionner
                        decimal TX_COMM_FACTORING = decimal.Parse(cf.TX_COMM_FACTORING.ToString().Replace(".", ","));
                        TX_COMM_FACTORING = TX_COMM_FACTORING / 100;
                        decimal TX_FDG = decimal.Parse(f.TX_FDG.ToString().Replace(".", ","));
                        TX_FDG=TX_FDG / 100;
                        decimal ITauxFDGF = decimal.Parse(f.TX_FDG.ToString().Replace(".", ","));
                        //Recuperation du mnt minimum de la comm. de Factoring depuis le contrat selectionner
                        decimal mONT_MIN_DOC_COMM_FACTORING;
                        try { mONT_MIN_DOC_COMM_FACTORING = decimal.Parse(cf.MONT_MIN_DOC_COMM_FACTORING.ToString().Replace(".", ",")); } catch (Exception e) { mONT_MIN_DOC_COMM_FACTORING = 0; };
                        decimal V_IMTCOMFAC = TX_COMM_FACTORING * (decimal)bo.MONT_TTC_DET_BORD;

                        decimal IMTCOMFAC = 0;
                        if (mONT_MIN_DOC_COMM_FACTORING > V_IMTCOMFAC)
                        {
                            IMTCOMFAC = mONT_MIN_DOC_COMM_FACTORING;
                        }
                        else
                        {
                            IMTCOMFAC = V_IMTCOMFAC;
                        }
                        decimal TauxTVA = 0.19M;
                        decimal IMtTVACOM = TauxTVA * IMTCOMFAC;
                        decimal IMTCOMTTC = IMTCOMFAC + IMtTVACOM;
                        string query = "select max(Convert(int,ID_DET_BORD)) from T_DET_BORD";
                        ref_max = db.Database.SqlQuery<int>(query).First();
                        bo.REF_CTR_DET_BORD = bord.REF_CTR_BORD;
                        bo.NUM_BORD = bord.NUM_BORD;
                        bo.ANNEE_BORD = bord.ANNEE_BORD;
                        bo.MONT_OUV_DET_BORD = decimal.Parse(bo.COMM_DET_BORD.Replace(".", ","));
                        bo.ANNUL_DET_BORD = false;
                        bo.ID_DET_BORD = (ref_max + 1).ToString();

                        bo.TX_FDG_DET_BORD = decimal.Parse(f.TX_FDG.ToString().Replace(".", ","));
                        bo.TX_COMM_FACT_DET_BORD = decimal.Parse(cf.TX_COMM_FACTORING.ToString().Replace(".", ","));
                        decimal MONT_FDG_DET_BORD = TX_FDG * (decimal)bo.MONT_TTC_DET_BORD;
                        bo.MONT_FDG_LIBERE_DET_BORD = 0;
                        bo.MONT_FDG_DET_BORD = Math.Round(MONT_FDG_DET_BORD, 3);
                        bo.MONT_COMM_FACT_DET_BORD = IMTCOMFAC;
                        bo.TX_TVA_COMM_FACT_DET_BORD = TauxTVA;
                        bo.MONT_TVA_COMM_FACT_DET_BORD = IMtTVACOM;
                        bo.MONT_TTC_COMM_FACT_DET_BORD = IMTCOMTTC;
                        bo.VALIDE_DET_BORD = false;
                        bo.COMM_DET_BORD = "";
                        bo.DEVISE_DET_BORD = "TND";
                        db.T_DET_BORD.Add(bo);

                        TJ_DOCUMENT_DET_BORD det = new TJ_DOCUMENT_DET_BORD();
                        det.NUM_BORD = bo.NUM_BORD;
                        det.REF_CTR_DET_BORD = bo.REF_CTR_DET_BORD;
                        det.ID_DET_BORD = bo.ID_DET_BORD;
                        det.REF_DOCUMENT_DET_BORD = ref_det_bord;
                        db.TJ_DOCUMENT_DET_BORD.Add(det);

                        db.SaveChanges();


                        


                    }
                    else
                    {
                        // TempData["errorBord"] = "La référence de facture  " + ref_det_bord + " est deja sauvgarder merci de verifier";
                        // return RedirectToAction("InternalServerError", "Error");
                      
                        return Json("La référence de facture  " + ref_det_bord + " est deja sauvgarder merci de verifier", JsonRequestBehavior.AllowGet);
                      //  break;
                        //break;
                    }
                  
                    
                }

                try
                {
                    T_DOC_GED ged = new T_DOC_GED();
                    ged.ID_IND_GED = REF_B;
                    ged.ID_CTR_GED = REF_CTR_B;
                    ged.ID_BOR_GED = int.Parse(NUM_B);
                    ged.LIBELLE_GED = "Bordereau";
                    ged.DATE_TACHE_GED = DateTime.Now;
                    ged.ID_GESTIONNAIRE_GED = int.Parse(Session["ID_USER"].ToString()); ;
                    ged.LIBELLE2_GED = NUM_B;
                    ged.ID_Emetteur_GED = Session["ID_USER"].ToString(); ;
                    ged.Etape_ged = "Création";
                    ged.Etat_GED = false;
                    db.T_DOC_GED.Add(ged);
                    db.SaveChanges();
                }
                catch (Exception) { };


                try
                {
                    T_HISTORIQUE histo = new T_HISTORIQUE();
                    histo.ABREV_ROLE_HIST = "Creation bordereau " + bord.NUM_BORD;
                    histo.ACTION = "Creation bordereau";
                    histo.ID_ENREGISTREMENT = bord.NUM_BORD.ToString();
                    histo.T_TABLE = "T_BORDEREAU";
                    histo.REF_CTR_HIST = bord.REF_CTR_BORD.ToString();
                    histo.REF_IND_HIST = bord.REF_ADH_BORD.ToString();
                    histo.LOGIN_USER = Session["UserLogin"].ToString();
                    histo.IP_PC = GetIp();
                    histo.NOM_PC = HttpContext.Request.UserHostName;
                    histo.DATE_ACTION = DateTime.Now;
                    db.T_HISTORIQUE.Add(histo);
                    db.SaveChanges();
                }
                catch (Exception e) { TempData["error"] = e.Message; }


                TempData["notice"] = "Enregistrement a été effectué avec succès ";
                return Json("save", JsonRequestBehavior.AllowGet);
               // return RedirectToAction("AjouterBordereau");
            }
            catch (Exception e) { //return RedirectToAction("InternalServerError", "Error"); 
                //TempData["errorBord"] = e.Message;
                return Json(e.Message, JsonRequestBehavior.AllowGet);
            }
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

        public JsonResult AjouterDetBord(T_DET_BORD det_bord)
        {
           
            if (ModelState.IsValid)
            {
             
              
            }
            return Json(det_bord.REF_CTR_DET_BORD);
        }

        public JsonResult RecherchCtrAdh(int id)
        {
            List<int> refctr = new List<int>();
            refctr = db.TJ_CIR.Where(p => p.REF_IND_CIR == id && p.ID_ROLE_CIR== "ADH" && p.REF_CTR_CIR != 28 && p.REF_CTR_CIR != 32 && p.REF_CTR_CIR != 38 && p.REF_CTR_CIR != 54 && p.REF_CTR_CIR != 56 && p.REF_CTR_CIR != 58 && p.REF_CTR_CIR != 65 && p.REF_CTR_CIR != 69 && p.REF_CTR_CIR != 72 && p.REF_CTR_CIR != 74 && p.REF_CTR_CIR != 80 && p.REF_CTR_CIR != 82 && p.REF_CTR_CIR != 93 && p.REF_CTR_CIR != 96).Select(p => p.REF_CTR_CIR ).ToList();
            return Json(refctr, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemplirTableau(int id)
        {

            //List<String> ListAch = new List<string>();
            //List<List<String>> ListAchTotal = new List<List<String>>();
            var test = (from q in db.T_DET_BORD
                        join q2 in db.T_INDIVIDU
                          on q.REF_IND_DET_BORD equals q2.REF_IND
                        where (q.REF_CTR_DET_BORD == id)
                        select new { q2.NOM_IND,q.REF_DET_BORD, q.REF_IND_DET_BORD, q.TYP_DET_BORD, q.DAT_DET_BORD, q.MONT_TTC_DET_BORD, q.ECH_DET_BORD, q.MONT_OUV_DET_BORD, q.TX_FDG_DET_BORD, q.TX_COMM_FACT_DET_BORD, q.REF_CTR_DET_BORD });
            var rst = db.T_DET_BORD.Where(p => p.REF_CTR_DET_BORD == id).Select(p=>p.ANNEE_BORD).ToList();

            return Json(test, JsonRequestBehavior.AllowGet);
        }

        ActionResult GetNamInd(int id)
        {

            return Json(db.T_INDIVIDU.Find(id).NOM_IND, JsonRequestBehavior.AllowGet);
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
        public JsonResult RecherchCtrAch2(int id)
        {
            var ListNomInd = db.Recherche_CTR_ADH_ACH_Par_CTR_IND(id);
            return Json(ListNomInd, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RecherchCtrAch22(int id)
        {
            T_CONTRAT contrat = db.T_CONTRAT.Where(p => p.REF_CTR == id).SingleOrDefault();
            switch (contrat.TYP_CTR)
            {
                case "1": TempData["Assurence"] = true; break;
                case "2": TempData["Assurence"] = true; break;
                case "3": TempData["Assurence"] = false; break;

            }
            var ListNomInd = db.Recherche_CTR_ADH_ACH_Par_CTR2019(id);
            return Json(ListNomInd, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LETTRE_RESTE_ENC(int id)
        {
            var ListNomInd = db.LETTRE_RESTE_ENC(id);
            //var rst = new
            //{
            //    REST= ListNomInd.RESTE

            //};
            return Json(ListNomInd, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RecherchCtrAch3(int id)
        {
            
            var test = db.T_Encaissement_materialiser_par_ref_ctr2(id);
            return Json(test, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListeDesEncaissement(int id)
        {
            ViewBag.ListeDesEncaissements = db.T_Encaissement_materialiser_par_ref_ctr2(id);
            return PartialView();
        }
        XFactor_PRODDataSet dd = new XFactor_PRODDataSet();
        public JsonResult RecherchFacture(int id,int id2)
        {
            var test = db.ListeFactureValiderkV2(id,id2);
           
            return Json(test, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRibParRef_IND(int id)
        {
            List<string> rib;
            try { rib = db.TR_RIB.Where(p => p.REF_IND_RIB == id).Select(p => p.RIB_RIB).ToList(); } catch (Exception) { rib = null; }
            //var ListNomInd = ;
            return Json(rib, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ValiderBordereau()
        {
            if (Session["UserLogin"] != null)
            {
                string sql = "select NUM_BORD, REF_CTR_BORD, NB_DOC_BORD, ANNEE_BORD,DAT_SAISIE_BORD," +
                        "(select count(*) from T_DET_BORD where T_DET_BORD.NUM_BORD = T_BORDEREAU.NUM_BORD and T_DET_BORD.ANNEE_BORD = T_BORDEREAU.ANNEE_BORD and REF_CTR_DET_BORD = REF_CTR_BORD) Nbre_Det," +
                        "(select nom_ind from T_INDIVIDU where REF_IND = REF_ADH_BORD) Nom_ADH," +
                        "(select isnull(SUM(MONT_TTC_DET_BORD), 0) from T_DET_BORD where T_DET_BORD.NUM_BORD = T_BORDEREAU.NUM_BORD and T_DET_BORD.ANNEE_BORD = T_BORDEREAU.ANNEE_BORD and REF_CTR_DET_BORD = REF_CTR_BORD) MontantTotale" +
                       " from T_BORDEREAU where VALIDE_BORD = 0";
                ViewBag.validbdr = (db.Database.SqlQuery<ValiderBordList>(sql).ToList());
                TempData["Bordereau"] = "active";
                TempData["ValiderBordereau"] = "active";
                return View();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }

        public ActionResult DetailleBord(string id,int id2,int id3)
        {
            if (Session["UserLogin"] != null)
            {
                string sql = "select ID_DET_BORD,NUM_BORD,(select REF_DOCUMENT_DET_BORD from TJ_DOCUMENT_DET_BORD where ID_DET_BORD = T_DET_BORD.ID_DET_BORD) as REF_DET_BORD,TYP_DET_BORD,DAT_DET_BORD,MONT_TTC_DET_BORD,ANNEE_BORD,REF_CTR_DET_BORD,ECH_DET_BORD,MODE_REG_DET_BORD ,(select NOM_IND from T_INDIVIDU where REF_IND = REF_IND_DET_BORD) as nomind from t_det_bord where num_bord = '" + id+ "' and REF_CTR_DET_BORD="+id2+ " and ANNEE_BORD="+id3+" and  VALIDE_DET_BORD = 0  ";
                ViewBag.Detaille = (db.Database.SqlQuery<DetailleBord>(sql).ToList());
                return PartialView();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }

        public JsonResult sendMail()
        {
            SendEmail email = new SendEmail("Notifiaction_Bordereau", "Notifiaction Bordereau", "test");
            if (email.SendEmailXpert()=="true")
            {
                return Json("true", JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json(email.SendEmailXpert(), JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetAllBordNonValide()
        {
            string sql = "select NUM_BORD,REF_CTR_BORD,NB_DOC_BORD,ANNEE_BORD," +
                         "(select count(*) from T_DET_BORD where T_DET_BORD.NUM_BORD = T_BORDEREAU.NUM_BORD and REF_CTR_DET_BORD = REF_CTR_BORD) Nbre_Det," +
                         "(select nom_ind from T_INDIVIDU where REF_IND = REF_ADH_BORD) Nom_ADH," +
                         "(select isnull(SUM(MONT_TTC_DET_BORD), 0) from T_DET_BORD where T_DET_BORD.NUM_BORD = T_BORDEREAU.NUM_BORD and REF_CTR_DET_BORD = REF_CTR_BORD) MontantTotale"+
                        "from T_BORDEREAU where VALIDE_BORD = 0";
            return Json(db.Database.SqlQuery<object>(sql).ToList());
        }
 
        public ActionResult ValiderBordereauJson(string id,int id2,int id3)
        {

            //db.Update_T_BORD(id2, id.ToString());
            //db.Update_T_DET_BORD(id2, id.ToString());

       

            T_BORDEREAU bor = db.T_BORDEREAU.Where(p => p.NUM_BORD == id && p.REF_CTR_BORD == id2).FirstOrDefault();
            List<T_DET_BORD> bord = db.T_DET_BORD.Where(p => p.REF_CTR_DET_BORD == id2 && p.NUM_BORD == id).ToList();

            String BodyEmail = "<html><body><H3>Bordereau :"+bor.NUM_BORD+" | Contrat : "+bor.REF_CTR_BORD + " | Nom adhérent :"+db.T_INDIVIDU.Find(bor.REF_ADH_BORD).NOM_IND +" </H3><table border=2 style='width: 100 %;border: 1px solid black;font-size:small'> ";
            BodyEmail = BodyEmail + "<tr><th> Ref contrat </th> <th> Nom adherent </th> <th> Code adherent MFG </th><th> Num bordereau </th> <th>Date bordereau </th>  <th> Date Saisi bordereau  </th><th> Montant Bordereau </th></tr> ";

            if (bor != null && bord != null)
            {
                BodyEmail = BodyEmail = BodyEmail + " <tr> <td> " + bor.REF_CTR_BORD + " </td> ";
                BodyEmail = BodyEmail + " <td> " + db.T_INDIVIDU.Find(db.TJ_CIR.Where(p => p.REF_CTR_CIR == bor.REF_CTR_BORD && p.ID_ROLE_CIR == "ADH").FirstOrDefault().REF_IND_CIR).NOM_IND + " </td> ";
                BodyEmail = BodyEmail + " <td> " + db.T_INDIVIDU.Find(db.TJ_CIR.Where(p => p.REF_CTR_CIR == bor.REF_CTR_BORD && p.ID_ROLE_CIR == "ADH").FirstOrDefault().REF_IND_CIR).REF_ADH_IND + " </td> ";
                BodyEmail = BodyEmail + " <td> " +bor.NUM_BORD + " </td> ";
                BodyEmail = BodyEmail + " <td> " + DateTime.Parse(bor.DAT_BORD.ToString()).ToString("d") + " </td> ";
                BodyEmail = BodyEmail + " <td> " + DateTime.Parse(bor.DAT_SAISIE_BORD.ToString()).ToString("d") + " </td> ";
                BodyEmail = BodyEmail + " <td> " + bor.MONT_TOT_BORD + " </td>  </tr>";
                BodyEmail = BodyEmail + "<tr><td colspan='7'>Details Bordereau</td></tr>";
                bor.VALIDE_BORD = true;
                BodyEmail = BodyEmail + "<tr> <th>acheteur </th> <th> Code acheteur MFG </th><th> Code  MFG </th><th>  Montant  facture  </th><th>  Montant Commission  </th>  <th>Ref facture </th><th> Date facture </th></tr> ";
                foreach (var item in bord)
                {

                    item.VALIDE_DET_BORD = true;
              
                    BodyEmail = BodyEmail + "<tr> <td> " + db.T_INDIVIDU.Find(item.REF_IND_DET_BORD).NOM_IND + " </td> ";
                    BodyEmail = BodyEmail + " <td> " + db.T_INDIVIDU.Find(item.REF_IND_DET_BORD).REF_ACH_IND + " </td> ";
                    BodyEmail = BodyEmail + " <td> ZZ70501101 </td>";
                    BodyEmail = BodyEmail + " <td> " + item.MONT_TTC_DET_BORD + "</td>";
                    BodyEmail = BodyEmail + " <td> " + item.MONT_COMM_FACT_DET_BORD + "</td>";
                    BodyEmail = BodyEmail + " <td> " + db.TJ_DOCUMENT_DET_BORD.Where(p => p.ID_DET_BORD == item.ID_DET_BORD).FirstOrDefault().REF_DOCUMENT_DET_BORD + "</td>";
                    BodyEmail = BodyEmail + " <td> " +DateTime.Parse(item.DAT_DET_BORD.ToString()).ToString("d") + "</td></tr>";
                    //  BodyEmail = BodyEmail + " <td> " + bor.DAT_SAISIE_BORD + "</td> </tr>";
                    //Les CRE BORD 




                    T_EC_CPT cp = new T_EC_CPT();
                    try
                    {

                        cp.ID_EC_CPT = db.T_EC_CPT.Max(p => p.ID_EC_CPT) + 1;
                        cp.ANNEE_EC_CPT = DateTime.Now.Year;
                        cp.CODE_DEP_EC_CPT = "A301";
                        cp.NUM_LIGNE_EC_CPT = 1;
                        cp.CODE_JOURNAL_EC_CPT = "JV";
                        cp.DATE_SAISIE_EC_CPT = DateTime.Now;
                        cp.DATE_EFFET_EC_CPT = DateTime.Now;
                        cp.COMPTE_GEN_EC_CPT = "40900000";
                        cp.MONTANT_EC_CPT = item.MONT_COMM_FACT_DET_BORD + 5 / 10;
                        cp.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                        cp.TYPE_TRANSACTION_EC_CPT = "SO";
                        cp.TYPE_DOC_EC_CPT = "I";
                        cp.NOM_USER_EC_CPT = Session["UserLogin"].ToString();
                        cp.CODE_TIERS_EC_CPT = bor.REF_ADH_BORD.ToString();
                        cp.REF_PIECE_EC_CPT =db.TJ_DOCUMENT_DET_BORD.Where(p=>p.ID_DET_BORD==item.ID_DET_BORD).SingleOrDefault().REF_DOCUMENT_DET_BORD;
                        cp.LOT_EC_CPT = item.NUM_BORD;
                        cp.DOMAINE_EC_CPT = "medfact";
                        cp.DATE_EC_CPT = DateTime.Now;
                        cp.INTITULE_EC_CPT = "Commission d affacturage";
                        db.T_EC_CPT.Add(cp);
                        db.SaveChanges();
                    }
                    catch (Exception) { }

                    //CRE 
                    T_EC_CPT cp2 = new T_EC_CPT();
                    try
                    {

                        cp2.ID_EC_CPT = db.T_EC_CPT.Max(p => p.ID_EC_CPT) + 1;
                        cp2.ANNEE_EC_CPT = DateTime.Now.Year;
                        cp2.CODE_DEP_EC_CPT = "A301";
                        cp2.NUM_LIGNE_EC_CPT = 2;
                        cp2.CODE_JOURNAL_EC_CPT = "JV";
                        cp2.DATE_SAISIE_EC_CPT = DateTime.Now;
                        cp2.DATE_EFFET_EC_CPT = DateTime.Now;
                        cp2.COMPTE_GEN_EC_CPT = "70501100";
                        cp2.MONTANT_EC_CPT = 0 - item.MONT_COMM_FACT_DET_BORD;
                        cp2.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                        cp2.TYPE_TRANSACTION_EC_CPT = "SO";
                        cp2.TYPE_DOC_EC_CPT = "I";
                        cp2.NOM_USER_EC_CPT = Session["UserLogin"].ToString();
                        cp2.CODE_TIERS_EC_CPT = bor.REF_ADH_BORD.ToString();
                        cp2.REF_PIECE_EC_CPT = db.TJ_DOCUMENT_DET_BORD.Where(p => p.ID_DET_BORD == item.ID_DET_BORD).SingleOrDefault().REF_DOCUMENT_DET_BORD;
                        cp2.LOT_EC_CPT = item.NUM_BORD;
                        cp2.DOMAINE_EC_CPT = "medfact";
                        cp2.DATE_EC_CPT = DateTime.Now;
                        cp2.INTITULE_EC_CPT = "Commission";
                        db.T_EC_CPT.Add(cp2);
                        db.SaveChanges();
                    }
                    catch (Exception) { }


                    //CRE 
                    T_EC_CPT cp3 = new T_EC_CPT();
                    try
                    {

                        cp3.ID_EC_CPT = db.T_EC_CPT.Max(p => p.ID_EC_CPT) + 1;
                        cp3.ANNEE_EC_CPT = DateTime.Now.Year;
                        cp3.CODE_DEP_EC_CPT = "A301";
                        cp3.NUM_LIGNE_EC_CPT = 3;
                        cp3.CODE_JOURNAL_EC_CPT = "JV";
                        cp3.DATE_SAISIE_EC_CPT = DateTime.Now;
                        cp3.DATE_EFFET_EC_CPT = DateTime.Now;
                        cp3.COMPTE_GEN_EC_CPT = "43671000";
                        cp3.MONTANT_EC_CPT = 0 - item.MONT_TVA_COMM_FACT_DET_BORD;
                        cp3.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                        cp3.TYPE_TRANSACTION_EC_CPT = "SO";
                        cp3.TYPE_DOC_EC_CPT = "I";
                        cp3.NOM_USER_EC_CPT = Session["UserLogin"].ToString();
                        cp3.CODE_TIERS_EC_CPT = bor.REF_ADH_BORD.ToString();
                        cp3.REF_PIECE_EC_CPT = db.TJ_DOCUMENT_DET_BORD.Where(p => p.ID_DET_BORD == item.ID_DET_BORD).SingleOrDefault().REF_DOCUMENT_DET_BORD;
                        cp3.LOT_EC_CPT = item.NUM_BORD;
                        cp3.DOMAINE_EC_CPT = "medfact";
                        cp3.DATE_EC_CPT = DateTime.Now;
                        cp3.INTITULE_EC_CPT = "TVA sur Commission d affacturage";
                        db.T_EC_CPT.Add(cp3);
                        db.SaveChanges();
                    }
                    catch (Exception) { }

                    //CRE 
                    T_EC_CPT cp4 = new T_EC_CPT();
                    try
                    {

                        cp4.ID_EC_CPT = db.T_EC_CPT.Max(p => p.ID_EC_CPT) + 1;
                        cp4.ANNEE_EC_CPT = DateTime.Now.Year;
                        cp4.CODE_DEP_EC_CPT = "A301";
                        cp4.NUM_LIGNE_EC_CPT = 4;
                        cp4.CODE_JOURNAL_EC_CPT = "JV";
                        cp4.DATE_SAISIE_EC_CPT = DateTime.Now;
                        cp4.DATE_EFFET_EC_CPT = DateTime.Now;
                        cp4.COMPTE_GEN_EC_CPT = "43700000";
                        cp4.MONTANT_EC_CPT = 0 - 5 / 10;
                        cp4.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                        cp4.TYPE_TRANSACTION_EC_CPT = "SO";
                        cp4.TYPE_DOC_EC_CPT = "I";
                        cp4.NOM_USER_EC_CPT = Session["UserLogin"].ToString();
                        cp4.CODE_TIERS_EC_CPT = bor.REF_ADH_BORD.ToString();
                        cp4.REF_PIECE_EC_CPT = item.ID_DET_BORD;
                        cp4.LOT_EC_CPT = item.NUM_BORD;
                        cp4.DOMAINE_EC_CPT = "medfact";
                        cp4.DATE_EC_CPT = DateTime.Now;
                        cp4.INTITULE_EC_CPT = "Droit de timbre sur Commission d affacturage";
                        db.T_EC_CPT.Add(cp4);
                        db.SaveChanges();
                    }
                    catch (Exception) { }

                    //CRE 
                    T_EC_CPT cp5 = new T_EC_CPT();
                    try
                    {

                        cp5.ID_EC_CPT = db.T_EC_CPT.Max(p => p.ID_EC_CPT) + 1;
                        cp5.ANNEE_EC_CPT = DateTime.Now.Year;
                        cp5.CODE_DEP_EC_CPT = "A301";
                        cp5.NUM_LIGNE_EC_CPT = 1;
                        cp5.CODE_JOURNAL_EC_CPT = "ME";
                        cp5.DATE_SAISIE_EC_CPT = DateTime.Now;
                        cp5.DATE_EFFET_EC_CPT = DateTime.Now;
                        cp5.COMPTE_GEN_EC_CPT = "41700000";
                        cp5.MONTANT_EC_CPT = item.MONT_TTC_DET_BORD;
                        cp5.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                        cp5.TYPE_TRANSACTION_EC_CPT = "AR";
                        cp5.TYPE_DOC_EC_CPT = "M";
                        cp5.NOM_USER_EC_CPT = Session["UserLogin"].ToString();
                        cp5.CODE_TIERS_EC_CPT = item.REF_IND_DET_BORD.ToString();
                        cp5.REF_PIECE_EC_CPT = db.TJ_DOCUMENT_DET_BORD.Where(p => p.ID_DET_BORD == item.ID_DET_BORD).SingleOrDefault().REF_DOCUMENT_DET_BORD;
                        cp5.LOT_EC_CPT = item.NUM_BORD;
                        cp5.DOMAINE_EC_CPT = "medfact";
                        cp5.DATE_EC_CPT = DateTime.Now;
                        cp5.INTITULE_EC_CPT = "Saisie facture";
                        db.T_EC_CPT.Add(cp5);
                        db.SaveChanges();
                    }
                    catch (Exception) { }

                    //CRE 
                    T_EC_CPT cp6 = new T_EC_CPT();
                    try
                    {

                        cp6.ID_EC_CPT = db.T_EC_CPT.Max(p => p.ID_EC_CPT) + 1;
                        cp6.ANNEE_EC_CPT = DateTime.Now.Year;
                        cp6.CODE_DEP_EC_CPT = "A301";
                        cp6.NUM_LIGNE_EC_CPT = 2;
                        cp6.CODE_JOURNAL_EC_CPT = "ME";
                        cp6.DATE_SAISIE_EC_CPT = DateTime.Now;
                        cp6.DATE_EFFET_EC_CPT = DateTime.Now;
                        cp6.COMPTE_GEN_EC_CPT = "40900000";
                        cp6.MONTANT_EC_CPT = 0 - item.MONT_TTC_DET_BORD - item.MONT_FDG_DET_BORD;
                        cp6.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                        cp6.TYPE_TRANSACTION_EC_CPT = "AR";
                        cp6.TYPE_DOC_EC_CPT = "M";
                        cp6.NOM_USER_EC_CPT = Session["UserLogin"].ToString();
                        cp6.CODE_TIERS_EC_CPT = bor.REF_ADH_BORD.ToString();
                        cp6.REF_PIECE_EC_CPT = db.TJ_DOCUMENT_DET_BORD.Where(p => p.ID_DET_BORD == item.ID_DET_BORD).SingleOrDefault().REF_DOCUMENT_DET_BORD;
                        cp6.LOT_EC_CPT = item.NUM_BORD;
                        cp6.DOMAINE_EC_CPT = "medfact";
                        cp6.DATE_EC_CPT = DateTime.Now;
                        cp6.INTITULE_EC_CPT = "Montant financé facture";
                        db.T_EC_CPT.Add(cp6);
                        db.SaveChanges();
                    }
                    catch (Exception) { }

                    //CRE 
                    T_EC_CPT cp7 = new T_EC_CPT();
                    try
                    {

                        cp7.ID_EC_CPT = db.T_EC_CPT.Max(p => p.ID_EC_CPT) + 1;
                        cp7.ANNEE_EC_CPT = DateTime.Now.Year;
                        cp7.CODE_DEP_EC_CPT = "A301";
                        cp7.NUM_LIGNE_EC_CPT = 2;
                        cp7.CODE_JOURNAL_EC_CPT = "ME";
                        cp7.DATE_SAISIE_EC_CPT = DateTime.Now;
                        cp7.DATE_EFFET_EC_CPT = DateTime.Now;
                        cp7.COMPTE_GEN_EC_CPT = "40700000";
                        cp7.MONTANT_EC_CPT = 0 - item.MONT_FDG_DET_BORD;
                        cp7.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                        cp7.TYPE_TRANSACTION_EC_CPT = "AR";
                        cp7.TYPE_DOC_EC_CPT = "M";
                        cp7.NOM_USER_EC_CPT = Session["UserLogin"].ToString();
                        cp7.CODE_TIERS_EC_CPT = bor.REF_ADH_BORD.ToString();
                        cp7.REF_PIECE_EC_CPT = db.TJ_DOCUMENT_DET_BORD.Where(p => p.ID_DET_BORD == item.ID_DET_BORD).SingleOrDefault().REF_DOCUMENT_DET_BORD;
                        cp7.LOT_EC_CPT = item.NUM_BORD;
                        cp7.DOMAINE_EC_CPT = "medfact";
                        cp7.DATE_EC_CPT = DateTime.Now;
                        cp7.INTITULE_EC_CPT = "FDG sur facture";
                        db.T_EC_CPT.Add(cp7);
                        db.SaveChanges();
                    }
                    catch (Exception) { }




                }

                BodyEmail = BodyEmail + "<tr></tr> ";
                BodyEmail = BodyEmail + "<tr><th> Total Commission  </th> <td>"+bord.Sum(p=>p.MONT_COMM_FACT_DET_BORD)+"</td></tr> ";
                BodyEmail = BodyEmail + "<tr><th> Total FDG  </th> <td>" + bord.Sum(p => p.MONT_FDG_DET_BORD) + "</td></tr> ";
               

                BodyEmail = BodyEmail + "</table></body></html>";
                db.SaveChanges();
                SendEmail email = new SendEmail("Notifiaction_Bordereau", "Notifiaction Bordereau "+bor.REF_CTR_BORD+" | "+db.T_INDIVIDU.Find(bor.REF_ADH_BORD).NOM_IND, BodyEmail);
                email.SendEmailXpert();
                TempData["notice"] = "enregistrement a été effectué avec succès"; }
            else { return RedirectToAction("InternalServerError", "Error"); }

            /******* Les Event */
     
            try
            {
                T_EXTRAIT extrait = new T_EXTRAIT();
                extrait.REF_CTR_EXTRAIT = bor.REF_CTR_BORD;
                extrait.LIB_EXTRAIT = "Achat Bordereau - N° " + bor.NUM_BORD;
                extrait.CREDIT_EXTRAIT = bor.MONT_TOT_BORD;
                extrait.DEBIT_EXTRAIT = 0;
                extrait.AUTRE_EXTRAIT = 0;
                extrait.ENCOURFACT_EXTRAIT = db.All_Ecran_Financements(bor.REF_CTR_BORD).Select(p => p.Encours_Facture).FirstOrDefault();
                extrait.DISPONIBLE_EXTRAIT = db.All_Ecran_Financements(bor.REF_CTR_BORD).Select(p => p.Disponible).FirstOrDefault();
                extrait.DAT_OP_EXTRAIT = DateTime.Now;
                extrait.DAT_VAL_EXTRAIT = bor.DAT_BORD;
                extrait.TOTAL_FIN_EXTRAIT = db.All_Ecran_Financements(bor.REF_CTR_BORD).Select(p => p.Total_Financement).FirstOrDefault();
                db.T_EXTRAIT.Add(extrait);
                db.SaveChanges();

            }
            catch (Exception) { }

            try
            {
                T_EXTRAIT extrait = new T_EXTRAIT();
                extrait.REF_CTR_EXTRAIT = bor.REF_CTR_BORD;
                extrait.LIB_EXTRAIT = "Commission - Bordereau N° " + bor.NUM_BORD;
                extrait.DEBIT_EXTRAIT = db.T_DET_BORD.Where(p => p.ANNEE_BORD == bor.ANNEE_BORD && p.NUM_BORD == id && p.REF_CTR_DET_BORD == id2).Sum(p => p.MONT_COMM_FACT_DET_BORD);
                extrait.CREDIT_EXTRAIT = 0;
                extrait.AUTRE_EXTRAIT = 0;
                extrait.ENCOURFACT_EXTRAIT = db.All_Ecran_Financements(bor.REF_CTR_BORD).Select(p => p.Encours_Facture).FirstOrDefault();
                extrait.DISPONIBLE_EXTRAIT = db.All_Ecran_Financements(bor.REF_CTR_BORD).Select(p => p.Disponible).FirstOrDefault();
                extrait.DAT_OP_EXTRAIT = DateTime.Now;
                extrait.DAT_VAL_EXTRAIT = bor.DAT_BORD;
                extrait.TOTAL_FIN_EXTRAIT = db.All_Ecran_Financements(bor.REF_CTR_BORD).Select(p => p.Total_Financement).FirstOrDefault();
                db.T_EXTRAIT.Add(extrait);
                db.SaveChanges();

            }
            catch (Exception) { }

            try
            {
                T_EXTRAIT extrait = new T_EXTRAIT();
                extrait.REF_CTR_EXTRAIT = bor.REF_CTR_BORD;
                extrait.LIB_EXTRAIT = "FDG - Bordereau N° " + bor.NUM_BORD;
                extrait.DEBIT_EXTRAIT = db.T_DET_BORD.Where(p => p.ANNEE_BORD == bor.ANNEE_BORD && p.NUM_BORD == id && p.REF_CTR_DET_BORD == id2).Sum(p => p.MONT_FDG_DET_BORD);
                extrait.CREDIT_EXTRAIT = 0;
                extrait.AUTRE_EXTRAIT = 0;
                extrait.ENCOURFACT_EXTRAIT = db.All_Ecran_Financements(bor.REF_CTR_BORD).Select(p => p.Encours_Facture).FirstOrDefault();
                extrait.DISPONIBLE_EXTRAIT = db.All_Ecran_Financements(bor.REF_CTR_BORD).Select(p => p.Disponible).FirstOrDefault();
                extrait.DAT_OP_EXTRAIT = DateTime.Now;
                extrait.DAT_VAL_EXTRAIT = bor.DAT_BORD;
                extrait.TOTAL_FIN_EXTRAIT = db.All_Ecran_Financements(bor.REF_CTR_BORD).Select(p => p.Total_Financement).FirstOrDefault();
                db.T_EXTRAIT.Add(extrait);
                db.SaveChanges();

            }
            catch (Exception) { }


            try
            {
                T_HISTORIQUE histo = new T_HISTORIQUE();
                histo.ABREV_ROLE_HIST = "Valider bordereau " + bor.NUM_BORD;
                histo.ACTION = "Valider bordereau";
                histo.ID_ENREGISTREMENT = bor.NUM_BORD.ToString();
                histo.T_TABLE = "T_BORDEREAU";
                histo.REF_CTR_HIST = bor.REF_CTR_BORD.ToString();
                histo.REF_IND_HIST = bor.REF_ADH_BORD.ToString();
                histo.LOGIN_USER = Session["UserLogin"].ToString();
                histo.IP_PC = HttpContext.Request.UserHostAddress;
                histo.NOM_PC = HttpContext.Request.UserHostName;
                histo.DATE_ACTION = DateTime.Now;
                db.T_HISTORIQUE.Add(histo);
                db.SaveChanges();
            }
            catch (Exception) { }

            try
            {
                if (db.All_Ecran_Financements(bor.REF_CTR_BORD).First().Depass_Limite > 0)
                {
                    T_EXTRAIT extrait = new T_EXTRAIT();
                    extrait.REF_CTR_EXTRAIT =  bor.REF_CTR_BORD;
                    extrait.LIB_EXTRAIT = "Depassement ";
                    extrait.DEBIT_EXTRAIT = 0;
                    extrait.CREDIT_EXTRAIT = 0;
                    extrait.AUTRE_EXTRAIT = db.All_Ecran_Financements(bor.REF_CTR_BORD).First().Depass_Limite;
                    extrait.ENCOURFACT_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Encours_Facture).FirstOrDefault();
                    extrait.DISPONIBLE_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Disponible).FirstOrDefault();
                    extrait.DAT_OP_EXTRAIT = DateTime.Now;
                    extrait.DAT_VAL_EXTRAIT = DateTime.Now;
                    extrait.TOTAL_FIN_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Total_Financement).FirstOrDefault();
                    db.T_EXTRAIT.Add(extrait);
                    db.SaveChanges();

                }

            }
            catch (Exception e) {

            }


            try
            {
                string responseContent = null;
                HttpWebRequest request = WebRequest.Create("http://51.210.243.165:59827/api/Authentificate/sendNotification") as HttpWebRequest;
                request.KeepAlive = true;
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";
                var serializer = new JavaScriptSerializer();
                var obj = new
                {
                    loginWeb = db.T_INDIVIDU.Find(db.TJ_CIR.Where(p => p.REF_CTR_CIR == bor.REF_CTR_BORD && p.ID_ROLE_CIR == "ADH").FirstOrDefault().REF_IND_CIR).ABRV_IND,
                    message = "le bordereau n°" + bor.NUM_BORD + " a été valider"
                };
                var param = serializer.Serialize(obj);
                byte[] byteArray = Encoding.UTF8.GetBytes(param);
                try
                {
                    using (var writer = request.GetRequestStream())
                    {
                        writer.Write(byteArray, 0, byteArray.Length);
                    }

                    using (var response = request.GetResponse() as HttpWebResponse)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            responseContent = reader.ReadToEnd();
                        }
                    }
                    System.Diagnostics.Debug.WriteLine(responseContent);
                    //  return true;
                }
                catch (WebException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
                    //  return false;
                }
            }
            catch (Exception e)
            {

            }
            return RedirectToAction("ValiderBordereau");
        }

        public ActionResult RejeterBordereauJson(int id, int id2, int id3)
        {
            int ref_ctr = id2;
            string num_bord = id.ToString();
            if (id < 10) { num_bord ='0' + num_bord; }
            String anne_bord = id3.ToString().Replace(" ", "") ;
            List<T_DET_BORD> detb = new List<T_DET_BORD>();
            detb = db.T_DET_BORD.Where(p => p.REF_CTR_DET_BORD == ref_ctr && p.ANNEE_BORD == anne_bord && p.NUM_BORD == num_bord).ToList();
            T_BORDEREAU bord = new T_BORDEREAU();
            bord = db.T_BORDEREAU.Where(p => p.REF_CTR_BORD == ref_ctr && p.ANNEE_BORD == anne_bord && p.NUM_BORD == num_bord).FirstOrDefault();
            db.T_BORDEREAU.Remove(bord);
            try
            {
                foreach (T_DET_BORD b in detb)
                {
                    if (b != null) { db.T_DET_BORD.Remove(b); }
                }
            }
            catch (Exception) { };
            List<TJ_DOCUMENT_DET_BORD> docdet = new List<TJ_DOCUMENT_DET_BORD>();
            docdet = db.TJ_DOCUMENT_DET_BORD.Where(p => p.NUM_BORD == num_bord && p.REF_CTR_DET_BORD == ref_ctr).ToList();
            try
            {
                foreach (TJ_DOCUMENT_DET_BORD b in docdet)
                {
                    if (b != null) { db.TJ_DOCUMENT_DET_BORD.Remove(b); }
                }
            }
            catch (Exception) { }
            db.SaveChanges();


            try
            {
                T_HISTORIQUE histo = new T_HISTORIQUE();
                histo.ABREV_ROLE_HIST = "Rejeter Bordereau " + num_bord;
                histo.ACTION = "Rejeter bordereau";
                histo.ID_ENREGISTREMENT = num_bord;
                histo.T_TABLE = "T_BORDEREAU";
                histo.REF_CTR_HIST = ref_ctr.ToString();
                histo.REF_IND_HIST = bord.REF_ADH_BORD.ToString();
                histo.LOGIN_USER = Session["UserLogin"].ToString();
                histo.IP_PC = HttpContext.Request.UserHostAddress;
                histo.NOM_PC = HttpContext.Request.UserHostName;
                histo.DATE_ACTION = DateTime.Now;
                db.T_HISTORIQUE.Add(histo);
                db.SaveChanges();
            }
            catch (Exception) { }
            try
            {
                string responseContent = null;
                HttpWebRequest request = WebRequest.Create("http://51.210.243.165:59827/api/Authentificate/sendNotification") as HttpWebRequest;
                request.KeepAlive = true;
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";
                var serializer = new JavaScriptSerializer();
                var obj = new
                {
                    loginWeb = db.T_INDIVIDU.Find(db.TJ_CIR.Where(p => p.REF_CTR_CIR == bord.REF_CTR_BORD && p.ID_ROLE_CIR == "ADH").FirstOrDefault().REF_IND_CIR).ABRV_IND,
                    message = "le bordereau n°" + bord.NUM_BORD + " a été rejeter"
                };
                var param = serializer.Serialize(obj);
                byte[] byteArray = Encoding.UTF8.GetBytes(param);
                try
                {
                    using (var writer = request.GetRequestStream())
                    {
                        writer.Write(byteArray, 0, byteArray.Length);
                    }

                    using (var response = request.GetResponse() as HttpWebResponse)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            responseContent = reader.ReadToEnd();
                        }
                    }
                    System.Diagnostics.Debug.WriteLine(responseContent);
                    //  return true;
                }
                catch (WebException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
                    //  return false;
                }
            }
            catch (Exception e)
            {

            }
            TempData["notice"] = "suppression avec succès";
            return RedirectToAction("ValiderBordereau");
        }
 
        public ActionResult LettrageEtEncaissement()
        {
            if (Session["UserLogin"] != null)
            {
                //var ListAdh = (from q in db.T_INDIVIDU
                //               join q2 in db.TJ_CIR
                //               on q.REF_IND equals q2.REF_IND_CIR
                //               where (q2.ID_ROLE_CIR == "ADH")
                //               select new { q.PRE_IND, q2.REF_IND_CIR }).Distinct();
                //var ListAdh2 = (from q in db.T_INDIVIDU
                //                join q2 in db.TJ_CIR
                //                on q.REF_IND equals q2.REF_IND_CIR
                //                where (q2.ID_ROLE_CIR == "ADH" && q2.REF_CTR_CIR != 28 && q2.REF_CTR_CIR != 32 && q2.REF_CTR_CIR != 38 && q2.REF_CTR_CIR != 54 && q2.REF_CTR_CIR != 56 && q2.REF_CTR_CIR != 58 && q2.REF_CTR_CIR != 65 && q2.REF_CTR_CIR != 69 && q2.REF_CTR_CIR != 72 && q2.REF_CTR_CIR != 74 && q2.REF_CTR_CIR != 80 && q2.REF_CTR_CIR != 82 && q2.REF_CTR_CIR != 93 && q2.REF_CTR_CIR != 96)
                //                select new { q.PRE_IND, q2.REF_CTR_CIR }).Distinct();
                //List<SelectListItem> Adherents = new List<SelectListItem>{
                //    new SelectListItem {Text="***choisir un adhérent***",Value="",Selected=true,Disabled=true }
                //} ;
                //foreach (var item in ListAdh)
                //{
                //    Adherents.Add(new SelectListItem { Text=item.PRE_IND,Value=item.REF_IND_CIR.ToString() });
                //}
                //List<SelectListItem> Adherents2 = new List<SelectListItem>{
                //    new SelectListItem {Text="***choisir un adhérent***",Value="",Selected=true,Disabled=true }
                //};
                //foreach (var item in ListAdh2)
                //{
                //    Adherents2.Add(new SelectListItem { Text = item.PRE_IND+"|"+item.REF_CTR_CIR.ToString(), Value = item.REF_CTR_CIR.ToString() });
                //}
                ViewBag.ADH = db.Recherche_CTR_ADH().GroupBy(p => p.RefAdh).Select(p => p.FirstOrDefault()).ToList();
                ViewBag.ADH2 = db.Recherche_CTR_ADH().ToList();
               
                ViewBag.trop_percu = db.Trop_Percu().ToList();

      
                TempData["LettrageEtEncaissement"] = "active";
                return View();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
        public ActionResult valider_trop_percu(string id,string id2,string id3)
        {
            T_MVT_CREDIT cred = new T_MVT_CREDIT();
            cred.REF_CTR_CREDIT = int.Parse(id);
            cred.ABRV_CREDIT = "6";
            cred.MONT_CREDIT = decimal.Parse(id3.Replace(".",","));
            cred.DATE_CREDIT = DateTime.Now;
            cred.REF_ENC_CREDIT = id2;
            cred.DAT_VAL_ENC_CREDIT = DateTime.Now;
            cred.LIBELLE_LIBRE_CREDIT = "Trop Percu";
            db.T_MVT_CREDIT.Add(cred);
            db.SaveChanges();
            TempData["messagee"] = "opération terminé avec succès";
            TempData["tab4"] = "active";


            try
            {
                T_EXTRAIT extrait = new T_EXTRAIT();
                extrait.REF_CTR_EXTRAIT = cred.REF_CTR_CREDIT;
                extrait.LIB_EXTRAIT = "Trop percu";
                extrait.DEBIT_EXTRAIT = 0;
                extrait.CREDIT_EXTRAIT = cred.MONT_CREDIT;
                extrait.AUTRE_EXTRAIT = 0;
                extrait.ENCOURFACT_EXTRAIT = db.All_Ecran_Financements(cred.REF_CTR_CREDIT).Select(p => p.Encours_Facture).FirstOrDefault();
                extrait.DISPONIBLE_EXTRAIT = db.All_Ecran_Financements(cred.REF_CTR_CREDIT).Select(p => p.Disponible).FirstOrDefault();
                extrait.DAT_OP_EXTRAIT = DateTime.Now;
                extrait.DAT_VAL_EXTRAIT = cred.DAT_VAL_ENC_CREDIT;
                extrait.TOTAL_FIN_EXTRAIT = db.All_Ecran_Financements(cred.REF_CTR_CREDIT).Select(p => p.Total_Financement).FirstOrDefault();
                db.T_EXTRAIT.Add(extrait);
                db.SaveChanges();

            }
            catch (Exception) { }


            try
            {
                T_HISTORIQUE histo = new T_HISTORIQUE();
                histo.ABREV_ROLE_HIST = "Trop percu " + cred.REF_ENC_CREDIT;
                histo.ACTION = "Trop percu";
                histo.ID_ENREGISTREMENT = db.T_MVT_CREDIT.Max().ToString();
                histo.T_TABLE = "T_MVT_CREDIT";
                histo.REF_CTR_HIST = cred.REF_CTR_CREDIT.ToString();
                histo.REF_IND_HIST = db.TJ_CIR.Where(p => p.REF_CTR_CIR == cred.REF_CTR_CREDIT && p.ID_ROLE_CIR == "ADH").FirstOrDefault().REF_IND_CIR.ToString();
                histo.LOGIN_USER = Session["UserLogin"].ToString();
                histo.IP_PC = HttpContext.Request.UserHostAddress;
                histo.NOM_PC = HttpContext.Request.UserHostName;
                histo.DATE_ACTION = DateTime.Now;
                db.T_HISTORIQUE.Add(histo);
                db.SaveChanges();
            }
            catch (Exception) { }


            return RedirectToAction("LettrageEtEncaissement");


        }
        public ActionResult AJouterEnc(T_ENCAISSEMENT enc,string MONT_ENC)

        {
            if (enc.DAT_VAL_ENC.Value.Date >= DateTime.Now.Date)
            {
                //  string query = "select max(Convert(int,)) from T_ENCAISSEMENT";
                List<T_ENCAISSEMENT> enclist = new List<T_ENCAISSEMENT>();
                try { enclist = db.T_ENCAISSEMENT.Where(p => p.REF_ACH_ENC == enc.REF_ACH_ENC && p.REF_ENC == enc.REF_ENC && p.VALIDE_ENC == true).ToList(); } catch (Exception) { }
                if (enclist.Count == 0)
                {
                    try
                    {
                        if (enc.TYP_ENC == "Ret")
                        {
                            enc.TYP_ENC = "A";
                            enc.REF_ENC = "Ret" + enc.REF_ENC;
                        }

                        int ref_max = db.T_ENCAISSEMENT.Max(p => p.ID_ENC);
                        enc.ID_ENC = (short)(ref_max + 1);
                        enc.MONT_ENC = decimal.Parse(MONT_ENC.Replace(".", ","));
                        enc.VALIDE_ENC = true;
                        db.T_ENCAISSEMENT.Add(enc);
                        db.SaveChanges();
                        TempData["message"] = "encaissement  enregistrer" + enc.REF_ENC + " pour l acheteur " + db.T_INDIVIDU.Where(p => p.REF_IND == enc.REF_ACH_ENC).Select(p => p.NOM_IND).SingleOrDefault() + " qui appartient a l adhérent " + db.T_INDIVIDU.Where(p => p.REF_IND == enc.REF_ADH_ENC).Select(p => p.NOM_IND).SingleOrDefault();

                        try
                        {
                            T_EXTRAIT extrait = new T_EXTRAIT();
                            extrait.REF_CTR_EXTRAIT = enc.REF_CTR_ENC;
                            extrait.LIB_EXTRAIT = "Encaissement " + enc.REF_ENC;
                            extrait.DEBIT_EXTRAIT = enc.MONT_ENC * (db.T_FOND_GARANTIE.Where(p => p.REF_CTR_FDG == enc.REF_CTR_ENC && p.TYP_FDG == "F").Select(p => p.TX_FDG).FirstOrDefault() / 100);
                            extrait.CREDIT_EXTRAIT = 0;
                            extrait.AUTRE_EXTRAIT = enc.MONT_ENC;
                            extrait.ENCOURFACT_EXTRAIT = db.All_Ecran_Financements(enc.REF_CTR_ENC).Select(p => p.Encours_Facture).FirstOrDefault();
                            extrait.DISPONIBLE_EXTRAIT = db.All_Ecran_Financements(enc.REF_CTR_ENC).Select(p => p.Disponible).FirstOrDefault();
                            extrait.DAT_OP_EXTRAIT = DateTime.Now;
                            extrait.DAT_VAL_EXTRAIT = enc.DAT_VAL_ENC;
                            extrait.TOTAL_FIN_EXTRAIT = db.All_Ecran_Financements(enc.REF_CTR_ENC).Select(p => p.Total_Financement).FirstOrDefault();
                            db.T_EXTRAIT.Add(extrait);
                            db.SaveChanges();

                        }
                        catch (Exception) { }


                        try
                        {
                            T_HISTORIQUE histo = new T_HISTORIQUE();
                            histo.ABREV_ROLE_HIST = "Creation Encaissement " + enc.REF_ENC;
                            histo.ACTION = "Creation Encaissement";
                            histo.ID_ENREGISTREMENT = enc.ID_ENC.ToString();
                            histo.T_TABLE = "T_ENCAISSEMENT";
                            histo.REF_CTR_HIST = enc.REF_CTR_ENC.ToString();
                            histo.REF_IND_HIST = enc.REF_ACH_ENC.ToString();
                            histo.LOGIN_USER = Session["UserLogin"].ToString();
                            histo.IP_PC = HttpContext.Request.UserHostAddress;
                            histo.NOM_PC = HttpContext.Request.UserHostName;
                            histo.DATE_ACTION = DateTime.Now;
                            db.T_HISTORIQUE.Add(histo);
                            db.SaveChanges();
                        }
                        catch (Exception) { }

                    }
                    catch (Exception) { }
                    try
                    {
                        string query2 = "select max(Convert(int,ID_EC_CPT)) from T_EC_CPT";
                        int ref_max2 = db.Database.SqlQuery<int>(query2).First();

                        T_EC_CPT cp1 = new T_EC_CPT();
                        T_EC_CPT cp2 = new T_EC_CPT();
                        T_DOC_GED ged = new T_DOC_GED();

                        switch (enc.TYP_ENC)
                        {

                            case "C":
                                cp1.ID_EC_CPT = (ref_max2 += 1);
                                cp1.ANNEE_EC_CPT = DateTime.Now.Year;
                                cp1.CODE_DEP_EC_CPT = "A301";
                                cp1.NUM_LIGNE_EC_CPT = 1;
                                cp1.CODE_JOURNAL_EC_CPT = "CR";
                                cp1.DATE_SAISIE_EC_CPT = DateTime.Now;
                                cp1.DATE_EFFET_EC_CPT = enc.DAT_RECEP_ENC;
                                cp1.COMPTE_GEN_EC_CPT = "53200000";
                                cp1.MONTANT_EC_CPT = enc.MONT_ENC;
                                cp1.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                                cp1.REF_PIECE_EC_CPT = enc.REF_ENC;
                                cp1.TYPE_TRANSACTION_EC_CPT = "AR";
                                cp1.TYPE_DOC_EC_CPT = "P";
                                cp1.NOM_USER_EC_CPT = "Admin"; // a changer 
                                cp1.LOT_EC_CPT = enc.REF_ENC;
                                cp1.CODE_TIERS_EC_CPT = enc.REF_ADH_ENC.ToString();
                                cp1.DOMAINE_EC_CPT = "medfact";
                                cp1.DATE_EC_CPT = DateTime.Now;
                                cp1.INTITULE_EC_CPT = "Saisie chèque" + enc.REF_ENC;
                                /***************************************************************************************************/
                                cp2.ID_EC_CPT = (ref_max2 += 2);
                                cp2.ANNEE_EC_CPT = DateTime.Now.Year;
                                cp2.CODE_DEP_EC_CPT = "A301";
                                cp2.NUM_LIGNE_EC_CPT = 2;
                                cp2.CODE_JOURNAL_EC_CPT = "CR";
                                cp2.DATE_SAISIE_EC_CPT = DateTime.Now;
                                cp2.DATE_EFFET_EC_CPT = enc.DAT_RECEP_ENC;
                                cp2.COMPTE_GEN_EC_CPT = "41700000";
                                cp2.MONTANT_EC_CPT = 0 - enc.MONT_ENC;
                                cp2.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                                cp2.REF_PIECE_EC_CPT = enc.REF_ENC;
                                cp2.TYPE_TRANSACTION_EC_CPT = "AR";
                                cp2.TYPE_DOC_EC_CPT = "P";
                                cp2.NOM_USER_EC_CPT = "Admin"; // a changer 
                                cp2.LOT_EC_CPT = enc.REF_ENC;
                                cp2.CODE_TIERS_EC_CPT = enc.REF_ADH_ENC.ToString();
                                cp2.DOMAINE_EC_CPT = "medfact";
                                cp2.DATE_EC_CPT = DateTime.Now;
                                cp2.INTITULE_EC_CPT = enc.REF_ENC + "   " + enc.REF_ENC;
                                /****************************************************GED****************************************/
                                ged.ID_IND_GED = enc.REF_ADH_ENC;
                                ged.ID_CTR_GED = enc.REF_CTR_ENC;
                                ged.ID_ENC_GED = enc.ID_ENC;
                                ged.LIBELLE_GED = "Encaissement Cheque";
                                ged.DATE_TACHE_GED = DateTime.Now;
                                ged.ID_GESTIONNAIRE_GED = int.Parse(Session["ID_USER"].ToString());
                                ged.LIBELLE2_GED = enc.REF_ENC;
                                ged.ID_Emetteur_GED = Session["ID_USER"].ToString();
                                ged.Etape_ged = "Création";
                                ged.Etat_GED = false;


                                break;
                            /**********************************************************************************/
                            case "T":
                                cp1.ID_EC_CPT = (ref_max2 += 1);
                                cp1.ANNEE_EC_CPT = DateTime.Now.Year;
                                cp1.CODE_DEP_EC_CPT = "A301";
                                cp1.NUM_LIGNE_EC_CPT = 1;
                                cp1.CODE_JOURNAL_EC_CPT = "AR";
                                cp1.DATE_SAISIE_EC_CPT = DateTime.Now;
                                cp1.DATE_EFFET_EC_CPT = enc.DAT_RECEP_ENC;
                                cp1.COMPTE_GEN_EC_CPT = "41301000";
                                cp1.MONTANT_EC_CPT = enc.MONT_ENC;
                                cp1.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                                cp1.REF_PIECE_EC_CPT = enc.REF_ENC;
                                cp1.TYPE_TRANSACTION_EC_CPT = "AR";
                                cp1.TYPE_DOC_EC_CPT = "D";
                                cp1.NOM_USER_EC_CPT = "Admin"; // a changer 
                                cp1.LOT_EC_CPT = enc.REF_ENC;
                                cp1.CODE_TIERS_EC_CPT = enc.REF_ADH_ENC.ToString();
                                cp1.DOMAINE_EC_CPT = "medfact";
                                cp1.DATE_EC_CPT = DateTime.Now;
                                cp1.INTITULE_EC_CPT = "Saisie traite";
                                /***************************************************************************************************/
                                cp2.ID_EC_CPT = (ref_max2 += 2);
                                cp2.ANNEE_EC_CPT = DateTime.Now.Year;
                                cp2.CODE_DEP_EC_CPT = "A301";
                                cp2.NUM_LIGNE_EC_CPT = 2;
                                cp2.CODE_JOURNAL_EC_CPT = "CR";
                                cp2.DATE_SAISIE_EC_CPT = DateTime.Now;
                                cp2.DATE_EFFET_EC_CPT = enc.DAT_RECEP_ENC;
                                cp2.COMPTE_GEN_EC_CPT = "41700000";
                                cp2.MONTANT_EC_CPT = 0 - enc.MONT_ENC;
                                cp2.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                                cp2.REF_PIECE_EC_CPT = enc.REF_ENC;
                                cp2.TYPE_TRANSACTION_EC_CPT = "AR";
                                cp2.TYPE_DOC_EC_CPT = "P";
                                cp2.NOM_USER_EC_CPT = "Admin"; // a changer 
                                cp2.LOT_EC_CPT = enc.REF_ENC;
                                cp2.CODE_TIERS_EC_CPT = enc.REF_ADH_ENC.ToString();
                                cp2.DOMAINE_EC_CPT = "medfact";
                                cp2.DATE_EC_CPT = DateTime.Now;
                                cp2.INTITULE_EC_CPT = enc.REF_ENC + "   " + enc.REF_ENC;
                                /****************************************************GED****************************************/
                                if (enc.REF_ENC.Length == 7)
                                {
                                    ged.ID_IND_GED = enc.REF_ADH_ENC;
                                    ged.ID_CTR_GED = enc.REF_CTR_ENC;
                                    ged.ID_ENC_GED = enc.ID_ENC;
                                    ged.LIBELLE_GED = "Encaissement Chéque";
                                    ged.DATE_TACHE_GED = DateTime.Now;
                                    ged.ID_GESTIONNAIRE_GED = int.Parse(Session["ID_USER"].ToString()); ;
                                    ged.LIBELLE2_GED = enc.REF_ENC;
                                    ged.ID_Emetteur_GED = Session["ID_USER"].ToString(); ;
                                    ged.Etape_ged = "Création";
                                    ged.Etat_GED = false;
                                }
                                else
                                {
                                    ged.ID_IND_GED = enc.REF_ADH_ENC;
                                    ged.ID_CTR_GED = enc.REF_CTR_ENC;
                                    ged.ID_ENC_GED = enc.ID_ENC;
                                    ged.LIBELLE_GED = "Encaissement Traite";
                                    ged.DATE_TACHE_GED = DateTime.Now;
                                    ged.ID_GESTIONNAIRE_GED = int.Parse(Session["ID_USER"].ToString()); ;
                                    ged.LIBELLE2_GED = enc.REF_ENC;
                                    ged.ID_Emetteur_GED = Session["ID_USER"].ToString(); ;
                                    ged.Etape_ged = "Création";
                                    ged.Etat_GED = false;
                                }


                                break;
                            /************************************************************************************************************/
                            case "A":
                                cp1.ID_EC_CPT = (ref_max2 += 1);
                                cp1.ANNEE_EC_CPT = DateTime.Now.Year;
                                cp1.CODE_DEP_EC_CPT = "A301";
                                cp1.NUM_LIGNE_EC_CPT = 1;
                                cp1.CODE_JOURNAL_EC_CPT = "ME";
                                cp1.DATE_SAISIE_EC_CPT = DateTime.Now;
                                cp1.DATE_EFFET_EC_CPT = enc.DAT_RECEP_ENC;
                                cp1.COMPTE_GEN_EC_CPT = "40900000";
                                cp1.MONTANT_EC_CPT = enc.MONT_ENC;
                                cp1.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                                cp1.REF_PIECE_EC_CPT = enc.REF_ENC;
                                cp1.TYPE_TRANSACTION_EC_CPT = "AR";
                                cp1.TYPE_DOC_EC_CPT = "M";
                                cp1.NOM_USER_EC_CPT = "Admin"; // a changer 
                                cp1.LOT_EC_CPT = enc.REF_ENC;
                                cp1.CODE_TIERS_EC_CPT = enc.REF_ADH_ENC.ToString();
                                cp1.DOMAINE_EC_CPT = "medfact";
                                cp1.DATE_EC_CPT = DateTime.Now;
                                cp1.INTITULE_EC_CPT = "Saisie  avoir";
                                /***************************************************************************************************/
                                cp2.ID_EC_CPT = (ref_max2 += 2);
                                cp2.ANNEE_EC_CPT = DateTime.Now.Year;
                                cp2.CODE_DEP_EC_CPT = "A301";
                                cp2.NUM_LIGNE_EC_CPT = 2;
                                cp2.CODE_JOURNAL_EC_CPT = "ME";
                                cp2.DATE_SAISIE_EC_CPT = DateTime.Now;
                                cp2.DATE_EFFET_EC_CPT = enc.DAT_RECEP_ENC;
                                cp2.COMPTE_GEN_EC_CPT = "41700000";
                                cp2.MONTANT_EC_CPT = 0 - enc.MONT_ENC;
                                cp2.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                                cp2.REF_PIECE_EC_CPT = enc.REF_ENC;
                                cp2.TYPE_TRANSACTION_EC_CPT = "AR";
                                cp2.TYPE_DOC_EC_CPT = "M";
                                cp2.NOM_USER_EC_CPT = "Admin"; // a changer 
                                cp2.LOT_EC_CPT = enc.REF_ENC;
                                cp2.CODE_TIERS_EC_CPT = enc.REF_ADH_ENC.ToString();
                                cp2.DOMAINE_EC_CPT = "medfact";
                                cp2.DATE_EC_CPT = DateTime.Now;
                                cp2.INTITULE_EC_CPT = "saisie avoir débit";
                                /****************************************************GED****************************************/
                                ged.ID_IND_GED = enc.REF_ADH_ENC;
                                ged.ID_CTR_GED = enc.REF_CTR_ENC;
                                ged.ID_ENC_GED = enc.ID_ENC;
                                ged.LIBELLE_GED = "Avoir";
                                ged.DATE_TACHE_GED = DateTime.Now;
                                ged.ID_GESTIONNAIRE_GED = int.Parse(Session["ID_USER"].ToString()); ;
                                ged.LIBELLE2_GED = enc.REF_ENC;
                                ged.ID_Emetteur_GED = Session["ID_USER"].ToString(); ;
                                ged.Etape_ged = "Création";
                                ged.Etat_GED = false;


                                break;
                            /*****************************************************************************************************/

                            case "E":
                                cp1.ID_EC_CPT = (ref_max2 += 1);
                                cp1.ANNEE_EC_CPT = DateTime.Now.Year;
                                cp1.CODE_DEP_EC_CPT = "A301";
                                cp1.NUM_LIGNE_EC_CPT = 1;
                                cp1.CODE_JOURNAL_EC_CPT = "CR";
                                cp1.DATE_SAISIE_EC_CPT = DateTime.Now;
                                cp1.DATE_EFFET_EC_CPT = enc.DAT_RECEP_ENC;
                                cp1.COMPTE_GEN_EC_CPT = "54000000";
                                cp1.MONTANT_EC_CPT = enc.MONT_ENC;
                                cp1.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                                cp1.REF_PIECE_EC_CPT = enc.REF_ENC;
                                cp1.TYPE_TRANSACTION_EC_CPT = "AR";
                                cp1.TYPE_DOC_EC_CPT = "P";
                                cp1.NOM_USER_EC_CPT = "Admin"; // a changer 
                                cp1.LOT_EC_CPT = enc.REF_ENC;
                                cp1.CODE_TIERS_EC_CPT = enc.REF_ADH_ENC.ToString();
                                cp1.DOMAINE_EC_CPT = "medfact";
                                cp1.DATE_EC_CPT = DateTime.Now;
                                cp1.INTITULE_EC_CPT = "Saisie  espèce";
                                /***************************************************************************************************/
                                cp2.ID_EC_CPT = (ref_max2 += 2);
                                cp2.ANNEE_EC_CPT = DateTime.Now.Year;
                                cp2.CODE_DEP_EC_CPT = "A301";
                                cp2.NUM_LIGNE_EC_CPT = 2;
                                cp2.CODE_JOURNAL_EC_CPT = "CR";
                                cp2.DATE_SAISIE_EC_CPT = DateTime.Now;
                                cp2.DATE_EFFET_EC_CPT = enc.DAT_RECEP_ENC;
                                cp2.COMPTE_GEN_EC_CPT = "41700000";
                                cp2.MONTANT_EC_CPT = 0 - enc.MONT_ENC;
                                cp2.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                                cp2.REF_PIECE_EC_CPT = enc.REF_ENC;
                                cp2.TYPE_TRANSACTION_EC_CPT = "AR";
                                cp2.TYPE_DOC_EC_CPT = "P";
                                cp2.NOM_USER_EC_CPT = "Admin"; // a changer 
                                cp2.LOT_EC_CPT = enc.REF_ENC;
                                cp2.CODE_TIERS_EC_CPT = enc.REF_ADH_ENC.ToString();
                                cp2.DOMAINE_EC_CPT = "medfact";
                                cp2.DATE_EC_CPT = DateTime.Now;
                                cp2.INTITULE_EC_CPT = "saisie espèce débit";
                                /****************************************************GED****************************************/
                                ged.ID_IND_GED = enc.REF_ADH_ENC;
                                ged.ID_CTR_GED = enc.REF_CTR_ENC;
                                ged.ID_ENC_GED = enc.ID_ENC;
                                ged.LIBELLE_GED = "Encaissement Espece";
                                ged.DATE_TACHE_GED = DateTime.Now;
                                ged.ID_GESTIONNAIRE_GED = int.Parse(Session["ID_USER"].ToString()); ;
                                ged.LIBELLE2_GED = enc.REF_ENC;
                                ged.ID_Emetteur_GED = Session["ID_USER"].ToString(); ;
                                ged.Etape_ged = "Création";
                                ged.Etat_GED = false;

                                break;

                            case "B":
                                cp1.ID_EC_CPT = (ref_max2 += 1);
                                cp1.ANNEE_EC_CPT = DateTime.Now.Year;
                                cp1.CODE_DEP_EC_CPT = "A301";
                                cp1.NUM_LIGNE_EC_CPT = 1;
                                cp1.CODE_JOURNAL_EC_CPT = "CR";
                                cp1.DATE_SAISIE_EC_CPT = DateTime.Now;
                                cp1.DATE_EFFET_EC_CPT = enc.DAT_RECEP_ENC;
                                cp1.COMPTE_GEN_EC_CPT = "41700000";
                                cp1.MONTANT_EC_CPT = enc.MONT_ENC;
                                cp1.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                                cp1.REF_PIECE_EC_CPT = enc.REF_ENC;
                                cp1.TYPE_TRANSACTION_EC_CPT = "AR";
                                cp1.TYPE_DOC_EC_CPT = "P";
                                cp1.NOM_USER_EC_CPT = "Admin"; // a changer 
                                cp1.LOT_EC_CPT = enc.REF_ENC;
                                cp1.CODE_TIERS_EC_CPT = enc.REF_ADH_ENC.ToString();
                                cp1.DOMAINE_EC_CPT = "medfact";
                                cp1.DATE_EC_CPT = DateTime.Now;
                                cp1.INTITULE_EC_CPT = "saisie bon de commande";
                                /***************************************************************************************************/
                                cp2.ID_EC_CPT = (ref_max2 += 2);
                                cp2.ANNEE_EC_CPT = DateTime.Now.Year;
                                cp2.CODE_DEP_EC_CPT = "A301";
                                cp2.NUM_LIGNE_EC_CPT = 2;
                                cp2.CODE_JOURNAL_EC_CPT = "CR";
                                cp2.DATE_SAISIE_EC_CPT = DateTime.Now;
                                cp2.DATE_EFFET_EC_CPT = enc.DAT_RECEP_ENC;
                                cp2.COMPTE_GEN_EC_CPT = "41300000";
                                cp2.MONTANT_EC_CPT = 0 - enc.MONT_ENC;
                                cp2.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                                cp2.REF_PIECE_EC_CPT = enc.REF_ENC;
                                cp2.TYPE_TRANSACTION_EC_CPT = "AR";
                                cp2.TYPE_DOC_EC_CPT = "P";
                                cp2.NOM_USER_EC_CPT = "Admin"; // a changer 
                                cp2.LOT_EC_CPT = enc.REF_ENC;
                                cp2.CODE_TIERS_EC_CPT = enc.REF_ADH_ENC.ToString();
                                cp2.DOMAINE_EC_CPT = "medfact";
                                cp2.DATE_EC_CPT = DateTime.Now;
                                cp2.INTITULE_EC_CPT = "saisie bon de commande";
                                /****************************************************GED****************************************/
                                ged.ID_IND_GED = enc.REF_ADH_ENC;
                                ged.ID_CTR_GED = enc.REF_CTR_ENC;
                                ged.ID_ENC_GED = enc.ID_ENC;
                                ged.LIBELLE_GED = "Encaissement billet à ordre";
                                ged.DATE_TACHE_GED = DateTime.Now;
                                ged.ID_GESTIONNAIRE_GED = int.Parse(Session["ID_USER"].ToString()); ;
                                ged.LIBELLE2_GED = enc.REF_ENC;
                                ged.ID_Emetteur_GED = Session["ID_USER"].ToString(); ;
                                ged.Etape_ged = "Création";
                                ged.Etat_GED = false;


                                break;

                            case "V":
                                cp1.ID_EC_CPT = (ref_max2 += 1);
                                cp1.ANNEE_EC_CPT = DateTime.Now.Year;
                                cp1.CODE_DEP_EC_CPT = "A301";
                                cp1.NUM_LIGNE_EC_CPT = 1;
                                cp1.CODE_JOURNAL_EC_CPT = "10";
                                cp1.DATE_SAISIE_EC_CPT = DateTime.Now;
                                cp1.DATE_EFFET_EC_CPT = enc.DAT_RECEP_ENC;
                                cp1.COMPTE_GEN_EC_CPT = "53201000";
                                cp1.MONTANT_EC_CPT = enc.MONT_ENC;
                                cp1.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                                cp1.REF_PIECE_EC_CPT = enc.REF_ENC;
                                cp1.TYPE_TRANSACTION_EC_CPT = "AR";
                                cp1.TYPE_DOC_EC_CPT = "P";
                                cp1.NOM_USER_EC_CPT = Session["UserLogin"].ToString();  // a changer 
                                cp1.LOT_EC_CPT = enc.REF_ENC;
                                cp1.CODE_TIERS_EC_CPT = enc.REF_ADH_ENC.ToString();
                                cp1.DOMAINE_EC_CPT = "medfact";
                                cp1.DATE_EC_CPT = DateTime.Now;
                                cp1.INTITULE_EC_CPT = "Saisie  virement";
                                /***************************************************************************************************/
                                cp2.ID_EC_CPT = (ref_max2 += 2);
                                cp2.ANNEE_EC_CPT = DateTime.Now.Year;
                                cp2.CODE_DEP_EC_CPT = "A301";
                                cp2.NUM_LIGNE_EC_CPT = 2;
                                cp2.CODE_JOURNAL_EC_CPT = "11";
                                cp2.DATE_SAISIE_EC_CPT = DateTime.Now;
                                cp2.DATE_EFFET_EC_CPT = enc.DAT_RECEP_ENC;
                                cp2.COMPTE_GEN_EC_CPT = "41700000";
                                cp2.MONTANT_EC_CPT = 0 - enc.MONT_ENC;
                                cp2.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                                cp2.REF_PIECE_EC_CPT = enc.REF_ENC;
                                cp2.TYPE_TRANSACTION_EC_CPT = "AR";
                                cp2.TYPE_DOC_EC_CPT = "P";
                                cp2.NOM_USER_EC_CPT = "Admin"; // a changer 
                                cp2.LOT_EC_CPT = enc.REF_ENC;
                                cp2.CODE_TIERS_EC_CPT = enc.REF_ADH_ENC.ToString();
                                cp2.DOMAINE_EC_CPT = "medfact";
                                cp2.DATE_EC_CPT = DateTime.Now;
                                cp2.INTITULE_EC_CPT = "saisie virement débit";
                                /****************************************************GED****************************************/
                                ged.ID_IND_GED = enc.REF_ADH_ENC;
                                ged.ID_CTR_GED = enc.REF_CTR_ENC;
                                ged.ID_ENC_GED = enc.ID_ENC;
                                ged.LIBELLE_GED = "Encaissement Virement";
                                ged.DATE_TACHE_GED = DateTime.Now;
                                ged.ID_GESTIONNAIRE_GED = int.Parse(Session["ID_USER"].ToString()); ;
                                ged.LIBELLE2_GED = enc.REF_ENC;
                                ged.ID_Emetteur_GED = Session["ID_USER"].ToString(); ;
                                ged.Etape_ged = "Création";
                                ged.Etat_GED = false;


                                break;
                        }
                        // ajoute t historique 
                        db.T_EC_CPT.Add(cp1);
                        db.T_EC_CPT.Add(cp2);
                        db.T_DOC_GED.Add(ged);
                        db.SaveChanges();

                    }
                    catch (Exception e)
                    {

                        TempData["errorBord"] = e.Message;
                        return Json("error", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    TempData["errorBord"] = "l encaissement n°  " + enc.REF_ENC + "   est deja sauvgarder";
                    var x = new
                    {
                        type = "error",
                        message = TempData["errorBord"]
                    };
                    return Json(x, JsonRequestBehavior.AllowGet);
                }
                // return RedirectToAction("InternalServerError", "Error");
                var y = new
                {
                    type = "save",
                    message = TempData["message"]
                };
                return Json(y, JsonRequestBehavior.AllowGet);
            }
            else
            {

                TempData["errorBord"] = "Merci de verifier date valeur d l encaissement";
                var x = new
                {
                    type = "error_date_valeur",
                    message = TempData["errorBord"]
                };
                return Json(x, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult CreditDebit()

        {
            try
            {
                ViewBag.List = new SelectList(db.TR_LIST_VAL.Where(p => p.TYP_LIST_VAL == "Typ_Credit_Debit"), "ABR_LIST_VAL", "LIB_LIST_VAL");
             //   var ListAdh = (from q in db.T_INDIVIDU
             //                  join q2 in db.TJ_CIR
             //                  on q.REF_IND equals q2.REF_IND_CIR
             //                  where (q2.ID_ROLE_CIR == "ADH" && q2.REF_CTR_CIR != 28LeF && q2.REF_CTR_CIR != 32 && q2.REF_CTR_CIR != 38 && q2.REF_CTR_CIR != 54 && q2.REF_CTR_CIR != 56 && q2.REF_CTR_CIR != 58 && q2.REF_CTR_CIR != 65 && q2.REF_CTR_CIR != 69 && q2.REF_CTR_CIR != 72 && q2.REF_CTR_CIR != 74 && q2.REF_CTR_CIR != 80 && q2.REF_CTR_CIR != 82 && q2.REF_CTR_CIR != 93 && q2.REF_CTR_CIR != 96)
             //                  select new { q.PRE_IND, q2.REF_CTR_CIR });
             ////  = new SelectList(ListAdh, "REF_CTR_CIR", "PRE_IND");
             //   List<SelectListItem> Adherents = new List<SelectListItem>{
             //       new SelectListItem {Text="***choisir un adhérent***",Value="",Selected=true,Disabled=true }
             //   };
             //   foreach (var item in ListAdh)
             //   {
             //       Adherents.Add(new SelectListItem { Text = item.PRE_IND + "|" + item.REF_CTR_CIR.ToString(), Value = item.REF_CTR_CIR.ToString() });
             //   }
                ViewBag.ADH = db.Recherche_CTR_ADH().ToList();
               TempData["CreditDebit"] = "active";
            
            }
            catch (Exception) { return RedirectToAction("InternalServerError", "Error"); }
            return View();
        }
        [HttpPost]
        public ActionResult CreditDebit(int REF_CTR,string MNTCR,string MNTDB, string ABEV_DEBIT,string REF_SEQ,string libelleDebitCredit,string ref_enc_cred,DateTime dat_val_enc_cred)
        {
            try
            {
                decimal montantCredit = decimal.Parse(MNTCR.Replace(".", ","));
                decimal montantDebit = decimal.Parse(MNTDB.Replace(".", ","));

                T_MVT_DEBIT debit = new T_MVT_DEBIT();
                debit.ID_DEBIT = db.T_MVT_DEBIT.Select(p => p.ID_DEBIT).Max() + 1;
                debit.REF_CTR_DEBIT = REF_CTR;
                debit.ABEV_DEBIT = ABEV_DEBIT;
                debit.MONT_DEBIT = montantDebit;
                debit.DATE_DEBIT = DateTime.Now;
                db.T_MVT_DEBIT.Add(debit);

                T_MVT_CREDIT credit = new T_MVT_CREDIT();
                credit.ID_CREDIT = db.T_MVT_CREDIT.Select(p => p.ID_CREDIT).Max() + 1;
                credit.REF_CTR_CREDIT = REF_CTR;
                credit.ABRV_CREDIT = ABEV_DEBIT;
                credit.MONT_CREDIT = montantCredit;
                credit.DATE_CREDIT = DateTime.Now;
                credit.REF_ENC_CREDIT = ref_enc_cred;
                credit.DAT_VAL_ENC_CREDIT = dat_val_enc_cred;
                credit.LIBELLE_LIBRE_CREDIT = libelleDebitCredit;
                db.T_MVT_CREDIT.Add(credit);
                db.SaveChanges();


                if (debit.MONT_DEBIT != 0)
                {

                    try
                    {
                        T_EXTRAIT extrait = new T_EXTRAIT();
                        extrait.REF_CTR_EXTRAIT = debit.REF_CTR_DEBIT;
                        extrait.LIB_EXTRAIT = " Debit ";
                        extrait.DEBIT_EXTRAIT = debit.MONT_DEBIT;
                        extrait.CREDIT_EXTRAIT = 0;
                        extrait.AUTRE_EXTRAIT = 0;
                        extrait.ENCOURFACT_EXTRAIT = db.All_Ecran_Financements(debit.REF_CTR_DEBIT).Select(p => p.Encours_Facture).FirstOrDefault();
                        extrait.DISPONIBLE_EXTRAIT = db.All_Ecran_Financements(debit.REF_CTR_DEBIT).Select(p => p.Disponible).FirstOrDefault();
                        extrait.DAT_OP_EXTRAIT = DateTime.Now;
                        extrait.DAT_VAL_EXTRAIT = debit.DATE_DEBIT;
                        extrait.TOTAL_FIN_EXTRAIT = db.All_Ecran_Financements(debit.REF_CTR_DEBIT).Select(p => p.Total_Financement).FirstOrDefault();
                        db.T_EXTRAIT.Add(extrait);
                        db.SaveChanges();

                    }
                    catch (Exception) { }

                    try
                    {
                        T_HISTORIQUE histo = new T_HISTORIQUE();
                        histo.ABREV_ROLE_HIST = "Debit ";
                        histo.ACTION = "Creation Debit";
                        histo.ID_ENREGISTREMENT = db.T_MVT_DEBIT.Max().ToString();
                        histo.T_TABLE = "T_MVT_DEBIT";
                        histo.REF_CTR_HIST = debit.REF_CTR_DEBIT.ToString();
                        histo.REF_IND_HIST = db.TJ_CIR.Where(p=>p.REF_CTR_CIR== debit.REF_CTR_DEBIT && p.ID_ROLE_CIR=="ADH").FirstOrDefault().REF_IND_CIR.ToString();
                        histo.LOGIN_USER = Session["UserLogin"].ToString();
                        histo.IP_PC = HttpContext.Request.UserHostAddress;
                        histo.NOM_PC = HttpContext.Request.UserHostName;
                        histo.DATE_ACTION = DateTime.Now;
                        db.T_HISTORIQUE.Add(histo);
                        db.SaveChanges();
                    }
                    catch (Exception) { }

                }

                if (credit.MONT_CREDIT != 0)
                {
                    try
                    {
                        T_EXTRAIT extrait = new T_EXTRAIT();
                        extrait.REF_CTR_EXTRAIT = credit.REF_CTR_CREDIT;
                        extrait.LIB_EXTRAIT = "Credit " + credit.LIBELLE_LIBRE_CREDIT;
                        extrait.DEBIT_EXTRAIT = 0;
                        extrait.CREDIT_EXTRAIT = credit.MONT_CREDIT;
                        extrait.AUTRE_EXTRAIT = 0;
                        extrait.ENCOURFACT_EXTRAIT = db.All_Ecran_Financements(debit.REF_CTR_DEBIT).Select(p => p.Encours_Facture).FirstOrDefault();
                        extrait.DISPONIBLE_EXTRAIT = db.All_Ecran_Financements(debit.REF_CTR_DEBIT).Select(p => p.Disponible).FirstOrDefault();
                        extrait.DAT_OP_EXTRAIT = DateTime.Now;
                        extrait.DAT_VAL_EXTRAIT = credit.DAT_VAL_ENC_CREDIT;
                        extrait.TOTAL_FIN_EXTRAIT = db.All_Ecran_Financements(debit.REF_CTR_DEBIT).Select(p => p.Total_Financement).FirstOrDefault();
                        db.T_EXTRAIT.Add(extrait);
                        db.SaveChanges();

                    }
                    catch (Exception) { }


                    try
                    {
                        T_HISTORIQUE histo = new T_HISTORIQUE();
                        histo.ABREV_ROLE_HIST = "Credit " + credit.LIBELLE_LIBRE_CREDIT;
                        histo.ACTION = "Creation Credit";
                        histo.ID_ENREGISTREMENT = db.T_MVT_CREDIT.Max().ToString();
                        histo.T_TABLE = "T_MVT_CREDIT";
                        histo.REF_CTR_HIST = credit.REF_CTR_CREDIT.ToString();
                        histo.REF_IND_HIST = db.TJ_CIR.Where(p => p.REF_CTR_CIR == credit.REF_CTR_CREDIT && p.ID_ROLE_CIR == "ADH").FirstOrDefault().REF_IND_CIR.ToString();
                        histo.LOGIN_USER = Session["UserLogin"].ToString();
                        histo.IP_PC = HttpContext.Request.UserHostAddress;
                        histo.NOM_PC = HttpContext.Request.UserHostName;
                        histo.DATE_ACTION = DateTime.Now;
                        db.T_HISTORIQUE.Add(histo);
                        db.SaveChanges();
                    }
                    catch (Exception) { }


                }




                int ref_ind_adh;
                try { ref_ind_adh = db.TJ_CIR.Where(k => k.REF_CTR_CIR == REF_CTR && k.ID_ROLE_CIR == "ADH").Select(k => k.REF_IND_CIR).SingleOrDefault(); } catch (Exception) { ref_ind_adh = 0; }
              // = db.TJ_CIR.Where(k => k.REF_CTR_CIR == REF_CTR && k.ID_ROLE_CIR == "ADH").Select(k => k.REF_IND_CIR).SingleOrDefault();
                db.Database.ExecuteSqlCommand("insert into T_DOC_GED (ID_CTR_GED,ID_IND_GED,ID_DEBIT_GED,LIBELLE_GED,DATE_TACHE_GED,LIBELLE2_GED,ID_Emetteur_GED,Etape_ged,Etat_GED) values({0},{1},{2},{3},{4},{5},{6},{7},{8})", REF_CTR, ref_ind_adh, db.T_MVT_DEBIT.Select(p => p.ID_DEBIT).Max() , "Debit", DateTime.Now, ABEV_DEBIT, Session["ID_USER"].ToString(), "Création",false);

                T_DOC_GED ged2 = new T_DOC_GED();
                ged2.ID_CTR_GED = REF_CTR;
                ged2.ID_CREDIT_GED = db.T_MVT_CREDIT.Select(p => p.ID_CREDIT).Max() + 1;
                ged2.LIBELLE_GED = "Credit : " + libelleDebitCredit;
                ged2.DATE_TACHE_GED = DateTime.Now;
                ged2.LIBELLE2_GED = (db.T_MVT_CREDIT.Select(p => p.ID_CREDIT).Max() + 1).ToString();
                ged2.Etape_ged = "Création";
                ged2.Etat_GED = false;
                db.T_DOC_GED.Add(ged2);
                db.Database.ExecuteSqlCommand("insert into T_DOC_GED (ID_CTR_GED,ID_IND_GED,ID_DEBIT_GED,LIBELLE_GED,DATE_TACHE_GED,LIBELLE2_GED,ID_Emetteur_GED,Etape_ged,Etat_GED) values({0},{1},{2},{3},{4},{5},{6},{7},{8})", REF_CTR, ref_ind_adh, db.T_MVT_CREDIT.Select(p => p.ID_CREDIT).Max(), "Credit : " + libelleDebitCredit, DateTime.Now, (db.T_MVT_CREDIT.Select(p => p.ID_CREDIT).Max()).ToString(), Session["ID_USER"].ToString(), "Création", false);
                TempData["notice"] = "valider";
            }
            catch (Exception) {
                return RedirectToAction("InternalServerError", "Error"); }
            return RedirectToAction("CreditDebit");
        }

        public ActionResult SaisiImpaye()
        {
            if (Session["UserLogin"] != null)
            {
                //var ListAdh = (from q in db.T_INDIVIDU
                //               join q2 in db.TJ_CIR
                //               on q.REF_IND equals q2.REF_IND_CIR
                //               where (q2.ID_ROLE_CIR == "ADH" && q2.REF_CTR_CIR != 28 && q2.REF_CTR_CIR != 32 && q2.REF_CTR_CIR != 38 && q2.REF_CTR_CIR != 54 && q2.REF_CTR_CIR != 56 && q2.REF_CTR_CIR != 58 && q2.REF_CTR_CIR != 65 && q2.REF_CTR_CIR != 69 && q2.REF_CTR_CIR != 72 && q2.REF_CTR_CIR != 74 && q2.REF_CTR_CIR != 80 && q2.REF_CTR_CIR != 82 && q2.REF_CTR_CIR != 93 && q2.REF_CTR_CIR != 96)
                //               select new { q.PRE_IND, q2.REF_CTR_CIR });
                //ViewBag.ADH = new SelectList(ListAdh, "REF_CTR_CIR", "PRE_IND");

                //List<NewDataCollection> op = new List<NewDataCollection>();
                //foreach (var data in ListAdh)
                //{
                //    op.Add(new NewDataCollection(data.REF_CTR_CIR, data.PRE_IND));
                //}

                ViewBag.ListNomInd = db.Recherche_CTR_ADH().ToList();

                TempData["Impaye"] = "active";
                TempData["SaisiImpaye"] = "active";
                return View();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
        [HttpPost]
        public ActionResult SaisiImpaye(T_IMPAYE imp,string MONT_IMP)
        {
            TR_TVA tva ;
            try
            {
                tva = db.TR_TVA.OrderByDescending(p => p.ID_TVA).SingleOrDefault();
            }
            catch (Exception e)
            {
                tva = null;
            }
            string test = db.T_ENCAISSEMENT.Where(p => p.ID_ENC == imp.ID_ENC_IMP).Select(p => p.TYP_ENC).SingleOrDefault();
            T_ENCAISSEMENT enc = db.T_ENCAISSEMENT.Where(p => p.ID_ENC == imp.ID_ENC_IMP).SingleOrDefault();
            int ref_ctr = db.T_ENCAISSEMENT.Where(p => p.ID_ENC == imp.ID_ENC_IMP).Select(p => p.REF_CTR_ENC).SingleOrDefault();
            if(test.Trim()!="A" && test.Trim()!="V")
            {
                try
                {
                    imp.ID_IMP = db.T_IMPAYE.Max(p => p.ID_IMP) + 1;
                    imp.DATE_SAISI_IMP = DateTime.Now;
                    imp.MONT_IMP = decimal.Parse(MONT_IMP.Replace(" ", ""));
                    db.T_IMPAYE.Add(imp);
                    db.SaveChanges();
                    List<TJ_LETTRAGE> let = db.TJ_LETTRAGE.Where(p => p.ID_ENC_LET == imp.ID_ENC_IMP).ToList();
                    foreach (var item in let)
                    {
                        item.VALIDE_RECONCIL = false;
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    db.Anuuler_rec_t_det_bord(imp.ID_ENC_IMP);
                    decimal diver;
                    
                    try
                    {
                        diver = (decimal)db.T_FRAIS_DIVERS.Where(p => p.REF_CTR_FRAIS_DIVERS == ref_ctr && p.TYP_FRAIS_DIVERS.Contains("Imp")).Select(p => p.MONT_PAR_UNIT_FRAIS_DIVERS).SingleOrDefault();
                        T_MVT_DEBIT deb = new T_MVT_DEBIT();
                        deb.REF_CTR_DEBIT = ref_ctr;
                        deb.ABEV_DEBIT = "Impaye";
                        if(tva != null)
                        {
                            deb.MONT_DEBIT = diver * tva.VAL_TVA;
                        }
                        else
                        {
                            deb.MONT_DEBIT = diver;
                        }
                      
                        deb.DATE_DEBIT = DateTime.Now;
                        db.T_MVT_DEBIT.Add(deb);
                        db.SaveChanges();


                        try
                        {
                            T_EXTRAIT extrait = new T_EXTRAIT();
                            extrait.REF_CTR_EXTRAIT = deb.REF_CTR_DEBIT;
                            extrait.LIB_EXTRAIT = "Debit " + deb.ABEV_DEBIT;
                            extrait.DEBIT_EXTRAIT = deb.MONT_DEBIT;
                            extrait.CREDIT_EXTRAIT = 0;
                            extrait.AUTRE_EXTRAIT = 0;
                            extrait.ENCOURFACT_EXTRAIT = db.All_Ecran_Financements(deb.REF_CTR_DEBIT).Select(p => p.Encours_Facture).FirstOrDefault();
                            extrait.DISPONIBLE_EXTRAIT = db.All_Ecran_Financements(deb.REF_CTR_DEBIT).Select(p => p.Disponible).FirstOrDefault();
                            extrait.DAT_OP_EXTRAIT = DateTime.Now;
                            extrait.DAT_VAL_EXTRAIT = deb.DATE_DEBIT;
                            extrait.TOTAL_FIN_EXTRAIT = db.All_Ecran_Financements(deb.REF_CTR_DEBIT).Select(p => p.Total_Financement).FirstOrDefault();
                            db.T_EXTRAIT.Add(extrait);
                            db.SaveChanges();

                        }
                        catch (Exception) { }


                        try
                        {
                            T_HISTORIQUE histo = new T_HISTORIQUE();
                            histo.ABREV_ROLE_HIST = "Debit "+ deb.ABEV_DEBIT;
                            histo.ACTION = "Creation Debit";
                            histo.ID_ENREGISTREMENT = db.T_MVT_DEBIT.Max().ToString();
                            histo.T_TABLE = "T_MVT_DEBIT";
                            histo.REF_CTR_HIST = deb.REF_CTR_DEBIT.ToString();
                            histo.REF_IND_HIST = db.TJ_CIR.Where(p => p.REF_CTR_CIR == deb.REF_CTR_DEBIT && p.ID_ROLE_CIR == "ADH").FirstOrDefault().REF_IND_CIR.ToString();
                            histo.LOGIN_USER = Session["UserLogin"].ToString();
                            histo.IP_PC = HttpContext.Request.UserHostAddress;
                            histo.NOM_PC = HttpContext.Request.UserHostName;
                            histo.DATE_ACTION = DateTime.Now;
                            db.T_HISTORIQUE.Add(histo);
                            db.SaveChanges();
                        }
                        catch (Exception) { }


                    }
                    catch (Exception) { TempData["error"] = "0x362114"; }
                    db.SaveChanges();
                    T_EC_CPT cp1 = new T_EC_CPT();
                    T_EC_CPT cp2 = new T_EC_CPT();

                    switch (test.Trim())
                    {

                        case "C":
                            try
                            {
                                cp1.ID_EC_CPT = db.T_EC_CPT.Max(p => p.ID_EC_CPT) + 1;
                            cp1.ANNEE_EC_CPT = DateTime.Now.Year;
                            cp1.CODE_DEP_EC_CPT = "A301";
                            cp1.NUM_LIGNE_EC_CPT = 1;
                            cp1.CODE_JOURNAL_EC_CPT = "ME";
                            cp1.DATE_SAISIE_EC_CPT = DateTime.Now;
                            cp1.DATE_EFFET_EC_CPT = enc.DAT_RECEP_ENC;
                            cp1.COMPTE_GEN_EC_CPT = "41700000";
                            cp1.MONTANT_EC_CPT = enc.MONT_ENC;
                            cp1.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                            cp1.REF_PIECE_EC_CPT = enc.REF_ENC;
                            cp1.TYPE_TRANSACTION_EC_CPT = "AR";
                            cp1.TYPE_DOC_EC_CPT = "M";
                            cp1.NOM_USER_EC_CPT = Session["UserLogin"].ToString(); // a changer 
                            cp1.LOT_EC_CPT = enc.REF_ENC;
                            cp1.CODE_TIERS_EC_CPT = enc.REF_ADH_ENC.ToString();
                            cp1.DOMAINE_EC_CPT = "medfact";
                            cp1.DATE_EC_CPT = DateTime.Now;
                            cp1.INTITULE_EC_CPT = "Saisie impayé chèque" + enc.REF_ENC;
                                db.T_EC_CPT.Add(cp1);
                                db.SaveChanges();
                            }
                            catch (Exception) { }

                            /***************************************************************************************************/

                            try
                            {
                                cp2.ID_EC_CPT = db.T_EC_CPT.Max(p => p.ID_EC_CPT) + 1;
                                cp2.ANNEE_EC_CPT = DateTime.Now.Year;
                                cp2.CODE_DEP_EC_CPT = "A301";
                                cp2.NUM_LIGNE_EC_CPT = 2;
                                cp2.CODE_JOURNAL_EC_CPT = "ME";
                                cp2.DATE_SAISIE_EC_CPT = DateTime.Now;
                                cp2.DATE_EFFET_EC_CPT = enc.DAT_RECEP_ENC;
                                cp2.COMPTE_GEN_EC_CPT = "41601000";
                                cp2.MONTANT_EC_CPT = 0 - enc.MONT_ENC;
                                cp2.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                                cp2.REF_PIECE_EC_CPT = enc.REF_ENC;
                                cp2.TYPE_TRANSACTION_EC_CPT = "AR";
                                cp2.TYPE_DOC_EC_CPT = "M";
                                cp2.NOM_USER_EC_CPT = Session["UserLogin"].ToString(); // a changer 
                                cp2.LOT_EC_CPT = enc.REF_ENC;
                                cp2.CODE_TIERS_EC_CPT = enc.REF_ADH_ENC.ToString();
                                cp2.DOMAINE_EC_CPT = "medfact";
                                cp2.DATE_EC_CPT = DateTime.Now;
                                cp2.INTITULE_EC_CPT = enc.REF_ENC + "   " + enc.REF_ENC;

                                db.T_EC_CPT.Add(cp2);
                                db.SaveChanges();
                            }
                            catch (Exception) { }
                            break;
                        /**********************************************************************************/
                        case "T":
                            int n = 0;
                            try
                            {
                                n = int.Parse(enc.REF_SEQ_ENC.Substring(5, 5));
                                //    (int.Parse(tYP_SEQTextBox_imp.Text.Substring(5, 5)));
                            }
                            catch { }
                            if (n < 50000)
                            {
                                try
                                {
                                    cp1.ID_EC_CPT = db.T_EC_CPT.Max(p => p.ID_EC_CPT) + 1;
                                    cp1.ANNEE_EC_CPT = DateTime.Now.Year;
                                    cp1.CODE_DEP_EC_CPT = "A301";
                                    cp1.NUM_LIGNE_EC_CPT = 1;
                                    cp1.CODE_JOURNAL_EC_CPT = "ME";
                                    cp1.DATE_SAISIE_EC_CPT = DateTime.Now;
                                    cp1.DATE_EFFET_EC_CPT = enc.DAT_RECEP_ENC;
                                    cp1.COMPTE_GEN_EC_CPT = "41700000";
                                    cp1.MONTANT_EC_CPT = enc.MONT_ENC;
                                    cp1.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                                    cp1.REF_PIECE_EC_CPT = enc.REF_ENC;
                                    cp1.TYPE_TRANSACTION_EC_CPT = "AR";
                                    cp1.TYPE_DOC_EC_CPT = "M";
                                    cp1.NOM_USER_EC_CPT = Session["UserLogin"].ToString(); // a changer 
                                    cp1.LOT_EC_CPT = enc.REF_ENC;
                                    cp1.CODE_TIERS_EC_CPT = enc.REF_ADH_ENC.ToString();
                                    cp1.DOMAINE_EC_CPT = "medfact";
                                    cp1.DATE_EC_CPT = DateTime.Now;
                                    cp1.INTITULE_EC_CPT = "Saisie impayé traite à l'encaissement";
                                    db.T_EC_CPT.Add(cp1);
                                    db.SaveChanges();
                                }
                                catch (Exception) { }
                                /***************************************************************************************************/
                                try
                                {
                                    cp2.ID_EC_CPT = db.T_EC_CPT.Max(p => p.ID_EC_CPT) + 1;
                                    cp2.ANNEE_EC_CPT = DateTime.Now.Year;
                                    cp2.CODE_DEP_EC_CPT = "A301";
                                    cp2.NUM_LIGNE_EC_CPT = 2;
                                    cp2.CODE_JOURNAL_EC_CPT = "CR";
                                    cp2.DATE_SAISIE_EC_CPT = DateTime.Now;
                                    cp2.DATE_EFFET_EC_CPT = enc.DAT_RECEP_ENC;
                                    cp2.COMPTE_GEN_EC_CPT = "41601000";
                                    cp2.MONTANT_EC_CPT = 0 - enc.MONT_ENC;
                                    cp2.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                                    cp2.REF_PIECE_EC_CPT = enc.REF_ENC;
                                    cp2.TYPE_TRANSACTION_EC_CPT = "AR";
                                    cp2.TYPE_DOC_EC_CPT = "M";
                                    cp2.NOM_USER_EC_CPT = Session["UserLogin"].ToString(); // a changer 
                                    cp2.LOT_EC_CPT = enc.REF_ENC;
                                    cp2.CODE_TIERS_EC_CPT = enc.REF_ADH_ENC.ToString();
                                    cp2.DOMAINE_EC_CPT = "medfact";
                                    cp2.DATE_EC_CPT = DateTime.Now;
                                    cp2.INTITULE_EC_CPT = "Saisie impayé traite à l'encaissement" + "   " + enc.REF_ENC;

                                    db.T_EC_CPT.Add(cp2);
                                    db.SaveChanges();
                                }
                                catch (Exception) { }
                                /**************************/
                                T_EC_CPT cp3 = new T_EC_CPT();
                                T_EC_CPT cp4 = new T_EC_CPT();
                                T_EC_CPT cp5 = new T_EC_CPT();
                                T_EC_CPT cp6 = new T_EC_CPT();
                                /*****************************************************/
                                try
                                {
                                    cp3.ID_EC_CPT = db.T_EC_CPT.Max(p => p.ID_EC_CPT) + 1;
                                    cp3.ANNEE_EC_CPT = DateTime.Now.Year;
                                    cp3.CODE_DEP_EC_CPT = "A301";
                                    cp3.NUM_LIGNE_EC_CPT = 1;
                                    cp3.CODE_JOURNAL_EC_CPT = "JN";
                                    cp3.DATE_SAISIE_EC_CPT = DateTime.Now;
                                    cp3.DATE_EFFET_EC_CPT = enc.DAT_RECEP_ENC;
                                    cp3.COMPTE_GEN_EC_CPT = "41601000";
                                    cp3.MONTANT_EC_CPT = enc.MONT_ENC;
                                    cp3.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                                    cp3.REF_PIECE_EC_CPT = enc.REF_ENC;
                                    cp3.TYPE_TRANSACTION_EC_CPT = "AR";
                                    cp3.TYPE_DOC_EC_CPT = "D";
                                    cp3.NOM_USER_EC_CPT = Session["UserLogin"].ToString(); // a changer 
                                    cp3.LOT_EC_CPT = enc.REF_ENC;
                                    cp3.CODE_TIERS_EC_CPT = enc.REF_ADH_ENC.ToString();
                                    cp3.DOMAINE_EC_CPT = "medfact";
                                    cp3.DATE_EC_CPT = DateTime.Now;
                                    cp3.INTITULE_EC_CPT = "Saisie impayé traite à l'encaissement";
                                    db.T_EC_CPT.Add(cp3);
                                    db.SaveChanges();
                                }
                                catch (Exception) { }

                                try
                                {
                                    cp4.ID_EC_CPT = db.T_EC_CPT.Max(p => p.ID_EC_CPT) + 1;
                                    cp4.ANNEE_EC_CPT = DateTime.Now.Year;
                                    cp4.CODE_DEP_EC_CPT = "A301";
                                    cp4.NUM_LIGNE_EC_CPT = 2;
                                    cp4.CODE_JOURNAL_EC_CPT = "JN";
                                    cp4.DATE_SAISIE_EC_CPT = DateTime.Now;
                                    cp4.DATE_EFFET_EC_CPT = enc.DAT_RECEP_ENC;
                                    cp4.COMPTE_GEN_EC_CPT = "41301000";
                                    cp4.MONTANT_EC_CPT = 0 - enc.MONT_ENC;
                                    cp4.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                                    cp4.REF_PIECE_EC_CPT = enc.REF_ENC;
                                    cp4.TYPE_TRANSACTION_EC_CPT = "AR";
                                    cp4.TYPE_DOC_EC_CPT = "D";
                                    cp4.NOM_USER_EC_CPT = Session["UserLogin"].ToString(); // a changer 
                                    cp4.LOT_EC_CPT = enc.REF_ENC;
                                    cp4.CODE_TIERS_EC_CPT = enc.REF_ADH_ENC.ToString();
                                    cp4.DOMAINE_EC_CPT = "medfact";
                                    cp4.DATE_EC_CPT = DateTime.Now;
                                    cp4.INTITULE_EC_CPT = "Saisie impayé traite à l'encaissement" + enc.REF_ENC;
                                    db.T_EC_CPT.Add(cp4);
                                    db.SaveChanges();
                                }
                                catch (Exception) { }

                                try
                                {
                                    cp5.ID_EC_CPT = db.T_EC_CPT.Max(p => p.ID_EC_CPT) + 1;
                                    cp5.ANNEE_EC_CPT = DateTime.Now.Year;
                                    cp5.CODE_DEP_EC_CPT = "A301";
                                    cp5.NUM_LIGNE_EC_CPT = 3;
                                    cp5.CODE_JOURNAL_EC_CPT = "JN";
                                    cp5.DATE_SAISIE_EC_CPT = DateTime.Now;
                                    cp5.DATE_EFFET_EC_CPT = enc.DAT_RECEP_ENC;
                                    cp5.COMPTE_GEN_EC_CPT = "41301000";
                                    cp5.MONTANT_EC_CPT = enc.MONT_ENC;
                                    cp5.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                                    cp5.REF_PIECE_EC_CPT = enc.REF_ENC;
                                    cp5.TYPE_TRANSACTION_EC_CPT = "AR";
                                    cp5.TYPE_DOC_EC_CPT = "D";
                                    cp5.NOM_USER_EC_CPT = Session["UserLogin"].ToString(); // a changer 
                                    cp5.LOT_EC_CPT = enc.REF_ENC;
                                    cp5.CODE_TIERS_EC_CPT = enc.REF_ADH_ENC.ToString();
                                    cp5.DOMAINE_EC_CPT = "medfact";
                                    cp5.DATE_EC_CPT = DateTime.Now;
                                    cp5.INTITULE_EC_CPT = "Saisie impayé traite à l'encaissement";
                                    db.T_EC_CPT.Add(cp5);
                                    db.SaveChanges();
                                }
                                catch (Exception) { }

                                try
                                {
                                    cp5.ID_EC_CPT = db.T_EC_CPT.Max(p => p.ID_EC_CPT) + 4;
                                    cp5.ANNEE_EC_CPT = DateTime.Now.Year;
                                    cp5.CODE_DEP_EC_CPT = "A301";
                                    cp5.NUM_LIGNE_EC_CPT = 4;
                                    cp5.CODE_JOURNAL_EC_CPT = "JN";
                                    cp5.DATE_SAISIE_EC_CPT = DateTime.Now;
                                    cp5.DATE_EFFET_EC_CPT = enc.DAT_RECEP_ENC;
                                    cp5.COMPTE_GEN_EC_CPT = "53130000";
                                    cp5.MONTANT_EC_CPT = 0 - enc.MONT_ENC;
                                    cp5.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                                    cp5.REF_PIECE_EC_CPT = enc.REF_ENC;
                                    cp5.TYPE_TRANSACTION_EC_CPT = "AR";
                                    cp5.TYPE_DOC_EC_CPT = "D";
                                    cp5.NOM_USER_EC_CPT = Session["UserLogin"].ToString(); // a changer 
                                    cp5.LOT_EC_CPT = enc.REF_ENC;
                                    cp5.CODE_TIERS_EC_CPT = enc.REF_ADH_ENC.ToString();
                                    cp5.DOMAINE_EC_CPT = "medfact";
                                    cp5.DATE_EC_CPT = DateTime.Now;
                                    cp5.INTITULE_EC_CPT = "Saisie impayé traite à l'encaissement" + enc.REF_ENC;
                                    db.T_EC_CPT.Add(cp6);
                                    db.SaveChanges();
                                }
                                catch (Exception) { }


                            }
                            else {
                                try
                                {
                                    cp1.ID_EC_CPT = db.T_EC_CPT.Max(p => p.ID_EC_CPT) + 1;
                                    cp1.ANNEE_EC_CPT = DateTime.Now.Year;
                                    cp1.CODE_DEP_EC_CPT = "A301";
                                    cp1.NUM_LIGNE_EC_CPT = 1;
                                    cp1.CODE_JOURNAL_EC_CPT = "ME";
                                    cp1.DATE_SAISIE_EC_CPT = DateTime.Now;
                                    cp1.DATE_EFFET_EC_CPT = enc.DAT_RECEP_ENC;
                                    cp1.COMPTE_GEN_EC_CPT = "41700000";
                                    cp1.MONTANT_EC_CPT = enc.MONT_ENC;
                                    cp1.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                                    cp1.REF_PIECE_EC_CPT = enc.REF_ENC;
                                    cp1.TYPE_TRANSACTION_EC_CPT = "AR";
                                    cp1.TYPE_DOC_EC_CPT = "M";
                                    cp1.NOM_USER_EC_CPT = Session["UserLogin"].ToString(); // a changer 
                                    cp1.LOT_EC_CPT = enc.REF_ENC;
                                    cp1.CODE_TIERS_EC_CPT = enc.REF_ADH_ENC.ToString();
                                    cp1.DOMAINE_EC_CPT = "medfact";
                                    cp1.DATE_EC_CPT = DateTime.Now;
                                    cp1.INTITULE_EC_CPT = "Saisie impayé traite à l'escompte";
                                    db.T_EC_CPT.Add(cp1);
                                    db.SaveChanges();
                                }
                                catch (Exception) { }
                                /***************************************************************************************************/
                                try
                                {
                                    cp2.ID_EC_CPT = db.T_EC_CPT.Max(p => p.ID_EC_CPT) + 1;
                                    cp2.ANNEE_EC_CPT = DateTime.Now.Year;
                                    cp2.CODE_DEP_EC_CPT = "A301";
                                    cp2.NUM_LIGNE_EC_CPT = 2;
                                    cp2.CODE_JOURNAL_EC_CPT = "CR";
                                    cp2.DATE_SAISIE_EC_CPT = DateTime.Now;
                                    cp2.DATE_EFFET_EC_CPT = enc.DAT_RECEP_ENC;
                                    cp2.COMPTE_GEN_EC_CPT = "41601000";
                                    cp2.MONTANT_EC_CPT = 0 - enc.MONT_ENC;
                                    cp2.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                                    cp2.REF_PIECE_EC_CPT = enc.REF_ENC;
                                    cp2.TYPE_TRANSACTION_EC_CPT = "AR";
                                    cp2.TYPE_DOC_EC_CPT = "M";
                                    cp2.NOM_USER_EC_CPT = Session["UserLogin"].ToString(); // a changer 
                                    cp2.LOT_EC_CPT = enc.REF_ENC;
                                    cp2.CODE_TIERS_EC_CPT = enc.REF_ADH_ENC.ToString();
                                    cp2.DOMAINE_EC_CPT = "medfact";
                                    cp2.DATE_EC_CPT = DateTime.Now;
                                    cp2.INTITULE_EC_CPT = "Saisie impayé traite à l'escompte" + "   " + enc.REF_ENC;

                                    db.T_EC_CPT.Add(cp2);
                                    db.SaveChanges();
                                }
                                catch (Exception) { }


                            }
                         

                            break;
                        /************************************************************************************************************/
                     
                    }




                    TempData["notice"] = "l impayé a éte enregistrer";
                    return RedirectToAction("SaisiImpaye");
                }
                catch (Exception) { return RedirectToAction("InternalServerError", "Error"); }
            }
            else
            {
                TempData["error"] = "0x963524";
                return RedirectToAction("InternalServerError", "Error");
            }
        }

        public ActionResult ResoluionImpaye(int id,int id2)
        {
            T_IMPAYE imp;
            try { imp = db.T_IMPAYE.Where(p => p.ID_ENC_IMP == id).FirstOrDefault(); } catch (Exception) { imp = null; }
            if (imp == null)
            {
                db.UpdateNvEnc(id, id2);
                db.Up_Remp_IDAncEnc_IDNvEnc(id, id2);
                TempData["notice"] = "La résolution d impayé est traité";

                try
                {
                    T_HISTORIQUE histo = new T_HISTORIQUE();
                    histo.ABREV_ROLE_HIST = "Resoluion Impaye " + db.T_ENCAISSEMENT.Find(imp.ID_ENC_IMP).REF_ENC;
                    histo.ACTION = "Resoluion Impaye";
                    histo.ID_ENREGISTREMENT = imp.ID_IMP.ToString();
                    histo.T_TABLE = "T_IMPAYE";
                    histo.REF_CTR_HIST = db.T_ENCAISSEMENT.Find(imp.ID_ENC_IMP).REF_CTR_ENC.ToString();
                    histo.REF_IND_HIST = db.T_ENCAISSEMENT.Find(imp.ID_ENC_IMP).REF_ACH_ENC.ToString();
                    histo.LOGIN_USER = Session["UserLogin"].ToString();
                    histo.IP_PC = HttpContext.Request.UserHostAddress;
                    histo.NOM_PC = HttpContext.Request.UserHostName;
                    histo.DATE_ACTION = DateTime.Now;
                    db.T_HISTORIQUE.Add(histo);
                    db.SaveChanges();
                }
                catch (Exception) { }


                return RedirectToAction("ListeDesImpaye", "Bordereau");
             
            }
            else { return RedirectToAction("InternalServerError", "Error"); }
        }


        public ActionResult ListeDesImpaye()
        {
            if (Session["UserLogin"] != null)
            {
                try
                {
                    ViewBag.ListImpaye = db.ListeDesImpayes();
                    TempData["Impaye"] = "active";
                    TempData["ListeDesImpaye"] = "active";
                    return View();
                }
                catch (Exception) { return RedirectToAction("InternalServerError", "Error"); }
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }

        public JsonResult FillByRefAdh(string refAdh)
        {
            refAdh = refAdh.Trim();
            var t = db.T_ENC_MATERIALISER_FillByRefAdh(refAdh).ToList();
            return Json(t,JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecherchEncAchRefCtr(int ref_ctr,int ref_ach)
        {
            //var ListNomInd = ;

            try
            {
                var  listeDesEncaissement = db.SelectRefEncaissementParCtrETAch(ref_ctr, ref_ach).ToList();
                return Json(listeDesEncaissement, JsonRequestBehavior.AllowGet);
            }
            catch (Exception) { return Json("", JsonRequestBehavior.AllowGet); }

         
        }



        public JsonResult RecherchEncCtr(int ref_ctr)
        {
            //var ListNomInd = ;

            try
            {
                var listeDesEncaissement = db.SelectRefEncaissementParCtr(ref_ctr).ToList();
                return Json(listeDesEncaissement, JsonRequestBehavior.AllowGet);
            }
            catch (Exception) { return Json("", JsonRequestBehavior.AllowGet); }


        }

        public ActionResult LettrageEncFact(string MNT_OUV, string MNT_ENC, string RET, string LET, string REST, string ID_DET_BORD, string ID_ENC)
        {
            try
            {
                if (MNT_OUV != "" && MNT_ENC != "" && RET != "" && LET != "" && REST != "" && ID_DET_BORD != "" && ID_ENC != "")
                {
                    int v = int.Parse(ID_ENC);
                    decimal montant_lettrage = 0;
                        try { montant_lettrage = (decimal)db.TJ_LETTRAGE.Where(p => p.ID_ENC_LET == v).Sum(p => p.MONT_TTC_LET); } catch (Exception) { montant_lettrage = 0; }
                    decimal test = (decimal)db.T_ENCAISSEMENT.Where(p => p.ID_ENC == v).Select(p => p.MONT_ENC).SingleOrDefault();

                    decimal MTrestant = 0;
                    string message = "";
                    //string mnto = MNT_OUV.ToString("0.00");
                    MNT_OUV = MNT_OUV.Replace(" ", "");
                    MNT_ENC = MNT_ENC.Replace(" ", "");
                    RET= RET.Replace(" ", "");
                    decimal MNT_OUV_L = decimal.Parse(MNT_OUV.Replace(".", ","));
                    decimal MNT_ENC_L = decimal.Parse(MNT_ENC.Replace(".", ","));
                    decimal RETC_L = decimal.Parse(RET.Replace(".", ","));
                    //      if (test - montant_lettrage < MNT_ENC_L) { return RedirectToAction("InternalServerError", "Error"); }
                    if (MNT_OUV_L == MNT_ENC_L)
                    {
                        MTrestant = MNT_OUV_L;
                    }
                    else if (MNT_ENC_L < MNT_OUV_L)
                    {
                        MTrestant = MNT_ENC_L;

                    }
                    else
                    {
                        MTrestant = MNT_OUV_L;
                        TempData["war"] = "tt";
                    }
                    if (MTrestant < 0)
                    { MTrestant = 0; }

                    TJ_LETTRAGE let = new TJ_LETTRAGE();
                    let.ID_DET_BORD_LET = int.Parse(ID_DET_BORD);
                    let.ID_ENC_LET = int.Parse(ID_ENC);
                    let.MONT_TTC_LET = MTrestant;
                    let.DAT_LET = DateTime.Now;
                    let.DAT_RECONCIL = DateTime.Now;
                    let.VALIDE_LET = true;
                    let.VALIDE_RECONCIL = false;
                    db.TJ_LETTRAGE.Add(let);
                    if (RETC_L > 0) {

                        T_DET_BORD bor = db.T_DET_BORD.Where(p => p.ID_DET_BORD == ID_DET_BORD).Single();
                        string x = "";
                        switch (bor.TYP_DET_BORD.ToString().Trim())
                        {
                            case "fact": x = "F"; break;
                            case "Caut": x = "C"; break;
                            case "bdc": x = "BC"; break;
                            case "march": x = "M"; break;
                        }

                        T_FOND_GARANTIE f = db.T_FOND_GARANTIE.Where(p => p.REF_CTR_FDG == bor.REF_CTR_DET_BORD && p.TYP_FDG == x).SingleOrDefault();
                        if (f.TX_FDG.ToString() == "")
                        {
                            f.TX_FDG = 0;
                        }
                        decimal TX_FDG = decimal.Parse(f.TX_FDG.ToString().Replace(".", ","));
                        TX_FDG = TX_FDG / 100;
                        if (bor.MONT_OUV_DET_BORD - RETC_L < 0)
                        {
                            bor.MONT_OUV_DET_BORD = 0;
                            bor.MONT_FDG_LIBERE_DET_BORD = bor.MONT_FDG_LIBERE_DET_BORD + bor.MONT_FDG_DET_BORD;
                            bor.MONT_FDG_DET_BORD = 0;
                        }
                        else
                        {
                            bor.MONT_OUV_DET_BORD = bor.MONT_OUV_DET_BORD - RETC_L;
                            bor.MONT_FDG_LIBERE_DET_BORD = bor.MONT_FDG_LIBERE_DET_BORD + bor.MONT_FDG_DET_BORD;
                            bor.MONT_FDG_DET_BORD = TX_FDG * bor.MONT_OUV_DET_BORD;
                            db.Entry(bor).State = EntityState.Modified;
                        }
                    }
                   
                    TempData["messagee"] = message + " Lettrage de facture n° " + db.TJ_DOCUMENT_DET_BORD.Where(p => p.ID_DET_BORD == ID_DET_BORD).Select(p => p.REF_DOCUMENT_DET_BORD).SingleOrDefault() + " avec l encaissement n° " + db.T_ENCAISSEMENT.Where(p => p.ID_ENC.ToString() == ID_ENC).Select(p => p.REF_ENC).SingleOrDefault() + "a ete bien enregistrer";
                    db.SaveChanges();

                }
                else
                {
                    TempData["alert"] = "merci de choisir une facture ";
                }
                TempData["tab2"] = "active";

                return RedirectToAction("LettrageEtEncaissement");
            }
            catch (Exception e) { TempData["error"] = e.Message; return RedirectToAction("InternalServerError", "Error"); }
        }
        /**************Lettrage JSON *****************************/

        public ActionResult LettrageEncFactJson(string MNT_OUV, string MNT_ENC, string RET, string LET, string REST, string ID_DET_BORD, string ID_ENC)
        {
            try
            {
                if (MNT_OUV != "" && MNT_ENC != "" && RET != "" && LET != "" && REST != "" && ID_DET_BORD != "" && ID_ENC != "")
                {
                    int v = int.Parse(ID_ENC);
                    //if (db.T_ENCAISSEMENT.Where(p => p.ID_ENC == v).Select(p => p.TYP_ENC).SingleOrDefault() == "A")
                    //{

                    //    TJ_LETTRAGE let2 = new TJ_LETTRAGE();
                    //    let2.ID_DET_BORD_LET = int.Parse(ID_DET_BORD);
                    //    let2.ID_ENC_LET = int.Parse(ID_ENC);
                    //    let2.MONT_TTC_LET = db.T_ENCAISSEMENT.Where(p => p.ID_ENC == v).Select(p => p.MONT_ENC).SingleOrDefault();
                    //    let2.DAT_LET = DateTime.Now;
                    //    let2.DAT_RECONCIL = DateTime.Now;
                    //    let2.VALIDE_LET = true;
                    //    let2.VALIDE_RECONCIL = true;
                    //    db.TJ_LETTRAGE.Add(let2);

                    //    T_DET_BORD bor = db.T_DET_BORD.Where(p => p.ID_DET_BORD == ID_DET_BORD).Single();
                    //    string x = "";
                    //    switch (bor.TYP_DET_BORD.ToString().Trim())
                    //    {
                    //        case "fact": x = "F"; break;
                    //        case "Caut": x = "C"; break;
                    //        case "bdc": x = "BC"; break;
                    //        case "march": x = "M"; break;
                    //    }

                    //    T_FOND_GARANTIE f = db.T_FOND_GARANTIE.Where(p => p.REF_CTR_FDG == bor.REF_CTR_DET_BORD && p.TYP_FDG == x).SingleOrDefault();
                    //    if (f.TX_FDG.ToString() == "")
                    //    {
                    //        f.TX_FDG = 0;
                    //    }
                    //    decimal TX_FDG = (decimal)bor.TX_FDG_DET_BORD;
                    //    TX_FDG = TX_FDG / 100;
                    //    //bor.RETENU_DET_BORD = RETC_L;
                    //    bor.MONT_OUV_DET_BORD = bor.MONT_OUV_DET_BORD - db.T_ENCAISSEMENT.Where(p => p.ID_ENC == v).Select(p => p.MONT_ENC).SingleOrDefault();
                    //    bor.MONT_FDG_DET_BORD = Math.Round((decimal)(TX_FDG * bor.MONT_OUV_DET_BORD),3);
                    //    db.Entry(bor).State = EntityState.Modified;
                    //    db.SaveChanges();
                    //    TempData["messagee"] =  " Lettrage de facture n° " + db.TJ_DOCUMENT_DET_BORD.Where(p => p.ID_DET_BORD == ID_DET_BORD).Select(p => p.REF_DOCUMENT_DET_BORD).SingleOrDefault() + " avec l encaissement n° " + db.T_ENCAISSEMENT.Where(p => p.ID_ENC.ToString() == ID_ENC).Select(p => p.REF_ENC).SingleOrDefault() + "a ete bien enregistrer et réconsilier ";
                       

                    //}

                   // else {
                    decimal montant_lettrage = 0;
                    try { montant_lettrage = (decimal)db.TJ_LETTRAGE.Where(p => p.ID_ENC_LET == v).Sum(p => p.MONT_TTC_LET); } catch (Exception) { montant_lettrage = 0; }
                    decimal test = (decimal)db.T_ENCAISSEMENT.Where(p => p.ID_ENC == v).Select(p => p.MONT_ENC).SingleOrDefault();

                    decimal MTrestant = 0;
                    string message = "";
                    //string mnto = MNT_OUV.ToString("0.00");
                    MNT_OUV = MNT_OUV.Replace(" ", "");
                    MNT_ENC = MNT_ENC.Replace(" ", "");
                    RET = RET.Replace(" ", "");
                    decimal MNT_OUV_L = decimal.Parse(MNT_OUV.Replace(".", ","));
                    decimal MNT_ENC_L = decimal.Parse(MNT_ENC.Replace(".", ","));
                    decimal RETC_L = decimal.Parse(RET.Replace(".", ","));
                    //      if (test - montant_lettrage < MNT_ENC_L) { return RedirectToAction("InternalServerError", "Error"); }
                    if (MNT_OUV_L == MNT_ENC_L)
                    {
                        MTrestant = MNT_OUV_L;
                    }
                    else if (MNT_ENC_L < MNT_OUV_L)
                    {
                        MTrestant = MNT_ENC_L;

                    }
                    else
                    {
                        MTrestant = MNT_OUV_L;
                        TempData["war"] = "tt";
                    }
                    if (MTrestant < 0)
                    { MTrestant = 0; }

                    TJ_LETTRAGE let = new TJ_LETTRAGE();
                    let.ID_DET_BORD_LET = int.Parse(ID_DET_BORD);
                    let.ID_ENC_LET = int.Parse(ID_ENC);
                    let.MONT_TTC_LET = MTrestant;
                    let.DAT_LET = DateTime.Now;
                    let.DAT_RECONCIL = DateTime.Now;
                    let.VALIDE_LET = true;
                    let.VALIDE_RECONCIL = false;
                    db.TJ_LETTRAGE.Add(let);
                    db.SaveChanges();
                    if (RETC_L > 0)
                    {

                        T_DET_BORD bor = db.T_DET_BORD.Where(p => p.ID_DET_BORD == ID_DET_BORD).Single();
                        string x = "";
                        switch (bor.TYP_DET_BORD.ToString().Trim())
                        {
                            case "fact": x = "F"; break;
                            case "Caut": x = "C"; break;
                            case "bdc": x = "BC"; break;
                            case "march": x = "M"; break;
                        }

                        T_FOND_GARANTIE f = db.T_FOND_GARANTIE.Where(p => p.REF_CTR_FDG == bor.REF_CTR_DET_BORD && p.TYP_FDG == x).SingleOrDefault();
                        if (f.TX_FDG.ToString() == "")
                        {
                            f.TX_FDG = 0;
                        }
                        decimal TX_FDG = (decimal)bor.TX_FDG_DET_BORD;
                        TX_FDG = TX_FDG / 100;
                        decimal fdganc = (decimal)bor.MONT_FDG_DET_BORD;
                        bor.RETENU_DET_BORD = RETC_L;

                        if (bor.MONT_OUV_DET_BORD - RETC_L < 0) {
                            bor.MONT_OUV_DET_BORD = 0;
                            bor.MONT_FDG_DET_BORD = 0;
                            bor.MONT_FDG_LIBERE_DET_BORD = bor.MONT_FDG_LIBERE_DET_BORD + (fdganc - bor.MONT_FDG_DET_BORD);
                            db.Entry(bor).State = EntityState.Modified;
                        }
                        else {
                            bor.MONT_OUV_DET_BORD = bor.MONT_OUV_DET_BORD - RETC_L;
                            bor.MONT_FDG_DET_BORD = Math.Round((decimal)(TX_FDG * bor.MONT_OUV_DET_BORD), 3);
                            bor.MONT_FDG_LIBERE_DET_BORD = bor.MONT_FDG_LIBERE_DET_BORD + (fdganc - bor.MONT_FDG_DET_BORD);
                            db.Entry(bor).State = EntityState.Modified;
                        }
                       
                        db.SaveChanges();

                        try
                        {
                            T_EXTRAIT extrait = new T_EXTRAIT();
                            extrait.REF_CTR_EXTRAIT = bor.REF_CTR_DET_BORD;
                            extrait.LIB_EXTRAIT = "RS " + db.TJ_DOCUMENT_DET_BORD.Where(p=>p.ID_DET_BORD== bor.ID_DET_BORD).First().REF_DOCUMENT_DET_BORD;
                            extrait.DEBIT_EXTRAIT = RETC_L;
                            extrait.CREDIT_EXTRAIT = 0;
                            extrait.AUTRE_EXTRAIT = 0;
                            extrait.ENCOURFACT_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Encours_Facture).FirstOrDefault();
                            extrait.DISPONIBLE_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Disponible).FirstOrDefault();
                            extrait.DAT_OP_EXTRAIT = DateTime.Now;
                            extrait.DAT_VAL_EXTRAIT = DateTime.Now;
                            extrait.TOTAL_FIN_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Total_Financement).FirstOrDefault();
                            db.T_EXTRAIT.Add(extrait);
                            db.SaveChanges();
                        }
                        catch (Exception e) { }


                    }

                    TempData["messagee"] = message + " Lettrage de facture n° " + db.TJ_DOCUMENT_DET_BORD.Where(p => p.ID_DET_BORD == ID_DET_BORD).Select(p => p.REF_DOCUMENT_DET_BORD).SingleOrDefault() + " avec l encaissement n° " + db.T_ENCAISSEMENT.Where(p => p.ID_ENC.ToString() == ID_ENC).Select(p => p.REF_ENC).SingleOrDefault() + "a ete bien enregistrer";
                    db.SaveChanges();

                    try
                    {
                        T_HISTORIQUE histo = new T_HISTORIQUE();
                        histo.ABREV_ROLE_HIST = " Lettrage de facture n° "+let.ID_DET_BORD_LET;
                        histo.ACTION = "Lettrage";
                        histo.ID_ENREGISTREMENT = db.TJ_LETTRAGE.Count().ToString();
                        histo.T_TABLE = "TJ_LETTRAGE";
                        histo.REF_CTR_HIST = db.TJ_DOCUMENT_DET_BORD.Where(p => p.ID_DET_BORD == ID_DET_BORD).Select(p => p.REF_CTR_DET_BORD).SingleOrDefault().ToString();
                        histo.REF_IND_HIST = db.T_ENCAISSEMENT.Where(p => p.ID_ENC.ToString() == ID_ENC).Select(p => p.REF_ACH_ENC).SingleOrDefault().ToString();
                        histo.LOGIN_USER = Session["UserLogin"].ToString();
                        histo.IP_PC = HttpContext.Request.UserHostAddress;
                        histo.NOM_PC = HttpContext.Request.UserHostName;
                        histo.DATE_ACTION = DateTime.Now;
                        db.T_HISTORIQUE.Add(histo);
                        db.SaveChanges();
                    }
                    catch (Exception) { }


                    //}
                    var y = new
                    {
                        type = "save",
                        messagee = TempData["messagee"],
                        war = TempData["war"]
                    };
                    return Json(y, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    TempData["alert"] = "merci de choisir une facture ";
                    var y = new
                    {
                        type = "error",
                        messagee = TempData["alert"],
                       // war = TempData["war"]
                    };
                    return Json(y, JsonRequestBehavior.AllowGet);
                }
               // TempData["tab2"] = "active";
               
               
            }
            catch (Exception e) {

                TempData["alert"] = e.Message;
                var y = new
                {
                    type = "error",
                    messagee = TempData["alert"],
                    // war = TempData["war"]
                };
                return Json(y, JsonRequestBehavior.AllowGet);
            }
        }

        /*******************END *********************************/



        public ActionResult UpdateDateValeurEnc(DateTime Date_Val, int id_enc)
        {
            if (id_enc == -1) { TempData["error"] = "0x361148"; return RedirectToAction("InternalServerError", "Error"); }
            else
            {
                T_ENCAISSEMENT en = db.T_ENCAISSEMENT.Where(p => p.ID_ENC == id_enc).FirstOrDefault();
                en.DAT_VAL_ENC = Date_Val;
                db.Entry(en).State = EntityState.Modified;
                db.SaveChanges();
                try
                {
                    decimal fraispro = (decimal)db.T_FRAIS_DIVERS.Where(p => p.REF_CTR_FRAIS_DIVERS == en.REF_CTR_ENC && p.TYP_FRAIS_DIVERS.Contains("ProE")).Select(p => p.MONT_PAR_UNIT_FRAIS_DIVERS).SingleOrDefault();
                    T_MVT_DEBIT deb = new T_MVT_DEBIT();
                    deb.REF_CTR_DEBIT = en.REF_CTR_ENC;
                    deb.ABEV_DEBIT = "prorogation echeance";
                    deb.TYP_DEBIT = en.ID_ENC.ToString();
                    deb.MONT_DEBIT = fraispro;
                    deb.DATE_DEBIT = DateTime.Now;
                    db.T_MVT_DEBIT.Add(deb);
                    db.SaveChanges();


                    try
                    {
                        T_EXTRAIT extrait = new T_EXTRAIT();
                        extrait.REF_CTR_EXTRAIT = deb.REF_CTR_DEBIT;
                        extrait.LIB_EXTRAIT = "Debit " + deb.ABEV_DEBIT;
                        extrait.DEBIT_EXTRAIT = deb.MONT_DEBIT;
                        extrait.CREDIT_EXTRAIT = 0;
                        extrait.AUTRE_EXTRAIT = 0;
                        extrait.ENCOURFACT_EXTRAIT = db.All_Ecran_Financements(deb.REF_CTR_DEBIT).Select(p => p.Encours_Facture).FirstOrDefault();
                        extrait.DISPONIBLE_EXTRAIT = db.All_Ecran_Financements(deb.REF_CTR_DEBIT).Select(p => p.Disponible).FirstOrDefault();
                        extrait.DAT_OP_EXTRAIT = DateTime.Now;
                        extrait.DAT_VAL_EXTRAIT = deb.DATE_DEBIT;
                        extrait.TOTAL_FIN_EXTRAIT = db.All_Ecran_Financements(deb.REF_CTR_DEBIT).Select(p => p.Total_Financement).FirstOrDefault();
                        db.T_EXTRAIT.Add(extrait);
                        db.SaveChanges();

                    }
                    catch (Exception) { }


                    try
                    {
                        T_HISTORIQUE histo = new T_HISTORIQUE();
                        histo.ABREV_ROLE_HIST = "Debit " + deb.ABEV_DEBIT;
                        histo.ACTION = "Creation Debit";
                        histo.ID_ENREGISTREMENT = db.T_MVT_DEBIT.Max().ToString();
                        histo.T_TABLE = "T_MVT_DEBIT";
                        histo.REF_CTR_HIST = deb.REF_CTR_DEBIT.ToString();
                        histo.REF_IND_HIST = db.TJ_CIR.Where(p => p.REF_CTR_CIR == deb.REF_CTR_DEBIT && p.ID_ROLE_CIR == "ADH").FirstOrDefault().REF_IND_CIR.ToString();
                        histo.LOGIN_USER = Session["UserLogin"].ToString();
                        histo.IP_PC = HttpContext.Request.UserHostAddress;
                        histo.NOM_PC = HttpContext.Request.UserHostName;
                        histo.DATE_ACTION = DateTime.Now;
                        db.T_HISTORIQUE.Add(histo);
                        db.SaveChanges();
                    }
                    catch (Exception) { }

                }
                catch (Exception) { TempData["error"] = "0x31192"; }
                TempData["message"] = "la date valeur de l encaissement  " + en.REF_ENC + "  a été effectué avec succes";
                TempData["tab3"] = "active";
                return RedirectToAction("LettrageEtEncaissement");
            }
        }

        public ActionResult Réconsiliation(int id)
        {
            List<TJ_LETTRAGE> lettraje = db.TJ_LETTRAGE.Where(p => p.ID_ENC_LET == id).ToList();
            string typeb = "";
            foreach (var item in lettraje)
            {
                item.VALIDE_RECONCIL = true;
                item.DAT_RECONCIL = DateTime.Now;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                T_DET_BORD bor = db.T_DET_BORD.Where(p => p.ID_DET_BORD == item.ID_DET_BORD_LET.ToString()).FirstOrDefault();
               
                switch (bor.TYP_DET_BORD.ToString().Trim())
                {
                    case "fact": typeb = "F"; break;
                    case "Caut": typeb = "C"; break;
                    case "bdc": typeb = "BC"; break;
                    case "march": typeb = "M"; break;
                }
                T_FOND_GARANTIE f = db.T_FOND_GARANTIE.Where(p => p.REF_CTR_FDG == bor.REF_CTR_DET_BORD && p.TYP_FDG == typeb).SingleOrDefault();
                decimal TX_FDG = decimal.Parse(f.TX_FDG.ToString().Replace(".", ","));
                TX_FDG = TX_FDG / 100;
                TJ_LETTRAGE lettraje2 = db.TJ_LETTRAGE.Where(p => p.ID_ENC_LET == id && p.ID_DET_BORD_LET.ToString() == bor.ID_DET_BORD).FirstOrDefault();
                bor.MONT_OUV_DET_BORD = bor.MONT_OUV_DET_BORD - lettraje2.MONT_TTC_LET;
                bor.MONT_FDG_DET_BORD = TX_FDG * (decimal)bor.MONT_OUV_DET_BORD;
                db.Entry(bor).State = EntityState.Modified;
                db.SaveChanges();
            }
            TempData["messagee"] = "la réconciliation a été effectué avec succes";
            TempData["tab3"] = "active";
            return RedirectToAction("LettrageEtEncaissement");
        }
        /**********************JSON rec *********************/
        public ActionResult RéconsiliationJson(int id)
        {
            List<TJ_LETTRAGE> lettraje = db.TJ_LETTRAGE.Where(p => p.ID_ENC_LET == id).ToList();

            string typeb = "";
            foreach (var item in lettraje)
            {
                item.VALIDE_RECONCIL = true;
                item.DAT_RECONCIL = DateTime.Now;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                T_DET_BORD bor = db.T_DET_BORD.Where(p => p.ID_DET_BORD == item.ID_DET_BORD_LET.ToString()).FirstOrDefault();

                switch (bor.TYP_DET_BORD.ToString().Trim())
                {
                    case "fact": typeb = "F"; break;
                    case "Caut": typeb = "C"; break;
                    case "bdc": typeb = "BC"; break;
                    case "march": typeb = "M"; break;
                }
               // T_FOND_GARANTIE f = db.T_FOND_GARANTIE.Where(p => p.REF_CTR_FDG == bor.REF_CTR_DET_BORD && p.TYP_FDG == typeb).SingleOrDefault();
                decimal TX_FDG = (decimal)bor.TX_FDG_DET_BORD;
                TX_FDG = TX_FDG / 100;
                TJ_LETTRAGE lettraje2 = db.TJ_LETTRAGE.Where(p => p.ID_ENC_LET == id && p.ID_DET_BORD_LET.ToString() == bor.ID_DET_BORD).FirstOrDefault();
                decimal fdganc = (decimal)bor.MONT_FDG_DET_BORD;
                if (bor.MONT_OUV_DET_BORD - lettraje2.MONT_TTC_LET < 0)
                {
                    bor.MONT_OUV_DET_BORD = 0;
                    bor.MONT_FDG_DET_BORD = TX_FDG * (decimal)bor.MONT_OUV_DET_BORD;
                    bor.MONT_FDG_DET_BORD = Math.Round((decimal)bor.MONT_FDG_DET_BORD, 3);
                    bor.MONT_FDG_LIBERE_DET_BORD = bor.MONT_FDG_LIBERE_DET_BORD + (fdganc - bor.MONT_FDG_DET_BORD);
                    db.Entry(bor).State = EntityState.Modified;
                }
                else
                {
                    bor.MONT_OUV_DET_BORD = bor.MONT_OUV_DET_BORD - lettraje2.MONT_TTC_LET;

                    bor.MONT_FDG_DET_BORD = TX_FDG * (decimal)bor.MONT_OUV_DET_BORD;
                    bor.MONT_FDG_DET_BORD = Math.Round((decimal)bor.MONT_FDG_DET_BORD, 3);
                    bor.MONT_FDG_LIBERE_DET_BORD = bor.MONT_FDG_LIBERE_DET_BORD+( fdganc - bor.MONT_FDG_DET_BORD);
                    db.Entry(bor).State = EntityState.Modified;
                }
               
                db.SaveChanges();
            }

            try {
                if (db.All_Ecran_Financements(db.T_ENCAISSEMENT.Find(id).REF_CTR_ENC).First().Depass_Limite > 0)
                {
                    T_EXTRAIT extrait = new T_EXTRAIT();
                    extrait.REF_CTR_EXTRAIT = db.T_ENCAISSEMENT.Find(id).REF_CTR_ENC;
                    extrait.LIB_EXTRAIT = "Depassement ";
                    extrait.DEBIT_EXTRAIT = 0;
                    extrait.CREDIT_EXTRAIT = 0;
                    extrait.AUTRE_EXTRAIT = db.All_Ecran_Financements(db.T_ENCAISSEMENT.Find(id).REF_CTR_ENC).First().Depass_Limite;
                    extrait.ENCOURFACT_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Encours_Facture).FirstOrDefault();
                    extrait.DISPONIBLE_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Disponible).FirstOrDefault();
                    extrait.DAT_OP_EXTRAIT = DateTime.Now;
                    extrait.DAT_VAL_EXTRAIT = DateTime.Now;
                    extrait.TOTAL_FIN_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Total_Financement).FirstOrDefault();
                    db.T_EXTRAIT.Add(extrait);
                    db.SaveChanges();

                }

            } catch (Exception e) { }
     


            try
            {
                T_EXTRAIT extrait = new T_EXTRAIT();
                extrait.REF_CTR_EXTRAIT = db.T_ENCAISSEMENT.Find(id).REF_CTR_ENC;
                extrait.LIB_EXTRAIT = "Réconsiliation-"+ id;
                extrait.DEBIT_EXTRAIT = db.TJ_LETTRAGE.Where(p => p.ID_ENC_LET == id).Sum(p => p.MONT_TTC_LET);
                extrait.CREDIT_EXTRAIT = 0;
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


            try
            {
                T_HISTORIQUE histo = new T_HISTORIQUE();
                histo.ABREV_ROLE_HIST = "Réconsiliation " + db.T_ENCAISSEMENT.Find(id).REF_CTR_ENC;
                histo.ACTION = "Réconsiliation";
                histo.ID_ENREGISTREMENT = db.T_ENCAISSEMENT.Find(id).ID_ENC.ToString();
                histo.T_TABLE = "TJ_LETTRAGE";
                histo.REF_CTR_HIST = db.T_ENCAISSEMENT.Find(id).REF_CTR_ENC.ToString();
                histo.REF_IND_HIST = db.T_ENCAISSEMENT.Find(id).REF_ACH_ENC.ToString();
                histo.LOGIN_USER = Session["UserLogin"].ToString();
                histo.IP_PC = HttpContext.Request.UserHostAddress;
                histo.NOM_PC = HttpContext.Request.UserHostName;
                histo.DATE_ACTION = DateTime.Now;
                db.T_HISTORIQUE.Add(histo);
                db.SaveChanges();
            }
            catch (Exception) { }

            TempData["messagee"] = "la réconciliation a été effectué avec succes";
            //TempData["tab3"] = "active";
            var j = new {
                type = "save",
                messagee = TempData["messagee"]
            };
            return Json(j, JsonRequestBehavior.AllowGet);
        }
        /*************************FIN*****************************/

        /***********************IMPAYE JSON *********************************/

        public ActionResult ImpayeJson(int id)
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
            T_ENCAISSEMENT enc = db.T_ENCAISSEMENT.Where(o => o.ID_ENC == id).First();
            if (enc.TYP_ENC != "A" && enc.TYP_ENC != "V")
            {
                T_IMPAYE imp = new T_IMPAYE();
                imp.ID_ENC_IMP = id;
                imp.DATE_IMP = DateTime.Now;
                imp.DATE_SAISI_IMP = DateTime.Now;
                imp.MONT_IMP = enc.MONT_ENC;
                db.T_IMPAYE.Add(imp);
                db.SaveChanges();
                List<TJ_LETTRAGE> lettraje = db.TJ_LETTRAGE.Where(p => p.ID_ENC_LET == id).ToList();
                db.Anuuler_rec_t_det_bord(imp.ID_ENC_IMP);
                try
                {
                    T_FRAIS_DIVERS f = db.T_FRAIS_DIVERS.Where(p => p.TYP_FRAIS_DIVERS.Contains("Imp") && p.REF_CTR_FRAIS_DIVERS == enc.REF_CTR_ENC).SingleOrDefault();

                    T_MVT_DEBIT debit = new T_MVT_DEBIT();
                    debit.REF_CTR_DEBIT = enc.REF_CTR_ENC;
                    debit.ABEV_DEBIT = "Impaye";
                    if (tva != null)
                    {
                        debit.MONT_DEBIT = f.MONT_PAR_UNIT_FRAIS_DIVERS * tva.VAL_TVA;
                    }
                    else
                    {
                        debit.MONT_DEBIT = f.MONT_PAR_UNIT_FRAIS_DIVERS;
                    }
                   
                    debit.DATE_DEBIT = DateTime.Today;
                    db.T_MVT_DEBIT.Add(debit);
                    db.SaveChanges();


                    try
                    {
                        T_EXTRAIT extrait = new T_EXTRAIT();
                        extrait.REF_CTR_EXTRAIT = debit.REF_CTR_DEBIT;
                        extrait.LIB_EXTRAIT = "Debit " + debit.ABEV_DEBIT;
                        extrait.DEBIT_EXTRAIT = debit.MONT_DEBIT;
                        extrait.CREDIT_EXTRAIT = 0;
                        extrait.AUTRE_EXTRAIT = 0;
                        extrait.ENCOURFACT_EXTRAIT = db.All_Ecran_Financements(debit.REF_CTR_DEBIT).Select(p => p.Encours_Facture).FirstOrDefault();
                        extrait.DISPONIBLE_EXTRAIT = db.All_Ecran_Financements(debit.REF_CTR_DEBIT).Select(p => p.Disponible).FirstOrDefault();
                        extrait.DAT_OP_EXTRAIT = DateTime.Now;
                        extrait.DAT_VAL_EXTRAIT = debit.DATE_DEBIT;
                        extrait.TOTAL_FIN_EXTRAIT = db.All_Ecran_Financements(debit.REF_CTR_DEBIT).Select(p => p.Total_Financement).FirstOrDefault();
                        db.T_EXTRAIT.Add(extrait);
                        db.SaveChanges();

                    }
                    catch (Exception) { }

                    try
                    {
                        T_HISTORIQUE histo = new T_HISTORIQUE();
                        histo.ABREV_ROLE_HIST = "Debit " + debit.ABEV_DEBIT;
                        histo.ACTION = "Creation Debit";
                        histo.ID_ENREGISTREMENT = db.T_MVT_DEBIT.Max().ToString();
                        histo.T_TABLE = "T_MVT_DEBIT";
                        histo.REF_CTR_HIST = debit.REF_CTR_DEBIT.ToString();
                        histo.REF_IND_HIST = db.TJ_CIR.Where(p => p.REF_CTR_CIR == debit.REF_CTR_DEBIT && p.ID_ROLE_CIR == "ADH").FirstOrDefault().REF_IND_CIR.ToString();
                        histo.LOGIN_USER = Session["UserLogin"].ToString();
                        histo.IP_PC = HttpContext.Request.UserHostAddress;
                        histo.NOM_PC = HttpContext.Request.UserHostName;
                        histo.DATE_ACTION = DateTime.Now;
                        db.T_HISTORIQUE.Add(histo);
                        db.SaveChanges();
                    }
                    catch (Exception) { }
                }
                catch (Exception e)
                {

                }
                TempData["messagee"] = "l encaissement " + enc.REF_ENC + "a ete enregistrer comme impaye ";
                // TempData["tab3"] = "active";
                var j = new {
                    type="save",
                    messagee= TempData["messagee"]
                };
                return Json(j,JsonRequestBehavior.AllowGet);
            }
            else
            {
                string text = "";
                if (
                enc.TYP_ENC == "A") { text = "avoir"; }
                else if (enc.TYP_ENC == "V") { text = "virement"; }
                TempData["error"] = "0x963524 type encaissemnt (" + text + ")";
                var j = new
                {
                    type = "error",
                    messagee = TempData["error"]
                };
                return Json(j, JsonRequestBehavior.AllowGet);
              
            }
        }



        /****************************FIN*************************/


        public ActionResult Impaye(int id)
        {

            T_ENCAISSEMENT enc = db.T_ENCAISSEMENT.Where(o => o.ID_ENC == id).First();
            if (enc.TYP_ENC != "A" && enc.TYP_ENC != "V")
            {
                T_IMPAYE imp = new T_IMPAYE();
                imp.ID_ENC_IMP = id;
                imp.DATE_IMP = DateTime.Now;
                imp.DATE_SAISI_IMP = DateTime.Now;
                imp.MONT_IMP = enc.MONT_ENC;
                db.T_IMPAYE.Add(imp);
                db.SaveChanges();
                List<TJ_LETTRAGE> lettraje = db.TJ_LETTRAGE.Where(p => p.ID_ENC_LET == id).ToList();
                db.Anuuler_rec_t_det_bord(imp.ID_ENC_IMP);
                try
                {
                    T_FRAIS_DIVERS f = db.T_FRAIS_DIVERS.Where(p => p.TYP_FRAIS_DIVERS.Contains("Imp") && p.REF_CTR_FRAIS_DIVERS == enc.REF_CTR_ENC).SingleOrDefault();

                    T_MVT_DEBIT debit = new T_MVT_DEBIT();
                    debit.REF_CTR_DEBIT = enc.REF_CTR_ENC;
                    debit.ABEV_DEBIT = "Impaye";
                    debit.MONT_DEBIT = f.MONT_PAR_UNIT_FRAIS_DIVERS;
                    debit.DATE_DEBIT = DateTime.Today;
                    db.T_MVT_DEBIT.Add(debit);
                    db.SaveChanges();


                    try
                    {
                        T_EXTRAIT extrait = new T_EXTRAIT();
                        extrait.REF_CTR_EXTRAIT = debit.REF_CTR_DEBIT;
                        extrait.LIB_EXTRAIT = "Debit " + debit.ABEV_DEBIT;
                        extrait.DEBIT_EXTRAIT = debit.MONT_DEBIT;
                        extrait.CREDIT_EXTRAIT = 0;
                        extrait.AUTRE_EXTRAIT = 0;
                        extrait.ENCOURFACT_EXTRAIT = db.All_Ecran_Financements(debit.REF_CTR_DEBIT).Select(p => p.Encours_Facture).FirstOrDefault();
                        extrait.DISPONIBLE_EXTRAIT = db.All_Ecran_Financements(debit.REF_CTR_DEBIT).Select(p => p.Disponible).FirstOrDefault();
                        extrait.DAT_OP_EXTRAIT = DateTime.Now;
                        extrait.DAT_VAL_EXTRAIT = debit.DATE_DEBIT;
                        extrait.TOTAL_FIN_EXTRAIT = db.All_Ecran_Financements(debit.REF_CTR_DEBIT).Select(p => p.Total_Financement).FirstOrDefault();
                        db.T_EXTRAIT.Add(extrait);
                        db.SaveChanges();

                    }
                    catch (Exception) { }


                    try
                    {
                        T_HISTORIQUE histo = new T_HISTORIQUE();
                        histo.ABREV_ROLE_HIST = "Debit " + debit.ABEV_DEBIT;
                        histo.ACTION = "Creation Debit";
                        histo.ID_ENREGISTREMENT = db.T_MVT_DEBIT.Max().ToString();
                        histo.T_TABLE = "T_MVT_DEBIT";
                        histo.REF_CTR_HIST = debit.REF_CTR_DEBIT.ToString();
                        histo.REF_IND_HIST = db.TJ_CIR.Where(p => p.REF_CTR_CIR == debit.REF_CTR_DEBIT && p.ID_ROLE_CIR == "ADH").FirstOrDefault().REF_IND_CIR.ToString();
                        histo.LOGIN_USER = Session["UserLogin"].ToString();
                        histo.IP_PC = HttpContext.Request.UserHostAddress;
                        histo.NOM_PC = HttpContext.Request.UserHostName;
                        histo.DATE_ACTION = DateTime.Now;
                        db.T_HISTORIQUE.Add(histo);
                        db.SaveChanges();
                    }
                    catch (Exception) { }

                }
                catch (Exception e)
                {

                }
                TempData["messagee"] = "l encaissement " + enc.REF_ENC + "a ete enregistrer comme impaye ";
                TempData["tab3"] = "active";
                return RedirectToAction("LettrageEtEncaissement");
            }
            else
            {
                string text="";
                if (
                enc.TYP_ENC == "A") { text = "avoir"; }
                else if (enc.TYP_ENC == "V") { text = "virement"; }
                TempData["error"] = "0x963524 type encaissemnt (" + text + ")"; ;
                return RedirectToAction("InternalServerError", "Error");
            }
        }




        public ActionResult RechercheMaxBordParCtr(int id)
        {
            int max;
            try {
                var annne = db.T_BORDEREAU.Where(p => p.REF_CTR_BORD == id).Max(p => p.ANNEE_BORD);
                string query = "select max(CONVERT(INT, num_bord)) from t_bordereau where ref_ctr_bord="+ id;
                max = db.Database.SqlQuery<int>(query).SingleOrDefault();
              } catch (Exception) { max = 000; }


                return Json(max+1, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SelectMntEncImpaye(int id, int id2)
        {
            decimal montant = 0;
            try
            {
                montant = (decimal)db.T_ENCAISSEMENT.Where(p=>p.ID_ENC==id).Select(p => p.MONT_ENC).SingleOrDefault();
            }
            catch (Exception) { montant = 0; }
            return Json(montant, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Verife_Facture(int id, string id2)
        {
            List<TJ_DOCUMENT_DET_BORD> docdetbord2 = new List<TJ_DOCUMENT_DET_BORD>();
            try {  docdetbord2 = db.TJ_DOCUMENT_DET_BORD.Where(p => p.REF_CTR_DET_BORD == id && p.REF_DOCUMENT_DET_BORD == id2).ToList(); } catch (Exception) { docdetbord2 = null; }
            bool verife;
            if (docdetbord2 != null && docdetbord2.Count > 0)
            {
                verife = true;
            }
            else
            {
                verife = false;
            }
            return Json(false, JsonRequestBehavior.AllowGet);

        }

        public ActionResult SelectRefSeq(string id)
        {
            string type_enc = id;
            if (id.Length >= 2)
            {
                id = "T";
            }
                string query = "SELECT RIGHT(CONVERT(varchar(6), 100001 + COUNT(*)), 5) FROM  T_ENCAISSEMENT WHERE  (TYP_ENC ='" + id + "') AND (YEAR(DAT_RECEP_ENC) = YEAR(GETDATE()))";
            var nomber = db.Database.SqlQuery<string>(query).FirstOrDefault();
            string ref_seq = (DateTime.Now.Year - 2000).ToString();
            
            switch (id)
            {
                case "C":
                   
                    ref_seq = ref_seq + "77";
                    //Console.WriteLine("Case 1");
                    break;
                case "T":
                
                    ref_seq = ref_seq + "99";
                    break;
                case "A":
                   
                    ref_seq = ref_seq + "88";
                    break;
                case "V":
                    ref_seq = ref_seq + "10";
                  
                    // System.Windows.Data.CollectionViewSource T_EC_CPTViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("t_EC_CPTViewSource")));
                    //T_EC_CPTViewSource.View.MoveCurrentToFirst();
                    break;
                case "E":
                  
                    ref_seq = ref_seq + "00";
                    break;
                case "B":
                  
                    // System.Windows.Data.CollectionViewSource T_EC_CPTViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("t_EC_CPTViewSource")));
                    //T_EC_CPTViewSource.View.MoveCurrentToFirst();
                    break;
                default:
                    //MessageBox.Show("Impossible d'annuler ce type d'encaissements");
                    break;
            }
            int counter = 0;
            if (type_enc == "TR")
            {
               counter = int.Parse(nomber); counter += 50000;
                // nombreTextBox.Text = counter.ToString();
                ref_seq = ref_seq + counter.ToString();
            }
            else { ref_seq = ref_seq + nomber; }
            return Json(ref_seq, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Bordereau_cheque()
        {
            ViewBag.ListDesEnc = db.GETENC02122019().ToList();
            TempData["total_c"] = db.GETENC02122019().Where(p => p.TYP_ENC == "C").Count();
            TempData["total_TR"] = db.GETENC02122019().Where(p => p.TYP_ENC == "T" && p.REF_ENC.Length>=8).Count();
            TempData["total_TC"] = db.GETENC02122019().Where(p => p.TYP_ENC == "T" && p.REF_ENC.Length<=7).Count();
            TempData["total_IP"] = db.GETENC02122019().Count();
            TempData["Bordereau"] = "active";
            TempData["Bordereau_cheque"] = "active";

            return View();
        }


        public ActionResult Ajouter_Bord_Ch_Jsoon(List<Bord_Ch> Bordereau_ch)
        {
            if (db.T_BORD_FACTOR.Where(p => p.VALID_BORD_FACTOR == false).Count() == 0)
            {
               
                try
                {
                    int c = db.T_BORD_FACTOR.GroupBy(p => p.NUM_BORD_FACTOR).Count();
                    foreach (var item in Bordereau_ch)
                    {
                        T_BORD_FACTOR bord_fact = new T_BORD_FACTOR();
                        bord_fact.REF_CTR_BORD_FACTOR = int.Parse(item.ref_ctr_enc);
                        bord_fact.REF_ENC_BORD_FACTOR = item.ref_enc;
                        bord_fact.NUM_BORD_FACTOR = (c+ 1) +""+ DateTime.Now.Year;
                        bord_fact.VALID_BORD_FACTOR = false;
                        string v = item.mnt.Replace("&nbsp;", "");
                        bord_fact.MONT_TOT_BORD_FACTOR = decimal.Parse(v);
                        bord_fact.ID_ENC_BORD_FACTOR = item.id_enc;
                        bord_fact.ID_FACTOR_BORD_FACTOR = int.Parse(item.id_banque);
                        bord_fact.DAT_CRE_BORD_FACTOR = DateTime.Now;
                        db.T_BORD_FACTOR.Add(bord_fact);
                        db.SaveChanges();
                    }
                    TempData["notice"] = "save";
                    return Json("save", JsonRequestBehavior.AllowGet);
                 
                 //   return RedirectToAction("Bordereau_cheque");
                }
                catch (Exception e)
                {
                    return Json(e.Message, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("il y a une bordereau n est pas valide", JsonRequestBehavior.AllowGet);

            }


        

        //    return View();
        }
        public ActionResult Reconsiliation()
        {
            ViewBag.Lettrage = db.Lettrage_Non_Reconsilier().ToList();
            return PartialView();
        }

        public ActionResult Lettrage()
        {
            ViewBag.Lettrage_Let = db.Lettrage_Non_Reconsilier().ToList();
            return PartialView();
        }


        public ActionResult ValiderBordereaucheque()
        {

            //ViewBag.ListeDesEncAvalide = db.T_BORD_FACTOR.Where(p => p.VALID_BORD_FACTOR == false).ToList();

            Entet_Bord_Ch entet_bord_ch = new Entet_Bord_Ch();
            TempData["Nom_Factor"]  = db.T_FACTOR.FirstOrDefault().RAISON_SOCIAL;
            TempData["Nomber_Ch"]  = db.T_BORD_FACTOR.Where(p => p.VALID_BORD_FACTOR == false).Count();
            TempData["TotalMnt"]  = (decimal)db.T_BORD_FACTOR.Where(p => p.VALID_BORD_FACTOR == false).Sum(p => p.MONT_TOT_BORD_FACTOR);
            DateTime x = (DateTime)db.T_BORD_FACTOR.Where(p => p.VALID_BORD_FACTOR == false).FirstOrDefault().DAT_CRE_BORD_FACTOR;
            TempData["Num_bord"] = db.T_BORD_FACTOR.Where(p => p.VALID_BORD_FACTOR == false).FirstOrDefault().NUM_BORD_FACTOR;
            TempData["Date_Creation"] = x.ToString("d");
            return PartialView();
        }


        public ActionResult ListeDesBordereauCHeques()
        {
            List<Bord_Ch> ListeDesBord = new List<Bord_Ch>();
            //ViewBag.ListeDesEncAvalide = db.T_BORD_FACTOR.Where(p => p.VALID_BORD_FACTOR == false).ToList();
            var bord = db.T_BORD_FACTOR.Where(p => p.VALID_BORD_FACTOR == false).ToList();
            TempData["id_banque"] = db.T_BORD_FACTOR.Where(p => p.VALID_BORD_FACTOR == false).FirstOrDefault().ID_FACTOR_BORD_FACTOR;
            int id_enc = (int)db.T_BORD_FACTOR.Where(p => p.VALID_BORD_FACTOR == false).FirstOrDefault().ID_ENC_BORD_FACTOR;

            if ((db.T_ENCAISSEMENT.Find(id_enc).TYP_ENC == "T" || db.T_ENCAISSEMENT.Find(id_enc).TYP_ENC == "TR") && db.T_ENCAISSEMENT.Find(id_enc).REF_ENC.Length == 7)
            {
                TempData["id_type"] = 3;
            }
            else
 if ((db.T_ENCAISSEMENT.Find(id_enc).TYP_ENC == "T" || db.T_ENCAISSEMENT.Find(id_enc).TYP_ENC == "TR") && db.T_ENCAISSEMENT.Find(id_enc).REF_ENC.Length == 12)
            {
                TempData["id_type"] = 1;
            }
            else

            {
                TempData["id_type"] = 3;
            }
            foreach (var item in bord)

            {
                Bord_Ch bordch = new Bord_Ch();
                bordch.ref_enc = item.REF_ENC_BORD_FACTOR;
                bordch.ref_ctr_enc = item.REF_CTR_BORD_FACTOR.ToString();
                bordch.ach = db.T_INDIVIDU.Find(db.T_ENCAISSEMENT.Find(item.ID_ENC_BORD_FACTOR).REF_ACH_ENC).NOM_IND;
             
                if (db.T_ENCAISSEMENT.Find(item.ID_ENC_BORD_FACTOR).TYP_ENC =="T" && db.T_ENCAISSEMENT.Find(item.ID_ENC_BORD_FACTOR).REF_ENC.Length==7)
                {
                    bordch.type = "TC";
                }
                else
                if (db.T_ENCAISSEMENT.Find(item.ID_ENC_BORD_FACTOR).TYP_ENC == "T" && db.T_ENCAISSEMENT.Find(item.ID_ENC_BORD_FACTOR).REF_ENC.Length == 12)
                {
                    bordch.type = "TR";
                }
                else
              
                {
                    bordch.type = "C";
                }
                bordch.mnt = item.MONT_TOT_BORD_FACTOR.ToString();
                ListeDesBord.Add(bordch);
            }

             ViewBag.ListeBord = ListeDesBord.ToList();
            return PartialView();
        }


        public ActionResult GenereBordCh(int id)
        {
            try
            {
                foreach (var item in db.T_BORD_FACTOR.Where(p => p.VALID_BORD_FACTOR == false).ToList())
                {
                    T_ENCAISSEMENT enc = db.T_ENCAISSEMENT.Find(item.ID_ENC_BORD_FACTOR);
                    enc.BORD_ENC = item.NUM_BORD_FACTOR;
                    T_BORD_FACTOR factor = db.T_BORD_FACTOR.Find(item.ID_BORD_FACTOR);
                    factor.VALID_BORD_FACTOR = true;
                    factor.DAT_EDIT_BORD_FACTOR = DateTime.Now;
                    factor.ID_FACTOR_BORD_FACTOR = id;
                    db.Entry(enc).State = EntityState.Modified;
                    db.Entry(factor).State = EntityState.Modified;
                }
                db.SaveChanges();
                return Json("True", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("False", JsonRequestBehavior.AllowGet);
            }
          //  return RedirectToAction("Bordereau_cheque");

        }



        public ActionResult Bordereau_CH_Rapport(string id1,int id2,int id3)
        {
            string id = "PDF";
        
            LocalReport lr = new LocalReport();
            string path = "";
            decimal mnt_total = (decimal)db.ProcedureBordereauCh1(id1).Sum(p => p.MONT_ENC);
            int cc = (int)db.ProcedureBordereauCh1(id1).Sum(p => p.MONT_ENC);
            int dec = (int)((mnt_total - cc) * 1000);
            string deviceInfo = "";
            string agence="";
            string codeAgenceBanque="";
            string numbord="";
            string montantEnlettres = "";
            if (id2 == 1)
            {
                codeAgenceBanque = db.T_RIB_FACTOR.Where(p => p.ID_FACTOR == id2).FirstOrDefault().RIB_FACTOR.Substring(0, 5);
                agence = db.TR_Ag_Bq.Where(p => p.Code_Bq_Ag == codeAgenceBanque).FirstOrDefault().Agence;
                numbord = id1.Substring(0, id1.Length - 4) + "/" + id1.Substring((id1.Length - 4), 4) + codeAgenceBanque;

                montantEnlettres = "Arrêté à la somme de: " + NumberToWords((int)mnt_total);
              
                    if (db.ProcedureBordereauCh1(id1).FirstOrDefault().TYP_ENC == "TR" || db.ProcedureBordereauCh1(id1).FirstOrDefault().TYP_ENC == "T" && id3 == 1)
                    {
                        path = Path.Combine(Server.MapPath("~/Report"), "Borderea de remise AMEN BANK.rdlc");
                    }

              else  if (db.ProcedureBordereauCh1(id1).FirstOrDefault().TYP_ENC == "TR" || db.ProcedureBordereauCh1(id1).FirstOrDefault().TYP_ENC == "T" && id3 == 2)
                {
                    path = Path.Combine(Server.MapPath("~/Report"), "Borderea de remise AMEN BANK.rdlc");
                }
                else 
                {
                    path = Path.Combine(Server.MapPath("~/Report"), "Copy of Remise de cheques amen bank.rdlc");
                }


                 deviceInfo =

                "<DeviceInfo>" +
                "  <OutputFormat>" + id + "</OutputFormat>" +
                "  <PageWidth>9.5in</PageWidth>" +
                "  <PageHeight>11in</PageHeight>" +
                "  <MarginTop>0.5in</MarginTop>" +
                "  <MarginLeft>1in</MarginLeft>" +
                "  <MarginRight>1in</MarginRight>" +
                "  <MarginBottom>0.5in</MarginBottom>" +
                "</DeviceInfo>";
            }
            if (id2 == 3)
            {
                codeAgenceBanque = db.T_RIB_FACTOR.Where(p => p.ID_FACTOR == id2).FirstOrDefault().RIB_FACTOR.Substring(0, 5);
                agence = db.TR_Ag_Bq.Where(p => p.Code_Bq_Ag == codeAgenceBanque).FirstOrDefault().Agence;
                numbord = id1.Substring(0, id1.Length - 4) + "/" + id1.Substring((id1.Length - 4), 4) + codeAgenceBanque;
                montantEnlettres = "Arrêté à la somme de: " + NumberToWords((int)mnt_total);
                if (db.ProcedureBordereauCh1(id1).FirstOrDefault().TYP_ENC == "TR" || db.ProcedureBordereauCh1(id1).FirstOrDefault().TYP_ENC == "T" && id3==1)

                {
                    //path = Path.Combine(Server.MapPath("~/Report"), "Atijeri.rdlc");
                     path = Path.Combine(Server.MapPath("~/Report"), "Atijeri.rdlc");
                }
                else 
                if (db.ProcedureBordereauCh1(id1).FirstOrDefault().TYP_ENC == "TR" || db.ProcedureBordereauCh1(id1).FirstOrDefault().TYP_ENC == "T" && id3 == 2)
                {
                    path = Path.Combine(Server.MapPath("~/Report"), "Atijeri.rdlc");
                }
                else
                {
                    path = Path.Combine(Server.MapPath("~/Report"), "Atijeri - REMISE_EFFET.rdlc");
                }


                deviceInfo =

                "<DeviceInfo>" +
                "  <OutputFormat>" + id + "</OutputFormat>" +
                "  <PageWidth>14in</PageWidth>" +
                "  <PageHeight>11in</PageHeight>" +
                "  <MarginTop>0.5in</MarginTop>" +
                "  <MarginLeft>0.5in</MarginLeft>" +
                "  <MarginRight>0.5in</MarginRight>" +
                "  <MarginBottom>0in</MarginBottom>" +
                "</DeviceInfo>";

            }

            if (id2 == 2)
            {
                codeAgenceBanque = db.T_RIB_FACTOR.Where(p => p.ID_FACTOR == id2).FirstOrDefault().RIB_FACTOR.Substring(0, 5);
                agence = db.TR_Ag_Bq.Where(p => p.Code_Bq_Ag == codeAgenceBanque).FirstOrDefault().Agence;
                numbord = id1.Substring(0, id1.Length - 4) + "/" + id1.Substring((id1.Length - 4), 4) + codeAgenceBanque;
                montantEnlettres = "Arrêté à la somme de: " + NumberToWords((int)mnt_total);
                if (db.ProcedureBordereauCh1(id1).FirstOrDefault().TYP_ENC == "TR" || db.ProcedureBordereauCh1(id1).FirstOrDefault().TYP_ENC == "T" && id3==1)
                {
                    path = Path.Combine(Server.MapPath("~/Report"), "Remise d effets escompte en interets brut.rdlc");
                }
                else
                      if (db.ProcedureBordereauCh1(id1).FirstOrDefault().TYP_ENC == "TR" || db.ProcedureBordereauCh1(id1).FirstOrDefault().TYP_ENC == "T" && id3 == 2)
                {
                    path = Path.Combine(Server.MapPath("~/Report"), "Remise d effets escompte en interets brut.rdlc");
                }
                else
                {
                    path = Path.Combine(Server.MapPath("~/Report"), "Remise d Effets à l Encaissement UBCI.rdlc");
                }


                 deviceInfo =

                "<DeviceInfo>" +
                "  <OutputFormat>" + id + "</OutputFormat>" +
                "  <PageWidth>9.5in</PageWidth>" +
                "  <PageHeight>11in</PageHeight>" +
                "  <MarginTop>0.5in</MarginTop>" +
                "  <MarginLeft>1in</MarginLeft>" +
                "  <MarginRight>1in</MarginRight>" +
                "  <MarginBottom>0.5in</MarginBottom>" +
                "</DeviceInfo>";

            }

            if (id2 == 4)
            {
                codeAgenceBanque = db.T_RIB_FACTOR.Where(p => p.ID_FACTOR == id2).FirstOrDefault().RIB_FACTOR.Substring(0, 5);

                try { agence = db.TR_Ag_Bq.Where(p => p.Code_Bq_Ag == codeAgenceBanque).FirstOrDefault().Agence; } catch (Exception) { agence = ""; }
                numbord = id1.Substring(0, id1.Length - 4) + "/" + id1.Substring((id1.Length - 4), 4) + codeAgenceBanque;
                montantEnlettres = "Arrêté à la somme de: " + NumberToWords((int)mnt_total);
                if (db.ProcedureBordereauCh1(id1).FirstOrDefault().TYP_ENC == "TR" || db.ProcedureBordereauCh1(id1).FirstOrDefault().TYP_ENC == "T" && id3 == 1)
                {
                    path = Path.Combine(Server.MapPath("~/Report"), "ABCEffet.rdlc");

                    deviceInfo =
    "<DeviceInfo>" +
                "  <OutputFormat>" + id + "</OutputFormat>" +
                "  <PageWidth>14in</PageWidth>" +
                "  <PageHeight>16in</PageHeight>" +
                "  <MarginTop>0.5in</MarginTop>" +
                "  <MarginLeft>0.5in</MarginLeft>" +
                "  <MarginRight>0.5in</MarginRight>" +
                "  <MarginBottom>0in</MarginBottom>" +
                "</DeviceInfo>";
                }
                else
                if (db.ProcedureBordereauCh1(id1).FirstOrDefault().TYP_ENC == "TR" || db.ProcedureBordereauCh1(id1).FirstOrDefault().TYP_ENC == "T" && id3 == 2)
                {
                    path = Path.Combine(Server.MapPath("~/Report"), "ABCEffet.rdlc");

                    deviceInfo =
    "<DeviceInfo>" +
                "  <OutputFormat>" + id + "</OutputFormat>" +
                "  <PageWidth>14in</PageWidth>" +
                "  <PageHeight>16in</PageHeight>" +
                "  <MarginTop>0.5in</MarginTop>" +
                "  <MarginLeft>0.5in</MarginLeft>" +
                "  <MarginRight>0.5in</MarginRight>" +
                "  <MarginBottom>0in</MarginBottom>" +
                "</DeviceInfo>";
                }
                else
                {
                    path = Path.Combine(Server.MapPath("~/Report"), "ABCcheque.rdlc");


                    deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>15in</PageWidth>" +
            "  <PageHeight>11in</PageHeight>" +
            "  <MarginTop>0.5in</MarginTop>" +
            "  <MarginLeft>1in</MarginLeft>" +
            "  <MarginRight>1in</MarginRight>" +
            "  <MarginBottom>0.5in</MarginBottom>" +
            "</DeviceInfo>";
                }




            }


            if (id2 == 5)
            {
                codeAgenceBanque = db.T_RIB_FACTOR.Where(p => p.ID_FACTOR == id2).FirstOrDefault().RIB_FACTOR.Substring(0, 5);
                agence = db.TR_Ag_Bq.Where(p => p.Code_Bq_Ag == codeAgenceBanque).FirstOrDefault().Agence;
                numbord = id1.Substring(0, id1.Length - 4) + "/" + id1.Substring((id1.Length - 4), 4) + codeAgenceBanque;
                montantEnlettres = "Arrêté à la somme de: " + NumberToWords((int)mnt_total);

                if (db.ProcedureBordereauCh1(id1).FirstOrDefault().TYP_ENC == "TR" || db.ProcedureBordereauCh1(id1).FirstOrDefault().TYP_ENC == "T" && id3 == 1)
                {
                    path = Path.Combine(Server.MapPath("~/Report"), "BNA_Effet.rdlc");
                }
                else
                if (db.ProcedureBordereauCh1(id1).FirstOrDefault().TYP_ENC == "TR" || db.ProcedureBordereauCh1(id1).FirstOrDefault().TYP_ENC == "T" && id3 == 2)
                {
                    path = Path.Combine(Server.MapPath("~/Report"), "BNA_Effet.rdlc");
                }
                else
                {
                    path = Path.Combine(Server.MapPath("~/Report"), "BNA_CHeque.rdlc");
                }


                deviceInfo =

                "<DeviceInfo>" +
                "  <OutputFormat>" + id + "</OutputFormat>" +
                "  <PageWidth>14in</PageWidth>" +
                "  <PageHeight>9in</PageHeight>" +
                "  <MarginTop>0.5in</MarginTop>" +
                "  <MarginLeft>0.5in</MarginLeft>" +
                "  <MarginRight>0.5in</MarginRight>" +
                "  <MarginBottom>0in</MarginBottom>" +
                "</DeviceInfo>";

            }


            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }


            var cm = db.ProcedureBordereauCh1(id1).ToList();

            ReportDataSource rd = new ReportDataSource("DataSet1", cm);
    
                ReportParameter p1 = new ReportParameter("MontantEnLettres", montantEnlettres);

            ReportParameter p2 = new ReportParameter("millimes", "dinars et " + dec.ToString() + " millimes");
            ReportParameter p3 = new ReportParameter("agence", agence);
            ReportParameter p4 = new ReportParameter("numbord", numbord);
            lr.SetParameters(new ReportParameter[] { p1, p2,p3,p4 });

                       
            lr.DataSources.Add(rd);
            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;




            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            return File(renderedBytes, mimeType);
        }




        public ActionResult ListeDesBanque()
        {
            List<RibFactor> ListeDesRib = new List<RibFactor>();
            foreach (var item in db.T_RIB_FACTOR.Where(p=>p.VALID_RIB_FACTOR==true).ToList())

            {
                RibFactor rib = new RibFactor();

                rib.Nom_Banque = db.TR_Ag_Bq.Where(p => p.Code_Bq == item.RIB_FACTOR.Substring(0, 2)).FirstOrDefault().Banque;
                rib.Agence = db.TR_Ag_Bq.Where(p => p.Code_Ag == item.RIB_FACTOR.Substring(2, 3)).FirstOrDefault().Agence;
                rib.num_rib = item.RIB_FACTOR;
                rib.id = item.ID_FACTOR;
                ListeDesRib.Add(rib);

            }

            return Json(ListeDesRib, JsonRequestBehavior.AllowGet);
              

        }

        public ActionResult Annuler_Encaissement()
        {
            ViewBag.ListeEnc = db.procedure_Annulation_Encaissement().ToList();
            TempData["Parametrage"] = "active";
            TempData["Annuler_Encaissement"] = "active";
            return View();

        }

       

        public ActionResult Liste_Des_Bordereaux_CH()
        {
            ViewBag.ListeEnc = db.Liste_Des_Bordereaux_CH().ToList();
            TempData["Bordereau"] = "active";
            TempData["Liste_Des_Bordereaux_CH"] = "active";
            return View();

        }

        public ActionResult OpenBord(string id)
        {
            string rib = db.Liste_Des_Bordereaux_CH().Where(p => p.NUM_BORD_FACTOR.Contains(id)).SingleOrDefault().RIB;
            int id1 = db.T_RIB_FACTOR.Where(p => p.RIB_FACTOR == rib).FirstOrDefault().ID_FACTOR;
            string type = db.T_ENCAISSEMENT.Where(p => p.BORD_ENC.Contains(id)).FirstOrDefault().TYP_ENC;
            string enc = db.T_ENCAISSEMENT.Where(p => p.BORD_ENC == id).FirstOrDefault().REF_ENC;
            int id2 = 0;
            if (type == "TR" || type == "T" && enc.Length == 7)
            {
                id2 = 2;
            }
            else if (type == "TR" || type == "T" && enc.Length == 12)
            {
                id2 = 1;
            }
            else
            {
                id2 = 3;
            }

            return RedirectToAction("Bordereau_CH_Rapport",
                new
                {
                    id1 = id,
                    id2 = id1,
                    id3 = id2
                });

        }


        public ActionResult Annuler_Encaissement_Job(int id)
        {
            T_ENCAISSEMENT enc = db.T_ENCAISSEMENT.Find(id);
            enc.VALIDE_ENC = false;
            db.Entry(enc).State = EntityState.Modified;
            db.SaveChanges();
            /*
                        T_HISTORIQUE hist = new T_HISTORIQUE();
                        hist.ACTION = "Annuler encaissement";
                        hist.DATE_ACTION = DateTime.Now;
                        hist.
                        */

            try
            {
                T_HISTORIQUE histo = new T_HISTORIQUE();
                histo.ABREV_ROLE_HIST = "Annulation encaissement " + enc.REF_ENC;
                histo.ACTION = "Annuler encaissement";
                histo.ID_ENREGISTREMENT = enc.ID_ENC.ToString();
                histo.T_TABLE = "T_ENCAISSEMENT";
                histo.REF_CTR_HIST = enc.REF_CTR_ENC.ToString();
                histo.REF_IND_HIST = enc.REF_ACH_ENC.ToString();
                histo.LOGIN_USER = Session["UserLogin"].ToString();
                histo.IP_PC = HttpContext.Request.UserHostAddress;
                histo.NOM_PC = HttpContext.Request.UserHostName;
                histo.DATE_ACTION = DateTime.Now;
                db.T_HISTORIQUE.Add(histo);
                db.SaveChanges();
            }
            catch (Exception) { }

            TempData["notice"] = "save";
           
            return RedirectToAction("Annuler_Encaissement");

        }


        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zéro";

            if (number < 0)
                return "moins " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " mille ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " cent ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += " ";

                var unitsMap = new[] { "zéro",
"un",
"deux",
"trois",
"quatre",
"cinq",
"six",
"sept",
"huit",
"neuf",
" dix",
"onze",
"douze",
"treize",
"quatorze",
"quinze",
"seize",
"dix-sept",
"dix-huit",
"dix-neuf",
"vingt",
"vingt-et-un",
"vingt-deux",
"vingt-trois",
"vingt-quatre",
"vingt-cinq",
"vingt-six",
"vingt-sept",
"vingt-huit",
"vingt-neuf",
"trente",
"trente-et-un",
"trente-deux",
"trente-trois",
"trente-quatre",
"trente-cinq",
"trente-six",
"trente-sept",
"trente-huit",
"trente-neuf",
"quarante",
"quarante-et-un",
"quarante-deux",
"quarante-trois",
"quarante-quatre",
"quarante-cinq",
"quarante-six",
"quarante-sept",
"quarante-huit",
"quarante-neuf",
"cinquante",
"cinquante-et-un",
"cinquante-deux",
"cinquante-trois",
"cinquante-quatre",
"cinquante-cinq",
"cinquante-six",
"cinquante-sept",
"cinquante-huit",
"cinquante-neuf",
"soixante",
"soixante-et-un",
"soixante-deux",
"soixante-trois",
"soixante-quatre",
"soixante-cinq",
"soixante-six",
"soixante-sept",
"soixante-huit",
"soixante-neuf",
"soixante-dix",
"soixante-et-onze",
"soixante-douze",
"soixante-treize",
"soixante-quatorze",
"soixante-quinze",
"soixante-seize",
"soixante-dix-sept",
"soixante-dix-huit",
"soixante-dix-neuf",
"quatre-vingts",
"quatre-vingt-un",
"quatre-vingt-deux",
"quatre-vingt-trois",
"quatre-vingt-quatre",
"quatre-vingt-cinq",
"quatre-vingt-six",
"quatre-vingt-sept",
"quatre-vingt-huit",
"quatre-vingt-neuf",
"quatre-vingt-dix",
"quatre-vingt-onze",
"quatre-vingt-douze",
"quatre-vingt-treize",
"quatre-vingt-quatorze",
"quatre-vingt-quinze",
"quatre-vingt-seize",
"quatre-vingt-dix-sept",
"quatre-vingt-dix-huit",
"quatre-vingt-dix-neuf",
};

                var tensMap = new[] { "zero", "dix", "vingt", "trente", "quarente", "cinquante", "soixante", "soixante dix", "quatre vingt", "quatre vingt dix" };

                if (number < 100)
                    words += unitsMap[number];
                //else
                //{
                //    words += tensMap[number / 10];
                //    if ((number % 10) > 0)
                //        words += "-" + unitsMap[number % 10];
                //}
            }
            words.Replace("un cent", "cent");
            return words;
        }



        public ActionResult impaye_partiel()
        {
            ViewBag.ListImpaye = db.ListeDesImpayes();
            TempData["Impaye"] = "active";
            TempData["impaye_partiel"] = "active";
            return View();
        }


        public ActionResult Liste_Des_Encaissements(int id)
        {
            ViewBag.List = db.AnnulerLettrageParRefEnc(id);

            T_ENCAISSEMENT enc = db.T_ENCAISSEMENT.Find(id);
            T_INDIVIDU ind = db.T_INDIVIDU.Find(enc.REF_ADH_ENC);
            //
            ViewBag.Liste_enc = db.T_ENC_MATERIALISER_FillByRefAdh(ind.NOM_IND).ToList();
            TempData["Montant_Encaissement"] = enc.MONT_ENC;
            TempData["ID_Montant_Encaissement"] = enc.ID_ENC;
            return PartialView();
        }



        public ActionResult Annuler_Lettrage(int id, List<Impaye> info)
        {

            //// int v = 0;

            ////List<int> impayedistinct= info.Select(x=>x.id_enc).Distinct().ToList();

            ////for (int v = 0; v < impayedistinct.Count; v++)
            ////{
            ////    List<Impaye> thiss = info.Where(p => p.id_enc == impayedistinct[v]).ToList();
            ////    decimal mnt = 0m;
            ////    foreach (Impaye item in thiss)
            ////    {
            ////        mnt = (decimal)db.TJ_LETTRAGE.Where(p => p.ID_DET_BORD_LET == item.id_det_bord).Sum(p => p.MONT_TTC_LET);
            ////        if(mnt==)
            ////    }
            ////}






            ///**************** Dé Lettrage **********************/
            //List<TJ_LETTRAGE> liste_Des_Lettrages;
            //try
            //{
            //    liste_Des_Lettrages = db.TJ_LETTRAGE.Where(p => p.ID_ENC_LET == id).ToList();
            //}
            //catch (Exception e) { liste_Des_Lettrages = null; }

            //if (liste_Des_Lettrages.Count > 0)
            //{
            //foreach (TJ_LETTRAGE item in liste_Des_Lettrages)
            //{
            //    /**************** Ecriture inverse **********************/
            //    item.MONT_TTC_LET = item.MONT_TTC_LET * -1;
            //    db.Entry(item).State = EntityState.Modified;
            //    db.SaveChanges();
            //}

            //}

            ///**************** Fin  **********************/
            ///**************** New Lettrage **********************/






            //TJ_LETTRAGE lettarge = new TJ_LETTRAGE();
            //let.DAT_LET = DateTime.Now;
            //let.DAT_RECONCIL = DateTime.Now;
            //let.ID_DET_BORD_LET = item.id_det_bord;
            //let.MONT_TTC_LET = montant;
            //let.ID_ENC_LET = item.id_enc;
            //let.VALIDE_LET = true;
            //let.VALIDE_RECONCIL = false;
            //db.TJ_LETTRAGE.Add(let);
            //db.SaveChanges();
            //}



            //TempData["notice"] = "save";
            /**************** Fin  **********************/


//            List<TJ_LETTRAGE> Liste_tj = db.TJ_LETTRAGE.Where(p => p.ID_ENC_LET == id).ToList().AsReadOnly().ToList();

            List<TJ_LETTRAGE> Liste_tj =db.Database.SqlQuery<TJ_LETTRAGE>("select * from tj_lettrage where id_enc_let="+id).ToList();
            //  const List<TJ_LETTRAGE> Liste_tj2 =(List<TJ_LETTRAGE>)db.TJ_LETTRAGE.Where(p => p.ID_ENC_LET == id).ToList().DefaultIfEmpty();

            List<Models.Impaye> lise = new List<Models.Impaye>();
            Models.Impaye impay;
            foreach (var item in Liste_tj)
            {
                /**************** Ecriture inverse **********************/
                impay = new Models.Impaye();
                impay.id_enc = item.ID_DET_BORD_LET;
                impay.mnt_enc = (decimal)item.MONT_TTC_LET;
                lise.Add(impay);
            }





            TJ_LETTRAGE lettarge;
            List<TJ_LETTRAGE> listedeslettragenew = new List<TJ_LETTRAGE>();
            int i = 0;
            foreach (Impaye item in info)
            {
                T_ENCAISSEMENT enc = db.T_ENCAISSEMENT.Where(p => p.ID_ENC == item.id_enc && p.VALIDE_ENC == true).FirstOrDefault() ;
                decimal mnt = (decimal)enc.MONT_ENC;
                do
                {
                    foreach (var let in Liste_tj)
                    {
                      //  decimal mnt_let =(decimal) let.MONT_TTC_LET;

                        if (let.MONT_TTC_LET > 0 && mnt > 0)
                        {
                            if (let.MONT_TTC_LET > mnt)
                            {

                                lettarge = new TJ_LETTRAGE();
                                lettarge.DAT_LET = DateTime.Now;
                                lettarge.DAT_RECONCIL = DateTime.Now;
                                lettarge.ID_DET_BORD_LET = let.ID_DET_BORD_LET;
                                lettarge.MONT_TTC_LET = mnt;
                                lettarge.ID_ENC_LET = item.id_enc;
                                lettarge.VALIDE_LET = true;
                                lettarge.VALIDE_RECONCIL = false;

                                listedeslettragenew.Add(lettarge);

                                //db.TJ_LETTRAGE.Add(lettarge);
                                //db.SaveChanges();

                                let.MONT_TTC_LET = let.MONT_TTC_LET - mnt;
                                mnt = 0;

                                break;
                            }
                            else if (let.MONT_TTC_LET < mnt)
                            {
                                // let.MONT_TTC_LET = let.MONT_TTC_LET - enc.MONT_ENC;
                                lettarge = new TJ_LETTRAGE();
                                lettarge.DAT_LET = DateTime.Now;
                                lettarge.DAT_RECONCIL = DateTime.Now;
                                lettarge.ID_DET_BORD_LET = let.ID_DET_BORD_LET;
                                lettarge.MONT_TTC_LET = mnt;
                                lettarge.ID_ENC_LET = item.id_enc;
                                lettarge.VALIDE_LET = true;
                                lettarge.VALIDE_RECONCIL = false;
                                //db.TJ_LETTRAGE.Add(lettarge);
                                //db.SaveChanges();
                                listedeslettragenew.Add(lettarge);
                                mnt = mnt - (decimal)let.MONT_TTC_LET;
                                let.MONT_TTC_LET = 0;
                         

                            }
                            else
                            {
                                lettarge = new TJ_LETTRAGE();
                                lettarge.DAT_LET = DateTime.Now;
                                lettarge.DAT_RECONCIL = DateTime.Now;
                                lettarge.ID_DET_BORD_LET = let.ID_DET_BORD_LET;
                                lettarge.MONT_TTC_LET = let.MONT_TTC_LET;
                                lettarge.ID_ENC_LET = item.id_enc;
                                lettarge.VALIDE_LET = true;
                                lettarge.VALIDE_RECONCIL = false;
                                listedeslettragenew.Add(lettarge);
                                //db.TJ_LETTRAGE.Add(lettarge);
                                //db.SaveChanges();

                                let.MONT_TTC_LET = 0;
                                mnt = 0;
                                break;
                            }
                        }
                    }


                } while (mnt > 0);
            }

           List<TJ_LETTRAGE> Liste_tj2 = db.TJ_LETTRAGE.Where(p => p.ID_ENC_LET == id).ToList().AsReadOnly().ToList();


            foreach (TJ_LETTRAGE item in listedeslettragenew)
            {
                /**************** Ecriture inverse **********************/
                db.TJ_LETTRAGE.Add(item);
                db.SaveChanges();
            }




            string liste_des_nv_enc = "";
            foreach (Impaye item in info)
            {
                liste_des_nv_enc += item.id_enc + ",";
            }
                T_IMPAYE imp = db.T_IMPAYE.Where(p => p.ID_ENC_IMP == id).FirstOrDefault();
            imp.ID_NV_ENCS = liste_des_nv_enc;
            imp.IS_RESOLU = true;
            imp.DATE_RESOL_IMP = DateTime.Now;
            db.Entry(imp).State = EntityState.Modified;
            db.SaveChanges();


            foreach (TJ_LETTRAGE item in Liste_tj2)
            {
                /**************** Ecriture inverse **********************/


                lettarge = new TJ_LETTRAGE();
                lettarge.DAT_LET = DateTime.Now;
                lettarge.DAT_RECONCIL = DateTime.Now;
                lettarge.ID_DET_BORD_LET =item.ID_DET_BORD_LET;
                lettarge.MONT_TTC_LET = item.MONT_TTC_LET*-1;
                lettarge.ID_ENC_LET = item.ID_ENC_LET;
                lettarge.VALIDE_LET = true;
                lettarge.VALIDE_RECONCIL = true;
                db.TJ_LETTRAGE.Add(lettarge);
                db.SaveChanges();
                
                item.VALIDE_RECONCIL = true;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }





            try
            {
                T_HISTORIQUE histo = new T_HISTORIQUE();
                histo.ABREV_ROLE_HIST = "Résolution Impaye " + db.T_IMPAYE.Max(p=>p.ID_IMP);
                histo.ACTION = "Résolution Impaye ";
                histo.ID_ENREGISTREMENT = db.T_IMPAYE.Max(p => p.ID_IMP).ToString();
                histo.T_TABLE = "T_IMPAYE";
                histo.REF_CTR_HIST = db.T_ENCAISSEMENT.Find(imp.ID_ENC_IMP).REF_CTR_ENC.ToString();
                int x = db.T_ENCAISSEMENT.Find(imp.ID_ENC_IMP).REF_CTR_ENC;
                histo.REF_IND_HIST = db.TJ_CIR.Where(p => p.REF_CTR_CIR == x && p.ID_ROLE_CIR == "ADH").FirstOrDefault().REF_IND_CIR.ToString();
                histo.LOGIN_USER = Session["UserLogin"].ToString();
                histo.IP_PC = HttpContext.Request.UserHostAddress;
                histo.NOM_PC = HttpContext.Request.UserHostName;
                histo.DATE_ACTION = DateTime.Now;
                db.T_HISTORIQUE.Add(histo);
                db.SaveChanges();
            }
            catch (Exception) { }



            TempData["notice"] = "save";


            return Json(new { redirectToUrl = Url.Action("impaye_partiel", "Bordereau") });
        }


    }
}
    