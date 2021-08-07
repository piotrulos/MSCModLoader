using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MSCLoader.Helper
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// Redirect Only [TO USE THIS YOU NEED TO GET REFERENCES SEPARATELY]
    /// IF references doesn't exists it does nothing.
    /// </summary>
    public static class ModHelper
    {
        public static void MakePickable(this GameObject gameObject, bool includeTag = true) => MSCLoaderHelpers.ModHelper.MakePickable(gameObject, includeTag);
        public static void MakePickable(this Transform transform, bool includeTag = true) => MSCLoaderHelpers.ModHelper.MakePickable(transform, includeTag);
        public static Transform GetTransform(string parentPath, string childPath) => MSCLoaderHelpers.ModHelper.GetTransform(parentPath, childPath);
        public static GameObject GetGameObject(string parentPath, string childPath) => MSCLoaderHelpers.ModHelper.GetGameObject(parentPath, childPath);
        public static void PlaySound3D(this Transform transform, string type, string variation, float volume = 1f, float pitch = 1f) => MSCLoaderHelpers.ModHelper.PlaySound3D(transform, type, variation, volume, pitch);
        public static void PlaySound3D(this Vector3 vector3, string type, string variation, float volume = 1f, float pitch = 1f) => MSCLoaderHelpers.ModHelper.PlaySound3D(vector3, type, variation, volume , pitch);
        public static T SelectRandom<T>(this IList<T> list) => MSCLoaderHelpers.ModHelper.SelectRandom<T>(list);
        public static bool InLayerMask(this LayerMask layerMask, int layer) => MSCLoaderHelpers.ModHelper.InLayerMask(layerMask, layer);
        public static bool InLayerMask(this LayerMask layerMask, string layer) => MSCLoaderHelpers.ModHelper.InLayerMask(layerMask, layer);
        public static void SetParent(this Transform transform, Transform parent, Vector3 position, Vector3 rotation, Vector3 scale, string name = "") => MSCLoaderHelpers.ModHelper.SetParent(transform, parent, position, rotation, scale, name);
        public static void OpenFolder(string path) => MSCLoaderHelpers.ModHelper.OpenFolder(path);
        public static void OpenWebsite(string url) => MSCLoaderHelpers.ModHelper.OpenWebsite(url);
        public static bool IsWithinRange(this int value, int minValue, int maxValue) => MSCLoaderHelpers.ModHelper.IsWithinRange(value, minValue, maxValue);
        public static string GetImagesFolder() => MSCLoaderHelpers.ModHelper.GetImagesFolder();
        public static string GetRadioFolder() => MSCLoaderHelpers.ModHelper.GetRadioFolder();
        public static string GetCD1Folder() => MSCLoaderHelpers.ModHelper.GetCD1Folder();
        public static string GetCD2Folder() => MSCLoaderHelpers.ModHelper.GetCD2Folder();
        public static string GetCD3Folder() => MSCLoaderHelpers.ModHelper.GetCD3Folder();
    }
    public static class PlayMakerHelper
    {
        public static FsmBool FSMGUIUse { get; internal set; }
        public static FsmBool FSMGUIAssemble { get; internal set; }
        public static FsmBool FSMGUIDisassemble { get; internal set; }
        public static FsmBool FSMGUIBuy { get; internal set; }
        public static FsmBool FSMGUIDrive { get; internal set; }
        public static FsmBool FSMGUIPassenger { get; internal set; }
        public static FsmString FSMGUIInteraction { get; internal set; }
        public static FsmString FSMGUISubtitle { get; internal set; }
        public static bool GUIUse { get => FSMGUIUse.Value; set => FSMGUIUse.Value = value; }
        public static bool GUIAssemble { get => FSMGUIAssemble.Value; set => FSMGUIAssemble.Value = value; }
        public static bool GUIDisassemble { get => FSMGUIDisassemble.Value; set => FSMGUIDisassemble.Value = value; }
        public static bool GUIBuy { get => FSMGUIBuy.Value; set => FSMGUIBuy.Value = value; }
        public static bool GUIDrive { get => FSMGUIDrive.Value; set => FSMGUIDrive.Value = value; }
        public static bool GUIPassenger { get => FSMGUIPassenger.Value; set => FSMGUIPassenger.Value = value; }
        public static string GUIInteraction { get => FSMGUIInteraction.Value; set => FSMGUIInteraction.Value = value; }
        public static string GUISubtitle { get => FSMGUISubtitle.Value; set => FSMGUISubtitle.Value = value; }
        public static void Setup()
        {
            /* FSMGUIUse = GetGlobalVariable<FsmBool>("GUIuse");
             FSMGUIAssemble = GetGlobalVariable<FsmBool>("GUIassemble");
             FSMGUIDisassemble = GetGlobalVariable<FsmBool>("GUIdisassemble");
             FSMGUIBuy = GetGlobalVariable<FsmBool>("GUIbuy");
             FSMGUIDrive = GetGlobalVariable<FsmBool>("GUIdrive");
             FSMGUIPassenger = GetGlobalVariable<FsmBool>("GUIpassenger");
             FSMGUIInteraction = GetGlobalVariable<FsmString>("GUIinteraction");
             FSMGUISubtitle = GetGlobalVariable<FsmString>("GUIsubtitle");    */
          /*  foreach (var s in typeof(MSCLoaderHelpers.PlayMakerHelper).GetFields())
                ModConsole.Print("F " + s.Name);
            foreach (var s in typeof(MSCLoaderHelpers.PlayMakerHelper).GetProperties())
                ModConsole.Print("P " + s.Name);*/
            // Type s = typeof(MSCLoaderHelpers.PlayMakerHelper)
              typeof(MSCLoaderHelpers.PlayMakerHelper).GetProperty("FSMGUIUse", BindingFlags.Public | BindingFlags.Static).SetValue("nothing", FsmVariables.GlobalVariables.FindFsmBool("GUIuse"), null);
            typeof(MSCLoaderHelpers.PlayMakerHelper).GetProperty("FSMGUIAssemble", BindingFlags.Public | BindingFlags.Static).SetValue("nothing", FsmVariables.GlobalVariables.FindFsmBool("GUIassemble"), null);

            typeof(MSCLoaderHelpers.PlayMakerHelper).GetProperty("FSMGUIDisassemble", BindingFlags.Public | BindingFlags.Static).SetValue("nothing", FsmVariables.GlobalVariables.FindFsmBool("GUIdisassemble"), null); 
              typeof(MSCLoaderHelpers.PlayMakerHelper).GetProperty("FSMGUIBuy", BindingFlags.Public | BindingFlags.Static).SetValue("nothing", FsmVariables.GlobalVariables.FindFsmBool("GUIbuy"), null); 
              typeof(MSCLoaderHelpers.PlayMakerHelper).GetProperty("FSMGUIDrive", BindingFlags.Public | BindingFlags.Static).SetValue("nothing", FsmVariables.GlobalVariables.FindFsmBool("GUIdrive"), null); 
              typeof(MSCLoaderHelpers.PlayMakerHelper).GetProperty("FSMGUIPassenger", BindingFlags.Public | BindingFlags.Static).SetValue("nothing", FsmVariables.GlobalVariables.FindFsmBool("GUIpassenger"), null); 
              typeof(MSCLoaderHelpers.PlayMakerHelper).GetProperty("FSMGUIInteraction", BindingFlags.Public | BindingFlags.Static).SetValue("nothing", FsmVariables.GlobalVariables.FindFsmString("GUIinteraction"), null); 
              typeof(MSCLoaderHelpers.PlayMakerHelper).GetProperty("FSMGUISubtitle", BindingFlags.Public | BindingFlags.Static).SetValue("nothing", FsmVariables.GlobalVariables.FindFsmString("GUIsubtitle"), null); 
            /*  typeof(MSCLoaderHelpers.PlayMakerHelper).GetProperty("FSMGUIAssemble", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, MSCLoaderHelpers.PlayMakerHelper.GetGlobalVariable<FsmBool>("GUIassemble"), null); 
              typeof(MSCLoaderHelpers.PlayMakerHelper).GetProperty("FSMGUIDisassemble", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, MSCLoaderHelpers.PlayMakerHelper.GetGlobalVariable<FsmBool>("GUIdisassemble"), null); 
              typeof(MSCLoaderHelpers.PlayMakerHelper).GetProperty("FSMGUIBuy", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, MSCLoaderHelpers.PlayMakerHelper.GetGlobalVariable<FsmBool>("GUIbuy"), null);
              typeof(MSCLoaderHelpers.PlayMakerHelper).GetProperty("FSMGUIDrive", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, MSCLoaderHelpers.PlayMakerHelper.GetGlobalVariable<FsmBool>("GUIdrive"), null); 
              typeof(MSCLoaderHelpers.PlayMakerHelper).GetProperty("FSMGUIPassenger", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, MSCLoaderHelpers.PlayMakerHelper.GetGlobalVariable<FsmBool>("GUIpassenger"), null); 
              typeof(MSCLoaderHelpers.PlayMakerHelper).GetProperty("FSMGUIInteraction", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, MSCLoaderHelpers.PlayMakerHelper.GetGlobalVariable<FsmBool>("GUIinteraction"), null);
              typeof(MSCLoaderHelpers.PlayMakerHelper).GetProperty("FSMGUISubtitle", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, MSCLoaderHelpers.PlayMakerHelper.GetGlobalVariable<FsmBool>("GUIsubtitle"),null);*/
            //  fi
               FSMGUIUse = GetGlobalVariable<FsmBool>("GUIuse");
               FSMGUIAssemble = GetGlobalVariable<FsmBool>("GUIassemble");
               FSMGUIDisassemble = GetGlobalVariable<FsmBool>("GUIdisassemble");
               FSMGUIBuy = GetGlobalVariable<FsmBool>("GUIbuy");
               FSMGUIDrive = GetGlobalVariable<FsmBool>("GUIdrive");
               FSMGUIPassenger = GetGlobalVariable<FsmBool>("GUIpassenger");
               FSMGUIInteraction = GetGlobalVariable<FsmString>("GUIinteraction");
               FSMGUISubtitle = GetGlobalVariable<FsmString>("GUIsubtitle");
        }
        public static PlayMakerFSM GetPlayMakerFSM(this GameObject gameObject, string fsmName) => MSCLoaderHelpers.PlayMakerHelper.GetPlayMakerFSM(gameObject, fsmName);
        public static PlayMakerFSM GetPlayMakerFSM(this Transform transform, string fsmName) => MSCLoaderHelpers.PlayMakerHelper.GetPlayMakerFSM(transform, fsmName);
        public static FsmState GetState(this PlayMakerFSM fsm, string stateName) => MSCLoaderHelpers.PlayMakerHelper.GetState(fsm, stateName);
        public static FsmState GetState(this PlayMakerFSM fsm, int stateIndex) => MSCLoaderHelpers.PlayMakerHelper.GetState(fsm, stateIndex);
        public static T GetAction<T>(this FsmState state, int actionIndex) where T : FsmStateAction => MSCLoaderHelpers.PlayMakerHelper.GetAction<T>(state, actionIndex);
        public static T GetAction<T>(this PlayMakerFSM fsm, string stateName, int actionIndex) where T : FsmStateAction => MSCLoaderHelpers.PlayMakerHelper.GetAction<T>(fsm, stateName, actionIndex);
        public static T GetAction<T>(this PlayMakerFSM fsm, int stateIndex, int actionIndex) where T : FsmStateAction => MSCLoaderHelpers.PlayMakerHelper.GetAction<T>(fsm, stateIndex, actionIndex);
        public static void InsertAction(this FsmState state, int actionIndex, FsmStateAction action) => MSCLoaderHelpers.PlayMakerHelper.InsertAction(state, actionIndex, action);
        public static void InsertAction(this PlayMakerFSM fsm, string stateName, int actionIndex, FsmStateAction action) => MSCLoaderHelpers.PlayMakerHelper.InsertAction(fsm, stateName, actionIndex, action);
        public static void InsertAction(this PlayMakerFSM fsm, int stateIndex, int actionIndex, FsmStateAction action) => MSCLoaderHelpers.PlayMakerHelper.InsertAction(fsm, stateIndex, actionIndex, action);
        public static void AddAction(this FsmState state, FsmStateAction action) => MSCLoaderHelpers.PlayMakerHelper.AddAction(state, action);
        public static void AddAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action) => MSCLoaderHelpers.PlayMakerHelper.AddAction(fsm, stateName, action);
        public static void AddAction(this PlayMakerFSM fsm, int stateIndex, FsmStateAction action) => MSCLoaderHelpers.PlayMakerHelper.AddAction(fsm, stateIndex, action);
        public static void ReplaceAction(this FsmState state, int actionIndex, FsmStateAction action) => MSCLoaderHelpers.PlayMakerHelper.ReplaceAction(state, actionIndex, action);
        public static void ReplaceAction(this PlayMakerFSM fsm, string stateName, int actionIndex, FsmStateAction action) => MSCLoaderHelpers.PlayMakerHelper.ReplaceAction(fsm, stateName, actionIndex, action);
        public static void ReplaceAction(this PlayMakerFSM fsm, int stateIndex, int actionIndex, FsmStateAction action) => MSCLoaderHelpers.PlayMakerHelper.ReplaceAction(fsm, stateIndex, actionIndex, action);
        public static void RemoveAction(this FsmState state, int actionIndex) => MSCLoaderHelpers.PlayMakerHelper.RemoveAction(state, actionIndex);
        public static void RemoveAction(this FsmState state, FsmStateAction action) => MSCLoaderHelpers.PlayMakerHelper.RemoveAction(state, action);
        public static void RemoveAction(this PlayMakerFSM fsm, string stateName, int actionIndex) => MSCLoaderHelpers.PlayMakerHelper.RemoveAction(fsm, stateName, actionIndex);
        public static void RemoveAction(this PlayMakerFSM fsm, int stateIndex, int actionIndex) => MSCLoaderHelpers.PlayMakerHelper.RemoveAction(fsm, stateIndex, actionIndex);
        public static T GetVariable<T>(this PlayMakerFSM fsm, string name) where T : NamedVariable => MSCLoaderHelpers.PlayMakerHelper.GetVariable<T>(fsm, name);
        public static T GetGlobalVariable<T>(string name) where T : NamedVariable => MSCLoaderHelpers.PlayMakerHelper.GetGlobalVariable<T>(name);
        public static T FindVariable<T>(this FsmVariables variables, string name) where T : NamedVariable => MSCLoaderHelpers.PlayMakerHelper.FindVariable<T>(variables, name);
        public static void Initialize(this PlayMakerFSM fsm) => MSCLoaderHelpers.PlayMakerHelper.Initialize(fsm);
    }
    public class CallAction : FsmStateAction
    {
        public Action actionToCall;
        public CallAction(Action action) { actionToCall = action; }

        public override void OnEnter()
        {
            actionToCall?.Invoke();
            Finish();
        }
    }
    public static class PlayMakerProxyHelper
    {
        public static PlayMakerArrayListProxy GetArrayListProxy(this GameObject gameObject, string referenceName) => MSCLoaderHelpers.PlayMakerProxyHelper.GetArrayListProxy(gameObject, referenceName);
        public static PlayMakerArrayListProxy GetArrayListProxy(this Transform transform, string referenceName) => MSCLoaderHelpers.PlayMakerProxyHelper.GetArrayListProxy(transform, referenceName);
        public static void Add(this PlayMakerArrayListProxy proxy, object item, bool clear = false) => MSCLoaderHelpers.PlayMakerProxyHelper.Add(proxy, item, clear);
        public static void Add(this PlayMakerArrayListProxy proxy, IEnumerable<object> items, bool clear = false) => MSCLoaderHelpers.PlayMakerProxyHelper.Add(proxy, items, clear);
        public static void Clear(this PlayMakerArrayListProxy proxy, bool clearPrefill = false) => MSCLoaderHelpers.PlayMakerProxyHelper.Clear(proxy, clearPrefill);
        public static void AddPrefill(this PlayMakerArrayListProxy proxy, object item, bool clear = false) => MSCLoaderHelpers.PlayMakerProxyHelper.AddPrefill(proxy, item, clear);
        public static void AddPrefill(this PlayMakerArrayListProxy proxy, IEnumerable<object> items, bool clear = false) => MSCLoaderHelpers.PlayMakerProxyHelper.AddPrefill(proxy, items, clear);
        public static void ClearPrefill(this PlayMakerArrayListProxy proxy) => MSCLoaderHelpers.PlayMakerProxyHelper.ClearPrefill(proxy);
        public static PlayMakerHashTableProxy GetHashTableProxy(this GameObject gameObject, string referenceName) => MSCLoaderHelpers.PlayMakerProxyHelper.GetHashTableProxy(gameObject, referenceName);
        public static PlayMakerHashTableProxy GetHashTableProxy(this Transform transform, string referenceName) => MSCLoaderHelpers.PlayMakerProxyHelper.GetHashTableProxy(transform, referenceName);
        public static void Add(this PlayMakerHashTableProxy proxy, string key, object item, bool clear = false) => MSCLoaderHelpers.PlayMakerProxyHelper.Add(proxy, key, item, clear);
        public static void Add(this PlayMakerHashTableProxy proxy, IEnumerable<string> keys, IEnumerable<object> items, bool clear = false) => MSCLoaderHelpers.PlayMakerProxyHelper.Add(proxy, keys, items, clear);
        public static void Clear(this PlayMakerHashTableProxy proxy, bool clearPrefill = false) => MSCLoaderHelpers.PlayMakerProxyHelper.Clear(proxy, clearPrefill);
        public static void AddPrefill(this PlayMakerHashTableProxy proxy, string key, object item, bool clear = false) => MSCLoaderHelpers.PlayMakerProxyHelper.AddPrefill(proxy, key, item, clear);
        public static void AddPrefill(this PlayMakerHashTableProxy proxy, IEnumerable<string> keys, IEnumerable<object> items, bool clear = false) => MSCLoaderHelpers.PlayMakerProxyHelper.AddPrefill(proxy, keys, items, clear);
        public static void ClearPrefill(this PlayMakerHashTableProxy proxy) => MSCLoaderHelpers.PlayMakerProxyHelper.ClearPrefill(proxy);
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
