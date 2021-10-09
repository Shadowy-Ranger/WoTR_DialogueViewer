using UnityModManagerNet;
using UnityEngine;
using ModKit;
using static ModKit.UI;

namespace WoTR_DialogueViewer
{
#if DEBUG
    [EnableReloading]
#endif  
    public class Main
    {
        public static Settings settings;
        public static bool enabled;
        static bool Load(UnityModManager.ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;

            return true;
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = true;

            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            BeginHorizontal();
            Label("Dialogue GUID:", UI.AutoWidth());
            Space(10f);
            TextField(ref settings.dialogueGUID, "dialogueGUID", AutoWidth(), Width(300f));
            Space(10f);
            ActionButton("Search", () => DialogueSearch.FindDialogue(settings.dialogueGUID), AutoWidth(), Width(100f));
            EndHorizontal();

            DialogueSearch.onGUI();
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }

    }
}

