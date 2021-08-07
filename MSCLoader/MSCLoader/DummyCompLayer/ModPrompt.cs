using UnityEngine.Events;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace MSCLoader
{
    /// <summary>
    /// Dummy class
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]

    public class ModPromptButton
    {
    }
    /// <summary>
    /// ModPrompt redirects.
    /// </summary>
    
    
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class ModPrompt
    {
        public ModPromptButton AddButton(string buttonText, UnityAction action)
        {
            return null;
        }
        public static ModPrompt CreatePrompt(string message, string title = "MESSAGE", UnityAction onPromptClose = null)
        {
           ModUI.ShowMessage(message);
           return null;
        }
        public static ModPrompt CreateYesNoPrompt(string message, string title, UnityAction onYes, UnityAction onNo = null, UnityAction onPromptClose = null)
        {
            ModUI.ShowYesNoMessage(message, title, null, onYes);
            return null;
        }
        public static ModPrompt CreateRetryCancelPrompt(string message, string title, UnityAction onRetry, UnityAction onCancel = null, UnityAction onPromptClose = null)
        {

            ModUI.ShowRetryCancelMessage(message, title, null, onRetry);
            return null;
        }

        public static ModPrompt CreateContinueAbortPrompt(string message, string title, UnityAction onContinue, UnityAction onAbort = null, UnityAction onPromptClose = null)
        {
            ModUI.ShowContinueAbortMessage(message, title, null, onContinue);
            return null;
        }
  
        public static ModPrompt CreateCustomPrompt()
        {
            return null;
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
