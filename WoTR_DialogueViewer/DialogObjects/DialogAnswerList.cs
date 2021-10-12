using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTR_DialogueViewer.DialogueObjects
{
    class DialogAnswerList
    {
        public List<DialogAnswer> answers { get; set; }
        public bool isHidden;

        public DialogAnswerList(List<DialogAnswer> answers, bool isHidden = false)
        {
            this.answers = answers;
            this.isHidden = isHidden;
        }
    }
}
