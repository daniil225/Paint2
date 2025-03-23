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
            double alpha = 180.0 / Math.PI * Math.Atan2((ax2.Y - ax1.Y), (ax2.X - ax1.X));

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
                pathElements[i] = pathElements[i] switch
                {
                    PathMoveTo pathMove => new PathMoveTo() { dest = RotatePoint(pathMove.dest, Center, cosAngle, sinAngle) },
                    PathLineTo pathLine => new PathLineTo() { dest = RotatePoint(pathLine.dest, Center, cosAngle, sinAngle) },
                    PathArcTo pathArc => new PathArcTo()
                    {
                        radiusX = pathArc.radiusX,
                        radiusY = pathArc.radiusY,
                        xAxisRotation = pathArc.xAxisRotation + angle,
                        largeArcFlag = pathArc.largeArcFlag,
                        sweepDirection = pathArc.sweepDirection,
                        dest = RotatePoint(pathArc.dest, Center, cosAngle, sinAngle)
                    },
                    PathCubicBezierTo pathCubicBezier => new PathCubicBezierTo()
                    {
                        dest = RotatePoint(pathCubicBezier.dest, Center, cosAngle, sinAngle),
                        controlPoint1 = RotatePoint(pathCubicBezier.controlPoint1, Center, cosAngle, sinAngle),
                        controlPoint2 = RotatePoint(pathCubicBezier.controlPoint2, Center, cosAngle, sinAngle)
                    },
                    _ => pathElements[i]
                };
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
