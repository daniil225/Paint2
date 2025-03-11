using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint2.ViewModels.Interfaces
{
    public interface IExportFormat
    {
        // Сохранить текущее состояние сцены в файл 
        void SaveTo(IExportSnapshot snapshot, string destinationPath);
    }
}
