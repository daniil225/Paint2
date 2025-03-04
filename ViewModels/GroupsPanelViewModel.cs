using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;

namespace Paint2.ViewModels
{
    public class GroupsPanelViewModel : ViewModelBase
    {
        private ObservableCollection<GroupItem> _groups =
        [
            new("Группа 1") { Children = { new GroupItem("Элемент 1"), new GroupItem("Элемент 2") } },
            new("Группа 2") { Children = { new GroupItem("Элемент 1"), new ("Элемент 2"), new GroupItem("Элемент 3") } }
        ];
        public HierarchicalTreeDataGridSource<GroupItem> TreeSource { get; }

        public ReactiveCommand<Unit, Unit> AddElementCommand { get; }

        public GroupsPanelViewModel()
        {
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
}