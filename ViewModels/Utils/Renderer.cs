using Avalonia.Media;
using Paint2.ViewModels.Interfaces;
using Point = Paint2.ViewModels.Utils.Point;

namespace Paint2.ViewModels.Utils
{
    public class Renderer : IRenderInterface
    {
        public void RenderLine(Geometry geometry, Point startPoint, Point endPoint)
        {
            throw new System.NotImplementedException();
        }

        public void RenderArc(Geometry geometry, Point startPoint, Point endPoint, double sizeX, double sizeY, bool isClosed)
        {
            throw new System.NotImplementedException();
        }

        public void RenderPolygon(Geometry geometry, Point[] points)
        {
            throw new System.NotImplementedException();
        }

        public void RenderEllipse(Geometry geometry, Point center, double radiusX, double radiusY, Transform? transformation)
        {
            throw new System.NotImplementedException();
        }

        public void RenderCubicBezierCurve(Geometry geometry, Point point1, Point point2, Point point3, Point point4)
        {
            throw new System.NotImplementedException();
        }
    }
}