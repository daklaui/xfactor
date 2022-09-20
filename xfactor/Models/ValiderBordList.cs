using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace xfactor.Models
{
    public class ValiderBordList
    {
        public string NUM_BORD { get; set; }
        public int REF_CTR_BORD { get; set; }
        public Nullable<short> NB_DOC_BORD { get; set; }
        public string ANNEE_BORD { get; set; }
        public int Nbre_Det { get; set; }
        public string Nom_ADH { get; set; }
        public decimal MontantTotale { get; set; }
        public DateTime DAT_SAISIE_BORD { get; set; }

    }
}