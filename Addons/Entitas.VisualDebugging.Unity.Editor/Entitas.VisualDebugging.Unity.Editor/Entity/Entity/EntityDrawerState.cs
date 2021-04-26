using System;
using System.Collections.Generic;
using System.Linq;

using DesperateDevs.Utils;

using UnityEditor;
using UnityEngine;

namespace Entitas.VisualDebugging.Unity.Editor {

    public static partial class EntityDrawer {

        static Dictionary<string, bool[]> _contextToUnfoldedComponents;
        public static Dictionary<string, bool[]> contextToUnfoldedComponents {
            get { if (_contextToUnfoldedComponents == null) { _contextToUnfoldedComponents = new Dictionary<string, bool[]>(); } return _contextToUnfoldedComponents; }
        }

        static Dictionary<string, string[]> _contextToComponentMemberSearch;
        public static Dictionary<string, string[]> contextToComponentMemberSearch {
            get { if (_contextToComponentMemberSearch == null) { _contextToComponentMemberSearch = new Dictionary<string, string[]>(); } return _contextToComponentMemberSearch; }
        }

        static Dictionary<string, GUIStyle[]> _contextToColoredBoxStyles;
        public static Dictionary<string, GUIStyle[]> contextToColoredBoxStyles {
            get { if (_contextToColoredBoxStyles == null) { _contextToColoredBoxStyles = new Dictionary<string, GUIStyle[]>(); } return _contextToColoredBoxStyles; }
        }

        public struct ComponentInfo {
            public int index;
            public string name;
            public Type type;
        }

        static Dictionary<string, ComponentInfo[]> _contextToComponentInfos;
        public static Dictionary<string, ComponentInfo[]> contextToComponentInfos {
            get { if (_contextToComponentInfos == null) { _contextToComponentInfos = new Dictionary<string, ComponentInfo[]>(); } return _contextToComponentInfos; }
        }

        static GUIStyle _foldoutStyle;
        public static GUIStyle foldoutStyle {
            get { if (_foldoutStyle == null) { _foldoutStyle = new GUIStyle(EditorStyles.foldout); _foldoutStyle.fontStyle = FontStyle.Bold; } return _foldoutStyle; }
        }

        static string _componentNameSearchString;
        public static string componentNameSearchString {
            get { if (_componentNameSearchString == null) { _componentNameSearchString = string.Empty; } return _componentNameSearchString; }
            set { _componentNameSearchString = value; }
        }

        public static readonly IDefaultInstanceCreator[] _defaultInstanceCreators;
        public static readonly ITypeDrawer[] _allTypeDrawers;
        public static readonly IComponentDrawer[] _allComponentDrawers;

        private static readonly Dictionary<Type, ITypeDrawer> _typeDrawers;
        private static readonly Dictionary<Type, IComponentDrawer> _componentDrawers;

        static EntityDrawer() {
            _defaultInstanceCreators = AppDomain.CurrentDomain.GetInstancesOf<IDefaultInstanceCreator>();
            _allTypeDrawers = AppDomain.CurrentDomain.GetInstancesOf<ITypeDrawer>();
            _allComponentDrawers = AppDomain.CurrentDomain.GetInstancesOf<IComponentDrawer>();

            _typeDrawers = new Dictionary<Type, ITypeDrawer>();
            _componentDrawers = new Dictionary<Type, IComponentDrawer>();
        }

        static bool[] getUnfoldedComponents(IEntity entity) {
            bool[] unfoldedComponents;
            if (!contextToUnfoldedComponents.TryGetValue(entity.contextInfo.name, out unfoldedComponents)) {
                unfoldedComponents = new bool[entity.totalComponents];
                for (int i = 0; i < unfoldedComponents.Length; i++) {
                    unfoldedComponents[i] = true;
                }
                contextToUnfoldedComponents.Add(entity.contextInfo.name, unfoldedComponents);
            }

            return unfoldedComponents;
        }

        static string[] getComponentMemberSearch(IEntity entity) {
            string[] componentMemberSearch;
            if (!contextToComponentMemberSearch.TryGetValue(entity.contextInfo.name, out componentMemberSearch)) {
                componentMemberSearch = new string[entity.totalComponents];
                for (int i = 0; i < componentMemberSearch.Length; i++) {
                    componentMemberSearch[i] = string.Empty;
                }
                contextToComponentMemberSearch.Add(entity.contextInfo.name, componentMemberSearch);
            }

            return componentMemberSearch;
        }

        static ComponentInfo[] getComponentInfos(IEntity entity) {
            ComponentInfo[] infos;
            if (!contextToComponentInfos.TryGetValue(entity.contextInfo.name, out infos)) {
                var contextInfo = entity.contextInfo;
                var infosList = new List<ComponentInfo>(contextInfo.componentTypes.Length);
                for (int i = 0; i < contextInfo.componentTypes.Length; i++) {
                    infosList.Add(new ComponentInfo {
                        index = i,
                        name = contextInfo.componentNames[i],
                        type = contextInfo.componentTypes[i]
                    });
                }
                infos = infosList.ToArray();
                contextToComponentInfos.Add(entity.contextInfo.name, infos);
            }

            return infos;
        }

        static GUIStyle getColoredBoxStyle(IEntity entity, int index) {
            GUIStyle[] styles;
            if (!contextToColoredBoxStyles.TryGetValue(entity.contextInfo.name, out styles)) {
                styles = new GUIStyle[entity.totalComponents];
                for (int i = 0; i < styles.Length; i++) {
                    var hue = (float)i / (float)entity.totalComponents;
                    var componentColor = Color.HSVToRGB(hue, 0.7f, 1f);
                    componentColor.a = 0.15f;
                    var style = new GUIStyle(GUI.skin.box);
                    style.normal.background = createTexture(2, 2, componentColor);
                    styles[i] = style;
                }
                contextToColoredBoxStyles.Add(entity.contextInfo.name, styles);
            }

            return styles[index];
        }

        static Texture2D createTexture(int width, int height, Color color) {
            var pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; ++i) {
                pixels[i] = color;
            }
            var result = new Texture2D(width, height);
            result.SetPixels(pixels);
            result.Apply();
            return result;
        }

        static IComponentDrawer getComponentDrawer(Type type) {

            if (!_componentDrawers.ContainsKey(type))
                _componentDrawers[type] = _allComponentDrawers.FirstOrDefault(d => d.HandlesType(type));

            return _componentDrawers[type];
        }

        static ITypeDrawer getTypeDrawer(Type type) {

            if (!_typeDrawers.ContainsKey(type))
                _typeDrawers[type] = _allTypeDrawers.FirstOrDefault(d => d.HandlesType(type));

            return _typeDrawers[type];
        }
    }
}
