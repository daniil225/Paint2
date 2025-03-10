using Formats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint2.ViewModels.Interfaces
{
    public interface IExportSnapshot
    {
        Formats.Brush? Brush { get; set; }
        void PushGroup(DocGroup group);
        void Pop();
        void AppendRect(DocRect rect);
        void AppendCircle(DocCircle circle);
        void AppendLine(DocLine line);
        // Ломаная линия, обычно используется для отображения
        //  незамкнутых линий. Для замкнутых линий см. метод AppendPolygon
        void AppendPolyline(DocPolyline polyline);
        // Замкнутая фигура, состоящая из точек Points.
        void AppendPolygon(DocPolygon polygon);
        // Сложная фигура, состоящая из PathElement.
        // Создаётся с помощью PathBuilder
        void AppendPath(DocPath path);
    }
}
