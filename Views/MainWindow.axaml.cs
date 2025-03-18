using Avalonia.Controls;
using Avalonia.Input;
using Paint2.ViewModels;
using Paint2.ViewModels.Utils;
using ReactiveUI;
using System;

namespace Paint2.Views
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel? _vm;
        public MainWindow()
        {
            InitializeComponent();
            this.WhenAnyValue(t => t.DataContext)
                .Subscribe(d => _vm = d as MainWindowViewModel);
        }

        private void Canvas_OnPointerMoved(object? sender, PointerEventArgs e)
        {
            var point = e.GetCurrentPoint(sender as Control);
            _vm?.FooterPanel.UpdatePointerCoordinates(point.Position.X, point.Position.Y);
        }

        private void Canvas_OnPointerExited(object? sender, PointerEventArgs e)
        {
            _vm?.FooterPanel.ClearPointerCoordinates();
        }

        private void Canvas_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (_vm?.HeaderPanel.MenuMode is MenuModesEnum.CreationMode)
            {
                var point = e.GetCurrentPoint(sender as Control);
                Point pointerCoordinates = new(point.Position.X, point.Position.Y);
                _vm?.CreateFigureCommand.Execute(pointerCoordinates);
            }
        }
    }
}