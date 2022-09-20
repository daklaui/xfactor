using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using xfactor.Models;
using XfactorClientWeb.Models;

namespace xfactor.Controllers
{
    public class RapportingController : Controller
    {
        private XFactor_PRODEntities1 db = new XFactor_PRODEntities1();
        // GET: Rapporting

        // Repporting 


        public ActionResult LesRapports()
        {
var ListAdh = (from q in db.T_INDIVIDU 
                           join q2 in db.TJ_CIR
                           on q.REF_IND equals q2.REF_IND_CIR
                          

                           where (q2.ID_ROLE_CIR == "ADH")
                           select new { q.PRE_IND,q.NOM_IND, q2.REF_CTR_CIR, q2.REF_IND_CIR ,q2.T_CONTRAT.REF_CTR_PAPIER_CTR}).ToList();


        

            List<NewDataCollection> op = new List<NewDataCollection>();
            foreach (var item in ListAdh)
            {
                op.Add(new NewDataCollection(item.REF_CTR_CIR, item.PRE_IND+ " | " +item.NOM_IND, item.REF_IND_CIR,item.REF_CTR_PAPIER_CTR));
            }
            ViewBag.ADH = op;

            //var c = db.usp_SituationAdhAch(id,id2).ToList();
            return View();
        }

        //*********
        public ActionResult SituationAdhérentAcheteur()
        {

            var ListAdh = (from q in db.T_INDIVIDU
                           join q2 in db.TJ_CIR
                           on q.REF_IND equals q2.REF_IND_CIR
                           where (q2.ID_ROLE_CIR == "ADH")
                           select new { q.PRE_IND, q2.REF_CTR_CIR, q2.REF_IND_CIR });
            List<NewDataCollection> op = new List<NewDataCollection>();
            foreach (var item in ListAdh)
            {
                op.Add(new NewDataCollection(item.REF_CTR_CIR, item.PRE_IND, item.REF_IND_CIR));
            }
            ViewBag.ADH = op;
            TempData["Rapporting"] = "active";
            TempData["SituationAdhérentAcheteur"] = "active";
            //var c = db.usp_SituationAdhAch(id,id2).ToList();
            return View();
        }

        public JsonResult RecherchCtrAch22(int id)
        {
            var ListNomInd = db.Recherche_CTR_ADH_ACH_Par_CTR2019(id);
            return Json(ListNomInd, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SituationAdhérentAcheteurRapport(int id)
        {
            //  ViewBag.c = db.usp_SituationAdhAch2(id, id2).ToList();
            ViewBag.c = db.usp_SituationAdh_ACH_ALL_Code_MFG(id).ToList();
            ViewBag.refadh = id;
          //  ViewBag.refach = id2;
            return PartialView();
        }
        public ActionResult Financmement_F()
        {
            string id = "PDF";
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



        public ActionResult Financmement_ParID()
        {
            string id = "PDF";
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

        public ActionResult Financmement_LIBFDG()
        {
            string id = "PDF";
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "Ordre de paiement FDG.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }


            var cm = db.usp_Etat_Financements().ToList();

            ReportDataSource rd = new ReportDataSource("DataSet1", cm);
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


        public ActionResult Report(string id,int id2,int id3)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "Situation Adhérent Acheteur.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("SituationAdhérentAcheteur");
            }
               var  cm = db.usp_SituationAdhAch2(id2,id3).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", cm);
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
        public ActionResult SituationAdhérent()
        {

            var ListAdh = (from q in db.T_INDIVIDU
                           join q2 in db.TJ_CIR
                           on q.REF_IND equals q2.REF_IND_CIR
                           where (q2.ID_ROLE_CIR == "ADH")
                           select new { q.PRE_IND, q2.REF_CTR_CIR, q2.REF_IND_CIR });
            List<NewDataCollection> op = new List<NewDataCollection>();
            foreach (var item in ListAdh)
            {
                op.Add(new NewDataCollection(item.REF_CTR_CIR, item.PRE_IND, item.REF_IND_CIR));
            }
            ViewBag.ADH = op;
            TempData["Rapporting"] = "active";
            TempData["SituationAdhérent"] = "active";
            //var c = db.usp_SituationAdhAch(id,id2).ToList();
            return View();
        }
        public ActionResult SituationAdhérentRapport(int id)
        {
            ViewBag.c = db.SituationAdh(id).ToList();
            ViewBag.c1 = db.SituationAdh14112019(id).ToList();
            ViewBag.refadh = id;
            return PartialView();
        }
        public ActionResult ReportSituationAdh(string id, int id2)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "Situation Adhérent2.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("SituationAdhérentAcheteur");
            }
            
            var cm = db.SituationAdh(id2).ToList();

            ReportDataSource rd = new ReportDataSource("situationAdh", cm);
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
        /**********************************************************************************/
        public ActionResult SituationAchteur()
        {

            var ListNomIndAch = (from q in db.T_INDIVIDU
                                 join q2 in db.TJ_CIR
                                 on q.REF_IND equals q2.REF_IND_CIR
                                 where (q2.ID_ROLE_CIR == "ACH")
                                 select new { q.PRE_IND, q2.REF_IND_CIR }).Distinct();
            List<NewDataCollection> op = new List<NewDataCollection>();
            foreach (var data in ListNomIndAch)
            {
                op.Add(new NewDataCollection(data.REF_IND_CIR, data.PRE_IND));
            }

            ViewBag.ACH = op;
            TempData["Rapporting"] = "active";
            TempData["SituationAchteur"] = "active";
            //var c = db.usp_SituationAdhAch(id,id2).ToList();
            return View();
        }

        public ActionResult SituationAchteurRapport(int id)
        {
            ViewBag.c = db.SituationAch(id).ToList();
            ViewBag.d = db.SituationAch14112019(id).ToList();
            ViewBag.refadh = id;
            return PartialView();
        }

        public ActionResult ReportSituationAch(string id, int id2)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "Situation Achteur.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("SituationAdhérentAcheteur");
            }

            var cm = db.SituationAch(id2).ToList();

            ReportDataSource rd = new ReportDataSource("DataSet1", cm);
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

        public ActionResult DepassementLimiteAll()
        {

 
            ViewBag.dep = db.usp_Etat_Depass_Lim_ALL().ToList();
            TempData["Rapporting"] = "active";
            TempData["DepassementLimiteAll"] = "active";
            //var c = db.usp_SituationAdhAch(id,id2).ToList();
            return View();
        }

        public ActionResult ReportDepLimiteh(string id)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "Report_depassement_Limite.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("SituationAdhérentAcheteur");
            }

            var cm = db.usp_Etat_Depass_Lim_ALL().ToList();

            ReportDataSource rd = new ReportDataSource("DataSet1", cm);
            lr.DataSources.Add(rd);
            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;



            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>9.5in</PageWidth>" +
            "  <PageHeight>8in</PageHeight>" +
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

        public ActionResult Commission_par_bord()
        {
            var ListAdh = (from q in db.T_INDIVIDU
                           join q2 in db.TJ_CIR
                           on q.REF_IND equals q2.REF_IND_CIR
                           where (q2.ID_ROLE_CIR == "ADH")
                           select new { q.PRE_IND, q2.REF_CTR_CIR, q2.REF_IND_CIR });
            List<NewDataCollection> op = new List<NewDataCollection>();
            foreach (var item in ListAdh)
            {
                op.Add(new NewDataCollection(item.REF_CTR_CIR, item.PRE_IND, item.REF_IND_CIR));
            }
            ViewBag.ADH = op;
            TempData["Rapporting"] = "active";
            TempData["Commission_par_bord"] = "active";
            //var c = db.usp_SituationAdhAch(id,id2).ToList();
            return View();


        }


        public ActionResult Commission_rap(int id)
        {

            try { ViewBag.commbord = db.CommparBord(id).ToList(); } catch (Exception) { ViewBag.commbord = 0; }
            try { ViewBag.sumMntTotalBord = db.CommparBord(id).Sum(p => p.MONT_TOT_BORD); } catch (Exception) { ViewBag.sumMntTotalBord = 0; }
            try { ViewBag.sumMntFdgInit = db.CommparBord(id).Sum(p => p.FDG_Initial); } catch (Exception) { ViewBag.sumMntTotalBord = 0; }
            try { ViewBag.sumMntFdgLibre = db.CommparBord(id).Sum(p => p.FDG_Libere); ; } catch (Exception) { ViewBag.sumMntTotalBord = 0; }
            try { ViewBag.sumFdg = db.CommparBord(id).Sum(p => p.FDG); } catch (Exception) { ViewBag.sumFdg = 0; }
            try { ViewBag.mntfin = db.CommparBord(id).Sum(p => p.Mont_fin); } catch (Exception) { ViewBag.mntfin = 0; }
            try { ViewBag.commht = db.CommparBord(id).Sum(p => p.COMM_HT); } catch (Exception) { ViewBag.commht = 0; }
            try { ViewBag.commttc = db.CommparBord(id).Sum(p => p.MONT_TTC); } catch (Exception) { ViewBag.commttc = 0; }
            ViewBag.refadh = id;
            return PartialView();
        }
        public ActionResult Commission_par_bord_Rapport(string id,int id2)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "Commission_par_bord.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("Commission_par_bord");
            }

