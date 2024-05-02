using System;
using UnityEditor;
using UnityEditor.Build.Reporting;


public class BuildScript
{
    private static void BuildWebGl()
    {
        // Set architecture in BuildSettings
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.target = BuildTarget.WebGL;
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/Main.unity" };
        buildPlayerOptions.locationPathName = "Builds/dune_io";
        buildPlayerOptions.options = BuildOptions.None;

        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
        
        var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        var summary = report.summary;
        
        if (summary.result == BuildResult.Succeeded)
        {
            Console.WriteLine("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Console.WriteLine("Build failed");
        }
    }
    /// <summary>
    /// This method is called from the CI/CD pipeline to build the game.
    /// </summary>
    public static void Build()
    {
        BuildWebGl();
    }
}
