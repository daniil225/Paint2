using Formats;
using Paint2.ViewModels.Interfaces;
using System;
using Paint2.ViewModels;
using Paint2.ViewModels.Utils;
using System.Composition;

namespace Paint2.Models.Figures
{
    public class Parallelogram : PathFigure
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Name), nameof(Parallelogram))]
        class ParallelogramCreator : IFigureCreator
        {
            public IFigure Create(Group parentGroup, Point coordinates)
            {
                return new Parallelogram(parentGroup, coordinates);
            }
        }
        Parallelogram(Group parentGroup, Point coordinates) : base(parentGroup, coordinates)
        {
            Name = "Parallelogram";
            double lengthSide = 30.0;
            double angle = 45 * Math.PI / 180.0;

            double offsetX = lengthSide * Math.Cos(angle);
            double offsetY = lengthSide * Math.Sin(angle);

            pathElements.Add(new PathMoveTo() { dest = new Point(coordinates.X - offsetY - lengthSide / 2.0, coordinates.Y - offsetY / 2.0) });
            pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X - offsetY + lengthSide / 2.0, coordinates.Y - offsetY / 2.0) });
            pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X - offsetY + lengthSide / 2.0 + offsetX, coordinates.Y + offsetY / 2.0) });
            pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X - offsetY - lengthSide / 2.0 + offsetX, coordinates.Y + offsetY / 2.0) });
            pathElements.Add(new PathClose());
        }
    }

}
