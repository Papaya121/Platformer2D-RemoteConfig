using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildAll
{
    private const string DESKTOP_DIR = "Builds/Desktop";
    private const string ANDROID_DIR = "Builds/Android";
    private const string GAME_NAME = "Platformer2D";

    [MenuItem("Build/Build All")]
    public static void BuildAllPlatforms()
    {
        var scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            Debug.LogError("Нет сцен в Build Settings/Build Profiles. Добавь и включи хотя бы одну сцену.");
            return;
        }

        BuildForDesktop(scenes);
        BuildForAndroid(scenes);

        Debug.Log("Build All: готово.");
    }

    private static void BuildForDesktop(string[] scenes)
    {
        EnsureDir(DESKTOP_DIR);

#if UNITY_EDITOR_OSX
        var group = BuildTargetGroup.Standalone;
        var target = BuildTarget.StandaloneOSX;

        var path = $"{DESKTOP_DIR}/{GAME_NAME}.app";
#else
        var group = BuildTargetGroup.Standalone;
        var target = BuildTarget.StandaloneWindows64;

        var path = $"{DESKTOP_DIR}/{GAME_NAME}.exe";
#endif

        EditorUserBuildSettings.SwitchActiveBuildTarget(group, target);

        var options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = path,
            target = target,
            options = BuildOptions.None
        };

        var report = BuildPipeline.BuildPlayer(options);
        LogReport("Desktop", report);
    }

    private static void BuildForAndroid(string[] scenes)
    {
        EnsureDir(ANDROID_DIR);

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

        var path = $"{ANDROID_DIR}/{GAME_NAME}.apk";

        var options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = path,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        var report = BuildPipeline.BuildPlayer(options);
        LogReport("Android", report);
    }

    private static void EnsureDir(string dir)
    {
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }

    private static void LogReport(string name, BuildReport report)
    {
        if (report.summary.result == BuildResult.Succeeded)
            Debug.Log($"{name} build succeeded. Size: {report.summary.totalSize} bytes");
        else
            Debug.LogError($"{name} build failed: {report.summary.result}");
    }
}
