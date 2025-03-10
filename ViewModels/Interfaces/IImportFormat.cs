using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint2.ViewModels.Interfaces
{
    public interface IImportFormat
    {
        // Заменить состояние сцены состоянием из файла 
        void LoadFrom(string destinationPath);
    }
}
