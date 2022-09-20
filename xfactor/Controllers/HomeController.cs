using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using xfactor.Models;
namespace xfactor.Controllers
{
    public class HomeController : Controller
    {
        XFactor_PRODEntities1 db = new XFactor_PRODEntities1();
        public ActionResult Index()
        {
            if (Session["UserLogin"] != null)
            {

                TempData["LesNotification"] = db.T_BORDEREAU.Where(p => p.DAT_SAISIE_BORD == DateTime.Today).Count();
                string sql = "select SUm(MONT_ENC) from t_encaissement where ID_ENC not in (select ID_ENC_LET from TJ_LETTRAGE) and year(dat_val_enc) > 2019 and VALIDE_ENC = 1 and TYP_ENC <> 'A'";
             
                TempData["EncNonLet"] = db.Database.SqlQuery<decimal>(sql).FirstOrDefault();
                TempData["Nb_Bord_Mois"] = db.T_BORDEREAU.Where(p => p.DAT_BORD.Value.Year == DateTime.Now.Year && p.DAT_BORD.Value.Month == DateTime.Now.Month && p.VALIDE_BORD==true).Sum(p=>p.MONT_TOT_BORD);
                TempData["FinMonth"] = db.T_FINANCEMENT.Where(p => p.DAT_FIN.Value.Month== DateTime.Now.Month && p.DAT_FIN.Value.Year == DateTime.Now.Year).Sum(p=>p.MONT_FIN);
                TempData["Impaye"] = db.T_IMPAYE.Where(p => p.DATE_IMP.Value.Year== DateTime.Now.Year).Sum(p=>p.MONT_IMP);
                TempData["ChequesTraites"] = db.GETENC02122019().Count();

                TempData["Fdg"] = db.T_DET_BORD.Where(p=>p.VALIDE_DET_BORD==true).Sum(p=>p.MONT_FDG_DET_BORD);


                return View();
            }
            else
            {
                return RedirectToAction("/Login/login");
            }
        }


