using System.Drawing;

namespace Pastat.Reporting.Diagram
{
    public struct PointL
    {
        public long X { get; set; }
        public long Y { get; set; }

        public PointL(long x, long y)
        {
            X = x;
            Y = y;
        }

        public static explicit operator Point(PointL p)
        {
            return new Point((int)p.X, (int)p.Y);
        } 
    }
}
