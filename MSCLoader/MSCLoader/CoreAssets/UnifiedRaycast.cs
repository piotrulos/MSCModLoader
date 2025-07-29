#if !Mini
using HutongGames.PlayMaker;

namespace MSCLoader;

/// <summary>
/// Unified Raycast, use this to get interaction raycast results
/// </summary>
public class UnifiedRaycast : MonoBehaviour
{
    private const float rayLenght = 1.35f;
    private Camera mainCam;
    private RaycastHit hit, hitInteraction;
    private RaycastHit[] hits;
    private int interactionLayerMask;
    private static UnifiedRaycast instance;
    private FsmBool inMenu = false;
    private bool isHit, isHitInteraction = false;

    private readonly static string[] emptystring = [];
    private readonly static RaycastHit emptyHit = new();
    private readonly static RaycastHit[] emptyaHit = [];
    void Start()
    {
        mainCam = FsmVariables.GlobalVariables.FindFsmGameObject("POV").Value.GetComponent<Camera>();
        inMenu = FsmVariables.GlobalVariables.FindFsmBool("PlayerInMenu");
        interactionLayerMask = LayerMask.GetMask("Tools", "HingedObjects", "Dashboard");
        instance = this;
    }

    void Update()
    {
        if (mainCam == null) return;
        if (inMenu.Value) return;
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        hits = Physics.RaycastAll(ray, rayLenght);
        isHit = Physics.Raycast(ray, out hit, rayLenght);
        isHitInteraction = Physics.Raycast(ray, out hitInteraction, rayLenght, interactionLayerMask);
    }

    void OnDestroy()
    {
        instance = null;
    }

    /// <summary>
    /// Returns true if provided collider is hit as first element on all layers
    /// </summary>
    /// <param name="collider">Your collider</param>
    /// <returns>true if hit</returns>
    public static bool GetHit(Collider collider)
    {
        if (instance == null || instance.inMenu.Value || !instance.isHit) return false;
        return instance.hit.collider == collider;
    }

    /// <summary>
    /// Returns true if collider is hit only on "Tools", "HingedObjects", "Dashboard" layers 
    /// </summary>
    /// <param name="collider">Your collider</param>
    /// <returns>true if hit</returns>
    public static bool GetHitInteraction(Collider collider)
    {
        if (instance == null || instance.inMenu.Value || !instance.isHitInteraction) return false;
        return instance.hitInteraction.collider == collider;
    }

    /// <summary>
    /// Returns true if provided collider is hit (checks against all colliders that raycast hits)
    /// </summary>
    /// <param name="collider">Your collider</param>
    /// <returns>true if hit</returns>
    public static bool GetHitAll(Collider collider)
    {
        if (instance == null || instance.inMenu.Value || instance.hits.Length == 0) return false;
        for (int i = 0; i < instance.hits.Length; i++)
        {
            if (instance.hits[i].collider == collider) return true;
        }
        return false;
    }

    /// <summary>
    /// Returns name of the first raycast hit
    /// </summary>
    /// <returns>name of the first raycast hit</returns>
    public static string GetHitName()
    {
        if (instance == null || !instance.isHit || instance.inMenu.Value) return string.Empty;
        return instance.hit.collider.name;
    }

    /// <summary>
    /// Returns names of all raycast hits
    /// </summary>
    /// <returns>names of all raycast hits</returns>
    public static string[] GetHitNames()
    {
        if (instance == null || instance.inMenu.Value || instance.hits.Length == 0) return emptystring;
        string[] names = new string[instance.hits.Length];
        for (int i = 0; i < instance.hits.Length; i++)
        {
            names[i] = instance.hits[i].collider.name;
        }
        return names;
    }

    /// <summary>
    /// Returns RaycastHit so you can parse it yourself
    /// </summary>
    /// <returns>RaycastHit</returns>
    public static RaycastHit GetRaycastHit()
    {
        if (instance == null || instance.inMenu.Value) return emptyHit;
        return instance.hit;
    }
    /// <summary>
    /// Returns RaycastHit from interaction layers so you can parse it yourself
    /// </summary>
    /// <returns>RaycastHit</returns>
    public static RaycastHit GetRaycastHitInteraction()
    {
        if (instance == null || instance.inMenu.Value) return emptyHit;
        return instance.hitInteraction;
    }

    /// <summary>
    /// Returns RaycastHit[] so you can parse it yourself
    /// </summary>
    /// <returns>RaycastHit[]</returns>
    public static RaycastHit[] GetRaycastHits()
    {
        if (instance == null || instance.inMenu.Value) return emptyaHit;
        return instance.hits;
    }
}
#endif