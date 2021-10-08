using UnityModManagerNet;
using UnityEngine;
using ModKit;

namespace WoTR_DialogueViewer
{
#if DEBUG
    [EnableReloading]
#endif  
    public class Main
    {
        public static Settings Settings;
        public static bool Enabled;
        static bool Load(UnityModManager.ModEntry modEntry)
        {
            Settings = Settings.Load<Settings>(modEntry);
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;

            return true;
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            Enabled = true;

            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Dialogue GUID:", UI.AutoWidth());
            GUILayout.Space(10f);
            GUILayout.TextField(Settings.dialogueGUID, UI.AutoWidth(), GUILayout.Width(300f));
            GUILayout.EndHorizontal();
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            Settings.Save(modEntry);
        }

    }
}

