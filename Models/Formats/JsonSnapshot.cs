using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using System;

namespace Formats.Json
{
    public class JsonSnapshot : IExportSnapshot
    {
        public Brush? Brush { get; set; }

        private readonly List<JsonGroup> _tree = new();
        private JsonGroup? _currentGroup;
        private int _groupLevel = 0;

        public JsonSnapshot()
        {
            _currentGroup = new JsonGroup();
            _tree.Add(_currentGroup);
        }

        public string ToJson()
        {
            var options = new JsonSerializerOptions 
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull 
            };
            return JsonSerializer.Serialize(_tree, options);
        }
        public void PushGroup(DocGroup group)
        {
            var newGroup = new JsonGroup
            {
                Name = group.Name,
                Transforms = group.Transforms
            };

            if (_currentGroup != null)
            {
                _currentGroup.Children.Add(newGroup);
                newGroup.Parent = _currentGroup;
            }

            _currentGroup = newGroup;
            _groupLevel++;
        }

        public void Pop()
        {
            if (_currentGroup?.Parent != null)
            {
                _currentGroup = _currentGroup.Parent;
            }
            else
            {
                throw new InvalidOperationException("Попытка вызвать Pop() при отсутствии текущей группы.");
            }
        }

        public void AppendRect(DocRect rect)
        {
            var jsonRect = new JsonRect
            {
                X = rect.Position.X,
                Y = rect.Position.Y,
                Width = rect.Width,
                Height = rect.Height,
                Name = rect.Name,
                Transforms = rect.Transforms,
                Brush = Brush
            };

            _currentGroup?.Children.Add(jsonRect);
        }

        public void AppendCircle(DocCircle circle)
        {
            var jsonCircle = new JsonCircle
            {
                X = circle.Center.X,
                Y = circle.Center.Y,
                Radius = circle.Radius,
                Name = circle.Name,
                Transforms = circle.Transforms,
                Brush = Brush
            };

            _currentGroup?.Children.Add(jsonCircle);
        }

        public void AppendLine(DocLine line)
        {
            var jsonLine = new JsonLine
            {
                X1 = line.Start.X,
                Y1 = line.Start.Y,
                X2 = line.End.X,
                Y2 = line.End.Y,
                Name = line.Name,
                Transforms = line.Transforms,
                Brush = Brush
            };

            _currentGroup?.Children.Add(jsonLine);
        }

        public void AppendPolyline(DocPolyline polyline)
        {
            var jsonPolyline = new JsonPolyline
            {
                Points = polyline.Points,
                Name = polyline.Name,
                Transforms = polyline.Transforms,
                Brush = Brush
            };

            _currentGroup?.Children.Add(jsonPolyline);
        }

        public void AppendPolygon(DocPolygon polygon)
        {
            var jsonPolygon = new JsonPolygon
            {
                Points = polygon.Points,
                Name = polygon.Name,
                Transforms = polygon.Transforms,
                Brush = Brush
            };

            _currentGroup?.Children.Add(jsonPolygon);
        }

        public void AppendPath(DocPath path)
        {
            var jsonPath = new JsonPath
            {
                Elements = path.Elements,
                Name = path.Name,
                Transforms = path.Transforms,
                Brush = Brush
            };

            _currentGroup?.Children.Add(jsonPath);
        }
    }

    public abstract class JsonElement
    {
        public string? Name { get; set; }
        public IEnumerable<ITransform>? Transforms { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Formats.Brush? Brush { get; set; }
    }

    public class JsonGroup : JsonElement
    {
        public List<JsonElement> Children { get; set; } = new();
        [JsonIgnore]
        public JsonGroup? Parent { get; set; }
    }

    public class JsonRect : JsonElement
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }

    public class JsonCircle : JsonElement
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Radius { get; set; }
    }

    public class JsonLine : JsonElement
    {
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
    }

    public class JsonPolyline : JsonElement
    {
        public IEnumerable<Point>? Points { get; set; }
    }

    public class JsonPolygon : JsonElement
    {
        public IEnumerable<Point> Points { get; set; }
    }

    public class JsonPath : JsonElement
    {
        public IEnumerable<IPathElement>? Elements { get; set; }
    }
}