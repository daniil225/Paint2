using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Linq;

namespace Paint2.ViewModels;

public class HeaderPanelViewModel : ViewModelBase
{
    [Reactive] public ObservableCollection<FigureMenuItem> FiguresInMenu { get; set; }
    [Reactive] public FigureMenuItem SelectedFigureMenuItem { get; set; }

    public HeaderPanelViewModel()
    {
        FiguresInMenu =
        [
            new FigureMenuItem("/Assets/Figures/rectangle.svg", "Rectangle"),
            new FigureMenuItem("/Assets/Figures/triangle.svg", "Triangle"),
            new FigureMenuItem("/Assets/Figures/rhombus.svg", "Rhombus"),
            new FigureMenuItem("/Assets/Figures/trapezoid.svg", "Trapezoid"),
            new FigureMenuItem("/Assets/Figures/parallelogram.svg", "Parallelogram"),
            new FigureMenuItem("/Assets/Figures/pentagon.svg", "Pentagon"),
            new FigureMenuItem("/Assets/Figures/circle.svg", "Circle"),
            new FigureMenuItem("/Assets/Figures/oval.svg", "Oval"),
            new FigureMenuItem("/Assets/Figures/line.svg", "Line"),
            new FigureMenuItem("/Assets/Figures/bezier-curve.svg", "Cubic Bezier curve")
        ];
        SelectedFigureMenuItem = FiguresInMenu.First();
    }
}

public class FigureMenuItem(string iconPath, string iconName)
{
    public string IconPath { get; set; } = iconPath;
    public string IconName { get; set; } = iconName;
}