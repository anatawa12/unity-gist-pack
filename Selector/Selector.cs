using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace anatawa12.gists.selector
{
    [InitializeOnLoad]
    static class Selector {
        const string ScriptsAsmdefGuid = "9f3777f8fca1841a28e974df5f37df83";
        const string SettingsFolderPath = "ProjectSettings/Packages/com.anatawa12.gists";
        const string GistsFilePath = SettingsFolderPath + "/EnabledGists.txt";

        public static readonly GistInfo[] Gists =
        {
            new GistInfo("257fbbebd9b7dab8bf39c0c710a2bfc7", 
                "CompileLogger",
                "The .cs file to log compilation to some file. useful with tail -f"),

            new GistInfo("def5f8a29179ecbcb45502fa3b4590ce", 
                "CreateAssemblyDefinitionForUdonSharp",
                "Adds menu to create assembly definition with U# assembly definition",
                Define.UDON_SHARP),

            new GistInfo("e825ec4ee39ae29b64fdcc2f3f07a58c", 
            "MapGameObjects",
                "MapGameObjects"),

            new GistInfo("4476430cfcc2ccef4bc40341d20001cf", 
                "Fake DynamicBone Components",
                "Fake DynamicBone Component to make DB -> PB Converter in VRChat SDK works."),

            new GistInfo("a4bb4e2e5d75b4fa5ba42e236aae564d", 
            "ActualPerformanceWindow",
                "A window to see actual performance rank on building avatars",
                Define.VRCSDK_AVATARS),

            new GistInfo("379c4d828c2a0add4d623f8668209cbc", 
            "PhysBoneEditorUtilities",
                "Set of utilities for PhysBone", 
            Define.VRCSDK_AVATARS),

            new GistInfo("4c900d5c15050fb5bdc0f9d027962183", 
            "GenerateMeshWithBackFace",
                "generate new mesh with backface. useful for quest avatars"),

            new GistInfo("b8799da5d3131e4020f414439a4ea037", 
            "Transfer Transform Window",
                "A window to copy transform info recursively"),

            new GistInfo("581b66619711eaf5ebacbd85369d62e6", 
            "SetRandomBlueprintId",
                "Set blueprint id without build",
                Define.VRCSDK_BASE),

            new GistInfo("5f847a1692fb30c2c9f00a47d50243ad", 
                "AndroidOnlyCheck",
                "A VRCSDKPreprocessAvatarCallback which prevents PC builds.",
                Define.VRCSDK_AVATARS),

            new GistInfo("f7476d2d727bc43d86121f6a3337d2c3", 
                "MergeAnimationClip",
                "Micro tool to multiple animation clip into one."),

            new GistInfo("5987f6b5357c3c91603fa07f215dfeab", 
            "CompilationLogWindow",
                "The window to see compilation progress"),

            new GistInfo("8375f82dbc751086a32fcd2c626fa09b",
                "HumanoidInfoWindow",
                "Actual bone mapping to Animator"),

            new GistInfo("875d0776305b771ba7ee74c656f082f6",
                "EditorScreenshotTakeTool",
                "A tool to take screen shot of an Editor"),

            new GistInfo("94d6fd4272025fd26962476100a20ff0",
                "SelectSkinnedMeshBones",
                "Select GameObjects used by SkinnedMeshRenderers"),

            new GistInfo("ae5f7b3c5e07150ddc1eb9f0948019ff", 
                "FindReferenceChainRecursive",
                "Tool to find unexpected references in avatars or scenes"),

            new GistInfo("930c08c724af17197a401bcfd580985b",
                "RemovePropertiesFromAnimations",
                "A window to remove some property from multiple animations"),

            new GistInfo("ecf33339c315f259cee62b304910fe43",
                "SetDirtyRecursively",
                "Set dirty all components on selected GameObject to avoid reference to prefab asset"),
        };

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
