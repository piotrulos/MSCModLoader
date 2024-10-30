namespace MSCLoader
{
    /// <summary>
    /// Class inheriting from MonoBehaviour, add this to any game object to make it interactable. Override the functions you need.
    /// </summary>
    public abstract class Interactable : MonoBehaviour
    {
        public Interactable()
        {
            if (InteractableHandler.initialized) return;
            Camera fpscam = GameObject.Find("PLAYER").transform.Find("Pivot/AnimPivot/Camera/FPSCamera/FPSCamera").GetComponent<Camera>();
            fpscam.gameObject.AddComponent<InteractableHandler>();
            InteractableHandler.initialized = true;
        }

        /// <summary>
        /// The 6 vanilla icons to display over the crosshair on aiming, also <b>none</b> if you dont want an icon to show
        /// </summary>
        public enum InteractionIcon
        {
            none = -1,
            /// <summary>Checkmark as seen when attaching parts</summary>
            assemble,
            /// <summary>Shopping cart for registers and when selecting items to buy</summary>
            buy,
            /// <summary>Circle with line as seen when detaching parts</summary>
            disassemble,
            /// <summary>Steering wheel for entering drive mode</summary>
            drive,
            /// <summary>Seat icon for entering passenger mode</summary>
            passenger,
            /// <summary>Hand icon used for most interactions</summary>
            use
        }
        /// <summary>Icon to show on aiming at the object, default <b>InteractionIcon.use</b> (Hand)</summary>
        public InteractionIcon Icon  = InteractionIcon.use;
        /// <summary>Text to show under the crosshair on aiming at the object</summary>
        public string InteractionText = string.Empty;

        /// <summary>Read-only property returning whether the player is aiming at the object or not. Also see <b>mouseEnter</b>, <b>mouseStay</b> and <b>mouseExit</b> methods.</summary>
        public bool MouseOver { get; internal set; }

        internal bool lmb;
        internal bool rmb;

        void DisplayInteraction(bool show)
        {
            if (Icon != InteractionIcon.none) MSCGUI.DisplayIcon(Icon, show);
            MSCGUI.InteractionText = show ? InteractionText : string.Empty;
        }

        /// <summary>
        /// Called once when the mouse/crosshair aims at the object
        /// </summary>
        public virtual void mouseEnter() { }


        // Additional method called by the interaction handler together with mouseEnter to make sure the virtual voids can be safely overriden and save the user from having to use base.method() every time
        internal void OnMouseEnter() => DisplayInteraction(true);

        /// <summary>
        /// Called each frame the mouse/crosshair stays at the object
        /// </summary>
        public virtual void mouseStay() { }

        // Additional method called by the interaction handler together with mouseExit to make sure the virtual voids can be safely overriden and save the user from having to use base.method() every time
        internal void OnMouseStay() => DisplayInteraction(true);

        /// <summary>
        /// Called once when the mouse/crosshair leaves the object
        /// </summary>
        public virtual void mouseExit() { }

        internal void OnMouseExit() => DisplayInteraction(false);

        /// <summary>
        /// Called once when aimed at the object and lmb pressed
        /// </summary>
        public virtual void lClick() { }

        /// <summary>
        /// Called each frame aimed at the object and lmb pressed
        /// </summary>
        public virtual void lHold() { }

        /// <summary>
        /// Called once when aimed at the object and lmb released or when lmb held and aiming away
        /// </summary>
        public virtual void lRelease() { }

        /// <summary>
        /// Called once when aimed at the object and rmb pressed
        /// </summary>
        public virtual void rClick() { }

        /// <summary>
        /// Called each frame aimed at the object and rmb pressed
        /// </summary>
        public virtual void rHold() { }

        /// <summary>
        /// Called once when aimed at the object and rmb released or when rmb held and aiming away
        /// </summary>
        public virtual void rRelease() { }

        /// <summary>
        /// Called once when aimed at the object and use key (f) pressed
        /// </summary>
        public virtual void use() { }

        /// <summary>
        /// Called once when aimed at the object and scrolling up one tick
        /// </summary>
        public virtual void scrollUp() { }

        /// <summary>
        /// Called once when aimed at the object and scrolling down one tick
        /// </summary>
        public virtual void scrollDown() { }
    }
}
