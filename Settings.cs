using UnityEngine;
using UnityModManagerNet;

namespace UMMAutoReload;

public class Settings : UnityModManager.ModSettings
{
    public readonly HashSet<string> enabledModIds = new();

    public bool loggingEnabled = false;

    public void Draw()
    {
        GUILayout.BeginVertical(GUILayout.ExpandWidth(false));

        var reloadableModIds = UnityModManager.modEntries
            .Where(mod => mod.CanReload && mod.HasAssembly)
            .Select(mod => mod.Info.Id);

        GUILayout.Label("Enabled Mods");
        foreach (var modId in reloadableModIds)
        {
            if (GUILayout.Toggle(enabledModIds.Contains(modId), modId))
            {
                enabledModIds.Add(modId);
            }
            else
            {
                enabledModIds.Remove(modId);
            }
        }

        GUILayout.Space(10);

        loggingEnabled = GUILayout.Toggle(loggingEnabled, "Logging Enabled");

        GUILayout.EndVertical();
    }

    public override void Save(UnityModManager.ModEntry modEntry)
    {
        Save(this, modEntry);
    }
}