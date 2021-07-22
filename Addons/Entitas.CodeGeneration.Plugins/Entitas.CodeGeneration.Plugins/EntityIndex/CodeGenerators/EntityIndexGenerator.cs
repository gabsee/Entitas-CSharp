using System.Collections.Generic;
using System.IO;
using System.Linq;

using DesperateDevs.CodeGeneration;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class EntityIndexGenerator : ICodeGenerator, IConfigurable {

        public string name { get { return "Entity Index"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();

        public Dictionary<string, string> defaultProperties { get { return _ignoreNamespacesConfig.defaultProperties; } }

        const string CLASS_TEMPLATE =
@"using Entitas;

public partial class ${ContextName}Context {

${indexCache}

    [Entitas.CodeGeneration.Attributes.PostConstructor]
    private void InitializeEntityIndices() {
${addIndices}
    }

${getIndices}
}
";

        const string INDEX_CACHE_TEMPLATE = @"    public ${IndexTypeFull} ${IndexName}Index;";

        const string ADD_INDEX_TEMPLATE = @"        ${IndexName}Index = new ${IndexTypeFull}(
            ""${IndexName}"",
            GetGroup(${ContextName}Matcher.${Matcher}),
            (e, c) => ((${ComponentType})c).${MemberName});
        AddEntityIndex(${IndexName}Index);";

        const string ADD_CUSTOM_INDEX_TEMPLATE = @"        ${IndexName}Index = new ${IndexTypeFull}(this);
        AddEntityIndex(${IndexName}Index);";

        const string GET_INDEX_TEMPLATE = @"    public System.Collections.Generic.HashSet<${ContextName}Entity> GetEntitiesWith${IndexName}(${KeyType} ${MemberName}) {
        return ${IndexName}Index.GetEntities(${MemberName});
    }";

        const string GET_PRIMARY_INDEX_TEMPLATE = @"    public ${ContextName}Entity GetEntityWith${IndexName}(${KeyType} ${MemberName}) {
        return ${IndexName}Index.GetEntity(${MemberName});
    }";

        const string CUSTOM_METHOD_TEMPLATE = @"    public ${ReturnType} ${MethodName}(${methodArgs}) {
        return ${IndexName}Index.${MethodName}(${args});
    }
";

        public void Configure(Preferences preferences) {
            _ignoreNamespacesConfig.Configure(preferences);
        }

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            try {
                var contextData = data
                    .OfType<ContextData>()
                    .OrderBy(d => d.GetContextName())
                    .ToArray();

                var entityIndexData = data
                    .OfType<EntityIndexData>()
                    .OrderBy(d => d.GetEntityIndexName())
                    .ToArray();

                List<CodeGenFile> files = new List<CodeGenFile>();
                foreach (var cd in contextData) {

                    var contextName = cd.GetContextName();
                    var indices = entityIndexData.Where(
                        d => d.GetContextNames().Contains(cd.GetContextName())
                    ).ToArray();

                    if (indices == null || indices.Length == 0)
                        continue;

                    var file = generateEntityIndices(contextName, indices);
                    if (file != null)
                        files.Add(file);
                }

                return files.ToArray();
            }
            catch (System.Exception e) {
                System.Console.WriteLine(e.ToString());
                throw;
            }
        }

        CodeGenFile generateEntityIndices(string contextName, EntityIndexData[] data) {

            var indexCache = string.Join("\n\n", data
                .Select(d => generateIndexCache(d, contextName))
                .ToArray());

            var addIndices = string.Join("\n\n", data
                .Select(d => generateAddMethod(d, contextName))
                .ToArray());

            var getIndices = string.Join("\n\n", data
                .Select(d => generateGetMethod(d, contextName))
                .ToArray());

            var fileContent = CLASS_TEMPLATE
                .Replace("${ContextName}", contextName)
                .Replace("${indexCache}", indexCache)
                .Replace("${addIndices}", addIndices)
                .Replace("${getIndices}", getIndices);

            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar + contextName.AddContextSuffix() + "Indices.cs",
                fileContent,
                GetType().FullName
            );
        }

        string generateIndexCache(EntityIndexData data, string contextName) {

            return INDEX_CACHE_TEMPLATE
                .Replace("${IndexTypeFull}", data.GetEntityIndexTypeFull(contextName))
                .Replace("${ContextName}", contextName)
                .Replace("${IndexName}", data.GetEntityIndexNameUnique());
        }

        string generateAddMethod(EntityIndexData data, string contextName) {
            return data.IsCustom()
                ? generateCustomMethods(data, contextName)
                : generateMethods(data, contextName);
        }

        string generateCustomMethods(EntityIndexData data, string contextName) {
            return ADD_CUSTOM_INDEX_TEMPLATE
                .Replace("${ContextName}", contextName)
                .Replace("${IndexName}", data.GetEntityIndexNameUnique())
                .Replace("${IndexTypeFull}", data.GetEntityIndexTypeFull(contextName));
        }

        string generateMethods(EntityIndexData data, string contextName) {
            return ADD_INDEX_TEMPLATE
                .Replace("${ContextName}", contextName)
                .Replace("${IndexName}", data.GetEntityIndexNameUnique())
                .Replace("${IndexTypeFull}", data.GetEntityIndexTypeFull(contextName))
                .Replace("${Matcher}", data.GetEntityIndexName())
                .Replace("${ComponentType}", data.GetComponentType())
                .Replace("${MemberName}", data.GetMemberName());
        }

        string generateGetMethod(EntityIndexData data, string contextName) {
            if (data.GetEntityIndexType() == "Entitas.EntityIndex") {
                return GET_INDEX_TEMPLATE
                    .Replace("${ContextName}", contextName)
                    .Replace("${IndexName}", data.GetEntityIndexNameUnique())
                    .Replace("${KeyType}", data.GetKeyType())
                    .Replace("${MemberName}", data.GetMemberName());
            } else if (data.GetEntityIndexType() == "Entitas.PrimaryEntityIndex") {
                return GET_PRIMARY_INDEX_TEMPLATE
                    .Replace("${ContextName}", contextName)
                    .Replace("${IndexName}", data.GetEntityIndexNameUnique())
                    .Replace("${KeyType}", data.GetKeyType())
                    .Replace("${MemberName}", data.GetMemberName());
            } else {
                return getCustomMethods(data, contextName);
            }
        }

        string getCustomMethods(EntityIndexData data, string contextName) {
            return string.Join("\n", data.GetCustomMethods()
                .Select(m => CUSTOM_METHOD_TEMPLATE
                    .Replace("${ContextName}", contextName)
                    .Replace("${IndexName}", data.GetEntityIndexNameUnique())
                    .Replace("${ReturnType}", m.returnType)
                    .Replace("${MethodName}", m.methodName)
                    .Replace("${methodArgs}", string.Join(", ", m.parameters.Select(p => p.type + " " + p.name).ToArray()))
                    .Replace("${args}", string.Join(", ", m.parameters.Select(p => p.name).ToArray()))).ToArray());
        }
    }
}
