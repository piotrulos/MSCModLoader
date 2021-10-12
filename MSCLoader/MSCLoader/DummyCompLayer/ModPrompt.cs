using UnityEngine.Events;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace MSCLoader
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [System.Obsolete("Nothing", true)]

    public class ModPromptButton
    {
    }  
    
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [System.Obsolete("=> ModUI", true)]
    public class ModPrompt
    {
        [System.Obsolete("=> ModUI", true)]
        public ModPromptButton AddButton(string buttonText, UnityAction action)
        {
            return null;
        }
        [System.Obsolete("=> ModUI.ShowMessage", true)]
        public static ModPrompt CreatePrompt(string message, string title = "MESSAGE", UnityAction onPromptClose = null)
        {
           ModUI.ShowMessage(message);
           return null;
        }
        [System.Obsolete("=> ModUI.ShowYesNoMessage", true)]
        public static ModPrompt CreateYesNoPrompt(string message, string title, UnityAction onYes, UnityAction onNo = null, UnityAction onPromptClose = null)
        {
            ModUI.ShowYesNoMessage(message, title, null, onYes);
            return null;
        }
        [System.Obsolete("=> ModUI.ShowRetryCancelMessage", true)]
        public static ModPrompt CreateRetryCancelPrompt(string message, string title, UnityAction onRetry, UnityAction onCancel = null, UnityAction onPromptClose = null)
        {

            ModUI.ShowRetryCancelMessage(message, title, null, onRetry);
            return null;
        }
        [System.Obsolete("=> ModUI.ShowContinueAbortMessage", true)]
        public static ModPrompt CreateContinueAbortPrompt(string message, string title, UnityAction onContinue, UnityAction onAbort = null, UnityAction onPromptClose = null)
        {
            ModUI.ShowContinueAbortMessage(message, title, null, onContinue);
            return null;
        }

        [System.Obsolete("=> ModUI.ShowCustomMessage", true)]
        public static ModPrompt CreateCustomPrompt()
        {
            return null;
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
