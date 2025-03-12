using Avalonia.Controls;
using Avalonia.Input;
using Paint2.ViewModels;
using ReactiveUI;
using System;

namespace Paint2.Views
{
    public partial class HeaderPanelView : UserControl
    {
        private HeaderPanelViewModel? _vm;
        public HeaderPanelView()
        {
            InitializeComponent();
            this.WhenAnyValue(t => t.DataContext)
                .Subscribe(d => _vm = d as HeaderPanelViewModel);
        }

        private void Figure_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            _vm?.AddFigureCommand.Execute();
        }
    }
}