using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace xfactor.Models
{
    public class Affiche
    {
        public Affiche(ListeDesContrat contrat, List<Recherche_CTR_ADH_ACH_Par_CTR2019_Result> listeDesInter)
        {
            this.contrat = contrat;
            this.listeDesInter = listeDesInter;
        }

        public ListeDesContrat contrat{get;set;}
        public List<Recherche_CTR_ADH_ACH_Par_CTR2019_Result> listeDesInter { get; set; }
    }
}