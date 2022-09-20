using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace xfactor.Models
{
    public class Bord_Ch
    {
        public string ref_enc { get; set; }
        public string type { get; set; }
        public string ach { get; set; }
        public string mnt { get; set; }
        public string dt { get; set; }
        public string ref_ctr_enc { get; set; }
        public int id_enc { get; set; }
        public string  id_banque { get; set; }
    }
}