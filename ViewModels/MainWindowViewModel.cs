using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;

namespace Paint2.ViewModels
{
    public class BorderType
    {
        public required string Name { get; set; }
        public required string ImagePath { get; set; }
    }

    public partial class MainWindowViewModel : ViewModelBase
    {
        [Reactive] public bool IsPropertiesPanelVisible { get; set; }
        [Reactive] public bool IsGroupsPanelVisible { get; set; }
        [Reactive] public GridLength PropertiesColumnWidth { get; set; }
        [Reactive] public GridLength GroupsColumnWidth { get; set; }
        public ReactiveCommand<Unit, Unit>? HidePropertiesPanelCommand { get; }
        public ReactiveCommand<Unit, Unit>? HideGroupsPanelCommand { get; }

        [Reactive] public string CurrentFigureName { get; set; }
        [Reactive] public double PositionX { get; set; }
        [Reactive] public double PositionY { get; set; }
        [Reactive] public double PercentOfHeight { get; set; }
        [Reactive] public double Height { get; set; }
        [Reactive] public double PercentOfWidth { get; set; }
        [Reactive] public double Width { get; set; }
        [Reactive] public double Angle { get; set; }
        [Reactive] public double Opacity { get; set; }
        [Reactive] public Color SelectedSolidColor { get; set; }
        [Reactive] public Color SelectedBorderColor { get; set; }
        [Reactive] public bool IsReflected { get; set; }
        [Reactive] public bool IsClosed { get; set; }
        [Reactive] public double BorderWidth { get; set; }
        [Reactive] public BorderType SelectedBorderType { get; set; }
        public ObservableCollection<BorderType> BorderTypes { get; set; } = new ObservableCollection<BorderType>
        {
            new() { Name = "Eagle", ImagePath = "avares://Paint2/Assets/Image1.png" },
            new() { Name = "Butterfly", ImagePath = "avares://Paint2/Assets/Image2.png" }

        };

        

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

            CurrentFigureName = "Rect 2";
            PositionX = 763;
            PositionY = 543;
            Width = 78;
            Height = 52;
            Angle = 0;
            Opacity = 100;
            SelectedBorderColor = Colors.Blue;
            SelectedSolidColor = Colors.White;
            IsClosed = true;
        }
    }
}
