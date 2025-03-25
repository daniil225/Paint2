using Avalonia.Threading;
using Formats;
using Paint2.ViewModels.Interfaces;
using System;
using Paint2.ViewModels;
using Paint2.ViewModels.Utils;
using System.Composition;

namespace Paint2.Models.Figures
{
    public partial class PathFigure
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Name), "Parallelogram")]
        class ParallelogramCreator : FigureCreator
        {
            public override IFigure Create(Group parentGroup, Point[] coordinates, IFigureGraphicProperties? figureGraphicProperties)
            {
                PathFigure newParallelogram = new(parentGroup, coordinates[0]);
                newParallelogram.Name = "Parallelogram";
                double lengthSide = 30.0;
                double angle = 45 * Math.PI / 180.0;

                double offsetX = lengthSide * Math.Cos(angle);
                double offsetY = lengthSide * Math.Sin(angle);

                newParallelogram.pathElements.Add(new PathMoveTo() { dest = new Point(coordinates[0].X - offsetY / 2.0 - lengthSide / 2.0, coordinates[0].Y - offsetY / 2.0) });
                newParallelogram.pathElements.Add(new PathLineTo() { dest = new Point(coordinates[0].X - offsetY / 2.0 + lengthSide / 2.0, coordinates[0].Y - offsetY / 2.0) });
                newParallelogram.pathElements.Add(new PathLineTo() { dest = new Point(coordinates[0].X - offsetY / 2.0 + lengthSide / 2.0 + offsetX, coordinates[0].Y + offsetY / 2.0) });
                newParallelogram.pathElements.Add(new PathLineTo() { dest = new Point(coordinates[0].X - offsetY / 2.0 - lengthSide / 2.0 + offsetX, coordinates[0].Y + offsetY / 2.0) });
                newParallelogram.pathElements.Add(new PathClose());
                newParallelogram.IsClosed = true;
                newParallelogram._graphicProperties = figureGraphicProperties;
                newParallelogram.OnGeometryChanged();
                Scene.Current.OnHierarchyChanged();

                return newParallelogram;
            }
        }
    }
}