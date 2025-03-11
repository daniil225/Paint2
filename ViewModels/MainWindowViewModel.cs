using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Paint2.Models.Figures;
using Paint2.ViewModels.Utils;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;

namespace Paint2.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ViewModelBase GroupsPanel { get; } = new GroupsPanelViewModel();
    public ViewModelBase PropertiesPanel { get; } = new PropertiesPanelViewModel();
    public ViewModelBase HeaderPanel { get; } = new HeaderPanelViewModel();
    
    [Reactive] public bool IsPropertiesPanelVisible { get; set; }
    [Reactive] public bool IsGroupsPanelVisible { get; set; }
    [Reactive] public GridLength PropertiesColumnWidth { get; set; }
    [Reactive] public GridLength GroupsColumnWidth { get; set; }
    public ReactiveCommand<Unit, Unit> HidePropertiesPanelCommand { get; }
    public ReactiveCommand<Unit, Unit> HideGroupsPanelCommand { get; }
    public ObservableCollection<GeometryViewModel> Figures { get; }

    private Circle circle;
    public MainWindowViewModel()
    {
        Figures = [];
        
        // Пример для работы с массивом фигур
        circle = new Circle(new Point {x = 100, y = 100}, 50);
        var properties = new FigureGraphicProperties()
        {
            SolidColor = new Color(255, 255 , 0, 0), BorderColor = new Color(255, 255, 128, 0), BorderThickness = 10
        };
        Figures.Add(new GeometryViewModel { Figure = circle, Properties = properties });
        Renderer renderer = new();
        circle.Render(renderer);
        ////////////////////////////////////
        
        IsPropertiesPanelVisible = true;
        PropertiesColumnWidth = new GridLength(0, GridUnitType.Auto);

        HidePropertiesPanelCommand = ReactiveCommand.CreateFromTask(async() =>
        {
            await Task.Run(() =>
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    //circle.Radius *= 1.3;
                    //circle.Render(renderer);
                });
                IsPropertiesPanelVisible = !IsPropertiesPanelVisible;
                PropertiesColumnWidth = IsPropertiesPanelVisible
                    ? new GridLength(0, GridUnitType.Auto)
                    : new GridLength(0);
            });
        });
        
        IsGroupsPanelVisible = true;
        GroupsColumnWidth = new GridLength(0, GridUnitType.Auto);
        
        HideGroupsPanelCommand = ReactiveCommand.CreateFromTask(async() =>
        {
            await Task.Run(() =>
            {
                IsGroupsPanelVisible = !IsGroupsPanelVisible;
                GroupsColumnWidth = IsGroupsPanelVisible
                    ? new GridLength(0, GridUnitType.Auto)
                    : new GridLength(0);
            });
        });
    }
}
