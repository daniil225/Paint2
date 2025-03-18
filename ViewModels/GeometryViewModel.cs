using Paint2.ViewModels.Interfaces;

namespace Paint2.ViewModels
{
    public class GeometryViewModel : ViewModelBase
    {
        public required IFigure Figure { get; init; }
        public required MainWindowViewModel MainWindowViewModel { get; init; }
    }
}