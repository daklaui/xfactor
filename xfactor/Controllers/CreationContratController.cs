using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using xfactor.Models;
using System.IO;
using Spire.Doc;
namespace xfactor.Controllers
{
    public class CreationContratController : Controller
    {
        private XFactor_PRODEntities1 db = new XFactor_PRODEntities1();

    // GET: CreationContrat
        public ActionResult Creation()
        {
            if (Session["UserLogin"] != null)
            {
              
                ViewBag.ctr = new SelectList(db.TR_LIST_VAL.Where(t => t.TYP_LIST_VAL == "Type_contrat" && t.LIB_LIST_VAL != "Reverse contrat"), "ORD_LIST_VAL", "LIB_LIST_VAL");
                ViewBag.Devise = new SelectList(db.TR_NLDP.Where(t => t.ABRV_DEVISE == "TND"), "ABRV_DEVISE", "ABRV_DEVISE");
                ViewBag.Ind = new SelectList(db.T_INDIVIDU, "REF_IND", "NOM_IND");
                ViewBag.Stat_CTR = new SelectList(db.TR_LIST_VAL.Where(t => t.TYP_LIST_VAL == "Statut_Contrat"), "ABR_LIST_VAL", "LIB_LIST_VAL");
                ViewBag.List = db.TR_LISTE_FRAIS_DIVERS.Where(t => t.TYPE_FRAIS_DIVERS == "FRAIS_DIVERS").ToList();
                ViewBag.List2 = db.TR_LISTE_FRAIS_DIVERS.Where(t => t.TYPE_FRAIS_DIVERS == "FRAIS_DEMANDE_LIMITE").ToList();
                ViewData["REfCTR"] = db.T_CONTRAT.Select(t => t.REF_CTR).Max() + 1;

                TempData["CreationContrat"] = "active";
                TempData["Creation"] = "active";
                return View();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
    
        [HttpPost]
        public ActionResult Creation(string type_contrat ,T_Contrat_Web contrat, TJ_CIR cir, T_DET_ASS ass, string Verif, int indiv, string comf, string tt, List<T_frais_divers_web> fins, List<T_frais_paiment_web> pais, List<T_FOND_GARANTIE> fondgars, List<T_Comm_Factoring_Web> commfacts, List<T_int_financements_web> intertfin, List<T_DEM_LIMITE> AchDem,List<TJ_CIR> FourCir,T_demande_limite Dem_Ach_Reverse)
        {
          
            List<T_CONTRAT> ctr;
            try
            {
                ctr = db.T_CONTRAT.Where(p => p.REF_CTR_PAPIER_CTR == contrat.REF_CTR_PAPIER_CTR).ToList();
            }
            catch (Exception) { ctr = null; }
            if (ctr.Count == 0)
            {
                try
                {
                    T_CONTRAT contrat_valider = new T_CONTRAT();
                    contrat_valider.CA_CTR = decimal.Parse(contrat.CA_CTR.Replace(" ", ""));
                    contrat_valider.CA_EXP_CTR = decimal.Parse(contrat.CA_EXP_CTR.Replace(" ", ""));
                    contrat_valider.CA_IMP_CTR = decimal.Parse(contrat.CA_IMP_CTR.Replace(" ", ""));
                    contrat_valider.DAT_DEB_CTR = contrat.DAT_DEB_CTR;
                    contrat_valider.DAT_PROCH_VERS_CTR = contrat.DAT_PROCH_VERS_CTR;
                    contrat_valider.DAT_RESIL_CTR = contrat.DAT_RESIL_CTR;
                    contrat_valider.DAT_SIGN_CTR = contrat.DAT_SIGN_CTR;
                    contrat_valider.DELAI_MAX_REG_CTR = contrat.DELAI_MAX_REG_CTR;
                    contrat_valider.DELAI_MOYEN_REG_CTR = contrat.DELAI_MOYEN_REG_CTR;
                 //   try { contrat_valider.DERN_MONT_DISP_2 = decimal.Parse(contrat.CA_CTR.Replace(" ", "")); } catch (Exception) { }
                   // try { contrat_valider.DERN_MONT_DISP_2 = decimal.Parse(contrat.CA_CTR.Replace(" ", "")); } catch (Exception) { }
                    //  contrat_valider.DEVISE_CTR = contrat.DEVISE_CTR;
                    // contrat_valider.FACT_REG_CTR = contrat.FACT_REG_CTR;
                    contrat_valider.LIM_FIN_CTR = decimal.Parse(contrat.LIM_FIN_CTR.Replace(" ", ""));
                    // contrat_valider.MIN_COMM_FACT = decimal.Parse(contrat.CA_CTR.Replace(" ", ""));
                    contrat_valider.NB_ACH_PREVU_CTR = contrat.NB_ACH_PREVU_CTR;
                    contrat_valider.NB_AVOIRS_PREVU_CTR =contrat.NB_AVOIRS_PREVU_CTR;
                    contrat_valider.NB_FACT_PREVU_CTR =contrat.NB_FACT_PREVU_CTR;
                    contrat_valider.NB_REMISES_PREVU_CTR =contrat.NB_REMISES_PREVU_CTR;
                    contrat_valider.REF_CTR =contrat.REF_CTR;
                    contrat_valider.REF_CTR_PAPIER_CTR =contrat.REF_CTR_PAPIER_CTR;
                    contrat_valider.SERVICE_CTR = contrat.SERVICE_CTR;
                    contrat_valider.STATUT_CTR = contrat.STATUT_CTR;
                    if (type_contrat.Contains("Contrat_Reverse")) { contrat_valider.TYP_CTR = "4"; } else { contrat_valider.TYP_CTR = contrat.TYP_CTR; }
                    contrat_valider.MIN_COMM_FACT = decimal.Parse(comf);
                    contrat_valider.DEVISE_CTR = contrat.DEVISE_CTR.Trim();
                    db.T_CONTRAT.Add(contrat_valider);
                    db.SaveChanges();
                }
                catch (Exception) { TempData["error"] = "Error d enregistrement contrat"; }

                try
                {

                    T_DOC_GED ged = new T_DOC_GED();
                    ged.ID_IND_GED = indiv;
                    ged.ID_CTR_GED = db.T_CONTRAT.Where(p => p.REF_CTR_PAPIER_CTR == contrat.REF_CTR_PAPIER_CTR).Select(p => p.REF_CTR).FirstOrDefault();
                    ged.LIBELLE_GED = "Contrat";
                    ged.DATE_TACHE_GED = DateTime.Now;
                    ged.LIBELLE2_GED = contrat.REF_CTR_PAPIER_CTR.Replace("/", "");
                    ged.Etape_ged = "Création";
                    ged.Etat_GED = false;
                    /***********************************/
                    T_DOC_GED ged2 = new T_DOC_GED();
                    ged2.ID_IND_GED = indiv;
                    ged2.ID_CTR_GED = db.T_CONTRAT.Where(p => p.REF_CTR_PAPIER_CTR == contrat.REF_CTR_PAPIER_CTR).Select(p => p.REF_CTR).FirstOrDefault();
                    ged2.LIBELLE_GED = "Conditions Particulières";
                    ged2.DATE_TACHE_GED = DateTime.Now;
                    ged2.LIBELLE2_GED = contrat.REF_CTR_PAPIER_CTR.Replace("/", "");
                    ged2.Etape_ged = "Création";
                    ged2.Etat_GED = false;
                    db.T_DOC_GED.Add(ged);
                    db.SaveChanges();
                    db.T_DOC_GED.Add(ged2);
                    db.SaveChanges();
                }
                catch (Exception e) { TempData["error"] = TempData["error"] +  "   Error d enregistrement Ged "; }

                    if (Verif == "true")
                    {
                    //= new T_DET_ASS();
                    try
                    {
                        ass.REF_CTR_ASS = db.T_CONTRAT.Where(p => p.REF_CTR_PAPIER_CTR == contrat.REF_CTR_PAPIER_CTR).Select(p => p.REF_CTR).FirstOrDefault();

                        db.T_DET_ASS.Add(ass);
                        TJ_CIR c = new TJ_CIR();
                        c.REF_CTR_CIR = db.T_CONTRAT.Where(p => p.REF_CTR_PAPIER_CTR == contrat.REF_CTR_PAPIER_CTR).Select(p => p.REF_CTR).FirstOrDefault();
                        c.REF_IND_CIR = Convert.ToInt32(indiv);
                        c.ID_ROLE_CIR = "ASS";
                        db.TJ_CIR.Add(c);
                        db.SaveChanges();
                    }
                    catch (Exception e) { TempData["error"] = TempData["error"] + "   Error d enregistrement Assurence "; }
                    }
                try
                {
                    int x = db.T_CONTRAT.Where(p => p.REF_CTR_PAPIER_CTR == contrat.REF_CTR_PAPIER_CTR).Select(p => p.REF_CTR).FirstOrDefault();
                    cir.REF_CTR_CIR = x;
                    cir.ID_ROLE_CIR = "ADH";
                    cir.REF_IND_CIR = indiv;
                    db.TJ_CIR.Add(cir);
                    db.SaveChanges();
                    if (type_contrat.Contains("Contrat_Reverse"))
                    {
                        TJ_CIR cir2 = new TJ_CIR();
                        cir2.REF_CTR_CIR = x;
                        cir2.ID_ROLE_CIR = "ACH";
                        cir2.REF_IND_CIR = indiv;
                        db.TJ_CIR.Add(cir2);
                        db.SaveChanges();
                    }

                        TempData["notice"] = "valider";
                }
                catch (Exception e) { TempData["error"] = TempData["error"] + "   Error d enregistrement Adhérent "; }

                try
                {
                    foreach (T_frais_divers_web fin in fins)
                    {
                        T_FRAIS_DIVERS frais_divers = new T_FRAIS_DIVERS();
                        frais_divers.LIB_FRAIS_DIVERS = fin.LIB_FRAIS_DIVERS;
                        try {
                            frais_divers.MONT_PAR_UNIT_FRAIS_DIVERS = decimal.Parse(fin.MONT_PAR_UNIT_FRAIS_DIVERS); 
                        } catch (Exception e) { frais_divers.MONT_PAR_UNIT_FRAIS_DIVERS = 0; }
                        frais_divers.REF_CTR_FRAIS_DIVERS = db.T_CONTRAT.Where(p => p.REF_CTR_PAPIER_CTR == contrat.REF_CTR_PAPIER_CTR).Select(p => p.REF_CTR).FirstOrDefault();
                        frais_divers.TYP_FRAIS_DIVERS = fin.TYP_FRAIS_DIVERS;
                        db.T_FRAIS_DIVERS.Add(frais_divers);
                        db.SaveChanges();
                    }
                }
                catch (Exception e) { TempData["error"] = TempData["error"] + "   Error d enregistrement Frais divers "; }
                try {
                    foreach (T_frais_paiment_web pai in pais)
                    {
                        T_FRAIS_PAIEMENT frais_ppaiment = new T_FRAIS_PAIEMENT();
                        frais_ppaiment.LIB_FRAIS_PAIE = pai.LIB_FRAIS_PAIE;
                        try {
                            frais_ppaiment.MONT_PAR_INSTR_FRAIS_PAIE = decimal.Parse(pai.MONT_PAR_INSTR_FRAIS_PAIE); 
                        } catch (Exception e) { frais_ppaiment.MONT_PAR_INSTR_FRAIS_PAIE = 0; }
                        frais_ppaiment.REF_CTR_FRAIS_PAIE = db.T_CONTRAT.Where(p => p.REF_CTR_PAPIER_CTR == contrat.REF_CTR_PAPIER_CTR).Select(p => p.REF_CTR).FirstOrDefault();
                        frais_ppaiment.TYP_FRAIS_PAIE = pai.TYP_FRAIS_PAIE;
                        db.T_FRAIS_PAIEMENT.Add(frais_ppaiment);
                        db.SaveChanges();
                    }
                }
                catch (Exception e) { TempData["error"] = TempData["error"] + "   Error d enregistrement Frais Paiement "; }


                try {

                    foreach (T_FOND_GARANTIE fond in fondgars)
                    {
                        fond.REF_CTR_FDG = db.T_CONTRAT.Where(p => p.REF_CTR_PAPIER_CTR == contrat.REF_CTR_PAPIER_CTR).Select(p => p.REF_CTR).FirstOrDefault();
                        db.T_FOND_GARANTIE.Add(fond);
                        db.SaveChanges();
                    }
                }
                catch (Exception e) { TempData["error"] = TempData["error"] + "   Error d enregistrement Fond Garantie "; }


                try {
                    foreach (T_Comm_Factoring_Web fact in commfacts)
                    {
                        T_COMM_FACTORING comm_factoring = new T_COMM_FACTORING();

                        try { comm_factoring.MONT_MIN_CTR_COMM_FACTORING =decimal.Parse(fact.MONT_MIN_CTR_COMM_FACTORING); } catch (Exception) { comm_factoring.MONT_MIN_CTR_COMM_FACTORING = 0; }
                        try { comm_factoring.MONT_MIN_DOC_COMM_FACTORING =decimal.Parse(fact.MONT_MIN_DOC_COMM_FACTORING); } catch (Exception) { comm_factoring.MONT_MIN_DOC_COMM_FACTORING = 0; }
                        comm_factoring.REF_CTR_COMM_FACTORING = db.T_CONTRAT.Where(p => p.REF_CTR_PAPIER_CTR == contrat.REF_CTR_PAPIER_CTR).Select(p => p.REF_CTR).FirstOrDefault();
                        comm_factoring.TX_COMM_FACTORING = fact.TX_COMM_FACTORING; 
                        comm_factoring.TYP_COMM_FACTORING = fact.TYP_COMM_FACTORING;
                        db.T_COMM_FACTORING.Add(comm_factoring);
                        db.SaveChanges();
                    }
                }
                catch (Exception e) { TempData["error"] = TempData["error"] + "   Error d enregistrement Comm Fact "; }
            


                try {
                    foreach (T_int_financements_web intfin in intertfin)
                    {
                        T_INT_FINANCEMENT int_financement = new T_INT_FINANCEMENT();
                        int_financement.DAT_DEB_VALID_INT_FIN = intfin.DAT_DEB_VALID_INT_FIN;
                        int_financement.DELAI_MAX_PAI_INT_FIN = intfin.DELAI_MAX_PAI_INT_FIN;
                        try { int_financement.MAJOR_INT_INT_FIN = decimal.Parse(intfin.MAJOR_INT_INT_FIN); } catch (Exception) { int_financement.MAJOR_INT_INT_FIN = 0; }
                        try { int_financement.PRECOMPTE_INT_FIN = decimal.Parse(intfin.PRECOMPTE_INT_FIN); } catch (Exception) { int_financement.PRECOMPTE_INT_FIN = 0; }
                        int_financement.REF_CTR_INT_FIN = db.T_CONTRAT.Where(p => p.REF_CTR_PAPIER_CTR == contrat.REF_CTR_PAPIER_CTR).Select(p => p.REF_CTR).FirstOrDefault();
                        int_financement.TX_INT_MARCHE_INT_FIN = intfin.TX_INT_MARCHE_INT_FIN;
                        int_financement.TX_MARGE_CTR_INT_FIN = intfin.TX_MARGE_CTR_INT_FIN;
                        int_financement.TYP_INSTR_INT_FIN = intfin.TYP_INSTR_INT_FIN;
                        db.T_INT_FINANCEMENT.Add(int_financement);
                        db.SaveChanges();
                    }
                }
                catch (Exception e) { TempData["error"] = TempData["error"] + "   Error d enregistrement interet fin "; }





                //foreach (TJ_CIR Ach in AchCir)
                //{
                //    Ach.ID_ROLE_CIR = "ACH";
                //    //Ach.REF_CTR_CIR = (int)ViewData["REfCTR"];
                //    db.TJ_CIR.Add(Ach);
                //}
                // db.SaveChanges();
                if (type_contrat.Contains("Contrat_Reverse"))
                {
                    try {
                        if (Verif != "true")
                        {
                            T_DEM_LIMITE demande_limite = new T_DEM_LIMITE();
                            demande_limite.ACTIF_DEM_LIMI = true;
                            demande_limite.DAT_DEM_LIM =(DateTime)Dem_Ach_Reverse.DATLIM_DEM_LIM;
                            demande_limite.DATLIM_DEM_LIM = Dem_Ach_Reverse.DATLIM_DEM_LIM;
                            demande_limite.TYP_DEM_LIM = "FIN";
                            try { demande_limite.MONT_DEM_LIM = decimal.Parse(Dem_Ach_Reverse.MONT_DEM_LIM.Replace(" ", "")); } catch (Exception) { }
                            try { demande_limite.MONT_ACC = decimal.Parse(Dem_Ach_Reverse.MONT_DEM_LIM.Replace(" ", "")); } catch (Exception) { }
                            demande_limite.REF_CTR_DEM_LIM = db.T_CONTRAT.Where(p => p.REF_CTR_PAPIER_CTR == contrat.REF_CTR_PAPIER_CTR).Select(p => p.REF_CTR).FirstOrDefault();
                            demande_limite.DEVISE = "TND";
                            demande_limite.DELAIS_DEM_LIM = Dem_Ach_Reverse.DELAIS_DEM_LIM;
                            demande_limite.MODE_PAY_DEM_LIM = Dem_Ach_Reverse.MODE_PAY_DEM_LIM;
                            demande_limite.MODE_PAY_ACC = Dem_Ach_Reverse.MODE_PAY_DEM_LIM;
                            demande_limite.DELAIS_ACC = Dem_Ach_Reverse.DELAIS_DEM_LIM;
                            demande_limite.REF_ACH_LIM = indiv;
                            db.T_DEM_LIMITE.Add(demande_limite);
                            db.SaveChanges();


                        }
                        else
                        {
                            T_DEM_LIMITE demande_limite = new T_DEM_LIMITE();
                            demande_limite.ACTIF_DEM_LIMI = true;
                            demande_limite.DAT_DEM_LIM = (DateTime)Dem_Ach_Reverse.DATLIM_DEM_LIM;
                            demande_limite.DATLIM_DEM_LIM = Dem_Ach_Reverse.DATLIM_DEM_LIM;
                            demande_limite.TYP_DEM_LIM = "FIN";
                            try { demande_limite.MONT_DEM_LIM = decimal.Parse(Dem_Ach_Reverse.MONT_DEM_LIM.Replace(" ", "")); } catch (Exception) { }
                            try { demande_limite.MONT_ACC = decimal.Parse(Dem_Ach_Reverse.MONT_DEM_LIM.Replace(" ", "")); } catch (Exception) { }
                            demande_limite.REF_CTR_DEM_LIM = db.T_CONTRAT.Where(p => p.REF_CTR_PAPIER_CTR == contrat.REF_CTR_PAPIER_CTR).Select(p => p.REF_CTR).FirstOrDefault();
                            demande_limite.DEVISE = "TND";
                            demande_limite.DELAIS_DEM_LIM = Dem_Ach_Reverse.DELAIS_DEM_LIM;
                            demande_limite.MODE_PAY_DEM_LIM = Dem_Ach_Reverse.MODE_PAY_DEM_LIM;
                            demande_limite.MODE_PAY_ACC = Dem_Ach_Reverse.MODE_PAY_DEM_LIM;
                            demande_limite.DELAIS_ACC = Dem_Ach_Reverse.DELAIS_DEM_LIM;
                            demande_limite.REF_ACH_LIM = indiv;
                            db.T_DEM_LIMITE.Add(demande_limite);
                            db.SaveChanges();


                            T_DEM_LIMITE demande_limiteCre = new T_DEM_LIMITE();
                            demande_limiteCre.ACTIF_DEM_LIMI = true;
                            demande_limiteCre.DAT_DEM_LIM = (DateTime)Dem_Ach_Reverse.DATLIM_DEM_LIM;
                            demande_limiteCre.DATLIM_DEM_LIM = Dem_Ach_Reverse.DATLIM_DEM_LIM;
                            demande_limiteCre.TYP_DEM_LIM = "CRE";
                            try { demande_limiteCre.MONT_DEM_LIM = decimal.Parse(Dem_Ach_Reverse.MONT_DEM_LIMC.Replace(" ", "")); } catch (Exception) { }
                            try { demande_limiteCre.MONT_ACC = decimal.Parse(Dem_Ach_Reverse.MONT_DEM_LIMC.Replace(" ", "")); } catch (Exception) { }
                            demande_limiteCre.REF_CTR_DEM_LIM = db.T_CONTRAT.Where(p => p.REF_CTR_PAPIER_CTR == contrat.REF_CTR_PAPIER_CTR).Select(p => p.REF_CTR).FirstOrDefault();
                            demande_limiteCre.DEVISE = "TND";
                            demande_limiteCre.DELAIS_DEM_LIM = Dem_Ach_Reverse.DELAIS_DEM_LIM;
                            demande_limiteCre.DELAIS_ACC = Dem_Ach_Reverse.DELAIS_DEM_LIM;
                            demande_limiteCre.MODE_PAY_DEM_LIM = Dem_Ach_Reverse.MODE_PAY_DEM_LIM;
                            demande_limite.MODE_PAY_ACC = Dem_Ach_Reverse.MODE_PAY_DEM_LIM;
                            demande_limiteCre.REF_ACH_LIM = indiv;
                            db.T_DEM_LIMITE.Add(demande_limiteCre);
                            db.SaveChanges();

                        }


                    } catch (Exception) { }
                  




                    try
                    {
                        foreach (TJ_CIR dem in FourCir)
                        {
                           dem.REF_CTR_CIR = db.T_CONTRAT.Where(p => p.REF_CTR_PAPIER_CTR == contrat.REF_CTR_PAPIER_CTR).Select(p => p.REF_CTR).FirstOrDefault();
                            dem.ID_ROLE_CIR = "FRS";
                            db.TJ_CIR.Add(dem);
                            db.SaveChanges();
                        }
                    }
                    catch (Exception e) { TempData["error"] = TempData["error"] + "   Error d enregistrement demande limite "; }
                }
                else
                {

                    try
                    {
                        foreach (T_DEM_LIMITE dem in AchDem)
                        {
                            TJ_CIR ci = new TJ_CIR();
                            ci.REF_CTR_CIR = db.T_CONTRAT.Where(p => p.REF_CTR_PAPIER_CTR == contrat.REF_CTR_PAPIER_CTR).Select(p => p.REF_CTR).FirstOrDefault();
                            ci.REF_IND_CIR = (int)dem.REF_ACH_LIM;
                            ci.ID_ROLE_CIR = "ACH";
                            db.TJ_CIR.Add(ci);

                            dem.REF_CTR_DEM_LIM = db.T_CONTRAT.Where(p => p.REF_CTR_PAPIER_CTR == contrat.REF_CTR_PAPIER_CTR).Select(p => p.REF_CTR).FirstOrDefault();
                            dem.MONT_ACC = dem.MONT_DEM_LIM;
                            dem.DELAIS_ACC = dem.DELAIS_DEM_LIM;
                            dem.MODE_PAY_ACC = dem.MODE_PAY_DEM_LIM;
                            dem.DEVISE = "TND";
                            dem.ACTIF_DEM_LIMI = true;
                            db.T_DEM_LIMITE.Add(dem);
                            db.SaveChanges();
                        }
                    }
                    catch (Exception e) { TempData["error"] = TempData["error"] + "   Error d enregistrement demande limite "; }

                }
                    //frais
                    try
                    {
                        T_MVT_DEBIT deb = new T_MVT_DEBIT();
                        deb.REF_CTR_DEBIT = db.T_CONTRAT.Where(p => p.REF_CTR_PAPIER_CTR == contrat.REF_CTR_PAPIER_CTR).Select(p => p.REF_CTR).FirstOrDefault();
                        deb.ABEV_DEBIT = "CTR";
                        decimal montantfraisf = (decimal)db.T_FRAIS_DIVERS.Where(p => p.TYP_FRAIS_DIVERS == "SurC" && p.REF_CTR_FRAIS_DIVERS == deb.REF_CTR_DEBIT).Select(p => p.MONT_PAR_UNIT_FRAIS_DIVERS).FirstOrDefault();
                        deb.MONT_DEBIT = montantfraisf + (montantfraisf * 0.19m);
                        deb.DATE_DEBIT = DateTime.Now;
                        db.T_MVT_DEBIT.Add(deb);
                        db.SaveChanges();
                    }
                    catch (Exception) { TempData["error"] = TempData["error"] + "   Problème d'ajout frais création contrat"; }

                    //****************
                    try
                    {
                        int ref_ctr = db.T_CONTRAT.Where(p => p.REF_CTR_PAPIER_CTR == contrat.REF_CTR_PAPIER_CTR).Select(p => p.REF_CTR).FirstOrDefault();
                        decimal montantfraisf = (decimal)db.T_FRAIS_DIVERS.Where(p => p.TYP_FRAIS_DIVERS == "SurC" && p.REF_CTR_FRAIS_DIVERS == ref_ctr).Select(p => p.MONT_PAR_UNIT_FRAIS_DIVERS).FirstOrDefault();
                        int max = db.T_EC_CPT.Max(p => p.ID_EC_CPT);
                        T_EC_CPT cp1 = new T_EC_CPT();
                        T_EC_CPT cp2 = new T_EC_CPT();
                        T_EC_CPT cp3 = new T_EC_CPT();
                        T_EC_CPT cp4 = new T_EC_CPT();
                        cp1.ID_EC_CPT = (max += 1);
                        cp1.ANNEE_EC_CPT = DateTime.Now.Year;
                        cp1.CODE_DEP_EC_CPT = "A301";
                        cp1.NUM_LIGNE_EC_CPT = 1;
                        cp1.CODE_JOURNAL_EC_CPT = "JV";
                        cp1.DATE_SAISIE_EC_CPT = DateTime.Now;
                        cp1.DATE_EFFET_EC_CPT = contrat.DAT_SIGN_CTR;
                        cp1.COMPTE_GEN_EC_CPT = "40900000";
                        cp1.MONTANT_EC_CPT = montantfraisf+(montantfraisf*0.19m);
                        cp1.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                        cp1.REF_PIECE_EC_CPT = ref_ctr.ToString();
                        cp1.TYPE_TRANSACTION_EC_CPT = "SO";
                        cp1.TYPE_DOC_EC_CPT = "I";
                        cp1.NOM_USER_EC_CPT = "Admin"; // a changer 
                        cp1.LOT_EC_CPT = ref_ctr.ToString();
                        cp1.CODE_TIERS_EC_CPT = db.TJ_CIR.Where(p=>p.ID_ROLE_CIR=="ADH" && p.REF_CTR_CIR==ref_ctr).Select(p=>p.REF_IND_CIR).SingleOrDefault().ToString();
                        cp1.DOMAINE_EC_CPT = "medfact";
                        cp1.DATE_EC_CPT = DateTime.Now;
                        cp1.INTITULE_EC_CPT = "Commission sur contrat"+ref_ctr;
                        /************************************************************/

                        cp2.ID_EC_CPT = cp1.ID_EC_CPT+1;
                        cp2.ANNEE_EC_CPT = DateTime.Now.Year;
                        cp2.CODE_DEP_EC_CPT = "A301";
                        cp2.NUM_LIGNE_EC_CPT = 2;
                        cp2.CODE_JOURNAL_EC_CPT = "JV";
                        cp2.DATE_SAISIE_EC_CPT = DateTime.Now;
                        cp2.DATE_EFFET_EC_CPT = contrat.DAT_SIGN_CTR;
                        cp2.COMPTE_GEN_EC_CPT = "70501300";
                        cp2.MONTANT_EC_CPT =0- montantfraisf ;
                        cp2.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                        cp2.REF_PIECE_EC_CPT = ref_ctr.ToString();
                        cp2.TYPE_TRANSACTION_EC_CPT = "SO";
                        cp2.TYPE_DOC_EC_CPT = "I";
                        cp2.NOM_USER_EC_CPT = "Admin"; // a changer 
                        cp2.LOT_EC_CPT = ref_ctr.ToString();
                        cp2.CODE_TIERS_EC_CPT = db.TJ_CIR.Where(p => p.ID_ROLE_CIR == "ADH" && p.REF_CTR_CIR == ref_ctr).Select(p => p.REF_IND_CIR).SingleOrDefault().ToString();
                        cp2.DOMAINE_EC_CPT = "medfact";
                        cp2.DATE_EC_CPT = DateTime.Now;
                        cp2.INTITULE_EC_CPT = "Montant commission contrat";

                        /***********************************************************************************/

                        cp3.ID_EC_CPT = cp2.ID_EC_CPT + 1;
                        cp3.ANNEE_EC_CPT = DateTime.Now.Year;
                        cp3.CODE_DEP_EC_CPT = "A301";
                        cp3.NUM_LIGNE_EC_CPT = 3;
                        cp3.CODE_JOURNAL_EC_CPT = "JV";
                        cp3.DATE_SAISIE_EC_CPT = DateTime.Now;
                        cp3.DATE_EFFET_EC_CPT = contrat.DAT_SIGN_CTR;
                        cp3.COMPTE_GEN_EC_CPT = "43700000";
                        cp3.MONTANT_EC_CPT = 0 - 5/10;
                        cp3.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                        cp3.REF_PIECE_EC_CPT = ref_ctr.ToString();
                        cp3.TYPE_TRANSACTION_EC_CPT = "SO";
                        cp3.TYPE_DOC_EC_CPT = "I";
                        cp3.NOM_USER_EC_CPT = "Admin"; // a changer 
                        cp3.LOT_EC_CPT = ref_ctr.ToString();
                        cp3.CODE_TIERS_EC_CPT = db.TJ_CIR.Where(p => p.ID_ROLE_CIR == "ADH" && p.REF_CTR_CIR == ref_ctr).Select(p => p.REF_IND_CIR).SingleOrDefault().ToString();
                        cp3.DOMAINE_EC_CPT = "medfact";
                        cp3.DATE_EC_CPT = DateTime.Now;
                        cp3.INTITULE_EC_CPT = "Droit de timbre contrat";
                        /**********************************************************************************************/
                        cp3.ID_EC_CPT = cp3.ID_EC_CPT + 1;
                        cp3.ANNEE_EC_CPT = DateTime.Now.Year;
                        cp3.CODE_DEP_EC_CPT = "A301";
                        cp3.NUM_LIGNE_EC_CPT = 4;
                        cp3.CODE_JOURNAL_EC_CPT = "JV";
                        cp3.DATE_SAISIE_EC_CPT = DateTime.Now;
                        cp3.DATE_EFFET_EC_CPT = contrat.DAT_SIGN_CTR;
                        cp3.COMPTE_GEN_EC_CPT = "43671000";
                        cp3.MONTANT_EC_CPT = 0 - montantfraisf*0.19m;
                        cp3.CODE_CENTRE_ANALYSE_EC_CPT = "zzzz";
                        cp3.REF_PIECE_EC_CPT = ref_ctr.ToString();
                        cp3.TYPE_TRANSACTION_EC_CPT = "SO";
                        cp3.TYPE_DOC_EC_CPT = "I";
                        cp3.NOM_USER_EC_CPT = "Admin"; // a changer 
                        cp3.LOT_EC_CPT = ref_ctr.ToString();
                        cp3.CODE_TIERS_EC_CPT = db.TJ_CIR.Where(p => p.ID_ROLE_CIR == "ADH" && p.REF_CTR_CIR == ref_ctr).Select(p => p.REF_IND_CIR).SingleOrDefault().ToString();
                        cp3.DOMAINE_EC_CPT = "medfact";
                        cp3.DATE_EC_CPT = DateTime.Now;
                        cp3.INTITULE_EC_CPT = "TVA sur commission contrat";
                        /*********************************/
                        db.T_EC_CPT.Add(cp1);
                        db.T_EC_CPT.Add(cp2);
                        db.T_EC_CPT.Add(cp3);
                        db.T_EC_CPT.Add(cp4);
                        db.SaveChanges();
                    }
                    catch (Exception) { TempData["error"] = TempData["error"] + "   Error d enregistrement T_EC_CPT "; }

                try
                {
                    T_EXTRAIT extrait = new T_EXTRAIT();
                    extrait.REF_CTR_EXTRAIT = contrat.REF_CTR;
                    extrait.LIB_EXTRAIT = "Contrat " + contrat.REF_CTR_PAPIER_CTR;
                    extrait.DEBIT_EXTRAIT = db.T_FRAIS_DIVERS.Where(p => p.REF_CTR_FRAIS_DIVERS == contrat.REF_CTR && p.TYP_FRAIS_DIVERS.Contains("SurC")).SingleOrDefault().MONT_PAR_UNIT_FRAIS_DIVERS; ;
                    extrait.CREDIT_EXTRAIT = 0;
                    extrait.AUTRE_EXTRAIT = 0;
                    extrait.ENCOURFACT_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Encours_Facture).FirstOrDefault();
                    extrait.DISPONIBLE_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Disponible).FirstOrDefault();
                    extrait.DAT_OP_EXTRAIT = DateTime.Now;
                    extrait.DAT_VAL_EXTRAIT = contrat.DAT_SIGN_CTR;
                    extrait.TOTAL_FIN_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Total_Financement).FirstOrDefault();
                    db.T_EXTRAIT.Add(extrait);
                    db.SaveChanges();

                }
                catch (Exception) { }

                try
                {
                    T_HISTORIQUE histo = new T_HISTORIQUE();
                    histo.ABREV_ROLE_HIST = "Creation Contrat " + contrat.REF_CTR_PAPIER_CTR;
                    histo.ACTION = "Creation Contrat";
                    histo.ID_ENREGISTREMENT = db.T_CONTRAT.Max().ToString();
                    histo.T_TABLE = "T_CONTRAT";
                    histo.REF_CTR_HIST = contrat.REF_CTR.ToString();
                    histo.REF_IND_HIST = db.TJ_CIR.Where(p=>p.REF_CTR_CIR== contrat.REF_CTR && p.ID_ROLE_CIR=="ADH").FirstOrDefault().REF_IND_CIR.ToString();
                    histo.LOGIN_USER = Session["UserLogin"].ToString();
                    histo.IP_PC = HttpContext.Request.UserHostAddress;
                    histo.NOM_PC = HttpContext.Request.UserHostName;
                    histo.DATE_ACTION = DateTime.Today;
                    db.T_HISTORIQUE.Add(histo);
                    db.SaveChanges();
                }
                catch (Exception) { }
                // db.SaveChanges();
                // T_INDIVIDU individu = db.T_INDIVIDU.Where(p => p.REF_IND == indiv).First();

                TempData["notice"] = "le contrat a ete enregistré avec succes N°" + db.T_CONTRAT.Where(p => p.REF_CTR_PAPIER_CTR == contrat.REF_CTR_PAPIER_CTR).Select(p => p.REF_CTR).FirstOrDefault()+" ";
                TempData["Link"] = "/CreationContrat/creationWordDoc/" + db.T_CONTRAT.Where(p => p.REF_CTR_PAPIER_CTR == contrat.REF_CTR_PAPIER_CTR).Select(p => p.REF_CTR).FirstOrDefault();
                return RedirectToAction("Creation");
               
            }
            else {

                TempData["error"] = "merci de vérifier la référence papier de contrat car il est déjà enregistré dans la base de données"; return RedirectToAction("InternalServerError", "Error");

            }
        }

