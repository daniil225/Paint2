using Avalonia.Media;
using Avalonia.Threading;
using Paint2.Models.Figures;
using Paint2.ViewModels.Utils;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace Paint2.ViewModels;

public class HeaderPanelViewModel : ViewModelBase
{
    [Reactive] public ObservableCollection<FigureMenuItem> FiguresInMenu { get; set; }
    [Reactive] public FigureMenuItem SelectedFigureMenuItem { get; set; }
    
    public ReactiveCommand<Unit, Unit> AddFigureCommand { get; }

    public HeaderPanelViewModel(ObservableCollection<GeometryViewModel> figures)
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

        AddFigureCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await Task.Run(() =>
            {
                var rand = new Random();
                var circle = new Circle(new Point(rand.Next(0, 500), rand.Next(0, 500)), rand.Next(3, 50), new Group(""));
                var properties = new FigureGraphicProperties
                {
                    SolidColor = new Color(255, (byte)rand.Next(128, 255) , (byte)rand.Next(128, 255), (byte)rand.Next(128, 255)),
                    BorderColor = new Color(255, (byte)rand.Next(128, 255), (byte)rand.Next(128, 255), (byte)rand.Next(128, 255)), 
                    BorderThickness = (byte)rand.Next(3, 15)
                };
                figures.Add(new GeometryViewModel { Figure = circle, Properties = properties });
                Renderer renderer = new();
                Dispatcher.UIThread.Invoke(() =>
                {
                    circle.Render(renderer);
                });
            });
        });
    }
}

public class FigureMenuItem(string iconPath, string iconName)
{
    public string IconPath { get; set; } = iconPath;
    public string IconName { get; set; } = iconName;
}