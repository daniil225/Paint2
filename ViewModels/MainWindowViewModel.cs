using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
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
        [Reactive] public FigureMenuItem SelectedFigureMenuItem { get; set; }
        
        private ObservableCollection<GroupItem> _groups =
        [
            new("Группа 1") { Children = { new GroupItem("Элемент 1"), new GroupItem("Элемент 2") } },

            new("Группа 2") { Children = { new GroupItem("Элемент 1"), new ("Элемент 2"), new GroupItem("Элемент 3") } }
        ];
        public HierarchicalTreeDataGridSource<GroupItem> TreeSource { get; }

        public ReactiveCommand<Unit, Unit> AddElementCommand { get; }
        

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

            TreeSource = new HierarchicalTreeDataGridSource<GroupItem>(_groups)
            {
                Columns =
                {
                    new HierarchicalExpanderColumn<GroupItem>(
                        new TextColumn<GroupItem, string>("Название", x => x.Name), x => x.Children)
                }
            };

            AddElementCommand = ReactiveCommand.Create(() =>
            {
                if (_groups.Count > 0)
                {
                    _groups[0].Children.Add(new GroupItem($"Элемент { _groups[0].Children.Count + 1}"));
                    TreeSource.Items = _groups;
                }
            });
        }
    }
    
    public class GroupItem
    {
        public string Name { get; set; }
        public ObservableCollection<GroupItem> Children { get; }

        public GroupItem(string name)
        {
            Name = name;
            Children = new ObservableCollection<GroupItem>();
        }
    }

    public class FigureMenuItem(string iconPath, string iconName)
    {
        public string IconPath { get; set; } = iconPath;
        public string IconName { get; set; } = iconName;
    }
}
