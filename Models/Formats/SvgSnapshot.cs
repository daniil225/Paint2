using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Avalonia.Media;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;

namespace Formats.Svg
{
    public class SvgSnapshot : IExportSnapshot
    {
        double _width;
        double _height;
        internal XElement Tree { get; }
        XElement _currentGroup;

        const string _transparentHex = "#00000000";

        public Brush? Brush { get; set; }

        public SvgSnapshot(double width, double height)
        {
            _width = width;
            _height = height;

            Tree = new XElement("svg",
                new XAttribute("width", _width),
                new XAttribute("height", _height)
            );
            _currentGroup = Tree;
        }
        static string PointToString(Point p)
        {
            return $"{p.X},{p.Y}";
        }
        static string TransformToString(ITransform a)
        {
            if (a is Translate t)
            {
                return $"translate({t.X}, {t.Y})";
            }
            else if (a is Rotate r)
            {
                if (r.Pivot != null)
                {
                    return $"rotate({r.Angle} {PointToString(r.Pivot)})";
                }
                else
                {
                    return $"rotate({r.Angle})";
                }
            }
            else
            {
                throw new ArgumentException("Неожиданный тип преобразования: " + a.GetType());
            }
        }
        static string? TransformsToString(IEnumerable<ITransform> transforms)
        {
            // эта проверка нужна потому что Join возвращает пустую строку
            // если transforms пустая
            if (!transforms.Any())
            {
                return null;
            }

            return string.Join(" ", transforms.Select(TransformToString));
        }
        static string PathElementToString(IPathElement a)
        {
            if (a is PathMoveTo e)
            {
                var dest = e.dest;
                return $"M {PointToString(dest)}";
            }
            else if (a is PathLineTo e2)
            {
                var dest = e2.dest;
                return $"L {PointToString(dest)}";
            }
            else if (a is PathArcTo e3)
            {
                uint largeArcFlag = e3.largeArcFlag switch
                {
                    true => 1,
                    false => 0,
                };
                uint sweepFlag = e3.sweepDirection switch

                {
                    SweepDirection.Clockwise => 1,
                    SweepDirection.CounterClockwise => 0,
                    _ => throw new ArgumentException("Некорректное направление вращения"),
                };

                return $"A {e3.radiusX} {e3.radiusY} {e3.xAxisRotation} {largeArcFlag} {sweepFlag} {PointToString(e3.dest)}";
            }
            else if (a is PathCubicBezierTo e4)
            {
                return $"C {PointToString(e4.controlPoint1)} "
                    + $"{PointToString(e4.controlPoint2)} "
                    + $"{PointToString(e4.dest)}";
            }
            else if (a is PathClose e5)
            {
                return "Z";
            }
            else
            {
                throw new ArgumentException("Неожиданный фрагмент Path: " + a.GetType());
            }
        }
        static string PathElementsToString(IEnumerable<IPathElement> elements)
        {
            return string.Join(" ", elements.Select(PathElementToString));
        }
        static string ColorToHexRGB(Color c)
        {
            return $"#{c.R:X2}{c.G:X2}{c.B:X2}";
        }
        static double ColorToOpacityNormalized(Color c)
        {
            return c.A / 255D;
        }
        void ApplyBrush(XElement elem, bool applyFill = true)
        {
            if (Brush != null)
            {
                elem.SetAttributeValue("stroke-width", Brush.StrokeWidth);

                if (applyFill == true)
                {
                    elem.SetAttributeValue("fill", ColorToHexRGB(Brush.Fill));
                    elem.SetAttributeValue("fill-opacity", ColorToOpacityNormalized(Brush.Fill));
                }
                else
                {
                    elem.SetAttributeValue("fill", _transparentHex);
                    elem.SetAttributeValue("fill-opacity", _transparentHex);
                }

                elem.SetAttributeValue("stroke", ColorToHexRGB(Brush.Stroke));
                elem.SetAttributeValue("stroke-opacity", ColorToOpacityNormalized(Brush.Stroke));
                if (Brush.Dash.HasValue)
                {
                    var (Length, Gap) = Brush.Dash.Value;
                    elem.SetAttributeValue("stroke-dasharray", $"{Length} {Gap}");
                }
            }
        }
        static void SetBaseAttrs(XElement elem, DocElement docElement)
        {
            elem.SetAttributeValue("transform", TransformsToString(docElement.Transforms));
            elem.SetAttributeValue("id", docElement.Name);
        }

        public void AppendCircle(DocCircle circle)
        {
            var tmp = new XElement(
                "circle",
                new XAttribute("cx", circle.Center.X),
                new XAttribute("cy", circle.Center.Y),
                new XAttribute("r", circle.Radius)
            );
            SetBaseAttrs(tmp, circle);
            ApplyBrush(tmp);

            _currentGroup.Add(tmp);
        }

        public void AppendLine(DocLine line)
        {
            var tmp = new XElement(
                "line",
                new XAttribute("x1", line.Start.X),
                new XAttribute("y1", line.Start.Y),
                new XAttribute("x2", line.End.X),
                new XAttribute("y2", line.End.Y)
            );
            SetBaseAttrs(tmp, line);
            ApplyBrush(tmp, false);

            _currentGroup.Add(tmp);
        }

        public void AppendPath(DocPath path)
        {
            var tmp = new XElement(
                "path",
                new XAttribute("d", PathElementsToString(path.Elements))
            );
            SetBaseAttrs(tmp, path);
            ApplyBrush(tmp, path.IsClosed);

            _currentGroup.Add(tmp);
        }

        public void AppendPolygon(DocPolygon polygon)
        {
            var pointArray = string.Join(" ", polygon.Points.Select(PointToString));
            var tmp = new XElement(
                "polygon",
                new XAttribute("points", pointArray)
            );
            SetBaseAttrs(tmp, polygon);
            ApplyBrush(tmp);

            _currentGroup.Add(tmp);
        }

        public void AppendPolyline(DocPolyline polyline)
        {
            var pointArray = string.Join(" ", polyline.Points.Select(PointToString));
            var tmp = new XElement(
                "polyline",
                new XAttribute("points", pointArray)
            );
            SetBaseAttrs(tmp, polyline);
            ApplyBrush(tmp, false);

            _currentGroup.Add(tmp);
        }

        public void AppendRect(DocRect rect)
        {
            var tmp = new XElement(
                "rect",
                new XAttribute("x", rect.Position.X),
                new XAttribute("y", rect.Position.Y),
                new XAttribute("width", rect.Width),
                new XAttribute("height", rect.Height)
            );
            SetBaseAttrs(tmp, rect);
            ApplyBrush(tmp);

            _currentGroup.Add(tmp);
        }

        public void Pop()
        {
            // Pop должен быть вызван только если ранее была добавлена группа.
            _currentGroup = _currentGroup.Parent!;
        }

        public void PushGroup(DocGroup group)
        {
            var tmp = new XElement("g");
            SetBaseAttrs(tmp, group);
            // здесь нету ApplyBrush потому что мне показалось
            // что применение кисти к группе делает API не очень
            // интуитивным

            _currentGroup.Add(tmp);
            _currentGroup = tmp;
        }
    }
}
