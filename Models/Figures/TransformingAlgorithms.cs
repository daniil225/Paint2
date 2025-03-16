using Formats;
using Paint2.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint2.Models.Figures
{
    public static class TransformingAlgorithms
    {
        public static Point ReflectionPoint(double a, double b, double c, Point point)
        {
            double factor = 2 * (a * point.X + b * point.Y + c) / (a * a + b * b);

            return new Point(point.X - a * factor, point.Y - b * factor);
        }

        public static double ReflectionAngle(Point ax1, Point ax2, double angle)
        {
            if (Math.Abs(ax1.X - ax2.X) < 1e-7)
                return -angle;

            double alpha = -180.0 / Math.PI * Math.Atan((ax2.Y - ax1.Y) / (ax2.X - ax1.X));

            return 2 * alpha - angle;
        }
    }
}
