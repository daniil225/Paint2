using System.Text.Json.Nodes;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using System.Linq;
using System;
using Avalonia.Media;
using System.Text.Json;
using System.Collections.Generic;
using Paint2;

namespace Formats.Json
{
    public class JsonSnapshot : IExportSnapshot
    {
        const string _transparentHex = "#00000000";
        private readonly JsonArray _objects = new();
        private JsonObject? _currentGroup;

        public Brush? Brush { get; set; }

        private static string PointToString(Point p)
        {
            return $"{p.X}, {p.Y}";
        }

        private JsonObject CreateBaseObject(DocElement element)
        {
            var baseObject = new JsonObject
            {
                ["id"] = element.Name,
                ["transform"] = element.Transforms.Any() ? TransformsToString(element.Transforms) : null
            };
            return baseObject;
        }

        private static string TransformsToString(IEnumerable<ITransform> transforms)
        {
            return string.Join("; ", transforms.Select(t =>
            {
                if (t is Translate translate)
                    return $"translate({translate.X}, {translate.Y})";
                if (t is Rotate rotate)
                    return rotate.Pivot != null
                        ? $"rotate({rotate.Angle}, {rotate.Pivot.X}, {rotate.Pivot.Y})"
                        : $"rotate({rotate.Angle})";
                throw new NotSupportedException($"Unknown transform type: {t.GetType().Name}");
            }));
        }

        static string ColorToHexRGB(Color c)
        {
            return $"#{c.R:X2}{c.G:X2}{c.B:X2}";
        }
        static double ColorToOpacityNormalized(Color c)
        {
            return c.A / 255D;
        }

        private void ApplyBrush(JsonObject obj, bool applyFill = true)
        {
            if (Brush is not null)
            {
                obj["stroke-width"] = Brush.StrokeWidth.ToString();
                if (applyFill == true)
                {
                    obj["fill"] = ColorToHexRGB(Brush.Fill);
                    obj["fill-opacity"] = ColorToOpacityNormalized(Brush.Fill).ToString();
                }
                else
                {
                    obj["fill"] = _transparentHex;
                    obj["fill-opacity"] = _transparentHex;
                }

                obj["stroke"] = ColorToHexRGB(Brush.Stroke);
                obj["stroke-opacity"] = ColorToOpacityNormalized(Brush.Stroke).ToString();
                if (Brush.Dash != null)
                {
                    var dasharray = string.Join(" ", Brush.Dash.Select(d => d.ToString()));
                    obj["stroke-dasharray"] = dasharray;
                }
            }
        }

        public void PushGroup(DocGroup group)
        {
            var groupObject = CreateBaseObject(group);
            groupObject["type"] = "g";
            groupObject["children"] = new JsonArray();

            if (_currentGroup is not null)
            {
                var childrenArray = _currentGroup["children"] as JsonArray;
                childrenArray?.Add(groupObject);
            }
            else
            {
                _objects.Add(groupObject);
            }

            _currentGroup = groupObject;
        }

        public void Pop()
        {
            if (_currentGroup is null)
            {
                throw new InvalidOperationException("No group to pop.");
            }

            var parentGroup = _objects.OfType<JsonObject>().FirstOrDefault(g => (g["children"] as JsonArray)?.Contains(_currentGroup!) == true);

            _currentGroup = parentGroup;
        }

        public void AppendRect(DocRect rect)
        {
            var rectObject = CreateBaseObject(rect);
            rectObject["type"] = "rect";
            rectObject["x"] = rect.Position.X;
            rectObject["y"] = rect.Position.Y;
            rectObject["width"] = rect.Width;
            rectObject["height"] = rect.Height;
            ApplyBrush(rectObject);

            var childrenArray = _currentGroup?["children"] as JsonArray;
            if (childrenArray != null)
            {
                childrenArray.Add(rectObject);
            }
            else
            {
                _objects.Add(rectObject);
            }
        }

        public void AppendCircle(DocCircle circle)
        {
            var circleObject = CreateBaseObject(circle);
            circleObject["type"] = "circle";
            circleObject["cx"] = circle.Center.X;
            circleObject["cy"] = circle.Center.Y;
            circleObject["r"] = circle.Radius;
            ApplyBrush(circleObject);

            var childrenArray = _currentGroup?["children"] as JsonArray;
            if (childrenArray != null)
            {
                childrenArray.Add(circleObject);
            }
            else
            {
                _objects.Add(circleObject);
            }
        }

        public void AppendLine(DocLine line)
        {
            var lineObject = CreateBaseObject(line);
            lineObject["type"] = "line";
            lineObject["x1"] = line.Start.X;
            lineObject["y1"] = line.Start.Y;
            lineObject["x2"] = line.End.X;
            lineObject["y2"] = line.End.Y;
            ApplyBrush(lineObject);

            var childrenArray = _currentGroup?["children"] as JsonArray;
            if (childrenArray != null)
            {
                childrenArray.Add(lineObject);
            }
            else
            {
                _objects.Add(lineObject);
            }
        }

        public void AppendPolyline(DocPolyline polyline)
        {
            var polylineObject = CreateBaseObject(polyline);
            polylineObject["type"] = "polyline";
            polylineObject["points"] = string.Join(" ", polyline.Points.Select(PointToString));
            ApplyBrush(polylineObject);

            var childrenArray = _currentGroup?["children"] as JsonArray;
            if (childrenArray != null)
            {
                childrenArray.Add(polylineObject);
            }
            else
            {
                _objects.Add(polylineObject);
            }
        }

        public void AppendPolygon(DocPolygon polygon)
        {
            var polygonObject = CreateBaseObject(polygon);
            polygonObject["type"] = "polygon";
            polygonObject["points"] = string.Join(" ", polygon.Points.Select(PointToString));
            ApplyBrush(polygonObject);

            var childrenArray = _currentGroup?["children"] as JsonArray;
            if (childrenArray != null)
            {
                childrenArray.Add(polygonObject);
            }
            else
            {
                _objects.Add(polygonObject);
            }
        }

        public void AppendPath(DocPath path, Point _)
        {
            var pathObject = CreateBaseObject(path);
            pathObject["type"] = "path";
            pathObject["d"] = string.Join(" ", path.Elements.Select(PathElementToString));
            ApplyBrush(pathObject);

            var childrenArray = _currentGroup?["children"] as JsonArray;
            if (childrenArray != null)
            {
                childrenArray.Add(pathObject);
            }
            else
            {
                _objects.Add(pathObject);
            }
        }

        private static string PathElementToString(IPathElement element)
        {
            return element switch
            {
                PathMoveTo move => $"M {PointToString(move.dest)}",
                PathLineTo line => $"L {PointToString(line.dest)}",
                PathCubicBezierTo cubic => $"C {PointToString(cubic.controlPoint1)} " +
                                           $"{PointToString(cubic.controlPoint2)} {PointToString(cubic.dest)}",
                PathArcTo arc => $"A {arc.radiusX} {arc.radiusY} {arc.xAxisRotation} " +
                                 $"{(arc.largeArcFlag ? 1 : 0)} {(arc.sweepDirection == SweepDirection.Clockwise ? 1 : 0)} {PointToString(arc.dest)}",
                PathClose _ => "Z",
                _ => throw new NotSupportedException($"Unknown path element: {element.GetType().Name}")
            };
        }

        public string Serialize()
        {
            return _objects.ToJsonString(new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
    }
}