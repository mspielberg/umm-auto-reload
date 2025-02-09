namespace UMMAutoReload;

using UnityEngine;
using UnityModManagerNet;

public static class Main
{
    internal static UnityModManager.ModEntry? mod;
    private static Settings? settings;
    
    private static GameObject? gameObject;

    public static Settings Settings => settings!;

    public static bool Load(UnityModManager.ModEntry modEntry)
    {
        mod = modEntry;
        settings = UnityModManager.ModSettings.Load<Settings>(modEntry);

        mod.OnGUI = OnGUI;
        mod.OnSaveGUI = settings.Save;
        mod.OnToggle = OnToggle;

        return true;
    }

    private static void OnGUI(UnityModManager.ModEntry modEntry)
    {
        settings!.Draw();
    }

    private static bool OnToggle(UnityModManager.ModEntry modEntry, bool enabled)
    {
        if (enabled)
        {
            gameObject = new GameObject(modEntry.Info.Id);
            Object.DontDestroyOnLoad(gameObject);
            gameObject.AddComponent<Checker>();
        }
        else
        {
            Object.Destroy(gameObject);
        }
        return true;
    }

    public static void DebugLog(Func<string> message)
    {
        if (settings!.loggingEnabled)
        {
            mod!.Logger.Log(message());
        }
    }
}
