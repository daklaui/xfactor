using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace xfactor.Models
{
    public class ImpayeCourbe
    {
        public Nullable<decimal> montant_impaye { get; set; }
        public int mois { get; set; }
    }
}