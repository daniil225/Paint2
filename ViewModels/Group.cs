using Interfaces;
using System.Collections.Generic;

namespace Paint2.ViewModels
{
    // При отрисовке по порядку проходятся все уровни иерархии групп, порядок внутри иерархии определяется порядком внутри листов
    public class Group
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
        // Тут хранятся все фигуры группы
        public IList<IFigure> figures;
        // Тут все дочерние группы
        public IList<Group> childGroups;

        private string _name;
        public Group(string name)
        {
            _name = name;
            figures = [];
            childGroups = [];
        }
    }
}