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
using WoTR_DialogueViewer.DialogueObjects;
using static ModKit.UI;

namespace WoTR_DialogueViewer
{
    class DialogueSearch
    {
        public static Settings settings => Main.settings;

        public static bool shouldRender = false;

        // list of rootCues
        public static List<DialogCue> firstCueList = 
            new List<DialogCue>();

        // dictionary of cues, keyed by the blueprint they are triggered by
        // - Cues can be called by a BlueprintCheck, BlueprintAnswer, or BlueprintCue
        public static Dictionary<BlueprintGuid, DialogCue> cueDictionary =
            new Dictionary<BlueprintGuid, DialogCue>();

        // dictionary of answers, keyed by the cue which holds the answerlist the answer is from
        // - Answers can be pointed towards by cues or first cues
        public static Dictionary<BlueprintGuid, DialogAnswerList> answerDictionary =
            new Dictionary<BlueprintGuid, DialogAnswerList>();

        // dictionary of checks, keyed by the answer they are called from
        public static Dictionary<BlueprintGuid, DialogCheck> checkDictionary =
            new Dictionary<BlueprintGuid, DialogCheck>();

        // dictionary of successCues, keyed by the check they are called from

        // dictionary of failureCues, keyed by the check they are called from

        public static void onGUI()
        {
            foreach(DialogCue currentFirstCue in firstCueList)
            {
                DisplayCue(currentFirstCue);
            }
        }
        public static void DisplayAllCues(DialogCue dialogCue)
        {
            if (cueDictionary.ContainsKey(dialogCue.cue.AssetGuid))
            {
                DisplayCue(cueDictionary[dialogCue.cue.AssetGuid]);
            }
        }
        public static void DisplayCue(DialogCue dialogCue)
        {
            BlueprintCue cue = dialogCue.cue;

            string cue_Name = cue.name.ToString();
            string cue_GUID = cue.AssetGuid.ToString();
            string cue_Speaker = "N/A";
            if (cue.Speaker != null && cue.Speaker.Blueprint != null) cue_Speaker = cue.Speaker.Blueprint.CharacterName;

            string title = "Cue";

            Div(0, 10f);
            DisclosureToggle(title, ref dialogCue.isHidden, 0f,
                () => HStack("", 0,
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
                ),
                () => HStack("", 0,
                    () => 
                    {
                        GUILayout.BeginVertical();
                        DisplayAllCues(dialogCue);

                        if (answerDictionary.ContainsKey(cue.AssetGuid))
                        {
                            DisplayAllAnswers(answerDictionary[cue.AssetGuid]);
                        }
                        GUILayout.EndVertical();
                    }
                )
            );
        }
        public static void DisplayAllAnswers(DialogAnswerList answerList)
        {
            foreach(DialogAnswer currentAnswer in answerList.answers)
            {
                DisplayAnswer(currentAnswer);
            }
        }
        public static void DisplayAnswer(DialogAnswer dialogAnswer)
        {
            BlueprintAnswer answer = dialogAnswer.answer;

            string cue_Name = answer.name.ToString();
            string cue_GUID = answer.AssetGuid.ToString();

            Div(0, 10f);
            DisclosureToggle("Answer", ref dialogAnswer.isHidden, 0f,
                () => HStack("", 0,
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
                ),
                () => HStack("", 0,
                    () =>
                    {
                        GUILayout.BeginVertical();  
                        if (checkDictionary.ContainsKey(answer.AssetGuid))
                        {
                            DisplayCheck(checkDictionary[answer.AssetGuid]);
                        }
                        GUILayout.EndVertical();
                    }
                )
            );
        }
        public static void DisplayCheck(DialogCheck dialogCheck)
        {
            BlueprintCheck check = dialogCheck.check;

            string cue_Name = check.name.ToString();
            string cue_GUID = check.AssetGuid.ToString();
            string title = $"Check";

            Div(0, 10f);
            DisclosureToggle(title, ref dialogCheck.isHidden, 0f,
                () => HStack("", 0,
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
                    () => GUILayout.EndVertical()
                ),
                () => HStack("", 0,
                    () =>
                    {
                        GUILayout.BeginVertical();
                        if (cueDictionary.ContainsKey(check.AssetGuid))
                        {
                            DisplayCue(cueDictionary[check.AssetGuid]);
                        }
                        GUILayout.EndVertical();
                    }
                )
            );
        }
        public static void FindDialog(string dialogGUID)
        {
            firstCueList.Clear();
            cueDictionary.Clear();
            answerDictionary.Clear();
            checkDictionary.Clear();

            BlueprintDialog dialog = ResourcesLibrary.TryGetBlueprint<BlueprintDialog>(dialogGUID);

            // TODO: throw a warning to let the user know that this dialog doesn't exist
            if (dialog == null) return;

            FindFirstCues(dialog.FirstCue.Cues);

            shouldRender = (firstCueList.Count > 0);
        }
        public static void FindFirstCues(List<BlueprintCueBaseReference> firstCueReferences)
        {
            foreach(BlueprintCueBaseReference currentCueReference in firstCueReferences)
            {
                BlueprintCue currentFirstCue = ResourcesLibrary.TryGetBlueprint<BlueprintCue>(currentCueReference.Guid);
                if (currentFirstCue != null)
                {
                    firstCueList.Add(new DialogCue(currentFirstCue));
                    if (currentFirstCue.Continue.Cues.Count > 0) FindCues(currentFirstCue.AssetGuid, currentFirstCue.Continue.Cues);
                    if (currentFirstCue.Answers.Count > 0) FindAnswers(currentFirstCue.AssetGuid, currentFirstCue.Answers);
                }
            }
        }       
        public static void FindCues(BlueprintGuid key, List<BlueprintCueBaseReference> cueReferences)
        {
            foreach(BlueprintCueBaseReference currentCueReference in cueReferences)
            {
                BlueprintCue currentCue = ResourcesLibrary.TryGetBlueprint<BlueprintCue>(currentCueReference.Guid);
                if (currentCue != null)
                {
                    DialogCue tempCue = new DialogCue(currentCue);
                    if (!cueDictionary.ContainsValue(tempCue) && !firstCueList.Contains(tempCue))
                    {
                        cueDictionary.Add(key, tempCue);

                        if (currentCue.Continue.Cues.Count > 0) FindCues(currentCue.AssetGuid, currentCue.Continue.Cues);
                        if (currentCue.Answers.Count > 0) FindAnswers(currentCue.AssetGuid, currentCue.Answers);
                    }
                }
            }
        }
        public static void FindAnswers(BlueprintGuid key, List<BlueprintAnswerBaseReference> answersReference)
        {
            foreach(BlueprintAnswerBaseReference answersListReference in answersReference)
            {
                BlueprintAnswersList currentAnswersList = ResourcesLibrary.TryGetBlueprint<BlueprintAnswersList>(answersListReference.Guid);
                if(currentAnswersList != null)
                {
                    List<DialogAnswer> answers = new List<DialogAnswer>();
                    foreach (BlueprintAnswerBaseReference currentAnswerReference in currentAnswersList.Answers)
                    {
                        BlueprintAnswer currentAnswer = ResourcesLibrary.TryGetBlueprint<BlueprintAnswer>(currentAnswerReference.Guid);
                        if (currentAnswer != null)
                        {
                            answers.Add(new DialogAnswer(currentAnswer));

                            if (currentAnswer.NextCue.Cues.Count > 0) FindChecks(currentAnswer.AssetGuid, currentAnswer.NextCue.Cues);
                        }
                    }
                    answerDictionary.Add(key, new DialogAnswerList(answers));
                }
            }
        }
        public static void FindChecks(BlueprintGuid key, List<BlueprintCueBaseReference> checkReferences)
        {
            foreach(BlueprintCueBaseReference currentCheckReference in checkReferences)
            {
                try
                {
                    BlueprintCheck currentCheck = ResourcesLibrary.TryGetBlueprint<BlueprintCheck>(currentCheckReference.Guid);
                    checkDictionary.Add(key, new DialogCheck(currentCheck));
                }
                catch (InvalidCastException e)
                {
                    BlueprintCue currentCue = ResourcesLibrary.TryGetBlueprint<BlueprintCue>(currentCheckReference.Guid);
                    cueDictionary.Add(key, new DialogCue(currentCue));

                    if (currentCue.Continue.Cues.Count > 0) FindCues(currentCue.AssetGuid, currentCue.Continue.Cues);
                }
                catch (Exception e) { };
            }
        }
    }
}
