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
    
    public partial class Les_Factures_En_retard_Result
    {
        public string NomAdh { get; set; }
        public string NomAch { get; set; }
        public string TYP_DET_BORD { get; set; }
        public Nullable<short> ECH_DET_BORD { get; set; }
        public Nullable<System.DateTime> DAT_DET_BORD { get; set; }
        public Nullable<decimal> MONT_TTC_DET_BORD { get; set; }
        public Nullable<decimal> MONT_OUV_DET_BORD { get; set; }
        public string REF_DOCUMENT_DET_BORD { get; set; }
        public Nullable<System.DateTime> echeance { get; set; }
        public Nullable<int> Retard { get; set; }
        public int REF_CTR_DET_BORD { get; set; }
    }
}
