using Avalonia.Media;
using Paint2.ViewModels.Utils;
using Paint2.ViewModels.Interfaces;
using System.Collections.Generic;

namespace Formats
{
    public abstract class DocElement
    {
        public string? Name { get; set; }
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
        internal IEnumerable<IPathElement> Elements { get; set; }

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
        public IPathBuilder CubicBezierTo(Point controlPoint1, Point controlPoint2, Point dest)
        {
            _elements.Add(new PathCubicBezierTo() {
                dest = dest,
                controlPoint1 = controlPoint1,
                controlPoint2 = controlPoint2
            });

            return this;
        }
        public IPathBuilder ArcTo(
            double radiusX, double radiusY,
            double xAxisRotation, bool largeArcFlag,
            SweepDirection sweepDirection,
            Point dest
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
    
    public class Brush(
        Color stroke,
        Color fill,
        double strokeWidth,
        (double Length, double Gap)? dash = null
    ) {
        public Color Stroke = stroke;
        // Если фигура не может иметь цвета заливки, это поле игнорируется
        public Color Fill = fill;
        public double StrokeWidth = strokeWidth;
        // Length - длина каждого штриха
        // Gap - расстояние между штрихами
        // для сплошных линий рекомендуется использовать null вместо
        // нулевого значения Gap
        public (double Length, double Gap)? Dash = dash;
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
    public class Translate (double x, double y) : ITransform
    {
        public double X = x;
        public double Y = y;
    }
    public class Rotate (double angle, Point? pivot = null) : ITransform
    {
        // Угол вращения системы координат относительно
        //  Pivot по часовой стрелке в градусах.
        public double Angle = angle;
        // Для вращения вокруг начала координат
        //  использовать null значение.
        public Point? Pivot = pivot;
    }
}
