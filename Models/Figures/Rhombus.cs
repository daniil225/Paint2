using Formats;
using Paint2.ViewModels;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.Composition;

namespace Paint2.Models.Figures
{
    public class Rhombus : PathFigure
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Name), nameof(Rectangle))]
        class RhombusCreator : IFigureCreator
        {
            public IFigure Create(Group parentGroup, Point coordinates)
            {
                return new Rhombus(parentGroup, coordinates);
            }
        }
        Rhombus(Group parentGroup, Point coordinates) : base(parentGroup, coordinates)
        {
            Name = "Rhombus";
            double lengthSide = 30.0;
            double halfDiagonal = lengthSide * Math.Sqrt(2) / 2; //острый угол 45 градусов

            pathElements.Add(new PathMoveTo() { dest = new Point(coordinates.X, coordinates.Y - halfDiagonal) });
            pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X + halfDiagonal, coordinates.Y) });
            pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X, coordinates.Y + halfDiagonal) });
            pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X - halfDiagonal, coordinates.Y) });
            pathElements.Add(new PathClose());
        }
    }
}
