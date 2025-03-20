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
        [ExportMetadata(nameof(FigureMetadata.Name), "Circle")]
        class CircleCreator : IFigureCreator
        {
            public IFigure Create(Group parentGroup, Point coordinates)
            {
                PathFigure newCircle = new(parentGroup, coordinates);
                newCircle.Name = "Circle";

                double R = 25.0;

                newCircle.pathElements.Add(new PathMoveTo() { dest = new Point(coordinates.X + R, coordinates.Y) });
                newCircle.pathElements.Add(new PathArcTo()
                {
                    radiusX = R,
                    radiusY = R,
                    xAxisRotation = 0,
                    largeArcFlag = true,
                    sweepDirection = SweepDirection.Clockwise,
                    dest = new Point(coordinates.X - R, coordinates.Y)
                });
                newCircle.pathElements.Add(new PathArcTo()
                {
                    radiusX = R,
                    radiusY = R,
                    xAxisRotation = 0,
                    largeArcFlag = true,
                    sweepDirection = SweepDirection.Clockwise,
                    dest = new Point(coordinates.X + R, coordinates.Y)
                });
                newCircle.pathElements.Add(new PathClose());
                newCircle.InitGeometry();
                return newCircle;
            }
        }
    }
}
