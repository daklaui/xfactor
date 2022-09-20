using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using xfactor.Models;

namespace xfactor.Controllers
{
    public class FinancementController : Controller
    {
        // GET: Financement
        private XFactor_PRODEntities1 db = new XFactor_PRODEntities1();
        public ActionResult financement()
        {
            if (Session["UserLogin"] != null)
            {
                try
                {
                //    var ListNomInd = (from q in db.T_INDIVIDU
                //                      join q2 in db.TJ_CIR
                //                      on q.REF_IND equals q2.REF_IND_CIR
                //                      where (q2.ID_ROLE_CIR == "ADH" && q2.REF_CTR_CIR != 28 && q2.REF_CTR_CIR != 32 && q2.REF_CTR_CIR != 38 && q2.REF_CTR_CIR != 54 && q2.REF_CTR_CIR != 56 && q2.REF_CTR_CIR != 58 && q2.REF_CTR_CIR != 65 && q2.REF_CTR_CIR != 69 && q2.REF_CTR_CIR != 72 && q2.REF_CTR_CIR != 74 && q2.REF_CTR_CIR != 80 && q2.REF_CTR_CIR != 82 && q2.REF_CTR_CIR != 93 && q2.REF_CTR_CIR != 96)
                //                      select new { q.PRE_IND, q2.REF_CTR_CIR });
                //    List<NewDataCollection> op = new List<NewDataCollection>();
                //    foreach (var data in ListNomInd)
                //    {
                //        op.Add(new NewDataCollection(data.REF_CTR_CIR, data.PRE_IND));
                //    }

                    ViewBag.ListNomInd = db.Recherche_CTR_ADH().ToList();
                    ViewBag.NumFIn = db.T_FINANCEMENT.Where(p=>p.DAT_INSTR_FIN.Value.Year==DateTime.Now.Year).Count() + 1;

                    TempData["financement"] = "active";
                    TempData["financement1"] = "active";

                }
                   
             
                catch (Exception) { return RedirectToAction("InternalServerError", "Error"); }
                return View();
            }
          
            else
            {
                return RedirectToAction("login", "Login");
            }

        }
        [HttpPost]
        public ActionResult financement(T_FINANCEMENT fin,string  MONT_FIN, string TypeFin, string FRS)
        {

            if (FRS!=null)
            {


                fin.REF_ADH_FIN = int.Parse(FRS);
            }
            else
            {

                fin.REF_ADH_FIN = db.All_Ecran_Financements(fin.REF_CTR_FIN).First().REF_IND;
            }

            try{
            MONT_FIN = MONT_FIN.Replace(".", ",");
            fin.MONT_FIN = decimal.Parse(MONT_FIN);
            string id_user = Session["ID_USER"].ToString();
            fin.ID_FIN = db.T_FINANCEMENT.Max(p => p.ID_FIN)+1;
          
                string ref_instr = fin.REF_INSTR_FIN;
            fin.REF_INSTR_FIN = DateTime.Now.Year + "" + fin.REF_INSTR_FIN;
            fin.DAT_INSTR_FIN = DateTime.Now;
        
          
            try
            {
                if (TypeFin.Equals("FinRDBTN"))
                {
                        T_EC_CPT cp = new T_EC_CPT();
                        try
                        {
                       
                            cp.ID_EC_CPT = db.T_EC_CPT.Max(p => p.ID_EC_CPT) + 1;
                            cp.ANNEE_EC_CPT = DateTime.Now.Year;
                            cp.CODE_DEP_EC_CPT = "A301";
                            cp.NUM_LIGNE_EC_CPT = 1;
                            cp.CODE_JOURNAL_EC_CPT = "CE";
                            cp.DATE_SAISIE_EC_CPT = DateTime.Now;
                            cp.DATE_EFFET_EC_CPT = fin.DAT_FIN;
                            cp.COMPTE_GEN_EC_CPT = "40900000";
                            cp.MONTANT_EC_CPT = fin.MONT_FIN;
                            cp.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                            cp.TYPE_TRANSACTION_EC_CPT = "AP";
                            cp.TYPE_DOC_EC_CPT = "CK";
                            cp.NOM_USER_EC_CPT = Session["UserLogin"].ToString();
                            cp.CODE_TIERS_EC_CPT = fin.REF_ADH_FIN.ToString();
                            cp.REF_PIECE_EC_CPT = DateTime.Now.Year + "/" + ref_instr;
                            cp.LOT_EC_CPT = DateTime.Now.Year + "/" + ref_instr;
                            cp.DOMAINE_EC_CPT = "medfact";
                            cp.DATE_EC_CPT = DateTime.Now;
                            cp.INTITULE_EC_CPT = "Financement";
                            db.T_EC_CPT.Add(cp);
                            db.SaveChanges();
                        }
                        catch (Exception) { }
                        /**************************/
                        try
                        {
                            T_EC_CPT cp2 = new T_EC_CPT();
                            cp2.ID_EC_CPT = cp.ID_EC_CPT + 1;
                            cp2.ANNEE_EC_CPT = DateTime.Now.Year;
                            cp2.CODE_DEP_EC_CPT = "A301";
                            cp2.NUM_LIGNE_EC_CPT = 2;
                            cp2.CODE_JOURNAL_EC_CPT = "CE";
                            cp2.DATE_SAISIE_EC_CPT = DateTime.Now;
                            cp2.DATE_EFFET_EC_CPT = fin.DAT_FIN;
                            cp2.COMPTE_GEN_EC_CPT = "53201000";
                            cp2.MONTANT_EC_CPT = 0 - fin.MONT_FIN;
                            cp2.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                            cp2.TYPE_TRANSACTION_EC_CPT = "AP";
                            cp2.TYPE_DOC_EC_CPT = "CK";
                            cp2.NOM_USER_EC_CPT = Session["UserLogin"].ToString();
                            cp2.CODE_TIERS_EC_CPT = fin.REF_ADH_FIN.ToString();
                            cp2.DOMAINE_EC_CPT = "medfact";
                            cp2.DATE_EC_CPT = DateTime.Now;
                            cp2.INTITULE_EC_CPT = "Financement débit";
                            cp2.REF_PIECE_EC_CPT = DateTime.Now.Year + "/" + ref_instr;
                            cp2.LOT_EC_CPT = DateTime.Now.Year + "/" + ref_instr;
                            db.T_EC_CPT.Add(cp2);
                            db.SaveChanges();
                        }
                        catch (Exception) { }
                        try
                        {
                            T_DOC_GED ged = new T_DOC_GED();
                            ged.ID_IND_GED = fin.REF_ADH_FIN;
                            ged.ID_CTR_GED = fin.REF_CTR_FIN;
                            ged.ID_FINCANEMENT_GED = fin.ID_FIN;
                            ged.LIBELLE_GED = "Financement";
                            ged.DATE_TACHE_GED = DateTime.Now;
                            ged.ID_GESTIONNAIRE_GED = int.Parse(id_user);
                            ged.LIBELLE2_GED = fin.REF_INSTR_FIN;
                            ged.ID_Emetteur_GED = Session["ID_USER"].ToString();
                            ged.Etape_ged = "Création";
                            ged.Etat_GED = false;
                            db.T_DOC_GED.Add(ged);
                            db.SaveChanges();
                        }
                        catch (Exception) { }
                        fin.TYPE_ENC = "F";

                        TempData["Link"] = "/Rapporting/Financmement_F";
                        TempData["name"] = "Ordre de paiement";
                    }
                else
                    if (TypeFin.Equals("LibFDGRDBTN"))
                {
                        T_EC_CPT cp = new T_EC_CPT();
                        try
                        {

                       
                            cp.ID_EC_CPT = db.T_EC_CPT.Max(p => p.ID_EC_CPT) + 1;
                            cp.ANNEE_EC_CPT = DateTime.Now.Year;
                            cp.CODE_DEP_EC_CPT = "A301";
                            cp.NUM_LIGNE_EC_CPT = 1;
                            cp.CODE_JOURNAL_EC_CPT = "CE";
                            cp.DATE_SAISIE_EC_CPT = DateTime.Now;
                            cp.DATE_EFFET_EC_CPT = fin.DAT_FIN;
                            cp.COMPTE_GEN_EC_CPT = "40900000";
                            cp.MONTANT_EC_CPT = fin.MONT_FIN;
                            cp.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                            cp.TYPE_TRANSACTION_EC_CPT = "AP";
                            cp.TYPE_DOC_EC_CPT = "CK";
                            cp.NOM_USER_EC_CPT = Session["UserLogin"].ToString();
                            cp.CODE_TIERS_EC_CPT = fin.REF_ADH_FIN.ToString();
                            cp.DOMAINE_EC_CPT = "medfact";
                            cp.DATE_EC_CPT = DateTime.Now;
                            cp.INTITULE_EC_CPT = "Libération de FDG pour ";
                            cp.REF_PIECE_EC_CPT = DateTime.Now.Year + "/" + ref_instr;
                            cp.LOT_EC_CPT = DateTime.Now.Year + "/" + ref_instr;
                            db.T_EC_CPT.Add(cp);
                        }
                        catch (Exception) { }
                        /**************************/
                        try
                        {
                            T_EC_CPT cp2 = new T_EC_CPT();
                            cp2.ID_EC_CPT = cp.ID_EC_CPT + 1;
                            cp2.ANNEE_EC_CPT = DateTime.Now.Year;
                            cp2.CODE_DEP_EC_CPT = "A301";
                            cp2.NUM_LIGNE_EC_CPT = 2;
                            cp2.CODE_JOURNAL_EC_CPT = "CE";
                            cp2.DATE_SAISIE_EC_CPT = DateTime.Now;
                            cp2.DATE_EFFET_EC_CPT = fin.DAT_FIN;
                            cp2.COMPTE_GEN_EC_CPT = "53201000";
                            cp2.MONTANT_EC_CPT = 0 - fin.MONT_FIN;
                            cp2.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                            cp2.TYPE_TRANSACTION_EC_CPT = "AP";
                            cp2.TYPE_DOC_EC_CPT = "CK";
                            cp2.NOM_USER_EC_CPT = Session["UserLogin"].ToString();
                            cp2.CODE_TIERS_EC_CPT = fin.REF_ADH_FIN.ToString();
                            cp2.DOMAINE_EC_CPT = "medfact";
                            cp2.DATE_EC_CPT = DateTime.Now;
                            cp2.INTITULE_EC_CPT = "UBCI";
                            cp2.REF_PIECE_EC_CPT = DateTime.Now.Year + "/" + ref_instr;
                            cp2.LOT_EC_CPT = DateTime.Now.Year + "/" + ref_instr;
                            db.T_EC_CPT.Add(cp2);

                            db.SaveChanges();

                        }
                        catch (Exception) { }
                        try
                        {
                            T_DOC_GED ged = new T_DOC_GED();
                            ged.ID_IND_GED = fin.REF_ADH_FIN;
                            ged.ID_CTR_GED = fin.REF_CTR_FIN;
                            ged.ID_FINCANEMENT_GED = fin.ID_FIN;
                            ged.LIBELLE_GED = "Liberation Financement";
                            ged.DATE_TACHE_GED = DateTime.Now;
                            ged.ID_GESTIONNAIRE_GED = int.Parse(id_user);
                            ged.LIBELLE2_GED = fin.REF_INSTR_FIN;
                            ged.ID_Emetteur_GED = Session["ID_USER"].ToString();
                            ged.Etape_ged = "Création";
                            ged.Etat_GED = false;
                            db.T_DOC_GED.Add(ged);
                            db.SaveChanges();
                        }
                        catch (Exception) { }
                        TempData["Link"] = "/Rapporting/Financmement_LIBFDG";
                        TempData["name"] = "Ordre de paiement libération de FDG";
                        fin.TYPE_ENC = "LIB_FIN";

                    }
                    db.T_FINANCEMENT.Add(fin);
                    db.SaveChanges();

                    if (fin.TYPE_ENC == "F")
                    {

                        try
                        {
                            T_EXTRAIT extrait = new T_EXTRAIT();
                            extrait.REF_CTR_EXTRAIT = fin.REF_CTR_FIN;
                            extrait.LIB_EXTRAIT = "Financement " + fin.REF_INSTR_FIN;
                            extrait.DEBIT_EXTRAIT = 0;
                            extrait.CREDIT_EXTRAIT = 0;
                            extrait.AUTRE_EXTRAIT = fin.MONT_FIN;
                            extrait.ENCOURFACT_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Encours_Facture).FirstOrDefault();
                            extrait.DISPONIBLE_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Disponible).FirstOrDefault();
                            extrait.DAT_OP_EXTRAIT = DateTime.Now;
                            extrait.DAT_VAL_EXTRAIT = fin.DAT_FIN;
                            extrait.TOTAL_FIN_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Total_Financement).FirstOrDefault();
                            db.T_EXTRAIT.Add(extrait);
                            db.SaveChanges();

                        }
                        catch (Exception) { }




                        try
                        {
                            T_HISTORIQUE histo = new T_HISTORIQUE();
                            histo.ABREV_ROLE_HIST = "Creation Financement " + fin.REF_INSTR_FIN;
                            histo.ACTION = "Creation bordereau";
                            histo.ID_ENREGISTREMENT = fin.REF_INSTR_FIN;
                            histo.T_TABLE = "T_Financement";
                            histo.REF_CTR_HIST = fin.REF_CTR_FIN.ToString();
                            histo.REF_IND_HIST = fin.REF_ADH_FIN.ToString();
                            histo.LOGIN_USER = Session["UserLogin"].ToString();
                            histo.IP_PC = GetIp();
                            histo.NOM_PC = HttpContext.Request.UserHostName;
                            histo.DATE_ACTION = DateTime.Now;
                            db.T_HISTORIQUE.Add(histo);
                            db.SaveChanges();
                        }
                        catch (Exception e) { TempData["error"] = e.Message; }


                    }

