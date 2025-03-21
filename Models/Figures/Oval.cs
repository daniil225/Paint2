using Avalonia.Media;
using Avalonia.Threading;
using System.Composition;
using Paint2.ViewModels;
using Paint2.ViewModels.Utils;
using Paint2.ViewModels.Interfaces;
using Formats;

namespace Paint2.Models.Figures
{
    public partial class PathFigure
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Name), "Oval")]
        class OvalCreator : IFigureCreator
        {
            public IFigure Create(Group parentGroup, Point[] coordinates)
            {
                PathFigure newOval = new(parentGroup, coordinates[0]);
                newOval.Name = "Oval";

                double Rx = 30.0;
                double Ry = 20.0;

                newOval.pathElements.Add(new PathMoveTo() { dest = new Point(coordinates[0].X + Rx, coordinates[0].Y) });
                newOval.pathElements.Add(new PathArcTo()
                {
                    radiusX = Rx,
                    radiusY = Ry,
                    xAxisRotation = 0,
                    largeArcFlag = true,
                    sweepDirection = SweepDirection.Clockwise,
                    dest = new Point(coordinates[0].X - Rx, coordinates[0].Y)
                });
                newOval.pathElements.Add(new PathArcTo()
                {
                    radiusX = Rx,
                    radiusY = Ry,
                    xAxisRotation = 0,
                    largeArcFlag = true,
                    sweepDirection = SweepDirection.Clockwise,
                    dest = new Point(coordinates[0].X + Rx, coordinates[0].Y)
                });
                newOval.pathElements.Add(new PathClose());
                newOval.OnGeometryChanged();

                return newOval;
            }
        }
    }
}
