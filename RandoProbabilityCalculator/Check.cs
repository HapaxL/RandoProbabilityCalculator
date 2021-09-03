using System;
using System.Collections.Generic;
using System.Text;

namespace RandoProbabilityCalculator
{
    public struct Check
    {
        public int ID;
        public bool HasKey;
        public int Req;

        public Check(int id, int req)
        {
            ID = id;
            HasKey = false;
            Req = req;
        }
    }
}
