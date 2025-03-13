using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using System.Collections.Generic;

namespace Formats.Svg
{
    public class SvgExporter : IExportFormat
    {
        public void SaveTo(IExportSnapshot snapshot, string destinationPath)
        {
            // ожидается что SaveTo будет вызван с SvgSnapshot
            var svgSnap = snapshot as SvgSnapshot;
            svgSnap!.Tree.Save(destinationPath);
        }
    }

    public static partial class Tests
    {
        public static void WriteTestSvg(string savePath)
        {
            var snap = new SvgSnapshot(100, 100);

            snap.PushGroup(new DocGroup() { Name = "group1" });
                snap.Brush = new(new (200, 255, 255, 255), new(255, 255, 0, 0), 1);
                snap.AppendCircle(new DocCircle(new(10, 10), 5));
                snap.Brush = null;
            snap.Pop();

            snap.PushGroup(new DocGroup() { Name = "group2" });
                var pos1 = new Point(60, 60);
                var ctrl1 = new Point(65, 60);
                var pos2 = new Point(60, 50);
                var ctrl2 = new Point(65, 45);

                snap.Brush = new(new (255, 160, 160,160), new(255, 0, 255, 0), 0.5);
                var path = new PathBuilder()
                    .MoveTo(new(50, 50))
                    .LineTo(new(50, 60))
                    .ArcTo(5, 6, 45, false, Avalonia.Media.SweepDirection.Clockwise, pos1)
                    .CubicBezierTo(ctrl1, ctrl2, pos2)
                    .Close()
                    .Build();
                snap.AppendPath(path);

                snap.Brush = new(new(255, 0, 0, 200), new(255, 255, 0, 255), 0.2);
                snap.AppendLine(new(pos1, ctrl1));
                snap.AppendLine(new(pos2, ctrl2));

                snap.Brush = new(new(255, 0, 0, 0), new(255, 255, 0, 255), 0);
                snap.AppendCircle(new DocCircle(ctrl1, 0.5));
                snap.AppendCircle(new DocCircle(ctrl2, 0.5));

                snap.PushGroup(new DocGroup() {
                    Transforms = [
                        new Translate(20, 20),
                        new Rotate(45),
                        new Translate(-20, -20),
                    ]
                });
                    snap.Brush.Dash = (2, 1);
                    snap.Brush.Stroke = new(255, 255, 255, 160);
                    snap.Brush.StrokeWidth = 1;
                    snap.AppendRect(new DocRect(new (15, 15), 10, 10));
                snap.Pop();

                snap.PushGroup(new DocGroup());
                    snap.Brush = new(new(255, 0, 200, 0), new(255, 120, 0, 200), 1);
                    List<Point> points = [
                        new(23, 23), new(30, 20),
                        new(25, 25), new (32, 23),
                        new(25, 27), new (32, 25),
                        new(22, 34)
                    ];
                    snap.AppendPolyline(new(points));
                    snap.Brush.Fill = new(0, 120, 0, 200);
                    snap.AppendPolygon(new(points) { Transforms = [
                        new Translate(15, 0)
                    ]});
                    snap.Brush = null;
                snap.Pop();

            snap.Pop();

            var exporter = new SvgExporter();
            exporter.SaveTo(snap, savePath);
        }
    }
}
