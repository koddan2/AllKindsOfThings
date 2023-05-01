using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMapster;
/*
dotnet msbuild -t:CleanGenerated
dotnet msbuild -t:Mapster
 */
[AdaptTo("[name]Dto"), GenerateMapper]
public class MyEntity
{
    public string Name { get; set; }
    public DateTimeOffset When { get; set; }
}
[AdaptTo("[name]Dto"), GenerateMapper]
public class EntityMetadata
{
    public int Count { get; set; }
}
