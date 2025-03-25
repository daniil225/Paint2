using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Formats;
using Paint2.ViewModels.Utils;

namespace Paint2.ViewModels.Interfaces
{
    public interface IFigure : ISceneObject
    {
        IFigureGraphicProperties GraphicProperties { get; set; }
        IReadOnlyCollection<IPathElement> PathElements { get; }
        event PropertyChangedEventHandler GeometryChanged;
        IFigure Intersect(IFigure other);
        IFigure Union(IFigure other);
        IFigure Subtract(IFigure other);
        void Export(IExportSnapshot snapshot);
    }
}
