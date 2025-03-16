using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using System.Composition;
using Paint2.ViewModels.Interfaces;

namespace Formats.json
{
    [Export(typeof(IJsonTypeInfoResolver))]
public class GeometryModelTypeResolver : DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var jsonTypeInfo = base.GetTypeInfo(type, options);
        
        if (jsonTypeInfo.Type == typeof(IFigure))
        {
            jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions();
            var figureTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => typeof(IFigure).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

            foreach (var figType in figureTypes)
            {
                jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(figType, figType.Name));
            }
        }

        return jsonTypeInfo;
    }
}

}

