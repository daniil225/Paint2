using Formats;
using Paint2.ViewModels;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using System;
using System.ComponentModel;
using System.Composition;

namespace Paint2.Models.Figures
{
    public class Trapezoid : PathFigure
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Name), nameof(Trapezoid))]
        private class TrapezoidCreator : IFigureCreator
        {
            public IFigure Create(Group parentGroup, Point coordinates)
            {
                return new Trapezoid(parentGroup, coordinates);
            }
        }

        private Trapezoid(Group parentGroup, Point coordinates) : base(parentGroup, coordinates)
        {
            Name = "Trapezoid";

            double lowerBase = 10.0;
            double upperBase = 15.0;
            double height = coordinates.Y * 3.0 * (lowerBase + upperBase) / (2.0 * lowerBase + upperBase);

            double YLowerBase = coordinates.Y - (2.0 * height / 3.0) * (upperBase / (lowerBase + upperBase));
            double YUpperBase = coordinates.Y + (height / 3.0) * (lowerBase - upperBase) / (lowerBase + upperBase);

            pathElements.Add(new PathMoveTo() { dest = new Point(coordinates.X - upperBase / 2.0, YUpperBase) });
            pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X + upperBase / 2.0, YUpperBase) });
            pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X + lowerBase / 2.0, YLowerBase) });
            pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X - lowerBase / 2.0, YLowerBase) });
            pathElements.Add(new PathClose());
        }
    }
}
