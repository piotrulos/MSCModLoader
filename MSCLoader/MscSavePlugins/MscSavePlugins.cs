using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

namespace MscSavePlugins
{
    public class MscSavePlugins : Mod
    {
        public override string ID { get { return "MscSavePlugins"; } }
        public override string Name { get { return "MSC Save Plugins"; } }
        public override string Author { get { return "Djoe45"; } }
        public override string Version { get { return "1.0.0"; } }

        private Keybind saveKey = new Keybind("KeyID", "Manuel Save", KeyCode.F1);


        public override void OnLoad()
        {
            Keybind.Add(this, saveKey);

        }

        public override void Update()
        {
            if (saveKey.IsDown()) {savemanuel(); };
        }

        public override void OnGUI()
        {

        }

        private void savemanuel()
        {
            ModConsole.Print("Savegame...");
            PlayMakerFSM.BroadcastEvent("SAVEGAME");
            Application.LoadLevel("MainMenu");
        }
    }
}
