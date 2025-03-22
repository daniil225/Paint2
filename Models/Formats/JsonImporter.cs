using System.Text.Json;
using System.IO;
using System.Text.Json.Serialization;
using Paint2.ViewModels.Interfaces;
using System;
using System.Linq;
using Paint2.ViewModels.Utils;
using System.Collections.Generic;

namespace Formats.Json
{
    public class JsonImporter : IImportFormat
    {
        public void LoadFrom(string sourcePath)
        {
            var jsonString = File.ReadAllText(sourcePath);
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            
            var jsonTree = JsonSerializer.Deserialize<List<JsonGroup>>(jsonString, options);

            if (jsonTree == null || !jsonTree.Any())
            {
                throw new ArgumentException("Невозможно загрузить данные из JSON, файл пуст или поврежден.");
            }
            var snapshot = new JsonSnapshot();
            RestoreJsonGroup(snapshot, jsonTree);
        }

        private void RestoreJsonGroup(JsonSnapshot snapshot, List<JsonGroup> groups)
        {
            foreach (var group in groups)
            {
                var docGroup = new DocGroup
                {
                    Name = group.Name,
                    Transforms = group.Transforms ?? new List<ITransform>()
                };
                snapshot.PushGroup(docGroup);

                // закомментированно, потому что это не собирается
                // после недавних изменений
                #if false
                foreach (var element in group.Children)
                {
                    // Восстанавливаем элементы внутри группы
                    switch (element)
                    {
                        case JsonRect rect:
                            snapshot.AppendRect(new DocRect(
                                new Point(rect.X, rect.Y),
                                rect.Width, rect.Height)
                            {
                                Name = rect.Name,
                                Transforms = rect.Transforms
                            });
                            break;

                        case JsonCircle circle:
                            snapshot.AppendCircle(new DocCircle(
                                new Point(circle.X, circle.Y),
                                circle.Radius)
                            {
                                Name = circle.Name,
                                Transforms = circle.Transforms
                            });
                            break;

                        case JsonLine line:
                            snapshot.AppendLine(new DocLine(
                                new Point(line.X1, line.Y1),
                                new Point(line.X2, line.Y2))
                            {
                                Name = line.Name,
                                Transforms = line.Transforms
                            });
                            break;

                        case JsonPolyline polyline:
                            snapshot.AppendPolyline(new DocPolyline(
                                polyline.Points ?? Enumerable.Empty<Point>())
                            {
                                Name = polyline.Name,
                                Transforms = polyline.Transforms
                            });
                            break;

                        case JsonPolygon polygon:
                            snapshot.AppendPolygon(new DocPolygon(
                                polygon.Points ?? Enumerable.Empty<Point>())
                            {
                                Name = polygon.Name,
                                Transforms = polygon.Transforms
                            });
                            break;

                        case JsonPath path:
                            snapshot.AppendPath(new DocPath(path.Elements ?? Enumerable.Empty<IPathElement>())
                            {
                                Name = path.Name,
                                Transforms = path.Transforms
                            });
                            break;

                        case JsonGroup childGroup:
                            // Восстанавливаем вложенную группу
                            RestoreJsonGroup(snapshot, new List<JsonGroup> { childGroup });
                            break;

                        default:
                            throw new ArgumentException($"Неизвестный тип элемента: {element.GetType()}");
                    }
                }
                #endif

                if (group.Brush != null)
                {
                    // Если у группы есть Brush, применяем его
                    snapshot.Brush = group.Brush;
                }

                snapshot.Pop();
            }
        }
    }
}
