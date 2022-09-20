using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace xfactor.Models
{
    public class ListeDesContrat
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
        public Nullable<decimal> CA_CTR { get; set; }
        public Nullable<decimal> CA_EXP_CTR { get; set; }
        public Nullable<decimal> CA_IMP_CTR { get; set; }
        public Nullable<decimal> LIM_FIN_CTR { get; set; }
        public string DEVISE_CTR { get; set; }
        public Nullable<short> NB_ACH_PREVU_CTR { get; set; }
        public Nullable<short> NB_FACT_PREVU_CTR { get; set; }
        public Nullable<short> NB_AVOIRS_PREVU_CTR { get; set; }
        public Nullable<short> NB_REMISES_PREVU_CTR { get; set; }
        public Nullable<short> DELAI_MOYEN_REG_CTR { get; set; }
        public Nullable<short> DELAI_MAX_REG_CTR { get; set; }
        public Nullable<bool> FACT_REG_CTR { get; set; }
        public Nullable<decimal> DERN_MONT_DISP_2 { get; set; }
        public Nullable<decimal> MIN_COMM_FACT { get; set; }
        public int REF_IND { get; set; }
        public string GENRE_IND { get; set; }
        public string TYP_DOC_ID_IND { get; set; }
        public string NUM_DOC_ID_IND { get; set; }
        public string LIEU_DOC_ID_IND { get; set; }
        public Nullable<System.DateTime> DATE_DOC_ID_IND { get; set; }
        public string COD_TVA_IND { get; set; }
        public string NOM_IND { get; set; }
        public string PRE_IND { get; set; }
        public Nullable<System.DateTime> DAT_NAISS_CREAT { get; set; }
        public Nullable<bool> GRP_IND { get; set; }
        public Nullable<decimal> LIM_CRED_GLO_IND { get; set; }
        public Nullable<decimal> LIM_FIN_GLO_IND { get; set; }
        public string INFO_IND { get; set; }
        public Nullable<System.DateTime> DAT_INFO_IND { get; set; }
        public Nullable<int> ID_NLDP { get; set; }
        public string COD_SCLAS { get; set; }
        public string ABRV_IND { get; set; }
        public byte[] LOGO_IND { get; set; }
        public byte[] PHOTO_IND { get; set; }
        public string MF_IND { get; set; }
        public string RC_IND { get; set; }
        public Nullable<bool> EXO_TVA { get; set; }
        public Nullable<System.DateTime> DAT_DEB_EXO { get; set; }
        public Nullable<System.DateTime> DAT_FIN_EXO { get; set; }
        public string TEL_IND { get; set; }
        public string FAX_IND { get; set; }
        public string EMAIL_IND { get; set; }
        public string REF_ADH_IND { get; set; }
        public string FROM_JUR_IND { get; set; }
        public Nullable<bool> EXO_IND { get; set; }
        public string ADR_IND { get; set; }
        public string CP_IND { get; set; }
        public string REF_ACH_IND { get; set; }
        public Nullable<int> ID_GROUPE { get; set; }
    }
}