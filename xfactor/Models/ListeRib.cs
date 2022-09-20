using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace xfactor.Models
{
    public class ListeRib
    {
        public string rib;
        public string banque;
        public string agence;
        public bool etat;


        public ListeRib(string rib, string banque, string agence, bool etat)
        {
            this.rib = rib;
            this.banque = banque;
            this.agence = agence;
            this.etat = etat;
        }


    }
}