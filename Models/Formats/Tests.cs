using System.Collections.Generic;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;

namespace Formats;

// Для тестирования своего формата нужно создать свой класс Tests,
// в котором будет создана своя реализация IFormatSnapshot и передана
// в методы ниже. Далее Snapshot должен быть сохранён в файл для 
// проверки на соответствие ожидаемому результату.
// На данный момент реализация Svg является наиболее полной и файлы,
// получаемые из тестов, могут быть использованы для сравнения.
// См. SvgExporter.cs
class Tests
{
    public static void SnapshotGeneral(IExportSnapshot snap)
    {
        var clockwise = Avalonia.Media.SweepDirection.Clockwise;
        
        snap.PushGroup(new DocGroup() { Name = "group1" });
        snap.Brush = new(new(200, 255, 255, 255), new(255, 255, 0, 0), 1);
        snap.AppendCircle(new DocCircle(new(10, 10), 5));
        snap.Brush = null;
        snap.Pop();

        snap.PushGroup(new DocGroup() { Name = "group2" });
        var pos1 = new Point(60, 60);
        var ctrl1 = new Point(65, 60);
        var pos2 = new Point(60, 50);
        var ctrl2 = new Point(65, 45);

        snap.Brush = new(new(255, 160, 160, 160), new(255, 0, 255, 0), 0.5);
        var path = new PathBuilder()
            .MoveTo(new(50, 50))
            .LineTo(new(50, 60))
            .ArcTo(5, 6, 45, false, clockwise, pos1)
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

        snap.PushGroup(new DocGroup()
        {
            Transforms = [
                new Translate(20, 20),
                    new Rotate(45),
                    new Translate(-20, -20),
                ]
        });
        snap.Brush.Dash = (2, 1);
        snap.Brush.Stroke = new(255, 255, 255, 160);
        snap.Brush.StrokeWidth = 1;
        snap.AppendRect(new DocRect(new(15, 15), 10, 10));
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
        snap.AppendPolygon(new(points)
        {
            Transforms = [
            new Translate(15, 0)
        ]
        });
        snap.Brush = null;
        snap.Pop();

        snap.Pop();
    }
    public static void SnapshotOnlyPaths(IExportSnapshot snap)
    {
        var clockwise = Avalonia.Media.SweepDirection.Clockwise;
        var counterClockwise = Avalonia.Media.SweepDirection.CounterClockwise;

        // фон
        snap.Brush = new(new(255, 255, 0, 255), new(255, 255, 255, 255), 4, null);
        snap.AppendPath(
            new PathBuilder()
                .MoveTo(new(0, 0))
                .LineTo(new(200, 0))
                .LineTo(new(200, 200))
                .LineTo(new(0, 200))
                .Close()
                .Build()
        );

        // примеры бинарных операций
        snap.Brush = new(new(255, 255, 0, 0), new(255, 180, 0, 220), 1, null);
        snap.PushGroup(new DocGroup() { Name = "group1" });
            Point interP1 = new(50, 20);
            Point interP2 = interP1 + new Point(0, 30);
            var ySpace = 35;
            snap.Brush.Fill = new(255/2, 180, 0, 220);
            // эллипс A
            snap.AppendPath(
                new PathBuilder()
                    .MoveTo(interP1)
                    .ArcTo(20, 15, 30, false, clockwise, interP2)
                    .ArcTo(20, 15, 30, true, clockwise, interP1)
                    .Close()
                    .Build()
            );
            // эллипс B
            snap.AppendPath(
                new PathBuilder()
                    .MoveTo(interP1)
                    .ArcTo(20, 15, 30, true, clockwise, interP2)
                    .ArcTo(20, 15, 30, false, clockwise, interP1)
                    .Close()
                    .Build()
            );
            // ленивая стрелка
            snap.AppendPath(
                new PathBuilder()
                    .MoveTo(new(90, 35))
                    .LineTo(new(110, 35))
                    .MoveTo(new(105, 30))
                    .LineTo(new(105, 40))
                    .Build()
            );
            snap.Brush.Fill = new(255, 180, 0, 220);
            // результат пересечения
            interP1.X += 90;
            interP2.X += 90;
            snap.AppendPath(
                new PathBuilder()
                    .MoveTo(interP1)
                    .ArcTo(20, 15, 30, false, clockwise, interP2)
                    .ArcTo(20, 15, 30, false, clockwise, interP1)
                    .Close()
                    .Build()
            );
            // результат объединения
            interP1.Y += ySpace;
            interP2.Y += ySpace;
            snap.AppendPath(
                new PathBuilder()
                    .MoveTo(interP1)
                    .ArcTo(20, 15, 30, true, clockwise, interP2)
                    .ArcTo(20, 15, 30, true, clockwise, interP1)
                    .Close()
                    .Build()
            );
            // результат A - B
            interP1.Y += ySpace;
            interP2.Y += ySpace;
            snap.AppendPath(
                new PathBuilder()
                    .MoveTo(interP1)
                    .ArcTo(20, 15, 30, false, counterClockwise, interP2)
                    .ArcTo(20, 15, 30, true, clockwise, interP1)
                    .Close()
                    .Build()
            );
            // A|B - A&B
            interP1.Y += ySpace;
            interP2.Y += ySpace;
            snap.AppendPath(
                new PathBuilder()
                    .MoveTo(interP1)
                    .ArcTo(20, 15, 30, false, counterClockwise, interP2)
                    .ArcTo(20, 15, 30, true, clockwise, interP1)
                    .Close()
                    .MoveTo(interP1)
                    .ArcTo(20, 15, 30, true, clockwise, interP2)
                    .ArcTo(20, 15, 30, false, counterClockwise, interP1)
                    .Build()
            );
        snap.Pop();
    }
    public static void SnapshotMirror(IExportSnapshot snap)
    {
        var clockwise = Avalonia.Media.SweepDirection.Clockwise;
        var counterClockwise = Avalonia.Media.SweepDirection.CounterClockwise;

        // фон
        snap.Brush = new(new(255, 255, 0, 255), new(255, 255, 255, 255), 4, null);
        snap.AppendPath(
            new PathBuilder()
                .MoveTo(new(0, 0))
                .LineTo(new(200, 0))
                .LineTo(new(200, 200))
                .LineTo(new(0, 200))
                .Close()
                .Build()
        );

        // зеркало
        snap.Brush = new(new(255, 150, 150, 255), new(0, 255, 255, 255), 2, (10, 5));
        snap.AppendPath(
            new PathBuilder()
                .MoveTo(new(100, 10))
                .LineTo(new(100, 190))
                .MoveTo(new(10, 100))
                .LineTo(new(190, 100))
                .Build()
        );

        snap.Brush = new(new(255, 255, 0, 0), new(255, 180, 0, 220), 1, null);
        snap.PushGroup(new DocGroup() { Name = "group1" });
            var p1 = new Point(50, 50);
            var p2 = p1 + new Point(40, 20);
            var angle = 45;

            // четверть IV
            snap.AppendPath(
                new PathBuilder()
                    .MoveTo(p1)
                    .ArcTo(20, 15, angle, true, clockwise, p2)
                    .Close()
                    .Build()
            );

            // I
            p1.X += 2*(100 - p1.X);
            p2.X += 2*(100 - p2.X);
            snap.AppendPath(
                new PathBuilder()
                    .MoveTo(p1)
                    .ArcTo(20, 15, -angle, true, counterClockwise, p2)
                    .Close()
                    .Build()
            );

            // II
            p1.Y += 2*(100 - p1.Y);
            p2.Y += 2*(100 - p2.Y);
            snap.AppendPath(
                new PathBuilder()
                    .MoveTo(p1)
                    .ArcTo(20, 15, angle, true, clockwise, p2)
                    .Close()
                    .Build()
            );

            // III
            p1.X += 2*(100 - p1.X);
            p2.X += 2*(100 - p2.X);
            snap.AppendPath(
                new PathBuilder()
                    .MoveTo(p1)
                    .ArcTo(20, 15, -angle, true, counterClockwise, p2)
                    .Close()
                    .Build()
            );
        snap.Pop();
    }
}
