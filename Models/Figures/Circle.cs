using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Composition;
using Paint2.ViewModels.Utils;
using Paint2.ViewModels.Interfaces;
using ReactiveUI.Fody.Helpers;

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
        public Point Coordinates { get; private set; }

        public float Angle { get; private set; }
        
        [Reactive] public Geometry Geometry { get; set; }
        public bool IsActive { get; set; }
        public bool IsMirrored { get; set; }

        private string name;
        public double Radius { get; set; }

        public Circle(Point c, double r)
        {
            Coordinates = c;
            Radius = r;
            name = "Circle";
            IsActive = true;
            IsMirrored = false;
        }

        public void Render(IRenderInterface toDraw)
        {
            // пока передаю null, может быть потом тут будет расчет угла поворота
            Geometry = toDraw.RenderEllipse(Coordinates, Radius, Radius, null);
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

        public void Export(IExportSnapshot snapshot)
        {
            throw new NotImplementedException();
        }

        public void Mirror(Point ax1, Point ax2)
        {
            throw new NotImplementedException();
        }
    }
}
