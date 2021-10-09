using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.DialogSystem;
using Kingmaker.DialogSystem.Blueprints;
using Kingmaker.ElementsSystem;
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
        public static Dictionary<BlueprintCue, List<BlueprintAnswer>> cues = new Dictionary<BlueprintCue, List<BlueprintAnswer>>();
        public static Dictionary<bool, Dictionary<BlueprintCue, Dictionary<bool, List<BlueprintAnswer>>>> temp;
        public static Dictionary<bool, Dictionary<BlueprintCue, Dictionary<bool, Dictionary<List<BlueprintAnswer>, Dictionary<bool, List<BlueprintCheck>>>>>> temp2;

        public static void onGUI()
        {
            if (!shouldRenderCue) return;

            int cueNumber = 1;
            foreach (KeyValuePair<BlueprintCue, List<BlueprintAnswer>> currentCue in cues)
            {
                string currentCue_GUID = currentCue.Key.AssetGuid.ToString();
                string currentCue_displayText = currentCue.Key.DisplayText;
                string currentCue_speakerName = currentCue.Key.Speaker.Blueprint.CharacterName;

                Div(0, 25);
                HStack($"Cue #{cueNumber}".color(RGBA.orange).bold(), 4,
                    // Top Row: GUID#, Speaker: <Name>
                    () => GUILayout.BeginVertical(),
                    () =>
                    {
                        GUILayout.BeginHorizontal();
                        Label($"GUID: ".color(RGBA.darkgrey), AutoWidth());
                        TextField(ref currentCue_GUID, "GUID", AutoWidth());
                        Space(20);
                        Label("Speaker: "+ currentCue_speakerName.color(RGBA.blue).bold(), Width(150f), AutoWidth());
                        GUILayout.EndHorizontal();
                    },

                    // Bottom Row: <Display Text>
                    () => Label(currentCue_displayText, Width(ummWidth * 3 / 4), AutoWidth()),
                    () => GUILayout.EndVertical()
                );
                
                if (currentCue.Value != null && currentCue.Value.Count > 0)
                {
                    HStack($"Answers".color(RGBA.red).bold(), 0,
                    () =>
                    {
                        int answerNumber = 1;
                        GUILayout.BeginVertical();
                        foreach (BlueprintAnswer answer in currentCue.Value)
                        {
                            string currentAnswer_GUID = answer.AssetGuid.ToString();
                            Div(0, 25);
                            HStack($"Answer #{answerNumber} ".color(RGBA.red).bold(), 0,
                                // Top Line: GUID: <GUID>
                                () => Label("GUID: ".color(RGBA.darkgrey), AutoWidth()),
                                () => Space(10f),
                                () => TextField(ref currentAnswer_GUID, "GUID", AutoWidth()),

                                () => GUILayout.BeginVertical(),
                                () =>
                                {
                                    if (answer.ShowConditions.Conditions.Length > 0)
                                    {
                                        foreach (Condition currentCondition in answer.ShowConditions.Conditions)
                                        {
                                            string conditionText = currentCondition.ToString();
                                            Label($"Show Condition: ".color(RGBA.yellow) + conditionText, AutoWidth());
                                        }
                                    } else if (answer.SelectConditions.Conditions.Length > 0)
                                    {
                                        Label("Select Condition: ", AutoWidth());
                                    }
                                },
                                // Bottom Line: <DisplayText>
                                () => Label(answer.DisplayText, AutoWidth()),
                                () => GUILayout.EndVertical()
                            );
                            answerNumber++;
                        }
                        GUILayout.EndVertical();
                    });
                }

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

            if (nextCue.Answers.Count > 0)
            {                
                cues.Add(nextCue, FindAnswers(nextCue.Answers));
            }
            else
            {
                cues.Add(nextCue, null);
            }
                
            if (nextCue.Continue.Cues.Count > 0)
            {
                FindNextCue(nextCue.Continue.Cues.ElementAt(0));
            }
        }

        public static List<BlueprintAnswer> FindAnswers(List<BlueprintAnswerBaseReference> cue)
        {
            List<BlueprintAnswer> answers = new List<BlueprintAnswer>();

            foreach(BlueprintAnswerBaseReference answersListGUID in cue)
            {
                BlueprintAnswersList answerList = ResourcesLibrary.TryGetBlueprint<BlueprintAnswersList>(answersListGUID.Guid);
                foreach(BlueprintAnswerBaseReference answerGUID in answerList.Answers)
                {
                    BlueprintAnswer answer = ResourcesLibrary.TryGetBlueprint<BlueprintAnswer>(answerGUID.Guid);
                    answers.Add(answer);
                }
            }

            return answers;
        }
    }
}
