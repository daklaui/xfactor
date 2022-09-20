using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using xfactor.Models;

namespace xfactor.Controllers
{
    public class AchteurController : Controller
    {
        private XFactor_PRODEntities1 db = new XFactor_PRODEntities1();
        // GET: Achteur
        public ActionResult SaisiLimite()
        {
            if (Session["UserLogin"] != null)
            {
                TempData["Assurence"] = true;
                //var ListAdh = (from q in db.T_INDIVIDU
                //               join q2 in db.TJ_CIR
                //               on q.REF_IND equals q2.REF_IND_CIR
                //               where (q2.ID_ROLE_CIR == "ADH" && q2.REF_CTR_CIR != 9 && q2.REF_CTR_CIR != 10 && q2.REF_CTR_CIR != 11 && q2.REF_CTR_CIR != 28 && q2.REF_CTR_CIR != 28 && q2.REF_CTR_CIR != 32 && q2.REF_CTR_CIR != 38 && q2.REF_CTR_CIR != 54 && q2.REF_CTR_CIR != 56 && q2.REF_CTR_CIR != 58 && q2.REF_CTR_CIR != 65 && q2.REF_CTR_CIR != 69 && q2.REF_CTR_CIR != 72 && q2.REF_CTR_CIR != 74 && q2.REF_CTR_CIR != 80 && q2.REF_CTR_CIR != 82 && q2.REF_CTR_CIR != 93 && q2.REF_CTR_CIR != 96)
                //               select new { q.PRE_IND, q2.REF_CTR_CIR });

                //List<SelectListItem> Adherents = new List<SelectListItem>{
                //    new SelectListItem {Text="***choisir un adhérent***",Value="",Selected=true,Disabled=true }
                //};
                //foreach (var item in ListAdh)
                //{
                //    Adherents.Add(new SelectListItem { Text = item.PRE_IND + "|" + item.REF_CTR_CIR.ToString(), Value = item.REF_CTR_CIR.ToString() });
                //}



                ViewBag.ADH =db.Recherche_CTR_ADH().ToList() ;
                TempData["Achteur"] = "active";
                TempData["SaisiLimite"] = "active";
                return View();
            }
            else
            {
                return RedirectToAction("login", "Login");
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
        [HttpPost]
        public ActionResult SaisiLimite(int Nom_Adh,int Nom_Ach,DateTime date_demande,string Limite_Credit,string Limite_fin,string Devis,string MNT_Ach_Adh,DateTime Date_limite,string Mode_Paye,int Delai)
        {
            TR_TVA tva;
            try
            {
                tva = db.TR_TVA.OrderByDescending(p => p.ID_TVA).SingleOrDefault();  
            }
            catch
            {

                tva = null;
            }
            try
            {
                T_DEM_LIMITE demlimit = new T_DEM_LIMITE();
                demlimit.REF_DEM_LIM = db.T_DEM_LIMITE.Select(p => p.REF_DEM_LIM).Max() + 1;
                demlimit.REF_CTR_DEM_LIM = Nom_Adh;
                demlimit.TYP_DEM_LIM = "FIN";
                demlimit.DAT_DEM_LIM = date_demande;
                demlimit.MONT_DEM_LIM = decimal.Parse(Limite_fin.Replace(" ",""));
                demlimit.DEVISE = Devis;
                demlimit.DELAIS_DEM_LIM = short.Parse(Delai.ToString());
                demlimit.MODE_PAY_DEM_LIM = Mode_Paye;
                demlimit.ACTIF_DEM_LIMI = false;
                demlimit.REF_ACH_LIM = Nom_Ach;
                db.T_DEM_LIMITE.Add(demlimit);
                TempData["notice"] = "valider";
                db.SaveChanges();


                try { T_HISTORIQUE histo = new T_HISTORIQUE();
                    histo.ABREV_ROLE_HIST = "Saisi limite " + demlimit.REF_CTR_DEM_LIM;
                    histo.ACTION = "Saisi limite";
                    histo.ID_ENREGISTREMENT = demlimit.REF_DEM_LIM.ToString();
                    histo.T_TABLE = "T_DEM_LIMITE";
                    histo.REF_CTR_HIST = demlimit.REF_CTR_DEM_LIM.ToString();
                    histo.REF_IND_HIST = demlimit.REF_ACH_LIM.ToString();
                    histo.LOGIN_USER = Session["UserLogin"].ToString();
                    histo.IP_PC = HttpContext.Request.UserHostAddress;
                    histo.NOM_PC = HttpContext.Request.UserHostName;
                    histo.DATE_ACTION = DateTime.Today;
                    db.T_HISTORIQUE.Add(histo);
                    db.SaveChanges();
                } catch (Exception) { }
                //suite mail de chedli 04052020
                try {

                    decimal diver;


                        try
                    {
                        diver = (decimal)db.T_FRAIS_DIVERS.Where(p => p.REF_CTR_FRAIS_DIVERS == Nom_Adh && p.TYP_FRAIS_DIVERS.Contains("ROP")).Select(p => p.MONT_PAR_UNIT_FRAIS_DIVERS).SingleOrDefault();
                      
                    } catch (Exception) { diver = 0; }
                    if (diver != 0)
                    {
                        T_MVT_DEBIT deb = new T_MVT_DEBIT();
                        deb.REF_CTR_DEBIT = Nom_Adh;
                        deb.ABEV_DEBIT = "RENOUVELLEMENT LIMITE";
                        deb.MONT_DEBIT = diver*tva.VAL_TVA;
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
                    }

                } catch (Exception ) { }





                return RedirectToAction("SaisiLimite");
            }
            catch (Exception) { return RedirectToAction("InternalServerError", "Error"); }
        }
        public ActionResult ValideLimite()
        {
            if (Session["UserLogin"] != null)
            {
                //var ListAdh = (from q in db.T_INDIVIDU
                //               join q2 in db.TJ_CIR
                //               on q.REF_IND equals q2.REF_IND_CIR
                //               where (q2.ID_ROLE_CIR == "ADH")
                //               select new { q.PRE_IND, q2.REF_CTR_CIR });
                //ViewBag.ADH = new SelectList(ListAdh, "REF_CTR_CIR", "PRE_IND");
                ViewBag.ADH = db.Recherche_CTR_ADH().ToList();
                TempData["Achteur"] = "active";
                TempData["ValideLimite"] = "active";
                return View();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
        [HttpPost]
        public ActionResult ValideLimite(string montant_acc,string refdemlimite,string ref_ctr_dem_limite,int delai,string mode_paye,string ref_ach_dem_limite)
        {
            if (montant_acc != "" || delai.ToString() != "")
            {
                try
                {
                    montant_acc = montant_acc.Replace(" ", "");
                    montant_acc = montant_acc.Replace(".", ",");
                    decimal montant_limite = decimal.Parse(montant_acc);
                    ref_ctr_dem_limite = ref_ctr_dem_limite.Trim();
                    int ref_ctr_dem_limitee = int.Parse(ref_ctr_dem_limite);
                    /*********************************************************/
                    ref_ach_dem_limite = ref_ach_dem_limite.Trim();
                    int ref_ach_dem_limitee = int.Parse(ref_ach_dem_limite);
                    /*********************************************************/
                    refdemlimite = refdemlimite.Trim();
                    int refdemlimitee = int.Parse(refdemlimite);
                    //T_DEM_LIMITE dem = db.T_DEM_LIMITE.Where(p => p.REF_DEM_LIM == ref_ctr_dem_limitee && p.REF_ACH_LIM == ref_ach_dem_limitee).SingleOrDefault();
                    //dem.ACTIF_DEM_LIMI = false;
                    //db.Entry(dem).State = EntityState.Modified;
                    //db.SaveChanges();
                    db.UpDdeLimite_NonActif(ref_ctr_dem_limitee, ref_ach_dem_limitee);
                    db.UpdateLimite(montant_limite, (short)delai, mode_paye, refdemlimitee, ref_ach_dem_limitee);
                    TempData["notice"] = "save";


                    try
                    {
                        T_HISTORIQUE histo = new T_HISTORIQUE();
                        histo.ABREV_ROLE_HIST = "Valide Limite " + ref_ctr_dem_limite;
                        histo.ACTION = "Saisi limite";
                        histo.ID_ENREGISTREMENT = refdemlimitee.ToString();
                        histo.T_TABLE = "T_DEM_LIMITE";
                        histo.REF_CTR_HIST = ref_ctr_dem_limite;
                        histo.REF_IND_HIST = ref_ach_dem_limitee.ToString();
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
                        if (db.All_Ecran_Financements(int.Parse(ref_ctr_dem_limite)).First().Depass_Limite > 0)
                        {
                            T_EXTRAIT extrait = new T_EXTRAIT();
                            extrait.REF_CTR_EXTRAIT = int.Parse(ref_ctr_dem_limite);
                            extrait.LIB_EXTRAIT = "Depassement ";
                            extrait.DEBIT_EXTRAIT = 0;
                            extrait.CREDIT_EXTRAIT = 0;
                            extrait.AUTRE_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).First().Depass_Limite;
                            extrait.ENCOURFACT_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Encours_Facture).FirstOrDefault();
                            extrait.DISPONIBLE_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Disponible).FirstOrDefault();
                            extrait.DAT_OP_EXTRAIT = DateTime.Now;
                            extrait.DAT_VAL_EXTRAIT = DateTime.Now;
                            extrait.TOTAL_FIN_EXTRAIT = db.All_Ecran_Financements(extrait.REF_CTR_EXTRAIT).Select(p => p.Total_Financement).FirstOrDefault();
                            db.T_EXTRAIT.Add(extrait);
                            db.SaveChanges();

                        }

                    }
                    catch (Exception e) { }



                    return RedirectToAction("ValideLimite");
                }
                catch (Exception) { return RedirectToAction("InternalServerError", "Error"); }
            }
            else
            {

                return RedirectToAction("InternalServerError", "Error");
            }
        }
        // public JsonResult ValideLimiteJson()

        [HttpGet]
        public JsonResult ListOfLimiteParCtr(int ref_ctr)
        {
            var tab = from dem in db.T_DEM_LIMITE
                      join iach in db.T_INDIVIDU on dem.REF_ACH_LIM equals iach.REF_IND
                      join cach in db.TJ_CIR on dem.REF_CTR_DEM_LIM equals cach.REF_CTR_CIR
                      where (cach.ID_ROLE_CIR == "ACH" && cach.REF_IND_CIR == dem.REF_ACH_LIM
                                                    && dem.ACTIF_DEM_LIMI == false
                                                    && dem.MONT_ACC == null
                                                    && dem.REF_CTR_DEM_LIM == ref_ctr)
                      select new
                      {
                          dem.REF_CTR_DEM_LIM,
                          dem.REF_ACH_LIM,
                          dem.ACTIF_DEM_LIMI,
                          dem.DAT_DEM_LIM,
                          dem.REF_DEM_LIM,
                          dem.DELAIS_DEM_LIM,
                          dem.DEVISE,
                          dem.MODE_PAY_DEM_LIM,
                          dem.MONT_DEM_LIM,
                          dem.SORT_DEM_LIM,
                          dem.TYP_DEM_LIM,
                          dem.DELAIS_ACC,
                          dem.MONT_ACC,
                          dem.MODE_PAY_ACC,
                          iach.NOM_IND
                      };


            //  var test = db.SelectQuery1(ref_ctr).ToList();
            return Json(tab, JsonRequestBehavior.AllowGet);
        }
    }
}