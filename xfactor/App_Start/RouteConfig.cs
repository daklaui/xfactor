using System.Web.Mvc;
using System.Web.Routing;

namespace xfactor
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
               "Bordereau/RecherchFacture","Bordereau/RecherchFacture/{id}/{id2}",defaults: new { controller = "Bordereau", action = "RecherchFacture", id = UrlParameter.Optional,id2=UrlParameter.Optional }
          );
            routes.MapRoute(
              "Parametrage/RecherchFact", "Parametrage/RecherchFact/{id}/{id2}", defaults: new { controller = "Parametrage", action = "RecherchFact", id = UrlParameter.Optional, id2 = UrlParameter.Optional }
         );
            routes.MapRoute(
             "Bordereau/valider_trop_percu", "Bordereau/valider_trop_percu/{id}/{id2}/{id3}", defaults: new { controller = "Bordereau", action = "valider_trop_percu", id = UrlParameter.Optional, id2 = UrlParameter.Optional,id3=UrlParameter.Optional }
        );
            routes.MapRoute(
             "LitigesEtProrogations/RecherchFacture", "LitigesEtProrogations/RecherchFacture/{id}/{id2}", defaults: new { controller = "LitigesEtProrogations", action = "RecherchFacture", id = UrlParameter.Optional, id2 = UrlParameter.Optional }
        );
            routes.MapRoute(
             "Bordereau/ResoluionImpaye", "Bordereau/ResoluionImpaye/{id}/{id2}", defaults: new { controller = "Bordereau", action = "ResoluionImpaye", id = UrlParameter.Optional, id2 = UrlParameter.Optional }
        );
            routes.MapRoute(
           "LitigesEtProrogations/RecherchFactureVersion2", "LitigesEtProrogations/RecherchFactureVersion2/{id}/{id2}", defaults: new { controller = "LitigesEtProrogations", action = "RecherchFactureVersion2", id = UrlParameter.Optional, id2 = UrlParameter.Optional }
      );
            routes.MapRoute(
                       "Bordereau/ValiderBordereauJson", "Bordereau/ValiderBordereauJson/{id}/{id2}/{id3}", defaults: new { controller = "Bordereau", action = "ValiderBordereauJson", id = UrlParameter.Optional, id2 = UrlParameter.Optional , id3 = UrlParameter.Optional }
                  );
            routes.MapRoute(
                      "Bordereau/RejeterBordereauJson", "Bordereau/RejeterBordereauJson/{id}/{id2}/{id3}", defaults: new { controller = "Bordereau", action = "RejeterBordereauJson", id = UrlParameter.Optional, id2 = UrlParameter.Optional, id3 = UrlParameter.Optional }
                 );

             routes.MapRoute(
                      "Rapporting/Rapport_Extrait_Rapp", "Rapporting/Rapport_Extrait_Rapp/{id}/{id2}/{id3}", defaults: new { controller = "Rapporting", action = "Rapport_Extrait_Rapp", id = UrlParameter.Optional, id2 = UrlParameter.Optional, id3 = UrlParameter.Optional }
                 );
            routes.MapRoute(
                     "Bordereau/DetailleBord", "Bordereau/DetailleBord/{id}/{id2}/{id3}", defaults: new { controller = "Bordereau", action = "DetailleBord", id = UrlParameter.Optional, id2 = UrlParameter.Optional, id3 = UrlParameter.Optional }
                );
            routes.MapRoute(
                    "Bordereau/SelectMntEncImpaye", "Bordereau/SelectMntEncImpaye/{id}/{id2}", defaults: new { controller = "Bordereau", action = "SelectMntEncImpaye", id = UrlParameter.Optional, id2 = UrlParameter.Optional }
               );
            routes.MapRoute(
             "Parametrage/AnnulerLetr", "Parametrage/AnnulerLetr/{id}/{id2}", defaults: new { controller = "Parametrage", action = "AnnulerLetr", id = UrlParameter.Optional, id2 = UrlParameter.Optional }
        );
            routes.MapRoute(
            "Rapporting/SituationAdhérentAcheteurRapport", "Rapporting/SituationAdhérentAcheteurRapport/{id}/{id2}", defaults: new { controller = "Rapporting", action = "SituationAdhérentAcheteurRapport", id = UrlParameter.Optional, id2 = UrlParameter.Optional }
       );
            routes.MapRoute(
           "Financement/DetaileFactureParAn", "Financement/DetaileFactureParAn/{id}/{id2}", defaults: new { controller = "Financement", action = "DetaileFactureParAn", id = UrlParameter.Optional, id2 = UrlParameter.Optional }
      );
            routes.MapRoute(
          "Financement/ValiderFinancementV", "Financement/ValiderFinancementV/{id}/{id2}", defaults: new { controller = "Financement", action = "ValiderFinancementV", id = UrlParameter.Optional, id2 = UrlParameter.Optional }
     );


            routes.MapRoute(
           "Rapporting/Commission_par_bord_Rapport", "Rapporting/Commission_par_bord_Rapport/{id}/{id2}", defaults: new { controller = "Rapporting", action = "Commission_par_bord_Rapport", id = UrlParameter.Optional, id2 = UrlParameter.Optional }
      );

            routes.MapRoute(
          "Rapporting/EtatDesFacturesParAchRapp", "Rapporting/EtatDesFacturesParAchRapp/{id}/{id2}", defaults: new { controller = "Rapporting", action = "EtatDesFacturesParAchRapp", id = UrlParameter.Optional, id2 = UrlParameter.Optional }
     );
            routes.MapRoute(
                    "Rapporting/EtatDesEncaissementsParAchRapp", "Rapporting/EtatDesEncaissementsParAchRapp/{id}/{id2}", defaults: new { controller = "Rapporting", action = "EtatDesEncaissementsParAchRapp", id = UrlParameter.Optional, id2 = UrlParameter.Optional }
               );
            routes.MapRoute(
                   "Bordereau/Verife_Facture", "Bordereau/Verife_Facture/{id}/{id2}", defaults: new { controller = "Bordereau", action = "Verife_Facture", id = UrlParameter.Optional, id2 = UrlParameter.Optional }
              );
            routes.MapRoute(
        "Rapporting/Encaissements_Non_Echus_Rapport", "Rapporting/Encaissements_Non_Echus_Rapport/{id}/{id2}", defaults: new { controller = "Rapporting", action = "Encaissements_Non_Echus_Rapport", id = UrlParameter.Optional, id2 = UrlParameter.Optional }
   );
            routes.MapRoute(
                   "Rapporting/EtatDesFacturesParAch_Rapport", "Rapporting/EtatDesFacturesParAch_Rapport/{id}/{id2}/{id3}", defaults: new { controller = "Rapporting", action = "EtatDesFacturesParAch_Rapport", id = UrlParameter.Optional, id2 = UrlParameter.Optional, id3 = UrlParameter.Optional }
              );
            routes.MapRoute(
                   "Rapporting/Encours_FacturesRappdate", "Rapporting/Encours_FacturesRappdate/{id}/{id2}/{id3}", defaults: new { controller = "Rapporting", action = "Encours_FacturesRappdate", id = UrlParameter.Optional, id2 = UrlParameter.Optional, id3 = UrlParameter.Optional }
              );
            routes.MapRoute(
                  "Rapporting/Encours_Factures_Rapportdate", "Rapporting/Encours_Factures_Rapportdate/{id}/{id2}/{id3}/{id4}", defaults: new { controller = "Rapporting", action = "Encours_Factures_Rapportdate", id = UrlParameter.Optional, id2 = UrlParameter.Optional, id3 = UrlParameter.Optional, id4 = UrlParameter.Optional }
             );

            routes.MapRoute(
                "Rapporting/Factures_En_Retard_Rapp_ach", "Rapporting/Factures_En_Retard_Rapp_ach/{id}/{id2}", defaults: new { controller = "Rapporting", action = "Factures_En_Retard_Rapp_ach", id = UrlParameter.Optional, id2 = UrlParameter.Optional }
           );

            routes.MapRoute(
                "Rapporting/Factures_En_Retard_Rapport_ach", "Rapporting/Factures_En_Retard_Rapport_ach/{id}/{id2}/{id3}", defaults: new { controller = "Rapporting", action = "Factures_En_Retard_Rapport_ach", id = UrlParameter.Optional, id2 = UrlParameter.Optional, id3 = UrlParameter.Optional }
           );
            routes.MapRoute(
                    "GED/ListeOfLibs", "GED/ListeOfLibs/{id}/{id2}", defaults: new { controller = "GED", action = "ListeOfLibs", id = UrlParameter.Optional, id2 = UrlParameter.Optional}
               );
            routes.MapRoute(
                   "Bordereau/GenereBordCh", "Bordereau/GenereBordCh/{id}/{id2}", defaults: new { controller = "Bordereau", action = "GenereBordCh", id = UrlParameter.Optional, id2 = UrlParameter.Optional }
              );
            routes.MapRoute(
                  "Bordereau/Bordereau_CH_Rapport", "Bordereau/Bordereau_CH_Rapport/{id1}/{id2}/{id3}", defaults: new { controller = "Bordereau", action = "Bordereau_CH_Rapport", id1 = UrlParameter.Optional, id2 = UrlParameter.Optional,id3= UrlParameter.Optional }
             );
            routes.MapRoute(
                 "Home/ListeDesContartAChercher", "Home/ListeDesContartAChercher/{id}/{id2}/{id3}/{id4}", defaults: new { controller = "Home", action = "ListeDesContartAChercher", id = UrlParameter.Optional, id2 = UrlParameter.Optional, id3 = UrlParameter.Optional, id4 = UrlParameter.Optional }
            );
            routes.MapRoute(
             "Rapporting/Extrait_Compte_par_mois", "Rapporting/Extrait_Compte_par_mois/{id}/{id1}/{id2}", defaults: new { controller = "Rapporting", action = "Extrait_Compte_par_mois", id = UrlParameter.Optional, id1 = UrlParameter.Optional, id2 = UrlParameter.Optional}
        );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Login", action = "login", id = UrlParameter.Optional }
            );
           
        }
    }
}
