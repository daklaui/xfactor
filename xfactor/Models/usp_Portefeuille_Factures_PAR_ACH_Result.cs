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
    
    public partial class usp_Portefeuille_Factures_PAR_ACH_Result
    {
        public int REF_CTR_ENC { get; set; }
        public Nullable<int> REF_ADH_ENC { get; set; }
        public Nullable<int> REF_IND_DET_BORD { get; set; }
        public int ID_ENC_LET { get; set; }
        public System.DateTime DAT_LET { get; set; }
        public Nullable<decimal> MONT_ENC { get; set; }
        public Nullable<decimal> MONT_OUV_DET_BORD { get; set; }
        public Nullable<System.DateTime> DAT_RECONCIL { get; set; }
        public Nullable<bool> VALIDE_LET { get; set; }
        public Nullable<bool> VALIDE_RECONCIL { get; set; }
    }
}