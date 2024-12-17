using HutongGames.PlayMaker;

namespace MSCLoader;

public class UnifiedRaycast : MonoBehaviour
{
    private float ray = 1.35f;
    private Camera mainCam;
    private RaycastHit hit;
    private RaycastHit[] hits;
    private static UnifiedRaycast instance;
    private FsmBool inMenu = false;
    private bool isHit = false;
    void Start()
    {
        mainCam = FsmVariables.GlobalVariables.FindFsmGameObject("POV").Value.GetComponent<Camera>();
        inMenu = FsmVariables.GlobalVariables.FindFsmBool("PlayerInMenu");
        instance = this;
    }

    void Update()
    {
        if (mainCam == null) return;
        if (inMenu.Value) return;

        hits = Physics.RaycastAll(mainCam.ScreenPointToRay(Input.mousePosition), ray);
        isHit = Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out hit, ray);
    }

    /// <summary>
    /// Returns true if provided collider is hit as first element
    /// </summary>
    /// <param name="collider">Your collider</param>
    /// <returns>true if hit</returns>
    public static bool GetHit(Collider collider)
    {
        if (!instance.isHit) return false;
        if (instance.inMenu.Value) return false;
        if (instance.hit.collider == null) return false;
        return instance.hit.collider == collider;
    }

    /// <summary>
    /// Returns true if provided collider is hit (checks against all colliders that raycast hits)
    /// </summary>
    /// <param name="collider">Your collider</param>
    /// <returns>true if hit</returns>
    public static bool GetHitAll(Collider collider)
    {
        if (instance.inMenu.Value) return false;
        if (instance.hits.Length == 0) return false;
        for (int i = 0; i < instance.hits.Length; i++)
        {
            if (instance.hits[i].collider == collider) return true;
        }
        return false;
    }

    /// <summary>
    /// Returns name of the first raycast  hit
    /// </summary>
    /// <returns>name of the first raycast hit</returns>
    public static string GetHitName()
    {
        if (!instance.isHit) return string.Empty;
        if (instance.inMenu.Value) return string.Empty;
        if (instance.hit.collider == null) return string.Empty;
        return instance.hit.collider.name;
    }

    /// <summary>
    /// Returns names of all raycast hits
    /// </summary>
    /// <returns>names of all raycast hits</returns>
    public static string[] GetHitNames()
    {
        if (instance.inMenu.Value) return new string[0];
        if (instance.hits.Length == 0) return new string[0];
        string[] names = new string[instance.hits.Length];
        for (int i = 0; i < instance.hits.Length; i++)
        {
            names[i] = instance.hits[i].collider.name;
        }
        return names;
    }
}
