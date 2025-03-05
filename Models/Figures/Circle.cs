using Interfaces;
using System;
using System.Collections.Generic;
using System.Composition;

namespace Paint2.Models.Figures
{
    [Export(typeof(IFigureCreator))]
    [ExportMetadata(nameof(FigureMetadata.Name), nameof(Circle))]
    class CircleCreator : IFigureCreator
    {
        public IReadOnlyCollection<string> PointParametersNames => ["Center"];

        public IReadOnlyCollection<string> DoubleParametersNames => ["Radius"];

        public IFigure Create(IDictionary<string, double> doubleParams, IDictionary<string, Point> pointParams)
        {
            return new Circle(pointParams["Center"], doubleParams["Radius"]);
        }
    }
    public class Circle : IFigure
    {
        public string Name
        {
            get => name;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    name = value;
            }
        }
        private Point Center { get; set; }
        private double Radius { get; set; }
        private string name;

        public Circle(Point c, double r)
        {
            Center = c;
            Radius = r;
            name = "Circle";
        }

        public void Render(IRenderInterface toDraw)
        {
            throw new NotImplementedException();
        }

        public IFigure Intersect(IFigure other)
        {
            throw new NotImplementedException();
        }

        public bool IsInternal(Point p, double eps)
        {
            throw new NotImplementedException();
        }

        public void Move(Point vector)
        {
            throw new NotImplementedException();
        }

        public void Reflection(Point ax1, Point ax2)
        {
            throw new NotImplementedException();
        }

        public void Rotate(Point Center, double angle)
        {
            throw new NotImplementedException();
        }

        public void Scale(double x, double y)
        {
            throw new NotImplementedException();
        }

        public void Scale(Point Center, double rad)
        {
            throw new NotImplementedException();
        }

        public void SetParameters(IDictionary<string, double> doubleParams, IDictionary<string, Point> pointParams)
        {
            throw new NotImplementedException();
        }

        public IFigure Subtract(IFigure other)
        {
            throw new NotImplementedException();
        }

        public IFigure Union(IFigure other)
        {
            throw new NotImplementedException();
        }
    }
}
