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
using Formats;


class FigureMetadata
{
    public string Name { get; init; }
}
public interface IFigureCreator
{
    IFigure Create(Group parentGroup, Point[] coordinates, IFigureGraphicProperties? figureGraphicProperties = null);
    IFigure Create(Group parentGroup, Point coordinates, IList<IPathElement> pathElements, string name);
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

    public static IEnumerable<string> AvailableFigures => info.AvailableFigures.Select(f => f.Metadata.Name);
    public static IFigure? CreateFigure(string FigureType, Group parentGroup, Point[] coordinates, IFigureGraphicProperties? figureGraphicProperties = null)
    {
        try
        {
            IFigure newFigure = info.AvailableFigures.First(f => f.Metadata.Name == FigureType).Value.Create(parentGroup, coordinates, figureGraphicProperties);
            return newFigure;
        }
        catch
        {
            Log.Error($"Фигуры {FigureType} не существует");
            return null;
        }
    }
    public static IFigure LoadFigure(string FigureName, Group parentGroup, Point coordinates, IList<IPathElement> elements)
    {
        IFigure newFigure = info.AvailableFigures.First(f => f.Metadata.Name == "Figure").Value.Create(parentGroup, coordinates, elements, FigureName);
        return newFigure;
    }
}
