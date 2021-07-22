using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;

namespace Entitas.CodeGeneration.Plugins {

    public class ComponentContextApiGenerator : AbstractGenerator {

        public override string name { get { return "Component (Context API)"; } }

        const string STANDARD_TEMPLATE =
            @"public partial class ${ContextType} {

    private Entitas.IGroup<${EntityType}> _${componentName}Group;

    [Entitas.CodeGeneration.Attributes.PostConstructor]
    private void Initialize${ComponentName}() {
        _${componentName}Group = GetGroup(${MatcherType}.${ComponentName});
    }    

    public ${EntityType} ${componentName}Entity { get { return _${componentName}Group.GetSingleEntity(); } }
    public ${ComponentType} ${validComponentName} { get { return ${componentName}Entity.${componentName}; } }
    public bool has${ComponentName} { get { return _${componentName}Group.count > 0; } }

    public ${EntityType} Set${ComponentName}(${newMethodParameters}) {
        if (has${ComponentName}) {
            throw new Entitas.EntitasException(""Could not set ${ComponentName}!\n"" + this + "" already has an entity with ${ComponentType}!"",
                ""You should check if the context already has a ${componentName}Entity before setting it or use context.Replace${ComponentName}()."");
        }
        var entity = CreateEntity();
        entity.Add${ComponentName}(${newMethodArgs});
        return entity;
    }

    public void Replace${ComponentName}(${newMethodParameters}) {
        var entity = ${componentName}Entity;
        if (entity == null) {
            entity = Set${ComponentName}(${newMethodArgs});
        } else {
            entity.Replace${ComponentName}(${newMethodArgs});
        }
    }

    public void Remove${ComponentName}() {
        ${componentName}Entity.Destroy();
    }
}
";

        const string FLAG_TEMPLATE =
            @"public partial class ${ContextType} {

    private Entitas.IGroup<${EntityType}> _${componentName}Group;

    [Entitas.CodeGeneration.Attributes.PostConstructor]
    private void Initialize${ComponentName}() {
        _${componentName}Group = GetGroup(${MatcherType}.${ComponentName});
    }    

    public ${EntityType} ${componentName}Entity { get { return _${componentName}Group.GetSingleEntity(); } }

    public bool ${prefixedComponentName} {
        get { return _${componentName}Group.count > 0; }
        set {
            var entity = ${componentName}Entity;
            if (value != (entity != null)) {
                if (value) {
                    CreateEntity().${prefixedComponentName} = true;
                } else {
                    entity.Destroy();
                }
            }
        }
    }
}
";

        public override CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ComponentData>()
                .Where(d => d.ShouldGenerateMethods())
                .Where(d => d.IsUnique())
                .SelectMany(generate)
                .ToArray();
        }

        CodeGenFile[] generate(ComponentData data) {
            return data.GetContextNames()
                .Select(contextName => generate(contextName, data))
                .ToArray();
        }

        CodeGenFile generate(string contextName, ComponentData data) {
            var template = data.GetMemberData().Length == 0
                ? FLAG_TEMPLATE
                : STANDARD_TEMPLATE;

            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar +
                "Components" + Path.DirectorySeparatorChar +
                data.ComponentNameWithContext(contextName).AddComponentSuffix() + ".cs",
                template.Replace(data, contextName),
                GetType().FullName
            );
        }
    }
}
