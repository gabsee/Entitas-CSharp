﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class TestEntity {

    static readonly My.Namespace.MyNamespaceFlagComponent myNamespaceMyNamespaceFlagComponent = new My.Namespace.MyNamespaceFlagComponent();

    public bool isMyNamespaceMyNamespaceFlag {
        get { return HasComponent(TestComponentsLookup.MyNamespaceMyNamespaceFlag); }
        set {
            if (value != isMyNamespaceMyNamespaceFlag) {
                var index = TestComponentsLookup.MyNamespaceMyNamespaceFlag;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.TryTake(out Entitas.IComponent comp) 
                            ? comp
                            : myNamespaceMyNamespaceFlagComponent;

                    AddComponent(index, component);
                } else {
                    RemoveComponent(index);
                }
            }
        }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class TestMatcher {

    static Entitas.IMatcher<TestEntity> _matcherMyNamespaceMyNamespaceFlag;

    public static Entitas.IMatcher<TestEntity> MyNamespaceMyNamespaceFlag {
        get {
            if (_matcherMyNamespaceMyNamespaceFlag == null) {
                var matcher = (Entitas.Matcher<TestEntity>)Entitas.Matcher<TestEntity>.AllOf(TestComponentsLookup.MyNamespaceMyNamespaceFlag);
                matcher.componentNames = TestComponentsLookup.componentNames;
                _matcherMyNamespaceMyNamespaceFlag = matcher;
            }

            return _matcherMyNamespaceMyNamespaceFlag;
        }
    }
}
