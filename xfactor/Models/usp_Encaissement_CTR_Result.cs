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
    
    public partial class usp_Encaissement_CTR_Result
    {
        public short ID_ENC { get; set; }
        public int REF_CTR_ENC { get; set; }
        public Nullable<int> REF_ADH_ENC { get; set; }
        public Nullable<int> REF_ACH_ENC { get; set; }
        public Nullable<decimal> MONT_ENC { get; set; }
        public string DEVISE_ENC { get; set; }
        public Nullable<System.DateTime> DAT_RECEP_ENC { get; set; }
        public Nullable<System.DateTime> DAT_VAL_ENC { get; set; }
        public string TYP_ENC { get; set; }
        public Nullable<bool> VALIDE_ENC { get; set; }
        public string REF_ENC { get; set; }
    }
}
