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
    
    public partial class Extrait_compte_Adhérent_Result
    {
        public string Acheteur { get; set; }
        public string Adhérent { get; set; }
        public string Ref_Acheteur { get; set; }
        public Nullable<decimal> Mont_TTC_Facture { get; set; }
        public Nullable<decimal> Mont_Total_Facture { get; set; }
        public Nullable<decimal> Mont_Ouvert_Facture { get; set; }
        public Nullable<decimal> Retenu_Facture { get; set; }
        public Nullable<System.DateTime> Date_Fature { get; set; }
        public string Bordereau { get; set; }
        public string Ref_Facture { get; set; }
        public string Num_Paiement { get; set; }
        public Nullable<decimal> MONT_Paiement { get; set; }
        public Nullable<System.DateTime> Echeance_Paiement { get; set; }
        public Nullable<decimal> total { get; set; }
        public string Mode_Paiement { get; set; }
        public Nullable<decimal> Montant_Lettré { get; set; }
        public Nullable<System.DateTime> Date_Reconciliation { get; set; }
        public decimal Trop_Perçu { get; set; }
        public decimal Deduction { get; set; }
        public System.DateTime DAT_LET { get; set; }
        public int ID_DET_BORD_LET { get; set; }
        public int ID_ENC_LET { get; set; }
    }
}
