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
    
    public partial class T_Fichiers_Scan
    {
        public int Id_Scan { get; set; }
        public string Nom_Fichier_Scan { get; set; }
        public Nullable<System.DateTime> Date_Scan { get; set; }
        public string Adresse_Scan { get; set; }
        public string Affect_Scan { get; set; }
        public string Nom_Fichier_SansEXT { get; set; }
    }
}
