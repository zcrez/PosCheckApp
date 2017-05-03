using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PosCheckApp
{
    class PosPT
    {
        public string ID;
        public string photoID;
        public double lat;
        public double lng;
        public double alt;
        public double pitch;
        public double head;
        public double roll;
        public bool isValid;

        public PosPT()
        {
            this.isValid = false;
        }
    }
}
