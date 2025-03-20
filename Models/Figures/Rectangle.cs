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
        class RectangleCreator : IFigureCreator
        {
            public IFigure Create(Group parentGroup, Point coordinates)
            {
                PathFigure newRect = new(parentGroup, coordinates);
                newRect.Name = "Rectangle";

                double lengthSide = 30.0;
                double halfSide = lengthSide / 2.0;

                Point point = new Point(coordinates.X - lengthSide / 2.0, coordinates.Y - lengthSide / 2.0);

                newRect.pathElements.Add(new PathMoveTo() { dest = new Point(coordinates.X - halfSide, coordinates.Y - halfSide) });
                newRect.pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X + halfSide, coordinates.Y - halfSide) });
                newRect.pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X + halfSide, coordinates.Y + halfSide) });
                newRect.pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X - halfSide, coordinates.Y + halfSide) });
                newRect.pathElements.Add(new PathClose());

                newRect.InitGeometry();
                return newRect;
            }
        }
    }
}
