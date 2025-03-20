using Avalonia.Threading;
using Formats;
using Paint2.ViewModels;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using System;
using System.Composition;

namespace Paint2.Models.Figures
{
    public partial class PathFigure
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Name), "Pentagon")]
        private class PentagonCreator : IFigureCreator
        {
            public IFigure Create(Group parentGroup, Point coordinates)
            {
                PathFigure newPentagon = new(parentGroup, coordinates);
                newPentagon.Name = "Pentagon";

                double lengthSide = 30.0;
                double R = lengthSide / (2.0 * Math.Sin(Math.PI / 5.0));
                double cos18 = Math.Cos(Math.PI / 10.0);
                double sin18 = Math.Sin(Math.PI / 10.0);
                double cos54 = Math.Cos(3.0 * Math.PI / 10.0);
                double sin54 = Math.Sin(3.0 * Math.PI / 10.0);

                newPentagon.pathElements.Add(new PathMoveTo() { dest = new Point(coordinates.X, coordinates.Y + R) });
                newPentagon.pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X + R * cos18, coordinates.Y + R * sin18) });
                newPentagon.pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X + R * cos54, coordinates.Y - R * sin54) });
                newPentagon.pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X - R * cos54, coordinates.Y - R * sin54) });
                newPentagon.pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X - R * cos18, coordinates.Y + R * sin18) });
                newPentagon.pathElements.Add(new PathClose());

                newPentagon.InitGeometry();
                return newPentagon;
            }
        }
    }
}
