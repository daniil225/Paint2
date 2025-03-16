using Paint2.ViewModels;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using System.Collections.Generic;
using System.Composition;

namespace Paint2.Models.Figures
{
    public class Square : Polygon
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Name), nameof(Square))]
        class SquareCreator : IFigureCreator
        {
            public IFigure Create(Group parentGroup, IDictionary<string, Point> pointParams)
            {
                return new Square(parentGroup, pointParams);
            }
        }
        Square(Group parentGroup, IDictionary<string, Point> pointParams) : base(parentGroup, pointParams)
        {
            name = "Square";
            // Тут нужно создать шаблонную версию фигуры (с со сторонами 1 например)
        }
    }
}
