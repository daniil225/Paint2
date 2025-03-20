using Avalonia.Media;
using Avalonia.Threading;
using Formats;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using ReactiveUI.Fody.Helpers;

namespace Paint2.ViewModels
{
    public class GeometryViewModel : ViewModelBase
    {
        public IFigure Figure { get; init; }
        [Reactive] public Geometry Geometry { get; set; }
        public required MainWindowViewModel MainWindowViewModel { get; init; }

        public GeometryViewModel(IFigure figure)
        {
            Figure = figure;
            Dispatcher.UIThread.Invoke(() => Geometry = Renderer.RenderPathElements(Figure.PathElements));
            Figure.GeometryChanged += (sender, args) => Dispatcher.UIThread.Invoke(() => Geometry = Renderer.RenderPathElements(Figure.PathElements));
        }
    }
}