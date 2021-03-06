//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class TestEntity {

    public NameAgeComponent nameAge { get { return (NameAgeComponent)GetComponent(TestComponentsLookup.NameAge); } }
    public bool hasNameAge { get { return HasComponent(TestComponentsLookup.NameAge); } }

    public void AddNameAge(string newName, int newAge) {
        var index = TestComponentsLookup.NameAge;
        var component = (NameAgeComponent)CreateComponent(index, typeof(NameAgeComponent));
        component.name = newName;
        component.age = newAge;
        AddComponent(index, component);
    }

    public void ReplaceNameAge(string newName, int newAge) {
        var index = TestComponentsLookup.NameAge;
        var component = (NameAgeComponent)CreateComponent(index, typeof(NameAgeComponent));
        component.name = newName;
        component.age = newAge;
        ReplaceComponent(index, component);
    }

    public void RemoveNameAge() {
        RemoveComponent(TestComponentsLookup.NameAge);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiInterfaceGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class TestEntity : INameAgeEntity { }

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class TestMatcher {

    static Entitas.IMatcher<TestEntity> _matcherNameAge;

    public static Entitas.IMatcher<TestEntity> NameAge {
        get {
            if (_matcherNameAge == null) {
                var matcher = (Entitas.Matcher<TestEntity>)Entitas.Matcher<TestEntity>.AllOf(TestComponentsLookup.NameAge);
                matcher.componentNames = TestComponentsLookup.componentNames;
                _matcherNameAge = matcher;
            }

            return _matcherNameAge;
        }
    }
}
