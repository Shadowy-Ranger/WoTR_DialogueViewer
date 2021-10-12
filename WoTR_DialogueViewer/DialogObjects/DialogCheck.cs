using Kingmaker.DialogSystem.Blueprints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTR_DialogueViewer.DialogueObjects
{
    class DialogCheck
    {
        public BlueprintCheck check { get; set; }
        public bool isHidden;

        public DialogCheck(BlueprintCheck check, bool isHidden = false)
        {
            this.check = check;
            this.isHidden = isHidden;
        }
        public override bool Equals(Object obj)
        {
            if (obj.GetType() != typeof(DialogCheck)) return false;
            if (((DialogCheck)obj).check.AssetGuid == this.check.AssetGuid) return true;

            return false;
        }
    }
}
