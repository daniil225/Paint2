using ReactiveUI.Fody.Helpers;

namespace Paint2.ViewModels;

public class FooterPanelViewModel : ViewModelBase
{
    [Reactive] public string PointerCoordinatesInFormat { get; set; }

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