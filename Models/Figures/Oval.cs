using Avalonia.Media;
using Avalonia.Threading;
using System.Composition;
using Paint2.ViewModels;
using Paint2.ViewModels.Utils;
using Paint2.ViewModels.Interfaces;
using Formats;
using System.Collections.Generic;
using System.Linq;

namespace Paint2.Models.Figures
{
    public partial class PathFigure
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Type), "Oval")]
        class OvalCreator : IFigureCreator
        {
            public IFigure Create(Group parentGroup, ICollection<Point> coordinatePoints)
            {
                Point coordinates = coordinatePoints.ElementAt(0);
                PathFigure newOval = new(parentGroup, coordinates);
                newOval.Name = "Oval";

                double Rx = 30.0;
                double Ry = 20.0;

                newOval.pathElements.Add(new PathMoveTo() { dest = new Point(coordinates.X + Rx, coordinates.Y) });
                newOval.pathElements.Add(new PathArcTo()
                {
                    radiusX = Rx,
                    radiusY = Ry,
                    xAxisRotation = 0,
                    largeArcFlag = true,
                    sweepDirection = SweepDirection.Clockwise,
                    dest = new Point(coordinates.X - Rx, coordinates.Y)
                });
                newOval.pathElements.Add(new PathArcTo()
                {
                    radiusX = Rx,
                    radiusY = Ry,
                    xAxisRotation = 0,
                    largeArcFlag = true,
                    sweepDirection = SweepDirection.Clockwise,
                    dest = new Point(coordinates.X + Rx, coordinates.Y)
                });
                newOval.pathElements.Add(new PathClose());
                newOval.OnGeometryChanged();

                return newOval;
            }
        }
    }
}
