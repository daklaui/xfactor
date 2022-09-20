using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Mvc;
using xfactor.Models;
using System.Web;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;

namespace xfactor.Controllers
{
    public class BCTController : Controller
    {
        // GET: BCT
        public ActionResult RNLPT130010()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreationRNLPT140(List<RNLPT40> ListeRNLPT, DateTime dt, string TypeFile)
        {

            string code = "";
            switch (TypeFile)
            {
                case "RNLPT130040": code = "133"; break;
                case "RNLPT130063": code = "138"; break;
                case "RNLPT130065": code = "140"; break;


            }

            string path = @"C:\\BCT\" + TypeFile + "_XML_" + DateTime.Now.Date.Year.ToString() + DateTime.Now.Date.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Second.ToString() + ".xml";
            //@"C:\\BCT\"
            StreamWriter sw = new StreamWriter(path);
            sw.WriteLine("<?xml version='1.0' encoding='UTF-8' standalone='no'?>");

            sw.WriteLine("<Document>");

            sw.WriteLine("<Entete>");

            sw.WriteLine("<CodeBanque>60</CodeBanque>");

            sw.WriteLine("<DateAnnexe>" + dt.ToString("yyyyMMdd") + "</DateAnnexe>");

            sw.WriteLine("<CodeAnnexe>" + code + "</CodeAnnexe>");

            sw.WriteLine("</Entete>");

            sw.WriteLine("<Annexe id='" + code + "'>");

            if (code == "140")
            {
                foreach (RNLPT40 pt in ListeRNLPT)
                {
                    if (pt.CodeRub != "13006590000000")
                    {
                        sw.WriteLine("<Instrument>");
                        sw.WriteLine("<CodeInstrument>" + pt.CodeInstrument + "</CodeInstrument>");
                        sw.WriteLine("<Colonnes>");
                        if (pt.Colonne1 != null)
                        {
                            sw.WriteLine("<Colonne id='1'>" + String.Format("{0:0.###}", pt.Colonne1.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='1'/>");
                        }
                        //
                        if (pt.Colonne2 != null)
                        {
                            sw.WriteLine("<Colonne id='2'>" + String.Format("{0:0.###}", pt.Colonne2.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='2'/>");
                        }
                        //
                        if (pt.Colonne3 != null)
                        {
                            sw.WriteLine("<Colonne id='3'>" + String.Format("{0:0.###}", pt.Colonne3.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='3'/>");
                        }
                        //
                        if (pt.Colonne4 != null)
                        {
                            sw.WriteLine("<Colonne id='4'>" + String.Format("{0:0.###}", pt.Colonne4.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='4'/>");
                        }
                        //
                        if (pt.Colonne5 != null)
                        {
                            sw.WriteLine("<Colonne id='5'>" + String.Format("{0:0.###}", pt.Colonne5.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='5'/>");
                        }
                        //
                        if (pt.Colonne6 != null)
                        {
                            sw.WriteLine("<Colonne id='6'>" + String.Format("{0:0.###}", pt.Colonne6.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='6'/>");
                        }
                        sw.WriteLine("</Colonnes>");
                        sw.WriteLine("</Instrument>");
                    }
                    else {
                        sw.WriteLine("<Rubrique id='" + pt.CodeRub + "'>");
                        sw.WriteLine("<Colonnes>");
                        if (pt.Colonne1 != null)
                        {
                            sw.WriteLine("<Colonne id='1'>" + String.Format("{0:0.###}", pt.Colonne1.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='1'/>");
                        }
                        //
                        if (pt.Colonne2 != null)
                        {
                            sw.WriteLine("<Colonne id='2'>" + String.Format("{0:0.###}", pt.Colonne2.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='2'/>");
                        }
                        //
                        if (pt.Colonne3 != null)
                        {
                            sw.WriteLine("<Colonne id='3'>" + String.Format("{0:0.###}", pt.Colonne3.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='3'/>");
                        }
                        //
                        if (pt.Colonne4 != null)
                        {
                            sw.WriteLine("<Colonne id='4'>" + String.Format("{0:0.###}", pt.Colonne4.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='4'/>");
                        }
                        //
                        if (pt.Colonne5 != null)
                        {
                            sw.WriteLine("<Colonne id='5'>" + String.Format("{0:0.###}", pt.Colonne5.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='5'/>");
                        }
                        //
                        if (pt.Colonne6 != null)
                        {
                            sw.WriteLine("<Colonne id='6'>" + String.Format("{0:0.###}", pt.Colonne6.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='6'/>");
                        }
                        sw.WriteLine("</Colonnes>");
                        sw.WriteLine("</Rubrique>");

                    }
                }

            }




           else if (code == "133")
            {

                foreach (RNLPT40 pt in ListeRNLPT)
                {

                    if (pt.CodeRub == "Instrument")
                    {
                        sw.WriteLine("<Instrument>");
                        sw.WriteLine("<CodeInstrument>" + pt.CodeInstrument + "</CodeInstrument>");
                        sw.WriteLine("<CodeTypeContrat>" + pt.CodeTypeContrat + "</CodeTypeContrat>");
                        sw.WriteLine("<CodeDevise>" + pt.CodeDevise + "</CodeDevise>");
                        sw.WriteLine("<Contrepartie>");
                        sw.WriteLine("<Identifiant>" + pt.Identifiant + "</Identifiant>");
                        sw.WriteLine("<TypeIdentifiant>" + pt.TypeIdentifiant + "</TypeIdentifiant>");
                        sw.WriteLine("<RaisonSociale>" + pt.RaisonSociale + "</RaisonSociale>");
                        sw.WriteLine("</Contrepartie>");
                        sw.WriteLine("<Colonnes>");
                        if (pt.Colonne1 != null)
                        {
                            sw.WriteLine("<Colonne id='1'>" + String.Format("{0:0.###}", pt.Colonne1.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='1'/>");
                        }
                        //
                        if (pt.Colonne2 != null)
                        {
                            sw.WriteLine("<Colonne id='2'>" + String.Format("{0:0.###}", pt.Colonne2.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='2'/>");
                        }
                        //
                        if (pt.Colonne3 != null)
                        {
                            sw.WriteLine("<Colonne id='3'>" + String.Format("{0:0.###}", pt.Colonne3.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='3'/>");
                        }
                        //
                        if (pt.Colonne4 != null)
                        {
                            sw.WriteLine("<Colonne id='4'>" + String.Format("{0:0.###}", pt.Colonne4.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='4'/>");
                        }
                        //
                        if (pt.Colonne5 != null)
                        {
                            sw.WriteLine("<Colonne id='5'>" + String.Format("{0:0.###}", pt.Colonne5.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='5'/>");
                        }
                        //
                        if (pt.Colonne6 != null)
                        {
                            sw.WriteLine("<Colonne id='6'>" + String.Format("{0:0.###}", pt.Colonne6.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='6'/>");
                        }
                        //
                        if (pt.Colonne7 != null)
                        {
                            sw.WriteLine("<Colonne id='7'>" + String.Format("{0:0.###}", pt.Colonne7.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='7'/>");
                        }
                        sw.WriteLine("</Colonnes>");
                        sw.WriteLine("</Instrument>");

                    }
                    else
                    {

                        sw.WriteLine("<Rubrique id='" + pt.CodeRub + "'>");
                        sw.WriteLine("<Colonnes>");
                        if (pt.Colonne1 != null)
                        {
                            sw.WriteLine("<Colonne id='1'>" + String.Format("{0:0.###}", pt.Colonne1.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='1'/>");
                        }
                        //
                        if (pt.Colonne2 != null)
                        {
                            sw.WriteLine("<Colonne id='2'>" + String.Format("{0:0.###}", pt.Colonne2.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='2'/>");
                        }
                        //
                        if (pt.Colonne3 != null)
                        {
                            sw.WriteLine("<Colonne id='3'>" + String.Format("{0:0.###}", pt.Colonne3.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='3'/>");
                        }
                        //
                        if (pt.Colonne4 != null)
                        {
                            sw.WriteLine("<Colonne id='4'>" + String.Format("{0:0.###}", pt.Colonne4.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='4'/>");
                        }
                        //
                        if (pt.Colonne5 != null)
                        {
                            sw.WriteLine("<Colonne id='5'>" + String.Format("{0:0.###}", pt.Colonne5.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='5'/>");
                        }
                        //
                        if (pt.Colonne6 != null)
                        {
                            sw.WriteLine("<Colonne id='6'>" + String.Format("{0:0.###}", pt.Colonne6.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='6'/>");
                        }
                        //
                        if (pt.Colonne7 != null)
                        {
                            sw.WriteLine("<Colonne id='7'>" + String.Format("{0:0.###}", pt.Colonne7.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='7'/>");
                        }
                        sw.WriteLine("</Colonnes>");
                        sw.WriteLine("</Rubrique>");
                    }


                }
            }
            else
            {
                string chaine = "";
                foreach (RNLPT40 pt in ListeRNLPT.FindAll(p => p.CodeRub != "D"))
                {
                    if (pt.CodeRub == "EM000000000000")
                    {

                        sw.WriteLine("<Rubrique id='" + pt.CodeRub + "'>");
                        sw.WriteLine("<Colonnes>");
                        if (pt.Colonne1 != null)
                        {
                            sw.WriteLine("<Colonne id='1'>" + String.Format("{0:0.###}", pt.Colonne1.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='1'/>");
                        }
                        //
                        if (pt.Colonne2 != null)
                        {
                            sw.WriteLine("<Colonne id='2'>" + String.Format("{0:0.###}", pt.Colonne2.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='2'/>");
                        }
                        //
                        if (pt.Colonne3 != null)
                        {
                            sw.WriteLine("<Colonne id='3'>" + String.Format("{0:0.###}", pt.Colonne3.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='3'/>");
                        }
                        sw.WriteLine("</Colonnes>");


                        foreach (RNLPT40 pt1 in ListeRNLPT.FindAll(p => p.CodeRub == "D"))
                        {
                            sw.WriteLine("<Emetteur>");
                            sw.WriteLine("<Identifiant>" + pt1.Identifiant + "</Identifiant>");
                            sw.WriteLine("<TypeIdentifiant>" + pt1.TypeIdentifiant + "</TypeIdentifiant>");
                            sw.WriteLine("<RaisonSociale>" + pt1.RaisonSociale + "</RaisonSociale>");

                            sw.WriteLine("<Colonnes>");
                            if (pt.Colonne1 != null)
                            {
                                sw.WriteLine("<Colonne id='1'>" + String.Format("{0:0.###}", pt.Colonne1.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='1'/>");
                            }
                            //
                            if (pt.Colonne2 != null)
                            {
                                sw.WriteLine("<Colonne id='2'>" + String.Format("{0:0.###}", pt.Colonne2.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='2'/>");
                            }
                            //
                            if (pt.Colonne3 != null)
                            {
                                sw.WriteLine("<Colonne id='3'>" + String.Format("{0:0.###}", pt.Colonne3.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='3'/>");
                            }
                            sw.WriteLine("</Colonnes>");
                            sw.WriteLine("</Emetteur>");

                        }
                        sw.WriteLine("</Rubrique>");
                    }



                    else
                    {
                        sw.WriteLine("<Rubrique id='" + pt.CodeRub + "'>");
                        sw.WriteLine("<Colonnes>");
                        if (pt.Colonne1 != null)
                        {
                            sw.WriteLine("<Colonne id='1'>" + String.Format("{0:0.###}", pt.Colonne1.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='1'/>");
                        }
                        //
                        if (pt.Colonne2 != null)
                        {
                            sw.WriteLine("<Colonne id='2'>" + String.Format("{0:0.###}", pt.Colonne2.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='2'/>");
                        }
                        //
                        if (pt.Colonne3 != null)
                        {
                            sw.WriteLine("<Colonne id='3'>" + String.Format("{0:0.###}", pt.Colonne3.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='3'/>");
                        }
                        sw.WriteLine("</Colonnes>");
                        sw.WriteLine("</Rubrique>");

                    }

                }

             

            }
            sw.WriteLine("</Annexe>");
            sw.WriteLine("</Document>");
            sw.Close();
            byte[] imgByteArr = System.IO.File.ReadAllBytes(path);
            return File(imgByteArr, System.Net.Mime.MediaTypeNames.Application.Octet, "TES.xml");
        }



        [HttpPost]
        public ActionResult creationXML(List<RNLPT> ListeRNLPT,DateTime dt,string TypeFile)
        {


            string code = "";
            switch (TypeFile)
            {
                case "RNLPT130010": code = "130"; break;
                case "RNLPT130020": code = "131"; break;
                case "RNLPT130030": code = "132"; break;
                case "RNLPT130050": code = "134"; break;
                case "RNLPT130060": code = "135"; break;
                case "RNLPT130061": code = "136"; break;
                case "RNLPT130062": code = "137"; break;
                case "RNLPT130064": code = "139"; break;
                case "RNLPT130066": code = "141"; break;
                case "RNLPT130070": code = "142"; break;

            }





            string name = TypeFile + "_XML_" + DateTime.Now.Date.Year.ToString() + DateTime.Now.Date.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Second.ToString() + ".xml";


                string path = @"C:\\BCT\" + name;
            // @"C:\\BCT\"
            StreamWriter sw = new StreamWriter(path);
                sw.WriteLine("<?xml version='1.0' encoding='UTF-8' standalone='no'?>");

                sw.WriteLine("<Document>");

                sw.WriteLine("<Entete>");

                sw.WriteLine("<CodeBanque>60</CodeBanque>");

                sw.WriteLine("<DateAnnexe>" + dt.ToString("yyyyMMdd") + "</DateAnnexe>");

                sw.WriteLine("<CodeAnnexe>"+code+"</CodeAnnexe>");

                sw.WriteLine("</Entete>");

                sw.WriteLine("<Annexe id='"+code+"'>");
            // int i = 0;
            bool t = false;
                switch (TypeFile)
                {
                    case "RNLPT130010":

                        foreach (RNLPT r in ListeRNLPT)
                        {
                            sw.WriteLine("<Rubrique id='" + r.CodeRub + "'>");
                            if (r.Colonne1 != null)
                            {
                                sw.WriteLine("<Colonne id='1'>" + String.Format("{0:0.###}", r.Colonne1.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='1'/>");
                            }

                            sw.WriteLine("</Rubrique>");
                        }
                        break;
                case "RNLPT130066":

                    foreach (RNLPT r in ListeRNLPT)
                    {
                        sw.WriteLine("<Rubrique id='" + r.CodeRub + "'>");
                        if (r.Colonne1 != null)
                        {
                            sw.WriteLine("<Colonne id='1'>" + String.Format("{0:0.###}", r.Colonne1.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='2'/>");
                        }
                        if (r.Colonne2 != null)
                        {
                            sw.WriteLine("<Colonne id='2'>" + String.Format("{0:0.###}", r.Colonne2.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='2'/>");
                        }
                        sw.WriteLine("</Rubrique>");
                    }
                    break;
                case "RNLPT130070":

                    foreach (RNLPT r in ListeRNLPT.FindAll(p=>p.CodeRub.IndexOf("D")==-1))
                    {
                        sw.WriteLine("<Rubrique id='" + r.CodeRub + "'>");
                        if (r.Colonne1 != null)
                        {
                            sw.WriteLine("<Colonne id='1'>" + String.Format("{0:0.###}", r.Colonne1.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='1'/>");
                        }
                        if (r.Colonne2 != null)
                        {
                            sw.WriteLine("<Colonne id='2'>" + String.Format("{0:0.###}", r.Colonne2.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='2'/>");
                        }
                        if (r.Colonne3 != null)
                        {
                            sw.WriteLine("<Colonne id='3'>" + String.Format("{0:0.###}", r.Colonne3.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='3'/>");
                        }
                        if (r.Colonne4 != null)
                        {
                            sw.WriteLine("<Colonne id='4'>" + String.Format("{0:0.###}", r.Colonne4.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='4'/>");
                        }
                  

                        foreach (RNLPT rnlpt in ListeRNLPT.FindAll(p => p.CodeRub.Contains("D"+r.CodeRub)))
                        {
                            sw.WriteLine("<Detail  id='" + rnlpt.CodeRub.Substring(rnlpt.CodeRub.IndexOf("-")+1,3) + "'>");
                            if (rnlpt.Colonne1 != null)
                            {
                                sw.WriteLine("<Colonne id='1'>" + String.Format("{0:0.###}", rnlpt.Colonne1.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='2'/>");
                            }
                            if (rnlpt.Colonne2 != null)
                            {
                                sw.WriteLine("<Colonne id='2'>" + String.Format("{0:0.###}", rnlpt.Colonne2.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='2'/>");
                            }
                            if (rnlpt.Colonne3 != null)
                            {
                                sw.WriteLine("<Colonne id='3'>" + String.Format("{0:0.###}", rnlpt.Colonne3.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='3'/>");
                            }
                            if (rnlpt.Colonne4 != null)
                            {
                                sw.WriteLine("<Colonne id='4'>" + String.Format("{0:0.###}", rnlpt.Colonne4.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='4'/>");
                            }
                            sw.WriteLine("</Detail>");
                        }
                        sw.WriteLine("</Rubrique>");

                    }
                    break;
                case "RNLPT130062":

                    RNLPT r1 = new RNLPT();
                    r1.Devise = "";
                    for (int i = 1; i < ListeRNLPT.Count; i++)
                    {
                        t = r1.Devise != ListeRNLPT[i].Devise;
                        if (r1.Devise != ListeRNLPT[i].Devise)
                        {
                            sw.WriteLine("<Devise id='" + ListeRNLPT[i].Devise + "'>");
                            r1 = ListeRNLPT[i];

                        }

                        sw.WriteLine("<Rubrique id='" + ListeRNLPT[i].CodeRub + "'>");
                        if (ListeRNLPT[i].Colonne1 != null)
                        {
                            sw.WriteLine("<Colonne id='1'>" + String.Format("{0:0.###}", ListeRNLPT[i].Colonne1.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='1'/>");
                        }

                        if (ListeRNLPT[i].Colonne2 != null)
                        {
                            sw.WriteLine("<Colonne id='2'>" + String.Format("{0:0.###}", ListeRNLPT[i].Colonne2.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='2'/>");
                        }

                        if (ListeRNLPT[i].Colonne3 != null)
                        {
                            sw.WriteLine("<Colonne id='3'>" + String.Format("{0:0.###}", ListeRNLPT[i].Colonne3.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='3'/>");
                        }

                        if (ListeRNLPT[i].Colonne4 != null)
                        {
                            sw.WriteLine("<Colonne id='4'>" + String.Format("{0:0.###}", ListeRNLPT[i].Colonne4.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='4'/>");
                        }

                        if (ListeRNLPT[i].Colonne5 != null)
                        {
                            sw.WriteLine("<Colonne id='5'>" + String.Format("{0:0.###}", ListeRNLPT[i].Colonne5.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='5'/>");
                        }
                        if (ListeRNLPT[i].Colonne6 != null)
                        {
                            sw.WriteLine("<Colonne id='6'>" + String.Format("{0:0.###}", ListeRNLPT[i].Colonne6.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='6'/>");
                        }
                        if (ListeRNLPT[i].Colonne7 != null)
                        {
                            sw.WriteLine("<Colonne id='7'>" + String.Format("{0:0.###}", ListeRNLPT[i].Colonne7.Replace(",", ".")) + "</Colonne>");
                        }
                        else
                        {
                            sw.WriteLine("<Colonne id='7'/>");
                        }
                       
                        sw.WriteLine("</Rubrique>");

                        if (ListeRNLPT[i].CodeRub.Contains("13006990000000"))
                        {
                            sw.WriteLine("</Devise>");

                        }
                    }
                
                  
                    break;
                case "RNLPT130050":

                        foreach (RNLPT r in ListeRNLPT)
                        {
                            sw.WriteLine("<Rubrique id='" + r.CodeRub + "'>");
                            if (r.Colonne1 != null)
                            {
                                sw.WriteLine("<Colonne id='1'>" + String.Format("{0:0.###}", r.Colonne1.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='1'/>");
                            }

                            sw.WriteLine("</Rubrique>");
                        }
                        break;
                    case "RNLPT130060":

                        foreach (RNLPT r in ListeRNLPT)
                        {
                            sw.WriteLine("<Rubrique id='" + r.CodeRub + "'>");
                            if (r.Colonne1 != null)
                            {
                                sw.WriteLine("<Colonne id='1'>" + String.Format("{0:0.###}", r.Colonne1.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='1'/>");
                            }

                            sw.WriteLine("</Rubrique>");
                        }
                        break;
                    case "RNLPT130020":

                        foreach (RNLPT r in ListeRNLPT)
                        {
                            sw.WriteLine("<Rubrique id='" + r.CodeRub + "'>");
                            if (r.Colonne1 != null)
                            {
                                sw.WriteLine("<Colonne id='1'>" + String.Format("{0:0.###}", r.Colonne1.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='1'/>");
                            }

                            sw.WriteLine("</Rubrique>");
                        }
                        break;
                    case "RNLPT130030":

                        foreach (RNLPT r in ListeRNLPT)
                        {
                            sw.WriteLine("<Rubrique id='" + r.CodeRub + "'>");
                            if (r.Colonne1 != null)
                            {
                                sw.WriteLine("<Colonne id='1'>" + String.Format("{0:0.###}", r.Colonne1.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='1'/>");
                            }
                            //
                            if (r.Colonne2 != null)
                            {
                                sw.WriteLine("<Colonne id='2'>" + String.Format("{0:0.###}", r.Colonne2.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='2'/>");
                            }
                            //
                            if (r.Colonne3 != null)
                            {
                                sw.WriteLine("<Colonne id='3'>" + String.Format("{0:0.###}", r.Colonne3.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='3'/>");
                            }
                            //
                            if (r.Colonne4 != null)
                            {
                                sw.WriteLine("<Colonne id='4'>" + String.Format("{0:0.###}", r.Colonne4.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='4'/>");
                            }
                            //
                            if (r.Colonne5 != null)
                            {
                                sw.WriteLine("<Colonne id='5'>" + String.Format("{0:0.###}", r.Colonne5.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='5'/>");
                            }
                            //
                            if (r.Colonne6 != null)
                            {
                                sw.WriteLine("<Colonne id='6'>" + String.Format("{0:0.###}", r.Colonne6.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='6'/>");
                            }
                            //
                            if (r.Colonne7 != null)
                            {
                                sw.WriteLine("<Colonne id='7'>" + String.Format("{0:0.###}", r.Colonne7.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='7'/>");
                            }
                            //
                            if (r.Colonne8 != null)
                            {
                                sw.WriteLine("<Colonne id='8'>" + String.Format("{0:0.###}", r.Colonne8.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='8'/>");
                            }
                            //
                            if (r.Colonne9 != null)
                            {
                                sw.WriteLine("<Colonne id='9'>" + String.Format("{0:0.###}", r.Colonne9.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='9'/>");
                            }
                            //
                            if (r.Colonne10 != null)
                            {
                                sw.WriteLine("<Colonne id='10'>" + String.Format("{0:0.###}", r.Colonne10.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='10'/>");
                            }
                            //
                            if (r.Colonne11 != null)
                            {
                                sw.WriteLine("<Colonne id='11'>" + String.Format("{0:0.###}", r.Colonne11.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='11'/>");
                            }
                            //
                            if (r.Colonne12 != null)
                            {
                                sw.WriteLine("<Colonne id='12'>" + String.Format("{0:0.###}", r.Colonne12.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='12'/>");
                            }
                            //
                            if (r.Colonne13 != null)
                            {
                                sw.WriteLine("<Colonne id='13'>" + String.Format("{0:0.###}", r.Colonne13.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='13'/>");
                            }
                            //
                            if (r.Colonne14 != null)
                            {
                                sw.WriteLine("<Colonne id='14'>" + String.Format("{0:0.###}", r.Colonne14.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='14'/>");
                            }
                            sw.WriteLine("</Rubrique>");
                        }
                        break;

                    case "RNLPT130061":

                        foreach (RNLPT r in ListeRNLPT)
                        {
                            sw.WriteLine("<Rubrique id='" + r.CodeRub + "'>");
                            if (r.Colonne1 != null)
                            {
                                sw.WriteLine("<Colonne id='1'>" + String.Format("{0:0.###}", r.Colonne1.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='1'/>");
                            }
                            //
                            if (r.Colonne2 != null)
                            {
                                sw.WriteLine("<Colonne id='2'>" + String.Format("{0:0.###}", r.Colonne2.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='2'/>");
                            }
                            //
                            if (r.Colonne3 != null)
                            {
                                sw.WriteLine("<Colonne id='3'>" + String.Format("{0:0.###}", r.Colonne3.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='3'/>");
                            }
                            //
                            if (r.Colonne4 != null)
                            {
                                sw.WriteLine("<Colonne id='4'>" + String.Format("{0:0.###}", r.Colonne4.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='4'/>");
                            }
                            
                            sw.WriteLine("</Rubrique>");
                        }
                        break;
                    case "RNLPT130064":

                        foreach (RNLPT r in ListeRNLPT)
                        {
                            sw.WriteLine("<Rubrique id='" + r.CodeRub + "'>");
                            if (r.Colonne1 != null)
                            {
                                sw.WriteLine("<Colonne id='1'>" + String.Format("{0:0.###}", r.Colonne1.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='1'/>");
                            }
                            //
                            if (r.Colonne2 != null)
                            {
                                sw.WriteLine("<Colonne id='2'>" + String.Format("{0:0.###}", r.Colonne2.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='2'/>");
                            }
                            //
                            if (r.Colonne3 != null)
                            {
                                sw.WriteLine("<Colonne id='3'>" + String.Format("{0:0.###}", r.Colonne3.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='3'/>");
                            }
                            //
                            if (r.Colonne4 != null)
                            {
                                sw.WriteLine("<Colonne id='4'>" + String.Format("{0:0.###}", r.Colonne4.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='4'/>");
                            }
                            //
                            if (r.Colonne5 != null)
                            {
                                sw.WriteLine("<Colonne id='5'>" + String.Format("{0:0.###}", r.Colonne5.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='5'/>");
                            }
                            //
                            if (r.Colonne6 != null)
                            {
                                sw.WriteLine("<Colonne id='6'>" + String.Format("{0:0.###}", r.Colonne6.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='6'/>");
                            }
                            //
                            if (r.Colonne7 != null)
                            {
                                sw.WriteLine("<Colonne id='7'>" + String.Format("{0:0.###}", r.Colonne7.Replace(",", ".")) + "</Colonne>");
                            }
                            else
                            {
                                sw.WriteLine("<Colonne id='7'/>");
                            }
                          
                            sw.WriteLine("</Rubrique>");
                        }
                        break;

                        
                }


               
                sw.WriteLine("</Annexe>");

                sw.WriteLine("</Document>");


                sw.Close();
              byte[] imgByteArr = System.IO.File.ReadAllBytes(path);

            DownloadFile(path, name);
            return File(imgByteArr, "text/xml", name);


           // string mp = Server.MapPath("~/Content/myXml.xml");
          //  myXmlDocument.Save(mp);
           // return File(path, "text/xml", name);
            // return File(Encoding.UTF8.GetBytes(path), "application/xml", "TES.xml");

        }
        public HttpResponseMessage DownloadFile(string fileName,string fileNamee)
        {

            if (!string.IsNullOrEmpty(fileName))
            {

                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    var fileStream = new FileStream(fileName, FileMode.Open);
                    response.Content = new StreamContent(fileStream);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentDisposition.FileName = fileNamee;
                    return response;
                
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);

        }
        public ActionResult CreationEmptyFile(DateTime dt)
        {

            string path = @"C:\\BCT\" + DateTime.Now.Date.Year.ToString() + DateTime.Now.Date.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Second.ToString() + ".xml";
          //  @"C:\\BCT\RCM00_XML_"
           StreamWriter sw = new StreamWriter(path);
            sw.WriteLine("<?xml version='1.0' encoding='UTF-8' standalone='no'?>");

            sw.WriteLine("<Document>");

            sw.WriteLine("<Entete>");

            sw.WriteLine("<CodeBanque>60</CodeBanque>");


            sw.WriteLine("<DateAnnexe>" + dt.ToString("yyyyMMdd") + "</DateAnnexe>");


            sw.WriteLine("<CodeAnnexe>00</CodeAnnexe>");

            sw.WriteLine("</Entete>");

            sw.WriteLine("<Annexe id='130'>");

            sw.WriteLine("</Annexe>");

            sw.WriteLine("</Document>");



            sw.Close();

             byte[] imgByteArr = System.IO.File.ReadAllBytes(path);
           // return File(Encoding.UTF8.GetBytes(path), "application/xml", "TES.xml");
            return File(imgByteArr, System.Net.Mime.MediaTypeNames.Application.Octet, "TES.xml");
          
        }
       

        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }
    }
}