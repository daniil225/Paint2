using Formats;
using Paint2.ViewModels;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using System.Collections.Generic;
using System.Composition;

namespace Paint2.Models.Figures
{
    public class Rectangle : PathFigure
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Name), nameof(Rectangle))]
        class RectangleCreator : IFigureCreator
        {
            public IFigure Create(Group parentGroup, Point coordinates)
            {
                return new Rectangle(parentGroup, coordinates);
            }
        }
        Rectangle(Group parentGroup, Point coordinates) : base(parentGroup, coordinates)
        {
            Name = "Rectangle";

            double lengthSide = 30.0;
            double halfSide = lengthSide / 2.0;

            Point point = new Point(coordinates.X - lengthSide / 2.0, coordinates.Y - lengthSide / 2.0);

            pathElements.Add(new PathMoveTo() { dest = new Point(coordinates.X - halfSide, coordinates.Y - halfSide) });
            pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X + halfSide, coordinates.Y - halfSide) });
            pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X + halfSide, coordinates.Y + halfSide) });
            pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X - halfSide, coordinates.Y + halfSide) });
            pathElements.Add(new PathClose());
        }
    }
}
