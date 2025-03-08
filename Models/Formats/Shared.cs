using Avalonia.Media;
using Interfaces;
using System.Collections.Generic;

namespace Formats
{
    public class Path
    {
        // доступен только для форматов
        internal List<PathElement> Elements { get; private set; }

        internal Path(IEnumerable<PathElement> elements)
        {
            Elements = new List<PathElement>(elements);
        }
    }
    
    public class PathBuilder : IPathBuilder
    {
        private List<PathElement> _elements = [];

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
        public Formats.Path Build()
        {
            return new Formats.Path(_elements);
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

    interface PathElement;
    struct PathMoveTo : PathElement
    {
        public Point dest;
    }
    struct PathLineTo : PathElement
    {
        public Point dest;
    }
    struct PathArcTo : PathElement
    {
        public double radiusX;
        public double radiusY;
        public double xAxisRotation;
        public bool largeArcFlag;
        public SweepDirection sweepDirection;
        public Point dest;
    }
    struct PathCubicBezierTo : PathElement
    {
        public Point dest;
        public Point controlPoint1;
        public Point controlPoint2;
    }
    struct PathClose : PathElement {}
}
