using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint2.ViewModels.Utils
{
    public class Point
    {
        public double x, y;
        public static readonly Point Zero = new(0, 0);

        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }
    // Сюда добавить переопределение + - и * (cross и dot product если  понадобятся)
}
