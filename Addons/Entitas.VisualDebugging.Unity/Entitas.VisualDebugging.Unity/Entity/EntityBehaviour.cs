using System.Collections.Generic;
using UnityEngine;

namespace Entitas.VisualDebugging.Unity {

    [ExecuteInEditMode]
    public class EntityBehaviour : MonoBehaviour {

        public IContext context { get { return _context; } }
        public IEntity entity { get { return _entity; } }

        IContext _context;
        IEntity _entity;
        Stack<EntityBehaviour> _entityBehaviourPool;
        string _cachedName;
        HashSet<int> _addedComponents = new HashSet<int>();
        HashSet<int> _removedComponents = new HashSet<int>();

        public void Init(IContext context, IEntity entity, Stack<EntityBehaviour> entityBehaviourPool) {
            _context = context;
            _entity = entity;
            _entityBehaviourPool = entityBehaviourPool;
            _entity.OnEntityReleased += onEntityReleased;
            _entity.OnComponentAdded += ComponentAdded;
            _entity.OnComponentRemoved += ComponentRemoved;
            gameObject.hideFlags = HideFlags.None;
            gameObject.SetActive(true);
            name = _cachedName = _entity.ToString();
            _addedComponents.Clear();
            _removedComponents.Clear();
        }

        void onEntityReleased(IEntity e) {
            _entity.OnEntityReleased -= onEntityReleased;
            _entity.OnComponentAdded -= ComponentAdded;
            _entity.OnComponentRemoved -= ComponentRemoved;
            gameObject.SetActive(false);
            gameObject.hideFlags = HideFlags.HideInHierarchy;
            _entityBehaviourPool.Push(this);
            _cachedName = null;
            name = string.Empty;
        }

        void ComponentAdded(IEntity entity, int index, IComponent component) {
            if(_removedComponents.Contains(index)) {
                _removedComponents.Remove(index);
            }
            else {
                _addedComponents.Add(index);
            }
        }

        void ComponentRemoved(IEntity entity, int index, IComponent component)  {
            if (_addedComponents.Contains(index)) {
                _addedComponents.Remove(index);
            }
            else {
                _removedComponents.Add(index);
            }
        }

        void Update() {
            if (_addedComponents.Count > 0 || _removedComponents.Count > 0)  {
                if (_entity != null && _cachedName != _entity.ToString()) {
                    name = _cachedName = _entity.ToString();
                }
                _addedComponents.Clear();
                _removedComponents.Clear();
            }
        }

        void OnDestroy() {
            if (_entity != null) {
                _entity.OnEntityReleased -= onEntityReleased;
            }
        }
    }
}