        public ActionResult GetLesAchats()
        {

            string sql = " select  month(DAT_FIN)  as mois ,sum(T_FINANCEMENT.MONT_FIN) as montant_financement , " +
        "(select sum(MONT_TOT_BORD)  from T_BORDEREAU where ANNEE_BORD = year(SYSDATETIME()) and VALIDE_BORD = 1 and month(DAT_BORD) = month(DAT_FIN)  group by month(DAT_BORD) ) as montant_achat" +
 " from T_FINANCEMENT "+
 "where year(DAT_FIN) = year(SYSDATETIME()) "+
 "group by month(DAT_FIN)";


            List<AchatFinancementChart> listeachatsfina = db.Database.SqlQuery<AchatFinancementChart>(sql).ToList();



            return Json(listeachatsfina, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetImpayeCourbe()
        {
            string sql = "select MONTH(DATE_IMP) as mois, sum(MONT_IMP) as montant_impaye from T_IMPAYE where year(DATE_IMP) = year(SYSDATETIME()) and(IS_RESOLU = 0 or IS_RESOLU is null) group by MONTH(DATE_IMP)";


            List<ImpayeCourbe> listeimpaye = db.Database.SqlQuery<ImpayeCourbe>(sql).ToList();



            return Json(listeimpaye, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AnotherLink()
        {
            return View("Index");
        }
        public ActionResult allevents()
        {
            //List<T_RELANCE> rel = 
            return Json(db.T_RELANCE.AsEnumerable().Where(e=>e.DATE_RELANCE<=DateTime.Today).Select(
                e => new
                {
                    id = e.ID_RELANCE,
                    title = e.REF_DOC_RELANCE + " | " + e.LIBELLE_RELANCE,
                    start = e.DATE_RELANCE.Value.ToString("MM/dd/yyyy"),
                    ref_det_bord=e.REF_DOC_RELANCE,
                    allDay=true

                }).ToList(), JsonRequestBehavior.AllowGet);
  
        }


        public ActionResult DetailleEvents(string id)
        {
            //List<T_RELANCE> rel = 
           var query = " select t_det_bord.ID_DET_BORD,t_det_bord.NUM_BORD,tj_document_det_bord.REF_DOCUMENT_DET_BORD as REF_DET_BORD,TYP_DET_BORD,DAT_DET_BORD,MONT_TTC_DET_BORD,ANNEE_BORD,t_det_bord.REF_CTR_DET_BORD,ECH_DET_BORD,MODE_REG_DET_BORD ,(select NOM_IND from T_INDIVIDU where REF_IND = REF_IND_DET_BORD) as nomind from t_det_bord , tj_document_det_bord  where t_det_bord.ID_DET_BORD = tj_document_det_bord.ID_DET_BORD and tj_document_det_bord.REF_DOCUMENT_DET_BORD='" + id +"'";
            ViewBag.listDetBord = db.Database.SqlQuery<DetailleBord>(query).ToList();

            return PartialView();

        }


        public ActionResult ListeDesContartAChercher(string id,string id2,string id3,string id4)
        {

            string sql = "select T_CONTRAT.*,T_INDIVIDU.* " +
                        "from T_CONTRAT, T_INDIVIDU, tj_cir " +
                         "where " +
                           "REF_CTR = REF_CTR_CIR and REF_IND = REF_IND_CIR and ID_ROLE_CIR = 'ADH' and " +
"(REF_CTR like '" + id + "' or REF_CTR_PAPIER_CTR like '" + id2 + "' or NOM_IND like '" + id3 + "' or PRE_IND like '" + id3 + "' or REF_ADH_IND like '" + id4 + "')" +
"and REF_CTR_PAPIER_CTR<> '' and REF_CTR_PAPIER_CTR is not null";
            List<ListeDesContrat> x = db.Database.SqlQuery<ListeDesContrat>(sql).ToList();
            List<Affiche> Final=new List<Affiche>();
            

            foreach (var item in x)
            {
              //  List<Recherche_CTR_ADH_ACH_Par_CTR2019_Result> l = ;

                Affiche ob = new Affiche(item, db.Recherche_CTR_ADH_ACH_Par_CTR2019(item.REF_CTR).ToList());
                Final.Add(ob);
            }

            ViewBag.Liste_Des_Ctr= Final;

            return PartialView();
        }



        public ActionResult DetaileCtr(int id)
        {
            decimal a, b, c;
            TJ_CIR cir = db.TJ_CIR.Where(p => p.REF_CTR_CIR == id && p.ID_ROLE_CIR == "ADH").First();
            T_INDIVIDU ind = db.T_INDIVIDU.Find(cir.REF_IND_CIR);
            T_CONTRAT contrat = db.T_CONTRAT.Where(p => p.REF_CTR == id).FirstOrDefault();
            try { TempData["FormJuridique"] = db.TR_LIST_VAL.Where(p => p.TYP_LIST_VAL == "Forme juridique " && p.ABR_LIST_VAL == ind.FROM_JUR_IND).Select(p => p.LIB_LIST_VAL).First(); } catch (Exception) { TempData["FormJuridique"] = ""; }
            try { TempData["StatuContrat"] = db.TR_LIST_VAL.Where(p => p.TYP_LIST_VAL == "Statut_Contrat" && p.ABR_LIST_VAL == contrat.STATUT_CTR).Select(p => p.LIB_LIST_VAL).First(); } catch (Exception) { TempData["StatuContrat"] = ""; }
            try { TempData["Type_Ctr"] = db.TR_LIST_VAL.Where(p => p.TYP_LIST_VAL == "Type_contrat" && p.ABR_LIST_VAL == contrat.TYP_CTR).Select(p => p.LIB_LIST_VAL).First(); } catch (Exception) { TempData["Type_Ctr"] = ""; }
            try { TempData["Litige"] = db.SumMntLitige(id).First(); } catch (Exception) { TempData["Litige"] = ""; }

            // calcule de disponible 2 
            try { a = (decimal)db.SumMntImpaye(id).First(); } catch (Exception) { a = 0; }
            try { b = (decimal)db.SumMntLitige(id).First(); } catch (Exception) { b = 0; }
            try { c = (decimal)db.SumMntFactDepAlgo(id).First(); } catch (Exception) { c = 0; }
            //
            TempData["Dispo2"] = db.All_Ecran_Financements(id).FirstOrDefault().Disponible - a - b - c;
            DetaileDunContrat ctr = new DetaileDunContrat(ind, db.All_Ecran_Financements(id).FirstOrDefault(), contrat);
            ViewBag.ListeDesDetailes = ctr;
            return PartialView();
        }






       


    }
}
