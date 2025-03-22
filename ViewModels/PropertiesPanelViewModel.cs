using Avalonia.Controls;
using Avalonia.Media;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Paint2.ViewModels;

public class PropertiesPanelViewModel : ViewModelBase
{
    public MainWindowViewModel MainWindow { get; }
    [Reactive] public bool IsPropertyVisible { get; set; }
    [Reactive] public bool IsClosed { get; set; }
    //public double Angle
    //{
    //    get
    //    {
    //        double angle = MainWindow.SelectedFigure.Angle;
    //        return angle;
    //    }
    //    set
    //    {
    //        double prevAngle = MainWindow.SelectedFigure.Angle;
    //        MainWindow.SelectedFigure.Rotate(value - prevAngle, MainWindow.SelectedFigure.Coordinates);
    //    }
    //}
    [Reactive] public BorderType SelectedBorderType { get; set; }
    public List<BorderType> BorderTypes { get; set; } =
    [
        new BorderType { Name = "Solid", ImagePath = "/Assets/BorderTypes/solid.svg" },
        new BorderType { Name = "Dashed", ImagePath = "/Assets/BorderTypes/dashed.svg" },
        new BorderType { Name = "Dotted", ImagePath = "/Assets/BorderTypes/dotted.svg" },
        new BorderType { Name = "Dash-dotted", ImagePath = "/Assets/BorderTypes/dashdotted.svg" }
    ];

    public PropertiesPanelViewModel(MainWindowViewModel mainWindow)
    {
        MainWindow = mainWindow;

        this.WhenAnyValue(vm => vm.MainWindow.SelectedFigure)
            .Subscribe(figure => IsPropertyVisible = figure is not null);

        IsClosed = true;
    }
}
    
public class BorderType
{
    public required string Name { get; set; }
    public required string ImagePath { get; set; }
}