using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace xfactor.Models
{
    public class T_int_financements_web
    {
        public string TYP_INSTR_INT_FIN { get; set; }
        public int REF_CTR_INT_FIN { get; set; }
        public Nullable<decimal> TX_INT_MARCHE_INT_FIN { get; set; }
        public Nullable<decimal> TX_MARGE_CTR_INT_FIN { get; set; }
        public Nullable<short> DELAI_MAX_PAI_INT_FIN { get; set; }
        public string PRECOMPTE_INT_FIN { get; set; }
        public string MAJOR_INT_INT_FIN { get; set; }
        public Nullable<System.DateTime> DAT_DEB_VALID_INT_FIN { get; set; }
    }
}