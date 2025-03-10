using Avalonia.Media;
using Formats;
using Paint2.ViewModels.Utils;

namespace Paint2.ViewModels.Interfaces
{
    public interface IPathBuilder
    {
        // Point везде в глобальных координатах сцены
        IPathBuilder MoveTo(Point dest);
        IPathBuilder LineTo(Point dest);
        IPathBuilder ArcTo(
            Point dest,
            double radiusX, double radiusY,
            // угол вращения системы координат кривой
            // по часовой стрелке в градусах
            double xAxisRotation,
            // из 4 возможных дуг выбирать из пары наибольших дуг
            bool largeArc,
            // из 4 возможных дуг выбирать пару дуг с
            // указанным направлением вращения
            // подробнее о флагах: https://www.w3.org/TR/SVG11/paths.html#PathDataEllipticalArcCommands
            SweepDirection sweepDirection
        );
        IPathBuilder CubicBezierTo(
            Point dest,
            Point controlPoint1,
            Point controlPoint2
        );
        IPathBuilder Close();
        DocPath Build();
    }
}
