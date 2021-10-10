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
        //public static Dictionary<BlueprintCue, List<BlueprintAnswer>> cues = new Dictionary<BlueprintCue, List<BlueprintAnswer>>();

        // holds all cues
        public static Dictionary<BlueprintCue, KeyValuePair<List<BlueprintCue>, bool>> cuesDictionary;
        // holds all answers to cues (search by cue)
        public static Dictionary<BlueprintCue, KeyValuePair<List<BlueprintAnswer>, bool>> answersDictionary;
        // holds all cues that are triggered from answers
        public static Dictionary<BlueprintAnswer, KeyValuePair<List<BlueprintCue>, bool>> answerCuesDictionary;
        // holds all checks that are triggered from answers
        public static Dictionary<BlueprintAnswer, KeyValuePair<List<BlueprintCheck>, bool>> answerChecksDictionary;


        public static void onGUI()
        {
            if (!shouldRenderCue) return;

            int cueNumber = 1;
            foreach (KeyValuePair<BlueprintCue, KeyValuePair<List<BlueprintCue>, bool>> currentCue in cuesDictionary)
            {
                HStack($"Cue #{cueNumber}", 0,
                () =>
                {
                    BeginHorizontal();
                }
                );

                cueNumber++;
            }
        }

        public static void FindDialogue(string guid)
        {
            BlueprintDialog dialog = ResourcesLibrary.TryGetBlueprint<BlueprintDialog>(guid);

            // clear out all the dictionaries
            cuesDictionary.Clear();
            answersDictionary.Clear();
            answerCuesDictionary.Clear();
            answerChecksDictionary.Clear();

            foreach (BlueprintCue cue in dialog.FirstCue.Cues)
            {
                KeyValuePair<List<BlueprintCue>, bool> cueList = FindCueList(dialog.FirstCue.Cues);
                cuesDictionary.Add(cue, cueList);
            }

            shouldRenderCue = (cuesDictionary.Count > 0);
        }
        public static KeyValuePair<List<BlueprintCue>,bool> FindCueList(List<BlueprintCueBaseReference> cueReferenceList)
        {
            List<BlueprintCue> cues = new List<BlueprintCue>();
            foreach (BlueprintCueBaseReference cueBaseReference in cueReferenceList)
            {
                cues.Add(FindNextCue(cueBaseReference));
            }

            KeyValuePair<List<BlueprintCue>,bool> toReturn = new KeyValuePair<List<BlueprintCue>, bool>(cues, false);
            return toReturn;
        }
        public static BlueprintCue FindNextCue(BlueprintCueBaseReference nextCueReference)
        {
            BlueprintCue nextCue = ResourcesLibrary.TryGetBlueprint<BlueprintCue>(nextCueReference.Guid);

            return nextCue;
        }
        // TODO: Implement these as well
        /*
        public static KeyValuePair<List<BlueprintAnswer>,bool> FindAnswerList(List<BlueprintAnswerBaseReference> answerReferenceList)
        {

            return null;
        }
        public static BlueprintAnswer FindNextAnswer(BlueprintAnswerBaseReference answerReference)
        {

            return null;
        }
        //*/
    }
}
