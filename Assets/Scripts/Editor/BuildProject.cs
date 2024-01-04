using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Valve.VR.InteractionSystem;
using System.IO.Compression;

#if UNITY_EDITOR
public class BuildProject : EditorWindow {

    private float progress;
    private int sum;

    [MenuItem("Build/BuildProject")]
    static void Init()
    {
        UnityEditor.EditorWindow window = GetWindow(typeof(BuildProject));
        window.Show();
    }

    public async void OnGUI()
    {
        if (GUILayout.Button("Build"))
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

            buildPlayerOptions.scenes = new string[]
            {
            "Assets/Scenes/Main.unity",
            "Assets/Scenes/Module1.unity",
            "Assets/Scenes/Module2.unity",
            "Assets/Scenes/Module3.unity",
            "Assets/Scenes/Module4.unity",
            "Assets/Scenes/Module6.unity",
            };

            var directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/BUILDS/";
            DirectoryInfo di = new DirectoryInfo(directory);
            var files = di.GetFiles();
            var dirs = di.GetDirectories();

            sum = files.Length + dirs.Length;
            sum += 6;


            System.Threading.Tasks.Task DeleteTask = new System.Threading.Tasks.Task(() =>
            {
                for (int i = 0; i < files.Length; ++i)
                {
                    progress++;
                    files[i].Delete();
                }
                for (int i = 0; i < dirs.Length; ++i)
                {
                    progress++;
                    dirs[i].Delete(true);
                }
            });

            DeleteTask.Start();
            await DeleteTask;

            var path = directory + "/KursTest";
            buildPlayerOptions.locationPathName = path + "/SimtechKursant.exe";
            buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
            buildPlayerOptions.options = BuildOptions.Development;

            BuildPipeline.BuildPlayer(buildPlayerOptions);

            Debug.Log("BUILD: MOVING FILES...");

            System.Threading.Tasks.Task[] tasks = new System.Threading.Tasks.Task[]
            {
            new System.Threading.Tasks.Task(() => {Copy("Assets/Media", path + "/SimtechKursant_Data/Media"); }),
            new System.Threading.Tasks.Task(() => {Copy("Assets/CSV", path + "/SimtechKursant_Data/CSV"); }),
            new System.Threading.Tasks.Task(() => {Copy("Assets/Audio", path + "/SimtechKursant_Data/Audio"); }),
            new System.Threading.Tasks.Task(() => {Copy("Assets/Texts", path + "/SimtechKursant_Data/Texts"); }),
            new System.Threading.Tasks.Task(() => {Copy("Assets/PDF", path + "/SimtechKursant_Data/PDF"); })
            };

            tasks.ForEach(x => x.Start());

            await System.Threading.Tasks.Task.WhenAll(tasks);

            Debug.Log("BUILD: FILES MOVED...");

            Debug.Log("BUILD: ZIP CREATION...");

            System.Threading.Tasks.Task CompressTask = new System.Threading.Tasks.Task(() =>
            {
                ZipFile.CreateFromDirectory(path, directory + "/KursTest.zip", System.IO.Compression.CompressionLevel.Optimal, false);
            });

            CompressTask.Start();
            await CompressTask;
            progress += 100;

            Debug.Log("BUILD: ZIP CREATED...");
        }

        if (progress < sum)
            EditorUtility.DisplayProgressBar("Building", "Moving files and compressing ZIP", progress / sum);
        if (progress >= sum)
            EditorUtility.ClearProgressBar();
    }

    private void Copy(string sourceDirectory, string targetDirectory)
    {
        var diSource = new DirectoryInfo(sourceDirectory);
        var diTarget = new DirectoryInfo(targetDirectory);

        CopyAll(diSource, diTarget);

        progress++;
        Debug.Log("COPIED: " + sourceDirectory + " TO: " + targetDirectory);
    }

    private void CopyAll(DirectoryInfo source, DirectoryInfo target)
    {
        Directory.CreateDirectory(target.FullName);

        foreach (FileInfo fi in source.GetFiles())
        {
            fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
        }

        foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
        {
            DirectoryInfo nextTargetSubDir =
                target.CreateSubdirectory(diSourceSubDir.Name);
            CopyAll(diSourceSubDir, nextTargetSubDir);
        }
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }
}
#endif
