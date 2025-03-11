using Avalonia.Media;
using Paint2.ViewModels.Interfaces;

namespace Paint2.ViewModels.Utils
{
    public class Renderer : IRenderInterface
    {
        public Geometry RenderLine(Point startPoint, Point endPoint)
        {
            Avalonia.Point aStartPoint = new(startPoint.x, startPoint.y);
            Avalonia.Point aEndPoint = new(endPoint.x, endPoint.y);
            LineGeometry lineGeometry = new() {StartPoint = aStartPoint, EndPoint = aEndPoint};
            return lineGeometry;
        }

        public Geometry RenderArc(Point startPoint, Point endPoint, double sizeX, double sizeY, bool isClosed)
        {
            throw new System.NotImplementedException();
        }

        public Geometry RenderPolygon(Point[] points)
        {
            throw new System.NotImplementedException();
        }

        public Geometry RenderEllipse(Point center, double radiusX, double radiusY, Transform? transformation)
        {
            Avalonia.Point aCenter = new(center.x, center.y);
            EllipseGeometry ellipseGeometry = new() { Center = aCenter, RadiusX = radiusX, RadiusY = radiusY };
            return ellipseGeometry;
        }

        public Geometry RenderCubicBezierCurve(Point point1, Point point2, Point point3, Point point4)
        {
            throw new System.NotImplementedException();
        }
    }
}