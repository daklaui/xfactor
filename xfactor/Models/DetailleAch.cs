using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace xfactor.Models
{
    public class DetailleAch
    {
        public string REF_CTR_DET_BORD { get; set; }
        public string nom_ind { get; set; }
        public decimal Encours_Factures_Ouvertes { get; set; }
        public decimal Limite_De_Financement { get; set; }
        public decimal Limite_De_Crédit { get; set; }
     
    }
}