            var cm = db.CommparBord(id2).ToList();

            ReportDataSource rd = new ReportDataSource("COMMPARBORD", cm);
            lr.DataSources.Add(rd);
            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;



            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>13in</PageWidth>" +
            "  <PageHeight>8in</PageHeight>" +
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



        public ActionResult Encaissements_Non_Echus()
        {

            var ListAdh = (from q in db.T_INDIVIDU
                           join q2 in db.TJ_CIR
                           on q.REF_IND equals q2.REF_IND_CIR
                           where (q2.ID_ROLE_CIR == "ADH")
                           select new { q.PRE_IND, q2.REF_CTR_CIR, q2.REF_IND_CIR });
            List<NewDataCollection> op = new List<NewDataCollection>();
            foreach (var item in ListAdh)
            {
                op.Add(new NewDataCollection(item.REF_CTR_CIR, item.PRE_IND, item.REF_IND_CIR));
            }
            ViewBag.ADH = op;
            TempData["Rapporting"] = "active";
            TempData["Encaissements_Non_Echus"] = "active";
            //var c = db.usp_SituationAdhAch(id,id2).ToList();
            return View();
        }
        public ActionResult Encaissements_Non_Echus_Rapp(int id)
        {

            try { ViewBag.EncNonEchus = db.RapportEncaissementsNonEchus(id).ToList(); } catch (Exception) { }
            try { ViewBag.sumMntEnc = db.RapportEncaissementsNonEchus(id).Sum(p => p.MONT_ENC); } catch (Exception) { ViewBag.sumMntEnc = 0; }
            ViewBag.refadh = id;
            return PartialView();
        }


        public ActionResult Encaissements_Non_Echus_Rapport(string id, int id2)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "Encaissements non échus.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("Encaissements_Non_Echus");
            }

            var cm = db.RapportEncaissementsNonEchus(id2).ToList();

            ReportDataSource rd = new ReportDataSource("ENCNONECH", cm);
            lr.DataSources.Add(rd);
            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;



            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>9.5in</PageWidth>" +
            "  <PageHeight>8in</PageHeight>" +
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

