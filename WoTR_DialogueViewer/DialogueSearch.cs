using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.DialogSystem;
using Kingmaker.DialogSystem.Blueprints;
using ModKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityModManagerNet;
using static ModKit.UI;

namespace WoTR_DialogueViewer
{
    class DialogueSearch
    {
        public static Settings settings => Main.settings;

        public static bool shouldRenderCue = false;
        public static List<BlueprintCue> cues = new List<BlueprintCue>();

        public static void onGUI()
        {
            if (!shouldRenderCue) return;

            int cueNumber = 1;
            foreach(BlueprintCue currentCue in cues)
            {
                string GUID = currentCue.AssetGuid.ToString();
                string displayText = currentCue.DisplayText;
                string speakerName = currentCue.Speaker.Blueprint.CharacterName;

                // Initial Dialogue Information
                Div(0, 25);
                BeginHorizontal();
                Label($"Cue #{cueNumber} GUID: ", Height(10f), Width(90f), AutoHeight(), AutoWidth());
                Space(10f);
                TextField(ref GUID, "GUID", Height(10f), Width(250), AutoHeight(), AutoWidth());
                Space(10f);
                GUILayout.BeginVertical();
                Label("Speaker: " + speakerName, Height(10f), Width(200f), AutoHeight(), AutoWidth());
                Space(10f);
                Label(displayText, Height(10f), Width(ummWidth - 170), AutoHeight(), AutoWidth());
                GUILayout.EndVertical();
                EndHorizontal();

                cueNumber++;
            }
        }

        public static void FindDialogue(string guid)
        {
            BlueprintDialog dialog = ResourcesLibrary.TryGetBlueprint<BlueprintDialog>(guid);

            cues.Clear();
            FindNextCue(dialog.FirstCue.Cues.ElementAt(0));

            shouldRenderCue = (cues.Count > 0);
        }

        public static void FindNextCue(BlueprintCueBaseReference nextCueReference)
        {
            BlueprintCue nextCue = ResourcesLibrary.TryGetBlueprint<BlueprintCue>(nextCueReference.Guid);
            cues.Add(nextCue);
            if (nextCue.Continue.Cues.Count > 0)
            {
                FindNextCue(nextCue.Continue.Cues.ElementAt(0));
            }
            else if (nextCue.Answers.Count > 0)
            {

            }
        }
    }
}
