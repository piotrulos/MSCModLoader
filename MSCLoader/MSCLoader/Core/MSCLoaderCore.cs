#if !Mini
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MSCLoader;

internal class MSCLoaderCore : MonoBehaviour
{
    private bool vsyncEnabled = false;
    private void OnLevelWasLoaded()
    {
        switch (Application.loadedLevelName)
        {
            case "MainMenu":
                if (ModLoader.allModsLoaded)
                {
                    ModLoader.Instance.mscUnloader.MSCLoaderReset();
                    ModLoader.unloader = true;
                    return;
                }
                ModLoader.PrepareModLoader();
                ModLoader.currentScene = CurrentScene.MainMenu;
                if (QualitySettings.vSyncCount != 0)
                    vsyncEnabled = true;
                if (ModMenu.forceMenuVsync.GetValue() && !vsyncEnabled)
                    QualitySettings.vSyncCount = 1; //vsync in menu
                if (GameObject.Find("MSCLoader Info") == null)
                {
                    ModLoader.Instance.MainMenuInfo();
                }
                if (ModLoader.IsReferencePresent("MSCCoreLibrary"))
                    ModLoader.TimeSchedulerCalls("stop");
                break;
            case "Intro":
                ModLoader.currentScene = CurrentScene.NewGameIntro;
                ModNewGameReset();
                break;
            case "GAME":
                ModLoader.currentScene = CurrentScene.Game;
                if (ModMenu.forceMenuVsync.GetValue() && !vsyncEnabled)
                    QualitySettings.vSyncCount = 0;

                ModLoader.Instance.menuInfoAnim.Play("fade_out");
                if (ModLoader.IsReferencePresent("MSCCoreLibrary"))
                    ModLoader.TimeSchedulerCalls("start");
                StartLoadingMods();
                ModMenu.ModMenuHandle();
                break;
            case "Ending":
                ModLoader.currentScene = CurrentScene.Ending;
                if (ModLoader.IsReferencePresent("MSCCoreLibrary"))
                    ModLoader.TimeSchedulerCalls("stop");
                break;
            default:
                break;
        }
    }
    void OnApplicationQuit()
    {
        //Save current log as "previous"
        if (File.Exists("output_log_previous.txt")) File.Delete("output_log_previous.txt");
        File.Copy("output_log.txt", "output_log_previous.txt");
    }

    private void ModNewGameReset()
    {
        if (!ModLoader.IsModsResetting)
        {
            ModLoader.IsModsResetting = true;
            StartCoroutine(ModLoader.Instance.NewGameMods());
        }
    }
    private void StartLoadingMods()
    {
        if (!ModLoader.allModsLoaded && !ModLoader.IsModsLoading)
        {
            ModLoader.IsModsLoading = true;
            StartCoroutine(ModLoader.Instance.LoadMods());
        }
    }

    internal void HandleCanv(GameObject go)
    {
        StartCoroutine(HandleCanvC(go));
    }

    IEnumerator HandleCanvC(GameObject go)
    {
        yield return null;
        go.SetActive(true);
    }

    internal void DownloadUpdateData()
    {
        StartCoroutine(ModLoader.Instance.CheckForRefModUpdates());
        ModLoader.Instance.updChecked = true;
    }

    internal void DownloadMetadata()
    {
        StartCoroutine(ModLoader.Instance.DownloadMetadataFiles());
    }

    internal void DownloadSingleUpdate(byte type, string mod = null)
    {
        StartCoroutine(ModLoader.Instance.DownloadSingleUpdateC(type, mod));
    }
    internal void DownloadModUpdates()
    {
        StartCoroutine(ModLoader.Instance.DownloadModsUpdatesC());
    }

    internal void UploadModFile(string ID, string ver, string key, bool isRef)
    {
        StartCoroutine(ModLoader.Instance.UploadModFile(ID, ver, key, isRef));
    }

    internal void DownloadRequiredFiles(List<string> refs, List<string> mods)
    {
        StartCoroutine(ModLoader.Instance.DownloadRequiredFilesC(refs, mods));
    }

}
#endif