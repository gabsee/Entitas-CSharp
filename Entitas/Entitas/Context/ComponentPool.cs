using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Entitas {
    public static class ComponentPool {

        private static Dictionary<Type, WeakReference> _componentPools;

        public static ConcurrentBag<IComponent> Get<TComponent>() where TComponent : IComponent {
            return Get(typeof(TComponent));
        }

        public static ConcurrentBag<IComponent> Get(Type componentType) {
            if (_componentPools == null) {
                _componentPools = new Dictionary<Type, WeakReference>();
            }
            if (!_componentPools.ContainsKey(componentType) || !_componentPools[componentType].IsAlive) {
                var stack = new ConcurrentBag<IComponent>();
                _componentPools[componentType] = new WeakReference(stack);
            }
            return _componentPools[componentType].Target as ConcurrentBag<IComponent>;
        }

        public static void Clear(this ConcurrentBag<IComponent> p_bag) {
            while (!p_bag.IsEmpty)
                p_bag.TryTake(out _);
        }
    }
}
