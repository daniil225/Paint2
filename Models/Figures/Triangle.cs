using Avalonia.Threading;
using Formats;
using Paint2.ViewModels;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

namespace Paint2.Models.Figures
{
    public partial class PathFigure
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Type), "Triangle")]
        private class TriangleCreator : IFigureCreator
        {
            public IFigure Create(Group parentGroup, ICollection<Point> coordinatePoints)
            {
                Point coordinates = coordinatePoints.ElementAt(0);
                PathFigure newTri = new(parentGroup, coordinates);
                newTri.Name = "Triangle";

                double sqrt3 = Math.Sqrt(3);
                double lengthSide = 30.0;

                newTri.pathElements.Add(new PathMoveTo() { dest = new Point(coordinates.X, coordinates.Y + lengthSide / sqrt3) });
                newTri.pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X + lengthSide / 2.0, coordinates.Y - lengthSide / (2 * sqrt3)) });
                newTri.pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X - lengthSide / 2.0, coordinates.Y - lengthSide / (2 * sqrt3)) });
                newTri.pathElements.Add(new PathClose());
                newTri.OnGeometryChanged();

                return newTri;
            }
        }
    }
}
