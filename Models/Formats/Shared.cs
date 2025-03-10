using Avalonia.Media;
using Interfaces;
using System.Collections.Generic;

namespace Formats
{
    public abstract class DocElement
    {
        public string? Name { get; }
        public IEnumerable<ITransform> Transforms { get; set; } = [];
    }

    public class DocGroup : DocElement {}

    public class DocRect(Point position, double width, double height) : DocElement
    {
        // Координата левого верхнего угла прямоугольника
        public Point Position { get; set; } = position;
        public double Width { get; set; } = width;
        public double Height { get; set; } = height;
    }

    public class DocCircle(Point center, double radius) : DocElement
    {
        public Point Center { get; set; } = center;
        public double Radius { get; set; } = radius;
    }

    public class DocLine(Point start, Point end) : DocElement
    {
        public Point Start { get; set; } = start;
        public Point End { get; set; } = end;
    }

    public class DocPolyline(IEnumerable<Point> points) : DocElement
    {
        public IEnumerable<Point> Points { get; set; } = points;
    }
    
    public class DocPolygon(IEnumerable<Point> points) : DocElement
    {
        public IEnumerable<Point> Points { get; set; } = points;
    }

    public class DocPath : DocElement
    {
        IEnumerable<IPathElement> Elements { get; set; }

        internal DocPath(IEnumerable<IPathElement> elements)
        {
            Elements = elements;
        }
    }
    
    public class PathBuilder : IPathBuilder
    {
        List<IPathElement> _elements = [];

        public PathBuilder() {}
        
        public IPathBuilder MoveTo(Point dest)
        {
            _elements.Add(new PathMoveTo() { dest = dest });
            
            return this;
        }
        public IPathBuilder LineTo(Point dest)
        {
            _elements.Add(new PathLineTo() { dest = dest });
            
            return this;
        }
        public IPathBuilder CubicBezierTo(Point dest, Point controlPoint1, Point controlPoint2)
        {
            _elements.Add(new PathCubicBezierTo() {
                dest = dest,
                controlPoint1 = controlPoint1,
                controlPoint2 = controlPoint2
            });

            return this;
        }
        public IPathBuilder ArcTo(
            Point dest,
            double radiusX, double radiusY,
            double xAxisRotation, bool largeArcFlag,
            SweepDirection sweepDirection
        ) {
            _elements.Add(new PathArcTo() {
                dest = dest,
                radiusX = radiusX,
                radiusY = radiusY,
                xAxisRotation = xAxisRotation,
                largeArcFlag = largeArcFlag,
                sweepDirection = sweepDirection
            });
            
            return this;
        }
        public IPathBuilder Close()
        {
            _elements.Add(new PathClose());
            
            return this;
        }
        public DocPath Build()
        {
            return new DocPath(_elements);
        }
    }
    
    public struct Brush
    {
        public Color Stroke;
        // Если фигура не может иметь цвета заливки, это поле игнорируется
        public Color Fill;
        public double StrokeWidth;
        // Length - длина каждого штриха
        // Gap - расстояние между штрихами
        // для сплошных линий рекомендуется использовать null вместо
        // нулевого значения Gap
        public (double Length, double Gap)? Dash;
    }

    interface IPathElement;
    struct PathMoveTo : IPathElement
    {
        public Point dest;
    }
    struct PathLineTo : IPathElement
    {
        public Point dest;
    }
    struct PathArcTo : IPathElement
    {
        public double radiusX;
        public double radiusY;
        public double xAxisRotation;
        public bool largeArcFlag;
        public SweepDirection sweepDirection;
        public Point dest;
    }
    struct PathCubicBezierTo : IPathElement
    {
        public Point dest;
        public Point controlPoint1;
        public Point controlPoint2;
    }
    struct PathClose : IPathElement {}

    public interface ITransform;
    public struct Translate : ITransform
    {
        public double X;
        public double Y;
    }
    public struct Rotate : ITransform
    {
        // Угол вращения системы координат относительно
        //  Pivot по часовой стрелке в градусах.
        public double Angle;
        // Для вращения вокруг начала координат
        //  использовать null значение.
        public Point? Pivot;
    }
}
