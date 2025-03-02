using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
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
                new FigureMenuItem("/Assets/Figures/rectangle.svg"),
                new FigureMenuItem("/Assets/Figures/triangle.svg"),
                new FigureMenuItem("/Assets/Figures/rhombus.svg"),
                new FigureMenuItem("/Assets/Figures/trapezoid.svg"),
                new FigureMenuItem("/Assets/Figures/parallelogram.svg"),
                new FigureMenuItem("/Assets/Figures/pentagon.svg"),
                new FigureMenuItem("/Assets/Figures/circle.svg"),
                new FigureMenuItem("/Assets/Figures/oval.svg"),
                new FigureMenuItem("/Assets/Figures/line.svg"),
                new FigureMenuItem("/Assets/Figures/bezier-curve.svg")
            ];
        }
    }

    public class FigureMenuItem(string iconPath)
    {
        public string IconPath { get; set; } = iconPath;
    }
}
