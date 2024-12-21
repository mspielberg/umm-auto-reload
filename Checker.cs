using System.Text.RegularExpressions;
using UnityEngine;
using UnityModManagerNet;

namespace UMM.AutoReload;

public class Checker : MonoBehaviour
{
    private static Regex cachedAssemblyRegex = new Regex(@"\.\d+\.cache$");

    public void Awake()
    {
        StartCoroutine(CheckMods());
    }

    private System.Collections.IEnumerator CheckMods()
    {
        while (true)
        {
            yield return new WaitForSeconds((float) Main.Settings.checkInterval.TotalSeconds);
            foreach (var modId in Main.Settings.enabledModIds)
            {
                var mod = UnityModManager.FindMod(modId);
                if (NeedsReload(mod))
                {
                    mod.Reload();
                }
            }
        }
    }

    private bool NeedsReload(UnityModManager.ModEntry mod)
    {
        if (!mod.CanReload || !mod.HasAssembly || !mod.Loaded)
            return false;

        var activeAssemblyLocation = mod.Assembly.Location;
        var uncachedAssemblyLocation = cachedAssemblyRegex.Replace(activeAssemblyLocation, "");

        var activeAssemblyModifiedTime = File.GetLastWriteTime(activeAssemblyLocation);
        var uncachedAssemblyModifiedTime = File.GetLastWriteTime(uncachedAssemblyLocation);

        return uncachedAssemblyModifiedTime > activeAssemblyModifiedTime;
    }
}