        private void FindAndReplace(Microsoft.Office.Interop.Word.Application doc, object findText, object replaceWithText)
        {
            //options
            object matchCase = false;
            object matchWholeWord = true;
            object matchWildCards = false;
            object matchSoundsLike = false;
            object matchAllWordForms = false;
            object forward = true;
            object format = false;
            object matchKashida = false;
            object matchDiacritics = false;
            object matchAlefHamza = false;
            object matchControl = false;
            object read_only = false;
            object visible = true;
            object replace = 2;
            object wrap = 1;
            //execute find and replace
            doc.Selection.Find.Execute(ref findText, ref matchCase, ref matchWholeWord,
                ref matchWildCards, ref matchSoundsLike, ref matchAllWordForms, ref forward, ref wrap, ref format, ref replaceWithText, ref replace,
                ref matchKashida, ref matchDiacritics, ref matchAlefHamza, ref matchControl);
        }

        public ActionResult creationWordDoc(int id)
        {
            try
            {
                try
                {
                    System.Diagnostics.Process[] proc = System.Diagnostics.Process.GetProcessesByName("winword");
                    proc[0].Kill();
                }
                catch { }
                T_CONTRAT contra = db.T_CONTRAT.Where(q => q.REF_CTR == id).FirstOrDefault();
                string namefile = contra.REF_CTR_PAPIER_CTR.Replace("/", "-") + "_" + DateTime.Now.ToString("dd-MM-yyyy");
                var ind = (from q in db.T_INDIVIDU
                           join q2 in db.TJ_CIR
                           on q.REF_IND equals q2.REF_IND_CIR
                           where (q2.REF_CTR_CIR == id)
                           select new { q.REF_IND, q.NOM_IND, q.ADR_IND, q.RC_IND, q.MF_IND }).FirstOrDefault();

                string savePath = Server.MapPath("/Content/" + namefile + ".doc");
                object templatePath = Server.MapPath("/Content/model_Contrat_Med_facto.doc");
                //object missing = Missing.Value;
                //Microsoft.Office.Interop.Word.Application app = new Microsoft.Office.Interop.Word.Application();
                //Microsoft.Office.Interop.Word.Document doc = new Microsoft.Office.Interop.Word.Document();
                //if (System.IO.File.Exists((string)templatePath))
                //{

                //    object readOnly = false;
                //    object isVisible = false;
                //    app.Visible = false;
                //    doc = app.Documents.Open(ref templatePath, ref missing, ref readOnly, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);
                //    doc.Activate();

                //    this.FindAndReplace(app, "<Adh>", ind.NOM_IND);
                //    this.FindAndReplace(app, "<AdrAdh>", ind.ADR_IND);
                //    this.FindAndReplace(app, "<CTRPap>", contra.REF_CTR_PAPIER_CTR);
                //    if (ind.RC_IND == "") { this.FindAndReplace(app, "<RC>", "... ... ... ... ... ..."); }
                //    else { this.FindAndReplace(app, "<RC>", ind.RC_IND); }
                //    if (ind.MF_IND == "") { this.FindAndReplace(app, "<MatFisc>", "... ... ... ... ... ..."); }
                //    else { this.FindAndReplace(app, "<MatFisc>", ind.MF_IND); }
                //    this.FindAndReplace(app, "<RC>", ind.RC_IND);
                //    this.FindAndReplace(app, "<MatFisc>", ind.MF_IND);
                //    // this.FindAndReplace(app, "<RespAdh>", rESP_INDTextBox1.Text.Trim());
                //    //this.FindAndReplace(app, "<FonctRespAdh>", fONCTION_INDTextBox1.Text.Trim());
                //    this.FindAndReplace(app, "<DatSigContrat>", DateTime.Now.ToString("dd-mm-yyyy"));
                //    this.FindAndReplace(app, "<ActAdh>", contra.SERVICE_CTR);
                //    this.FindAndReplace(app, "<DueMaxPaiAch>", contra.DELAI_MAX_REG_CTR);
                //    this.FindAndReplace(app, "<TxCommFact>", "");
                //    this.FindAndReplace(app, "<CACtr>", contra.CA_CTR);
                //    this.FindAndReplace(app, "<NBAchPrev>", contra.NB_ACH_PREVU_CTR);
                //    this.FindAndReplace(app, "<CommFactMin>", contra.MIN_COMM_FACT);
                //    this.FindAndReplace(app, "<TMM>", "TMM");
                //    this.FindAndReplace(app, "<DatTMM>", contra.DAT_SIGN_CTR);
                //    this.FindAndReplace(app, "<DirecteurFactor>", "Thameur KOUBAA");
                //    this.FindAndReplace(app, "<DatJour>", contra.DAT_SIGN_CTR);
                //    //   return Json("bien");
                //}
                //doc.SaveAs2(savePath);

                //doc.Close();
                //app.Application.Quit();

             
                Document document = new Document();
                document.LoadFromFile(Server.MapPath("/Content/model_Contrat_Med_facto.doc"));
                
                if (ind.NOM_IND == null || ind.NOM_IND == "") { document.Replace("<Adh>", "", false, true); } else { document.Replace("<Adh>", ind.NOM_IND, false, true); }
                if (ind.ADR_IND == null || ind.ADR_IND == "") { document.Replace("<AdrAdh>","", false, true); } else { document.Replace("<AdrAdh>", ind.ADR_IND.ToString(), false, true); }
                if (contra.REF_CTR_PAPIER_CTR == null || contra.REF_CTR_PAPIER_CTR == "") { document.Replace("<CTRPap>", "", false, true); } else { document.Replace("<CTRPap>", contra.REF_CTR_PAPIER_CTR, false, true); }

              

                if (ind.RC_IND == "" || ind.RC_IND==null) { document.Replace("<RC>", "... ... ... ... ... ...", false, true); }
                else {document.Replace("<RC>", ind.RC_IND, false, true); }
                if (ind.MF_IND == "" || ind.MF_IND==null) { document.Replace("<MatFisc>", "... ... ... ... ... ...", false, true); }
                else {document.Replace("<MatFisc>", ind.MF_IND, false, true); }

               document.Replace("<RespAdh>", "", false, true); 
               document.Replace("<FonctRespAdh>", "", false, true); 

                document.Replace("<DatSigContrat>", DateTime.Now.ToString("dd-mm-yyyy"), false, true);

                if (contra.SERVICE_CTR == null || contra.SERVICE_CTR.ToString() == "") { document.Replace("<ActAdh>", "", false, true); } else { document.Replace("<ActAdh>", contra.SERVICE_CTR.ToString(), false, true); }
                if (contra.DELAI_MAX_REG_CTR == null || contra.DELAI_MAX_REG_CTR.ToString() == "") { document.Replace("<DueMaxPaiAch>", "", false, true); } else { document.Replace("<DueMaxPaiAch>", contra.DELAI_MAX_REG_CTR.ToString(), false, true); }
            

                document.Replace("<TxCommFact>","", false, true);
                if (contra.CA_CTR == null || contra.CA_CTR.ToString() == "") { document.Replace("<CACtr>", "", false, true); } else { document.Replace("<CACtr>", contra.CA_CTR.ToString(), false, true); }
                if (contra.NB_ACH_PREVU_CTR == null || contra.NB_ACH_PREVU_CTR.ToString() == "") { document.Replace("<NBAchPrev>", "", false, true); } else { document.Replace("<NBAchPrev>", contra.NB_ACH_PREVU_CTR.ToString(), false, true); }

                if (contra.MIN_COMM_FACT == null || contra.MIN_COMM_FACT.ToString() == "") { document.Replace("<CommFactMin>", "", false, true); } else { document.Replace("<CommFactMin>", contra.MIN_COMM_FACT.ToString(), false, true); }
               
                document.Replace("<TMM>","TMM", false, true);
                if (contra.DAT_SIGN_CTR == null || contra.DAT_SIGN_CTR.ToString() == "") { document.Replace("<DatTMM>", "", false, true); } else { document.Replace("<DatTMM>", contra.DAT_SIGN_CTR.ToString(), false, true); }
              
                document.Replace("<DirecteurFactor>", "Thameur KOUBAA", false, true);
             
                if (contra.DAT_SIGN_CTR == null || contra.DAT_SIGN_CTR.ToString() == "") { document.Replace("<DatJour>", "", false, true); } else { document.Replace("<DatJour>", contra.DAT_SIGN_CTR.ToString(), false, true); }
                document.SaveToFile(Server.MapPath("/Content/" + namefile + ".doc"), FileFormat.Doc);
                document.Close();
               // System.Diagnostics.Process.Start("Replace.docx");
                byte[] imgByteArr = System.IO.File.ReadAllBytes(savePath);
                        return File(imgByteArr, System.Net.Mime.MediaTypeNames.Application.Octet, namefile+ ".doc");

            }
            catch (Exception e)
            {
                TempData["error"] = e.Message;
                RedirectToAction("InternalServerError", "Error");
                return Json("err");

            }
        }

       


