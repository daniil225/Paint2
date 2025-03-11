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
        public static Group CreateGroup(string name, Group? parentGroup = null)
        {
            Group newGroup = new(name);
            if (parentGroup is null) // Если в топ иерархии
            {
                Groups.Add(newGroup);
            } 
            else
            {
                newGroup.Parent = parentGroup;
            }
            return newGroup;
        }
        public static void RemoveObject(ISceneObject sceneObject)
        {
            if (sceneObject.Parent is null)
                Groups.Remove((Group)sceneObject);
            else
                sceneObject.Parent.childObjects.Remove(sceneObject);
        }
        public static void ResetScene()
        {
            Groups.Clear();
        }
    }
}
