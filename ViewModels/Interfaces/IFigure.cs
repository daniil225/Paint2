using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Paint2.ViewModels.Utils;

namespace Paint2.ViewModels.Interfaces
{
    public interface IFigure : ISceneObject
    {
        bool IsInternal(Point p, double eps);
        IFigure Intersect(IFigure other);
        IFigure Union(IFigure other);
        IFigure Subtract(IFigure other);
        void Export(IExportSnapshot snapshot);
        void SetParameters(IDictionary<string, double> doubleParams, IDictionary<string, Point> pointParams);
    }
}
