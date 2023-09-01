using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace anatawa12.gists.selector
{
    [InitializeOnLoad]
    static class Selector {
        const string ScriptsAsmdefGuid = "9f3777f8fca1841a28e974df5f37df83";
        const string SettingsFolderPath = "ProjectSettings/Packages/com.anatawa12.gists";
        const string GistsFilePath = SettingsFolderPath + "/EnabledGists.txt";

        public static readonly GistInfo[] Gists = GistInfoLoader.Load();

        public static readonly IReadOnlyDictionary<string, GistInfo> GistsById = Gists.ToDictionary(x => x.ID);

        static Selector()
        {
            if (!File.Exists(GistsFilePath))
            {
                SaveConfig(Array.Empty<string>());
            }
            else
            {
                UpdateAsmdef(LoadConfig());
            }
        }

        public static void UpdateAsmdef(string[] gists)
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

            foreach (var configPackage in gists)
            {
                if (string.IsNullOrEmpty(configPackage)) continue;
                var id = configPackage.Split(new[] {':'}, 2);
                if (GistsById.TryGetValue(id[0], out var info))
                    TryAddGist(info);
                else
                    Debug.LogWarning($"Gist with id {id[0]} ({(id.Length == 2 ? id[1] : "unknown")}) not found");
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

        public static string[] LoadConfig()
        {
            return File.ReadAllText(GistsFilePath).Split(new []{"\r\n", "\n"}, StringSplitOptions.None);
        }

        public static void SaveConfig(string[] config)
        {
            Directory.CreateDirectory(SettingsFolderPath);
            var result = new StringBuilder();
            foreach (var s in config)
                result.Append(s).Append('\n');
            File.WriteAllText(GistsFilePath, result.ToString());
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
