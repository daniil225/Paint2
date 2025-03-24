using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Paint2.ViewModels;

public class FooterPanelViewModel : ViewModelBase
{
    [Reactive] public string PointerCoordinatesInFormat { get; set; }

    private string? _currentDocument;
    public string? CurrentDocument
    {
        get => _currentDocument ?? "Document to save not found.";
        set => this.RaiseAndSetIfChanged(ref _currentDocument, value);
    }

    public FooterPanelViewModel()
    {
        ClearPointerCoordinates();
    }

    public void UpdatePointerCoordinates(double x, double y)
    {
        string coordinatesInFormat = $"x = {(int)x}, y = {(int)y} px";
        PointerCoordinatesInFormat = coordinatesInFormat;
    }

    public void ClearPointerCoordinates()
    {
        PointerCoordinatesInFormat = "";
    }
}