using Avalonia.Media;
using System.Collections.Generic;
using Paint2.ViewModels.Utils;
using Paint2.ViewModels.Interfaces;
using Serilog;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System;
using System.Linq;

namespace Paint2.ViewModels
{
    public class Group : ISceneObject
    {
        // Видимое имы группы
        public string Name
        {
            get => _name;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    _name = value;
            }
        }
        public Group? Parent
        {
            get => _parentGroup;
            set
            {
                if (value is null)
                {
                    if (_parentGroup is null)
                        return;
                    else
                    {
                        _parentGroup._childObjects.Remove(this);
                        Scene.Current.AddGroupToRoot(this);
                        _parentGroup = null;
                    }
                } 
                else
                {
                    if (_parentGroup is null)
                    {
                        Scene.Current.RemoveGroupFromRoot(this);
                        _parentGroup = value;
                        _parentGroup._childObjects.Add(this);
                    }
                    else
                    {
                        _parentGroup._childObjects.Remove(this);
                        _parentGroup = value;
                        _parentGroup._childObjects.Add(this);
                    }
                }
                Scene.Current.OnHierarchyChanged();
            }
        }
        public IReadOnlyList<ISceneObject> ChildObjects { get => _childObjects.AsReadOnly(); }
        public Point Coordinates
        {
            get
            {
                int count = ChildObjects.Count;
                if (count == 0)
                    return new Point(0, 0);

                Point sum = ChildObjects.Aggregate(new Point(0, 0), (acc, obj) => acc + obj.Coordinates);
                return sum / count;
            }
            private set { }
        }
        public float Angle { get; private set; }
        public Geometry Geometry { get; set; } // для группы это свойство по идеи не должно использоваться
        public bool IsActive { get; set; }
        public bool IsMirrored { get; set; }
        public IFigureGraphicProperties GraphicProperties { get; set; }

        private string _name;
        private Group? _parentGroup;
        private List<ISceneObject> _childObjects;

        public Group(string name, IFigureGraphicProperties graphicProperties)
        {
            _name = name;
            Coordinates = Point.Zero;
            Angle = 0f;
            _childObjects = [];
            IsActive = true;
            IsMirrored = false;
            GraphicProperties = graphicProperties;
        }
        public void SetIfParent(ISceneObject child, bool isParent)
        {
            if (isParent)
            {
                _childObjects.Add(child);
            }
            else if (_childObjects.Contains(child))
            {
                _childObjects.Remove(child);
            }
        }
        public void MoveObjectInsideGroup(int newId, ISceneObject movedObject)
        {
            if (!_childObjects.Contains(movedObject))
                Log.Error($"Попытка переместить объект {movedObject.Name} внутри группы {Name}, но объект не является дочерним");
            else
            {
                int oldId = _childObjects.IndexOf(movedObject);
                _childObjects[oldId] = null;
                if (newId != _childObjects.Count - 1)
                    _childObjects.Insert(newId, movedObject);
                else
                    _childObjects.Add(movedObject);
                _childObjects.RemoveAll((item) => item is null);
            }
            Scene.Current.OnHierarchyChanged();
        }
        public void Move(Point vector)
        {
            foreach (ISceneObject obj in ChildObjects)
                obj.Move(vector);
        }

        public void Rotate(double angle, Point? Center = null)
        {

            Point actualCenter = Center ?? Coordinates;
            foreach (ISceneObject obj in ChildObjects)
                obj.Rotate(angle, actualCenter);
        }

        public void Scale(double sx, double sy, Point? Center = null)
        {
            Point actualCenter = Center ?? Coordinates;
            foreach (ISceneObject obj in ChildObjects)
                obj.Scale(sx, sy, actualCenter);
        }

        public void Scale(double rad, Point? Center = null)
        {
            Point actualCenter = Center ?? Coordinates;
            foreach (ISceneObject obj in ChildObjects)
                obj.Scale(rad, actualCenter);
        }

        public void Mirror(Point ax1, Point ax2)
        {
            foreach (ISceneObject obj in ChildObjects)
                obj.Mirror(ax1, ax2);
            IsMirrored = !IsMirrored;
        }
        public void MirrorHorizontal()
        {
            Mirror(Coordinates, Coordinates + new Point(1, 0));
        }
        public void MirrorVertical()
        {
            Mirror(Coordinates, Coordinates + new Point(0, 1));
        }
    }
}