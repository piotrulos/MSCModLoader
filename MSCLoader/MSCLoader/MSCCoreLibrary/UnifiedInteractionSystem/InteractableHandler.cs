#if !Mini
namespace MSCLoader
{
    internal class InteractableHandler : MonoBehaviour
    {
        internal static bool initialized = false;
        internal static Camera cam;

        static GameObject menu;

        static readonly int layerMask =
        ~((1 << LayerMask.NameToLayer("Player")) |
        (1 << LayerMask.NameToLayer("PlayerOnlyColl")) |
        (1 << LayerMask.NameToLayer("DontCollide")) |
        (1 << LayerMask.NameToLayer("Collider")));

        void Start()
        {
            menu = GameObject.Find("Systems").transform.Find("OptionsMenu").gameObject;
            cam = GameObject.Find("PLAYER").transform.Find("Pivot/AnimPivot/Camera/FPSCamera/FPSCamera").GetComponent<Camera>();
        }

        RaycastHit hit;
        Interactable last;
        Interactable obj;

        void Update()
        {
            if (menu.activeInHierarchy) return;

            Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 1.35f, layerMask);
            obj = hit.collider ? hit.collider.GetComponent<Interactable>() : null;


            if (obj)
            {
                if (cInput.GetKeyDown("Use")) obj.use();
#region MOUSE
                if (!obj.MouseOver)
                {
                    obj.MouseOver = true;
                    obj.mouseEnter();
                    obj.OnMouseEnter();
                }

                obj.mouseStay();
                obj.OnMouseStay();

                if (Input.GetMouseButtonDown(0))
                {
                    if (!obj.lmb) obj.lClick();
                    obj.lmb = true;
                }

                if (Input.GetMouseButtonDown(1))
                {
                    if (!obj.rmb) obj.rClick();
                    obj.rmb = true;
                }

                if (Input.GetMouseButtonUp(0) && obj.lmb)
                {
                    obj.lRelease();
                    obj.lmb = false;
                }

                if (Input.GetMouseButtonUp(1) && obj.rmb)
                {
                    obj.rRelease();
                    obj.rmb = false;
                }

                if (obj.lmb) obj.lHold();
                if (obj.rmb) obj.rHold();

                if (Input.mouseScrollDelta.y > 0)
                {
                    obj.scrollUp();
                }

                if (Input.mouseScrollDelta.y < 0)
                {
                    obj.scrollDown();
                }
#endregion MOUSE
            }

            if (obj != last && last != null)
            {
                if (last.lmb) last.lRelease();
                if (last.rmb) last.rRelease();
                last.lmb = false;
                last.rmb = false;

                last.mouseExit();
                last.OnMouseExit();
                last.MouseOver = false;

                obj = null;
            }

            last = obj;
        }
    }
}
#endif