using Avalonia.Media;
using System.Composition;
using Paint2.ViewModels;
using Paint2.ViewModels.Utils;
using Paint2.ViewModels.Interfaces;
using Formats;

namespace Paint2.Models.Figures
{
    public class Oval : PathFigure
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Name), nameof(Oval))]
        class OvalCreator : IFigureCreator
        {
            public IFigure Create(Group parentGroup, Point coordinates)
            {
                return new Oval(parentGroup, coordinates);
            }
        }
        Oval(Group parentGroup, Point coordinates) : base(parentGroup, coordinates)
        {
            Name = "Oval";

            double Rx = 30.0;
            double Ry = 20.0;

            pathElements.Add(new PathMoveTo() { dest = new Point(coordinates.X + Rx, coordinates.Y) });
            pathElements.Add(new PathArcTo()
            {
                radiusX = Rx,
                radiusY = Ry,
                xAxisRotation = 0,
                largeArcFlag = true,
                sweepDirection = SweepDirection.Clockwise,
                dest = new Point(coordinates.X + Rx, coordinates.Y)
            });
            pathElements.Add(new PathClose());
        }
    }
}
