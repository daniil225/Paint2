using Avalonia.Threading;
using Formats;
using Paint2.ViewModels;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using System.Composition;

namespace Paint2.Models.Figures
{
    public class Trapezoid : PathFigure
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Name), nameof(Trapezoid))]
        private class TrapezoidCreator : IFigureCreator
        {
            public IFigure Create(Group parentGroup, Point coordinates)
            {
                return new Trapezoid(parentGroup, coordinates);
            }
        }

        private Trapezoid(Group parentGroup, Point coordinates) : base(parentGroup, coordinates)
        {
            Name = "Trapezoid";

            double lowerBase = 40.0;
            double upperBase = 25.0;
            double height = 20.0;

            pathElements.Add(new PathMoveTo() { dest = new Point(coordinates.X - lowerBase / 2.0, coordinates.Y + height / 2.0) });
            pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X + lowerBase / 2.0, coordinates.Y + height / 2.0) });
            pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X + upperBase / 2.0, coordinates.Y - height / 2.0) });
            pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X - upperBase / 2.0, coordinates.Y - height / 2.0) });
            pathElements.Add(new PathClose());
            
            Dispatcher.UIThread.Invoke(() => Geometry = Renderer.RenderPathElements(pathElements));
        }
    }
}
