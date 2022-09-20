using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace xfactor.Models
{
    public class NewDataCollection
    {
        public int a;
        public string b;
        public int c;
        public NewDataCollection(int a1, string b1)
        {
            this.a = a1;
            this.b = b1;
        }

        public NewDataCollection(int a1, string b1,int c1)
        {
            this.a = a1;
            this.b = b1;
            this.c = c1;
        }
    }
}