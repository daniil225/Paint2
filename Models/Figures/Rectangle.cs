using Avalonia.Threading;
using Formats;
using Paint2.ViewModels;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using ReactiveUI;
using System.Collections.Generic;
using System.Composition;

namespace Paint2.Models.Figures
{
    public partial class PathFigure
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Name), "Rectangle")]
        class RectangleCreator : FigureCreator
        {
            public override IFigure Create(Group parentGroup, Point[] coordinates)
            {
                PathFigure newRect = new(parentGroup, coordinates[0]);
                newRect.Name = "Rectangle";

                double lengthSide = 30.0;
                double halfSide = lengthSide / 2.0;

                Point point = new Point(coordinates[0].X - lengthSide / 2.0, coordinates[0].Y - lengthSide / 2.0);

                newRect.pathElements.Add(new PathMoveTo() { dest = new Point(coordinates[0].X - halfSide, coordinates[0].Y - halfSide) });
                newRect.pathElements.Add(new PathLineTo() { dest = new Point(coordinates[0].X + halfSide, coordinates[0].Y - halfSide) });
                newRect.pathElements.Add(new PathLineTo() { dest = new Point(coordinates[0].X + halfSide, coordinates[0].Y + halfSide) });
                newRect.pathElements.Add(new PathLineTo() { dest = new Point(coordinates[0].X - halfSide, coordinates[0].Y + halfSide) });
                newRect.pathElements.Add(new PathClose());
                newRect.IsClosed = true;
                newRect.OnGeometryChanged();

                return newRect;
            }
        }
    }
}
