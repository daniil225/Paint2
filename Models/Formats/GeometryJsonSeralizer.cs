using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using System.Composition;

namespace Formats.json{
[Export]
public class GeometryJsonSerializer
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    [ImportingConstructor]
    public GeometryJsonSerializer([ImportMany] IJsonTypeInfoResolver[] resolvers)
    {
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            TypeInfoResolver = JsonTypeInfoResolver.Combine(resolvers)
        };
    }

    public void SaveFigures<T>(string filename, IEnumerable<T> figures)
    {
        File.WriteAllText(filename, JsonSerializer.Serialize(figures, _jsonSerializerOptions));
    }

    public IEnumerable<T>? LoadFigures<T>(string filename)
    {
        if (!File.Exists(filename)) return null;

        return JsonSerializer.Deserialize<IEnumerable<T>>(File.ReadAllText(filename), _jsonSerializerOptions);
    }
}
}

