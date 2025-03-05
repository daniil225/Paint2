using Avalonia.Media;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;

namespace Paint2.ViewModels;

public class PropertiesPanelViewModel : ViewModelBase
{
    [Reactive] public string CurrentFigureName { get; set; }
    [Reactive] public double PositionX { get; set; }
    [Reactive] public double PositionY { get; set; }
    [Reactive] public double PercentOfHeight { get; set; }
    [Reactive] public double Height { get; set; }
    [Reactive] public double PercentOfWidth { get; set; }
    [Reactive] public double Width { get; set; }
    [Reactive] public double Angle { get; set; }
    [Reactive] public double Opacity { get; set; }
    [Reactive] public Color SelectedSolidColor { get; set; }
    [Reactive] public Color SelectedBorderColor { get; set; }
    [Reactive] public bool IsReflected { get; set; }
    [Reactive] public bool IsClosed { get; set; }
    [Reactive] public double BorderWidth { get; set; }
    [Reactive] public BorderType SelectedBorderType { get; set; }
    public List<BorderType> BorderTypes { get; set; } =
    [
        new BorderType { Name = "Solid", ImagePath = "/Assets/BorderTypes/solid.svg" },
        new BorderType { Name = "Dashed", ImagePath = "/Assets/BorderTypes/dashed.svg" },
        new BorderType { Name = "Dotted", ImagePath = "/Assets/BorderTypes/dotted.svg" },
        new BorderType { Name = "Dash-dotted", ImagePath = "/Assets/BorderTypes/dashdotted.svg" }
    ];

    public PropertiesPanelViewModel()
    {
        CurrentFigureName = "Rect 2";
        PositionX = 763;
        PositionY = 543;
        Width = 78;
        Height = 52;
        Angle = 0;
        Opacity = 100;
        SelectedBorderColor = Colors.Blue;
        SelectedSolidColor = Colors.White;
        IsClosed = true;
    }
}
    
public class BorderType
{
    public required string Name { get; set; }
    public required string ImagePath { get; set; }
}