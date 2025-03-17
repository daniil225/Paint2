using Avalonia.Controls;
using Avalonia.Media;
using Paint2.Models.Figures;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;

namespace Paint2.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ViewModelBase GroupsPanel { get; }
    public ViewModelBase PropertiesPanel { get; }
    public ViewModelBase HeaderPanel { get; }
    
    [Reactive] public bool IsPropertiesPanelVisible { get; set; }
    [Reactive] public bool IsGroupsPanelVisible { get; set; }
    [Reactive] public GridLength PropertiesColumnWidth { get; set; }
    [Reactive] public GridLength GroupsColumnWidth { get; set; }
    public ReactiveCommand<Unit, Unit> HidePropertiesPanelCommand { get; }
    public ReactiveCommand<Unit, Unit> HideGroupsPanelCommand { get; }
    public ObservableCollection<GeometryViewModel> Figures { get; }

    public MainWindowViewModel()
    {
        Figures = [];
        Scene.CreateScene(Figures);
        // Пример для работы с массивом фигур
        //IFigure circle = FigureFabric.CreateFigure("Circle", new Group(""), new Dictionary<string, Point> { { "Coordinates", Point.Zero } });
        //var properties = new FigureGraphicProperties()
        //{
        //    SolidColor = new Color(255, 255 , 0, 0), BorderColor = new Color(255, 255, 128, 0), BorderThickness = 10
        //};
        //Figures.Add(new GeometryViewModel { Figure = circle, Properties = properties });
        //Renderer renderer = new();
        //circle.Render(renderer);
        ////////////////////////////////////

        HeaderPanel = new HeaderPanelViewModel(Figures);
        PropertiesPanel = new PropertiesPanelViewModel();
        GroupsPanel = new GroupsPanelViewModel();
        
        IsPropertiesPanelVisible = true;
        PropertiesColumnWidth = new GridLength(0, GridUnitType.Auto);

        HidePropertiesPanelCommand = ReactiveCommand.CreateFromTask(async() =>
        {
            await Task.Run(() =>
            {
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
