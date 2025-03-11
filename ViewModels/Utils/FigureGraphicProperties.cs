using Avalonia.Media;
using Paint2.ViewModels.Interfaces;

namespace Paint2.ViewModels.Utils
{
    public class FigureGraphicProperties : IFigureGraphicProperties
    {
        public Color SolidColor { get; init; }
        public Color BorderColor { get; init; }
        public double BorderThickness { get; init; }
    }
}