﻿using Avalonia.Threading;
using System.Composition;
using Paint2.ViewModels;
using Paint2.ViewModels.Utils;
using Paint2.ViewModels.Interfaces;
using Formats;

namespace Paint2.Models.Figures
{
    public partial class PathFigure
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Name), "Line")]
        class LineCreator : IFigureCreator
        {
            public IFigure Create(Group parentGroup, Point[] coordinates)
            {
                PathFigure newLine = new(parentGroup, coordinates[0]);
                newLine.Name = "Line";

                double lengthLine = 50.0;

                newLine.pathElements.Add(new PathMoveTo() { dest = new Point(coordinates[0].X - lengthLine / 2.0, coordinates[0].Y) });
                newLine.pathElements.Add(new PathLineTo() { dest = new Point(coordinates[0].X + lengthLine / 2.0, coordinates[0].Y) });
                newLine.OnGeometryChanged();
                return newLine;
            }
        }
    }
}