        public JsonResult AjouterAll(List<T_FRAIS_DIVERS> fins, List<T_FRAIS_PAIEMENT> pais, List<T_FOND_GARANTIE> Fonds, List<T_COMM_FACTORING> facts, List<T_INT_FINANCEMENT> intfins, List<T_DEM_LIMITE> AchDem)
        {
            int rec = 0;
            if (VerifRefMaxVide() == false)
            {
                foreach (T_FRAIS_DIVERS fin in fins)
                {
                    db.T_FRAIS_DIVERS.Add(fin);
                    db.SaveChanges();
                }
                foreach (T_FRAIS_PAIEMENT pai in pais)
                {
                    db.T_FRAIS_PAIEMENT.Add(pai);
                    db.SaveChanges();
                }
                foreach (T_FOND_GARANTIE fond in Fonds)
                {
                    db.T_FOND_GARANTIE.Add(fond);
                    db.SaveChanges();
                }
                foreach (T_COMM_FACTORING fact in facts)
                {

                    db.T_COMM_FACTORING.Add(fact);
                    db.SaveChanges();
                }
                foreach (T_INT_FINANCEMENT intfin in intfins)
                {
                    db.T_INT_FINANCEMENT.Add(intfin);
                    db.SaveChanges();
                }
                //rec = db.SaveChanges();


            }
            return Json(rec);
        }
        public string ajouterAch(List<T_DEM_LIMITE> AchDem, List<TJ_CIR> AchCir)
        {
            string ch = "";
            try
            {
                //int x = db.T_CONTRAT.Select(t => t.REF_CTR).Max()+1;
                foreach (TJ_CIR Ach in AchCir)
                {
                    Ach.ID_ROLE_CIR = "ACH";
                    //Ach.REF_CTR_CIR = (int)ViewData["REfCTR"];
                    db.TJ_CIR.Add(Ach);
                }
                db.SaveChanges();
                foreach (T_DEM_LIMITE dem in AchDem)
                {
                    //dem.REF_CTR_DEM_LIM = (int)ViewData["REfCTR"];
                    db.T_DEM_LIMITE.Add(dem);
                }
                db.SaveChanges();

                ch = "bien enregistrer";
            }
            catch { ch = "non"; }
            return ch;
        }

