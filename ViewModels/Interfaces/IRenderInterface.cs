using Avalonia.Media;
using Point = Paint2.ViewModels.Utils.Point;

namespace Paint2.ViewModels.Interfaces
{
    public interface IRenderInterface
    {
        /// <summary>
        /// Подходит для рисования линий
        /// </summary>
        /// <param name="geometry">Геометрия фигуры</param>
        /// <param name="startPoint">Начальная точка</param>
        /// <param name="endPoint">Конечная точка</param>
        void RenderLine(Geometry geometry, Point startPoint, Point endPoint);

        /// <summary>
        /// Подходит для рисования дуги(как окружности, так и эллипса),
        /// полуокружности(если указать свойство IsClosed),
        /// половины эллипса(если указать свойство IsClosed)
        /// </summary>
        /// <param name="geometry">Геометрия фигуры</param>
        /// <param name="startPoint">Начальная точка</param>
        /// <param name="endPoint">Конечная точка</param>
        /// <param name="sizeX">Размер по оси X</param>
        /// <param name="sizeY">Размер по оси Y</param>
        /// <param name="isClosed">Замкнет фигуру и у нее появится заполненная внутренность</param>
        void RenderArc(Geometry geometry, Point startPoint, Point endPoint, double sizeX, double sizeY, bool isClosed);
        
        /// <summary>
        /// Подходит для рисования любых многоугольников
        /// </summary>
        /// <param name="geometry">Геометрия фигуры</param>
        /// <param name="points">Массив точек многоугольника</param>
        void RenderPolygon(Geometry geometry, Point[] points);

        /// <summary>
        /// Подходит для рисования окружности и эллипса
        /// </summary>
        /// <param name="geometry">Геометрия фигуры</param>
        /// <param name="center">Центр фигуры</param>
        /// <param name="radiusX">Радиус по оси X</param>
        /// <param name="radiusY">Радиус по оси Y</param>
        /// <param name="transformation">Трансформация, к примеру для поворота эллипса</param>
        void RenderEllipse(Geometry geometry, Point center, double radiusX, double radiusY, Transform? transformation);

        /// <summary>
        /// Подходит для рисования кривых Безье третьего порядка
        /// </summary>
        /// <param name="geometry">Геометрия фигуры</param>
        /// <param name="point1">Первая точка</param>
        /// <param name="point2">Вторая точка</param>
        /// <param name="point3">Третья точка</param>
        /// <param name="point4">Четвертая точка</param>
        void RenderCubicBezierCurve(Geometry geometry, Point point1, Point point2, Point point3, Point point4);
    }
}
