using Paint2.ViewModels.Interfaces;
using Paint2.Models.Figures;
using Formats;
using Formats.Json;
using Formats.Svg;
using ReactiveUI;
using Serilog;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;

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
        public void SaveScene(IExportSnapshot snapshot, string path)
        {
            // Обход объектов в глубину через стек
            var stack = new Stack<(Group, IEnumerator<ISceneObject>)>();

            foreach (var rootGroup in _groups)
            {
                snapshot.PushGroup(new Formats.DocGroup() { Name = rootGroup.Name });
                stack.Push((rootGroup, rootGroup.ChildObjects.GetEnumerator()));

                while (stack.Count > 0)
                {
                    var (currentGroup, enumerator) = stack.Peek();

                    if (enumerator.MoveNext())
                    {
                        var child = enumerator.Current;

                        if (child is Group nestedGroup) // Вложенная группа
                        {
                            snapshot.PushGroup(new Formats.DocGroup() { Name = nestedGroup.Name });
                            stack.Push((nestedGroup, nestedGroup.ChildObjects.GetEnumerator()));
                        }
                        else if (child is PathFigure pathFigure) // Фигура
                        {
                            var solidColor = pathFigure.GraphicProperties?.SolidColor ?? new(0, 0, 0, 0);
                            var borderColor = pathFigure.GraphicProperties?.BorderColor ?? new(0, 0, 0, 0);
                            var borderThickness = pathFigure.GraphicProperties?.BorderThickness ?? 0;
                            var dash = pathFigure.GraphicProperties?.BorderStyle.ToList() ?? [];
                            snapshot.Brush = new(borderColor, solidColor, borderThickness, dash);

                            PathBuilder pathBuilder = new PathBuilder(pathFigure.PathElements.ToList());

                            snapshot.AppendPath(pathBuilder.Build(), child.Coordinates);
                        }
                    }
                    else
                    {
                        stack.Pop();
                        snapshot.Pop();
                    }
                }
            }
            
            IExportFormat exportStrategy = snapshot switch
            {
                JsonSnapshot => new JsonExporter(),
                SvgSnapshot => new SvgExporter(),
                _ => new JsonExporter()
            };
            exportStrategy.SaveTo(snapshot, path);
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
