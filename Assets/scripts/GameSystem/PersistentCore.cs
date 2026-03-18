using UnityEngine;

public class PersistentCore : MonoBehaviour
{
    public static PersistentCore Instance { get; private set; }

    private void Awake()
    {
        // If another CoreSystem already exists (e.g., we died and reloaded Level 1), destroy this duplicate
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        // Unparent this object just in case, as DontDestroyOnLoad ONLY works on root GameObjects
        transform.SetParent(null); 
        DontDestroyOnLoad(gameObject);
    }
}