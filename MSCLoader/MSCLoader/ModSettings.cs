using System;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

namespace MSCLoader
{
	/// <summary>
	/// Handles the settings for all Mods.
	/// </summary>
	public class ModSettings : Mod
	{
		public override string ID { get { return "MSCLoader_Settings"; } }
		public override string Name { get { return "Settings"; } }
		public override string Version { get { return ModLoader.Version; } }
		public override string Author { get { return "piotrulos"; } }

		private static Mod selectedMod = null;

        private Keybind menuKey = new Keybind("Open", "Open menu", KeyCode.M, KeyCode.LeftControl);
        public SettingsView settings;

        /// <summary>
        /// Create Settings UI using UnityEngine.UI
        /// </summary>
        public void CreateSettingsUI()
        {
            
            GameObject settingsView = ModUI.CreateParent("MSCLoader Settings", true);
            settingsView.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 450);
            settingsView.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            settingsView.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            settingsView.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);

            settings = settingsView.AddComponent<SettingsView>();
            settingsView.GetComponent<SettingsView>().settingView = settingsView;

            GameObject settingsViewC = ModUI.CreateUIBase("MSCLoader SettingsContainer", settingsView);
            settingsViewC.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 450);
            settingsViewC.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            settingsViewC.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            settingsViewC.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            settingsViewC.AddComponent<Image>().color = Color.black;

            settingsView.GetComponent<SettingsView>().settingViewContainer = settingsViewC;

            GameObject title = ModUI.CreateTextBlock("Title", "Loaded Mods:", settingsViewC, TextAnchor.MiddleCenter, Color.white, false);
            title.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 20);
            title.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
            title.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
            title.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);
            title.GetComponent<Text>().fontSize = 16;
            title.GetComponent<Text>().fontStyle = FontStyle.Bold;

            GameObject goBack = ModUI.CreateUIBase("GoBackButton", title);
            goBack.GetComponent<RectTransform>().sizeDelta = new Vector2(80, 20);
            goBack.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f);
            goBack.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);
            goBack.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
            goBack.AddComponent<Image>();
            goBack.AddComponent<Button>().targetGraphic = goBack.GetComponent<Image>();
            ColorBlock cb = goBack.GetComponent<Button>().colors;
            cb.normalColor = Color.black;
            cb.highlightedColor = Color.red;
            goBack.GetComponent<Button>().colors = cb;
            goBack.GetComponent<Button>().targetGraphic = goBack.GetComponent<Image>();
            goBack.GetComponent<Button>().onClick.AddListener(() => settingsView.GetComponent<SettingsView>().goBack());

            settingsView.GetComponent<SettingsView>().goBackBtn = goBack;

            GameObject BtnTxt = ModUI.CreateTextBlock("Text", " < Back", goBack, TextAnchor.MiddleLeft, Color.white);
            BtnTxt.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            BtnTxt.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
            BtnTxt.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            BtnTxt.GetComponent<RectTransform>().sizeDelta = new Vector2(80, 20);
            BtnTxt.GetComponent<Text>().fontSize = 16;
            goBack.SetActive(false);
            //modList 
            GameObject modList = ModUI.CreateUIBase("ModList", settingsViewC);
            modList.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
            modList.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0);
            modList.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
            modList.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 430);
            modList.AddComponent<Image>().color = Color.black;
            modList.AddComponent<Mask>().showMaskGraphic = true;

            settingsView.GetComponent<SettingsView>().modList = modList;

            //ModView
            GameObject scrollbar = ModUI.CreateScrollbar(settingsViewC, 450, 10, 90);
            scrollbar.GetComponent<RectTransform>().anchorMin = new Vector2(1, 1);
            scrollbar.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            scrollbar.GetComponent<RectTransform>().pivot = new Vector2(1, 1);

            GameObject modView = ModListS(modList, scrollbar, "ModView");
            settingsView.GetComponent<SettingsView>().modView = modView;

            GameObject modSettings = ModUI.CreateUIBase("ModSettings", settingsViewC);
            modSettings.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
            modSettings.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0);
            modSettings.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
            modSettings.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 430);
            modSettings.AddComponent<Image>().color = Color.black;
            modSettings.AddComponent<Mask>().showMaskGraphic = true;

            GameObject modSettingsView = ModListS(modSettings, scrollbar, "ModSettingsView");
            settingsView.GetComponent<SettingsView>().modSettings = modSettings;
            settingsView.GetComponent<SettingsView>().ModSettingsView = modSettingsView;
            settingsView.GetComponent<SettingsView>().IDtxt = ModUI.CreateTextBlock("ID", "", modSettingsView).GetComponent<Text>();
            settingsView.GetComponent<SettingsView>().Nametxt = ModUI.CreateTextBlock("Name", "", modSettingsView).GetComponent<Text>();
            settingsView.GetComponent<SettingsView>().Versiontxt = ModUI.CreateTextBlock("Version", "", modSettingsView).GetComponent<Text>();
            settingsView.GetComponent<SettingsView>().Authortxt = ModUI.CreateTextBlock("Author", "", modSettingsView).GetComponent<Text>();

            //keybinds
            ModUI.Separator(modSettingsView, "Key Bindings");

            GameObject keybinds = ModUI.CreateUIBase("Keybinds", modSettingsView);
            keybinds.AddComponent<VerticalLayoutGroup>().spacing = 5;
            settingsView.GetComponent<SettingsView>().keybindsList = keybinds;

            ModUI.Separator(modSettingsView);

            modSettings.SetActive(false);
        }

 

        private static GameObject ModListS(GameObject modList, GameObject scrollbar, string name)
        {
            GameObject modView = ModUI.CreateUIBase(name, modList);
            modView.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
            modView.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
            modView.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);
            modView.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 430);
            modView.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            modView.AddComponent<VerticalLayoutGroup>().padding = new RectOffset(5, 5, 5, 5);
            modView.GetComponent<VerticalLayoutGroup>().spacing = 5;
            modView.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.UpperCenter;
            modView.GetComponent<VerticalLayoutGroup>().childForceExpandHeight = false;

            modList.AddComponent<ScrollRect>().content = modView.GetComponent<RectTransform>();
            modList.GetComponent<ScrollRect>().horizontal = false;
            modList.GetComponent<ScrollRect>().inertia = false;
            modList.GetComponent<ScrollRect>().movementType = ScrollRect.MovementType.Clamped;
            modList.GetComponent<ScrollRect>().scrollSensitivity = 30f;
            modList.GetComponent<ScrollRect>().verticalScrollbar = scrollbar.GetComponent<Scrollbar>();
            return modView;
        }

		// Reset keybinds
		private void resetBinds()
		{
			if (selectedMod != null)
			{
				// Delete file
				string path = ModLoader.ConfigFolder + selectedMod.ID + "\\keybinds.xml";
				File.WriteAllText(path, "");

				// Revert binds
				foreach (Keybind bind in Keybind.Get(selectedMod))
				{
					Keybind original = Keybind.DefaultKeybinds.Find(x => x.Mod == selectedMod && x.ID == bind.ID);

					if (original != null)
					{
						bind.Key = original.Key;
						bind.Modifier = original.Modifier;

						ModConsole.Print(original.Key.ToString() + " -> " + bind.Key.ToString());
					}
				}

				// Save binds
				SaveModBinds(selectedMod);
			}
		}
		
		/// <summary>
		/// Save all keybinds to config files.
		/// </summary>
		public static void SaveAllBinds()
		{
			foreach (Mod mod in ModLoader.LoadedMods)
			{
				SaveModBinds(mod);
			}
		}

		/// <summary>
		/// Save keybind for a single mod to config file.
		/// </summary>
		/// <param name="mod">The mod to save the config for.</param>
		public static void SaveModBinds(Mod mod)
		{
            
            
			string path = ModLoader.ConfigFolder + mod.ID + "\\keybinds.xml";

			// Clear file
			File.WriteAllText(path, "");


            // Write XML
            XmlDocument doc = new XmlDocument();
            XmlElement keybinds = doc.CreateElement(string.Empty, "Keybinds", string.Empty);
            
            
			foreach (Keybind bind in Keybind.Get(mod))
			{
				XmlElement keybind = doc.CreateElement(string.Empty, "Keybind", string.Empty);

				XmlElement name = doc.CreateElement(string.Empty, "ID", string.Empty);
				name.AppendChild(doc.CreateTextNode(bind.ID));
				keybind.AppendChild(name);

				XmlElement key = doc.CreateElement(string.Empty, "Key", string.Empty);
				key.AppendChild(doc.CreateTextNode(bind.Key.ToString()));
				keybind.AppendChild(key);

				XmlElement modifier = doc.CreateElement(string.Empty, "Modifier", string.Empty);
				modifier.AppendChild(doc.CreateTextNode(bind.Modifier.ToString()));
				keybind.AppendChild(modifier);

				keybinds.AppendChild(keybind);
               
                 }

			doc.AppendChild(keybinds);
			doc.Save(path);
             
        }

        /// <summary>
        /// Load all keybinds.
        /// </summary>
        public static void LoadBinds()
		{
			foreach (Mod mod in ModLoader.LoadedMods)
			{
				// Check if there are custom keybinds
				string path = Path.Combine(ModLoader.ConfigFolder, mod.ID + "\\keybinds.xml");

				if (!File.Exists(path))
				{
					SaveModBinds(mod);
					continue;
				}
				
				// Load XML
				XmlDocument doc = new XmlDocument();
				doc.Load(path);

				foreach (XmlNode keybind in doc.GetElementsByTagName("Keybind"))
				{
					XmlNode id = keybind.SelectSingleNode("ID");
					XmlNode key = keybind.SelectSingleNode("Key");
					XmlNode modifier = keybind.SelectSingleNode("Modifier");

					// Check if its valid and fetch
					if (id == null || key == null || modifier == null)
					{
						continue;
					}

					Keybind bind = Keybind.Keybinds.Find(x => x.Mod == mod && x.ID == id.InnerText);

					if (bind == null)
					{
						continue;
					}

					// Set bind
					try
					{
						KeyCode code = (KeyCode)Enum.Parse(typeof(KeyCode), key.InnerText);
						bind.Key = code;
					}
					catch (Exception e)
					{
						bind.Key = KeyCode.None;
                        ModConsole.Error(e.Message);
                    }

					try
					{
						KeyCode code = (KeyCode)Enum.Parse(typeof(KeyCode), modifier.InnerText);
						bind.Modifier = code;
					}
					catch (Exception e)
					{
						bind.Modifier = KeyCode.None;
                        ModConsole.Error(e.Message);
                    }
				}
			}
		}

		/// <summary>
		/// Load the keybinds.
		/// </summary>
		public override void OnLoad()
		{
			Keybind.Add(this, menuKey);
            CreateSettingsUI();
            settings.setVisibility(false);
            LoadBinds();
        }

		/// <summary>
		/// Open menu if the key is pressed.
		/// </summary>
		public override void Update()
		{
			// Open menu
			if (menuKey.IsDown())
			{
                settings.toggleVisibility();
			}
		}
	}
}
