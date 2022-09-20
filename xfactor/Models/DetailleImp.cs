using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace xfactor.Models
{
    public class DetailleImp
    {
        public string NOM_IND { get; set; }
        public string REF_ENC { get; set; }
        public Nullable<decimal> MONT_ENC { get; set; }
        public Nullable<System.DateTime> DAT_VAL_ENC { get; set; }
        public string ADRESS_DOC_GED { get; set; }
    }
}