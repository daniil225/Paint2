using Avalonia.Collections;
using Avalonia.Media;

namespace Paint2.ViewModels.Interfaces
{
    public interface IFigureGraphicProperties
    {
        Color SolidColor { get; set; }
        Color BorderColor { get; set; }
        double BorderThickness { get; set; }
        AvaloniaList<double> BorderStyle { get; set; }
    }
}
