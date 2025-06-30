using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace anatawa12.gists.selector
{
    class SelectorWindow : EditorWindow, ISerializationCallbackReceiver
    {
        private static Regex _spaceRegex = new Regex(@"\s+");
        private HashSet<string> _guids = new HashSet<string>();
        [SerializeField] private string searchString = "";
        [SerializeField] private Vector2 scroll;
        [SerializeField] private bool dirty;
        [SerializeField] private string[] guidsSerialized;

        private void OnGUI()
        {
            searchString = EditorGUILayout.TextField("Search", searchString);
            var keywords = _spaceRegex.Split(searchString);
            scroll = EditorGUILayout.BeginScrollView(scroll);
            var guiContent = new GUIContent();
            foreach (var gistInfo in Selector.Gists)
            {
                if (!keywords.All(keyword =>
                        gistInfo.Name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 
                        || gistInfo.Description.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0
                        || gistInfo.ID.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0))
                    continue;
                guiContent.text = $"{gistInfo.Name}";
                guiContent.tooltip = gistInfo.Description;
                var disabled = !Defines.IsActive(gistInfo.DependencyConstants);
                var contains = _guids.Contains(gistInfo.ID);

                if (disabled)
                {
                    guiContent.tooltip = "Because of environment, this this cannot be enabled.";
                    contains = false;
                }

                EditorGUI.BeginDisabledGroup(disabled);
                EditorGUI.BeginChangeCheck();
                contains = EditorGUILayout.ToggleLeft(guiContent, contains);
                EditorGUI.EndDisabledGroup();
                if (EditorGUI.EndChangeCheck())
                {
                    dirty = true;
                    if (contains) _guids.Add(gistInfo.ID);
                    else _guids.Remove(gistInfo.ID);
                }
            }
            EditorGUILayout.EndScrollView();

            EditorGUI.BeginDisabledGroup(!dirty);
            if (GUILayout.Button("Apply Changes"))
            {
                SaveApply();
                dirty = false;
            }
            if (GUILayout.Button("Revert Changes"))
            {
                LoadConfig();
                dirty = false;
            }
            EditorGUI.EndDisabledGroup();
        }

        public void OnBeforeSerialize()
        {
            guidsSerialized = _guids.ToArray();
        }

        public void OnAfterDeserialize()
        {
            _guids = new HashSet<string>(guidsSerialized);
        }

        private void Awake()
        {
            LoadConfig();
        }

        private void OnDestroy()
        {
            if (dirty)
            {
                if (EditorUtility.DisplayDialog("Confirm",
                        "Unapplied gist config found. Do you want to apply before closing?", "Yes", "No"))
                    SaveApply();
            }
        }

        private void SaveApply()
        {
            var list = new List<string>(_guids);
            list.Sort();
            for (var i = 0; i < list.Count; i++)
                if (Selector.GistsById.TryGetValue(list[i], out var info))
                    list[i] = list[i] + ":" + info.Name;
            var array = list.ToArray();
            Selector.SyncWithSettings(array);
            Selector.SaveConfig(array);
        }

        private void LoadConfig()
        {
            var config = Selector.LoadConfig();
            Selector.SyncWithSettings(config);
            _guids = new HashSet<string>(config.Select(x => x.Split(':')[0]));
        }

        [MenuItem("Tools/anatawa12's gist selector")]
        private static void Open() => GetWindow<SelectorWindow>("anatawa12's gist selector");
    }
}
