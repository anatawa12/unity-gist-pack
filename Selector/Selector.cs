using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace anatawa12.gists.selector
{
    [InitializeOnLoad]
    static class Selector {
        const string ScriptsAsmdefGuid = "9f3777f8fca1841a28e974df5f37df83";
        const string SettingsFolderPath = "ProjectSettings/Packages/com.anatawa12.gists";
        const string SettingsFilePath = SettingsFolderPath + "/settings.json";

        public static readonly GistInfo[] Gists =
        {
            new GistInfo("5987f6b5357c3c91603fa07f215dfeab", "CompilationLogWindow",
                "The window to see compilation progress")
        };

        private static readonly Dictionary<string, GistInfo> GistsById = Gists.ToDictionary(x => x.ID);

        static Selector()
        {
            if (!File.Exists(SettingsFilePath))
            {
                SaveConfig(new SelectorSettings());
            }
            else
            {
                UpdateAsmdef(LoadConfig());
            }
        }

        public static void UpdateAsmdef(SelectorSettings config)
        {
            var defines = new List<string>();

            void TryAddGist(in GistInfo info)
            {
                if (!Defines.IsActive(info.DependencyConstants))
                {
                    Debug.LogWarning(
                        $"Gist with id {info.ID} ({info.Name}) is not valid due to some missing dependencies");
                    return;
                }

                defines.Add($"GIST_{info.ID}");
            }

            if (config.allPackages)
            {
                foreach (var info in Gists)
                    TryAddGist(info);
            }
            else
            {
                foreach (var configPackage in config.packages)
                {
                    var id = configPackage.Split(new[] {':'}, 2);
                    if (GistsById.TryGetValue(id[0], out var info))
                        TryAddGist(info);
                    else
                        Debug.LogWarning($"Gist with id {id[0]} ({(id.Length == 2 ? id[1] : "unknown")}) not found");
                }
            }

            var asmdefPath = AssetDatabase.GUIDToAssetPath(ScriptsAsmdefGuid);
            if (string.IsNullOrEmpty(asmdefPath))
            {
                Debug.LogError($"assembly definition for com.anatawa12.gists.scripts not found");
                return;
            }

            var asmdef = JsonUtility.FromJson<AsmdefJson>(File.ReadAllText(asmdefPath));


            bool GistDefineFilter(VersionDefine x) =>
                x.name == "Unity" && x.expression == "" && x.define.StartsWith("GIST_", StringComparison.Ordinal);

            var alreadyDefined =
                new HashSet<string>(asmdef.versionDefines.Where(GistDefineFilter).Select(x => x.define));
            if (alreadyDefined.SetEquals(defines))
            {
                // asmdef is good. nothing to do.
                return;
            }

            // edit asmdef now.

            asmdef.versionDefines.RemoveAll(GistDefineFilter);
            asmdef.versionDefines.AddRange(defines.Select(define => new VersionDefine("Unity", "", define)));

            File.WriteAllText(asmdefPath, JsonUtility.ToJson(asmdef, true));
            AssetDatabase.Refresh();
        }

        public static SelectorSettings LoadConfig()
        {
            return JsonUtility.FromJson<SelectorSettings>(File.ReadAllText(SettingsFilePath));
        }

        public static void SaveConfig(SelectorSettings config)
        {
            Directory.CreateDirectory(SettingsFolderPath);
            File.WriteAllText(SettingsFilePath, JsonUtility.ToJson(config, true));
        }
    }

    [Serializable]
    class SelectorSettings
    {
        // <id>:<name>
        public string[] packages;
        public bool allPackages;
    }

    readonly struct GistInfo
    {
        public readonly string ID;
        public readonly string Name;
        public readonly string Description;
        public readonly Define DependencyConstants;

        public GistInfo([NotNull] string id, [NotNull] string name, [NotNull] string description,
            Define dependencyConstants = Define.None)
        {
            ID = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            DependencyConstants = dependencyConstants;
        }
    }

    [Serializable]
    class AsmdefJson
    {
        public string name;
        public string[] references;
        public string[] includePlatforms;
        public string[] excludePlatforms;
        public bool allowUnsafeCode;
        public bool overrideReferences;
        public string[] precompiledReferences;
        public bool autoReferenced;
        public string[] defineConstraints;
        public List<VersionDefine> versionDefines;
        public bool noEngineReferences;
    }

    [Serializable]
    class VersionDefine
    {
        public string name;
        public string expression;
        public string define;

        public VersionDefine(string name, string expression, string define)
        {
            this.name = name;
            this.expression = expression;
            this.define = define;
        }
    }
}