        public JsonResult SelectAch(int id)
        {

            T_INDIVIDU individu = new T_INDIVIDU();
            individu = db.T_INDIVIDU.Where(p => p.REF_IND == id).Single();
            List<String> test = new List<string>();
            test.Add(individu.TYP_DOC_ID_IND);
            test.Add(individu.NUM_DOC_ID_IND);
            return Json(test, JsonRequestBehavior.AllowGet);
        }
        bool VerifRefMaxVide()
        {
            bool ver = false;

            int ref_max = db.T_CONTRAT.Select(p => p.REF_CTR).Max();

            if (db.T_CONTRAT.Where(p => p.REF_CTR == (ref_max + 1)).Count() != 0)
            {
                ver = true;
            }

            if (db.T_DET_ASS.Where(p => p.REF_CTR_ASS == (ref_max + 1)).Count() != 0)
            {
                ver = true;
            }
            if (db.T_DOC_GED.Where(p => p.ID_CTR_GED == (ref_max + 1)).Count() != 0)
            {
                ver = true;
            }
            if (db.T_MVT_DEBIT.Where(p => p.REF_CTR_DEBIT == (ref_max + 1)).Count() != 0)
            {
                ver = true;
            }
            if (db.T_FRAIS_DIVERS.Where(p => p.REF_CTR_FRAIS_DIVERS == (ref_max + 1)).Count() != 0)
            {
                ver = true;
            }
            if (db.T_COMM_FACTORING.Where(p => p.REF_CTR_COMM_FACTORING == (ref_max + 1)).Count() != 0)
            {
                ver = true;
            }
            if (db.T_INT_FINANCEMENT.Where(p => p.REF_CTR_INT_FIN == (ref_max + 1)).Count() != 0)
            {
                ver = true;
            }

            if (db.T_DEM_LIMITE.Where(p => p.REF_CTR_DEM_LIM == (ref_max + 1)).Count() != 0)
            {
                ver = true;
            }

            return ver;
        }

