using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace xfactor.Models
{
    public class DetailleCredit
    {
        public int ID_CREDIT { get; set; }
        public int REF_CTR_CREDIT { get; set; }
        public string NOM_IND { get; set; }
        public Nullable<decimal> MONT_CREDIT { get; set; }
        public Nullable<System.DateTime> DATE_CREDIT { get; set; }
        public string LIBELLE_LIBRE_CREDIT { get; set; }
        public string REF_ENC_CREDIT { get; set; }
        public string ADRESS_DOC_GED { get; set; }

    }
}