using System.Text.Json;
using System.IO;
using System.Text.Json.Serialization;
using Paint2.ViewModels.Interfaces;
using System;
using System.Linq;
using Paint2.ViewModels.Utils;
using System.Collections.Generic;

namespace Formats.Json
{
    public class JsonImporter : IImportFormat
    {
        public void LoadFrom(string sourcePath)
        {
            var jsonString = File.ReadAllText(sourcePath);
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            
        }
    }
}