        public ActionResult ListeDesContrat()
        {
            //var contrat = from cir in db.TJ_CIR
            //              join ind in db.T_INDIVIDU on cir.REF_IND_CIR equals ind.REF_IND
            //              join ct in db.T_CONTRAT on cir.REF_CTR_CIR equals ct.REF_CTR
            //              where (cir.ID_ROLE_CIR == "ADH" && cir.REF_CTR_CIR > 26)
            //              orderby ind.NOM_IND
            //              select new
            //              {
            //                  Adhérent=ind.NOM_IND,

            //              ct,
            //              Encours_Factures=db.T_DET_BORD.Where(p=>p.REF_CTR_DET_BORD==cir.REF_CTR_CIR).Select(p=>p.MONT_OUV_DET_BORD).Sum(),
            //              Somme_Factures=db.T_DET_BORD.Where(p=>p.REF_CTR_DET_BORD==cir.REF_CTR_CIR).Select(p=>p.MONT_TTC_DET_BORD).Sum(),
            //              Somme_Bordereaux=db.T_BORDEREAU.Where(p=>p.REF_CTR_BORD==cir.REF_CTR_CIR).Select(p=>p.MONT_TOT_BORD).Sum(),
            //              Nombre_Factures=db.T_DET_BORD.Where(p => p.REF_CTR_DET_BORD == cir.REF_CTR_CIR).Count(),
            //              Nombre_Bordereaux=db.T_BORDEREAU.Where(p=>p.REF_CTR_BORD==cir.REF_CTR_CIR).Count(),
            //              Nombre_Acheteurs=db.T_DET_BORD.Where(p=>p.REF_CTR_DET_BORD==cir.REF_CTR_CIR).Select(p=>p.REF_IND_DET_BORD).Distinct().Count(),
            //              Totla_Encaissements=db.T_ENCAISSEMENT.Where(p=>p.REF_CTR_ENC==cir.REF_CTR_CIR).Select(p=>p.MONT_ENC).Sum(),
            //              Totlal_Avoirs= db.T_ENCAISSEMENT.Where(p => p.REF_CTR_ENC == cir.REF_CTR_CIR && p.TYP_ENC=="A").Select(p => p.MONT_ENC).Sum(),
            //              Total_Fianancements=db.T_FINANCEMENT.Where(p=>p.REF_CTR_FIN==cir.REF_CTR_CIR).Select(p=>p.MONT_FIN).Sum(),
            //              Total_interet=db.T_CALC_DISPO.Where(p=>p.REF_CTR_CALC_DISPO==cir.REF_CTR_CIR).Select(p=>p.INTERET_J_CALC_DISPO).Sum()
            //              };
            if (Session["UserLogin"] != null)
            {
                ViewBag.ListCtr = db.ListeDesContrats().ToList();
                TempData["CreationContrat"] = "active";
                TempData["ListeDesContrat"] = "active";
                return View();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }

        }
        [HttpPost]
        public JsonResult updatecontrat(T_CONTRAT contart)
        {

            T_CONTRAT ctr = new T_CONTRAT();
            ctr = db.T_CONTRAT.Where(p => p.REF_CTR == contart.REF_CTR).FirstOrDefault();
            ctr.STATUT_CTR = contart.STATUT_CTR;
            ctr.REF_CTR_PAPIER_CTR = contart.REF_CTR_PAPIER_CTR;
            ctr.DAT_RESIL_CTR = contart.DAT_RESIL_CTR;
            ctr.DAT_PROCH_VERS_CTR = contart.DAT_PROCH_VERS_CTR;
            ctr.CA_CTR = contart.CA_CTR;
            ctr.CA_EXP_CTR = contart.CA_EXP_CTR;
            ctr.CA_IMP_CTR = contart.CA_IMP_CTR;
            ctr.NB_ACH_PREVU_CTR = contart.NB_ACH_PREVU_CTR;
            ctr.NB_AVOIRS_PREVU_CTR = contart.NB_AVOIRS_PREVU_CTR;
            ctr.NB_FACT_PREVU_CTR = contart.NB_FACT_PREVU_CTR;
            ctr.NB_REMISES_PREVU_CTR = contart.NB_REMISES_PREVU_CTR;
            ctr.DELAI_MAX_REG_CTR = contart.DELAI_MAX_REG_CTR;
            ctr.DELAI_MOYEN_REG_CTR = contart.DELAI_MOYEN_REG_CTR;
            ctr.FACT_REG_CTR = contart.FACT_REG_CTR;
            ctr.DERN_MONT_DISP_2 = contart.DERN_MONT_DISP_2;
            db.Entry(ctr).State = EntityState.Modified;
            int rec = db.SaveChanges();
            return Json(rec);
        }


