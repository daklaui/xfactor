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
    
    public partial class usp_Rapport_Factures_En_Litige_Version2___Result
    {
        public int ID_LITIGE { get; set; }
        public int REF_IND { get; set; }
        public string NOM_IND { get; set; }
        public string TYP_DET_BORD { get; set; }
        public string ID_DET_BORD { get; set; }
        public Nullable<decimal> MONT_OUV_DET_BORD { get; set; }
        public Nullable<System.DateTime> ECH_LIT { get; set; }
        public Nullable<decimal> MONT_LT { get; set; }
    }
}
