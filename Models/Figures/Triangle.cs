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
        [ExportMetadata(nameof(FigureMetadata.Name), "Triangle")]
        private class TriangleCreator : FigureCreator
        {
            public override IFigure Create(Group parentGroup, Point[] coordinates, IFigureGraphicProperties? figureGraphicProperties)
            {
                PathFigure newTri = new(parentGroup, coordinates[0]);
                newTri.Name = "Triangle";

                double sqrt3 = Math.Sqrt(3);
                double lengthSide = 30.0;

                newTri.pathElements.Add(new PathMoveTo() { dest = new Point(coordinates[0].X, coordinates[0].Y - lengthSide / sqrt3) });
                newTri.pathElements.Add(new PathLineTo() { dest = new Point(coordinates[0].X + lengthSide / 2.0, coordinates[0].Y + lengthSide / (2 * sqrt3)) });
                newTri.pathElements.Add(new PathLineTo() { dest = new Point(coordinates[0].X - lengthSide / 2.0, coordinates[0].Y + lengthSide / (2 * sqrt3)) });
                newTri.pathElements.Add(new PathClose());
                newTri.IsClosed = true;
                newTri._graphicProperties = figureGraphicProperties;
                newTri.OnGeometryChanged();
                Scene.Current.OnHierarchyChanged();

                return newTri;
            }
        }
    }
}
