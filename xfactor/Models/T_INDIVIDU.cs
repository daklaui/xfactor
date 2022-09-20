//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace xfactor.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class T_INDIVIDU
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_INDIVIDU()
        {
            this.T_ADRESSE = new HashSet<T_ADRESSE>();
            this.TJ_CIR = new HashSet<TJ_CIR>();
        }
    
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
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_ADRESSE> T_ADRESSE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TJ_CIR> TJ_CIR { get; set; }
    }
}