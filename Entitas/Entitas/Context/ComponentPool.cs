using System;
using System.Collections.Generic;

namespace Entitas {
    public static class ComponentPool {

        private static Dictionary<Type, WeakReference> _componentPools;

        public static Stack<IComponent> Get<TComponent>() where TComponent : IComponent {
            return Get(typeof(TComponent));
        }

        public static Stack<IComponent> Get(Type componentType) {
            if (_componentPools == null) {
                _componentPools = new Dictionary<Type, WeakReference>();
            }
            if (!_componentPools.ContainsKey(componentType) || !_componentPools[componentType].IsAlive) {
                var stack = new Stack<IComponent>();
                _componentPools[componentType] = new WeakReference(stack);
            }
            return _componentPools[componentType].Target as Stack<IComponent>;
        }
    }
}