                    else
                    {

                        try
                        {
                            T_EXTRAIT extrait = new T_EXTRAIT();
                            extrait.REF_CTR_EXTRAIT = fin.REF_CTR_FIN;
                            extrait.LIB_EXTRAIT = "Deblocage FDG " + fin.REF_INSTR_FIN;
                            extrait.DEBIT_EXTRAIT = 0;
                            extrait.CREDIT_EXTRAIT = 0;
                            extrait.AUTRE_EXTRAIT = fin.MONT_FIN;
                            extrait.ENCOURFACT_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Encours_Facture).FirstOrDefault();
                            extrait.DISPONIBLE_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Disponible).FirstOrDefault();
                            extrait.DAT_OP_EXTRAIT = DateTime.Now;
                            extrait.DAT_VAL_EXTRAIT = fin.DAT_FIN;
                            extrait.TOTAL_FIN_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Total_Financement).FirstOrDefault();
                            db.T_EXTRAIT.Add(extrait);
                            db.SaveChanges();

                        }
                        catch (Exception) { }



                        try
                        {
                            T_HISTORIQUE histo = new T_HISTORIQUE();
                            histo.ABREV_ROLE_HIST = "Deblocage FDG " + fin.REF_INSTR_FIN;
                            histo.ACTION = "Creation Deblocage FDG";
                            histo.ID_ENREGISTREMENT = fin.REF_INSTR_FIN;
                            histo.T_TABLE = "T_Financement";
                            histo.REF_CTR_HIST = fin.REF_CTR_FIN.ToString();
                            histo.REF_IND_HIST = fin.REF_ADH_FIN.ToString();
                            histo.LOGIN_USER = Session["UserLogin"].ToString();
                            histo.IP_PC = GetIp();
                            histo.NOM_PC = HttpContext.Request.UserHostName;
                            histo.DATE_ACTION = DateTime.Now;
                            db.T_HISTORIQUE.Add(histo);
                            db.SaveChanges();
                        }
                        catch (Exception e) { TempData["error"] = e.Message; }

                    }


                }
            catch (Exception) { return RedirectToAction("InternalServerError", "Error"); }
            TempData["message"] = " enregistrement a été effectué avec succès ! N°Contrat" + fin.REF_CTR_FIN;
            return RedirectToAction("financement");
        }
  catch (Exception e) { TempData["error"] = e.Message; return RedirectToAction("InternalServerError", "Error") ;  }
}

        public ActionResult Financmement_F(string id)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "R_Financemts.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
     

            var cm = db.usp_Etat_Financements().ToList();

            ReportDataSource rd = new ReportDataSource("DataSet2", cm);
            lr.DataSources.Add(rd);
            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;



            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>8.5in</PageWidth>" +
            "  <PageHeight>11in</PageHeight>" +
            "  <MarginTop>0.5in</MarginTop>" +
            "  <MarginLeft>1in</MarginLeft>" +
            "  <MarginRight>1in</MarginRight>" +
            "  <MarginBottom>0.5in</MarginBottom>" +
            "</DeviceInfo>";

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

        [HttpGet]
        public JsonResult AllRecord(int ref_ctr)
        {
            db.Configuration.ProxyCreationEnabled = false;
            //   //Calcul d'interet par mois
            string query = "select ISNULL(SUM(INTERET_J_CALC_DISPO), 0) from t_calc_dispo where REF_CTR_CALC_DISPO = " +ref_ctr+ " and(day(DATE__CALC_DISPO) < day(getdate())) and(month(DATE__CALC_DISPO) = month(getdate()) and(year(DATE__CALC_DISPO)) = year(getdate()))";
            //Calcul d'interet de retard par mois
            string query2 = "select ISNULL(SUM(mont_calc_ir),0)  from T_CALC_INT_IR where REF_CTR_CALC_IR=" + ref_ctr + " and (day(Date_Echeance_IR)<day(getdate())) and (month(Date_Echeance_IR)=month(getdate()) and (year(Date_Echeance_IR))=year(getdate()))";
            //Calcul d'interet de retard par an
            string query3 = "select ISNULL(SUM(mont_calc_ir),0) from T_CALC_INT_IR where REF_CTR_CALC_IR=" + ref_ctr + " and (month(Date_Echeance_IR)<month(getdate())  and (year(Date_Echeance_IR))=year(getdate()))";
            //Calcul d'interet de retard avant l'année encours
            string query4 = "select ISNULL(SUM(mont_calc_ir),0) from T_CALC_INT_IR where REF_CTR_CALC_IR=" + ref_ctr + "  and (year(Date_Echeance_IR))<year(getdate())";
            //Calcul d'interet par an
            string query5 = "select ISNULL(SUM(INTERET_J_CALC_DISPO),0) from t_calc_dispo where REF_CTR_CALC_DISPO =" + ref_ctr + " and (month(DATE__CALC_DISPO)<month(getdate()) and (year(DATE__CALC_DISPO))=year(getdate()))";

            // string query6 = "select ISNULL(SUM(MONT_FIN) ,0) FROM dbo.T_FINANCEMENT ,t_contrat WHERE      REF_CTR_FIN = REF_CTR and REF_CTR ="+ref_ctr+"  and MONTH(DAT_FIN) = MONTH(GETDATE()) and YEAR(DAT_FIN) = YEAR(GETDATE())";
            decimal disponible=0;
            decimal a = 0;
            decimal b = 0;

            decimal c = 0;
            try { a= (decimal)db.SumMntImpaye(ref_ctr).First(); } catch (Exception) { a = 0; }
            try {b= (decimal)db.SumMntLitige(ref_ctr).First(); } catch (Exception) { b=0; }
            try {c= (decimal)db.SumMntFactDepAlgo(ref_ctr).First(); } catch (Exception) { c = 0; }

           // decimal montant_ouvert;
            // string qurey7 = "select ISNULL(SUM(mont_calc_ir), 0) from T_CALC_INT_IR where REF_CTR_CALC_IR = " + ref_ctr + "  and(year(Date_Echeance_IR)) < year(getdate())";
          
            var all = db.All_Ecran_Financements(ref_ctr).First();
            int ref_contart = all.REF_CTR;
            decimal total_ctr_final;
            try { total_ctr_final = (decimal)all.Encours_Facture; } catch (Exception) { total_ctr_final = 0; }
            decimal totalfact;
            try { totalfact = (decimal)all.Total_Facture; } catch (Exception) { totalfact = 0; }
             
            decimal total_ctr_fin = (decimal)all.Total_Financement;
            decimal finmois = (decimal)db.FIN_MOIS_CTR(ref_ctr).First();
           // try { montant_ouvert = (decimal)db.usp_Encours_Fact_Ouvert(ref_ctr).First(); } catch (Exception) { montant_ouvert = 0; }
            decimal dep_limite = (decimal)db.usp_Etat_Depass_Lim_ACH(ref_ctr).Sum(p=>p.DepassLim);
            decimal interet_par_mois = db.Database.SqlQuery<decimal>(query).First();
            decimal interet_retard_par_mois = db.Database.SqlQuery<decimal>(query2).First();
            decimal interet_retard_par_an = db.Database.SqlQuery<decimal>(query3).First();
            decimal interet_retard_avant_an_encour = db.Database.SqlQuery<decimal>(query4).First();
            decimal interet_par_an = db.Database.SqlQuery<decimal>(query5).First();
            decimal FDG = (decimal)all.Toatl_FDG;
            decimal Avoir = (decimal)all.Total_Avoir;
            decimal comfact = (decimal)all.Total_Commission;
            decimal t_fraisdivers;
            decimal t_fraisdiversC;
            decimal t_fraisdiversT;
            decimal t_fraisdiversV;
            decimal t_credit;
            decimal t_debit;
            decimal total_retenu;
            decimal SumMntImpaye;
            decimal SumMntLitige;
            decimal SumMntFactDepAlgo;
            decimal total_intert_de_retard;
            decimal total_intert;
            decimal FDR;
            string Contrat;
            string Condition;
            string RC;
            string MF;
            /************************************************/
            decimal disponible_SansFdg;
            decimal fdg_liberer_from_fin;
            decimal fdg_librer_calc;
            decimal fdg_librer_sum;
            try { disponible_SansFdg = (decimal)db.Disponible_Sans_FDG_Libérer(ref_ctr).First(); }
            catch (Exception e) {
                disponible_SansFdg = 0;
            };

            try { fdg_liberer_from_fin = (decimal)db.FDG_Libérer_From_T_Finacement(ref_ctr).First(); }
            catch (Exception e)
            {
                fdg_liberer_from_fin = 0;
            };

            try { fdg_librer_calc = (decimal)db.Financemnt_Sans_FDG_Libérer(ref_ctr).First(); }
            catch (Exception e)
            {
                fdg_librer_calc = 0;
            };

            try { fdg_librer_sum = (decimal)db.Sum_FDG_Libérer(ref_ctr).First(); }
            catch (Exception e)
            {
                fdg_librer_sum = 0;
            };
           
            /**********************************************/

            try
            {
                t_fraisdivers = (decimal)db.usp_FRAIS_DIVERS(ref_ctr).Select(p => p.FraisDiv).First();
            }
            catch (Exception) { t_fraisdivers = 0; }
            try { t_fraisdiversC = (decimal)db.usp_FRAIS_PAIEMENT_C(ref_ctr).First().FraisPaiC; } catch (Exception) { t_fraisdiversC = 0; }
            try { t_fraisdiversT = (decimal)db.usp_FRAIS_PAIEMENT_T(ref_ctr).First().FraisPaiT; } catch (Exception) { t_fraisdiversT = 0; }
            try { t_fraisdiversV = (decimal)db.usp_FRAIS_PAIEMENT_V(ref_ctr).First().FaisPaiV; } catch (Exception) { t_fraisdiversV = 0; }
            try { t_credit = (decimal)all.Total_Credit; } catch (Exception) { t_credit = 0; }
            try { t_debit = (decimal)all.Total_Debit;  } catch (Exception) { t_debit = 0; }
            try { total_retenu = (decimal)all.Total_Retenue; } catch (Exception) { total_retenu = 0; }
            try { SumMntImpaye = (decimal)db.SumMntImpaye(ref_ctr).First(); } catch (Exception) { SumMntImpaye = 0; }
            try { SumMntLitige = (decimal)db.SumMntLitige(ref_ctr).First(); } catch (Exception) { SumMntLitige = 0; }
            try { SumMntFactDepAlgo= (decimal)db.SumMntFactDepAlgo(ref_ctr).First(); } catch (Exception) { SumMntFactDepAlgo = 0;  }
            try { total_intert_de_retard = (decimal)all.Total_IR_All_annee; } catch (Exception) { total_intert_de_retard = 0; }
            try { total_intert = (decimal)all.Total_Interet; } catch (Exception) { total_intert = 0; }
            try { FDR = (decimal)db.T_CONTRAT.Where(p => p.REF_CTR == ref_ctr).Select(p => p.DERN_MONT_DISP_2).First(); } catch (Exception) { FDR = 0; }
            var usp_Etat_Depass_Lim_ACH = db.usp_Etat_Depass_Lim_ACH(ref_ctr).ToList();
            disponible = totalfact - total_ctr_fin - Avoir - comfact - t_fraisdiversC- t_fraisdivers - t_fraisdiversT -t_fraisdiversV  +t_credit-t_debit - FDG - dep_limite-FDR-total_intert - total_retenu - interet_par_an - interet_retard_par_an- total_intert_de_retard;
            decimal disponible2 = disponible - a - b - c;
            try { Contrat = db.T_DOC_GED.Where(p => p.ID_CTR_GED == ref_ctr && p.LIBELLE_GED == "Contrat").Select(p => p.ADRESS_DOC_GED).Single(); } catch (Exception) { Contrat = ""; }
            try { Condition = db.T_DOC_GED.Where(p => p.ID_CTR_GED == ref_ctr && p.LIBELLE_GED == "Conditions Particulières").Select(p => p.ADRESS_DOC_GED).Single(); } catch (Exception) { Condition = ""; }
            try { RC = db.T_DOC_GED.Where(p => p.ID_CTR_GED == ref_ctr && p.LIBELLE_GED == "Registre De Commerce").Select(p => p.ADRESS_DOC_GED).Single(); } catch (Exception) { RC = ""; }
            try { MF = db.T_DOC_GED.Where(p => p.ID_CTR_GED == ref_ctr && p.LIBELLE_GED == "Matricle Fiscale").Select(p => p.ADRESS_DOC_GED).Single(); } catch (Exception) { MF = ""; }
            decimal montFinEncours =  total_ctr_final - FDG - dep_limite - FDR - disponible;
            string type_contrat = db.T_CONTRAT.Where(p => p.REF_CTR == ref_ctr).FirstOrDefault().TYP_CTR;

            List<T_INDIVIDU> ListedesFRS = db.Database.SqlQuery<T_INDIVIDU>("select * from t_individu where ref_ind in (select ref_ind_cir from tj_cir where ref_ctr_cir='" + ref_ctr + "' and id_role_cir in ('FRS','ADH')) ").ToList();

            string sql = " select  month(DAT_FIN)  as mois ,sum(T_FINANCEMENT.MONT_FIN) as montant_financement , " +
     "(select sum(MONT_TOT_BORD)  from T_BORDEREAU where ANNEE_BORD = year(SYSDATETIME()) and VALIDE_BORD = 1 and month(DAT_BORD) = month(DAT_FIN)  group by month(DAT_BORD) ) as montant_achat" +
" from T_FINANCEMENT " +
"where REF_CTR_FIN='" + ref_ctr+"' and  year(DAT_FIN) = year(SYSDATETIME()) " +
"group by month(DAT_FIN)";


            List<AchatFinancementChart> listeachatsfina = db.Database.SqlQuery<AchatFinancementChart>(sql).ToList();


            var results = new
            {
                //a verifierb
                type_contrat,
                ListedesFRS,
                ref_contart,
                disponible_SansFdg,
                fdg_liberer_from_fin,
                fdg_librer_calc,
                fdg_librer_sum,
                total_ctr_final,
                totalfact,
                total_ctr_fin,
                finmois,
              //  montant_ouvert,
                dep_limite,
                interet_par_mois,
                interet_retard_par_mois,
                interet_retard_par_an,
                interet_retard_avant_an_encour,
                interet_par_an,
                FDG,
                Avoir,
                comfact,
                t_fraisdivers,
                t_fraisdiversAll= t_fraisdiversT+ t_fraisdiversV+ t_fraisdiversC,
                t_credit,
                t_debit,
                total_retenu,
                SumMntImpaye,
                SumMntLitige,
                SumMntFactDepAlgo,
                total_intert_de_retard,
                total_intert,
                usp_Etat_Depass_Lim_ACH,
                FDR,
                disponible,
                disponible2,
                Contrat,
                Condition,
                MF,
                RC,
                montFinEncours,
                listeachatsfina
                // finpar_rapport_encrours= db.usp_Encours_Fact_Ouvert(ref_ctr).First()- db.usp_FDG(ref_ctr).First()- db.usp_Etat_Depass_Lim_pour_ACH(ref_ctr).First()- db.T_CONTRAT.Where(p => p.REF_CTR == ref_ctr).Select(p => p.DERN_MONT_DISP_2).First()- db.Disponible_CTR(ref_ctr).First(),
                //fin_mois
            };
            return Json(results, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Detail_Cheques(int id)
        {
            //ViewBag.enc = db.T_ENCAISSEMENT.Where(p => p.REF_CTR_ENC == id && p.TYP_ENC == "C" && p.VALIDE_ENC == true).ToList();

            var listeCheque = (from q in db.T_ENCAISSEMENT
                              
                               where q.REF_CTR_ENC == id  && q.TYP_ENC.Trim() !="A" && q.VALIDE_ENC == true
                               select new { q.ID_ENC,q.REF_ENC, q.MONT_ENC, q.DAT_RECEP_ENC, q.DAT_VAL_ENC }).ToList();
            List<DetailleEnc> listeCheques = new List<DetailleEnc>();
            foreach (var item in listeCheque)
            {
                DetailleEnc enc = new DetailleEnc();
                enc.REF_ENC = item.REF_ENC;
                enc.MONT_ENC = item.MONT_ENC;
                enc.DAT_RECEP_ENC = item.DAT_RECEP_ENC;
                enc.DAT_VAL_ENC = item.DAT_VAL_ENC;

                try { enc.ADRESS_DOC_GED = (from q in db.T_DOC_GED where q.ID_FINCANEMENT_GED == item.ID_ENC select (q.ADRESS_DOC_GED)).FirstOrDefault(); } catch (Exception) { enc.ADRESS_DOC_GED = ""; }
              
                listeCheques.Add(enc);
            }
            ViewBag.enc = listeCheques;

            //ViewBag.traite = db.T_ENCAISSEMENT.Where(p => p.REF_CTR_ENC == id && p.TYP_ENC == "T" && p.VALIDE_ENC == true).ToList();


           /* var listetraite = (from q in db.T_ENCAISSEMENT
                               
                               where q.REF_CTR_ENC == id && q.TYP_ENC == "T" && q.VALIDE_ENC == true
                               select new { q.ID_ENC,q.REF_ENC, q.MONT_ENC, q.DAT_RECEP_ENC, q.DAT_VAL_ENC}).ToList();
            List<DetailleEnc> listetraites = new List<DetailleEnc>();
            foreach (var item in listetraite)
            {
                DetailleEnc enc = new DetailleEnc();
                enc.REF_ENC = item.REF_ENC;
                enc.MONT_ENC = item.MONT_ENC;
                enc.DAT_RECEP_ENC = item.DAT_RECEP_ENC;
                enc.DAT_VAL_ENC = item.DAT_VAL_ENC;
                try { enc.ADRESS_DOC_GED = (from q in db.T_DOC_GED where q.ID_FINCANEMENT_GED == item.ID_ENC select (q.ADRESS_DOC_GED)).FirstOrDefault(); } catch (Exception) { enc.ADRESS_DOC_GED = ""; }
                
                listeCheques.Add(enc);
            }
            ViewBag.traite = listeCheques;*/
            return PartialView();
        }



        public ActionResult Detail_Avoir(int id)
        {
            // List<T_ENCAISSEMENT> enc = db.T_ENCAISSEMENT.Where(p => p.REF_CTR_ENC == id && p.TYP_ENC=="A" && p.VALIDE_ENC==true).ToList();
            var listeAv = (from q in db.T_ENCAISSEMENT
                       
                               where q.REF_CTR_ENC == id && q.TYP_ENC == "A" && q.VALIDE_ENC == true
                               select new {q.ID_ENC, q.REF_ENC, q.MONT_ENC, q.DAT_RECEP_ENC, q.DAT_VAL_ENC }).ToList();
            List<DetailleEnc> listeAvoirs = new List<DetailleEnc>();
            foreach (var item in listeAv)
            {
                DetailleEnc enc = new DetailleEnc();
                enc.REF_ENC = item.REF_ENC;
                enc.MONT_ENC = item.MONT_ENC;
                enc.DAT_RECEP_ENC = item.DAT_RECEP_ENC;
                enc.DAT_VAL_ENC = item.DAT_VAL_ENC;
                try { enc.ADRESS_DOC_GED = (from q in db.T_DOC_GED where q.ID_FINCANEMENT_GED == item.ID_ENC select (q.ADRESS_DOC_GED)).FirstOrDefault(); } catch (Exception) { enc.ADRESS_DOC_GED = ""; }
             
                listeAvoirs.Add(enc);
            }
            ViewBag.enc = listeAvoirs;
            return PartialView();
        }

        public ActionResult Detail_FINANCEMENT(int id)
        {
            // List<T_FINANCEMENT> fin = db.T_FINANCEMENT.Where(p => p.REF_CTR_FIN == id).OrderBy(p => p.DAT_FIN).ToList();
            // string query = "select t_financement.* as fin,ADRESS_DOC_GED from t_financement,T_DOC_GED where REF_CTR_FIN = " + id+" and ID_FIN=ID_FINCANEMENT_GED";
            var listfin = (from q in db.T_FINANCEMENT
                           where (q.REF_CTR_FIN == id)
                           select new { q.REF_CTR_FIN, q.DAT_FIN, q.REF_INSTR_FIN, q.MONT_FIN, q.INSTR_FIN, q.ID_FIN }).ToList();
            
            List<DetailleFin> listefin2=new List<DetailleFin>();
            
            foreach (var item in listfin)
            {
                DetailleFin fin = new DetailleFin();
                fin.INSTR_FIN = item.INSTR_FIN;
                fin.MONT_FIN = item.MONT_FIN;
                fin.REF_CTR_FIN = item.REF_CTR_FIN;
                fin.REF_INSTR_FIN = item.REF_INSTR_FIN;
                fin.DAT_FIN = item.DAT_FIN;
                try { fin.ADRESS_DOC_GED = (from q in db.T_DOC_GED where q.ID_FINCANEMENT_GED == item.ID_FIN select (q.ADRESS_DOC_GED)).FirstOrDefault(); } catch (Exception) { fin.ADRESS_DOC_GED = ""; }
               
                listefin2.Add(fin);
            }


            ViewBag.financement = listefin2;
            return PartialView();
        }

        public ActionResult Detail_Facture(int id)
        {
            ViewBag.Detail_Facture= db.Detail_Facture(id);
            return PartialView();
        }


        public ActionResult Detail_DEBIT(int id)
        {
            //ViewBag.Detail_DEBIT = db.Detail_DEBIT(id);
            var listeDeb = db.Detail_DEBIT(id);
            List<DetailleDebit> ListeDebs = new List<DetailleDebit>();
            foreach (var item in listeDeb)
            {
                DetailleDebit deb = new DetailleDebit();
                deb.ID_DEBIT = item.ID_DEBIT;
                deb.ABEV_DEBIT = item.ABEV_DEBIT;
                deb.MONT_DEBIT = item.MONT_DEBIT;
                deb.NOM_IND = item.NOM_IND;
                deb.REF_CTR_CIR = item.REF_CTR_CIR;
                deb.DATE_DEBIT = item.DATE_DEBIT;
                try
                {
                    deb.ADRESS_DOC_GED = db.T_DOC_GED.Where(p => p.ID_CREDIT_GED == item.ID_DEBIT).Select(p => p.ADRESS_DOC_GED).Single();
                }
                catch (Exception) { deb.ADRESS_DOC_GED = ""; }
                ListeDebs.Add(deb);
            }
            ViewBag.Detail_DEBIT = ListeDebs;
            //ViewBag.Detail_CREDIT = db.Detail_CREDIT(id);
            var listeCred= db.Detail_CREDIT(id);
            List<DetailleCredit> ListeCreds = new List<DetailleCredit>();
            foreach (var item in listeCred)
            {
                DetailleCredit cred = new DetailleCredit();
                cred.ID_CREDIT = item.ID_CREDIT;
                cred.LIBELLE_LIBRE_CREDIT = item.LIBELLE_LIBRE_CREDIT;
                cred.MONT_CREDIT = item.MONT_CREDIT;
                cred.NOM_IND = item.NOM_IND;
                cred.REF_CTR_CREDIT = item.REF_CTR_CREDIT;
                cred.REF_ENC_CREDIT = item.REF_ENC_CREDIT;
                cred.DATE_CREDIT = item.DATE_CREDIT;
                try { cred.ADRESS_DOC_GED = db.T_DOC_GED.Where(p => p.ID_CREDIT_GED == item.ID_CREDIT).Select(p => p.ADRESS_DOC_GED).Single(); } catch (Exception) { cred.ADRESS_DOC_GED = ""; }
               
                ListeCreds.Add(cred);
            }
            ViewBag.Detail_CREDIT = ListeCreds;


            return PartialView();
        }


        public ActionResult Detail_RS(int id)
        {
            ViewBag.Detail_RS = db.Detail_RS(id);
            return PartialView();
        }

        public ActionResult E_Detail_IP_Impayes(int id)
        {
            // ViewBag.E_Detail_IP_Impayes = db.E_Detail_Impaye(id);
            var listeImp= db.E_Detail_Impaye(id);
            List<DetailleImp> E_Detail_Impayes = new List<DetailleImp>();
            foreach (var item in listeImp)
            {
                DetailleImp Imp = new DetailleImp();
                Imp.REF_ENC = item.REF_ENC;
                Imp.NOM_IND = item.NOM_IND;
                Imp.MONT_ENC = item.MONT_ENC;
                Imp.DAT_VAL_ENC = item.DAT_VAL_ENC;
                try
                {
                    Imp.ADRESS_DOC_GED = (from q in db.T_ENCAISSEMENT
                                          join q2 in db.T_DOC_GED
                                          on q.ID_ENC equals q2.ID_ENC_GED
                                          where q.REF_ENC == Imp.REF_ENC && q.REF_CTR_ENC == id
                                          select (q2.ADRESS_DOC_GED)).Single();
                } catch (Exception) { Imp.ADRESS_DOC_GED = ""; }
               
               // Imp.ADRESS_DOC_GED = db.T_DOC_GED.Where(p => p.ID_CREDIT_GED == item.ID_CREDIT).Select(p => p.ADRESS_DOC_GED).Single();
                E_Detail_Impayes.Add(Imp);
            }
            ViewBag.E_Detail_IP_Impayes = E_Detail_Impayes;
            //TempData["nomind"] = db.T_INDIVIDU.Where(k => k.REF_IND == db.TJ_CIR.Where(p => p.REF_CTR_CIR == id && p.ID_ROLE_CIR == "ADH").Select(p => p.REF_IND_CIR).SingleOrDefault()).SingleOrDefault();
            return PartialView();
        }

        public ActionResult E_Detail_Retard_Enc_Alog(int id)
        {
            ViewBag.E_Detail_Retard_Enc_Alog = db.E_Detail_Retard_Enc_Alog(id);
            return PartialView();
        }


        public ActionResult DetailFdg(int id)
        {
            int ref_ind = db.T_INDIVIDU.Find(db.TJ_CIR.Where(p=>p.REF_CTR_CIR==id && p.ID_ROLE_CIR=="ADH").SingleOrDefault().REF_IND_CIR).REF_IND;
            ViewBag.DetailFdg = db.CommparBord(ref_ind);
            return PartialView();
        }

        public ActionResult DetaileFactureParAn(int id,int id2)
        {
            ViewBag.DetaileFactureParAn = db.Detail_Facture_An(id,id2);
            return View();
        }
        public ActionResult Ralance(DateTime id)
        {
            ViewBag.ralance = db.T_RELANCE.Where(e=>e.DATE_RELANCE==id && e.VALIDE_RELANCE==false);
            return PartialView();
        }

        public ActionResult resolu(int id)
        {
            T_RELANCE rel =db.T_RELANCE.Find(id);

            rel.VALIDE_RELANCE = true;
            db.Entry(rel).State= EntityState.Modified;
            db.SaveChanges();
            TempData["message"] = "Notification est traitée";
            return RedirectToAction("Index", "Home", new { area = "" });
        }
        public ActionResult ValiderFinancement()
        {
            //  ViewBag.listfin = db.ValiderFincancement().Where(p=>p.DAT_INSTR_FIN==DateTime.Today).OrderByDescending(p=>p.Référence).ToList();
            ViewBag.listfin = db.ValiderFincancement().OrderByDescending(p => p.Référence).ToList();
            TempData["financement"] = "active";

            TempData["ValiderFinancement"] = "active";
            return View() ;

        }

        public ActionResult ValiderFinancementV(int id,string id2)
        {
            T_FINANCEMENT fin = db.T_FINANCEMENT.Find(id);
            fin.REF_INSTR_FIN = id2.Replace(" ","");
            fin.ETAT_FIN = "1";
            db.Entry(fin).State = EntityState.Modified;
            db.SaveChanges();
            TempData["message"] = " enregistrement a été effectué avec succès !";
            return RedirectToAction("ValiderFinancement");
        }


        public ActionResult DepassementLim(int id)
        {
            ViewBag.listedeplim = db.usp_Etat_Depass_Lim_ACH(id).ToList();
            return PartialView();
        }



        public ActionResult GetFinancement(string id)
        {
            string path;
            T_FINANCEMENT _FINANCEMENT = db.T_FINANCEMENT.Where(p => p.REF_INSTR_FIN == id).FirstOrDefault();
            if (db.usp_Etat_Financements_By_Id(_FINANCEMENT.ID_FIN).FirstOrDefault().TYPE_ENC.Equals("LIB_FIN"))
            {
                path = Path.Combine(Server.MapPath("~/Report"), "Ordre de paiement FDG.rdlc");
               // GetFileOfFinancement(, );
            }

            else
            {
                path = Path.Combine(Server.MapPath("~/Report"), "R_Financemts.rdlc");
            }

            //_FINANCEMENT.ID_FIN
            LocalReport lr = new LocalReport();
            // string path = ;
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }


            var cm = db.usp_Etat_Financements_By_Id(_FINANCEMENT.ID_FIN).ToList();

            ReportDataSource rd = new ReportDataSource("DataSet2", cm);
            lr.DataSources.Add(rd);
            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;



            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>PDF</OutputFormat>" +
            "  <PageWidth>8.5in</PageWidth>" +
            "  <PageHeight>11in</PageHeight>" +
            "  <MarginTop>0.5in</MarginTop>" +
            "  <MarginLeft>1in</MarginLeft>" +
            "  <MarginRight>1in</MarginRight>" +
            "  <MarginBottom>0.5in</MarginBottom>" +
            "</DeviceInfo>";

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



        public string GetIp()
        {
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;
        }


        public ActionResult GetFileOfFinancement(int id,string path)
        {
            LocalReport lr = new LocalReport();
           // string path = ;
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }


            var cm = db.usp_Etat_Financements_By_Id(id).ToList();

            ReportDataSource rd = new ReportDataSource("DataSet2", cm);
            lr.DataSources.Add(rd);
            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;



            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>PDF</OutputFormat>" +
            "  <PageWidth>8.5in</PageWidth>" +
            "  <PageHeight>11in</PageHeight>" +
            "  <MarginTop>0.5in</MarginTop>" +
            "  <MarginLeft>1in</MarginLeft>" +
            "  <MarginRight>1in</MarginRight>" +
            "  <MarginBottom>0.5in</MarginBottom>" +
            "</DeviceInfo>";

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

    }

   


}