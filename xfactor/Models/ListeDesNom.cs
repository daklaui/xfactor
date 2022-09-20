using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace xfactor.Models
{
    public class ListeDesNom
    {
        XFactor_PRODEntities1 db = new XFactor_PRODEntities1();
        public List<NewDataCollection> Liste_Des_Adhérents()
        {
            var ListAdh = (from q in db.T_INDIVIDU
                           join q2 in db.TJ_CIR
                           on q.REF_IND equals q2.REF_IND_CIR
                           where (q2.ID_ROLE_CIR == "ADH")
                           select new { q.PRE_IND, q2.REF_IND_CIR, q2.REF_CTR_CIR }).Distinct();

            List<NewDataCollection> Adherents = new List<NewDataCollection>();
            foreach (var item in ListAdh)
            {
                Adherents.Add(new NewDataCollection(item.REF_IND_CIR,item.PRE_IND,item.REF_CTR_CIR));
            }

            return Adherents;
        }
    }
}