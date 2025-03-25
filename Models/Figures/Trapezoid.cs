using Avalonia.Threading;
using Formats;
using Paint2.ViewModels;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using System.Collections.Generic;
using System.Composition;

namespace Paint2.Models.Figures
{
    public partial class PathFigure
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Name), "Trapezoid")]
        private class TrapezoidCreator : FigureCreator
        {
            public override IFigure Create(Group parentGroup, Point[] coordinates, IFigureGraphicProperties? figureGraphicProperties)
            {
                PathFigure newTrap = new(parentGroup, coordinates[0]);
                newTrap.Name = "Trapezoid";

                double lowerBase = 40.0;
                double upperBase = 25.0;
                double height = 20.0;

                newTrap.pathElements.Add(new PathMoveTo() { dest = new Point(coordinates[0].X - lowerBase / 2.0, coordinates[0].Y + height / 2.0) });
                newTrap.pathElements.Add(new PathLineTo() { dest = new Point(coordinates[0].X + lowerBase / 2.0, coordinates[0].Y + height / 2.0) });
                newTrap.pathElements.Add(new PathLineTo() { dest = new Point(coordinates[0].X + upperBase / 2.0, coordinates[0].Y - height / 2.0) });
                newTrap.pathElements.Add(new PathLineTo() { dest = new Point(coordinates[0].X - upperBase / 2.0, coordinates[0].Y - height / 2.0) });
                newTrap.pathElements.Add(new PathClose());
                newTrap.IsClosed = true;
                newTrap._graphicProperties = figureGraphicProperties;
                newTrap.OnGeometryChanged();
                Scene.Current.OnHierarchyChanged();

                return newTrap;
            }
        }
    }
}
