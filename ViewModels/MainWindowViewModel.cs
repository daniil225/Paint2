using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace Paint2.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [Reactive] public bool IsPropertiesPanelVisible { get; set; }
        [Reactive] public bool IsGroupsPanelVisible { get; set; }
        [Reactive] public GridLength PropertiesColumnWidth { get; set; }
        [Reactive] public GridLength GroupsColumnWidth { get; set; }
        public ReactiveCommand<Unit, Unit> HidePropertiesPanelCommand { get; }
        public ReactiveCommand<Unit, Unit> HideGroupsPanelCommand { get; }
        
        [Reactive] public ObservableCollection<FigureMenuItem> FiguresInMenu { get; set; }
        [Reactive] public FigureMenuItem SelectedFigureMenuItem { get; set; }

        public MainWindowViewModel()
        {
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
}
