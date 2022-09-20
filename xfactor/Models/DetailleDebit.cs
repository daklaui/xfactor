using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace xfactor.Models
{
    public class DetailleDebit
    {
        public int ID_DEBIT { get; set; }
        public int REF_CTR_CIR { get; set; }
        public string NOM_IND { get; set; }
        public string ABEV_DEBIT { get; set; }
        public Nullable<decimal> MONT_DEBIT { get; set; }
        public Nullable<System.DateTime> DATE_DEBIT { get; set; }
        public string ADRESS_DOC_GED { get; set; }
    }
}