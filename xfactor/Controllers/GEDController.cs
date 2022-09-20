using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;

using System.Web;
using System.Web.Mvc;
using xfactor.Models;
using xfactor.ServiceReference1;
namespace xfactor.Controllers
{
    public class GEDController : Controller
    {
        private XFactor_PRODEntities1 db = new XFactor_PRODEntities1();
        // GET: GED
        public ActionResult Numérisation_des_documents()
        {
            if (Session["UserLogin"] != null)
            {
                ViewBag.scan = db.T_DOC_GED_SCAN();
                TempData["GED"] = "active";
                TempData["Numérisation_des_documents"] = "active";
                return View();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }

        [HttpPost]
        public ActionResult Numérisation_des_documents(T_DOC_GED ged,HttpPostedFileBase file)
        {

            try
            {
               
                T_DOC_GED gd = db.T_DOC_GED.Find(ged.id);
                gd.ID_CTR_GED = ged.ID_CTR_GED;
                gd.LIBELLE2_GED = ged.LIBELLE2_GED;
                gd.salle_GED = ged.salle_GED;
                gd.Armoire_GED = ged.Armoire_GED;
                gd.Etgage_GED = ged.Etgage_GED;
                gd.Archive_GED = ged.Archive_GED;



                string filename = Path.GetFileNameWithoutExtension(file.FileName);
                string extention = Path.GetExtension(file.FileName);
                string filePath = "/Content/img/" + filename + extention;
                // filePath += @"\FILES";


                //  String CheminFichierSourceUd = (CheminFichierSource + "\\" + ged.LIBELLE2_GED+ extention);

                String NomFichier = ged.LIBELLE2_GED + extention;
                var path = Path.Combine(Server.MapPath("/Content/img/"), NomFichier);
                file.SaveAs(path);
                // Copier le fichier source selon le nom de la tache 



                // utliser pour lire image
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                // creation tableau binar pour remplir par file stream 
                byte[] imgByteArr = new byte[fs.Length];
                // lire les donnees dans le file stream et mettre dans le tableau 
                fs.Read(imgByteArr, 0, Convert.ToInt32(fs.Length));
                fs.Close();

                gd.DATE_SCAN_GED = DateTime.Now;
                gd.Data_GED = imgByteArr;
                gd.Name_GED = NomFichier;
                gd.Etape_ged = "En Attente de Validation";
                gd.ADRESS_DOC_GED = "";
                gd.Etat_GED = false;
                
                db.Entry(gd).State = EntityState.Modified;
                db.SaveChanges();
                TempData["notice"] = "sauvegarde enregistrée avec succès";
                try
                {
                    T_HISTORIQUE histo = new T_HISTORIQUE();
                    histo.ABREV_ROLE_HIST = "Numérisation_des_documents" + gd.LIBELLE2_GED;
                    histo.ACTION = "Numérisation_des_documents";
                    histo.ID_ENREGISTREMENT = gd.id.ToString();
                    histo.T_TABLE = "T_DOC_GED";
                    histo.REF_CTR_HIST = gd.ID_CTR_GED.ToString();
                    histo.REF_IND_HIST = gd.ID_IND_GED.ToString();
                    histo.LOGIN_USER = Session["UserLogin"].ToString();
                    histo.IP_PC = HttpContext.Request.UserHostAddress;
                    histo.NOM_PC = HttpContext.Request.UserHostName;
                    histo.DATE_ACTION = DateTime.Now;
                    db.T_HISTORIQUE.Add(histo);
                    db.SaveChanges();
                }
                catch (Exception) { }

            }
            catch (Exception e) { TempData["error"] = e.Message; }


         

            return RedirectToAction("Numérisation_des_documents");
        }

        public ActionResult Validation_Des_Docuemnts()
        {
            if (Session["UserLogin"] != null)
            {
                ViewBag.valid = db.T_DOC_GED_VALID().ToList().OrderBy(p=>p.DATE_TACHE_GED);
                TempData["GED"] = "active";
                TempData["Validation_Des_Docuemnts"] = "active";
                return View();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }

        public ActionResult Validation(int id)
        {
            try
            {
                T_DOC_GED ged = db.T_DOC_GED.Find(id);
                string type = ged.LIBELLE_GED;
                string adresse = "";
                if ((type == "Contrat") || (type == "Conditions Particulières") || (type == "Autre Doc.%"))
                {
                    string nom = (from q in db.T_INDIVIDU
                                  join q2 in db.TJ_CIR
                                  on q.REF_IND equals q2.REF_IND_CIR
                                  join q3 in db.T_CONTRAT
                                  on q2.REF_CTR_CIR equals q3.REF_CTR
                                  where q3.REF_CTR == ged.ID_CTR_GED && q2.ID_ROLE_CIR == "ADH"
                                  select (q.NOM_IND)).Single();
                    adresse += "/" + nom.TrimEnd().Replace(" ", "_") + "/";
                }
                if ((type == "Registre De Commerce") || (type == "Carte d’identité nationale"))
                {
                    T_INDIVIDU ind = new T_INDIVIDU();
                    try { ind = db.T_INDIVIDU.Where(p => p.NUM_DOC_ID_IND == ged.LIBELLE2_GED).Single(); } catch (Exception) { ind = null; }
                    TJ_CIR cir = new TJ_CIR();
                    try { cir = db.TJ_CIR.Where(p => p.REF_CTR_CIR == ged.ID_CTR_GED && p.REF_IND_CIR == ind.REF_IND).Single(); } catch (Exception) { cir = null; }
                    string nom = (from q in db.T_INDIVIDU
                                  join q2 in db.TJ_CIR
                                  on q.REF_IND equals q2.REF_IND_CIR
                                  join q3 in db.T_CONTRAT
                                  on q2.REF_CTR_CIR equals q3.REF_CTR
                                  where q3.REF_CTR == ged.ID_CTR_GED && q2.ID_ROLE_CIR == "ADH"
                                  select (q.NOM_IND)).Single();
                    if (cir != null) { if (cir.ID_ROLE_CIR == "ACH") { adresse += "/" + nom.TrimEnd().Replace(" ", "_") + "/" + ind.NOM_IND + "/Fiche_ACH" + "/"; } else { adresse += "/" + nom.TrimEnd().Replace(" ", "_") + "/" + "Fiche_ADH" + "/"; } }
                }
                if ((type == "Matricle Fiscale"))
                {
                    T_INDIVIDU ind = new T_INDIVIDU();
                    try { ind = db.T_INDIVIDU.Where(p => p.MF_IND == ged.LIBELLE2_GED).Single(); } catch (Exception) { ind = null; }
                    TJ_CIR cir = new TJ_CIR();
                    try { cir = db.TJ_CIR.Where(p => p.REF_CTR_CIR == ged.ID_CTR_GED && p.REF_IND_CIR == ind.REF_IND).Single(); } catch (Exception) { cir = null; }
                    string nom = (from q in db.T_INDIVIDU
                                  join q2 in db.TJ_CIR
                                  on q.REF_IND equals q2.REF_IND_CIR
                                  join q3 in db.T_CONTRAT
                                  on q2.REF_CTR_CIR equals q3.REF_CTR
                                  where q3.REF_CTR == ged.ID_CTR_GED && q2.ID_ROLE_CIR == "ADH"
                                  select (q.NOM_IND)).Single();
                    if (cir != null) { if (cir.ID_ROLE_CIR == "ACH") { adresse += "/" + nom.TrimEnd().Replace(" ", "_") + "/" + ind.NOM_IND + "/Fiche_ACH" + "/"; } else { adresse += "/" + nom.TrimEnd().Replace(" ", "_") + "/" + "Fiche_ADH" + "/"; } }
                }
                //correct

                if ((type == "Encaissement Cheque") || (type == "Encaissement Chéque") || (type == "Encaissement Traite") || (type == "Encaissement Virement"))
                {

                    string nom = (from q in db.T_INDIVIDU
                                  join q2 in db.TJ_CIR
                                  on q.REF_IND equals q2.REF_IND_CIR
                                  join q3 in db.T_CONTRAT
                                  on q2.REF_CTR_CIR equals q3.REF_CTR
                                  where q3.REF_CTR == ged.ID_CTR_GED && q2.ID_ROLE_CIR == "ADH"
                                  select (q.NOM_IND)).Single();
                    T_ENCAISSEMENT enc = new T_ENCAISSEMENT();
                    try { enc = db.T_ENCAISSEMENT.Where(p => p.ID_ENC == ged.ID_ENC_GED).Single(); } catch (Exception) { enc = null; }

                    adresse += "/" + nom.TrimEnd().Replace(" ", "_") + "/" + db.T_INDIVIDU.Where(p => p.REF_IND == enc.REF_ACH_ENC).Select(p => p.NOM_IND).Single().Replace(" ", "_") + "/" + "Encaissements" + '/';

                }

                if ((type == "Avoir"))
                {
                    string nom = (from q in db.T_INDIVIDU
                                  join q2 in db.TJ_CIR
                                  on q.REF_IND equals q2.REF_IND_CIR
                                  join q3 in db.T_CONTRAT
                                  on q2.REF_CTR_CIR equals q3.REF_CTR
                                  where q3.REF_CTR == ged.ID_CTR_GED && q2.ID_ROLE_CIR == "ADH"
                                  select (q.NOM_IND)).Single();
                    T_ENCAISSEMENT enc = new T_ENCAISSEMENT();
                    try { enc = db.T_ENCAISSEMENT.Where(p => p.ID_ENC == ged.ID_ENC_GED).Single(); } catch (Exception) { enc = null; }

                    adresse += "/" + nom.TrimEnd().Replace(" ", "_") + "/" + db.T_INDIVIDU.Where(p => p.REF_IND == enc.REF_ACH_ENC).Select(p => p.NOM_IND).Single().Replace(" ", "_") + "/" + "Avoir" + '/';

                }

                if (type == "Bordereau")
                {
                    string nom = (from q in db.T_INDIVIDU
                                  join q2 in db.TJ_CIR
                                  on q.REF_IND equals q2.REF_IND_CIR
                                  join q3 in db.T_CONTRAT
                                  on q2.REF_CTR_CIR equals q3.REF_CTR
                                  where q3.REF_CTR == ged.ID_CTR_GED && q2.ID_ROLE_CIR == "ADH"
                                  select (q.NOM_IND)).Single();
                    T_BORDEREAU bord = new T_BORDEREAU();
                    // var bor =null;
                    string query = "select * from t_bordereau where convert(int,num_bord)=" + ged.ID_BOR_GED + "and REF_CTR_BORD=" + ged.ID_CTR_GED;
                    var bor = db.T_BORDEREAU.SqlQuery(query).Single();

                    // var bor = (from q in db.T_BORDEREAU where (Convert.ToInt16(q.NUM_BORD) == ged.ID_BOR_GED && q.REF_CTR_BORD == ged.ID_CTR_GED) select (q)).Single();
                    bord.ANNEE_BORD = bor.ANNEE_BORD;
                    bord.NUM_BORD = bor.NUM_BORD;
                    adresse += "/" + nom.TrimEnd().Replace(" ", "_") + "/" + "Bordereau" + '/' + bord.ANNEE_BORD + '/' + bord.NUM_BORD + '/';
                }
                if (type == "Financement")
                {
                    string nom = (from q in db.T_INDIVIDU
                                  join q2 in db.TJ_CIR
                                  on q.REF_IND equals q2.REF_IND_CIR
                                  join q3 in db.T_CONTRAT
                                  on q2.REF_CTR_CIR equals q3.REF_CTR
                                  where q3.REF_CTR == ged.ID_CTR_GED && q2.ID_ROLE_CIR == "ADH"
                                  select (q.NOM_IND)).Single();

                    adresse += "/" + nom.TrimEnd().Replace(" ", "_") + "/" + "Financement" + '/';
                }
                //if (type == "Detail Bordereau")
                //{
                //    string nom = (from q in db.T_INDIVIDU
                //                  join q2 in db.TJ_CIR
                //                  on q.REF_IND equals q2.REF_IND_CIR
                //                  join q3 in db.T_CONTRAT
                //                  on q2.REF_CTR_CIR equals q3.REF_CTR
                //                  where q3.REF_CTR == ged.ID_CTR_GED && q2.ID_ROLE_CIR == "ADH"
                //                  select (q.NOM_IND)).Single();
                //    T_DET_BORD bord = new T_DET_BORD();
                //    try { bord = db.T_DET_BORD.Where(p => p.NUM_BORD == ged.ID_BOR_GED.ToString() && p.REF_CTR_DET_BORD == ged.ID_CTR_GED).Single(); } catch (Exception) { bord = null; }
                //    adresse += nom + "/" + "Bordereau" + '/' + bord.ANNEE_BORD + '/' + bord.NUM_BORD + '/';

                //}

                // Web Service
                ServiceReference1.XPFactorModel ModelWebService = new ServiceReference1.XPFactorModel();
                ServiceReference1.XPFactorClient ClientRunWebService = new ServiceReference1.XPFactorClient();
                ModelWebService.ID_Contrat = ged.ID_CTR_GED.ToString();
                ModelWebService.ID_Piece = ged.LIBELLE2_GED;
                ModelWebService.Date_piece = DateTime.Today;
                ModelWebService.Type_Piece = Path.GetExtension(ged.Name_GED);
                ServiceReference1.ResClass ResultWebService = new ServiceReference1.ResClass();

                ResultWebService = ClientRunWebService.Upload(ModelWebService, ged.Data_GED, ged.Name_GED, adresse);
                var path = Path.Combine(Server.MapPath("/Content/img/"), ged.Name_GED);
                ged.Etape_ged = "Valider/Affecter";
                ged.Etat_GED = true;
                ged.Data_GED = null;
                ged.ADRESS_DOC_GED = "http://srvshpgedweb:2018/CustomApplication/Lists/XPFactor" + adresse + ged.LIBELLE2_GED + Path.GetExtension(ged.Name_GED);
               // ged.ADRESS_DOC_GED = path;
                db.Entry(ged).State = EntityState.Modified;
                db.SaveChanges();
                TempData["notice"] = "sauvegarde enregistrée avec succès / lien = " + ged.ADRESS_DOC_GED;

                try
                {
                    T_HISTORIQUE histo = new T_HISTORIQUE();
                    histo.ABREV_ROLE_HIST = "Validation_des_documents" + ged.LIBELLE2_GED;
                    histo.ACTION = "Validation_des_documents";
                    histo.ID_ENREGISTREMENT = ged.id.ToString();
                    histo.T_TABLE = "T_DOC_GED";
                    histo.REF_CTR_HIST = ged.ID_CTR_GED.ToString();
                    histo.REF_IND_HIST = ged.ID_IND_GED.ToString();
                    histo.LOGIN_USER = Session["UserLogin"].ToString();
                    histo.IP_PC = HttpContext.Request.UserHostAddress;
                    histo.NOM_PC = HttpContext.Request.UserHostName;
                    histo.DATE_ACTION = DateTime.Now;
                    db.T_HISTORIQUE.Add(histo);
                    db.SaveChanges();
                }
                catch (Exception) { }

            }
            catch (Exception e) { TempData["error"] = e.Message;  }
            // TempData["notice"] = adresse;
            return RedirectToAction("Validation_Des_Docuemnts");
        }

        public ActionResult Refuse(int id)
        {
            T_DOC_H_GED gedh = new T_DOC_H_GED();
      
            T_DOC_GED ged = db.T_DOC_GED.Find(id);
            gedh.LIBELLE_H_GED = ged.LIBELLE_GED;
            gedh.DATE_TACHE_H_GED = DateTime.Now;
            gedh.LIBELLE2_H_GED = ged.LIBELLE2_GED;
            gedh.DATE_ANN_H_GED = DateTime.Today;
            gedh.MOTIF_ANN_H_GED = "Demande de Révision";
            db.SaveChanges();
            ged.Etape_ged = "Document à réviser";
            ged.Etat_GED = false;
            ged.salle_GED = "";
            ged.Armoire_GED = "";
            ged.Etgage_GED = "";
            ged.Archive_GED = "";
            ged.DATE_TACHE_GED = DateTime.Today;
            ged.DATE_SCAN_GED = null;
            ged.ADRESS_DOC_GED = null;
            ged.Data_GED = null;
            ged.Name_GED = null;
            db.Entry(ged).State = EntityState.Modified;
            db.SaveChanges();
            TempData["notice"] = "sauvegarde enregistrée avec succès";


            try
            {
                T_HISTORIQUE histo = new T_HISTORIQUE();
                histo.ABREV_ROLE_HIST = "Refuse" + ged.LIBELLE2_GED;
                histo.ACTION = "Validation_des_documents";
                histo.ID_ENREGISTREMENT = ged.id.ToString();
                histo.T_TABLE = "T_DOC_H_GED";
                histo.REF_CTR_HIST = ged.ID_CTR_GED.ToString();
                histo.REF_IND_HIST = ged.ID_IND_GED.ToString();
                histo.LOGIN_USER = Session["UserLogin"].ToString();
                histo.IP_PC = HttpContext.Request.UserHostAddress;
                histo.NOM_PC = HttpContext.Request.UserHostName;
                histo.DATE_ACTION = DateTime.Now;
                db.T_HISTORIQUE.Add(histo);
                db.SaveChanges();
            }
            catch (Exception) { }


            return RedirectToAction("Validation_Des_Docuemnts");
        }


        public ActionResult visualiser(int id)
        {
            byte[] imgByteArr = db.T_DOC_GED.Where(p => p.id == id).Select(p => p.Data_GED).First();

            return File(imgByteArr,
              System.Net.Mime.MediaTypeNames.Application.Octet, db.T_DOC_GED.Where(p => p.id == id).Select(p => p.Name_GED).First());
          //  return Json(db.T_DOC_GED.Where(p => p.id == id).Select(p => p.ADRESS_DOC_GED).SingleOrDefault(), JsonRequestBehavior.AllowGet);
  

        }


        public ActionResult visualitationDesDocumetsGed()
        {

            ViewBag.ListNomInd = db.Recherche_CTR_ADH().ToList();
            TempData["GED"] = "active";
            TempData["visualitationDesDocumetsGed"] = "active";
            return View();
        }

        public ActionResult ListeDesDocGed(int id)
        {
            ViewBag.Liste = db.T_DOC_GED.Where(p => p.ID_CTR_GED == id && p.Etat_GED == true && !p.LIBELLE_GED.ToLower().Contains("cred") && !p.LIBELLE_GED.ToLower().Contains("deb")).Select(p => p.LIBELLE_GED).Distinct();
            ViewBag.Ref_ctr = id;
            return PartialView();
        }

        public ActionResult OpenDoc(string id)
        {

            return File(id, "application/pdf", Server.UrlEncode(id));

        }


        public ActionResult ListeOfLibs(int id,string id2)
        {
            if(id2.ToLower().Contains("encaissementcheque"))
            {
                ViewBag.Liste = db.T_DOC_GED.Where(p => p.ID_CTR_GED == id && p.LIBELLE_GED == "Encaissement Cheque").ToList();
            }
            if (id2.ToLower().Contains("encaissementchéque"))
            {
                ViewBag.Liste = db.T_DOC_GED.Where(p => p.ID_CTR_GED == id && p.LIBELLE_GED == "Encaissement Chéque").ToList();
            }
            if (id2.ToLower().Contains("encaissementtraite"))
            {
                ViewBag.Liste = db.T_DOC_GED.Where(p => p.ID_CTR_GED == id && p.LIBELLE_GED == "Encaissement Traite").ToList();
            }
            if (id2.ToLower().Contains("avoir"))
            {
                ViewBag.Liste = db.T_DOC_GED.Where(p => p.ID_CTR_GED == id && p.LIBELLE_GED == "Avoir").ToList();
            }
            if (id2.ToLower().Contains("encaissementvirement"))
            {
                ViewBag.Liste = db.T_DOC_GED.Where(p => p.ID_CTR_GED == id && p.LIBELLE_GED == "Encaissement Virement").ToList();
            }
            if (id2.ToLower().Contains("financement"))
            {
                ViewBag.Liste = db.T_DOC_GED.Where(p => p.ID_CTR_GED == id && p.LIBELLE_GED == "Financement").ToList();
            }
            if (id2.ToLower().Contains("encaissementespece"))
            {
                ViewBag.Liste = db.T_DOC_GED.Where(p => p.ID_CTR_GED == id && p.LIBELLE_GED == "Encaissement Espece").ToList();
            }
            if (id2.ToLower().Contains("bordereau"))
            {
                ViewBag.Liste = db.T_DOC_GED.Where(p => p.ID_CTR_GED == id && p.LIBELLE_GED == "Bordereau").ToList();
            }
            if (id2.ToLower().Contains("contrat"))
            {
                ViewBag.Liste = db.T_DOC_GED.Where(p => p.ID_CTR_GED == id && p.LIBELLE_GED == "Contrat").ToList();
            }
            if (id2.ToLower().Contains("avance"))
            {
                ViewBag.Liste = db.T_DOC_GED.Where(p => p.ID_CTR_GED == id && p.LIBELLE_GED == "Avance").ToList();
            }
            if (id2.ToLower().Contains("liberationfinancement"))
            {
                ViewBag.Liste = db.T_DOC_GED.Where(p => p.ID_CTR_GED == id && p.LIBELLE_GED == "Liberation Financement").ToList();
            }
            if (id2.ToLower().Contains("cartedidentiténationale"))
            {
                ViewBag.Liste = db.T_DOC_GED.Where(p => p.ID_CTR_GED == id && p.LIBELLE_GED == "Carte d’identité nationale").ToList();
            }
            if (id2.ToLower().Contains("registredecommerce"))
            {
                ViewBag.Liste = db.T_DOC_GED.Where(p => p.ID_CTR_GED == id && p.LIBELLE_GED == "Registre De Commerce").ToList();
            }
            if (id2.ToLower().Contains("conditionsparticulières"))
            {
                ViewBag.Liste = db.T_DOC_GED.Where(p => p.ID_CTR_GED == id && p.LIBELLE_GED == "Conditions Particulières").ToList();
            }
            if (id2.ToLower().Contains("matriclefiscale"))
            {
                ViewBag.Liste = db.T_DOC_GED.Where(p => p.ID_CTR_GED == id && p.LIBELLE_GED == "Matricle Fiscale").ToList();
            }
            return PartialView();
        }

        [HttpPost]
        public ActionResult ReportDate(DateTime nouveuadate,int id_doc_ged,string motif_par_date)
        {
            T_DOC_GED ged;
            try
            {
                ged = db.T_DOC_GED.Where(p => p.id == id_doc_ged).FirstOrDefault();
            }
            catch (Exception) { ged = null; }

            if (ged != null)
            {
                T_DOC_H_GED ged_histo = new T_DOC_H_GED();
                ged_histo.MOTIF_ANN_H_GED = motif_par_date;
                ged_histo.LIBELLE_H_GED = ged.LIBELLE_GED;
                ged_histo.LIBELLE2_H_GED = ged.LIBELLE2_GED;
                int id_g;
                try { id_g = int.Parse(Session["ID_USER"].ToString()); } catch (Exception) { id_g = 0; }
                ged_histo.ID_GESTIONNAIRE_H_GED=id_g  ;
                ged_histo.DATE_TACHE_H_GED = ged.DATE_TACHE_GED;
                ged_histo.DATE_ANN_H_GED = DateTime.Now;
                db.T_DOC_H_GED.Add(ged_histo);
                db.SaveChanges();
                ged.DATE_TACHE_GED = nouveuadate;
                db.Entry(ged).State = EntityState.Modified;
                db.SaveChanges();


                try
                {
                    T_HISTORIQUE histo = new T_HISTORIQUE();
                    histo.ABREV_ROLE_HIST = "Report Date" + ged.LIBELLE2_GED;
                    histo.ACTION = "ReportDate";
                    histo.ID_ENREGISTREMENT = ged.id.ToString();
                    histo.T_TABLE = "T_DOC_H_GED";
                    histo.REF_CTR_HIST = ged.ID_CTR_GED.ToString();
                    histo.REF_IND_HIST = ged.ID_IND_GED.ToString();
                    histo.LOGIN_USER = Session["UserLogin"].ToString();
                    histo.IP_PC = HttpContext.Request.UserHostAddress;
                    histo.NOM_PC = HttpContext.Request.UserHostName;
                    histo.DATE_ACTION = DateTime.Now;
                    db.T_HISTORIQUE.Add(histo);
                    db.SaveChanges();
                }
                catch (Exception) { }

            }
            TempData["notice"] = "sauvegarde enregistrée avec succès";
            return RedirectToAction("Numérisation_des_documents");
        }


        [HttpPost]
        public ActionResult AnuulerTache(int id_doc_ged, string motif_par_date)
        {
            T_DOC_GED ged;
            try
            {
                ged = db.T_DOC_GED.Where(p => p.id == id_doc_ged).FirstOrDefault();
            }
            catch (Exception) { ged = null; }

            if (ged != null)
            {
                T_DOC_H_GED ged_histo = new T_DOC_H_GED();
                ged_histo.MOTIF_ANN_H_GED = motif_par_date;
                ged_histo.LIBELLE_H_GED = ged.LIBELLE_GED;
                ged_histo.LIBELLE2_H_GED = ged.LIBELLE2_GED;
                int id_g;
                try { id_g = int.Parse(Session["ID_USER"].ToString()); } catch (Exception) { id_g = 0; }
                ged_histo.ID_GESTIONNAIRE_H_GED = id_g;
                ged_histo.DATE_TACHE_H_GED = ged.DATE_TACHE_GED;
                ged_histo.DATE_ANN_H_GED = DateTime.Now;
                db.T_DOC_H_GED.Add(ged_histo);
                db.SaveChanges();
              
                db.Entry(ged).State = EntityState.Deleted;
                db.SaveChanges();


                try
                {
                    T_HISTORIQUE histo = new T_HISTORIQUE();
                    histo.ABREV_ROLE_HIST = "AnuulerTache" + ged.LIBELLE2_GED;
                    histo.ACTION = "AnuulerTache";
                    histo.ID_ENREGISTREMENT = ged.id.ToString();
                    histo.T_TABLE = "T_DOC_H_GED";
                    histo.REF_CTR_HIST = ged.ID_CTR_GED.ToString();
                    histo.REF_IND_HIST = ged.ID_IND_GED.ToString();
                    histo.LOGIN_USER = Session["UserLogin"].ToString();
                    histo.IP_PC = HttpContext.Request.UserHostAddress;
                    histo.NOM_PC = HttpContext.Request.UserHostName;
                    histo.DATE_ACTION = DateTime.Now;
                    db.T_HISTORIQUE.Add(histo);
                    db.SaveChanges();
                }
                catch (Exception) { }

            }
            TempData["notice"] = "sauvegarde enregistrée avec succès";
            return RedirectToAction("Numérisation_des_documents");
        }

        public ActionResult Modal()
        {
            return PartialView();
        }
    }
}