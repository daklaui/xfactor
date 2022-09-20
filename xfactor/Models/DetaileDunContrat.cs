using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//Dev 27/02/2020 par moataz a 5:01PM
namespace xfactor.Models
{
    public class DetaileDunContrat
    {
        public DetaileDunContrat(T_INDIVIDU ind, All_Ecran_Financements_Result finacnement,T_CONTRAT contrat)
        {
            this.ind = ind;
            this.finacnement = finacnement;
            this.contrat = contrat;
        }

        public virtual T_INDIVIDU ind { get; set; }
        public virtual All_Ecran_Financements_Result finacnement {get;set;}
        public virtual T_CONTRAT contrat { get; set; }

    }
}