using System;
using System.Collections.Generic;
using System.Text;

namespace RandoProbabilityCalculator
{
    public class Area
    {
        public int Req;
        public List<Area> SubAreas;
        public int[] Checks;

        public Area()
        {
            Req = 0;
            SubAreas = new List<Area>();
            Checks = new int[0];
        }
    }
}