        /****** etat des contrat   */
        public ActionResult Etatcontrat()
        {

            var ListAdh = (from q in db.T_INDIVIDU
                           join q2 in db.TJ_CIR
                           on q.REF_IND equals q2.REF_IND_CIR
                           where (q2.ID_ROLE_CIR == "ADH")
                           select new { q.PRE_IND, q2.REF_CTR_CIR, q2.REF_IND_CIR });
            List<NewDataCollection> op = new List<NewDataCollection>();
            foreach (var item in ListAdh)
            {
                op.Add(new NewDataCollection(item.REF_CTR_CIR, item.PRE_IND, item.REF_IND_CIR));
            }
            ViewBag.ADH = op;
            TempData["Rapporting"] = "active";
            TempData["Etatcontrat"] = "active";
            //var c = db.usp_SituationAdhAch(id,id2).ToList();
            return View();
        }
        public ActionResult EtatcontratRapp(DateTime id)
        {

           try { ViewBag.EtatRap = db.Etat_Contrat_05082019(id).ToList(); } catch (Exception) { }
           // try { ViewBag.sumMntEnc = db.RapportEncaissementsNonEchus(id).Sum(p => p.MONT_ENC); } catch (Exception) { ViewBag.sumMntEnc = 0; }
            ViewBag.refadh = id;
            return PartialView();
        }
        public ActionResult Etatcontrat_Rapport(string id, DateTime id2)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "Etat_Contrat.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("Etatcontrat");
            }

            var cm = db.Etat_Contrat_05082019(id2).ToList();

            ReportDataSource rd = new ReportDataSource("Etat_Contrat", cm);
            lr.DataSources.Add(rd);
            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;



            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>14in</PageWidth>" +
            "  <PageHeight>8in</PageHeight>" +
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




        /****** etat factures */
        public ActionResult EtatDesFactures()
        {

            var ListAdh = (from q in db.T_INDIVIDU
                           join q2 in db.TJ_CIR
                           on q.REF_IND equals q2.REF_IND_CIR
                           where (q2.ID_ROLE_CIR == "ADH")
                           select new { q.PRE_IND, q2.REF_CTR_CIR, q2.REF_IND_CIR });
            List<NewDataCollection> op = new List<NewDataCollection>();
            foreach (var item in ListAdh)
            {
                op.Add(new NewDataCollection(item.REF_CTR_CIR, item.PRE_IND, item.REF_IND_CIR));
            }
            ViewBag.ADH = op;
            TempData["Rapporting"] = "active";
            TempData["EtatDesFactures"] = "active";
            //var c = db.usp_SituationAdhAch(id,id2).ToList();
            return View();
        }
        public ActionResult EtatDesFacturesRapp(int id)
        {

            try { ViewBag.EtatRap = db.ETATFACTURES(id).ToList(); } catch (Exception) { }
            // try { ViewBag.sumMntEnc = db.RapportEncaissementsNonEchus(id).Sum(p => p.MONT_ENC); } catch (Exception) { ViewBag.sumMntEnc = 0; }
            ViewBag.refadh = id;
            try { TempData["adh"] = db.ETATFACTURES(id).Select(p => p.Adhérent).SingleOrDefault() ; } catch (Exception) { }
           
            return PartialView();
        }
        public ActionResult EtatDesFactures_Rapport(string id, int id2)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "ETATDESFACTURES.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("EtatDesFactures");
            }

            var cm = db.ETATFACTURES(id2).ToList();

            ReportDataSource rd = new ReportDataSource("ETATFACTURES", cm);
            lr.DataSources.Add(rd);
            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;



            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>13in</PageWidth>" +
            "  <PageHeight>8in</PageHeight>" +
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


        public ActionResult EtatDesFacturesParAchRapp(int id,int id2)
        {

            try { ViewBag.EtatRap = db.ETATFACTURESALLACH(id,id2).ToList(); } catch (Exception) { }
            // try { ViewBag.sumMntEnc = db.RapportEncaissementsNonEchus(id).Sum(p => p.MONT_ENC); } catch (Exception) { ViewBag.sumMntEnc = 0; }
            ViewBag.refadh = id;
            ViewBag.refach = id2;
            try { TempData["adh"] = db.ETATFACTURES(id).Select(p => p.Adhérent).SingleOrDefault(); } catch (Exception) { }

            return PartialView();
        }
        public ActionResult EtatDesFacturesParAch_Rapport(string id, int id2,int id3)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "ETATDESFACTURESACH.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("EtatDesFactures");
            }

            var cm = db.ETATFACTURESALLACH(id2,id3).ToList();

            ReportDataSource rd = new ReportDataSource("ETATFACTURESACH", cm);
            lr.DataSources.Add(rd);
            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;



            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>13in</PageWidth>" +
            "  <PageHeight>8in</PageHeight>" +
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


        /****** etat des encaissements  */
        public ActionResult EtatDesEncaissements()
        {

            var ListAdh = (from q in db.T_INDIVIDU
                           join q2 in db.TJ_CIR
                           on q.REF_IND equals q2.REF_IND_CIR
                           where (q2.ID_ROLE_CIR == "ADH")
                           select new { q.PRE_IND, q2.REF_CTR_CIR, q2.REF_IND_CIR });
            List<NewDataCollection> op = new List<NewDataCollection>();
            foreach (var item in ListAdh)
            {
                op.Add(new NewDataCollection(item.REF_CTR_CIR, item.PRE_IND, item.REF_IND_CIR));
            }
            ViewBag.ADH = op;
            TempData["Rapporting"] = "active";
            TempData["EtatDesEncaissements"] = "active";
            //var c = db.usp_SituationAdhAch(id,id2).ToList();
            return View();
        }
        public ActionResult EtatDesEncaissementsRapp(int id)
        {

            try { ViewBag.EtatRap = db.Etatdesencaissements(id).ToList(); } catch (Exception) { }
            // try { ViewBag.sumMntEnc = db.RapportEncaissementsNonEchus(id).Sum(p => p.MONT_ENC); } catch (Exception) { ViewBag.sumMntEnc = 0; }
            ViewBag.refadh = id;
            return PartialView();
        }
        public ActionResult EtatDesEncaissements_Rapport(string id, int id2)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "Etatdes encaissements.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("EtatDesEncaissements");
            }

            var cm = db.Etatdesencaissements(id2).ToList();

            ReportDataSource rd = new ReportDataSource("ETATDESENCAISSEMENTS", cm);
            lr.DataSources.Add(rd);
            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;



            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>14in</PageWidth>" +
            "  <PageHeight>8in</PageHeight>" +
            "  <MarginTop>0.5in</MarginTop>" +
            "  <MarginLeft>0.5in</MarginLeft>" +
            "  <MarginRight>0.5in</MarginRight>" +
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


        public ActionResult EtatDesEncaissementsParAchRapp(int id, int id2)
        {

            try { ViewBag.EtatRap = db.EtatdesencaissementsAllACH(id, id2).ToList(); } catch (Exception) { }
            // try { ViewBag.sumMntEnc = db.RapportEncaissementsNonEchus(id).Sum(p => p.MONT_ENC); } catch (Exception) { ViewBag.sumMntEnc = 0; }
            ViewBag.refadh = id;
            ViewBag.refach = id2;
         
            return PartialView();
        }
        public ActionResult EtatDesEncaissementsParAch_Rapport(string id, int id2, int id3)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "Etatdes encaissementsAch.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("EtatDesEncaissements");
            }

            var cm = db.EtatdesencaissementsAllACH(id2, id3).ToList();

            ReportDataSource rd = new ReportDataSource("ETATDESENCAISSEMENTSACH", cm);
            lr.DataSources.Add(rd);
            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;



            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>13in</PageWidth>" +
            "  <PageHeight>8in</PageHeight>" +
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


        /****** Retenu a la source  */
        public ActionResult RetenuAlaSource()
        {

            var ListAdh = (from q in db.T_INDIVIDU
                           join q2 in db.TJ_CIR
                           on q.REF_IND equals q2.REF_IND_CIR
                           where (q2.ID_ROLE_CIR == "ADH")
                           select new { q.PRE_IND, q2.REF_CTR_CIR, q2.REF_IND_CIR });
            List<NewDataCollection> op = new List<NewDataCollection>();
            foreach (var item in ListAdh)
            {
                op.Add(new NewDataCollection(item.REF_CTR_CIR, item.PRE_IND, item.REF_IND_CIR));
            }
            ViewBag.ADH = op;
            TempData["Rapporting"] = "active";
            TempData["RetenuAlaSource"] = "active";
            //var c = db.usp_SituationAdhAch(id,id2).ToList();
            return View();
        }
        public ActionResult RetenuAlaSourceRapp(int id)
        {

            try { ViewBag.EtatRap = db.RETENUALASOURCE(id).ToList(); } catch (Exception) { }
            // try { ViewBag.sumMntEnc = db.RapportEncaissementsNonEchus(id).Sum(p => p.MONT_ENC); } catch (Exception) { ViewBag.sumMntEnc = 0; }
            ViewBag.refadh = id;
           // try { TempData["adh"] = db.ETATFACTURES(id).Select(p => p.Adhérent).SingleOrDefault(); } catch (Exception) { }

            return PartialView();
        }
        public ActionResult RetenuAlaSource_Rapport(string id, int id2)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "Retenu à la source.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("RetenuAlaSource");
            }

            var cm = db.RETENUALASOURCE(id2).ToList();

            ReportDataSource rd = new ReportDataSource("RETENU_A_LA_SOURCE", cm);
            lr.DataSources.Add(rd);
            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;



            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>13in</PageWidth>" +
            "  <PageHeight>8in</PageHeight>" +
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



        /******  Rapport Mensuel des Adhérents ***/
        public ActionResult Rapport_Mensuel_des_AdhérentsRapp()
        {

            try { ViewBag.EtatRap = db.Rapport_mensuel_tous_les_adhérents().ToList(); } catch (Exception) { }
            TempData["Rapporting"] = "active";
            TempData["Rapport_Mensuel_des_AdhérentsRapp"] = "active";
            return View();
        }
        public ActionResult Rapport_Mensuel_des_Adhérents_Rapport(string id, int id2)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "Retenu à la source.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("RetenuAlaSource");
            }
            var cm = db.RETENUALASOURCE(id2).ToList();
            ReportDataSource rd = new ReportDataSource("RETENU_A_LA_SOURCE", cm);
            lr.DataSources.Add(rd);
            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>13in</PageWidth>" +
            "  <PageHeight>8in</PageHeight>" +
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


        /****** Relevé mensuel  */
        public ActionResult ReleveMensuel()
        {

            var ListAdh = (from q in db.T_INDIVIDU
                           join q2 in db.TJ_CIR
                           on q.REF_IND equals q2.REF_IND_CIR
                           where (q2.ID_ROLE_CIR == "ADH")
                           select new { q.PRE_IND, q2.REF_CTR_CIR, q2.REF_IND_CIR });
            List<NewDataCollection> op = new List<NewDataCollection>();
            foreach (var item in ListAdh)
            {
                op.Add(new NewDataCollection(item.REF_CTR_CIR, item.PRE_IND, item.REF_IND_CIR));
            }
            ViewBag.ADH = op;
            TempData["Rapporting"] = "active";
            TempData["ReleveMensuel"] = "active";
            //var c = db.usp_SituationAdhAch(id,id2).ToList();
            return View();
        }
        public ActionResult ReleveMensuelRapp(int id)
        {

            try { ViewBag.EtatRap = db.Extrait__Compte_ADH(id).ToList(); } catch (Exception) { }
            try { TempData["contrat"] = db.INFO_ADH20082019(id).Select(p=>p.REF_CTR).FirstOrDefault(); } catch (Exception) { }
            try { TempData["nom"] = db.INFO_ADH20082019(id).Select(p => p.NOM_IND).FirstOrDefault(); } catch (Exception) { }
            ViewBag.refadh = id;
            return PartialView();
        }
        public ActionResult ReleveMensuel_Rapport(string id, int id2)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "Extrait_ADH.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("ReleveMensuel");
            }

            var cm = db.Extrait__Compte_ADH(id2).ToList();
            ReportDataSource rd = new ReportDataSource("Extrait__Compte_ADH", cm);
            var cm2= db.INFO_ADH20082019(id2).ToList();
            ReportDataSource rd2 = new ReportDataSource("Info_ADH", cm2);
            lr.DataSources.Add(rd);
            lr.DataSources.Add(rd2);
            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>13in</PageWidth>" +
            "  <PageHeight>8in</PageHeight>" +
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



        /****** Prorogation */
        public ActionResult Prorogation()
        {

            var ListAdh = (from q in db.T_INDIVIDU
                           join q2 in db.TJ_CIR
                           on q.REF_IND equals q2.REF_IND_CIR
                           where (q2.ID_ROLE_CIR == "ADH")
                           select new { q.PRE_IND, q2.REF_CTR_CIR, q2.REF_IND_CIR });
            List<NewDataCollection> op = new List<NewDataCollection>();
            foreach (var item in ListAdh)
            {
                op.Add(new NewDataCollection(item.REF_CTR_CIR, item.PRE_IND, item.REF_IND_CIR));
            }
            ViewBag.ADH = op;
            TempData["Rapporting"] = "active";
            TempData["Prorogation"] = "active";
            //var c = db.usp_SituationAdhAch(id,id2).ToList();
            return View();
        }
        public ActionResult ProrogationRapp(int id)
        {

            try { ViewBag.EtatRap = db.Detail_Prorogation20082019(id).ToList(); } catch (Exception) { }
            ViewBag.refadh = id;
            return PartialView();
        }
        public ActionResult Prorogation_Rapport(string id, int id2)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "E_Detail_Prorogation.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("ReleveMensuel");
            }

            var cm = db.Detail_Prorogation20082019(id2).ToList();
            ReportDataSource rd = new ReportDataSource("Detail_Prorogation", cm);
            lr.DataSources.Add(rd);
            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>13in</PageWidth>" +
            "  <PageHeight>8in</PageHeight>" +
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




        /****** Fiche Adhérent */
        public ActionResult FicheAdh()
        {

            var ListAdh = (from q in db.T_INDIVIDU
                           join q2 in db.TJ_CIR
                           on q.REF_IND equals q2.REF_IND_CIR
                           where (q2.ID_ROLE_CIR == "ADH")
                           select new { q.PRE_IND, q2.REF_CTR_CIR, q2.REF_IND_CIR });
            List<NewDataCollection> op = new List<NewDataCollection>();
            foreach (var item in ListAdh)
            {
                op.Add(new NewDataCollection(item.REF_CTR_CIR, item.PRE_IND, item.REF_IND_CIR));
            }
            ViewBag.ADH = op;
            TempData["Rapporting"] = "active";
            TempData["FicheAdh"] = "active";
            //var c = db.usp_SituationAdhAch(id,id2).ToList();
            return View();
        }
        public ActionResult FicheAdhRapp(int id)
        {
            var Info_Adh = from q in db.T_CONTRAT
                           join q2 in db.TJ_CIR on q.REF_CTR equals q2.REF_CTR_CIR
                           join q3 in db.T_INDIVIDU on q2.REF_IND_CIR equals q3.REF_IND
                           join q4 in db.TR_ACTPROF_BCT on q3.COD_SCLAS equals q4.Code_SousClasse
                           where (q2.ID_ROLE_CIR == "ADH" && q.REF_CTR == id)
                           select new {q.REF_CTR,q3.NOM_IND,q3.REF_IND,q3.RC_IND,q3.COD_TVA_IND,q.CA_CTR ,q3.GRP_IND,q4.Section};

            List<INFO_ADH20082019_Result> l = new List<INFO_ADH20082019_Result>();
            foreach (var item in Info_Adh)
            {
                INFO_ADH20082019_Result b = new INFO_ADH20082019_Result();
                b.NOM_IND = item.NOM_IND;
                b.GRP_IND = item.GRP_IND;
                b.COD_TVA_IND = item.COD_TVA_IND;
                b.CA_CTR = item.CA_CTR;
                b.RC_IND = item.RC_IND;
                b.REF_CTR = item.REF_CTR;
                b.REF_IND = item.REF_IND;
                b.Section = item.Section;
                l.Add(b);
            }
            try { ViewBag.AdhInfo2 = l; } catch (Exception) { }
            try { ViewBag.DetailAchAdh = db.Detail_ACH_Fiche_ADH06082019(id).ToList(); } catch (Exception) { }
            try { ViewBag.DetailCtr = db.Detail_ctr_06082019(id).ToList(); } catch (Exception) { }
            try { ViewBag.infoctr = db.Info_ctr_06082019(id).ToList(); } catch (Exception) { }
            try { ViewBag.Gerant = db.t_CONTACT2_06082019(id).ToList(); } catch (Exception) { }
            try { ViewBag.Responsable = db.t_CONTACT3_06082019(id).ToList(); } catch (Exception) { }
            try { ViewBag.ListeAch = db.Recherche_CTR_ADH_ACH_Par_CTR(id).ToList(); } catch (Exception) { }
            /***********************************************************************************************************/
            try { TempData["f"]=db.financement_N06082019(id).Sum(p=>p.financement_N); }catch (Exception) { TempData["f"] = 0; }
            try { TempData["f1"] = db.financement_N06082019(id).Sum(p => p.financement_N1); } catch (Exception) { TempData["f1"] = 0; }
            try { TempData["f2"] = db.financement_N06082019(id).Sum(p => p.financement_N2); } catch (Exception) { TempData["f2"] = 0; }
            /***************************************************************************************************************************/
            try { TempData["eng_total_fact"] = db.Engagement_N06082019(id).Sum(p => p.Total_Facture); } catch (Exception) { TempData["eng_total_fact"] = 0; }
            try { TempData["eng1_total_fact"] = db.Engagement_N1_06082019(id).Sum(p => p.Total_Facture); } catch (Exception) { TempData["eng1_total_fact"] = 0; }
            try { TempData["eng2_total_fact"] = db.Engagement_N3_06082019(id).Sum(p => p.Total_Facture); } catch (Exception) { TempData["eng2_total_fact"] = 0; }
            /*****************************************************************************************************************************/

            /***************************************************************************************************************************/
            try { TempData["eng_Total_Encours"] = db.Engagement_N06082019(id).Sum(p => p.Total_Encours); } catch (Exception) { TempData["eng_Total_Encours"] = 0; }
            try { TempData["eng1_Total_Encours"] = db.Engagement_N1_06082019(id).Sum(p => p.Total_Encours); } catch (Exception) { TempData["eng1_Total_Encours"] = 0; }
            try { TempData["eng2_Total_Encours"] = db.Engagement_N3_06082019(id).Sum(p => p.Total_Encours); } catch (Exception) { TempData["eng2_Total_Encours"] = 0; }
            /*****************************************************************************************************************************/



            /***************************************************************************************************************************/
            try { TempData["eng_Total_Interet"] = db.Engagement_N06082019(id).Sum(p => p.Total_Interet); } catch (Exception) { TempData["eng_Total_Interet"] = 0; }
            try { TempData["eng1_Total_Interet"] = db.Engagement_N1_06082019(id).Sum(p => p.Total_Interet); } catch (Exception) { TempData["eng1_Total_Interet"] = 0; }
            try { TempData["eng2_Total_Interet"] = db.Engagement_N3_06082019(id).Sum(p => p.Total_Interet); } catch (Exception) { TempData["eng2_Total_Interet"] = 0; }
            /*****************************************************************************************************************************/


            /***************************************************************************************************************************/
            try { TempData["eng_Total_Commission"] = db.Engagement_N06082019(id).Sum(p => p.Total_Commission); } catch (Exception) { TempData["eng_Total_Commission"] = 0; }
            try { TempData["eng1_Total_Commission"] = db.Engagement_N1_06082019(id).Sum(p => p.Total_Commission); } catch (Exception) { TempData["eng1_Total_Commission"] = 0; }
            try { TempData["eng2_Total_Commission"] = db.Engagement_N3_06082019(id).Sum(p => p.Total_Commission); } catch (Exception) { TempData["eng2_Total_Commission"] = 0; }
            /*****************************************************************************************************************************/

            /***************************************************************************************************************************/
            try { TempData["eng_Total_FDG"] = db.Engagement_N06082019(id).Sum(p => p.Toatl_FDG); } catch (Exception) { TempData["eng_Total_FDG"] = 0; }
           
            /*****************************************************************************************************************************/

            ViewBag.refadh = id;
            return PartialView();
        }
        public ActionResult FicheAdh_Rapport(string id, int id2)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "Fiche_ADH.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("FicheAdhRapp");
            }

            var Info_Adh = from q in db.T_CONTRAT
                           join q2 in db.TJ_CIR on q.REF_CTR equals q2.REF_CTR_CIR
                           join q3 in db.T_INDIVIDU on q2.REF_IND_CIR equals q3.REF_IND
                           join q4 in db.TR_ACTPROF_BCT on q3.COD_SCLAS equals q4.Code_SousClasse
                           where (q2.ID_ROLE_CIR == "ADH" && q.REF_CTR == id2)
                           select new { q.REF_CTR, q3.NOM_IND, q3.REF_IND, q3.RC_IND, q3.COD_TVA_IND, q.CA_CTR, q3.GRP_IND, q4.Section };

            List<INFO_ADH20082019_Result> l = new List<INFO_ADH20082019_Result>();
            foreach (var item in Info_Adh)
            {
                INFO_ADH20082019_Result b = new INFO_ADH20082019_Result();
                b.NOM_IND = item.NOM_IND;
                b.GRP_IND = item.GRP_IND;
                b.COD_TVA_IND = item.COD_TVA_IND;
                b.CA_CTR = item.CA_CTR;
                b.RC_IND = item.RC_IND;
                b.REF_CTR = item.REF_CTR;
                b.REF_IND = item.REF_IND;
                b.Section = item.Section;
                l.Add(b);
            }
            var cm = l;
            var cm1 = db.Detail_ACH_Fiche_ADH06082019(id2).ToList();
            var cm2 = db.Detail_ctr_06082019(id2).ToList();
            var cm3 = db.Info_ctr_06082019(id2).ToList();
            var cm4 = db.t_CONTACT2_06082019(id2).ToList();
            var cm5 = db.t_CONTACT3_06082019(id2).ToList();
            var cm6 = db.Recherche_CTR_ADH_ACH_Par_CTR(id2).ToList();
            var cm7 = db.Engagement_N06082019(id2).ToList();
            var cm8 = db.Engagement_N1_06082019(id2).ToList();
            var cm9 = db.Engagement_N3_06082019(id2).ToList();
            var cm10 = db.financement_N06082019(id2).ToList();
            ReportDataSource rd = new ReportDataSource("ADH_INFO", cm);
            ReportDataSource rd1 = new ReportDataSource("Detail_Ach_ADH", cm1);
            ReportDataSource rd2= new ReportDataSource("Contrat", cm2);
            ReportDataSource rd3 = new ReportDataSource("Info_CTR", cm3);
            ReportDataSource rd4 = new ReportDataSource("Gerant", cm4);
            ReportDataSource rd5 = new ReportDataSource("Responsable", cm5);
            ReportDataSource rd6 = new ReportDataSource("Liste_ACH", cm6);
            ReportDataSource rd7 = new ReportDataSource("Eng_N", cm7);
            ReportDataSource rd8 = new ReportDataSource("Eng_N1", cm8);
            ReportDataSource rd9 = new ReportDataSource("Eng_2", cm9);
            ReportDataSource rd10 = new ReportDataSource("financement", cm10);
            lr.DataSources.Add(rd);
            lr.DataSources.Add(rd1);
            lr.DataSources.Add(rd2);
            lr.DataSources.Add(rd3);
            lr.DataSources.Add(rd4);
            lr.DataSources.Add(rd5);
            lr.DataSources.Add(rd6);
            lr.DataSources.Add(rd7);
            lr.DataSources.Add(rd8);
            lr.DataSources.Add(rd9);
            lr.DataSources.Add(rd10);
            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>13in</PageWidth>" +
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



        /****** Facture en litige */
        public ActionResult FactEnLitige()
        {

            var ListAdh = (from q in db.T_INDIVIDU
                           join q2 in db.TJ_CIR
                           on q.REF_IND equals q2.REF_IND_CIR
                           where (q2.ID_ROLE_CIR == "ADH")
                           select new { q.PRE_IND, q2.REF_CTR_CIR, q2.REF_IND_CIR });
            List<NewDataCollection> op = new List<NewDataCollection>();
            foreach (var item in ListAdh)
            {
                op.Add(new NewDataCollection(item.REF_CTR_CIR, item.PRE_IND, item.REF_IND_CIR));
            }
            ViewBag.ADH = op;
            TempData["Rapporting"] = "active";
            TempData["FactEnLitige"] = "active";
            //var c = db.usp_SituationAdhAch(id,id2).ToList();
            return View();
        }
        public ActionResult FactEnLitigeRapp(int id)
        {

            try { ViewBag.EtatRap = db.usp_Liste_factures_en_litige(id).ToList(); } catch (Exception) { }
            ViewBag.refadh = id;
            return PartialView();
        }
        public ActionResult FactEnLitige_Rapport(string id, int id2)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "Report_factures_en_litige.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("FactEnLitige");
            }

            var cm = db.usp_Liste_factures_en_litige(id2).ToList();
            ReportDataSource rd = new ReportDataSource("Factures_en_litige", cm);
            lr.DataSources.Add(rd);
            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>13in</PageWidth>" +
            "  <PageHeight>8in</PageHeight>" +
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



        /****** DISPONIBLE CONTRAT */
        public ActionResult DisponibleContrats()
        {
            try { ViewBag.EtatRap = db.usp_dispo_ctr().ToList(); } catch (Exception) { }
            TempData["Rapporting"] = "active";
            TempData["DisponibleContrats"] = "active";
            //var c = db.usp_SituationAdhAch(id,id2).ToList();
            return View();
        }
       
        public ActionResult DisponibleContrats_Rapport(string id)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "R_Disponible.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("DisponibleContrats");
            }

            var cm = db.usp_dispo_ctr().ToList();
            ReportDataSource rd = new ReportDataSource("Disponible", cm);
            lr.DataSources.Add(rd);
            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>13in</PageWidth>" +
            "  <PageHeight>8in</PageHeight>" +
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



        /****** ImpayeContrat */
        public ActionResult ImpayeContrat()
        {
            var ListAdh = (from q in db.T_INDIVIDU
                           join q2 in db.TJ_CIR
                           on q.REF_IND equals q2.REF_IND_CIR
                           where (q2.ID_ROLE_CIR == "ADH")
                           select new { q.PRE_IND, q2.REF_CTR_CIR, q2.REF_IND_CIR });
            List<NewDataCollection> op = new List<NewDataCollection>();
            foreach (var item in ListAdh)
            {
                op.Add(new NewDataCollection(item.REF_CTR_CIR, item.PRE_IND, item.REF_IND_CIR));
            }
            ViewBag.ADH = op;
            TempData["Rapporting"] = "active";
            TempData["ImpayeContrat"] = "active";
            //var c = db.usp_SituationAdhAch(id,id2).ToList();
            return View();
        }
        public ActionResult ImpayeContratRapp(int id)
        {

            try { ViewBag.EtatRap = db.E_Detail_Impaye21082019(id).ToList(); } catch (Exception) { }
            ViewBag.refadh = id;
            return PartialView();
        }
        public ActionResult ImpayeContrat_Rapport(string id,int id2)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "Impayé contrat.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("ImpayeContrat");
            }
            var cm = db.E_Detail_Impaye21082019(id2).ToList();
            ReportDataSource rd = new ReportDataSource("Impaye_contrat", cm);
            lr.DataSources.Add(rd);
            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>13in</PageWidth>" +
            "  <PageHeight>8in</PageHeight>" +
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


        /****** Encours_Factures */
        public ActionResult Encours_Factures()
        {
            var ListAdh = (from q in db.T_INDIVIDU
                           join q2 in db.TJ_CIR
                           on q.REF_IND equals q2.REF_IND_CIR
                           where (q2.ID_ROLE_CIR == "ADH")
                           select new { q.PRE_IND, q2.REF_CTR_CIR, q2.REF_IND_CIR });
            List<NewDataCollection> op = new List<NewDataCollection>();
            foreach (var item in ListAdh)
            {
                op.Add(new NewDataCollection(item.REF_CTR_CIR, item.PRE_IND, item.REF_IND_CIR));
            }
            ViewBag.ADH = op;
            TempData["Rapporting"] = "active";
            TempData["Encours_Factures"] = "active";
            //var c = db.usp_SituationAdhAch(id,id2).ToList();
            return View();
        }
        public ActionResult Encours_FacturesRapp(int id)
        {

            try { ViewBag.EtatRap = db.Encours_Factures_Par_ADH(id).ToList(); } catch (Exception) { }
            ViewBag.refadh = id;
            return PartialView();
        }


        public ActionResult Encours_FacturesRappdate(int id,DateTime id2,DateTime id3)
        {

            try { ViewBag.EtatRap = db.Encours_Factures_Par_ADH(id).Where(p=>p.DAT_DET_BORD>=id2.Date && p.DAT_DET_BORD<=id3.Date).ToList(); } catch (Exception) { }
            ViewBag.refadh = id;
            TempData["debut"] = id2.Date;
            TempData["fin"] = id3.Date;
            return PartialView();
        }


        public ActionResult Encours_Factures_Rapport(string id, int id2)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "R_Encours_Factures_Adhérent.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("ImpayeContrat");
            }
            var cm = db.Encours_Factures_Par_ADH(id2).ToList();
            ReportDataSource rd = new ReportDataSource("Encours_Factures_Adherent", cm);
            lr.DataSources.Add(rd);
            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>13in</PageWidth>" +
            "  <PageHeight>8in</PageHeight>" +
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



        public ActionResult Encours_Factures_Rapportdate(string id, int id2,DateTime id3,DateTime id4)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "R_Encours_Factures_Adhérent.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("ImpayeContrat");
            }
            var cm = db.Encours_Factures_Par_ADH(id2).Where(p => p.DAT_DET_BORD >= id3.Date && p.DAT_DET_BORD <= id4.Date).ToList();
            ReportDataSource rd = new ReportDataSource("Encours_Factures_Adherent", cm);
            lr.DataSources.Add(rd);
            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>13in</PageWidth>" +
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


        /****** Liste_Financement */
        public ActionResult Liste_Financement()
        {
            var ListAdh = (from q in db.T_INDIVIDU
                           join q2 in db.TJ_CIR
                           on q.REF_IND equals q2.REF_IND_CIR
                           where (q2.ID_ROLE_CIR == "ADH")
                           select new { q.PRE_IND, q2.REF_CTR_CIR, q2.REF_IND_CIR });
            List<NewDataCollection> op = new List<NewDataCollection>();
            foreach (var item in ListAdh)
            {
                op.Add(new NewDataCollection(item.REF_CTR_CIR, item.PRE_IND, item.REF_IND_CIR));
            }
            ViewBag.ADH = op;
            TempData["Rapporting"] = "active";
            TempData["Liste_Financement"] = "active";
            //var c = db.usp_SituationAdhAch(id,id2).ToList();
            return View();
        }
        public ActionResult Liste_FinancementRapp(int id)
        {

            try { ViewBag.EtatRap = db.LISTEDESFINANCEMENTS(id).OrderByDescending(p=>p.DAT_FIN).ToList(); } catch (Exception) { }
            ViewBag.refadh = id;
            return PartialView();
        }
        public ActionResult Liste_Financement_Rapport(string id, int id2)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "Liste des financements.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("ImpayeContrat");
            }
            var cm = db.LISTEDESFINANCEMENTS(id2).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", cm);
            lr.DataSources.Add(rd);
            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>10in</PageWidth>" +
            "  <PageHeight>8in</PageHeight>" +
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




        /****** Extrait_Adhérent */
        public ActionResult Extrait_Adhérent()
        {
            var ListAdh = (from q in db.T_INDIVIDU
                           join q2 in db.TJ_CIR
                           on q.REF_IND equals q2.REF_IND_CIR
                           where (q2.ID_ROLE_CIR == "ADH")
                           select new { q.PRE_IND, q2.REF_CTR_CIR, q2.REF_IND_CIR });
            List<NewDataCollection> op = new List<NewDataCollection>();
            foreach (var item in ListAdh)
            {
                op.Add(new NewDataCollection(item.REF_CTR_CIR, item.PRE_IND, item.REF_IND_CIR));
            }
            ViewBag.ADH = op;
            TempData["Rapporting"] = "active";
            TempData["Extrait_Adhérent"] = "active";
            //var c = db.usp_SituationAdhAch(id,id2).ToList();
            return View();
        }
        public ActionResult Extrait_AdhérentRapp(int id)
        {

            try { ViewBag.EtatRap = db.Extrait_compte_Adhérent(id).ToList(); } catch (Exception) { }
            ViewBag.refadh = id;
            return PartialView();
        }
        public ActionResult Extrait_Adhérent_Rapport(string id, int id2)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "Extrait Adhérent.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("Extrait_Adhérent");
            }
            var cm = db.Extrait_compte_Adhérent(id2).ToList();
            ReportDataSource rd = new ReportDataSource("Extrait", cm);
            lr.DataSources.Add(rd);
            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>13in</PageWidth>" +
            "  <PageHeight>8in</PageHeight>" +
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


        public ActionResult Extrait_Compte(int id)
        {
            int o = db.TJ_CIR.Where(p => p.REF_CTR_CIR == id && p.ID_ROLE_CIR == "ADH").Select(p => p.REF_IND_CIR).FirstOrDefault();

            ViewData["nameadh"] = db.T_INDIVIDU.Find(o).NOM_IND;
            ViewBag.ListeExtrait = db.Extrait_Compte_New(id).OrderBy(p => p.Date_OP).ToList();
            ViewBag.ListeExtraitDetAchat = db.Extrait_Compte_Detaille_Achat(id).OrderBy(p => p.DAT_BORD).ToList();
            ViewBag.ListeExtraitFrais = db.Extrait_Compte_Frais(id).OrderBy(p => p.DATE_DEBIT).ToList();
            ViewBag.ListeExtraitCom = db.Extrait_Compte_COMMFACT(id).OrderBy(p => p.DAT_BORD).ToList();
            ViewBag.ListeExtraitFraisP = db.Extrait_Compte_FRAISP(id).OrderBy(p => p.dat_recep_enc).ToList();
            ViewData["AncienEnc"] = db.Database.SqlQuery<decimal>("  select Total_Encours_Facture_ETAT_DISPO from T_ETAT_DISPO where Date_ETAT_DISPO =(  select DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) - 1, 0)) and Ref_Ctr__ETAT_DISPO=" + id).FirstOrDefault();
            ViewData["AncienDispo"] = db.Database.SqlQuery<decimal>("  select Disponible_ETAT_DISPO from T_ETAT_DISPO where Date_ETAT_DISPO =(  select DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) - 1, 0)) and Ref_Ctr__ETAT_DISPO=" + id).FirstOrDefault();


            ViewData["NouveauEnc"] = db.Database.SqlQuery<decimal>("select Total_Encours_Facture_ETAT_DISPO from T_ETAT_DISPO where Date_ETAT_DISPO=DATEADD(DAY, -1, DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0)) and Ref_Ctr__ETAT_DISPO=" + id).FirstOrDefault();
            ViewData["NouveauDispo"] = db.Database.SqlQuery<decimal>("select Disponible_ETAT_DISPO from T_ETAT_DISPO where Date_ETAT_DISPO=DATEADD(DAY, -1, DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0)) and Ref_Ctr__ETAT_DISPO=" + id).FirstOrDefault();

            //List<CalculeFraisLimite> Lf = new List<CalculeFraisLimite>();

            //var list = db.T_DEM_LIMITE.Where(p => p.REF_CTR_DEM_LIM == id && p.ACTIF_DEM_LIMI == true && p.TYP_DEM_LIM == "FIN" && p.DATLIM_DEM_LIM.Value.Month == DateTime.Now.Month - 1 && p.DATLIM_DEM_LIM.Value.Year == DateTime.Now.Year).ToList();

            //foreach (var item in list)
            //{
            //    CalculeFraisLimite c = new CalculeFraisLimite();
            //    c.Date = (DateTime)item.DATLIM_DEM_LIM;

            //    if (db.T_DEM_LIMITE.Where(p => p.REF_CTR_DEM_LIM == id && p.ACTIF_DEM_LIMI == true && p.TYP_DEM_LIM == "FIN" && p.REF_ACH_LIM == item.REF_ACH_LIM).Count() > 1)
            //    {
            //        c.Description = "Renouvellement";
            //        c.frais=db.T_FRAIS_DIVERS.Where(p=>p.REF_CTR_FRAIS_DIVERS==id && p.TYP_FRAIS_DIVERS== "OP").Select(p=>p.MONT_PAR_UNIT_FRAIS_DIVERS)
            //    }
            //    else
            //    {
            //        c.Description = "Demande de limites";
            //    }



            //}

            DateTime now = DateTime.Now;
            int prevMonth = now.AddMonths(-1).Month;
            int year = now.AddMonths(-1).Year;
            int daysInPrevMonth = DateTime.DaysInMonth(year, prevMonth);

            List<CalculeFraisLimite> ob = new List<CalculeFraisLimite>();
            CalculeFraisLimite v = new CalculeFraisLimite();

            v.Date = new DateTime(year, prevMonth, daysInPrevMonth);
            v.Description = "Commission de timbre fiscal";
            v.frais = 0.6M;
            
            ob.Add(v);
            ViewBag.ListeExtraitFraisTimbre = ob.ToList();

            return PartialView();
        }


        public ActionResult Factures_En_Retard()
        {
            var ListAdh = (from q in db.T_INDIVIDU
                           join q2 in db.TJ_CIR
                           on q.REF_IND equals q2.REF_IND_CIR
                           where (q2.ID_ROLE_CIR == "ADH")
                           select new { q.PRE_IND, q2.REF_CTR_CIR, q2.REF_IND_CIR });
            List<NewDataCollection> op = new List<NewDataCollection>();
            foreach (var item in ListAdh)
            {
                op.Add(new NewDataCollection(item.REF_CTR_CIR, item.PRE_IND, item.REF_IND_CIR));
            }
            ViewBag.ADH = op;
            TempData["Rapporting"] = "active";
            TempData["Factures_en_retard"] = "active";
            //var c = db.usp_SituationAdhAch(id,id2).ToList();
            return View();
        }

        public ActionResult Factures_En_Retard_Rapp(int id)
        {

            try { ViewBag.Factures_retard = db.Les_Factures_En_retard_Par_Adh(id).ToList(); } catch (Exception) { }
            ViewBag.refadh = id;
            return PartialView();

            //var c = db.usp_SituationAdhAch(id,id2).ToList();
          //  return View();
        }

        public ActionResult Factures_En_Retard_Rapport(string id, int id2)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "Report_Factures_EN_Retard.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("Factures_En_Retard");
            }
            var cm = db.Les_Factures_En_retard_Par_Adh(id2).ToList();
            ReportDataSource rd = new ReportDataSource("R", cm);
            lr.DataSources.Add(rd);
            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>13in</PageWidth>" +
            "  <PageHeight>8in</PageHeight>" +
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


        public ActionResult Factures_En_Retard_Rapp_ach(int id,int id2)
        {

            try { ViewBag.Factures_retard = db.Les_Factures_En_retard(id,id2).ToList(); } catch (Exception) { }
            ViewBag.refadh = id;
            ViewBag.refach = id2;
            return PartialView();

            //var c = db.usp_SituationAdhAch(id,id2).ToList();
            //  return View();
        }

        public ActionResult Factures_En_Retard_Rapport_ach(string id, int id2,int id3)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "Report_Factures_EN_Retard.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("Factures_En_Retard");
            }
            var cm = db.Les_Factures_En_retard(id2, id3).ToList();
            ReportDataSource rd = new ReportDataSource("R", cm);
            lr.DataSources.Add(rd);
            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>13in</PageWidth>" +
            "  <PageHeight>8in</PageHeight>" +
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


        public ActionResult Encours_Factures_D()
        {
            var ListAdh = (from q in db.T_INDIVIDU
                           join q2 in db.TJ_CIR
                           on q.REF_IND equals q2.REF_IND_CIR
                           where (q2.ID_ROLE_CIR == "ADH")
                           select new { q.PRE_IND, q2.REF_CTR_CIR, q2.REF_IND_CIR });
            List<NewDataCollection> op = new List<NewDataCollection>();
            foreach (var item in ListAdh)
            {
                op.Add(new NewDataCollection(item.REF_CTR_CIR, item.PRE_IND, item.REF_IND_CIR));
            }
            ViewBag.ADH = op;
            TempData["Rapporting"] = "active";
            TempData["Encours_Factures_D"] = "active";
            //var c = db.usp_SituationAdhAch(id,id2).ToList();
            return View();
        }

        public ActionResult Rapport_Extrait_()
        {
            var ListAdh = (from q in db.T_INDIVIDU
                           join q2 in db.TJ_CIR
                           on q.REF_IND equals q2.REF_IND_CIR
                           where (q2.ID_ROLE_CIR == "ADH")
                           select new { q.PRE_IND, q2.REF_CTR_CIR, q2.REF_IND_CIR });
            List<NewDataCollection> op = new List<NewDataCollection>();
            foreach (var item in ListAdh)
            {
                op.Add(new NewDataCollection(item.REF_CTR_CIR, item.PRE_IND, item.REF_IND_CIR));
            }
            ViewBag.ADH = op;
            TempData["Rapporting"] = "active";
            TempData["Rapport_Extrait_"] = "active";
            //var c = db.usp_SituationAdhAch(id,id2).ToList();
            return View();
        }

        public ActionResult Rapport_Extrait_Rapp(int id,DateTime id2,DateTime id3)
        {
           
            ViewBag.ListeExtrait = db.Extrait_Compte_Periode2(id).Where(p => p.Date_OP >= id2.Date && p.Date_OP <= id3.Date).ToList();
            try {  } catch (Exception) { }
            return PartialView();
        }


        public ActionResult Encours_Factures_Detail(DateTime id)
        {

            try { ViewBag.Encours = db.Encours_Factures_Par_Date_Fin(id).ToList(); } catch (Exception) { }
            
            return PartialView();

            //var c = db.usp_SituationAdhAch(id,id2).ToList();
            //  return View();
        }

        public ActionResult Extrait_Par_Mois()
        {
            var ListAdh = (from q in db.T_INDIVIDU
                           join q2 in db.TJ_CIR
                           on q.REF_IND equals q2.REF_IND_CIR
                           where (q2.ID_ROLE_CIR == "ADH")
                           select new { q.PRE_IND, q2.REF_CTR_CIR, q2.REF_IND_CIR });
            List<NewDataCollection> op = new List<NewDataCollection>();
            foreach (var item in ListAdh)
            {
                op.Add(new NewDataCollection(item.REF_CTR_CIR, item.PRE_IND, item.REF_IND_CIR));
            }
            ViewBag.ADH = op;
            TempData["Rapporting"] = "active";
            TempData["Extrait_Par_Mois"] = "active";
           
            return View();
        }

        public ActionResult GetMonthYearFn(int id)
        {
           TempData["ref_ctr"] = id;
            string sql = "select MONTH(DAT_OP_EXTRAIT) as mois,YEAR(DAT_OP_EXTRAIT) as annee from T_EXTRAIT where MONTH(DAT_OP_EXTRAIT)<> MONTH(SYSDATETIME())  group by MONTH(DAT_OP_EXTRAIT),YEAR(DAT_OP_EXTRAIT) ";
            List<GetMonthYear> getMonthYear = db.Database.SqlQuery<GetMonthYear>(sql).OrderByDescending(p => p.mois).ToList();
            ViewBag.getMonthYear = getMonthYear;
            return View();
        }

        public ActionResult Extrait_Compte_par_mois(int id,int id1, int id2)
        {
           
            int o = db.TJ_CIR.Where(p => p.REF_CTR_CIR == id && p.ID_ROLE_CIR == "ADH").Select(p => p.REF_IND_CIR).FirstOrDefault();

            ViewData["nameadh"] = db.T_INDIVIDU.Find(o).NOM_IND;
            ViewBag.ListeExtrait = db.Extrait_Compte_New_Rapport(id).Where(p => p.Date_OP.Value.Month == id1 && p.Date_OP.Value.Year == id2).OrderBy(p => p.Date_OP).ToList();
            ViewBag.ListeExtraitDetAchat = db.Extrait_Compte_Detaille_Achat_Rapport(id).Where(p => p.DAT_BORD.Value.Month == id1 && p.DAT_BORD.Value.Year == id2).OrderBy(p => p.DAT_BORD).ToList();
            ViewBag.ListeExtraitFrais = db.Extrait_Compte_Frais_Rapport(id).Where(p => p.DATE_DEBIT.Value.Month == id1 && p.DATE_DEBIT.Value.Year == id2).OrderBy(p => p.DATE_DEBIT).ToList();
            ViewBag.ListeExtraitCom = db.Extrait_Compte_COMMFACT_Rapport(id).Where(p => p.DAT_BORD.Value.Month == id1 && p.DAT_BORD.Value.Year == id2).OrderBy(p => p.DAT_BORD).ToList();
            ViewBag.ListeExtraitFraisP = db.Extrait_Compte_FRAISP_Rapport(id).Where(p => p.dat_recep_enc.Value.Month == id1 && p.dat_recep_enc.Value.Year == id2).OrderBy(p => p.dat_recep_enc).ToList();
            int mon = DateTime.Today.Month - id1;
            ViewData["AncienEnc"] = db.Database.SqlQuery<decimal>("  select Total_Encours_Facture_ETAT_DISPO from T_ETAT_DISPO where Date_ETAT_DISPO =(  select DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) - " + mon + ", 0)) and Ref_Ctr__ETAT_DISPO=" + id).FirstOrDefault();
            ViewData["AncienDispo"] = db.Database.SqlQuery<decimal>("  select Disponible_ETAT_DISPO from T_ETAT_DISPO where Date_ETAT_DISPO =(  select DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) -  " + mon + ", 0)) and Ref_Ctr__ETAT_DISPO=" + id).FirstOrDefault();


            ViewData["NouveauEnc"] = db.Database.SqlQuery<decimal>("select Total_Encours_Facture_ETAT_DISPO from T_ETAT_DISPO where Date_ETAT_DISPO=DATEADD(DAY, -1, DATEADD(MONTH, DATEDIFF(MONTH, 0, DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) - " + (mon - 1) + ", 0)), 0)) and Ref_Ctr__ETAT_DISPO=" + id).FirstOrDefault();
            ViewData["NouveauDispo"] = db.Database.SqlQuery<decimal>("select Disponible_ETAT_DISPO from T_ETAT_DISPO where Date_ETAT_DISPO=DATEADD(DAY, -1, DATEADD(MONTH, DATEDIFF(MONTH, 0,DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) - " + (mon - 1) + ", 0)), 0)) and Ref_Ctr__ETAT_DISPO=" + id).FirstOrDefault();

            //List<CalculeFraisLimite> Lf = new List<CalculeFraisLimite>();

            //var list = db.T_DEM_LIMITE.Where(p => p.REF_CTR_DEM_LIM == id && p.ACTIF_DEM_LIMI == true && p.TYP_DEM_LIM == "FIN" && p.DATLIM_DEM_LIM.Value.Month == DateTime.Now.Month - 1 && p.DATLIM_DEM_LIM.Value.Year == DateTime.Now.Year).ToList();

            //foreach (var item in list)
            //{
            //    CalculeFraisLimite c = new CalculeFraisLimite();
            //    c.Date = (DateTime)item.DATLIM_DEM_LIM;

            //    if (db.T_DEM_LIMITE.Where(p => p.REF_CTR_DEM_LIM == id && p.ACTIF_DEM_LIMI == true && p.TYP_DEM_LIM == "FIN" && p.REF_ACH_LIM == item.REF_ACH_LIM).Count() > 1)
            //    {
            //        c.Description = "Renouvellement";
            //        c.frais=db.T_FRAIS_DIVERS.Where(p=>p.REF_CTR_FRAIS_DIVERS==id && p.TYP_FRAIS_DIVERS== "OP").Select(p=>p.MONT_PAR_UNIT_FRAIS_DIVERS)
            //    }
            //    else
            //    {
            //        c.Description = "Demande de limites";
            //    }



            //}

            /* DateTime now = DateTime.Now;
             int prevMonth = now.AddMonths(-1).Month;
             int year = now.AddMonths(-1).Year;
             int daysInPrevMonth = DateTime.DaysInMonth(year, prevMonth);

             List<CalculeFraisLimite> ob = new List<CalculeFraisLimite>();
             CalculeFraisLimite v = new CalculeFraisLimite();

             v.Date = new DateTime(year, prevMonth, daysInPrevMonth);
             v.Description = "Commission de timbre fiscal";
             v.frais = 0.6M;

             ob.Add(v);
             ViewBag.ListeExtraitFraisTimbre = ob.ToList();*/

            return PartialView();
        }


        public ActionResult Liste_Bordereau_Par_Jour()
        {
            ViewBag.liste_Bord = db.Entet_Bordereau_par_jour().ToList();
            return View();
        }

        public ActionResult Liste_Bordereau_Par_Jour_Det(string codeadh,string num_bord,int annee)
        {
            ViewBag.liste_Bord2 = db.Bordereau_par_jour().Where(p=>p.code_adherent.Equals(codeadh) && p.NUM_BORD.Equals(num_bord) && p.DAT_SAISIE_BORD.Value.Year==annee ).ToList();
            return View();
        }
        public ActionResult Liste_Bordereau_SendToMFG()
        {
            ViewBag.liste_Bord = db.Bordereau_SendToMFG().ToList();
            return View();
        }

        public ActionResult Liste_Réconsiliation()
        {
            if (Session["UserLogin"] != null)
            {
                var ListAdh = (from q in db.T_INDIVIDU
                               join q2 in db.TJ_CIR
                               on q.REF_IND equals q2.REF_IND_CIR
                               where (q2.ID_ROLE_CIR == "ADH")
                               select new { q.PRE_IND, q2.REF_CTR_CIR, q2.REF_IND_CIR });
                List<NewDataCollection> op = new List<NewDataCollection>();
                foreach (var item in ListAdh)
                {
                    op.Add(new NewDataCollection(item.REF_CTR_CIR, item.PRE_IND, item.REF_IND_CIR));
                }
                ViewBag.ADH = op;
                TempData["Parametrage"] = "active";
                TempData["Liste_Réconsiliation"] = "active";
                //  ViewBag.Liste_Réconsiliation = db.Annulation_Reconsiliation_Liste().ToList();
                return View();
            }
            else {
                return RedirectToAction("login", "Login");
            }
        }
        public ActionResult Liste_Réconsiliation_Detaile(int ref_ctr,string ref_enc)
        {
              ViewBag.Liste_Réconsiliation = db.Annulation_Reconsiliation_Liste(ref_ctr, ref_enc).ToList();
            return PartialView();
        }


        public ActionResult AnnulationRéconsiliationAction(int id_enc)
        {
            // ViewBag.Liste_Réconsiliation = db.Annulation_Reconsiliation_Liste(ref_ctr, ref_enc).ToList();
            try
            {
                List<TJ_LETTRAGE> Lettrage = db.TJ_LETTRAGE.Where(p => p.ID_ENC_LET == id_enc).ToList();

                foreach (TJ_LETTRAGE lettrage in Lettrage)
                {//"29606"

                    T_DET_BORD facture = db.T_DET_BORD.Where(p => p.ID_DET_BORD == lettrage.ID_DET_BORD_LET.ToString()).FirstOrDefault();

                    facture.MONT_OUV_DET_BORD = facture.MONT_OUV_DET_BORD + lettrage.MONT_TTC_LET;

                    facture.MONT_FDG_DET_BORD = facture.MONT_OUV_DET_BORD * (facture.TX_FDG_DET_BORD / 100);

                    facture.MONT_FDG_LIBERE_DET_BORD = (facture.MONT_TTC_DET_BORD * (facture.TX_FDG_DET_BORD / 100)) - facture.MONT_FDG_DET_BORD;

                    lettrage.VALIDE_RECONCIL = false;

                    db.SaveChanges();

                    try
                    {
                        T_HISTORIQUE histo = new T_HISTORIQUE();
                        histo.ABREV_ROLE_HIST = "Annulation Réconsiliation " + lettrage.ID_ENC_LET;
                        histo.ACTION = "Annulation Réconsiliation ";
                        histo.ID_ENREGISTREMENT = lettrage.ID_ENC_LET.ToString();
                        histo.T_TABLE = "TJ_LETTRAGE";
                        histo.REF_CTR_HIST = facture.REF_CTR_DET_BORD.ToString();
                        // int x = db.T_ENCAISSEMENT.Find(imp.ID_ENC_IMP).REF_CTR_ENC;
                        histo.REF_IND_HIST = facture.REF_IND_DET_BORD.ToString();
                        histo.LOGIN_USER = Session["UserLogin"].ToString();
                        histo.IP_PC = HttpContext.Request.UserHostAddress;
                        histo.NOM_PC = HttpContext.Request.UserHostName;
                        histo.DATE_ACTION = DateTime.Now;
                        db.T_HISTORIQUE.Add(histo);
                        db.SaveChanges();
                    }
                    catch (Exception) { }

                }

                return Json("True",JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json("False", JsonRequestBehavior.AllowGet);
            }
        }

        /*public ActionResult Financmement_F()
        {
            string id = "PDF";
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
        }*/

    }
}