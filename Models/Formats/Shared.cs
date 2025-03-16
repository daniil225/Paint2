using Avalonia.Media;
using Paint2.ViewModels.Utils;
using Paint2.ViewModels.Interfaces;
using System.Collections.Generic;
using System.Linq;

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
        internal bool IsClosed { get; private set; }

        internal DocPath(IEnumerable<IPathElement> elements, bool isClosed)
        {
            Elements = elements;
            IsClosed = isClosed;
        }
    }
    
    public class PathBuilder : IPathBuilder
    {
        List<IPathElement> _elements = [];
        bool _isClosed = false;

        public PathBuilder() {}
        public PathBuilder(List<IPathElement> elements)
        {
            _elements = elements;
            _isClosed = elements.OfType<PathClose>().Any();
        }

        public IPathBuilder MoveTo(Point dest)
        {
            _elements.Add(new PathMoveTo() { dest = new(dest) });
            
            return this;
        }
        public IPathBuilder LineTo(Point dest)
        {
            _elements.Add(new PathLineTo() { dest = new(dest) });
            
            return this;
        }
        public IPathBuilder CubicBezierTo(Point controlPoint1, Point controlPoint2, Point dest)
        {
            _elements.Add(new PathCubicBezierTo()
            {
                dest = new(dest),
                controlPoint1 = new(controlPoint1),
                controlPoint2 = new(controlPoint2)
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
                dest = new(dest),
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
            _isClosed = true;

            return this;
        }
        public DocPath Build()
        {
            return new DocPath(_elements, _isClosed);
        }
    }
    
    public class Brush
    {
        public Color Stroke;
        // Это поле игнорируется при отрисовке Polyline, Line,
        // а также Path, если в нём не встречается сегмент PathClose
        public Color Fill;
        public double StrokeWidth;
        // Length - длина каждого штриха
        // Gap - расстояние между штрихами
        // для сплошных линий рекомендуется использовать null вместо
        // нулевого значения Gap
        public (double Length, double Gap)? Dash;

        public Brush(Color stroke,
                     Color fill,
                     double strokeWidth,
                     (double Length, double Gap)? dash = null)
        {
            Stroke = stroke;
            Fill = fill;
            StrokeWidth = strokeWidth;
            Dash = dash;
        }
        public Brush(Brush other)
        {
            if(other != null)
            {
                Stroke = other.Stroke;
                Fill = other.Fill;
                StrokeWidth = other.StrokeWidth;
                Dash = other.Dash;
            }
        }
    }

    public interface IPathElement;
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
