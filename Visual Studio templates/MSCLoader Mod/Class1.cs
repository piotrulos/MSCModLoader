﻿using MSCLoader;
using UnityEngine;

namespace $safeprojectname$  
{
    public class $safeprojectname$ : Mod
    {
        public override string ID => "$safeprojectname$"; // Your (unique) mod ID 
        public override string Name => "$modName$"; // Your mod name
        public override string Author => "$modAuthor$"; // Name of the Author (your name)
        public override string Version => "$modVersion$"; // Version
        public override string Description => ""; // Short description of your mod

        public override void ModSetup()
        {$if$ ($setOnMenuLoad$ == true)
            SetupFunction(Setup.OnMenuLoad, Mod_OnMenuLoad);$endif$ $if$ ($setOnNewGame$ == true)
            SetupFunction(Setup.OnNewGame, Mod_OnNewGame);$endif$ $if$ ($setPreLoad$ == true)
            SetupFunction(Setup.PreLoad, Mod_PreLoad);$endif$ $if$ ($setOnLoad$ == true)
            SetupFunction(Setup.OnLoad, Mod_OnLoad);$endif$ $if$ ($setPostLoad$ == true)
            SetupFunction(Setup.PostLoad, Mod_PostLoad);$endif$ $if$ ($setOnSave$ == true)
            SetupFunction(Setup.OnSave, Mod_OnSave);$endif$ $if$ ($setOnGUI$ == true)
            SetupFunction(Setup.OnGUI, Mod_OnGUI);$endif$ $if$ ($setUpdate$ == true)
            SetupFunction(Setup.Update, Mod_Update);$endif$ $if$ ($setFixedUpdate$ == true)
            SetupFunction(Setup.FixedUpdate, Mod_FixedUpdate);$endif$
            SetupFunction(Setup.ModSettings, Mod_Settings);
        }

        private void Mod_Settings()
        {
            // All settings should be created here. 
            // DO NOT put anything that isn't settings or keybinds in here!
        }
        $if$ ($setOnMenuLoad$ == true)
        private void Mod_OnMenuLoad()
        {
            // Called once, when the mod is loaded in the main menu
        }$endif$ $if$ ($setOnNewGame$ == true)
        private void Mod_OnNewGame()
        {
            // Called once, when creating a new game. This is useful for deleting old mod saves
        }$endif$ $if$ ($setPreLoad$ == true)
        private void Mod_PreLoad()
        {
            // Called once, right after GAME scene loads but before the game is fully loaded
        }$endif$ $if$ ($setOnLoad$ == true)
        private void Mod_OnLoad()
        {
            // Called once, when mod is loading after game is fully loaded
        }$endif$ $if$ ($setPostLoad$ == true)
        private void Mod_PostLoad()
        {
            // Called once, after all mods finished OnLoad
        }$endif$ $if$ ($setOnSave$ == true)
        private void Mod_OnSave()
        {
            // Called once, when save and quit
            // Serialize your save file here.
        }$endif$ $if$ ($setOnGUI$ == true)
        private void Mod_OnGUI()
        {
            // Draw unity OnGUI() here
        }$endif$ $if$ ($setUpdate$ == true)
        private void Mod_Update()
        {
            // Update is called once per frame
        }$endif$ $if$ ($setFixedUpdate$ == true)
        private void Mod_FixedUpdate()
        {
            // FixedUpdate is called once per fixed frame
        }$endif$
    }
}
