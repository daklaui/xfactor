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
    
    public partial class TJ_LETTRAGE
    {
        public int ID_DET_BORD_LET { get; set; }
        public int ID_ENC_LET { get; set; }
        public System.DateTime DAT_LET { get; set; }
        public Nullable<decimal> MONT_TTC_LET { get; set; }
        public Nullable<System.DateTime> DAT_RECONCIL { get; set; }
        public Nullable<bool> VALIDE_LET { get; set; }
        public Nullable<bool> VALIDE_RECONCIL { get; set; }
    }
}
