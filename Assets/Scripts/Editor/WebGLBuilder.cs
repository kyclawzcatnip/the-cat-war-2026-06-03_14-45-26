#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace CatWar.Editor
{
    public static class WebGLBuilder
    {
        [MenuItem("Cat War/Build WebGL (Standard)")]
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

        [MenuItem("Cat War/Build for GitHub Pages (docs)")]
        public static void BuildForGitHubPages()
        {
            // Set build path to "docs" at the project root
            string buildPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "docs");

            // Clear old docs folder to prevent file accumulation
            if (Directory.Exists(buildPath))
            {
                try
                {
                    Directory.Delete(buildPath, true);
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"[Cat War Build] Could not clear old docs folder: {ex.Message}");
                }
            }
            Directory.CreateDirectory(buildPath);

            // Configure WebGL player settings to ensure compatibility with GitHub Pages (disabled compression format)
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = new[] { "Assets/Scenes/Battlefield.unity" };
            buildPlayerOptions.locationPathName = buildPath;
            buildPlayerOptions.target = BuildTarget.WebGL;
            buildPlayerOptions.options = BuildOptions.None;

            Debug.Log($"[Cat War Build] Starting WebGL Build for GitHub Pages to: {buildPath}...");

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"[Cat War Build] GitHub Pages WebGL Build Succeeded! Total size: {summary.totalSize / 1024f / 1024f:F2} MB");
                EditorUtility.RevealInFinder(buildPath);
            }
            else if (summary.result == BuildResult.Failed)
            {
                Debug.LogError("[Cat War Build] GitHub Pages Build Failed.");
            }
        }
    }
}
#endif
