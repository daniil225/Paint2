using Avalonia.Threading;
using Formats;
using Paint2.ViewModels;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using System;
using System.Composition;

namespace Paint2.Models.Figures
{
    public class Triangle : PathFigure
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Name), nameof(Triangle))]
        private class TriangleCreator : IFigureCreator
        {
            public IFigure Create(Group parentGroup, Point coordinates)
            {
                return new Triangle(parentGroup, coordinates);
            }
        }

        private Triangle(Group parentGroup, Point coordinates) : base(parentGroup, coordinates)
        {
            Name = "Triangle";

            double sqrt3 = Math.Sqrt(3);
            double lengthSide = 30.0;

            pathElements.Add(new PathMoveTo() { dest = new Point(coordinates.X, coordinates.Y + lengthSide / sqrt3) });
            pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X + lengthSide / 2.0, coordinates.Y - lengthSide / (2 * sqrt3)) });
            pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X - lengthSide / 2.0, coordinates.Y - lengthSide / (2 * sqrt3)) });
            pathElements.Add(new PathClose());
            
//            Dispatcher.UIThread.Invoke(() => Geometry = Renderer.RenderPathElements(pathElements));
        }
    }
}
