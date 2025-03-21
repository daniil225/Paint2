using Formats;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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

        public static Point RotatePoint(Point point, Point center, double cosAngle, double sinAngle)
        {
            double rotatedX = (point.X - center.X) * cosAngle - (point.Y - center.Y) * sinAngle + center.X;
            double rotatedY = (point.X - center.X) * sinAngle + (point.Y - center.Y) * cosAngle + center.Y;

            return new Point(rotatedX, rotatedY);
        }

        public static void RotateElements(IList<IPathElement> pathElements, double angle, Point Center)
        {
            double radians = angle * Math.PI / 180;
            double cosAngle = Math.Cos(radians);
            double sinAngle = Math.Sin(radians);

            for (int i = 0; i < pathElements.Count; i++)
            {
                if (pathElements[i] is PathMoveTo pathMove)
                {
                    pathElements[i] = new PathMoveTo() { dest = RotatePoint(pathMove.dest, Center, cosAngle, sinAngle) };
                }
                else if (pathElements[i] is PathLineTo pathLine)
                {
                    pathElements[i] = new PathLineTo() { dest = RotatePoint(pathLine.dest, Center, cosAngle, sinAngle) };
                }
                else if (pathElements[i] is PathArcTo pathArc)
                {
                    pathArc.dest = RotatePoint(pathArc.dest, Center, cosAngle, sinAngle);
                    pathArc.xAxisRotation += angle;
                    pathElements[i] = pathArc;
                }
                else if (pathElements[i] is PathCubicBezierTo pathCubicBezier)
                {
                    pathElements[i] = new PathCubicBezierTo()
                    {
                        dest = RotatePoint(pathCubicBezier.dest, Center, cosAngle, sinAngle),
                        controlPoint1 = RotatePoint(pathCubicBezier.controlPoint1, Center, cosAngle, sinAngle),
                        controlPoint2 = RotatePoint(pathCubicBezier.controlPoint2, Center, cosAngle, sinAngle)
                    };
                }
            }
        }

        public static Point ScalePoint(Point point, Point center, double sx, double sy)
        {
            double scaledX = center.X + (point.X - center.X) * sx;
            double scaledY = center.Y + (point.Y - center.Y) * sy;

            return new Point(scaledX, scaledY);
        }
    }
}
