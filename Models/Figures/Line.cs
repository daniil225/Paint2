using Avalonia.Threading;
using System.Composition;
using Paint2.ViewModels;
using Paint2.ViewModels.Utils;
using Paint2.ViewModels.Interfaces;
using Formats;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Paint2.Models.Figures
{
    public partial class PathFigure
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Type), "Line")]
        class LineCreator : IFigureCreator
        {
            public IFigure Create(Group parentGroup, ICollection<Point> coordinatePoints)
            {
                Point coordinates = coordinatePoints.ElementAt(0);
                PathFigure newLine = new(parentGroup, coordinates);
                newLine.Name = "Line";

                double lengthLine = 50.0;

                newLine.pathElements.Add(new PathMoveTo() { dest = new Point(coordinates.X - lengthLine / 2.0, coordinates.Y) });
                newLine.pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X + lengthLine / 2.0, coordinates.Y) });
                newLine.OnGeometryChanged();
                return newLine;
            }
        }
    }
}
