using Formats;
using Paint2.ViewModels;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using System;
using System.ComponentModel;
using System.Composition;

namespace Paint2.Models.Figures
{
    public class Pentagon : PathFigure
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Name), nameof(Pentagon))]
        private class PentagonCreator : IFigureCreator
        {
            public IFigure Create(Group parentGroup, Point coordinates)
            {
                return new Pentagon(parentGroup, coordinates);
            }
        }

        private Pentagon(Group parentGroup, Point coordinates) : base(parentGroup, coordinates)
        {
            Name = "Pentagon";

            double lengthSide = 10.0;
            double R = lengthSide / (2.0 * Math.Sin(Math.PI / 5.0));
            double cos18 = Math.Cos(Math.PI / 10.0);
            double sin18 = Math.Sin(Math.PI / 10.0);
            double cos54 = Math.Cos(3.0 * Math.PI / 10.0);
            double sin54 = Math.Sin(3.0 * Math.PI / 10.0);

            pathElements.Add(new PathMoveTo() { dest = new Point(coordinates.X, coordinates.Y + R) });
            pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X + R * cos18, coordinates.Y + R * sin18) });
            pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X + R * cos54, coordinates.Y - R * sin54) });
            pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X - R * cos54, coordinates.Y - R * sin54) });
            pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X - R * cos18, coordinates.Y + R * sin18) });
            pathElements.Add(new PathClose());
        }
    }
}
