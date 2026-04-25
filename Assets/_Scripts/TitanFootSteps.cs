using UnityEngine;
using Cinemachine;

public class TitanFootsteps : MonoBehaviour
{
    private CinemachineImpulseSource impulseSource;
    private PlayerScaleController scaleController;

    [SerializeField] private float impulseForce;
        
    void Start()
    {
        impulseSource = GetComponentInParent<CinemachineImpulseSource>();
        scaleController = GetComponentInParent<PlayerScaleController>();
    }
    
    public void OnFootstep()
    {
        if (scaleController != null && scaleController.currentSize == PlayerScaleController.PlayerSize.Titan)
        {
            if (impulseSource != null)
            {
                impulseSource.GenerateImpulseWithForce(impulseForce);
            }
        }
    }
}