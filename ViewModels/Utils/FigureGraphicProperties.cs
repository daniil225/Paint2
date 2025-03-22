using Avalonia.Media;
using Paint2.ViewModels.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Paint2.ViewModels.Utils
{
    public class FigureGraphicProperties : ReactiveObject, IFigureGraphicProperties
    {
        [Reactive] public Color SolidColor { get; set; }
        [Reactive] public Color BorderColor { get; set; }
        [Reactive] public double BorderThickness { get; set; }
        [Reactive] public DashStyle BorderStyle{ get; set; }
    }
}