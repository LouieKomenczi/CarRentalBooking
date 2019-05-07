using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA3
{
    public partial class Car
    {
        public override string ToString()
        {
            //return  "Car ID: "+Id + "\n"+
            //        "Make: "+Make + "\n" + 
            //        "Model: "+ Model;
            return Make + "-" + Model;
        }

    }
}
