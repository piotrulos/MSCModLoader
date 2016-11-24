using HutongGames.PlayMaker;
using MSCLoader;
using UnityEngine;

namespace MscCheat
{
    public class MscCheat : Mod
    {
        public override string ID { get { return "MscCheat"; } }
        public override string Name { get { return "Msc Cheat"; } }
        public override string Author { get { return "Djoe45"; } }
        public override string Version { get { return "1.0.0"; } }

        // Keybinds
        private Keybind survivalKey = new Keybind("CheatKey1", "Reset Survival Stat", KeyCode.Alpha1, KeyCode.LeftShift);
        private Keybind giveMoneyKey = new Keybind("CheatKey2", "Give 1000$", KeyCode.Alpha2, KeyCode.LeftShift);
        private Keybind changeDayKey = new Keybind("CheatKey3", "Change Day", KeyCode.Alpha3, KeyCode.LeftShift);

        public override void OnLoad()
        {
            Keybind.Add(this, survivalKey);
            Keybind.Add(this, giveMoneyKey);
            Keybind.Add(this, changeDayKey);

            ModConsole.Print("MSC Cheat plugin has been loaded!");
        }

        public override void Update()
        {
            if (survivalKey.IsDown()) { ResetSurvival(); };
            if (giveMoneyKey.IsDown()) { givemoney(); };
            if (changeDayKey.IsDown()) { changeday(); };
        }

        private void ResetSurvival()
        {
            ModConsole.Print("Reset Survival Value...");
            FsmVariables.GlobalVariables.FindFsmFloat("PlayerFatigue").Value = 0.0f;
            FsmVariables.GlobalVariables.FindFsmFloat("PlayerDirtiness").Value = 0.0f;
            FsmVariables.GlobalVariables.FindFsmFloat("PlayerDrunk").Value = 0.0f;
            FsmVariables.GlobalVariables.FindFsmFloat("PlayerHunger").Value = 0.0f;
            FsmVariables.GlobalVariables.FindFsmFloat("PlayerThirst").Value = 0.0f;
            FsmVariables.GlobalVariables.FindFsmFloat("PlayerUrine").Value = 0.0f;
        }

        private void givemoney()
        {
            ModConsole.Print("Add 1000$ !");
            var money = FsmVariables.GlobalVariables.FindFsmFloat("PlayerMoney").Value;
            FsmVariables.GlobalVariables.FindFsmFloat("PlayerMoney").Value = money + 1000;
        }

        private void changeday()
        {
            ModConsole.Print("Day Change !");
            var day = FsmVariables.GlobalVariables.FindFsmInt("GlobalDay").Value;
            if (day == 7)
            {
                FsmVariables.GlobalVariables.FindFsmInt("GlobalDay").Value = 1;
            }
            else
            {
                FsmVariables.GlobalVariables.FindFsmInt("GlobalDay").Value = day + 1;
            }
            ModConsole.Print("Now is Day:" + day);
        }
    }
}
