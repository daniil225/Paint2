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
        public delegate void HierarchyUpdateDeligate(IList<ISceneObject> groups);
        public HierarchyUpdateDeligate OnHierarcyUpdate;
        public static Scene Current { get; private set; }
        // Тут хранятся все группы сцены. Корневой группы явно нет, по сути сама сцена ей является
        public IImportFormat ImportStrategy { get; set; }
        public IExportFormat ExportStrategy { get; set; }
        public IReadOnlyList<Group> Groups { get => _groups.AsReadOnly(); }

        private IList<Group> _groups { get; set; }

        Scene()
        {
            _groups = [];
            CreateScene();
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
        public static void CreateScene()
        {
            Current = new Scene();
        }
        public Group CreateGroup(string name, IFigureGraphicProperties graphicProperties, Group? parentGroup = null)
        {
            Group newGroup = new(name, graphicProperties);
            if (parentGroup is null) // Если в топ иерархии
            {
                _groups.Add(newGroup);
            } 
            else
            {
                newGroup.Parent = parentGroup;
            }
            return newGroup;
        }
        public void AddGroupToRoot(Group group)
        {
            Current._groups.Add(group);
        }
        public void RemoveGroupFromRoot(Group group)
        {
            Current._groups.Remove(group);
        }
        public void RemoveObject(ISceneObject sceneObject)
        {
            if (sceneObject.Parent is null)
                _groups.Remove((Group)sceneObject);
            else
                sceneObject.Parent.childObjects.Remove(sceneObject);
            TriggerHeirarchyRebuild();
        }
        public void TriggerHeirarchyRebuild() => OnHierarcyUpdate.Invoke([.. _groups]);
        public void ResetScene()
        {
            _groups.Clear();
            TriggerHeirarchyRebuild();
        }
    }
}
