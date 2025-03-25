using Formats;
using Paint2.ViewModels;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint2.Models.Figures
{
    public partial class PathFigure
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Name), "CubicBezierCurve")]
        private class CubicBezierCurveCreator : FigureCreator
        {
            public override IFigure Create(Group parentGroup, Point[] coordinates)
            {
                Point center = 1.0 / 8.0 * coordinates[0] + 3.0 / 8.0 * coordinates[1] + 3.0 / 8.0 * coordinates[2] + 1.0 / 8.0 * coordinates[3];

                PathFigure newCubicBezierCurve = new(parentGroup, center);
                newCubicBezierCurve.Name = "Cubic Bezier curve";

                newCubicBezierCurve.pathElements.Add(new PathMoveTo() { dest = coordinates[0] });
                newCubicBezierCurve.pathElements.Add(new PathCubicBezierTo()
                {
                    controlPoint1 = coordinates[1],
                    controlPoint2 = coordinates[2],
                    dest = coordinates[3]
                });
                newCubicBezierCurve.IsClosed = false;
                newCubicBezierCurve.OnGeometryChanged();
                return newCubicBezierCurve;
            }
        }
    }
}
