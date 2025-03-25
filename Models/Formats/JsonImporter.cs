using System.IO;
using System.Text.Json.Nodes;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using System.Collections.Generic;
using Paint2.ViewModels;
using Avalonia.Media;
using System.Linq;
using System;

namespace Formats.Json
{
    public class JsonImporter : IImportFormat
    {
        public void LoadFrom(string destinationPath)
        {
            var jsonContent = File.ReadAllText(destinationPath);
            var jsonObject = JsonNode.Parse(jsonContent) as JsonObject;

            if (jsonObject == null)
            {
                throw new InvalidDataException("Invalid JSON format.");
            }

            Scene scene = Scene.Current;
            scene.ResetScene();

            var objectsArray = jsonObject["objects"]?.AsArray();
            if (objectsArray == null)
            {
                return;
            }

            foreach (var objNode in objectsArray)
            {
                ImportObject(objNode.AsObject(), null);
            }
        }

        private void ImportObject(JsonObject obj, Group? parentGroup)
        {
            string? type = obj["type"]?.ToString();
            string? name = obj["id"]?.ToString();
            if (type == null || name == null)
            {
                return;
            }

            if (type == "g")
            {
                var properties = ParseGraphicProperties(obj);
                var newGroup = Scene.Current.CreateGroup(name, properties, parentGroup);
                var childrenArray = obj["children"]?.AsArray();
                if (childrenArray != null)
                {
                    foreach (var childNode in childrenArray)
                    {
                        ImportObject(childNode.AsObject(), newGroup);
                    }
                }
            }
            else
            {
                Brush brush = ParseBrush(obj);

                switch (type)
                {
                    case "path":
                        ImportPath(obj, brush, parentGroup);
                        break;
                    default:
                        throw new NotSupportedException($"Unsupported object type: {type}");
                }
            }
        }

        private void ImportPath(JsonObject obj, Brush brush, Group? parentGroup)
        {
            var pathData = obj["d"]?.ToString();
            if (string.IsNullOrWhiteSpace(pathData)) return;

            var pathBuilder = new PathBuilder();
            ParsePathData(pathData, pathBuilder);
            var newPath = pathBuilder.Build();
            var figureProperties = new FigureGraphicProperties
            {
                SolidColor = brush.Fill,
                BorderColor = brush.Stroke,
                BorderThickness = brush.StrokeWidth,
                BorderStyle = [.. brush.Dash ?? new List<double>()]
            };

            Scene.Current.CreateGroup("Path", figureProperties, parentGroup);
        }

        private Brush ParseBrush(JsonObject obj)
        {
            var fillColor = ParseColor(obj["fill"]?.ToString(), obj["fill-opacity"]?.ToString());
            var strokeColor = ParseColor(obj["stroke"]?.ToString(), obj["stroke-opacity"]?.ToString());
            var strokeWidth = double.Parse(obj["stroke-width"]?.ToString() ?? "0");

            var dashArray = obj["stroke-dasharray"]?.ToString()
                ?.Split(' ')
                ?.Select(double.Parse)
                ?.ToList();

            return new Brush(strokeColor, fillColor, strokeWidth, dashArray);
        }

        private Color ParseColor(string? hex, string? opacity)
        {
            if (hex == null || hex.Length < 7) return Colors.Transparent;

            var r = byte.Parse(hex.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
            var a = (byte)(double.Parse(opacity ?? "1.0") * 255);

            return Color.FromArgb(a, r, g, b);
        }

        private void ParsePathData(string pathData, PathBuilder pathBuilder)
        {
            if (string.IsNullOrWhiteSpace(pathData))
            {
                throw new ArgumentException("Path data cannot be null or empty.");
            } 

            var regex = new System.Text.RegularExpressions.Regex(@"([MmLlCcSsQqTtAaZz])|(-?\d*\.?\d+(?:[eE][+-]?\d+)?)");
            var matches = regex.Matches(pathData);

            char? currentCommand = null;
            List<double> args = new();

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                if (match.Groups[1].Success)
                {
                    if (currentCommand.HasValue)
                    {
                        ProcessPathCommand(currentCommand.Value, args, pathBuilder);
                        args.Clear();
                    }

                    currentCommand = match.Groups[1].Value[0];
                }
                else if (match.Groups[2].Success)
                {
                    args.Add(double.Parse(match.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture));
                }
            }

            if (currentCommand.HasValue)
            {
                ProcessPathCommand(currentCommand.Value, args, pathBuilder);
            }
        }

        private void ProcessPathCommand(char command, List<double> args, PathBuilder pathBuilder)
        {
            switch (char.ToUpper(command))
            {
                case 'M': 
                    for (int i = 0; i < args.Count; i += 2)
                    {
                        pathBuilder.MoveTo(new Point(args[i], args[i + 1]));
                    }
                    break;

                case 'L': 
                    for (int i = 0; i < args.Count; i += 2)
                    {
                        pathBuilder.LineTo(new Point(args[i], args[i + 1]));
                    }
                    break;

                case 'C':
                    for (int i = 0; i < args.Count; i += 6)
                    {
                        pathBuilder.CubicBezierTo(
                            new Point(args[i], args[i + 1]),
                            new Point(args[i + 2], args[i + 3]),
                            new Point(args[i + 4], args[i + 5])
                        );
                    }
                    break;

                case 'A':
                    for (int i = 0; i < args.Count; i += 7)
                    {
                        pathBuilder.ArcTo(
                            args[i], 
                            args[i + 1],
                            args[i + 2],
                            args[i + 3] != 0, 
                            args[i + 4] != 0 ? SweepDirection.Clockwise : SweepDirection.CounterClockwise, 
                            new Point(args[i + 5], args[i + 6]) 
                        );
                    }       
                    break;

                case 'Z':
                    pathBuilder.Close();
                    break;

                default:
                    throw new NotSupportedException($"Command {command} is not supported.");
            }
        }

        private IFigureGraphicProperties ParseGraphicProperties(JsonObject obj)
        {
            var fillColor = ParseColor(obj["fill"]?.ToString(), obj["fill-opacity"]?.ToString());
            var borderColor = ParseColor(obj["stroke"]?.ToString(), obj["stroke-opacity"]?.ToString());
            var borderThickness = double.Parse(obj["stroke-width"]?.ToString() ?? "0");

            var dashArray = obj["stroke-dasharray"]?.ToString()
                ?.Split(' ')
                ?.Select(double.Parse)
                ?.ToList();

            return new FigureGraphicProperties
            {
                SolidColor = fillColor,
                BorderColor = borderColor,
                BorderThickness = borderThickness,
                BorderStyle = [.. dashArray ?? new List<double>()]
            };
        }
    }
}