using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace xfactor.Models
{
    public class T_demande_limite
    {
        public int REF_DEM_LIM { get; set; }
        public int REF_CTR_DEM_LIM { get; set; }
        public string TYP_DEM_LIM { get; set; }
        public System.DateTime DAT_DEM_LIM { get; set; }
        public System.DateTime DAT_DEM_LIMC { get; set; }
        public string SORT_DEM_LIM { get; set; }
        public string MONT_DEM_LIM { get; set; }
        public string MONT_DEM_LIMC { get; set; }
        public string DEVISE { get; set; }
        public Nullable<System.DateTime> DAT_LAST_DEM_LIM { get; set; }
        public string DECISION_LIM { get; set; }
        public string MONT_ACC { get; set; }
        public string MONT_LIM_ACH_ADH { get; set; }
        public Nullable<System.DateTime> DAT_ANNUL_DEM_LIM { get; set; }
        public Nullable<System.DateTime> DATLIM_DEM_LIM { get; set; }
        public Nullable<short> DELAIS_DEM_LIM { get; set; }
        public Nullable<short> DELAIS_ACC { get; set; }
        public string MODE_PAY_DEM_LIM { get; set; }
        public string MODE_PAY_ACC { get; set; }
        public Nullable<bool> ACTIF_DEM_LIMI { get; set; }
        public Nullable<int> REF_ACH_LIM { get; set; }
    }
}