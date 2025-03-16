using Paint2.ViewModels;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using System.Collections.Generic;
using System.Composition;

namespace Paint2.Models.Figures
{
    public class Square : PathFigure
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Name), nameof(Square))]
        class SquareCreator : IFigureCreator
        {
            public IFigure Create(Group parentGroup, Point coordinates)
            {
                return new Square(parentGroup, coordinates);
            }
        }
        Square(Group parentGroup, Point coordinates) : base(parentGroup, coordinates)
        {
            name = "Square";
            // Тут нужно создать шаблонную версию фигуры (с со сторонами 1 например)
        }
    }
}
