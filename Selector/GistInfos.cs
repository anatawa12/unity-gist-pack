using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace anatawa12.gists.selector
{
    static class GistInfoLoader
    {
        public static GistInfo[] Load()
        {
            var path = AssetDatabase.GUIDToAssetPath("0dcdca9645fe442ea8825da0c868bffa");
            return JsonUtility.FromJson<AllGistsInfo>(File.ReadAllText(path))
                .gists.Select(Convert).ToArray();
        }

        private static GistInfo Convert(JsonGistInfo jsonInfo) =>
            new GistInfo(
                jsonInfo.id ?? throw new ArgumentException("id is null"),
                jsonInfo.name ?? throw new ArgumentException("name is null"),
                jsonInfo.description ?? throw new ArgumentException("description is null"),
                ConvertDefines(jsonInfo.constraints ?? Array.Empty<string>()),
                jsonInfo.guids
            );

        private static Define ConvertDefines(string[] jsonInfoConstraints) => jsonInfoConstraints.Aggregate(Define.None,
            (current, jsonInfoConstraint) => current | Defines[jsonInfoConstraint]);

        private static readonly Dictionary<string, Define> Defines = Enum.GetValues(typeof(Define))
            .Cast<Define>()
            .ToDictionary(x => x.ToString(), x => x);
    }

    readonly struct GistInfo
    {
        public readonly string ID;
        public readonly string Name;
        public readonly string Description;
        public readonly Define DependencyConstants;
        public readonly GuidMapping[] Guids;

        public GistInfo([NotNull] string id, [NotNull] string name, [NotNull] string description,
            Define dependencyConstants = Define.None,
            GuidMapping[] guids = null)
        {
            ID = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            DependencyConstants = dependencyConstants;
            Guids = guids ?? Array.Empty<GuidMapping>();
        }
    }

    [Serializable]
    struct AllGistsInfo
    {
        public JsonGistInfo[] gists;
    }

    [Serializable]
    struct JsonGistInfo
    {
        public string id;
        public string name;
        public string description;
        public string[] constraints;
        public GuidMapping[] guids;
    }

    [Serializable]
    struct GuidMapping
    {
        public string disabled;
        public string enabled;
    }
}
