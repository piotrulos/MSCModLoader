using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.IO;

namespace MSCLoader
{
    internal class MenuElementList : MonoBehaviour
    {
        public Mod mod;
        [Header("Values")]
        public RawImage icon;
        public Text Title, Author, Description, QuickInfo, WarningInfo, WarningText;
        public Button SettingsBtn, KeybindsBtn, MoreInfoBtn;
        public Toggle DisableMod;
        public Texture2D invalidIcon;
        [Header("Additional Values")]
        public Button DownloadUpdateBtn, OpenDownloadWebsiteBtn;
        public Text DownloadInfoTxt;
        [Header("Anim stuff")]
        public GameObject quickInfo, warningInfo;
        public RectTransform contents;
        private bool collapsed;
        private bool anim;

        public void ModInfoFill()
        {
            if (mod != null)
            {
                if (mod.isDisabled)
                {
                    Title.text = $"<color=red>{mod.Name}</color>";
                    WarningText.gameObject.SetActive(true);
                    WarningText.text = "Mod is disabled";
                }
                else
                {
                    Title.text = mod.Name;
                    WarningText.text = string.Empty;
                    WarningText.gameObject.SetActive(false);
                }
                Author.text = $"by <color=orange>{mod.Author}</color> (<color=aqua>{mod.Version}</color>)";
                if (string.IsNullOrEmpty(mod.Description))
                    Description.text = "No short description provided...";
                else
                    Description.text = mod.Description;
                DisableMod.isOn = mod.isDisabled;
                DisableMod.onValueChanged.AddListener(DisableThisMod);
                QuickInfo.text = $"<color=yellow>ID:</color> <color=aqua>{mod.ID}</color> (MSCLoader <color=yellow>{mod.compiledVersion}</color>){Environment.NewLine}";
                if (mod.hasUpdate)
                    QuickInfo.text += $"<color=yellow>Version:</color> <color=aqua>{mod.Version}</color> (<color=lime>{mod.RemMetadata.version} available</color>){Environment.NewLine}";
                else
                    QuickInfo.text += $"<color=yellow>Version:</color> <color=aqua>{mod.Version}</color>{Environment.NewLine}";
                QuickInfo.text += $"<color=yellow>Author:</color> <color=aqua>{mod.Author}</color>";

                if (mod.metadata != null)
                {
                    if (!string.IsNullOrEmpty(mod.metadata.description))
                    {
                        if (string.IsNullOrEmpty(mod.Description))
                            Description.text = mod.metadata.description;
                    }
                    if (!string.IsNullOrEmpty(mod.metadata.icon.iconFileName))
                    {
                        if (File.Exists(Path.Combine(ModLoader.MetadataFolder, @"Mod Icons\" + mod.metadata.icon.iconFileName)))
                        {
                            try
                            {
                                Texture2D t2d = new Texture2D(1, 1);
                                t2d.LoadImage(File.ReadAllBytes(Path.Combine(ModLoader.MetadataFolder, @"Mod Icons\" + mod.metadata.icon.iconFileName)));
                                icon.texture = t2d;
                            }
                            catch (Exception e)
                            {
                                ModConsole.Error(e.Message);
                                System.Console.WriteLine(e);
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            if (mod.Icon != null)
                            {
                                Texture2D t2d = new Texture2D(1, 1);
                                t2d.LoadImage(mod.Icon);
                                icon.texture = t2d;
                            }
                        }
                        catch (Exception e)
                        {
                            ModConsole.Error(e.Message);
                            System.Console.WriteLine(e);
                        }

                    }
                }
                else
                {
                    try
                    {
                        if (mod.Icon != null)
                        {
                            Texture2D t2d = new Texture2D(1, 1);
                            t2d.LoadImage(mod.Icon);
                            icon.texture = t2d;
                        }
                    }
                    catch (Exception e)
                    {
                        ModConsole.Error(e.Message);
                        System.Console.WriteLine(e);
                    }

                }
            }
        }
        public void InvalidMod(string name)
        {
            Title.text = $"<color=red>{name}</color>";
            WarningText.gameObject.SetActive(true);
            WarningText.text = "Failed to load";
            Author.text = string.Empty;
            Description.text = "Failed to load this file";
            icon.texture = invalidIcon;
        }

        public void UpdateInfoFill()
        {
            if (mod != null)
            {
                Title.text = $"<color=lime>{mod.Name}</color>";              
                Author.text = $"by <color=orange>{mod.Author}</color> (<color=aqua>{mod.Version}</color>)";

                if (mod.metadata != null)
                {
                    DownloadInfoTxt.text = $"Update available ({mod.RemMetadata.version})";
                    if (!string.IsNullOrEmpty(mod.metadata.description))
                    {
                        if (string.IsNullOrEmpty(mod.Description))
                            Description.text = mod.metadata.description;
                    }
                    if (!string.IsNullOrEmpty(mod.metadata.icon.iconFileName))
                    {
                        if (File.Exists(Path.Combine(ModLoader.MetadataFolder, @"Mod Icons\" + mod.metadata.icon.iconFileName)))
                        {
                            try
                            {
                                Texture2D t2d = new Texture2D(1, 1);
                                t2d.LoadImage(File.ReadAllBytes(Path.Combine(ModLoader.MetadataFolder, @"Mod Icons\" + mod.metadata.icon.iconFileName)));
                                icon.texture = t2d;
                            }
                            catch (Exception e)
                            {
                                ModConsole.Error(e.Message);
                                System.Console.WriteLine(e);
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            if (mod.Icon != null)
                            {
                                Texture2D t2d = new Texture2D(1, 1);
                                t2d.LoadImage(mod.Icon);
                                icon.texture = t2d;
                            }
                        }
                        catch (Exception e)
                        {
                            ModConsole.Error(e.Message);
                            System.Console.WriteLine(e);
                        }

                    }
                }
                else
                {
                    try
                    {
                        if (mod.Icon != null)
                        {
                            Texture2D t2d = new Texture2D(1, 1);
                            t2d.LoadImage(mod.Icon);
                            icon.texture = t2d;
                        }
                    }
                    catch (Exception e)
                    {
                        ModConsole.Error(e.Message);
                        System.Console.WriteLine(e);
                    }

                }
            }
        }
        public void DisableThisMod(bool ischecked)
        {
            if (mod.isDisabled != ischecked)
            {
                mod.isDisabled = ischecked;
                if (ischecked)
                {
                    mod.OnModDisabled();
                    Title.text = $"<color=red>{mod.Name}</color>";
                    WarningText.gameObject.SetActive(true);
                    WarningText.text = "Mod is disabled";
                    ModConsole.Print($"Mod <b><color=orange>{mod.Name}</color></b> has been <color=red><b>Disabled</b></color>");
                }
                else
                {
                    mod.OnModEnabled();
                    Title.text = mod.Name;
                    WarningText.text = string.Empty;
                    WarningText.gameObject.SetActive(false);
                    ModConsole.Print($"Mod <b><color=orange>{mod.Name}</color></b> has been <color=green><b>Enabled</b></color>");
                }
                ModMenu.SaveSettings(mod);
            }
        }

        public void ToggleInfo()
        {
            if (anim) return;
            if (collapsed)
            {
                StartCoroutine(Anim(true));
                quickInfo.SetActive(false);
            }
            else
            {
                StartCoroutine(Anim(false));
                quickInfo.SetActive(true);
            }
        }
        public void ToggleWarning()
        {
            if (anim) return;
            if (collapsed)
            {
                StartCoroutine(Anim(true));
                warningInfo.SetActive(false);
            }
            else
            {
                StartCoroutine(Anim(false));
                warningInfo.SetActive(true);
            }
        }
        IEnumerator Anim(bool expand)
        {

            anim = true;
            if (expand)
            {
                for (int i = 0; i < 50; i++)
                {
                    contents.localScale = new Vector3(1, (float)System.Math.Round(contents.localScale.y + 0.02f, 2), 1);
                    yield return null;
                }
                collapsed = false;
            }
            else
            {

                for (int i = 0; i < 50; i++)
                {
                    contents.localScale = new Vector3(1, (float)System.Math.Round(contents.localScale.y - 0.02f, 2), 1);
                    yield return null;
                }
                collapsed = true;

            }
            anim = false;
        }
    }
}