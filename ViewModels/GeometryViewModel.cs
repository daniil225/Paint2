using Paint2.ViewModels.Interfaces;
using ReactiveUI.Fody.Helpers;

namespace Paint2.ViewModels
{
    public class GeometryViewModel : ViewModelBase
    {
        public required IFigure Figure { get; init; }
        [Reactive] public required IFigureGraphicProperties Properties { get; set; }
    }
}