        [HttpPost]
        public JsonResult updateFdg(string[] info)
        {
            T_FOND_GARANTIE fdg = new T_FOND_GARANTIE();
            int ref_ctr = int.Parse(info[1]);
            string typ = info[2];
            fdg = db.T_FOND_GARANTIE.Where(p => p.REF_CTR_FDG == ref_ctr && p.TYP_FDG == typ).FirstOrDefault();
            fdg.TX_FDG = decimal.Parse(info[0].Replace(".",","));
            db.Entry(fdg).State = EntityState.Modified;
            db.SaveChanges();
            return Json(db.T_FOND_GARANTIE.Where(p => p.REF_CTR_FDG == ref_ctr).ToList());
        }

        [HttpPost]
        public JsonResult updateFraisP(string[] info)
        {
            T_FRAIS_PAIEMENT pai = new T_FRAIS_PAIEMENT();
            int ref_ctr = int.Parse(info[1]);
            string typ = info[2];
            pai = db.T_FRAIS_PAIEMENT.Where(p => p.REF_CTR_FRAIS_PAIE == ref_ctr && p.TYP_FRAIS_PAIE == typ).FirstOrDefault();
            pai.MONT_PAR_INSTR_FRAIS_PAIE = decimal.Parse(info[0].Replace(".",","));
            db.Entry(pai).State = EntityState.Modified;
            db.SaveChanges();
            return Json(db.T_FRAIS_PAIEMENT.Where(p => p.REF_CTR_FRAIS_PAIE == ref_ctr).ToList());
        }

        [HttpPost]
        public JsonResult updateFraisD(string[] info)
        {
            T_FRAIS_DIVERS diver = new T_FRAIS_DIVERS();
            int ref_ctr = int.Parse(info[1]);
            string typ = info[2];
            diver = db.T_FRAIS_DIVERS.Where(p => p.REF_CTR_FRAIS_DIVERS == ref_ctr && p.TYP_FRAIS_DIVERS == typ).FirstOrDefault();
            diver.MONT_PAR_UNIT_FRAIS_DIVERS = decimal.Parse(info[0].Replace(".", ","));
            db.Entry(diver).State = EntityState.Modified;
            db.SaveChanges();
            return Json(db.T_FRAIS_DIVERS.Where(p => p.REF_CTR_FRAIS_DIVERS == ref_ctr).ToList());
        }


        [HttpPost]
        public JsonResult updateCommF(string[] info)
        {
            T_COMM_FACTORING diver = new T_COMM_FACTORING();
            int ref_ctr = int.Parse(info[1]);
            string typ = info[4];
            diver = db.T_COMM_FACTORING.Where(p => p.REF_CTR_COMM_FACTORING == ref_ctr && p.TYP_COMM_FACTORING == typ).FirstOrDefault();
            diver.TX_COMM_FACTORING = decimal.Parse(info[0].Replace(".", ","));
            diver.MONT_MIN_DOC_COMM_FACTORING = decimal.Parse(info[2].Replace(".", ","));
            diver.MONT_MIN_CTR_COMM_FACTORING = decimal.Parse(info[3].Replace(".", ","));
            db.Entry(diver).State = EntityState.Modified;
            db.SaveChanges();
            return Json(db.T_COMM_FACTORING.Where(p => p.REF_CTR_COMM_FACTORING == ref_ctr).ToList());
        }



