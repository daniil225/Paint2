using Avalonia.Media;
using System.Collections.Generic;

namespace Interfaces
{
    public class Point
    {
        public double x, y;
    }
    public interface IFigureGraphicProperties
    {
        Color SolidColor { get; }
        Color BorderColor { get; }
    
    }
    public interface IRenderInterface
    {
        
    }

    public interface IFigure
    {
        // Наверное лучше в виде поля оставить, для имени нужен будет ReactiveUI, а через функции вроде так не делают
        string Name { get; set; }

        bool IsInternal(Point p, double eps);
        IFigure Intersect(IFigure other);
        IFigure Union(IFigure other);
        IFigure Subtract(IFigure other);
        void Move(Point vector);
        void Rotate(Point Center, double angle);
        void Scale(double x, double y);
        void Scale(Point Center, double rad);
        void Reflection(Point ax1, Point ax2);
        void Render(IRenderInterface toDraw);
        void Export(IExportInterface exportInterface);
        void SetParameters(IDictionary<string, double> doubleParams, IDictionary<string, Point> pointParams);
    }
    
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
        Formats.Path Build();
    }
    
    public interface IExportInterface
    {
        Formats.Brush Brush { get; set; }
        // Position - координата левого верхнего угла прямоугольника
        void ExportRect(Point position, double width, double height, string? id);
        void ExportCircle(Point center, double Radius, string? id);
        void ExportLine(Point start, Point end, string? id);
        // Ломаная линия, обычно используется для отображения
        //  незамкнутых линий. Для замкнутых линий см. метод AppendPolygon
        void ExportPolyline(IEnumerable<Point> points, string? id);
        // Замкнутая фигура, состоящая из точек Points.
        void ExportPolygon(IEnumerable<Point> points, string? id);
        // Сложная фигура, состоящая из PathElement.
        void ExportPath(Formats.Path path, string? id);
    }
    
    public interface IExportFormat {
        // Заменить состояние сцены состоянием из файла 
        void LoadFrom(string destinationPath);
    }
    
    public interface IImportFormat {
        // Сохранить текущее состояние сцены в файл 
        void SaveTo(string destinationPath);
    }
}
