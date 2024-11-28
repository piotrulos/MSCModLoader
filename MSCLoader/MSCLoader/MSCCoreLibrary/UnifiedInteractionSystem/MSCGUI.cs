#if !Mini 
using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;

namespace MSCLoader
{
    public static class MSCGUI
    {
        static bool assemble { set { _assemble.Value = value; } }
        static bool buy { set { _buy.Value = value; } }
        static bool disassemble { set { _disassemble.Value = value; } }
        static bool drive { set { _drive.Value = value; } }
        static bool passenger { set { _passenger.Value = value; } }
        static bool use { set { _use.Value = value; } }

        static readonly Dictionary<int, Action<bool>> interactions = new Dictionary<int, Action<bool>>();

        static MSCGUI()
        {
            interactions[0] = value => assemble = value;
            interactions[1] = value => buy = value;
            interactions[2] = value => disassemble = value;
            interactions[3] = value => drive = value;
            interactions[4] = value => passenger = value;
            interactions[5] = value => use = value;
        }

        internal static void DisplayIcon(Interactable.InteractionIcon icon, bool display) => interactions[(int)icon](display);

        internal static string InteractionText { set { _interaction.Value = value; } }

        /// <summary>
        /// Set this property to any string you want to display as subtitle, auto-hidden after a short time just as in vanilla.
        /// Returns the current subtitle displayed
        /// </summary>
        public static string Subtitle { set { _subtitle.Value = value; } get { return _subtitle.Value; } }

        #region FsmVariables
        static readonly FsmBool _assemble = FsmVariables.GlobalVariables.FindFsmBool("GUIassemble");
        static readonly FsmBool _buy = FsmVariables.GlobalVariables.FindFsmBool("GUIbuy");
        static readonly FsmBool _disassemble = FsmVariables.GlobalVariables.FindFsmBool("GUIdisassemble");
        static readonly FsmBool _drive = FsmVariables.GlobalVariables.FindFsmBool("GUIdrive");
        static readonly FsmBool _passenger = FsmVariables.GlobalVariables.FindFsmBool("GUIpassenger");
        static readonly FsmBool _use = FsmVariables.GlobalVariables.FindFsmBool("GUIuse");

        static readonly FsmString _gear = FsmVariables.GlobalVariables.FindFsmString("GUIgear");
        static readonly FsmString _interaction = FsmVariables.GlobalVariables.FindFsmString("GUIinteraction");
        static readonly FsmString _subtitle = FsmVariables.GlobalVariables.FindFsmString("GUIsubtitle");
        #endregion FsmVariables
    }
}
#endif