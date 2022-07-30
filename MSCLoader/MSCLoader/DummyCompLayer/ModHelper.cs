#if !Mini
using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MSCLoader.Helper;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
[Obsolete("This class requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;'")]
public static class ModHelper
{
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void MakePickable(this GameObject gameObject, bool includeTag = true) => MSCLoaderHelpers.ModHelper.MakePickable(gameObject, includeTag);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void MakePickable(this Transform transform, bool includeTag = true) => MSCLoaderHelpers.ModHelper.MakePickable(transform, includeTag);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static Transform GetTransform(string parentPath, string childPath) => MSCLoaderHelpers.ModHelper.GetTransform(parentPath, childPath);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static GameObject GetGameObject(string parentPath, string childPath) => MSCLoaderHelpers.ModHelper.GetGameObject(parentPath, childPath);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void PlaySound3D(this Transform transform, string type, string variation, float volume = 1f, float pitch = 1f) => MSCLoaderHelpers.ModHelper.PlaySound3D(transform, type, variation, volume, pitch);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void PlaySound3D(this Vector3 vector3, string type, string variation, float volume = 1f, float pitch = 1f) => MSCLoaderHelpers.ModHelper.PlaySound3D(vector3, type, variation, volume, pitch);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static T SelectRandom<T>(this IList<T> list) => MSCLoaderHelpers.ModHelper.SelectRandom<T>(list);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static bool InLayerMask(this LayerMask layerMask, int layer) => MSCLoaderHelpers.ModHelper.InLayerMask(layerMask, layer);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static bool InLayerMask(this LayerMask layerMask, string layer) => MSCLoaderHelpers.ModHelper.InLayerMask(layerMask, layer);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void SetParent(this Transform transform, Transform parent, Vector3 position, Vector3 rotation, Vector3 scale, string name = "") => MSCLoaderHelpers.ModHelper.SetParent(transform, parent, position, rotation, scale, name);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void OpenFolder(string path) => MSCLoaderHelpers.ModHelper.OpenFolder(path);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void OpenWebsite(string url) => MSCLoaderHelpers.ModHelper.OpenWebsite(url);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static bool IsWithinRange(this int value, int minValue, int maxValue) => MSCLoaderHelpers.ModHelper.IsWithinRange(value, minValue, maxValue);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static string GetImagesFolder() => MSCLoaderHelpers.ModHelper.GetImagesFolder();
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static string GetRadioFolder() => MSCLoaderHelpers.ModHelper.GetRadioFolder();
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static string GetCD1Folder() => MSCLoaderHelpers.ModHelper.GetCD1Folder();
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static string GetCD2Folder() => MSCLoaderHelpers.ModHelper.GetCD2Folder();
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static string GetCD3Folder() => MSCLoaderHelpers.ModHelper.GetCD3Folder();
}

