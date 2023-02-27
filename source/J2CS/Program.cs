using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileSystemGlobbing;
#if DEBUG
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
#endif
using Xamasoft.JsonClassGenerator;
using Xamasoft.JsonClassGenerator.CodeWriterConfiguration;
using Xamasoft.JsonClassGenerator.CodeWriters;
using Xamasoft.JsonClassGenerator.Models;

namespace J2CS
{
    internal class ProgramArgs
    {
        public string Directory { get; set; } = ".";
        public string IncludeGlob { get; set; } = "*.json";
        public string? JsonConfigFile { get; set; }
    }

    internal class Program
    {
        private static readonly Dictionary<string, string>? switchMappings =
            new()
            {
                ["-d"] = "Directory",
                ["-i"] = "IncludeGlob",
                ["-c"] = "JsonConfigFile",
            };

        static int Main(string[] args)
        {
            try
            {
                return InnerMain(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }
        }

        static int InnerMain(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddCommandLine(args, switchMappings)
                .Build();
            var programArgs =
                configuration.Get<ProgramArgs>()
                ?? throw new ApplicationException("Invalid program arguments.");

            CSharpCodeWriterConfig? codeWriterConfig = null;
            if (programArgs.JsonConfigFile is string jsonConfigFile)
            {
                var configuration2 = new ConfigurationBuilder()
                    .AddJsonFile(jsonConfigFile, true)
                    .Build();
                codeWriterConfig = configuration2.Get<CSharpCodeWriterConfig>();
            }

            var cfg =
                codeWriterConfig
                ?? new CSharpCodeWriterConfig
                {
                    AlwaysUseNullables = true,
                    AttributeLibrary = JsonLibrary.SystemTextJson,
                    AttributeUsage = JsonPropertyAttributeUsage.Always,
                    CollectionType = OutputCollectionType.Array,
                    ExamplesInDocumentation = false,
                    InternalVisibility = true,
                    Namespace = "Generated",
                    NullValueHandlingIgnore = true,
                    ReadOnlyCollectionProperties = false,
                    OutputType = OutputTypes.MutableClass,
                    UsePascalCase = true,
                    UseNestedClasses = false,
                    ApplyObfuscationAttributes = false,
                    OutputMembers = OutputMembers.AsProperties,
                    SecondaryNamespace = null,
                    MainClass = null,
                };

#if DEBUG
            Console.WriteLine(
                JsonConvert.SerializeObject(
                    cfg,
                    Formatting.Indented,
                    new JsonSerializerSettings { Converters = { new StringEnumConverter() } }
                )
            );
#endif

            var writer = new JsonClassGenerator { CodeWriter = new CSharpCodeWriter(cfg), };

            Matcher matcher = new();
            matcher.AddIncludePatterns(new[] { programArgs.IncludeGlob });

            var dir =
                Path.GetFullPath(programArgs.Directory)
                ?? throw new ApplicationException(
                    $"Could not get full path to: {programArgs.Directory}"
                );
            IEnumerable<string> matchingFiles = matcher.GetResultsInFullPath(dir);
            foreach (var path in matchingFiles)
            {
                var json = File.ReadAllText(path);
                var sb = writer.GenerateClasses(json, out string error);
                if (!string.IsNullOrEmpty(error))
                {
                    throw new ApplicationException($"Conversion yielded error: {error}");
                }
                File.WriteAllText($"{path}.cs", sb.ToString());
            }

            return 0;
        }
    }
}
