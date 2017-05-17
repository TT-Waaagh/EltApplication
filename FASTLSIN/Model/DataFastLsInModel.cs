using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EltApplication
{
    class DataFastLsInModel
    {
        private String mpn;

        public String pn
        {
            get { return mpn; }
            set { mpn = value; }
        }
        private String mtn;

        public String tn
        {
            get { return mtn; }
            set { mtn = value; }
        }
        private String msts;

        public String sts
        {
            get { return msts; }
            set { msts = value; }
        }
    }
}
