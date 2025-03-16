using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Composition;
using Paint2.ViewModels;
using Paint2.ViewModels.Utils;
using Paint2.ViewModels.Interfaces;
using ReactiveUI.Fody.Helpers;
using Serilog;

namespace Paint2.Models.Figures
{
    public class Circle : PathFigure
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Name), nameof(Circle))]
        class CircleCreator : IFigureCreator
        {
            public IFigure Create(Group parentGroup, Point coordinates)
            {
                return new Circle(parentGroup, coordinates);
            }
        }
        Circle(Group parentGroup, Point pointParams) : base(parentGroup, pointParams)
        {
            name = "Circle";
            // Тут нужно создать шаблонную версию фигуры (с радиусом 1 например)
        }
    }
}
