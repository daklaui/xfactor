using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace xfactor.Models
{
    public class T_Contrat_Web
    {
        public int REF_CTR { get; set; }
        public string STATUT_CTR { get; set; }
        public string REF_CTR_PAPIER_CTR { get; set; }
        public Nullable<bool> SERVICE_CTR { get; set; }
        public string TYP_CTR { get; set; }
        public Nullable<System.DateTime> DAT_SIGN_CTR { get; set; }
        public Nullable<System.DateTime> DAT_DEB_CTR { get; set; }
        public Nullable<System.DateTime> DAT_RESIL_CTR { get; set; }
        public Nullable<System.DateTime> DAT_PROCH_VERS_CTR { get; set; }
        public string CA_CTR { get; set; }
        public string CA_EXP_CTR { get; set; }
        public string CA_IMP_CTR { get; set; }
        public string LIM_FIN_CTR { get; set; }
        public string DEVISE_CTR { get; set; }
        public Nullable<short> NB_ACH_PREVU_CTR { get; set; }
        public Nullable<short> NB_FACT_PREVU_CTR { get; set; }
        public Nullable<short> NB_AVOIRS_PREVU_CTR { get; set; }
        public Nullable<short> NB_REMISES_PREVU_CTR { get; set; }
        public Nullable<short> DELAI_MOYEN_REG_CTR { get; set; }
        public Nullable<short> DELAI_MAX_REG_CTR { get; set; }
        public Nullable<bool> FACT_REG_CTR { get; set; }
        public string DERN_MONT_DISP_2 { get; set; }
        public string MIN_COMM_FACT { get; set; }
    }
}