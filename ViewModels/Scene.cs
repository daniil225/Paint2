using Paint2.ViewModels.Interfaces;
using System;
using System.Collections.Generic;

namespace Paint2.ViewModels
{
    public static class Scene
    {
        // Тут хранятся все группы сцены. Корневой группы явно нет, по сути сама сцена ей является
        public static IList<Group> Groups { get; private set; }
        public static IImportFormat ImportStrategy { get; set; }
        public static IExportFormat ExportStrategy { get; set; }

        static Scene()
        {
            Groups = [];
            //FileStrategy = new SVGStrategy();
        }
        // Не помню какие именно тут параметры нужны были
        public static void SaveScene(IExportSnapshot snapshort, string path)
        {
            ExportStrategy.SaveTo(snapshort, path);
        }
        public static void LoadScene(string path)
        {
            ImportStrategy.LoadFrom(path);
        }
        public static void ResetScene()
        {
            Groups.Clear();
        }
    }
}
