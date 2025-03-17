using Paint2.ViewModels;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using System.Composition;

namespace Paint2.Models.Figures
{
    public class Rectangle : PathFigure
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Name), nameof(Rectangle))]
        class SquareCreator : IFigureCreator
        {
            public IFigure Create(Group parentGroup, Point coordinates)
            {
                return new Rectangle(parentGroup, coordinates);
            }
        }
        Rectangle(Group parentGroup, Point coordinates) : base(parentGroup, coordinates)
        {
            name = "Rectangle";
            // Тут нужно создать шаблонную версию фигуры (с со сторонами 1 например)
        }
    }
}
