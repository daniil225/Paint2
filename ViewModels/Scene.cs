using Paint2.ViewModels.Interfaces;
using ReactiveUI;
using Serilog;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Paint2.ViewModels
{
    public class Scene : ReactiveObject
    {
        public event PropertyChangedEventHandler HierarchyChanged;
        
        public static Scene Current { get; private set; }
        // Тут хранятся все группы сцены. Корневой группы явно нет, по сути сама сцена ей является
        public IImportFormat ImportStrategy { get; set; }
        public IExportFormat ExportStrategy { get; set; }
        public IReadOnlyList<Group> Groups { get => _groups.AsReadOnly(); }

        private List<Group> _groups { get; set; }

        Scene()
        {
            _groups = [];
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
        public void MoveGroupInsideRoot(int newId, Group group)
        {
            if (!_groups.Contains(group))
                Log.Error($"Попытка переместить объект {group.Name} внутри корня, но объект не внутри корня");
            else
            {
                int oldId = _groups.IndexOf(group);
                _groups[oldId] = null;
                _groups.RemoveAll((item) => item is null);
                if (newId != _groups.Count - 1)
                    _groups.Insert(newId, group);
                else
                    _groups.Add(group);
                _groups.RemoveAll((item) => item is null);
            }
        }
        public void RemoveObject(ISceneObject sceneObject)
        {
            if (sceneObject.Parent is null)
                _groups.Remove((Group)sceneObject);
            else
                sceneObject.Parent.SetIfParent(sceneObject, false);
            OnHierarchyChanged();
        }
        public void ResetScene()
        {
            _groups.Clear();
            OnHierarchyChanged();
        }
        public void OnHierarchyChanged([CallerMemberName] string prop = "")
        {
            HierarchyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
