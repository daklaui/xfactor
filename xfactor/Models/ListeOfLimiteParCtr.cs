using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace xfactor.Models
{
    public class ListeOfLimiteParCtr
    {
        public int REF_CTR_DEM_LIM { get; set; }
        public string TYP_DEM_LIM { get; set; }
        public Nullable<System.DateTime> DAT_DEM_LIM { get; set; }
        public string DEVISE { get; set; }
        public Nullable<long> DELAIS_DEM_LIM { get; set; }
        public string MODE_PAY_DEM_LIM { get; set; }
        public Nullable<decimal> MONT_DEM_LIM { get; set; }
        public Nullable<int> DELAIS_ACC { get; set; }
        public Nullable<decimal> MONT_ACC { get; set; }
        public string MODE_PAY_ACC { get; set; }
        public Nullable<int> REF_ACH_LIM { get; set; }
        public string NOM_IND { get; set; }


        public ListeOfLimiteParCtr(int REF_CTR_DEM_LIM, string TYP_DEM_LIM, Nullable<System.DateTime> DAT_DEM_LIM, string DEVISE, Nullable<long> DELAIS_DEM_LIM, string MODE_PAY_DEM_LIM, Nullable<decimal> MONT_DEM_LIM, Nullable<int> DELAIS_ACC, string MODE_PAY_ACC, Nullable<int> REF_ACH_LIM, string NOM_IND)
        {
            this.REF_CTR_DEM_LIM = REF_CTR_DEM_LIM;
            this.TYP_DEM_LIM = TYP_DEM_LIM;
            this.DAT_DEM_LIM = DAT_DEM_LIM;
            this.DEVISE = DEVISE;
            this.DELAIS_DEM_LIM = DELAIS_DEM_LIM;
            this.MODE_PAY_DEM_LIM = MODE_PAY_DEM_LIM;
            this.MONT_DEM_LIM = MONT_DEM_LIM;
            this.DELAIS_ACC = DELAIS_ACC;
            this.MONT_ACC = MONT_ACC;
            this.MODE_PAY_ACC = MODE_PAY_ACC;
            this.REF_ACH_LIM = REF_ACH_LIM;
            this.NOM_IND = NOM_IND;

        }

    }
}