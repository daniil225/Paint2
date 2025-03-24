using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint2.ViewModels.Utils
{
    public class Point
    {
        public static readonly Point Zero = new(0, 0);

        public double X;
        public double Y;

        public Point Normalize()
        {
            double magnitude = Magnitude();
            double newX = X / magnitude;
            double newY = Y / magnitude;
            return new Point(newX, newY);
        }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Point(Point other)
        {
            X = other.X;
            Y = other.Y;
        }

        public double Magnitude() => Math.Sqrt(X * X + Y * Y);
        
        public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y);
        public static Point operator -(Point a) => new(-a.X, -a.Y);
        public static Point operator -(Point a, Point b) => new(a.X - b.X, a.Y - b.Y);
        public static Point operator *(Point a, double coef) => new(a.X * coef, a.Y * coef);
        public static Point operator *(double coef, Point a) => new(a.X * coef, a.Y * coef);
        public static double operator *(Point a, Point b) => a.X * b.X + a.Y * b.Y;
        public static Point operator /(Point a, double coef) => new(a.X / coef, a.Y / coef);
    }
    // Сюда добавить переопределение + - и * (cross и dot product если  понадобятся)
}