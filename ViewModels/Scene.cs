using Paint2.ViewModels.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Paint2.ViewModels
{
    public class Scene
    {
        public static Scene Current { get; private set; }
        // Тут хранятся все группы сцены. Корневой группы явно нет, по сути сама сцена ей является
        public IList<Group> Groups { get; private set; }
        public IImportFormat ImportStrategy { get; set; }
        public IExportFormat ExportStrategy { get; set; }
        public ObservableCollection<GeometryViewModel> RenderedFigures { get; private set; }

        Scene(ObservableCollection<GeometryViewModel> renderedFigures)
        {
            Groups = [];
            RenderedFigures = renderedFigures;
        }

        // Не помню какие именно тут параметры нужны были
        public void SaveScene(IExportSnapshot snapshort, string path)
        {
            ExportStrategy.SaveTo(snapshort, path);
        }
        public void LoadScene(string path)
        {
            ImportStrategy.LoadFrom(path);
        }
        public static void CreateScene(ObservableCollection<GeometryViewModel> renderedFigures)
        {
            Current = new Scene(renderedFigures);
        }
        public Group CreateGroup(string name, IFigureGraphicProperties graphicProperties, Group? parentGroup = null)
        {
            Group newGroup = new(name, graphicProperties);
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
        public void RemoveObject(ISceneObject sceneObject)
        {
            if (sceneObject.Parent is null)
                Groups.Remove((Group)sceneObject);
            else
                sceneObject.Parent.childObjects.Remove(sceneObject);

            if (sceneObject is IFigure fig)
            {
                RenderedFigures.Remove(RenderedFigures.First(fig => fig.Figure == sceneObject));
            }
        }
        public void ResetScene()
        {
            Groups.Clear();
        }
    }
}
