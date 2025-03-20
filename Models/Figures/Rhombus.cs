using Avalonia.Threading;
using Formats;
using Paint2.ViewModels;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.Composition;

namespace Paint2.Models.Figures
{
    public partial class PathFigure
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Name), "Rhombus")]
        class RhombusCreator : IFigureCreator
        {
            public IFigure Create(Group parentGroup, Point coordinates)
            {
                PathFigure newPhombus = new(parentGroup, coordinates);
                newPhombus.Name = "Rhombus";
                double lengthSide = 30.0;
                double halfDiagonal = lengthSide * Math.Sqrt(2) / 2; //острый угол 45 градусов

                newPhombus.pathElements.Add(new PathMoveTo() { dest = new Point(coordinates.X, coordinates.Y - halfDiagonal) });
                newPhombus.pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X + halfDiagonal, coordinates.Y) });
                newPhombus.pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X, coordinates.Y + halfDiagonal) });
                newPhombus.pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X - halfDiagonal, coordinates.Y) });
                newPhombus.pathElements.Add(new PathClose());

                newPhombus.InitGeometry();
                return newPhombus;
            }
        }
    }
}
