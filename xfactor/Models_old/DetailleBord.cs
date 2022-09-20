using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace xfactor.Models
{
    public class DetailleBord
    {
        public string ID_DET_BORD { get; set; }
        public string NUM_BORD { get; set; }
        public string REF_DET_BORD { get; set; }
        public string TYP_DET_BORD { get; set; }
        public Nullable<DateTime> DAT_DET_BORD { get; set; }
        public decimal MONT_TTC_DET_BORD { get; set; }
        public string ANNEE_BORD { get; set; }
        public int REF_CTR_DET_BORD { get; set; }
        public short ECH_DET_BORD { get; set; }

        public string MODE_REG_DET_BORD { get; set; }
      
        public string nomind { get; set; }
    }
}