[Obsolete("This class requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;'")]
public static class PlayMakerHelper
{
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static FsmBool FSMGUIUse { get; internal set; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static FsmBool FSMGUIAssemble { get; internal set; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static FsmBool FSMGUIDisassemble { get; internal set; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static FsmBool FSMGUIBuy { get; internal set; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static FsmBool FSMGUIDrive { get; internal set; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static FsmBool FSMGUIPassenger { get; internal set; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static FsmString FSMGUIInteraction { get; internal set; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static FsmString FSMGUISubtitle { get; internal set; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static bool GUIUse { get => FSMGUIUse.Value; set => FSMGUIUse.Value = value; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static bool GUIAssemble { get => FSMGUIAssemble.Value; set => FSMGUIAssemble.Value = value; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static bool GUIDisassemble { get => FSMGUIDisassemble.Value; set => FSMGUIDisassemble.Value = value; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static bool GUIBuy { get => FSMGUIBuy.Value; set => FSMGUIBuy.Value = value; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static bool GUIDrive { get => FSMGUIDrive.Value; set => FSMGUIDrive.Value = value; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static bool GUIPassenger { get => FSMGUIPassenger.Value; set => FSMGUIPassenger.Value = value; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static string GUIInteraction { get => FSMGUIInteraction.Value; set => FSMGUIInteraction.Value = value; }
    [Obsolete("This property requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static string GUISubtitle { get => FSMGUISubtitle.Value; set => FSMGUISubtitle.Value = value; }
    [Obsolete("DON'T USE THIS WILL MESS UP REFERENCE")]
    public static void Setup()
    {
        typeof(MSCLoaderHelpers.PlayMakerHelper).GetProperty("FSMGUIUse", BindingFlags.Public | BindingFlags.Static).SetValue("nothing", FsmVariables.GlobalVariables.FindFsmBool("GUIuse"), null);
        typeof(MSCLoaderHelpers.PlayMakerHelper).GetProperty("FSMGUIAssemble", BindingFlags.Public | BindingFlags.Static).SetValue("nothing", FsmVariables.GlobalVariables.FindFsmBool("GUIassemble"), null);
        typeof(MSCLoaderHelpers.PlayMakerHelper).GetProperty("FSMGUIDisassemble", BindingFlags.Public | BindingFlags.Static).SetValue("nothing", FsmVariables.GlobalVariables.FindFsmBool("GUIdisassemble"), null);
        typeof(MSCLoaderHelpers.PlayMakerHelper).GetProperty("FSMGUIBuy", BindingFlags.Public | BindingFlags.Static).SetValue("nothing", FsmVariables.GlobalVariables.FindFsmBool("GUIbuy"), null);
        typeof(MSCLoaderHelpers.PlayMakerHelper).GetProperty("FSMGUIDrive", BindingFlags.Public | BindingFlags.Static).SetValue("nothing", FsmVariables.GlobalVariables.FindFsmBool("GUIdrive"), null);
        typeof(MSCLoaderHelpers.PlayMakerHelper).GetProperty("FSMGUIPassenger", BindingFlags.Public | BindingFlags.Static).SetValue("nothing", FsmVariables.GlobalVariables.FindFsmBool("GUIpassenger"), null);
        typeof(MSCLoaderHelpers.PlayMakerHelper).GetProperty("FSMGUIInteraction", BindingFlags.Public | BindingFlags.Static).SetValue("nothing", FsmVariables.GlobalVariables.FindFsmString("GUIinteraction"), null);
        typeof(MSCLoaderHelpers.PlayMakerHelper).GetProperty("FSMGUISubtitle", BindingFlags.Public | BindingFlags.Static).SetValue("nothing", FsmVariables.GlobalVariables.FindFsmString("GUIsubtitle"), null);
        FSMGUIUse = GetGlobalVariable<FsmBool>("GUIuse");
        FSMGUIAssemble = GetGlobalVariable<FsmBool>("GUIassemble");
        FSMGUIDisassemble = GetGlobalVariable<FsmBool>("GUIdisassemble");
        FSMGUIBuy = GetGlobalVariable<FsmBool>("GUIbuy");
        FSMGUIDrive = GetGlobalVariable<FsmBool>("GUIdrive");
        FSMGUIPassenger = GetGlobalVariable<FsmBool>("GUIpassenger");
        FSMGUIInteraction = GetGlobalVariable<FsmString>("GUIinteraction");
        FSMGUISubtitle = GetGlobalVariable<FsmString>("GUIsubtitle");
    }
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static PlayMakerFSM GetPlayMakerFSM(this GameObject gameObject, string fsmName) => MSCLoaderHelpers.PlayMakerHelper.GetPlayMakerFSM(gameObject, fsmName);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static PlayMakerFSM GetPlayMakerFSM(this Transform transform, string fsmName) => MSCLoaderHelpers.PlayMakerHelper.GetPlayMakerFSM(transform, fsmName);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static FsmState GetState(this PlayMakerFSM fsm, string stateName) => MSCLoaderHelpers.PlayMakerHelper.GetState(fsm, stateName);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static FsmState GetState(this PlayMakerFSM fsm, int stateIndex) => MSCLoaderHelpers.PlayMakerHelper.GetState(fsm, stateIndex);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static T GetAction<T>(this FsmState state, int actionIndex) where T : FsmStateAction => MSCLoaderHelpers.PlayMakerHelper.GetAction<T>(state, actionIndex);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static T GetAction<T>(this PlayMakerFSM fsm, string stateName, int actionIndex) where T : FsmStateAction => MSCLoaderHelpers.PlayMakerHelper.GetAction<T>(fsm, stateName, actionIndex);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static T GetAction<T>(this PlayMakerFSM fsm, int stateIndex, int actionIndex) where T : FsmStateAction => MSCLoaderHelpers.PlayMakerHelper.GetAction<T>(fsm, stateIndex, actionIndex);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void InsertAction(this FsmState state, int actionIndex, FsmStateAction action) => MSCLoaderHelpers.PlayMakerHelper.InsertAction(state, actionIndex, action);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void InsertAction(this PlayMakerFSM fsm, string stateName, int actionIndex, FsmStateAction action) => MSCLoaderHelpers.PlayMakerHelper.InsertAction(fsm, stateName, actionIndex, action);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void InsertAction(this PlayMakerFSM fsm, int stateIndex, int actionIndex, FsmStateAction action) => MSCLoaderHelpers.PlayMakerHelper.InsertAction(fsm, stateIndex, actionIndex, action);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void AddAction(this FsmState state, FsmStateAction action) => MSCLoaderHelpers.PlayMakerHelper.AddAction(state, action);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void AddAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action) => MSCLoaderHelpers.PlayMakerHelper.AddAction(fsm, stateName, action);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void AddAction(this PlayMakerFSM fsm, int stateIndex, FsmStateAction action) => MSCLoaderHelpers.PlayMakerHelper.AddAction(fsm, stateIndex, action);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void ReplaceAction(this FsmState state, int actionIndex, FsmStateAction action) => MSCLoaderHelpers.PlayMakerHelper.ReplaceAction(state, actionIndex, action);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void ReplaceAction(this PlayMakerFSM fsm, string stateName, int actionIndex, FsmStateAction action) => MSCLoaderHelpers.PlayMakerHelper.ReplaceAction(fsm, stateName, actionIndex, action);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void ReplaceAction(this PlayMakerFSM fsm, int stateIndex, int actionIndex, FsmStateAction action) => MSCLoaderHelpers.PlayMakerHelper.ReplaceAction(fsm, stateIndex, actionIndex, action);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void RemoveAction(this FsmState state, int actionIndex) => MSCLoaderHelpers.PlayMakerHelper.RemoveAction(state, actionIndex);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void RemoveAction(this FsmState state, FsmStateAction action) => MSCLoaderHelpers.PlayMakerHelper.RemoveAction(state, action);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void RemoveAction(this PlayMakerFSM fsm, string stateName, int actionIndex) => MSCLoaderHelpers.PlayMakerHelper.RemoveAction(fsm, stateName, actionIndex);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void RemoveAction(this PlayMakerFSM fsm, int stateIndex, int actionIndex) => MSCLoaderHelpers.PlayMakerHelper.RemoveAction(fsm, stateIndex, actionIndex);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static T GetVariable<T>(this PlayMakerFSM fsm, string name) where T : NamedVariable => MSCLoaderHelpers.PlayMakerHelper.GetVariable<T>(fsm, name);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static T GetGlobalVariable<T>(string name) where T : NamedVariable => MSCLoaderHelpers.PlayMakerHelper.GetGlobalVariable<T>(name);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static T FindVariable<T>(this FsmVariables variables, string name) where T : NamedVariable => MSCLoaderHelpers.PlayMakerHelper.FindVariable<T>(variables, name);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void Initialize(this PlayMakerFSM fsm) => MSCLoaderHelpers.PlayMakerHelper.Initialize(fsm);
}

[Obsolete("This class requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;'")]
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

[Obsolete("This class requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;'")]
public static class PlayMakerProxyHelper
{
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static PlayMakerArrayListProxy GetArrayListProxy(this GameObject gameObject, string referenceName) => MSCLoaderHelpers.PlayMakerProxyHelper.GetArrayListProxy(gameObject, referenceName);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static PlayMakerArrayListProxy GetArrayListProxy(this Transform transform, string referenceName) => MSCLoaderHelpers.PlayMakerProxyHelper.GetArrayListProxy(transform, referenceName);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void Add(this PlayMakerArrayListProxy proxy, object item, bool clear = false) => MSCLoaderHelpers.PlayMakerProxyHelper.Add(proxy, item, clear);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void Add(this PlayMakerArrayListProxy proxy, IEnumerable<object> items, bool clear = false) => MSCLoaderHelpers.PlayMakerProxyHelper.Add(proxy, items, clear);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void Clear(this PlayMakerArrayListProxy proxy, bool clearPrefill = false) => MSCLoaderHelpers.PlayMakerProxyHelper.Clear(proxy, clearPrefill);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void AddPrefill(this PlayMakerArrayListProxy proxy, object item, bool clear = false) => MSCLoaderHelpers.PlayMakerProxyHelper.AddPrefill(proxy, item, clear);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void AddPrefill(this PlayMakerArrayListProxy proxy, IEnumerable<object> items, bool clear = false) => MSCLoaderHelpers.PlayMakerProxyHelper.AddPrefill(proxy, items, clear);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void ClearPrefill(this PlayMakerArrayListProxy proxy) => MSCLoaderHelpers.PlayMakerProxyHelper.ClearPrefill(proxy);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static PlayMakerHashTableProxy GetHashTableProxy(this GameObject gameObject, string referenceName) => MSCLoaderHelpers.PlayMakerProxyHelper.GetHashTableProxy(gameObject, referenceName);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static PlayMakerHashTableProxy GetHashTableProxy(this Transform transform, string referenceName) => MSCLoaderHelpers.PlayMakerProxyHelper.GetHashTableProxy(transform, referenceName);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void Add(this PlayMakerHashTableProxy proxy, string key, object item, bool clear = false) => MSCLoaderHelpers.PlayMakerProxyHelper.Add(proxy, key, item, clear);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void Add(this PlayMakerHashTableProxy proxy, IEnumerable<string> keys, IEnumerable<object> items, bool clear = false) => MSCLoaderHelpers.PlayMakerProxyHelper.Add(proxy, keys, items, clear);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void Clear(this PlayMakerHashTableProxy proxy, bool clearPrefill = false) => MSCLoaderHelpers.PlayMakerProxyHelper.Clear(proxy, clearPrefill);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void AddPrefill(this PlayMakerHashTableProxy proxy, string key, object item, bool clear = false) => MSCLoaderHelpers.PlayMakerProxyHelper.AddPrefill(proxy, key, item, clear);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void AddPrefill(this PlayMakerHashTableProxy proxy, IEnumerable<string> keys, IEnumerable<object> items, bool clear = false) => MSCLoaderHelpers.PlayMakerProxyHelper.AddPrefill(proxy, keys, items, clear);
    [Obsolete("This extension requires user to have 'Compatibility References' installed, consider not 'using MSCLoader.Helper;' and check expanded base extensions (usually under similar name)")]
    public static void ClearPrefill(this PlayMakerHashTableProxy proxy) => MSCLoaderHelpers.PlayMakerProxyHelper.ClearPrefill(proxy);
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#endif