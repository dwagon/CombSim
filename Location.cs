using System;
using System.Runtime.Remoting.Proxies;

namespace CombSim
{
    public class Location
    {
        public int x { get; private set; }
        public int y{ get; private set; }

        public Location(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return "(" + x + "," + y + ")";
        }
        
        public float DistanceBetween(Location other)
        {
            float distance = (float)Math.Sqrt(Math.Pow(x - other.x, 2f) + Math.Pow(y - other.y, 2f));
            return distance;
        }
    }
}