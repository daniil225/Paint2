using Avalonia.Collections;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;

namespace Paint2.ViewModels;

public class PropertiesPanelViewModel : ViewModelBase
{
    public MainWindowViewModel MainWindow { get; }
    [Reactive] public bool IsPropertyVisible { get; set; }
    [Reactive] public bool IsClosed { get; set; }

    private BorderType _selectedBorderType;
    public BorderType SelectedBorderType
    {
        get => _selectedBorderType;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedBorderType, value);
            if (MainWindow.SelectedFigure?.GraphicProperties is null)
            {
                return;
            }
            MainWindow.SelectedFigure.GraphicProperties.BorderStyle = value.Style;
        }
    }
    public List<BorderType> BorderTypes { get; set; } =
    [
        new BorderType
        {
            Name = "Solid", ImagePath = "/Assets/BorderTypes/solid.svg", Style = []
        },
        new BorderType
        {
            Name = "Dashed", ImagePath = "/Assets/BorderTypes/dashed.svg", Style = [3, 1]
        },
        new BorderType
        {
            Name = "Dotted", ImagePath = "/Assets/BorderTypes/dotted.svg", Style = [1, 1]
        },
        new BorderType
        {
            Name = "Dash-dotted", ImagePath = "/Assets/BorderTypes/dashdotted.svg", Style = [3, 3, 1, 3]
        }
    ];

    public PropertiesPanelViewModel(MainWindowViewModel mainWindow)
    {
        MainWindow = mainWindow;
        SelectedBorderType = BorderTypes[0];

        this.WhenAnyValue(vm => vm.MainWindow.SelectedFigure)
            .Subscribe(figure => IsPropertyVisible = figure is not null);

        IsClosed = true;
    }
}
    
public class BorderType
{
    public required string Name { get; set; }
    public required string ImagePath { get; set; }
    public required AvaloniaList<double> Style { get; set; }
}