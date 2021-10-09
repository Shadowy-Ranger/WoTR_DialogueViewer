using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityModManagerNet;

namespace WoTR_DialogueViewer
{
    public class Settings : UnityModManager.ModSettings
    {
        // DialogueSearch
        public string dialogueGUID = "";
        public bool searchResults = false;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
}
