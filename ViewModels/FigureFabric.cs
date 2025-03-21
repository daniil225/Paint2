using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Linq;
using Paint2.Models.Figures;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels;
using Point = Paint2.ViewModels.Utils.Point;
using Serilog;


class FigureMetadata
{
    public string Type { get; init; }
}
public interface IFigureCreator
{
    IFigure Create(Group parentGroup, ICollection<Point> coordinatePoints);
}

public static class FigureFabric
{
    class ImportInfo
    {
        [ImportMany]
        public IEnumerable<Lazy<IFigureCreator, FigureMetadata>> AvailableFigures { get; set; } = [];
    }
    static ImportInfo info;
    static FigureFabric()
    {
        var assemblies = new[] { typeof(PathFigure).Assembly };
        var conf = new ContainerConfiguration();
        try
        {
            conf = conf.WithAssemblies(assemblies);
        }
        catch (Exception e)
        {
            Log.Error($"Не удалось получить доступные фигуры: {e}");
        }

        var cont = conf.CreateContainer();
        info = new ImportInfo();
        cont.SatisfyImports(info);
    }

    public static IEnumerable<string> AvailableFigures => info.AvailableFigures.Select(f => f.Metadata.Type);
    public static IFigure? CreateFigure(string FigureType, Group parentGroup, Point coordinates)
    {
        ICollection<Point> coordinatePoints = [ coordinates ];
        try
        {
            IFigure newFigure = info.AvailableFigures.First(f => f.Metadata.Type == FigureType).Value.Create(parentGroup, coordinatePoints);
            return newFigure;
        }
        catch
        {
            Log.Error($"Фигуры {FigureType} не существует");
            return null;
        }
    }
    public static IFigure? CreateBezier(Group parentGroup, ICollection<Point> points)
    {
        IFigure newBezier = info.AvailableFigures.First(f => f.Metadata.Type == "Bezier").Value.Create(parentGroup, points);
        return newBezier;
    }
}
