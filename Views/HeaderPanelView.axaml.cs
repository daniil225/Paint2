using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Paint2.ViewModels;
using System.Collections.Generic;
using ReactiveUI;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Paint2.Views
{
    public partial class HeaderPanelView : UserControl
    {
        private HeaderPanelViewModel? _vm;
        public HeaderPanelView()
        {
            InitializeComponent();
            this.WhenAnyValue(v => v.DataContext)
                .Subscribe(d => _vm = d as HeaderPanelViewModel);
        }

        private async void SaveAsMenuItem_OnClick(object? sender, RoutedEventArgs e)
        {
            if (_vm is null)
            {
                return;
            }
            IStorageFile? file = await OpenSaveFileDialog();
            if (file is null)
            {
                return;
            }
            _vm.CurrentSavedToPath = file.Path.LocalPath;
            _vm.SaveCommand.Execute();
        }

        private async void SaveMenuItem_OnClick(object? sender, RoutedEventArgs e)
        {
            if (_vm is null)
            {
                return;
            }
            if (_vm.CurrentSavedToPath is null)
            {
                IStorageFile? file = await OpenSaveFileDialog();
                if (file is null)
                {
                    return;
                }
                _vm.CurrentSavedToPath = file.Path.LocalPath;
            }
            _vm.SaveCommand.Execute();
        }

        private async Task<IStorageFile?> OpenSaveFileDialog()
        {
            TopLevel? topLevel = TopLevel.GetTopLevel(this);
            if (topLevel is null)
            {
                return null;
            }
            FilePickerFileType jsonExt = new("Json") { Patterns = ["*.json"] };
            FilePickerFileType svgExt = new("Svg") { Patterns = ["*.svg"] };
            FilePickerFileType pdfExt = new("PDF") { Patterns = ["*.pdf"] };
            IReadOnlyList<FilePickerFileType> fileTypes = [jsonExt, svgExt, pdfExt];
            IStorageFile? file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Saving",
                FileTypeChoices = fileTypes,
                SuggestedFileName = "Paint2-file"
            });
            return file;
        }

        private async void OpenMenuItem_OnClick(object? sender, RoutedEventArgs e)
        {
            if (_vm is null)
            {
                return;
            }
            TopLevel? topLevel = TopLevel.GetTopLevel(this);
            if (topLevel is null)
            {
                return;
            }
            FilePickerFileType jsonExt = new("Json") { Patterns = ["*.json"] };
            IReadOnlyList<FilePickerFileType> fileTypes = [jsonExt];
            IReadOnlyList<IStorageFile> files = await topLevel.StorageProvider
                .OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Opening",
                AllowMultiple = false,
                FileTypeFilter = fileTypes
            });
            IStorageFile? file = files.FirstOrDefault();
            if (file is null)
            {
                return;
            }
            _vm.CurrentSavedToPath = file.Path.LocalPath;
            _vm.OpenCommand.Execute();
        }

        private void ExitMenuItem_OnClick(object? sender, RoutedEventArgs e)
        {
            _vm?.ExitCommand.Execute();
        }

        private void CreateMenuItem_OnClick(object? sender, RoutedEventArgs e)
        {
            if (_vm is null)
            {
                return;
            }
            _vm.CurrentSavedToPath = null;
            _vm.CreateCommand.Execute();
        }
    }
}