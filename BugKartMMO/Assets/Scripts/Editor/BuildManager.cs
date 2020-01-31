using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using Network;

public class BuildManager
{
    [MenuItem("Build/Windows")]
    public static void BuildWindows()
    {
        Object.FindObjectOfType<NetworkManager>().BuildPrefabIDs();


        BuildPlayerOptions buildOptions = new BuildPlayerOptions();
        buildOptions.locationPathName = "Builds/Windows.exe";
        buildOptions.scenes = new string[] 
        {
            // --> only relevant scenes for game!
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/Lobby.unity",
            "Assets/Scenes/Game.unity",
        };
        buildOptions.target = BuildTarget.StandaloneWindows;
        buildOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build was succesful!");
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = Application.dataPath + "/../Builds",
                UseShellExecute = true,
                Verb = "open"
            });
        }
        else
        {
            Debug.LogError("Build failed!");
        }
    }
}
