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
    
    public partial class usp_SituationAdh_ACH_ALL_Result
    {
        public string Nom_ACH { get; set; }
        public Nullable<decimal> Encours_Factures_Ouvertes { get; set; }
        public Nullable<decimal> Somme_Des_Avoirs { get; set; }
        public Nullable<decimal> Limite_Totale_De_Financement { get; set; }
        public Nullable<decimal> Limite_Totale_De_Crédit { get; set; }
        public Nullable<decimal> Encours_Factures_Ouvertes_Echues { get; set; }
        public Nullable<decimal> Encours_Factures_Ouvertes_Non_Echues { get; set; }
        public Nullable<decimal> Encours_Factures { get; set; }
        public Nullable<decimal> Total_Litiges_Encours { get; set; }
        public Nullable<decimal> Total_Paiement_Direct { get; set; }
    }
}
