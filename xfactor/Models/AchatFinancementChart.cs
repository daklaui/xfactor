using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace xfactor.Models
{
    public class AchatFinancementChart
    {
        public Nullable<decimal> montant_financement { get; set; }
        public Nullable<decimal> montant_achat { get; set; }
        public int mois { get; set; }
    }
}