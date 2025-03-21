using System.Composition;
using Paint2.ViewModels;
using Paint2.ViewModels.Utils;
using Paint2.ViewModels.Interfaces;
using Formats;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Paint2.Models.Figures
{
    public partial class PathFigure
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Type), "Bezier")]
        class BezierCreator : IFigureCreator
        {
            public IFigure Create(Group parentGroup, ICollection<Point> coordinatePoints)
            {
                PathFigure newBezier = new(parentGroup, coordinatePoints.ElementAt(0));
                


                newBezier.OnGeometryChanged();
                return newBezier;
            }
        }
    }
}
