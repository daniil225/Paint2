using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
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
        public ReactiveCommand<Unit, Unit>? HidePropertiesPanelCommand { get; }
        public ReactiveCommand<Unit, Unit>? HideGroupsPanelCommand { get; }

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
        }
    }
}
