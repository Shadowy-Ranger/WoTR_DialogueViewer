using Kingmaker.DialogSystem.Blueprints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTR_DialogueViewer.DialogueObjects
{
    class DialogAnswer
    {
        public BlueprintAnswer answer { get; set; }
        public bool isHidden;

        public DialogAnswer(BlueprintAnswer answer, bool isHidden = false)
        {
            this.answer = answer;
            this.isHidden = isHidden;
        }
        public override bool Equals(Object obj)
        {
            if (obj.GetType() != typeof(DialogAnswer)) return false;
            if (((DialogAnswer)obj).answer.AssetGuid == this.answer.AssetGuid) return true;

            return false;
        }
    }
}
