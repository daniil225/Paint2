using Avalonia.Controls;
using Avalonia.Media;
using Paint2.Models.Figures;
using Paint2.ViewModels.Utils;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using Formats.json;
using System.Linq;
using Paint2.ViewModels.Interfaces;
using System.IO;
using System;

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
    public ReactiveCommand<Unit, Unit> SaveJsonCommand { get; }
    public ReactiveCommand<Unit, Unit> LoadJsonCommand { get; }
    public ObservableCollection<GeometryViewModel> Figures { get; }
    public GeometryJsonSerializer? GeometryJsonSerializer { get; set; }

    public MainWindowViewModel()
    {
        Figures = [];

        // Пример для работы с массивом фигур
        Circle circle = new(new Point(100, 100), 50, new Group(""));
        var properties = new FigureGraphicProperties()
        {
            SolidColor = new Color(255, 255 , 0, 0), BorderColor = new Color(255, 255, 128, 0), BorderThickness = 10
        };
        Figures.Add(new GeometryViewModel { Figure = circle, Properties = properties });
        Renderer renderer = new();
        circle.Render(renderer);
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
SaveJsonCommand = ReactiveCommand.CreateFromTask(async () =>
{
    await Task.Run(() =>
    {
        var figuresToSave = Figures.Select(f => f.Figure).ToList();
        GeometryJsonSerializer?.SaveFigures("/Assets/SaveFiles/figures.json", figuresToSave);

    });
});


LoadJsonCommand = ReactiveCommand.CreateFromTask(async () =>
{
    await Task.Run(() =>
    {
        var loadedFigures = GeometryJsonSerializer?.LoadFigures<IFigure>("/Assets/SaveFiles/figures.json");

        if (loadedFigures != null)
        {
            Figures.Clear();
            foreach (var fig in loadedFigures)
            {
                var viewModel = new GeometryViewModel
                {
                    Figure = fig,
                    Properties = new FigureGraphicProperties()
                    {
                        BorderThickness = 1,
                        SolidColor = Colors.Black,
                        BorderColor = Colors.Black,
                    }
                };
                Figures.Add(viewModel);
            }
        }
    });
});
    }
}
