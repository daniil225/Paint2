using Avalonia.Media;
using Avalonia.Threading;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;

namespace Paint2.ViewModels
{
    public class GeometryViewModel : ViewModelBase
    {
        public IFigure Figure { get; init; }
        [Reactive] public Geometry Geometry { get; set; }
        [Reactive] public DropShadowEffect? ShadowEffect { get; set; }
        public MainWindowViewModel MainWindowViewModel { get; }

        public GeometryViewModel(IFigure figure, MainWindowViewModel mainWindow)
        {
            Figure = figure;
            MainWindowViewModel = mainWindow;
            Dispatcher.UIThread.Invoke(() => Geometry = Renderer.RenderPathElements(Figure.PathElements));
            Figure.GeometryChanged += (sender, args)
                => Dispatcher.UIThread.Invoke(() => Geometry = Renderer.RenderPathElements(Figure.PathElements));
            MainWindowViewModel
                .WhenAnyValue(mvm => mvm.SelectedFigure)
                .Subscribe(UpdateSelection);
        }

        private void UpdateSelection(IFigure? figure)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                if (figure != Figure)
                {
                    ShadowEffect = null;
                    return;
                }
                ShadowEffect = new DropShadowEffect
                {
                    BlurRadius = 10,
                    OffsetX = 3,
                    OffsetY = 3,
                    Color = Colors.Black,
                    Opacity = 0.5
                };
            });
        }
    }
}