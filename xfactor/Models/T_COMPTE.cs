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
    
    public partial class T_COMPTE
    {
        public int ID_COMPT { get; set; }
        public int ID_CIR { get; set; }
        public string DEVISE_ACH_CPT { get; set; }
        public Nullable<decimal> LIM_FIN_ACH_ADH { get; set; }
        public Nullable<short> DELAI_PAI_CPT { get; set; }
        public string MODE_REG_CPT { get; set; }
        public Nullable<decimal> TOT_FACT_CPT { get; set; }
        public Nullable<decimal> FDG_REL_FACT_CPT { get; set; }
        public Nullable<decimal> DEPASS_LIM_CPT { get; set; }
        public Nullable<decimal> TOT_LIT_CPT { get; set; }
        public Nullable<decimal> TOT_IMP_CPT { get; set; }
        public Nullable<System.DateTime> DAT_CPT { get; set; }
    }
}
