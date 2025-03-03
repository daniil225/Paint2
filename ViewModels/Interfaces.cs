using Avalonia.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    
    public struct Brush
    {
        public Color Stroke;
        // Если фигура не может иметь цвета заливки, это поле игнорируется
        public Color Fill;
        public double StrokeWidth;
        // Length - длина каждого штриха
        // Gap - расстояние между штрихами
        // для сплошных линий рекомендуется использовать null вместо
        // нулевого значения Gap
        public (double Length, double Gap)? Dash;
    }

    public interface PathElement;
    public struct PathMoveTo : PathElement
    {
        Point point;
    }
    public struct PathLineTo : PathElement
    {
        Point point;
    }
    public struct PathEllipticArcCurve : PathElement
    {
        double radiusX;
        double radiusY;
        // угол вращения системы координат кривой
        // по часовой стрелке в градусах
        bool xAxisRotation;
        // из 4 возможных дуг выбирать из пары наибольших дуг
        bool largeArcFlag;
        // из 4 возможных дуг выбирать пару дуг с
        // направлением по часовой стрелке
        // подробнее о флагах: https://www.w3.org/TR/SVG11/paths.html#PathDataEllipticalArcCommands
        bool sweepFlag;
        Point endPoint;
    }
    public struct PathCubicBezierCurve : PathElement
    {
        Point controlPoint1;
        Point controlPoint2;
        Point endPoint;
    }
    public struct PathClose : PathElement {}

    public interface GroupTransform;
    public struct GroupTranslate
    {
        double x;
        double y;
    }

    public struct GroupRotate
    {
        // Угол вращения системы координат относительно
        // начала координат по часовой стрелке в градусах.
        double angle;
    }

    // Интерфейс для полного описания векторного документа,
    //  позволяющий его дальнейшую отрисовку или сохранение
    //  в файл.
    // Некоторые реализации могут реализовать часть
    //  методов пустым телом метода.
    // Порядок вызова методов влияет на порядок отрисовки.
    // Фигуры, добавленные последними, будут отрисованы поверх остальных
    public interface IRenderInterface
    {
        // Метод должен быть вызван ровно один раз 
        //  перед заданием сцены.
        void PushScene(double width, double height, string? id);
        // Добавить в дерево сцены группу объектов.
        // Последующие вызовы Append*, до вызова Pop, будут
        //  присоединять узлы в последную группу.
        void PushGroup(string? id, GroupTransform transform);
        // Position - координата левого верхнего угла прямоугольника
        void AppendRect(Point position, double width, double height, Brush brush, string? id);
        void AppendCircle(Point center, double Radius, Brush brush, string? id);
        void AppendLine(Point start, Point end, Brush brush, string? id);
        // Ломаная линия, обычно используется для отображения
        //  незамкнутых линий. Для замкнутых линий см. метод AppendPolygon
        void AppendPolyline(IEnumerable<Point> points, Brush brush, string? id);
        // Замкнутая фигура, состоящая из точек Points.
        void AppendPolygon(IEnumerable<Point> points, Brush brush, string? id);
        // Сложная фигура, состоящая из PathElement.
        // Каждый элемент (PathElement) задаётся относительно предыдущего
        //  элемента.
        void AppendPath(IEnumerable<PathElement> elements, Brush brush, string? id);
        // "Закрывает" последний контейнер.
        // Каждый контейнер должен быть закрыт ровно один раз.
        // Вызов Append* методов после закрытия сцены является ошибкой.
        void Pop();
    }

    public interface IFigure
    {
        double Width { get; }
        double Height { get; }
        string Name { get; }

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

        void SetParameters(IDictionary<string, double> doubleParams, IDictionary<string, Point> pointParams);
    }

}
