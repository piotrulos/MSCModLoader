﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MSCLoader
{   
    /// <summary>
    /// List of possible scenes
    /// </summary>
    public enum CurrentScene
    {
        /// <summary>
        /// Main Menu
        /// </summary>
        MainMenu,
        /// <summary>
        /// Game Scene
        /// </summary>
        Game,
        /// <summary>
        /// Intro for new game
        /// </summary>
        NewGameIntro,
        /// <summary>
        /// End game scene
        /// </summary>
        Ending
    }

    public partial class ModLoader
    {
        /// <summary>
        /// Current scene
        /// </summary>
        public static CurrentScene CurrentScene { get; internal set; }

        /// <summary>
        /// Check if steam is present
        /// </summary>
        /// <returns>Valid steam detected.</returns>
        public static bool CheckSteam()
        {
            if (!string.IsNullOrEmpty(steamID) && steamID != "0")
                return true;
            else
                return false;
        }

        /// <summary>
        /// Check if steam release is from experimental branch
        /// </summary>
        /// <returns>Experimental detected.</returns>
        public static bool CheckIfExperimental()
        {
            if (!CheckSteam())
            {
                System.Console.WriteLine("Cannot check if it is experimental branch or not, because not valid steam installation detected");
                return false;
            }
            bool ret = Steamworks.SteamApps.GetCurrentBetaName(out string Name, 128);
            if (ret)
            {
                if (Name != "default_32bit") //32bit is NOT experimental branch
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Get Current Game Scene
        /// </summary>
        /// <returns>CurrentScene enum</returns>
        public static CurrentScene GetCurrentScene()
        {
            return CurrentScene;
        }

        /// <summary>
        /// Get Mod class of modID
        /// </summary>
        /// <param name="modID">Mod ID of other mod to check (Case sensitive)</param>
        /// <param name="ignoreEnabled">Include disabled mods</param>
        /// <returns>Mod class</returns>
        public static Mod GetMod(string modID, bool ignoreEnabled = false)
        {
            if (IsModPresent(modID))
            {
                return LoadedMods.Where(x => x.ID.Equals(modID) && !x.isDisabled).FirstOrDefault();
            }
            else if (ignoreEnabled)
            {
                return LoadedMods.Where(x => x.ID.Equals(modID)).FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// Check if other ModID is present and enabled
        /// </summary>
        /// <param name="ModID">Mod ID of other mod to check (Case sensitive)</param>
        /// <returns>true if mod ID is present</returns>
        public static bool IsModPresent(string ModID)
        {
            Mod m = LoadedMods.Where(x => x.ID.Equals(ModID) && !x.isDisabled).FirstOrDefault();
            if (m != null)
                return true;
            return false;
        }

        /// <summary>
        /// Mod settings folder, use this if you want save something. 
        /// </summary>
        /// <returns>Path to your mod settings folder</returns>
        /// <param name="mod">Your mod Class.</param>
        /// <example>Example Code in Mod subclass.
        /// <code source="Examples.cs" region="GetModConfigFolder" lang="C#" />
        /// Example from other than Mod subclass.
        /// <code source="Examples.cs" region="GetModConfigFolder2" lang="C#" />
        /// </example>
        public static string GetModSettingsFolder(Mod mod) => Path.Combine(SettingsFolder, mod.ID);

        /// <summary>
        /// [Obsolete] Change to GetModSettingsFolder()
        /// </summary>
        /// <returns>Path to your mod config folder</returns>
        /// <param name="mod">Your mod Class.</param>
        /// <example>Example Code in Mod subclass.
        /// <code source="Examples.cs" region="GetModConfigFolder" lang="C#" />
        /// Example from other than Mod subclass.
        /// <code source="Examples.cs" region="GetModConfigFolder2" lang="C#" />
        /// </example>
        [Obsolete("Rename to GetModSettingsFolder(), config is old unused name", true)]
        public static string GetModConfigFolder(Mod mod)
        {
            return Path.Combine(SettingsFolder, mod.ID);
        }

        /// <summary>
        /// Mod assets folder, use this if you want load custom content. 
        /// </summary>
        /// <returns>Path to your mod assets folder</returns>
        /// <param name="mod">Your mod Class.</param>
        /// <example>Example Code in Mod subclass.
        /// <code source="Examples.cs" region="GetModAssetsFolder" lang="C#" />
        /// Example from other than Mod subclass.
        /// <code source="Examples.cs" region="GetModAssetsFolder2" lang="C#" />
        /// </example> 
        public static string GetModAssetsFolder(Mod mod)
        {
            return Path.Combine(AssetsFolder, mod.ID);
        }
    }
}