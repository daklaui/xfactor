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
    
    public partial class T_EXTRAIT
    {
        public long ID_EXTRAIT { get; set; }
        public Nullable<int> REF_CTR_EXTRAIT { get; set; }
        public string LIB_EXTRAIT { get; set; }
        public Nullable<System.DateTime> DAT_OP_EXTRAIT { get; set; }
        public Nullable<System.DateTime> DAT_VAL_EXTRAIT { get; set; }
        public Nullable<decimal> DEBIT_EXTRAIT { get; set; }
        public Nullable<decimal> CREDIT_EXTRAIT { get; set; }
        public Nullable<decimal> ENCOURFACT_EXTRAIT { get; set; }
        public Nullable<decimal> TOTAL_FIN_EXTRAIT { get; set; }
        public Nullable<decimal> DISPONIBLE_EXTRAIT { get; set; }
        public Nullable<decimal> FDG_EXTRAIT { get; set; }
        public Nullable<decimal> AUTRE_EXTRAIT { get; set; }
    }
}
