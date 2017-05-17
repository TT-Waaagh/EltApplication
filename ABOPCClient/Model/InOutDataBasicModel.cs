using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EltApplication
{
    class InOutDataBasicModel
    {
        private String id;

        public String Id
        {
            get { return id; }
            set { id = value; }
        }
        private String data;

        public String Data
        {
            get { return data; }
            set { data = value; }
        }
        private String datein;

        public String Datein
        {
            get { return datein; }
            set { datein = value; }
        }
        private String dateout;

        public String Dateout
        {
            get { return dateout; }
            set { dateout = value; }
        }
        private String status;

        public String Status
        {
            get { return status; }
            set { status = value; }
        }
        private String erpid;

        public String Erpid
        {
            get { return erpid; }
            set { erpid = value; }
        }
    }
}
