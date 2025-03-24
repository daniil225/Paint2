using Avalonia.Collections;
using Paint2.ViewModels.Utils;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Paint2.ViewModels;

public class PropertiesPanelViewModel : ViewModelBase
{
    public MainWindowViewModel MainWindow { get; }
    [Reactive] public bool IsPropertyVisible { get; set; }
    [Reactive] public bool IsClosed { get; set; }

    private double _xCoord;
    public double Xcoord
    {
        get => _xCoord;
        set
        {
            if (MainWindow.SelectedFigure is null)
            {
                return;
            }
            this.RaiseAndSetIfChanged(ref _xCoord, value);
            Point coordinates = MainWindow.SelectedFigure.Coordinates;
            MainWindow.SelectedFigure
                .Move( new Point(value, coordinates.Y) - coordinates, false);
        }
    }
    private double _yCoord;
    public double Ycoord
    {
        get => _yCoord;
        set
        {
            if (MainWindow.SelectedFigure is null)
            {
                return;
            }
            this.RaiseAndSetIfChanged(ref _yCoord, value);
            Point coordinates = MainWindow.SelectedFigure.Coordinates;
            MainWindow.SelectedFigure
                .Move( new Point(coordinates.X, value) - coordinates, false);
        }
    }
    

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

        this.WhenAnyValue(vm => vm.MainWindow.SelectedFigure)
            .Subscribe(figure => IsPropertyVisible = figure is not null);

        this.WhenAnyValue(vm => vm.MainWindow.SelectedFigure)
            .WhereNotNull()
            .Subscribe(f =>
            {
                f.WhenAnyValue(g => g.Coordinates)
                    .Subscribe(p => 
                    {
                        Xcoord = p.X;
                        Ycoord = p.Y;
                    });
                var borderType = BorderTypes.FirstOrDefault(
                    x => x.Style.SequenceEqual(f.GraphicProperties.BorderStyle));
                if (borderType is not null)
                {
                    SelectedBorderType = borderType;
                }
            });
        
        IsClosed = true;
    }
}
    
public class BorderType
{
    public required string Name { get; set; }
    public required string ImagePath { get; set; }
    public required AvaloniaList<double> Style { get; set; }
}