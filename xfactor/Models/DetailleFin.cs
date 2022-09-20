using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using xfactor.Models;
namespace xfactor.Models
{
    public class DetailleFin
    {
        public Nullable<int> REF_CTR_FIN { get; set; }
        public Nullable<decimal> MONT_FIN { get; set; }
        public Nullable<System.DateTime> DAT_FIN { get; set; }
        public string INSTR_FIN { get; set; }
        public string REF_INSTR_FIN { get; set; }
        public string ADRESS_DOC_GED { get; set; }
    }
}