        [HttpPost]
        public JsonResult updateInteretFin(string[] info)
        {
            T_INT_FINANCEMENT diver = new T_INT_FINANCEMENT();
            int ref_ctr = int.Parse(info[1]);
            string typ = info[8];
            diver = db.T_INT_FINANCEMENT.Where(p => p.REF_CTR_INT_FIN == ref_ctr && p.TYP_INSTR_INT_FIN == typ).FirstOrDefault();
            if (info[0] != "" && info[0] != "null")
            {
                diver.TX_INT_MARCHE_INT_FIN = decimal.Parse(info[0].Replace(".", ","));
            }
            else
            {
                diver.TX_INT_MARCHE_INT_FIN = null;
            }
            if (info[2] != "" && info[2] != "null")
            {
                diver.TX_MARGE_CTR_INT_FIN = decimal.Parse(info[2].Replace(".", ","));
            }
            else
            {
                diver.TX_MARGE_CTR_INT_FIN = null;
            }
            if (info[3] != "" && info[3] != "null")
            {
                diver.DELAI_MAX_PAI_INT_FIN = short.Parse(info[3]);
            }
            else {
                diver.DELAI_MAX_PAI_INT_FIN = null;
            }
            if (info[4] != "" & info[4] != "null")
            {
                diver.PRECOMPTE_INT_FIN = decimal.Parse(info[4].Replace(".", ","));
            }
            else { diver.PRECOMPTE_INT_FIN = null; }
            if (info[5] != "" && info[5] != "null") {
                diver.MAJOR_INT_INT_FIN = decimal.Parse(info[5].Replace(".", ","));
            }
            else
            {
                diver.MAJOR_INT_INT_FIN = null;
            }
            diver.DAT_DEB_VALID_INT_FIN = DateTime.Parse(info[6]);
            db.Entry(diver).State = EntityState.Modified;
            db.SaveChanges();
            return Json(db.T_INT_FINANCEMENT.Where(p => p.REF_CTR_INT_FIN == ref_ctr).ToList(),JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetAllRecord(int ref_ctr)
        {
            if (Session["UserLogin"] != null)
            {
                T_CONTRAT contrat = db.T_CONTRAT.Find(ref_ctr);

                var ctr = new
                {
                    REF_CTR = contrat.REF_CTR,
                    STATUT_CTR = contrat.STATUT_CTR,
                    REF_CTR_PAPIER_CTR = contrat.REF_CTR_PAPIER_CTR,
                    DAT_RESIL_CTR = contrat.DAT_RESIL_CTR,
                    DAT_PROCH_VERS_CTR = contrat.DAT_PROCH_VERS_CTR,
                    CA_CTR = contrat.CA_CTR,
                    SERVICE_CTR = contrat.SERVICE_CTR,
                    CA_EXP_CTR = contrat.CA_EXP_CTR,
                    CA_IMP_CTR = contrat.CA_IMP_CTR,
                    NB_ACH_PREVU_CTR = contrat.NB_ACH_PREVU_CTR,
                    NB_FACT_PREVU_CTR = contrat.NB_FACT_PREVU_CTR,
                    NB_AVOIRS_PREVU_CTR = contrat.NB_AVOIRS_PREVU_CTR,
                    NB_REMISES_PREVU_CTR = contrat.NB_REMISES_PREVU_CTR,
                    DELAI_MOYEN_REG_CTR = contrat.DELAI_MOYEN_REG_CTR,
                    DELAI_MAX_REG_CTR = contrat.DELAI_MAX_REG_CTR,
                    FACT_REG_CTR = contrat.FACT_REG_CTR,
                    DERN_MONT_DISP_2 = contrat.DERN_MONT_DISP_2,


                };


                var AllRec = new
                {
                    ctr,
                    fraisd = db.T_FRAIS_DIVERS.Where(p => p.REF_CTR_FRAIS_DIVERS == ref_ctr).ToList(),
                    fraisp = db.T_FRAIS_PAIEMENT.Where(p => p.REF_CTR_FRAIS_PAIE == ref_ctr).ToList(),
                    fondg = db.T_FOND_GARANTIE.Where(p => p.REF_CTR_FDG == ref_ctr).ToList(),
                    commfact = db.T_COMM_FACTORING.Where(p => p.REF_CTR_COMM_FACTORING == ref_ctr),
                    fin = db.T_INT_FINANCEMENT.Where(p => p.REF_CTR_INT_FIN == ref_ctr).ToList(),
                    int_ctr = db.Recherche_CTR_ADH_ACH_Par_CTR2019(ref_ctr).ToList(),
                    nom_adh=db.T_INDIVIDU.Find(db.Recherche_CTR_ADH_ACH_Par_CTR2019(ref_ctr).Select(p=>p.RefAdh).FirstOrDefault()).NOM_IND
                };
                return Json(AllRec, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // RedirectToAction("login", "Login");
                return Json(RedirectToAction("login", "Login"));
            }
        }
        [HttpGet]
        public JsonResult GetValCtr(int ref_ctr)
        {
            T_CONTRAT ctr = db.T_CONTRAT.Where(p => p.REF_CTR == ref_ctr).First();
            return Json(ctr, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AjouterAchteur()
        {
            if (Session["UserLogin"] != null)
            {
                //var ListNomInd = (from q in db.T_INDIVIDU
                //                  join q2 in db.TJ_CIR
                //                  on q.REF_IND equals q2.REF_IND_CIR
                //                  where (q2.ID_ROLE_CIR == "ADH" && q2.REF_CTR_CIR != 28 && q2.REF_CTR_CIR != 32 && q2.REF_CTR_CIR != 38 && q2.REF_CTR_CIR != 54 && q2.REF_CTR_CIR != 56 && q2.REF_CTR_CIR != 58 && q2.REF_CTR_CIR != 65 && q2.REF_CTR_CIR != 69 && q2.REF_CTR_CIR != 72 && q2.REF_CTR_CIR != 74 && q2.REF_CTR_CIR != 80 && q2.REF_CTR_CIR != 82 && q2.REF_CTR_CIR != 93 && q2.REF_CTR_CIR != 96)
                //                  select new { q.PRE_IND, q2.REF_CTR_CIR, q.REF_IND });
                //List<NewDataCollection> op = new List<NewDataCollection>();
                //foreach (var data in ListNomInd)
                //{
                //    op.Add(new NewDataCollection(data.REF_CTR_CIR, data.PRE_IND+"|"+data.REF_CTR_CIR, data.REF_IND));
                //}

                ViewBag.ListNomInd = db.Recherche_CTR_ADH().ToList();

                ViewBag.ListInd = new SelectList(db.T_INDIVIDU, "REF_IND", "NOM_IND");
            
                TempData["AjouterAchteur"] = "active";
                return View();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }


        public ActionResult AjouterAchteurJson(String[] info)
        {
            try
            {
                string message = "";
                TJ_CIR cir = new TJ_CIR();
                cir.ID_CIR = db.TJ_CIR.Select(p => p.ID_CIR).Max() + 1;
                cir.REF_CTR_CIR = int.Parse(info[0]);
                cir.REF_IND_CIR = int.Parse(info[1]);
                cir.ID_ROLE_CIR = "ACH";
                db.TJ_CIR.Add(cir);
                message = "tj_cir is bien sauv";
                db.SaveChanges();
                T_INDIVIDU ind = new T_INDIVIDU();
                ind = db.T_INDIVIDU.Find(int.Parse(info[1]));
                T_DOC_GED ged = new T_DOC_GED();
                T_DOC_GED ged2 = new T_DOC_GED();
                if (ind.GENRE_IND.Contains("PP"))
                {
        

                    db.Database.ExecuteSqlCommand("insert into T_DOC_GED (ID_CTR_GED,ID_IND_GED,LIBELLE_GED,DATE_TACHE_GED,LIBELLE2_GED,Etape_ged,Etat_GED) values({0},{1},{2},{3},{4},{5},{6})", int.Parse(info[0]), int.Parse(info[1]), ind.TYP_DOC_ID_IND, DateTime.Today, ind.NUM_DOC_ID_IND, "Création", false);

                    message += "/ doc_ged type pp bien sauve";
                }
                else
                if (ind.GENRE_IND.Contains("PM"))
                {
                  
                    db.Database.ExecuteSqlCommand("insert into T_DOC_GED (ID_CTR_GED,ID_IND_GED,LIBELLE_GED,DATE_TACHE_GED,LIBELLE2_GED,Etape_ged,Etat_GED) values({0},{1},{2},{3},{4},{5},{6})", int.Parse(info[0]), int.Parse(info[1]), "Registre De Commerce", DateTime.Today, ind.NUM_DOC_ID_IND, "Création", false);


                    db.Database.ExecuteSqlCommand("insert into T_DOC_GED (ID_CTR_GED,ID_IND_GED,LIBELLE_GED,DATE_TACHE_GED,LIBELLE2_GED,Etape_ged,Etat_GED) values({0},{1},{2},{3},{4},{5},{6})", int.Parse(info[0]), int.Parse(info[1]), "Matricle Fiscale", DateTime.Today, ind.MF_IND, "Création", false);

                    message += "/ doc ged pm bien save ";
                }
                T_DEM_LIMITE dem1 = new T_DEM_LIMITE();
                if (db.T_CONTRAT.Find(int.Parse(info[0])).TYP_CTR == "3")
                {
                    T_DEM_LIMITE dem = new T_DEM_LIMITE();
                    dem.REF_CTR_DEM_LIM = int.Parse(info[0]);
                    dem.TYP_DEM_LIM = "CRE";
                    dem.DATLIM_DEM_LIM = DateTime.ParseExact(info[5], "yyyy-MM-dd", null); ;
                    dem1.DAT_DEM_LIM = DateTime.ParseExact(info[5], "yyyy-MM-dd", null);
                    dem.MONT_DEM_LIM = decimal.Parse(info[2].Replace(".", ","));
                    dem.MONT_ACC = decimal.Parse(info[2].Replace(".", ","));
                    dem.DEVISE = "TND";
                    dem.REF_ACH_LIM = int.Parse(info[1]);
                    dem.ACTIF_DEM_LIMI = true;
                    db.T_DEM_LIMITE.Add(dem);
                    db.SaveChanges();
                    message += "/ ddem de type 3 bien save ";
                }
                dem1.REF_CTR_DEM_LIM = int.Parse(info[0]);
                dem1.TYP_DEM_LIM = "FIN";
                dem1.DATLIM_DEM_LIM = DateTime.ParseExact(info[5], "yyyy-MM-dd", null);
                dem1.DAT_DEM_LIM = DateTime.ParseExact(info[5], "yyyy-MM-dd", null);
                dem1.MONT_DEM_LIM = decimal.Parse(info[4].Replace(".",","));
                dem1.MONT_ACC = decimal.Parse(info[4].Replace(".", ","));
                dem1.DEVISE = "TND";
                dem1.REF_ACH_LIM = int.Parse(info[1]);
                dem1.ACTIF_DEM_LIMI = true;
                db.T_DEM_LIMITE.Add(dem1);



                //Historique 21072020



                try
                {
                    T_HISTORIQUE histo = new T_HISTORIQUE();
                    histo.ABREV_ROLE_HIST = "Saisi limite " + dem1.REF_CTR_DEM_LIM;
                    histo.ACTION = "Saisi limite";
                    histo.ID_ENREGISTREMENT = dem1.REF_DEM_LIM.ToString();
                    histo.T_TABLE = "T_DEM_LIMITE";
                    histo.REF_CTR_HIST = dem1.REF_CTR_DEM_LIM.ToString();
                    histo.REF_IND_HIST = dem1.REF_ACH_LIM.ToString();
                    histo.LOGIN_USER = Session["UserLogin"].ToString();
                    histo.IP_PC = HttpContext.Request.UserHostAddress;
                    histo.NOM_PC = HttpContext.Request.UserHostName;
                    histo.DATE_ACTION = DateTime.Today;
                    db.T_HISTORIQUE.Add(histo);
                    db.SaveChanges();
                }
                catch (Exception) { }


                try
                {
                    T_HISTORIQUE histo = new T_HISTORIQUE();
                    histo.ABREV_ROLE_HIST = "Valide Limite " + dem1.REF_CTR_DEM_LIM;
                    histo.ACTION = "Saisi limite";
                    histo.ID_ENREGISTREMENT = dem1.REF_DEM_LIM.ToString();
                    histo.T_TABLE = "T_DEM_LIMITE";
                    histo.REF_CTR_HIST = dem1.REF_CTR_DEM_LIM.ToString();
                    histo.REF_IND_HIST = dem1.REF_ACH_LIM.ToString();
                    histo.LOGIN_USER = Session["UserLogin"].ToString();
                    histo.IP_PC = HttpContext.Request.UserHostAddress;
                    histo.NOM_PC = HttpContext.Request.UserHostName;
                    histo.DATE_ACTION = DateTime.Today;
                    db.T_HISTORIQUE.Add(histo);
                    db.SaveChanges();
                }
                catch (Exception) { }



                //


                db.SaveChanges();
                int ref_ind = int.Parse(info[1]);
                string nom_ind = db.T_INDIVIDU.Where(p => p.REF_IND == ref_ind).Select(p => p.NOM_IND).SingleOrDefault();
                TempData["MessageAch"] = "l Acheteur " + nom_ind + " a été effectué a contrat N° " + cir.REF_CTR_CIR;
                // message += "/ ddem  bien save ";
                return Json(db.TR_LIST_ACHTEURS_PAR_CTR(int.Parse(info[0])), JsonRequestBehavior.AllowGet);
            }
            catch (Exception) { TempData["error"] = "merci de bien vérifier que l'acheteur n'est pas enregistré dans ce contrat.Si non merci de répiter l action "; return RedirectToAction("InternalServerError", "Error"); }
        }

        public ActionResult ListeAchteur(int id)
        {
            return Json(db.TR_LIST_ACHTEURS_PAR_CTR(id), JsonRequestBehavior.AllowGet);
        }
        public JsonResult RecherchCtrAch2(int id)
        {
           var ListNomInd = db.Recherche_CTR_ADH_ACH_Par_CTR2019(id);
            return Json(ListNomInd, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FactureParRefCtrAch2(int id)
        {

            ViewBag.listctr = db.FactureParRefCtrAch(id).ToList();
            return PartialView();
        }

        public ActionResult FactureParRefCtrAch(int id)
        {

            ViewBag.listctr = db.FactureParRefCtr(id).ToList();
            return PartialView();
        }
        public JsonResult RecherchLesCtrAch(int ref_ind)
        {
            var ListNomInd = (from q in db.T_INDIVIDU
                              join q2 in db.TJ_CIR
                              on q.REF_IND equals q2.REF_IND_CIR
                              where (q2.ID_ROLE_CIR == "ACH" && q.REF_IND == ref_ind)
                              select new { q.PRE_IND, q2.REF_CTR_CIR, q.REF_IND, q2.ID_ROLE_CIR }).OrderBy(p => p.REF_CTR_CIR);

            return Json(ListNomInd, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Recouvrement()
        {
            if (Session["UserLogin"] != null)
            {
                //var ListNomInd = (from q in db.T_INDIVIDU
                //                  join q2 in db.TJ_CIR
                //                  on q.REF_IND equals q2.REF_IND_CIR
                //                  where (q2.ID_ROLE_CIR == "ADH")
                //                  select new { q.PRE_IND, q.REF_IND,q2.REF_CTR_CIR });
                //// ViewBag.ADH = new SelectList(ListNomInd, "REF_CTR_CIR", "PRE_IND");
                //List<NewDataCollection> op1 = new List<NewDataCollection>();
                //foreach (var data in ListNomInd)
                //{
                //    op1.Add(new NewDataCollection(data.REF_IND, data.PRE_IND, data.REF_CTR_CIR));
                //}
                //ViewBag.ADH = op1;
                //var ListNomIndAch = (from q in db.T_INDIVIDU
                //                     join q2 in db.TJ_CIR
                //                     on q.REF_IND equals q2.REF_IND_CIR
                //                     where (q2.ID_ROLE_CIR == "ACH")
                //                     select new { q.PRE_IND, q2.REF_IND_CIR }).Distinct();
                //List<NewDataCollection> op = new List<NewDataCollection>();
                //foreach (var data in ListNomIndAch)
                //{
                //    op.Add(new NewDataCollection(data.REF_IND_CIR, data.PRE_IND));
                //}

                //ViewBag.ACH = op;
                try
                {
                    ViewBag.ListeFacture = db.Recouvrement_All_Factures().ToList();
                    ViewBag.ListeFactureEch = db.FacturesEchu().ToList();
                    ViewBag.listefactureNonEchu = db.FacturesNonEchu().ToList();
                    ViewBag.Listval = new SelectList(db.TR_LIST_VAL.Where(p => p.TYP_LIST_VAL == "COMM_RECOUV"), "LIB_LIST_VAL", "LIB_LIST_VAL");

                   
                    TempData["Recouvrement"] = "active";
                }
                catch (Exception) { return RedirectToAction("InternalServerError", "Error"); }

                return View();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }


        public ActionResult UpdateComm(string comm_rec,DateTime date_rec,string com,int REF_CTR_DET_BORD_FACTURE,string ID_DET_BORD)
        {
            try
            {
                //string commantaire = com;
                T_DET_BORD bord = db.T_DET_BORD.Where(p => p.ID_DET_BORD == ID_DET_BORD && p.REF_CTR_DET_BORD == REF_CTR_DET_BORD_FACTURE).FirstOrDefault();
                //bord.COMM_DET_BORD = commantaire;
                //db.Entry(bord).State = EntityState.Modified;
                //db.SaveChanges();
                string query = "update t_det_bord set COMM_DET_BORD={0} where ID_DET_BORD={1} and REF_CTR_DET_BORD = {2}";
                db.Database.ExecuteSqlCommand(query, comm_rec+"("+com+")", ID_DET_BORD, REF_CTR_DET_BORD_FACTURE);
                TempData["notice"] = "sauvegarde enregistrée avec succès Ref°Facture : "+db.TJ_DOCUMENT_DET_BORD.Where(p=>p.ID_DET_BORD==bord.ID_DET_BORD).Select(p=>p.REF_DOCUMENT_DET_BORD).SingleOrDefault()+" N°Contrat : "+bord.REF_CTR_DET_BORD;
            }
            catch (Exception) { TempData["error"] = "il y a une problème de sauvegarde merci de vérifier votre commentaire si non contacter administrateur"; }
                return RedirectToAction("Recouvrement");
            

        }

        public ActionResult AjoutTiers()
        {
            var ListNomInd = (from q in db.T_INDIVIDU
                              join q2 in db.TJ_CIR
                              on q.REF_IND equals q2.REF_IND_CIR
                              where (q2.ID_ROLE_CIR == "ADH" && q2.REF_CTR_CIR != 28 && q2.REF_CTR_CIR != 32 && q2.REF_CTR_CIR != 38 && q2.REF_CTR_CIR != 54 && q2.REF_CTR_CIR != 56 && q2.REF_CTR_CIR != 58 && q2.REF_CTR_CIR != 65 && q2.REF_CTR_CIR != 69 && q2.REF_CTR_CIR != 72 && q2.REF_CTR_CIR != 74 && q2.REF_CTR_CIR != 80 && q2.REF_CTR_CIR != 82 && q2.REF_CTR_CIR != 93 && q2.REF_CTR_CIR != 96)
                              select new { q.PRE_IND, q2.REF_CTR_CIR });
            List<NewDataCollection> op = new List<NewDataCollection>();

            foreach (var data in ListNomInd)
            {
                op.Add(new NewDataCollection(data.REF_CTR_CIR, data.PRE_IND+"|"+ data.REF_CTR_CIR));
            }

            ViewBag.ADH = op;
            ViewBag.ListeInd = db.T_INDIVIDU.Select(p => new SelectListItem { Text = p.NOM_IND + " | " + p.PRE_IND, Value = p.REF_IND.ToString() });
            TempData["AjoutTiers"] = "active";
            return View();
        }

        [HttpPost]
        public ActionResult AjoutTiers(TJ_CIR cir)
        {
            cir.ID_ROLE_CIR = "TR";
            cir.ID_CIR = db.TJ_CIR.Max(p => p.ID_CIR) + 1;
            db.TJ_CIR.Add(cir);
            db.SaveChanges();
            TempData["notice"] = "sauvegarde enregistrée avec succès";
            return RedirectToAction("AjoutTiers");
        }

        public ActionResult AjoutFRS()
        {
            var ListNomInd = (from q in db.T_INDIVIDU
                              join q2 in db.TJ_CIR
                              on q.REF_IND equals q2.REF_IND_CIR
                              join q3 in db.T_CONTRAT
                              on q2.REF_CTR_CIR equals q3.REF_CTR
                              where (q2.ID_ROLE_CIR == "ADH" && q3.TYP_CTR.Trim() == "4")
                              select new { q.PRE_IND, q2.REF_CTR_CIR });
            List<NewDataCollection> op = new List<NewDataCollection>();

            foreach (var data in ListNomInd)
            {
                op.Add(new NewDataCollection(data.REF_CTR_CIR, data.PRE_IND + "|" + data.REF_CTR_CIR));
            }

            ViewBag.ADH = op;
            ViewBag.ListeInd = db.T_INDIVIDU.Select(p => new SelectListItem { Text = p.NOM_IND + " | " + p.PRE_IND, Value = p.REF_IND.ToString() });
            TempData["AjoutTiers"] = "active";
            return View();
        }
        [HttpPost]
        public ActionResult AjoutFRS(TJ_CIR cir)
        {
            cir.ID_ROLE_CIR = "FRS";
            cir.ID_CIR = db.TJ_CIR.Max(p => p.ID_CIR) + 1;
            db.TJ_CIR.Add(cir);
            db.SaveChanges();
            TempData["notice"] = "sauvegarde enregistrée avec succès";
            return RedirectToAction("AjoutFRS");
        }
        
    }

}

//    if (db.T_INDIVIDU.Where(p => p.REF_IND == indiv).Select(p => p.GENRE_IND).Single().ToString() == "PP")
//    {
//        T_DOC_GED ged = new T_DOC_GED();
//        ged.ID_IND_GED = indiv;
//        ged.ID_CTR_GED= db.T_CONTRAT.Select(t => t.REF_CTR).Max() + 1;


//    }

//else

//        if (db.T_INDIVIDU.Where(p => p.REF_IND == indiv).Select(p => p.GENRE_IND).Single().ToString() == "PM")
//{





//}
