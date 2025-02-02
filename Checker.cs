using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;
using UnityModManagerNet;

namespace UMMAutoReload;

public class Checker : MonoBehaviour
{
    private FileSystemWatcher? fsw;

    private readonly ConcurrentQueue<UnityModManager.ModEntry> modsChanged = new();

    public void Awake()
    {
        DirectoryInfo modDirectory = new(Main.mod!.Path);
        DirectoryInfo modsDirectory = modDirectory.Parent;

        fsw = new FileSystemWatcher(modsDirectory.FullName, "*.dll");
        fsw.IncludeSubdirectories = true;

        fsw.Changed += Changed;

        fsw.EnableRaisingEvents = true;

        Main.DebugLog(() => $"Started watching {modsDirectory} for DLL changes");

        StartCoroutine(Coro());
    }

    private void Changed(object sender, FileSystemEventArgs e)
    {
        Main.DebugLog(() => "File changed: " + e.FullPath);
        var mod = UnityModManager.modEntries.FirstOrDefault(mod => e.FullPath.StartsWith(mod.Path));
        if (mod != null)
            modsChanged.Enqueue(mod);
    }

    private IEnumerator Coro()
    {
        while (true)
        {
            if (modsChanged.TryDequeue(out var mod)
                && Main.Settings.enabledModIds.Contains(mod.Info.Id))
            {
                Main.DebugLog(() => $"Mod change detected {mod.Info.Id}");
                mod.Reload();
            }
            yield return null;
        }
    }
}