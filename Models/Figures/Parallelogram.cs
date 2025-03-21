using Avalonia.Threading;
using Formats;
using Paint2.ViewModels.Interfaces;
using System;
using Paint2.ViewModels;
using Paint2.ViewModels.Utils;
using System.Composition;
using System.Collections.Generic;
using System.Linq;

namespace Paint2.Models.Figures
{
    public partial class PathFigure
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Type), "Parallelogram")]
        class ParallelogramCreator : IFigureCreator
        {
            public IFigure Create(Group parentGroup, ICollection<Point> coordinatePoints)
            {
                Point coordinates = coordinatePoints.ElementAt(0);
                PathFigure newParallelogram = new(parentGroup, coordinates);
                newParallelogram.Name = "Parallelogram";
                double lengthSide = 30.0;
                double angle = 45 * Math.PI / 180.0;

                double offsetX = lengthSide * Math.Cos(angle);
                double offsetY = lengthSide * Math.Sin(angle);

                newParallelogram.pathElements.Add(new PathMoveTo() { dest = new Point(coordinates.X - offsetY / 2.0 - lengthSide / 2.0, coordinates.Y - offsetY / 2.0) });
                newParallelogram.pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X - offsetY / 2.0 + lengthSide / 2.0, coordinates.Y - offsetY / 2.0) });
                newParallelogram.pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X - offsetY / 2.0 + lengthSide / 2.0 + offsetX, coordinates.Y + offsetY / 2.0) });
                newParallelogram.pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X - offsetY / 2.0 - lengthSide / 2.0 + offsetX, coordinates.Y + offsetY / 2.0) });
                newParallelogram.pathElements.Add(new PathClose());
                newParallelogram.OnGeometryChanged();

                return newParallelogram;
            }
        }
    }
}