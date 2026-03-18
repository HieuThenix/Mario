using UnityEngine;

public class MobileInputHub : MonoBehaviour
{
    public static MobileInputHub Instance { get; private set; }

    [Header("Assign your persistent Mobile Buttons here")]
    public MobileTouchButton leftBtn;
    public MobileTouchButton rightBtn;
    public MobileTouchButton jumpBtn;
    public MobileTouchButton crouchBtn;
    public MobileTouchButton fireBtn;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}