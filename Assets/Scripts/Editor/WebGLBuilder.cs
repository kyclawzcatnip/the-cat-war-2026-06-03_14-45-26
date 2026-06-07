#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace CatWar.Editor
{
    public static class WebGLBuilder
    {
        [MenuItem("Cat War/Build WebGL")]
        public static void Build()
        {
            string buildPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Build/WebGL");
            
            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
            }

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = new[] { "Assets/Scenes/Battlefield.unity" };
            buildPlayerOptions.locationPathName = buildPath;
            buildPlayerOptions.target = BuildTarget.WebGL;
            buildPlayerOptions.options = BuildOptions.None;

            Debug.Log($"[Cat War Build] Starting WebGL Build to: {buildPath}...");
            
            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"[Cat War Build] WebGL Build Succeeded! Total size: {summary.totalSize / 1024f / 1024f:F2} MB");
                EditorUtility.RevealInFinder(buildPath);
            }
            else if (summary.result == BuildResult.Failed)
            {
                Debug.LogError("[Cat War Build] WebGL Build Failed. Check the Console for compilation errors.");
            }
        }
    }
}
#endif
