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

        public static bool shouldRender = false;

        // list of rootCues
        public static Dictionary<BlueprintCue, bool> firstCueDictionary = 
            new Dictionary<BlueprintCue, bool>();

        // dictionary of cues, keyed by the blueprint they are triggered by
        // - Cues can be called by a BlueprintCheck, BlueprintAnswer, or BlueprintCue
        public static Dictionary<BlueprintGuid, KeyValuePair<BlueprintCue, bool>> cueDictionary =
            new Dictionary<BlueprintGuid, KeyValuePair<BlueprintCue, bool>>();

        // dictionary of answers, keyed by the cue which holds the answerlist the answer is from
        // - Answers can be pointed towards by cues or first cues
        public static Dictionary<BlueprintGuid, KeyValuePair<List<KeyValuePair<BlueprintAnswer, bool>>, bool>> answerDictionary =
            new Dictionary<BlueprintGuid, KeyValuePair<List<KeyValuePair<BlueprintAnswer, bool>>, bool>>();

        // dictionary of checks, keyed by the answer they are called from
        public static Dictionary<BlueprintGuid, KeyValuePair<BlueprintCheck, bool>> checkDictionary =
            new Dictionary<BlueprintGuid, KeyValuePair<BlueprintCheck, bool>>();

        public static void onGUI()
        {
            if (!shouldRender) return;

            foreach(BlueprintCue firstCue in firstCueDictionary.Keys)
            {
                DisplayCue(firstCue,0);
                if (cueDictionary.ContainsKey(firstCue.AssetGuid))
                {
                    DisplayAllCues(firstCue);
                }
                else if (answerDictionary.ContainsKey(firstCue.AssetGuid))
                {
                    KeyValuePair<BlueprintCue, bool> answerCue = new KeyValuePair<BlueprintCue, bool>(firstCue, false);
                    DisplayAllAnswers(answerCue);
                }
            }
        }
        public static void DisplayAllCues(BlueprintCue cue)
        {
            int cueNumber = 1;
            KeyValuePair<BlueprintCue, bool> currentCue = new KeyValuePair<BlueprintCue, bool>(cue, false);
            do
            {
                currentCue = cueDictionary[currentCue.Key.AssetGuid];
                // TODO: call alternate rendering code if this should be hidden
                if (currentCue.Value == true) 
                {
                    DisplayHiddenCue(currentCue.Key);
                }
                else
                {
                    DisplayCue(currentCue.Key, cueNumber);
                }
                
                if (answerDictionary.ContainsKey(currentCue.Key.AssetGuid))
                {
                    DisplayAllAnswers(currentCue);
                }

                cueNumber++;
            } while (cueDictionary.ContainsKey(currentCue.Key.AssetGuid));
        }
        public static void DisplayCue(BlueprintCue cue, int cueNumber)
        {
            string cue_Name = cue.name.ToString();
            string cue_GUID = cue.AssetGuid.ToString();
            string cue_Speaker = "N/A";
            if (cue.Speaker != null && cue.Speaker.Blueprint != null) cue_Speaker = cue.Speaker.Blueprint.CharacterName;
            string HStackTitle = $"Cue #{cueNumber}";
            

            if (cueNumber == 0) HStackTitle = "Root Cue";

            Div(0, 10f);
            HStack(HStackTitle, 0,
            () => GUILayout.BeginVertical(),
            () => BeginHorizontal(),
            () =>
            {
                Label($"GUID: ", AutoWidth());
                Space(10f);
                TextField(ref cue_GUID, "cueGUID", Width(300f), AutoWidth());
                Space(10f);
                Label($"Name: ", AutoWidth());
                Space(10f);
                TextField(ref cue_Name, "cueName", Width(100f), AutoWidth());
            },
            () => EndHorizontal(),
            () => Label($"Speaker: {cue_Speaker}"),
            () => Label(cue.DisplayText, AutoHeight()),
            () => GUILayout.EndVertical()
            );
        }
        public static void DisplayHiddenCue(BlueprintCue cue)
        {

        }
        public static void DisplayAllAnswers(KeyValuePair<BlueprintCue, bool> cue)
        {
            int answerNumber = 1;
            bool answerGUIDPresent = false;
            do
            {
                // TODO: call alternate rendering code if this should be hidden
                if (answerDictionary[cue.Key.AssetGuid].Value == true) { }
                else
                {
                    foreach (KeyValuePair<BlueprintAnswer, bool> currentAnswer in answerDictionary[cue.Key.AssetGuid].Key)
                    {
                        DisplayAnswer(currentAnswer.Key, answerNumber);

                        answerGUIDPresent = answerDictionary.ContainsKey(currentAnswer.Key.AssetGuid);
                        answerNumber++;
                    }
                }
            } while (answerGUIDPresent);
        }
        public static void DisplayAnswer(BlueprintAnswer answer, int cueNumber)
        {
            string cue_Name = answer.name.ToString();
            string cue_GUID = answer.AssetGuid.ToString();
            string HStackTitle = $"Answer #{cueNumber}";

            if (cueNumber == 0) HStackTitle = "Root Cue";

            Div(0, 10f);
            HStack("",0,
            () => HStack(HStackTitle, 0,
                () => GUILayout.BeginVertical(),
                () => BeginHorizontal(),
                () =>
                {
                    Label($"GUID: ", AutoWidth());
                    Space(10f);
                    TextField(ref cue_GUID, "cueGUID", Width(300f), AutoWidth());
                    Space(10f);
                    Label($"Name: ", AutoWidth());
                    Space(10f);
                    TextField(ref cue_Name, "cueName", Width(100f), AutoWidth());
                },
                () => EndHorizontal(),
                () => Label(answer.DisplayText, AutoHeight()),
                () => GUILayout.EndVertical()
                )
            );
        }
        public static void FindDialog(string dialogGUID)
        {
            firstCueDictionary.Clear();
            cueDictionary.Clear();
            answerDictionary.Clear();

            BlueprintDialog dialog = ResourcesLibrary.TryGetBlueprint<BlueprintDialog>(dialogGUID);

            // TODO: throw a warning to let the user know that this dialog doesn't exist
            if (dialog == null) return;

            foreach(BlueprintCue currentCue in FindFirstCues(dialog.FirstCue.Cues))
            {
                firstCueDictionary.Add(currentCue, false);

                // Now, find all cues that these first cues point to
                FindCues(currentCue.AssetGuid, currentCue.Continue.Cues);
            }

            shouldRender = (firstCueDictionary.Count > 0);
        }
        public static List<BlueprintCue> FindFirstCues(List<BlueprintCueBaseReference> firstCueReferences)
        {
            List<BlueprintCue> firstCueList = new List<BlueprintCue>();

            // Find the firstCues
            foreach(BlueprintCueBaseReference firstCueReference in firstCueReferences)
            {
                BlueprintCue firstCue = ResourcesLibrary.TryGetBlueprint<BlueprintCue>(firstCueReference.Guid);
                if (firstCue == null) break;

                firstCueList.Add(firstCue);

                // Find all answers this cue points to
                if (firstCue.Answers.Count > 0) FindAnswers(firstCue.AssetGuid, firstCue.Answers);
            }

            return firstCueList;
        }       
        public static void FindCues(BlueprintGuid key, List<BlueprintCueBaseReference> cueReferences)
        {
            List<BlueprintCue> cues = new List<BlueprintCue>();
            foreach(BlueprintCueBaseReference currentCueReference in cueReferences)
            {
                BlueprintCue currentCue = ResourcesLibrary.TryGetBlueprint<BlueprintCue>(currentCueReference.Guid);
                if (currentCue == null) break;

                KeyValuePair<BlueprintCue, bool> currentCueKVP = new KeyValuePair<BlueprintCue, bool>(currentCue, false);
                cueDictionary.Add(key, currentCueKVP);

                // Find all cues this cue points to
                if (currentCue.Continue.Cues.Count > 0) FindCues(currentCue.AssetGuid, currentCue.Continue.Cues);

                // Find all answers this cue points to
                if (currentCue.Answers.Count > 0) FindAnswers(currentCue.AssetGuid, currentCue.Answers);
            }
        }
        public static void FindAnswers(BlueprintGuid key, List<BlueprintAnswerBaseReference> answersReference)
        {
            foreach(BlueprintAnswerBaseReference currentAnswerListReference in answersReference)
            {
                BlueprintAnswersList currentAnswerList = ResourcesLibrary.TryGetBlueprint<BlueprintAnswersList>(currentAnswerListReference.Guid);
                List<KeyValuePair<BlueprintAnswer,bool>> answersList = new List<KeyValuePair<BlueprintAnswer,bool>>();
                foreach (BlueprintAnswerBaseReference currentAnswerReference in currentAnswerList.Answers)
                {
                    BlueprintAnswer currentAnswer = ResourcesLibrary.TryGetBlueprint<BlueprintAnswer>(currentAnswerReference.Guid);
                    if (currentAnswer == null) break;

                    KeyValuePair<BlueprintAnswer, bool> currentAnswerKVP = new KeyValuePair<BlueprintAnswer, bool>(currentAnswer, false);
                    answersList.Add(currentAnswerKVP);
                }

                KeyValuePair<List<KeyValuePair<BlueprintAnswer, bool>>, bool> answersListKVP = new KeyValuePair<List<KeyValuePair<BlueprintAnswer, bool>>, bool>(answersList, false);
                answerDictionary.Add(key, answersListKVP);
            }
        }
    }
}
