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
        class RhombusCreator : FigureCreator
        {
            public override IFigure Create(Group parentGroup, Point[] coordinates, IFigureGraphicProperties? figureGraphicProperties)
            {
                PathFigure newPhombus = new(parentGroup, coordinates[0]);
                newPhombus.Name = "Rhombus";
                double lengthSide = 30.0;
                double halfDiagonal = lengthSide * Math.Sqrt(2) / 2; //острый угол 45 градусов

                newPhombus.pathElements.Add(new PathMoveTo() { dest = new Point(coordinates[0].X, coordinates[0].Y - halfDiagonal) });
                newPhombus.pathElements.Add(new PathLineTo() { dest = new Point(coordinates[0].X + halfDiagonal, coordinates[0].Y) });
                newPhombus.pathElements.Add(new PathLineTo() { dest = new Point(coordinates[0].X, coordinates[0].Y + halfDiagonal) });
                newPhombus.pathElements.Add(new PathLineTo() { dest = new Point(coordinates[0].X - halfDiagonal, coordinates[0].Y) });
                newPhombus.pathElements.Add(new PathClose());
                newPhombus.IsClosed = true;
                newPhombus._graphicProperties = figureGraphicProperties;
                newPhombus.OnGeometryChanged();
                Scene.Current.OnHierarchyChanged();

                return newPhombus;
            }
        }
    }
}
