using Kingmaker.DialogSystem.Blueprints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTR_DialogueViewer.DialogueObjects
{
    class DialogCue
    {
        public BlueprintCue cue { get; set; }
        public bool isHidden;

        public DialogCue(BlueprintCue cue, bool isHidden = false)
        {
            this.cue = cue;
            this.isHidden = isHidden;
        }
        public override bool Equals(Object obj)
        {
            if (obj.GetType() != typeof(DialogCue)) return false;
            if (((DialogCue)obj).cue.AssetGuid == this.cue.AssetGuid) return true;

            return false;
        }
    }
}
