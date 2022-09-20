using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace xfactor.Models
{
    public class DetailleEnc
    {
        public string REF_ENC { get; set; }
        public Nullable<decimal> MONT_ENC { get; set; }
        public Nullable<System.DateTime> DAT_RECEP_ENC { get; set; }
        public Nullable<System.DateTime> DAT_VAL_ENC { get; set; }
        public string ADRESS_DOC_GED { get; set; }
    }
}