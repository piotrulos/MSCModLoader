using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSCLoader
{
    public partial class Mod
    {
        /// <summary>
        /// Type of Function to setup
        /// </summary>
        public enum Setup
        {
            /// <summary>
            /// OnNewGame - Called once when new game (not continue old save) is started
            /// </summary>
            OnNewGame,
            /// <summary>
            /// OnMenuLoad - Setup function that is executed once in MainMenu
            /// </summary>
            OnMenuLoad,
            /// <summary>
            /// PreLoad - Phase 1 of mod loading (executed once after GAME scene is loaded)
            /// </summary>
            PreLoad,
            /// <summary>
            /// OnLoad - Phase 2 of mod loading (executed once GAME scene is fully loaded)
            /// </summary>
            OnLoad,
            /// <summary>
            /// PostLoad - Phase 3 of mod loading (executed once after all mods finished with Phase 2)
            /// </summary>
            PostLoad,
            /// <summary>
            /// OnSave - Executed once after game is being saved.
            /// </summary>
            OnSave,
            /// <summary>
            /// OnGUI - Works same way as unity OnGUI
            /// </summary>
            OnGUI,
            /// <summary>
            /// Update - Works same way as unity Update
            /// </summary>
            Update,
            /// <summary>
            /// FixedUpdate - Works same way as unity FixedUpdate
            /// </summary>
            FixedUpdate
        }

        /// <summary>
        /// true if mod is disabled
        /// </summary>
        public bool isDisabled { get; internal set; }
        internal bool hasUpdate = false;
        internal bool newFormat = false;
        internal bool menuCallbacks = false;
        internal int modErrors = 0;
        internal string compiledVersion;
        internal string fileName;
        internal ModsManifest metadata;    //Local metadata
        internal ModsManifest RemMetadata; //Remote metadata

        //Action list
        internal Action A_OnMenuLoad;  //Load in main menu
        internal Action A_OnNewGame;   //When New Game is started
        internal Action A_PreLoad;     //Phase 1 (mod loading)
        internal Action A_OnLoad;      //Phase 2 (mod loading)  
        internal Action A_PostLoad;    //Phase 3 (mod loading)
        internal Action A_OnSave;      //When game saves
        internal Action A_OnGUI;       //Calls unity OnGUI
        internal Action A_Update;      //Calls unity Update
        internal Action A_FixedUpdate; //Calls unity FixedUpdate

        internal List<Settings> modSettingsList = new List<Settings>();
        internal List<Settings> modSettingsDefault = new List<Settings>();
        internal List<Keybind> Keybinds = new List<Keybind>();
        internal List<Keybind> DefaultKeybinds = new List<Keybind>();
        /// <summary>
        /// Setup selected function for your mod
        /// </summary>
        /// <param name="functionType">Function type</param>
        /// <param name="function">Your own function to execute that type</param>
        public void SetupFunction(Setup functionType, Action function)
        {
            newFormat = true;
            switch (functionType)
            {
                case Setup.OnNewGame:
                    if (A_OnNewGame == null)
                        A_OnNewGame = function;
                    else
                        ModConsole.Error($"SetupMod() error for <b>{ID}</b>. You already created <b>OnNewGame</b> function type.");
                    break;
                case Setup.OnMenuLoad:
                    menuCallbacks = true;
                    if (A_OnMenuLoad == null)
                        A_OnMenuLoad = function;
                    else
                        ModConsole.Error($"SetupMod() error for <b>{ID}</b>. You already created <b>OnMenuLoad</b> function type.");
                    break;
                case Setup.PreLoad:
                    if (A_PreLoad == null)
                        A_PreLoad = function;
                    else
                        ModConsole.Error($"SetupMod() error for <b>{ID}</b>. You already created <b>PreLoad</b> function type.");
                    break;
                case Setup.OnLoad:
                    if (A_OnLoad == null)
                        A_OnLoad = function;
                    else
                        ModConsole.Error($"SetupMod() error for <b>{ID}</b>. You already created <b>OnLoad</b> function type.");
                    break;
                case Setup.PostLoad:
                    if (A_PostLoad == null)
                        A_PostLoad = function;
                    else
                        ModConsole.Error($"SetupMod() error for <b>{ID}</b>. You already created <b>PostLoad</b> function type.");
                    break;
                case Setup.OnSave:
                    if (A_OnSave == null)
                        A_OnSave = function;
                    else
                        ModConsole.Error($"SetupMod() error for <b>{ID}</b>. You already created <b>OnSave</b> function type.");
                    break;
                case Setup.OnGUI:
                    if (A_OnGUI == null)
                        A_OnGUI = function;
                    else
                        ModConsole.Error($"SetupMod() error for <b>{ID}</b>. You already created <b>OnGUI</b> function type.");
                    break;
                case Setup.Update:
                    if (A_Update == null)
                        A_Update = function;
                    else
                        ModConsole.Error($"SetupMod() error for <b>{ID}</b>. You already created <b>Update</b> function type.");
                    break;
                case Setup.FixedUpdate:
                    if (A_FixedUpdate == null)
                        A_FixedUpdate = function;
                    else
                        ModConsole.Error($"SetupMod() error for <b>{ID}</b>. You already created <b>FixedUpdate</b> function type.");
                    break;
                default:
                    ModConsole.Print("The hell happened here? That's impossible.");
                    break;
            }

        }
    }
}