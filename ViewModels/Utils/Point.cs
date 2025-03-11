using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint2.ViewModels.Utils
{
    public class Point (double x, double y)
    {
        public static readonly Point Zero = new(0, 0);
        public double X = x;
        public double Y = y;
    }
    // Сюда добавить переопределение + - и * (cross и dot product если  понадобятся)
}
